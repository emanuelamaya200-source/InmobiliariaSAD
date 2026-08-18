using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Propietario
    {
        [Key]
        [Display(Name = "Código Prop.")]
        public int IdPropietario { get; set; }
        [Required]
        public string Nombre { get; set; } = "";
        [Required]
        public string Apellido { get; set; } = "";
        [Required]
        public string Dni { get; set; } = "";
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "La clave es obligatoria"), DataType(DataType.Password)]
        public string Clave { get; set; } = "";
        // public List<Propiedades> MisPropiedades { get; set;} = new List<Propiedades>();
        public override string ToString()
        {
            //return $"{Apellido}, {Nombre}";
            //return $"{Nombre} {Apellido}";
            var res = $"{Nombre} {Apellido}";
            if(!String.IsNullOrEmpty(Dni)) {
                res += $" ({Dni})";
            }
            return res;
        }
    }
}