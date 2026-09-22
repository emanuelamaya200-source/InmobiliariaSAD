namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioInquilino : IRepositorio<Inquilino>
    {
        IList<Inquilino> BuscarPorNombre(string Nombre);
    }
}