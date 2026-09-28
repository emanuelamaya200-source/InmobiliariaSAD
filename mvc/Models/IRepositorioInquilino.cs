namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        IList<Inquilino> BuscarPorNombre(string Nombre);
        
        IList<Inquilino> BuscarPorNombre(string nombre, int pagina, int tamPagina);
        int ObtenerCantidadPorNombre(string? nombre);
    }
}