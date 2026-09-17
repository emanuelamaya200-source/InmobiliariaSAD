using System;
using System.Collections.Generic;

namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioReserva : IRepositorio<Reserva>
    {
        IList<Inmueble> VerificarDisponibilidad(DateTime inicioFecha, DateTime finFecha, int cupo = 0);
        decimal CalcularMulta(int idReserva, DateTime finFecha);
        bool TerminarReservaAnticipada(int idReserva, DateTime nuevoFinFecha);
        Reserva RenovarReserva(int idReserva, DateTime nuevoFinFecha, decimal nuevoPrecio);
        IList<Reserva> ObtenerPorRango(DateTime? inicio, DateTime? fin, int? cupo);
        int Cancelar(int id, int usuarioId);
    }
}