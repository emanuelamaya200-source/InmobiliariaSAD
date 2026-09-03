using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "Identificacion de Reserva")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public DateTime FechaDeEntrada { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public DateTime FechaDeSalida { get; set; }
    }
}