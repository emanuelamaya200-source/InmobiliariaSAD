using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class ReservasController : Controller
    {
        private readonly IRepositorioReserva repositorio;
        private readonly IRepositorioPago repoPago;
        private readonly IRepositorioInquilino repoInquilino;
        private readonly IRepositorioInmueble repoInmueble;
        private readonly IRepositorioAuditoria auditoriaRepositorio;

        public ReservasController(IRepositorioReserva repositorio, IRepositorioPago repoPago, IRepositorioInquilino repoInquilino, IRepositorioInmueble repoInmueble, IRepositorioAuditoria auditoriaRepositorio)
        {
            this.repositorio = repositorio;
            this.repoPago = repoPago;
            this.repoInquilino = repoInquilino;
            this.repoInmueble = repoInmueble;
            this.auditoriaRepositorio = auditoriaRepositorio;
        }

        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Crear()
        {
            ViewBag.Inquilinos = repoInquilino.ObtenerLista(1, int.MaxValue);
            ViewBag.Inmuebles = repoInmueble.ObtenerLista(1, int.MaxValue);
            return View(new Reserva());
        }

        // POST: Reservas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Crear(Reserva reserva)
        {
            try
            {
                if (reserva.FechaDeSalida <= reserva.FechaDeEntrada)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "La fecha de salida debe ser posterior a la fecha de entrada."
                    );
                }

                var inmueble = repoInmueble.ObtenerPorId(reserva.IdInmueble);

                if (inmueble == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "No se encontró el inmueble seleccionado."
                    );
                }
                else
                {
                    // Validar solapamiento de fechas directo en la base de datos
                    bool haySolapamiento = repositorio.ExisteSolapamiento(reserva.IdInmueble, reserva.FechaDeEntrada, reserva.FechaDeSalida);
                    if (haySolapamiento)
                    {
                        ModelState.AddModelError(string.Empty, "El inmueble no está disponible en las fechas elegidas.");
                    }
                }

                // Evitar bloqueos de ModelState con campos calculados
                ModelState.Remove(nameof(reserva.MontoTotal));
                ModelState.Remove(nameof(reserva.MontoDiario));
                ModelState.Remove(nameof(reserva.Estado));

                if (ModelState.IsValid)
                {
                    reserva.UsuarioCreacionId = UsuarioActualId();
                    reserva.MontoDiario = inmueble!.PrecioPorDia;

                    int cantidadDias = Math.Max(
                        1,
                        (reserva.FechaDeSalida.Date - reserva.FechaDeEntrada.Date).Days
                    );

                    reserva.MontoTotal = (int)(
                        cantidadDias * inmueble.PrecioPorDia
                    );
                    reserva.Estado = "Activo";

                    repositorio.Alta(reserva);

                    decimal montoSeniaMinima =
                        reserva.MontoTotal *
                        (inmueble.PorcentajeReserva / 100m);

                    var pago = new Pago
                    {
                        IdReserva = reserva.IdReserva,
                        Monto = montoSeniaMinima,
                        Concepto = $"Seña inicial ({inmueble.PorcentajeReserva}%)",
                        Estado = "Activo",
                        Fecha = DateOnly.FromDateTime(DateTime.Today),
                        UsuarioCreacionId = UsuarioActualId()
                    };

                    repoPago.Alta(pago);

                    auditoriaRepositorio.Registrar(
                        User,
                        "Reserva",
                        reserva.IdReserva,
                        "Alta",
                        $"Inmueble {reserva.IdInmueble}"
                    );

                    TempData["Mensaje"] = "Reserva creada correctamente";

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Error al crear la reserva: " + ex.Message
                );
            }

            ViewBag.Inquilinos = repoInquilino.ObtenerLista(1, int.MaxValue);
            ViewBag.Inmuebles = repoInmueble.ObtenerLista(1, int.MaxValue);

            return View(reserva);
        }

        // GET: Reservas
        public IActionResult Index(int? id, DateTime? inicio, DateTime? fin, int? cupo, int pagina = 1)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);
            IList<Reserva> resultados;

            if (id.HasValue && id.Value > 0)
            {
                var r = repositorio.ObtenerPorId(id.Value);
                resultados = r is null ? new List<Reserva>() : new List<Reserva> { r };
            }
            else
            {
                resultados = repositorio.ObtenerPorRango(inicio, fin, cupo);
            }

            var total = resultados.Count;
            var lista = resultados.Skip((pagina - 1) * tamPagina).Take(tamPagina).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (total + tamPagina - 1) / tamPagina;
            ViewBag.Inicio = inicio?.ToString("yyyy-MM-dd");
            ViewBag.Fin = fin?.ToString("yyyy-MM-dd");
            ViewBag.Cupo = cupo;

            return View(lista);
        }

        // GET: Reservas/Detalles/5
        public IActionResult Detalles(int id)
        {
            var reserva = repositorio.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewBag.Pagos = repoPago.ObtenerListaPorReserva(id);
            return View(reserva);
        }

        // GET: Reservas/Editar/5 
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            ViewBag.Inquilinos = repoInquilino.ObtenerLista();
            ViewBag.Inmuebles = repoInmueble.ObtenerLista();

            if (id > 0)
            {
                var reserva = repositorio.ObtenerPorId(id);
                if (reserva == null)
                {
                    return NotFound();
                }
                return View(reserva);
            }

            return View(new Reserva());
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Renovar(int id)
        {
            var original = repositorio.ObtenerPorId(id);
            if (original == null) return NotFound();
            var entrada = original.FechaDeSalida;
            original.IdReserva = 0;
            original.FechaDeEntrada = entrada;
            original.FechaDeSalida = entrada.AddDays(1);
            ViewBag.Inquilinos = repoInquilino.ObtenerLista();
            ViewBag.Inmuebles = repoInmueble.ObtenerLista();
            return View("Editar", original);
        }

        // POST: Reservas/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Guardar(Reserva reserva)
        {
            if (reserva.FechaDeSalida <= reserva.FechaDeEntrada)
            {
                ModelState.AddModelError(string.Empty, "La fecha de salida debe ser posterior a la fecha de entrada.");
            }

            // Validar solapamiento usando el método 
            int? idExcluir = reserva.IdReserva > 0 ? reserva.IdReserva : null;
            bool haySolapamiento = repositorio.ExisteSolapamiento(reserva.IdInmueble, reserva.FechaDeEntrada, reserva.FechaDeSalida, idExcluir);

            if (haySolapamiento)
            {
                ModelState.AddModelError(string.Empty, "El inmueble no está disponible en las fechas elegidas.");
            }

            ModelState.Remove(nameof(reserva.MontoTotal));
            ModelState.Remove(nameof(reserva.MontoDiario));

            if (!ModelState.IsValid)
            {
                ViewBag.Inquilinos = repoInquilino.ObtenerLista();
                ViewBag.Inmuebles = repoInmueble.ObtenerLista();
                return View("Editar", reserva);
            }

            try
            {
                reserva.UsuarioCreacionId = UsuarioActualId();
                var inmueble = repoInmueble.ObtenerPorId(reserva.IdInmueble);

                if (inmueble == null)
                    throw new InvalidOperationException("No se encontró el inmueble de la reserva");

                var cantidadDias = Math.Max(1, (reserva.FechaDeSalida.Date - reserva.FechaDeEntrada.Date).Days);
                reserva.MontoDiario = inmueble.PrecioPorDia;
                reserva.MontoTotal = (int)(cantidadDias * inmueble.PrecioPorDia);

                if (reserva.IdReserva > 0)
                {
                    repositorio.Modificacion(reserva);
                    auditoriaRepositorio.Registrar(User, "Reserva", reserva.IdReserva, "Modificacion", $"Inmueble {reserva.IdInmueble}");
                }
                else
                {
                    repositorio.Alta(reserva);

                    decimal montoSeniaMinima = reserva.MontoTotal * (inmueble.PorcentajeReserva / 100m);

                    repoPago.Alta(new Pago
                    {
                        IdReserva = reserva.IdReserva,
                        Monto = montoSeniaMinima,
                        Concepto = $"Seña inicial ({inmueble.PorcentajeReserva}%)",
                        Estado = "Activo",
                        Fecha = DateOnly.FromDateTime(DateTime.Today),
                        UsuarioCreacionId = UsuarioActualId()
                    });
                    auditoriaRepositorio.Registrar(User, "Reserva", reserva.IdReserva, "Alta", $"Inmueble {reserva.IdInmueble}");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar: " + ex.Message);
                ViewBag.Inquilinos = repoInquilino.ObtenerLista();
                ViewBag.Inmuebles = repoInmueble.ObtenerLista();
                return View("Editar", reserva);
            }
        }

        // GET: Reservas/Eliminar/5 
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var reserva = repositorio.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // POST: Reservas/Eliminar
        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Cancelar(id, UsuarioActualId());
                auditoriaRepositorio.Registrar(User, "Reserva", id, "Baja", "Reserva cancelada");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar la reserva: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reservas/Finalizar/5
        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Finalizar(int id)
        {
            var reserva = repositorio.ObtenerPorId(id);

            if (reserva == null)
                return NotFound();

            if (reserva.Estado != "Activo")
                return BadRequest("La reserva no está activa.");

            return View(reserva);
        }

        // GET: Reservas/VerificarDisponibilidad
        [HttpGet]
        public IActionResult VerificarDisponibilidad(DateTime inicioFecha, DateTime finFecha, int cupo = 0)
        {
            var disponibles = repositorio.VerificarDisponibilidad(inicioFecha, finFecha, cupo);
            return Json(disponibles);
        }

        private int UsuarioActualId()
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        }

        // GET: Reservas/CalcularMulta
        [HttpGet]
        public IActionResult CalcularMulta(int idReserva, DateTime finFecha)
        {
            try
            {
                var multa = repositorio.CalcularMulta(idReserva, finFecha);
                return Json(new { Multa = multa });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: Reservas/TerminarReservaAnticipada
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult TerminarReservaAnticipada(int idReserva, DateTime nuevoFinFecha)
        {
            try
            {
                var reserva = repositorio.ObtenerPorId(idReserva);

                if (reserva == null)
                    return NotFound();

                if (reserva.Estado != "Activo")
                    return BadRequest("La reserva no está activa.");

                if (nuevoFinFecha <= reserva.FechaDeEntrada)
                    return BadRequest("La fecha de finalización debe ser posterior a la fecha de entrada.");

                if (nuevoFinFecha >= reserva.FechaDeSalida)
                    return BadRequest("La fecha seleccionada debe ser anterior a la fecha de salida original.");

                int usuarioId = UsuarioActualId();

                if (usuarioId <= 0)
                    return BadRequest("No se pudo identificar al usuario actual.");

                decimal multa = repositorio.CalcularMulta(idReserva, nuevoFinFecha);

                bool finalizada = repositorio.TerminarReservaAnticipada(
                    idReserva,
                    nuevoFinFecha,
                    usuarioId
                );

                if (!finalizada)
                    return BadRequest("No se pudo finalizar la reserva.");

                if (multa > 0)
                {
                    var pago = new Pago
                    {
                        IdReserva = idReserva,
                        Monto = multa,
                        Concepto = "Multa por finalización anticipada",
                        Estado = "Activo",
                        Fecha = DateOnly.FromDateTime(DateTime.Today),
                        UsuarioCreacionId = usuarioId
                    };

                    repoPago.Alta(pago);

                    auditoriaRepositorio.Registrar(
                        User,
                        "Pago",
                        pago.IdPago,
                        "Alta",
                        $"Multa por finalización anticipada de reserva {idReserva}: {multa:C}"
                    );
                }

                auditoriaRepositorio.Registrar(
                    User,
                    "Reserva",
                    idReserva,
                    "Modificacion",
                    $"Finalizada anticipadamente el {nuevoFinFecha:dd/MM/yyyy}. Multa: {multa:C}"
                );

                TempData["Mensaje"] = $"Reserva finalizada correctamente. Multa: {multa:C}";

                return RedirectToAction(nameof(Detalles), new { id = idReserva });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Reservas/RenovarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult RenovarReserva(int idReserva, DateTime nuevoFinFecha, decimal nuevoPrecio)
        {
            try
            {
                var reserva = repositorio.RenovarReserva(idReserva, nuevoFinFecha, nuevoPrecio);
                if (reserva == null)
                {
                    return BadRequest("No se pudo renovar la reserva.");
                }

                auditoriaRepositorio.Registrar(User, "Reserva", idReserva, "Alta", $"Renovada hasta {nuevoFinFecha:dd/MM/yyyy}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}