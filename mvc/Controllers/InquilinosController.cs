using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class InquilinosController : Controller
    {
        private readonly IRepositorioInquilino repositorio;
        private readonly IRepositorioAuditoria auditoriaRepositorio;

        public InquilinosController(IRepositorioInquilino repositorio, IRepositorioAuditoria auditoriaRepositorio)
        {
            this.repositorio = repositorio;
            this.auditoriaRepositorio = auditoriaRepositorio;
        }

        // GET: Inquilinos
        
        public IActionResult Index(string? nombre, int pagina = 1)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);
            IList<Inquilino> resultados;

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

        // GET: Inquilinos/Crear
        public IActionResult Crear()
        {
            return View(new Inquilino());
        }

        // POST: Inquilinos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                repositorio.Alta(inquilino);
                auditoriaRepositorio.Registrar(User, "Inquilino", inquilino.IdInquilino, "Alta", $"Email: {inquilino.Email}");
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }


        // GET: Inquilinos/Editar/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            if (inquilino == null)
                return NotFound();
            return View(inquilino);
        }

        // POST: Inquilinos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Editar(int id, Inquilino inquilino)
        {
            if (id != inquilino.IdInquilino)
                return NotFound();

            if (ModelState.IsValid)
            {
                repositorio.Modificacion(inquilino);
                auditoriaRepositorio.Registrar(User, "Inquilino", inquilino.IdInquilino, "Modificacion", $"Email: {inquilino.Email}");
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }


        // GET: Inquilinos/Eliminar/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(int id)
        {
            repositorio.Baja(id);
            auditoriaRepositorio.Registrar(User, "Inquilino", id, "Baja", "Inquilino eliminado");
            return RedirectToAction(nameof(Index));
        }
    }
}