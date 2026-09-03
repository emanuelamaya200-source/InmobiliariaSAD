using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class tipoInmueble{
        
        [Key]
        [Display(Name = "Código")]
        public int idTipoInmueble { get; set; }

        [Required(ErrorMessage = "El nombre o descripcion es obligatoria")]
        [Display(Name = "Tipo de Inmueble")]
        public string Descripcion { get; set; } = string.Empty;
    }
}