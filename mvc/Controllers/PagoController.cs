using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;
    
        public PagoController(IRepositorioPago repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Pagos
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerLista(1, 100);
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
            if (id > 0)
            {
                var pago = repositorio.ObtenerPorId(id);
                if (pago == null)
                    return NotFound();
                return View(pago);
            }
            return View(new Pago());
        }

        // POST: Pagos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(Pago pago)
        {
            if (ModelState.IsValid)
            {
                if (pago.IdPago > 0)
                    repositorio.Modificacion(pago);
                else
                    repositorio.Alta(pago);

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