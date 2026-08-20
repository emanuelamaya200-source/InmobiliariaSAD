
namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorio <E>
    {
        int Alta(E p);
        int Baja(int id);
        int Modificacion(E p);
    }
}