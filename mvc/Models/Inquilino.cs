using System.ComponentModel.DataAnnotations;
namespace Inmobiliaria_.Net_Core.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInquilino { get; set; }
        [Required]
        public string Nombre { get; set; } = "";
        [Required]
        public string Apellido { get; set; }= "";
        [Required]
        public string Dni { get; set; } ="";
        public string Telefono { get; set; }="";
        [Required, EmailAddress]
        public string Email { get; set; }= "";
        // public List<Reservas> MisReservas { get; set;} = new List<Reservas>();
    }
}