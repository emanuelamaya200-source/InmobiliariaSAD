namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		int ModificarPortada(int InmuebleId, string ruta);
		IList<Inmueble> BuscarPorPropietario(int idPropietario);

		IList<Inmueble> BuscarPorTipo(string tipo);

		IList<Inmueble> BuscarPorTipo(string tipo, int pagina, int tamPagina);
		int ObtenerCantidadFiltrada(string? busqueda, int? disponibilidad);
		IList<Inmueble> ObtenerListaFiltrada(string? busqueda, int? disponibilidad, int pagina, int tamPagina);

		IList<Inmueble> ObtenerInformeReservas(bool sinReservas, int dias, int pagina, int tamPagina, out int total);

		IList<Inmueble> MasReservados(int dias);
	}
}