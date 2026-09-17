using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
     public class Pago
    {
        [Key]
        [Display(Name = "Identificacion de Pago")]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public int idReserva { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [Display(Name = "Estado")]
        public String Estado { get; set; }

    }
}