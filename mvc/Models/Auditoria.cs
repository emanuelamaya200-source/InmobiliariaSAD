using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_.Net_Core.Models
{
    public class Auditoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Entidad")]
        public string Entidad { get; set; } = "";

        [Display(Name = "Id de entidad")]
        public int EntidadId { get; set; }

        [Required]
        [Display(Name = "Acción")]
        public string Accion { get; set; } = "";

        [Display(Name = "Usuario")]
        public int? UsuarioId { get; set; }

        [Display(Name = "Nombre del usuario")]
        public string? UsuarioNombre { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Detalle")]
        public string? Detalle { get; set; }
    }
}
