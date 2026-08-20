namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorio <E>
    {
        int Alta(E entidad);
        int Baja(int id);
        int Modificacion(E entidad);
    }
}