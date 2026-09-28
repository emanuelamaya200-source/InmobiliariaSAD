using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inmobiliaria_.Net_Core.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;
        private readonly IRepositorioTipoInmueble repoTipoInmueble;
        private readonly IRepositorioAuditoria auditoriaRepositorio;
        private readonly IWebHostEnvironment environment;

        public InmueblesController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietrio, IRepositorioTipoInmueble repoTipoInmueble, IRepositorioAuditoria auditoriaRepositorio, IWebHostEnvironment environment)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietrio;
            this.repoTipoInmueble = repoTipoInmueble;
            this.auditoriaRepositorio = auditoriaRepositorio;
            this.environment = environment;
            
        }

                // GET: Inmuebles
        [HttpGet]
        public ActionResult Index(int pagina = 1, string? nombre = null, int? disponibilidad = null)
        {
            const int tamPagina = 10;
            pagina = Math.Max(1, pagina);

            var total = repositorio.ObtenerCantidadFiltrada(nombre, disponibilidad);
            var lista = repositorio.ObtenerListaFiltrada(nombre, disponibilidad, pagina, tamPagina);

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (total + tamPagina - 1) / tamPagina;
            ViewBag.Nombre = nombre;
            ViewBag.Disponibilidad = disponibilidad;

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            return View(lista);
        }

        [HttpGet]
        public ActionResult InformeReservas(bool sinReservas = false, int plazo = 30, int? diasPersonalizados = null, int pagina = 1)
        {
            const int tamPagina = 10;
            int dias = sinReservas ? Math.Clamp(plazo == 0 ? diasPersonalizados ?? 30 : plazo, 1, 3650) : 365;
            pagina = Math.Max(1, pagina);
            var inmuebles = repositorio.ObtenerInformeReservas(sinReservas, dias, pagina, tamPagina, out int total);
            ViewBag.SinReservas = sinReservas;
            ViewBag.Dias = dias;
            ViewBag.Plazo = sinReservas ? plazo : 365;
            ViewBag.DiasPersonalizados = diasPersonalizados;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = (total + tamPagina - 1) / tamPagina;
            return View(inmuebles);
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

        // GET: Inmuebles/Crear
        [HttpGet]
        public ActionResult Crear()
        {
            ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();
            return View(new Inmueble());
        }

        // POST: Inmuebles/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Inmueble entidad)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();
                return View(entidad);
            }
            try
            {
                int res = repositorio.Alta(entidad); 

                if (entidad.PortadaFile != null && entidad.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "inmueble_" + entidad.Id + Path.GetExtension(entidad.PortadaFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    entidad.Portada = Path.Combine("/Uploads", fileName).Replace("\\", "/");

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        entidad.PortadaFile.CopyTo(stream);
                    }

                    repositorio.Modificacion(entidad); 
                }

                auditoriaRepositorio.Registrar(User, "Inmueble", entidad.Id, "Alta", $"Dirección: {entidad.Direccion}");
                TempData["Mensaje"] = "Inmueble creado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();
                return View(entidad);
            }
        }

        // GET: Inmuebles/Editar/5
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Editar(int id)
        {
            ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();

            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];

            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null)
                return NotFound();

            ViewBag.PropietarioSeleccionado = repoPropietario.ObtenerPorId(entidad.PropietarioId);
            return View(entidad);
        }

        // POST: Inmuebles/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Editar(int id, Inmueble entidad)
        {
            if (id != entidad.Id)
                return NotFound();

            try
            {
                var actual = repositorio.ObtenerPorId(id);
                if (actual == null)
                    return NotFound();

                actual.Direccion = entidad.Direccion;
                actual.Cupo = entidad.Cupo;
                actual.PrecioPorDia = entidad.PrecioPorDia;
                actual.PorcentajeReserva = entidad.PorcentajeReserva;
                actual.Latitud = entidad.Latitud;
                actual.Longitud = entidad.Longitud;
                actual.PropietarioId = entidad.PropietarioId;
                actual.IdTipoInmueble = entidad.IdTipoInmueble;
                actual.Habilitado = entidad.Habilitado;

                if (entidad.PortadaFile != null)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "inmueble_" + id + Path.GetExtension(entidad.PortadaFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);

                    using (var stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        entidad.PortadaFile.CopyTo(stream);
                    }

                    actual.Portada = Path.Combine("/Uploads", fileName).Replace("\\", "/");
                }

                repositorio.Modificacion(actual);
                auditoriaRepositorio.Registrar(User, "Inmueble", actual.Id, "Modificacion", $"Dirección: {actual.Direccion}");
                TempData["Mensaje"] = "Inmueble modificado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.PropietarioSeleccionado = repoPropietario.ObtenerPorId(entidad.PropietarioId);
                ViewBag.tipoInmueble = repoTipoInmueble.ObtenerLista();
                return View(entidad);
            }
        }

        [HttpGet]
        public IActionResult BuscarPropietarios(string term = "")
        {
            var propietarios = string.IsNullOrWhiteSpace(term)
                ? repoPropietario.ObtenerLista(1, int.MaxValue)
                : repoPropietario.BuscarPorNombre(term.Trim());

            var resultados = propietarios
                .Select(p => new
                {
                    id = p.IdPropietario,
                    text = $"{p.Dni} - {p.Nombre} {p.Apellido}"
                });

            return Json(new { results = resultados });
        }


        // GET: Inmuebles/BuscarPorPropietario/5
        [HttpGet]
        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.BuscarPorPropietario(id);
            return Ok(lista);
        }

        // GET: Inmuebles/Details/5
        [HttpGet]
        public ActionResult Ver(int id)
        {
            var entidad = id == 0 ? new Inmueble() : repositorio.ObtenerPorId(id);
            return View(entidad);
        }

        // POST: Inmuebles/GuardarAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarAjax(int id, Inmueble entidad)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (id == 0)
                {
                    id = repositorio.Alta(entidad);
                    auditoriaRepositorio.Registrar(User, "Inmueble", id, "Alta", $"Dirección: {entidad.Direccion}");
                }
                else
                {
                    repositorio.Modificacion(entidad);
                    auditoriaRepositorio.Registrar(User, "Inmueble", id, "Modificacion", $"Dirección: {entidad.Direccion}");
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
        [HttpGet]
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
                auditoriaRepositorio.Registrar(User, "Inmueble", id, "Baja", "Inmueble eliminado");
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View(entidad);
            }
        }

        // POST: Inmuebles/Borrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Borrar(int id)
        {
            try
            {
                repositorio.Baja(id);
                auditoriaRepositorio.Registrar(User, "Inmueble", id, "Baja", "Inmueble eliminado");
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
                auditoriaRepositorio.Registrar(User, "Inmueble", id, "Modificacion", $"Estado habilitado: {entidad.Habilitado}");
                return Ok(entidad);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // esto retorna los datos en json, tiene que ser asi para que lo maneje el front en javascript
        public IActionResult ListarJson()
        {
            var inmuebles = repositorio.ObtenerLista();
            return Json(inmuebles);
        }

        
        [HttpGet]
        [Authorize(Roles ="Administrador,Empleado")]
        public IActionResult MasReservados(int dias = 365)
        {
            var lista = repositorio.MasReservados(dias);
            return Ok(lista); 
        }
    }
}