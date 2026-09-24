using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "Numero de Reserva")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        
        [Display(Name ="Inmueble")]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]

        [Display(Name ="Inquilino")]
        public int IdInquilino { get; set; }
        public string NombreInquilino { get; set; } = "";
        public string NombreInmueble { get; set; } = "";

        [Required(ErrorMessage = "El campo es obligatorio")]
        public DateTime FechaDeEntrada { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public DateTime FechaDeSalida { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        public int MontoTotal { get; set; }

        public int PagoRealizado { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";

        public decimal MontoDiario { get; set; }
        public DateTime? FechaFinEfectiva { get; set; }
        public int? UsuarioCreacionId { get; set; }
        public int? UsuarioFinalizacionId { get; set; }
    }
}