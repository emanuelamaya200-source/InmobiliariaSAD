using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
        public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;

        public TipoInmuebleController(IRepositorioTipoInmueble repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Inquilinos
        public IActionResult Index(int pagina = 1)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);
            var lista = repositorio.ObtenerLista(pagina, tamPagina);
            var total = repositorio.ObtenerCantidad();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (total + tamPagina - 1) / tamPagina;
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
          // GET: Propietarios/Editar/5 
          [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            if (id > 0)
            {
                var tipo = repositorio.ObtenerPorId(id);
                if (tipo == null)
                {
                    return NotFound();
                }
                return View(tipo);
            }

            return View(new tipoInmueble());
        }

        // POST: Propietarios/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Guardar(tipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                if (tipoInmueble.idTipoInmueble > 0)
                {
                    repositorio.Modificacion(tipoInmueble);
                }
                else
                {
                    repositorio.Alta(tipoInmueble);
                }

                return RedirectToAction(nameof(Index));
            }

            return View("Editar", tipoInmueble);
        }

        // GET: Propietarios/Eliminar/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        // POST: Propietarios/Borrar/5
        
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