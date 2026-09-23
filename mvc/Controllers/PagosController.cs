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

        // GET: Pagos/Editar/5 (SOLO Administrador)
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

        // GET: Pagos/Crear (Permitido para Administradores y Empleados)
        public IActionResult Crear(int idReserva)
        {
            if (idReserva <= 0) return BadRequest();
            return View("Crear", new Pago { IdReserva = idReserva, Estado = "Activo", Fecha = DateOnly.FromDateTime(DateTime.Today) });
        }

        // POST: Pagos/GuardarCrear (Para nuevos pagos creados por Empleados o Admins)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarCrear(Pago pago)
        {
            if (ModelState.IsValid)
            {
                pago.UsuarioCreacionId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId) ? usuarioId : null;
                pago.Estado = "Activo";
                repositorio.Alta(pago);
                return RedirectToAction(nameof(Index));
            }
            return View("Crear", pago);
        }

        // POST: Pagos/Guardar (SOLO Administrador para modificaciones)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Guardar(Pago pago)
        {
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(pago);
                return RedirectToAction(nameof(Index));
            }
            return View("Editar", pago);
        }

        // GET: Pagos/Eliminar/5 (SOLO Administrador)
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
                return NotFound();

            return View("Baja", pago);
        }

        // POST: Pagos/Borrar/5 (Baja lógica - SOLO Administrador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Pagos/Alta/5 (Reactivación - SOLO Administrador)
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