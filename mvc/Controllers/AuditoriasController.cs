using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mvc.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AuditoriasController : Controller
    {
        private readonly IRepositorioAuditoria repositorio;

        public AuditoriasController(IRepositorioAuditoria repositorio)
        {
            this.repositorio = repositorio;
        }

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
    }
}
