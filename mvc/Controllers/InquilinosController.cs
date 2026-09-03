using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;

namespace mvc.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorio;

        public InquilinosController(IRepositorioInquilino repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Inquilinos
        public IActionResult Index()
        {
            var lista = repositorio.ObtenerLista(1, 100);
            return View(lista);
        }

        public IActionResult Detalles(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // GET: Inquilinos/Editar/5
        public IActionResult Editar(int id)
        {
            if (id > 0)
            {
                var inquilino = repositorio.ObtenerPorId(id);
                if (inquilino == null)
                    return NotFound();
                return View(inquilino);
            }
            return View(new Inquilino());
        }

        // POST: Inquilinos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                if (inquilino.IdInquilino > 0)
                    repositorio.Modificacion(inquilino);
                else
                    repositorio.Alta(inquilino);

                return RedirectToAction(nameof(Index));
            }
            return View("Editar", inquilino);
        }

        // GET: Inquilinos/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
                return NotFound();

            return View(inquilino);
        }

        // POST: Inquilinos/Borrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrar(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}