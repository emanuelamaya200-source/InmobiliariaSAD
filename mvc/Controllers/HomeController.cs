using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Inmobiliaria_.Net_Core.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Inmobiliaria_.Net_Core.Controllers;


public class HomeController : Controller
{
    private readonly IRepositorioUsuario repositorio;

    public HomeController(IRepositorioUsuario repositorio)
    {
            this.repositorio = repositorio;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpGet]
    [AllowAnonymous]
    [Route("Login")]
    [Route("Home/Login")]
    public IActionResult Login(string returnUrl = null)
    {

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login modelo)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var usuario = repositorio.ObtenerPorEmail(modelo.Email);

        if (usuario == null)
        {
            ViewBag.Error = "El correo es incorrecto";
            return View(modelo);
        }

        if (usuario.Clave != modelo.Clave)
        {
            ViewBag.Error = "La clave es incorrecta";
            return View(modelo);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol == 1 ? "Administrador" : "Empleado")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));


        if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
        {
            return Redirect(modelo.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");

        
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
