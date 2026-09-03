using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;

namespace mvc.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorio;

        public PropietariosController(IRepositorioPropietario repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Propietarios
        public IActionResult Index(string? nombre)
        {
            IList<Propietario> lista;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                lista = repositorio.BuscarPorNombre(nombre);
            }
            else
            {
                lista = repositorio.ObtenerLista();
            }

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
        public IActionResult Editar(int id)
        {
            if (id > 0)
            {
                var propietario = repositorio.ObtenerPorId(id);
                if (propietario == null)
                {
                    return NotFound();
                }
                return View(propietario);
            }

            return View(new Propietario());
        }

        // POST: Propietarios/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                if (propietario.IdPropietario > 0)
                {
                    repositorio.Modificacion(propietario);
                }
                else
                {
                    repositorio.Alta(propietario);
                }

                return RedirectToAction(nameof(Index));
            }

            return View("Editar", propietario);
        }

        // GET: Propietarios/Eliminar/5
        public IActionResult Eliminar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }

            return View(propietario);
        }

        // POST: Propietarios/Borrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Borrar(int id)
        {
            repositorio.Baja(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Propietarios/BuscarEmail?email=test@test.com
        [HttpGet]
        public IActionResult BuscarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("El email no puede estar vacío.");
            }

            var propietario = repositorio.ObtenerPorEmail(email);
            if (propietario == null)
            {
                return NotFound();
            }

            return Json(propietario);
        }
    }
}