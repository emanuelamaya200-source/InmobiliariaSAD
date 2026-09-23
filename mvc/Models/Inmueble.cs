using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria_.Net_Core.Models
{
	public class Inmueble
	{
		[Display(Name = "Nº")]
		public int Id { get; set; }
		//[Required]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "La dirección es requerida")]
		public string? Direccion { get; set; }
		[Required]
		public int Cupo { get; set; }
		[Required]
		public decimal PrecioPorDia { get; set; }
		[Required]
		public decimal PorcentajeReserva  { get; set; }
		public decimal Latitud { get; set; }
		public decimal Longitud { get; set; }
		[Display(Name = "Dueño")]
		public int PropietarioId { get; set; }
		public int IdTipoInmueble { get; set; }
    [BindNever]
		public Propietario? Duenio { get; set; }
		public tipoInmueble? Tipo { get; set;}
		public string? Portada { get; set; }
		public bool Habilitado { get; set; } = true;
	}
	
}
