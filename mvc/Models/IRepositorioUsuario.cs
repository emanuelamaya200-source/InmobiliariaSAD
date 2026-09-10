namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioUsuario : IRepositorio<Usuario>
	{
		Usuario? ObtenerPorEmail(string email);
	}
}
