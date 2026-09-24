using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class TipoInmuebleController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;
        private readonly IRepositorioAuditoria auditoriaRepositorio;

        public TipoInmuebleController(IRepositorioTipoInmueble repositorio, IRepositorioAuditoria auditoriaRepositorio)
        {
            this.repositorio = repositorio;
            this.auditoriaRepositorio = auditoriaRepositorio;
        }

        // GET: TipoInmueble
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


        // GET: TipoInmueble/Crear
        public IActionResult Crear()
        {
            return View(new tipoInmueble());
        }

        // POST: TipoInmueble/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(tipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(tipoInmueble);
                auditoriaRepositorio.Registrar(User, "TipoInmueble", tipoInmueble.idTipoInmueble, "Alta", tipoInmueble.Descripcion);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }



        // GET: TipoInmueble/Editar/5 
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var tipo = repositorio.ObtenerPorId(id);
            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        // POST: TipoInmueble/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id, tipoInmueble tipoInmueble)
        {
            if (id != tipoInmueble.idTipoInmueble)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(tipoInmueble);
                auditoriaRepositorio.Registrar(User, "TipoInmueble", tipoInmueble.idTipoInmueble, "Modificacion", tipoInmueble.Descripcion);
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }


        // GET: TipoInmueble/Eliminar/5
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

        // POST: TipoInmueble/Borrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(int id)
        {
            try
            {
                repositorio.Baja(id);
                auditoriaRepositorio.Registrar(User, "TipoInmueble", id, "Baja", "Tipo de inmueble eliminado");
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                TempData["Error"] = "No se puede eliminar este tipo porque tiene inmuebles o reservas relacionadas.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}