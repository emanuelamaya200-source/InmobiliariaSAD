namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		int ModificarPortada(int InmuebleId, string ruta);
		IList<Inmueble> BuscarPorPropietario(int idPropietario);
	}
}