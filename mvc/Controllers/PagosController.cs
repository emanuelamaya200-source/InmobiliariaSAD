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

        public PagosController(IRepositorioPago repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Pagos
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            if (id <= 0)
                return BadRequest();

            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            return View("Editar", pago);
        }

        // POST: Pagos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Guardar(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.UsuarioCreacionId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) ? usuarioId : null;
                if (pago.IdPago <= 0) repositorio.Alta(pago);
                else repositorio.Modificacion(pago);

                return RedirectToAction(nameof(Index));
            }
            return View("Editar", pago);
        }

        // GET: Pagos/Eliminar/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            return View("Baja", pago);
        }

        // GET: Pagos/Pagar/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Pagar(int idReserva)
        {
            if (idReserva <= 0)
                return BadRequest();

            var pago = repositorio.ObtenerPorReserva(idReserva);
            if (pago == null)
                return NotFound();

            return View("Editar", pago);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Nuevo(int idReserva)
        {
            if (idReserva <= 0) return BadRequest();
            return View("Editar", new Pago { IdReserva = idReserva, Estado = "Activo", Fecha = DateOnly.FromDateTime(DateTime.Today) });
        }

        // POST: Pagos/Borrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Pagos/Alta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Alta(int id)
        {
            repositorio.Reactivar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}