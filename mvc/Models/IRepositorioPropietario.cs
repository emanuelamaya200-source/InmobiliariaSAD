namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        Propietario? ObtenerPorEmail(string Email);
        IList<Propietario> BuscarPorNombre(string Nombre);
        
        IList<Propietario> BuscarPorNombre(string nombre, int pagina, int tamPagina);
        int ObtenerCantidadPorNombre(string? nombre);
    }

}