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
        [HttpGet]

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


        // GET: Propietarios/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Propietario());
        }

        // POST: Propietarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(propietario);
                auditoriaRepositorio.Registrar(User, "Propietario", propietario.IdPropietario, "Alta", $"Email: {propietario.Email}");
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }



        // GET: Propietarios/Editar/5 
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }

        // POST: Propietarios/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id, Propietario propietario)
        {
            if (id != propietario.IdPropietario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(propietario);
                auditoriaRepositorio.Registrar(User, "Propietario", propietario.IdPropietario, "Modificacion", $"Email: {propietario.Email}");
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }



        // GET: Propietarios/Eliminar/5
        [HttpGet]
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