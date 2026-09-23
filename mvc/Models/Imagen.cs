using System;


namespace Inmobiliaria_.Net_Core.Models
{
	public class Imagen
	{
		public int Id { get; set; }
		public int InmuebleId { get; set; }
		public string Url { get; set; } = "";
		public IFormFile? Archivo { get; set; } = null;
	}
}
