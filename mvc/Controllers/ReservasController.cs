using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Inmobiliaria_.Net_Core.Models;

namespace mvc.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IRepositorioReserva repositorio;

        public ReservasController(IRepositorioReserva repositorio)
        {
            this.repositorio = repositorio;
        }

        // GET: Reservas
        public IActionResult Index(int? id)
        {
            IList<Reserva> lista;

            if (id.HasValue && id.Value > 0)
            {
                var r = repositorio.ObtenerPorId(id.Value);
                lista = r is null ? new List<Reserva>() : new List<Reserva> { r };
            }
            else
            {
                lista = repositorio.ObtenerLista();
            }

            return View(lista);
        }

        // GET: Reservas/Detalles/5
        public IActionResult Detalles(int id)
        {
            var reserva = repositorio.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // GET: Reservas/Editar/5 (Crear si id <= 0, Modificar si id > 0)
        public IActionResult Editar(int id)
        {
            if (id > 0)
            {
                var reserva = repositorio.ObtenerPorId(id);
                if (reserva == null)
                {
                    return NotFound();
                }
                return View(reserva);
            }

            return View(new Reserva());
        }

        // POST: Reservas/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                return View("Editar", reserva);
            }

            if (reserva.FechaDeSalida <= reserva.FechaDeEntrada)
            {
                ModelState.AddModelError(string.Empty, "La fecha de salida debe ser posterior a la fecha de entrada.");
                return View("Editar", reserva);
            }

            // Validar solapamiento de fechas
            var reservasExistentes = repositorio.ObtenerLista();
            bool haySolapamiento = reservasExistentes.Any(r => 
                r.IdInmueble == reserva.IdInmueble &&
                r.IdReserva != reserva.IdReserva &&
                r.FechaDeEntrada < reserva.FechaDeSalida &&
                r.FechaDeSalida > reserva.FechaDeEntrada
            );

            if (haySolapamiento)
            {
                ModelState.AddModelError(string.Empty, "El inmueble no está disponible en las fechas elegidas.");
                return View("Editar", reserva);
            }

            try
            {
                if (reserva.IdReserva > 0)
                {
                    repositorio.Modificacion(reserva);
                }
                else
                {
                    repositorio.Alta(reserva);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar: " + ex.Message);
                return View("Editar", reserva);
            }
        }

        // GET: Reservas/Eliminar/5 
        public IActionResult Eliminar(int id)
        {
            var reserva = repositorio.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // POST: Reservas/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmado(int id)
        {
            try
            {
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar la reserva: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Reservas/VerificarDisponibilidad
        [HttpGet]
        public IActionResult VerificarDisponibilidad(DateTime inicioFecha, DateTime finFecha)
        {
            var disponibles = repositorio.VerificarDisponibilidad(inicioFecha, finFecha);
            return Json(disponibles);
        }

        // GET: Reservas/CalcularMulta
        [HttpGet]
        public IActionResult CalcularMulta(int idReserva, DateTime finFecha)
        {
            try
            {
                var multa = repositorio.CalcularMulta(idReserva, finFecha);
                return Json(new { Multa = multa });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: Reservas/TerminarReservaAnticipada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TerminarReservaAnticipada(int idReserva, DateTime nuevoFinFecha)
        {
            try
            {
                var ok = repositorio.TerminarReservaAnticipada(idReserva, nuevoFinFecha);
                if (!ok)
                {
                    return BadRequest("No se pudo terminar la reserva.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: Reservas/RenovarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RenovarReserva(int idReserva, DateTime nuevoFinFecha, decimal nuevoPrecio)
        {
            try
            {
                var reserva = repositorio.RenovarReserva(idReserva, nuevoFinFecha, nuevoPrecio);
                if (reserva == null)
                {
                    return BadRequest("No se pudo renovar la reserva.");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}