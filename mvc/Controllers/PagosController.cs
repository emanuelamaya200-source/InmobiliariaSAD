using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly IRepositorioAuditoria auditoriaRepositorio;
        private readonly IRepositorioReserva repoReserva;
        private readonly IRepositorioInmueble repoInmueble;

        public PagosController(
            IRepositorioPago repositorio,
            IRepositorioAuditoria auditoriaRepositorio,
            IRepositorioReserva repoReserva,
            IRepositorioInmueble repoInmueble)
        {
            this.repositorio = repositorio;
            this.auditoriaRepositorio = auditoriaRepositorio;
            this.repoReserva = repoReserva;
            this.repoInmueble = repoInmueble;
        }

        // GET: Pagos
        [HttpGet]
        public IActionResult Index(int pagina = 1)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);
            var lista = repositorio.ObtenerLista(pagina, tamPagina, true);
            var total = repositorio.ObtenerCantidad(true);
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (total + tamPagina - 1) / tamPagina;
            return View(lista);
        }

        public IActionResult Detalles(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View("Editar", pago);
        }

        // GET: Pagos/Editar/5
        [HttpGet]
        [Authorize(Roles = "Administrador, Empleado")]
        public IActionResult Editar(int id)
        {
            if (id <= 0)
                return BadRequest();

            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            return View("Editar", pago);
        }

        // GET: Pagos/Crear 
        [HttpGet]
        public IActionResult Crear(int idReserva)
        {
            if (idReserva <= 0) return BadRequest();

            var reserva = repoReserva.ObtenerPorId(idReserva);
            if (reserva == null) return NotFound();

            var inmueble = repoInmueble.ObtenerPorId(reserva.IdInmueble);

            decimal montoTotal = reserva.MontoTotal > 0 ? reserva.MontoTotal : 0;
            if (montoTotal == 0 && inmueble != null)
            {
                int dias = Math.Max(1, (reserva.FechaDeSalida.Date - reserva.FechaDeEntrada.Date).Days);
                montoTotal = dias * inmueble.PrecioPorDia;
            }


            decimal porcentajeSeña = inmueble?.PorcentajeReserva ?? 0;
            decimal montoSeñaMinima = montoTotal * (porcentajeSeña / 100m);

            ViewBag.MontoTotalReserva = montoTotal;
            ViewBag.MontoSeniaMinima = montoSeñaMinima;
            ViewBag.PorcentajeSenia = porcentajeSeña;

            return View("Crear", new Pago
            {
                IdReserva = idReserva,
                Monto = montoSeñaMinima,
                Concepto = $"Seña de reserva ({porcentajeSeña}%)",
                Estado = "Activo",
                Fecha = DateOnly.FromDateTime(DateTime.Today)
            });
        }

        // POST: Pagos/GuardarCrear 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarCrear(Pago pago)
        {
            var reserva = repoReserva.ObtenerPorId(pago.IdReserva);
            decimal montoTotal = reserva?.MontoTotal ?? 0;
            decimal porcentajeSeña = 0;
            decimal minSeña = 0;

            if (reserva != null)
            {
                var inmueble = repoInmueble.ObtenerPorId(reserva.IdInmueble);

                if (montoTotal == 0 && inmueble != null)
                {
                    int dias = Math.Max(1, (reserva.FechaDeSalida.Date - reserva.FechaDeEntrada.Date).Days);
                    montoTotal = dias * inmueble.PrecioPorDia;
                }

                porcentajeSeña = inmueble?.PorcentajeReserva ?? 0;
                minSeña = montoTotal * (porcentajeSeña / 100m);

                if (pago.Monto < minSeña)
                {
                    ModelState.AddModelError("Monto", $"El monto a pagar no puede ser menor a la seña mínima requerida ({minSeña.ToString("C")}).");
                }
                if (pago.Monto > montoTotal)
                {
                    ModelState.AddModelError("Monto", $"El monto no puede superar el total de la reserva ({montoTotal.ToString("C")}).");
                }
            }

            if (ModelState.IsValid)
            {
                pago.UsuarioCreacionId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) ? usuarioId : null;
                pago.Estado = "Activo";
                repositorio.Alta(pago);
                auditoriaRepositorio.Registrar(User, "Pago", pago.IdPago, "Alta", $"Reserva {pago.IdReserva}: {pago.Concepto} - Monto: {pago.Monto}");
                return RedirectToAction(nameof(Index));
            }


            ViewBag.MontoTotalReserva = montoTotal;
            ViewBag.MontoSeniaMinima = minSeña;
            ViewBag.PorcentajeSenia = porcentajeSeña;

            return View("Crear", pago);
        }

        // POST: Pagos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Guardar(Pago pago)
        {
            var pagoOriginal = repositorio.ObtenerPorId(pago.IdPago);

            if (pagoOriginal == null)
                return NotFound();

            if (User.IsInRole("Empleado"))
            {
                pagoOriginal.Concepto = pago.Concepto;

                repositorio.Modificacion(pagoOriginal);

                auditoriaRepositorio.Registrar(
                    User,
                    "Pago",
                    pagoOriginal.IdPago,
                    "Modificacion",
                    $"Concepto del pago modificado: {pagoOriginal.Concepto}"
                );

                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(pago);

                auditoriaRepositorio.Registrar(
                    User,
                    "Pago",
                    pago.IdPago,
                    "Modificacion",
                    $"Reserva {pago.IdReserva}: {pago.Concepto}"
                );

                return RedirectToAction(nameof(Index));
            }

            return View("Editar", pago);
        }
        // GET: Pagos/Eliminar/5
        [HttpGet] 
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            return View("Baja", pago);
        }

        // POST: Pagos/Borrar/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(int id)
        {
            repositorio.Baja(id);
            auditoriaRepositorio.Registrar(User, "Pago", id, "Baja", "Pago dado de baja");
            return RedirectToAction(nameof(Index));
        }

        // POST: Pagos/Alta/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Alta(int id)
        {
            repositorio.Reactivar(id);
            auditoriaRepositorio.Registrar(User, "Pago", id, "Alta", "Pago reactivado");
            return RedirectToAction(nameof(Index));
        }
    }
}