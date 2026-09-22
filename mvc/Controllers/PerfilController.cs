using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Models;
using System.Security.Claims;

namespace mvc.Controllers
{
    [Authorize] 
    public class PerfilController : Controller
    {
        private readonly IRepositorioUsuario repositorio; 

        public PerfilController(IRepositorioUsuario repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: /Perfil o /Perfil/Index
        public IActionResult Index()
        {

            var emailUsuario = User.FindFirst(ClaimTypes.Email)?.Value; 
            
            var usuario = repositorio.ObtenerPorEmail(emailUsuario); 

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
    }
}