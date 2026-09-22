using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;

        private readonly IRepositorioTipoInmueble repoTipoInmueble;

        public InmueblesController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietrio, IRepositorioTipoInmueble repoTipoInmueble)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietrio;
            this.repoTipoInmueble = repoTipoInmueble;
        }

        // GET: Inmuebles
        public ActionResult Index(int pagina = 1, string? nombre = null)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);
            IList<Inmueble> resultados;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                resultados = repositorio.BuscarPorTipo(nombre);
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

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

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

        // GET: Inmuebles/Editar/5 (o Inmuebles/Editar para Alta)
        [Authorize(Roles = "Administrador")]
        public ActionResult Editar(int id)
        {
            ViewBag.Propietarios = repoPropietario.ObtenerLista(1, 100);
            ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();

            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            if (id > 0)
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                return View(entidad);
            }

            return View(new Inmueble());
        }

        // GET: Inmuebles/BuscarPorPropietario/5
        [HttpGet]
        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.BuscarPorPropietario(id);
            return Ok(lista);
        }

        // GET: Inmuebles/Details/5
        public ActionResult Ver(int id)
        {
            var entidad = id == 0 ? new Inmueble() : repositorio.ObtenerPorId(id);
            return View(entidad);
        }

        // POST: Inmuebles/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Guardar(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (entidad.Id == 0)
                    {
                        repositorio.Alta(entidad);
                        TempData["Id"] = entidad.Id;
                        TempData["Mensaje"] = "Inmueble creado correctamente";
                    }
                    else
                    {
                        repositorio.Modificacion(entidad);
                        TempData["Mensaje"] = "Inmueble modificado correctamente";
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerLista(1, 100);
                    ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();
                    return View("Editar", entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerLista(1, 100);
                ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View("Editar", entidad);
            }
        }

        // POST: Inmuebles/GuardarAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult GuardarAjax(int id, Inmueble entidad)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (id == 0)
                {
                    id = repositorio.Alta(entidad);
                }
                else
                {
                    repositorio.Modificacion(entidad);
                }
                var res = repositorio.BuscarPorPropietario(entidad.PropietarioId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Inmuebles/Eliminar/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmuebles/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Eliminar(int id, Inmueble entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        // POST: Inmuebles/Borrar/5 (para formularios que postean a Borrar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Borrar(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Inmuebles/CambiarEstado/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult CambiarEstado(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();
                entidad.Habilitado = !entidad.Habilitado;
                repositorio.Modificacion(entidad);
                return Ok(entidad);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}