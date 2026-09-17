using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorio;
    
        public PagosController(IRepositorioPago repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Pagos
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerLista(1, 100);
            Console.WriteLine(lista);
            return View(lista);
        }

        public IActionResult Detalles(int id)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
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

            return View(pago);
        }

        // POST: Pagos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Guardar(Pago pago)
        {
            if (pago.IdPago <= 0)
                return BadRequest("Los pagos se crean al registrar una reserva.");

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(pago);

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

            return View(pago);
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
    }
}