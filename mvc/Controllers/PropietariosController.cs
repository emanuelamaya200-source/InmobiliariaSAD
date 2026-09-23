using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorio;
        private readonly IRepositorioAuditoria auditoriaRepositorio;

        public PropietariosController(IRepositorioPropietario repositorio, IRepositorioAuditoria auditoriaRepositorio)
        {
            this.repositorio = repositorio;
            this.auditoriaRepositorio = auditoriaRepositorio;
        }

        // GET: Propietarios
        public IActionResult Index(string? nombre, int pagina = 1)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);
            IList<Propietario> resultados;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                resultados = repositorio.BuscarPorNombre(nombre);
            }
            else
            {
                resultados = repositorio.ObtenerLista(1, int.MaxValue);
            }

            var total = resultados.Count;
            var lista = resultados.Skip((pagina - 1) * tamPagina).Take(tamPagina).ToList();
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (total + tamPagina - 1) / tamPagina;
            ViewBag.Nombre = nombre;
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Guardar(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                if (propietario.IdPropietario > 0)
                {
                    repositorio.Modificacion(propietario);
                    auditoriaRepositorio.Registrar(User, "Propietario", propietario.IdPropietario, "Modificacion", $"Email: {propietario.Email}");
                }
                else
                {
                    repositorio.Alta(propietario);
                    auditoriaRepositorio.Registrar(User, "Propietario", propietario.IdPropietario, "Alta", $"Email: {propietario.Email}");
                }

                return RedirectToAction(nameof(Index));
            }

            return View("Editar", propietario);
        }

        // GET: Propietarios/Eliminar/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(int id)
        {
            try
            {
                repositorio.Baja(id);
                auditoriaRepositorio.Registrar(User, "Propietario", id, "Baja", "Propietario eliminado");
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                TempData["Error"] = "No se puede eliminar este propietario porque tiene inmuebles relacionados.";
            }
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