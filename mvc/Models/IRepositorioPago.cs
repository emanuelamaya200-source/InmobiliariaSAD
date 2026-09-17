namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        int Reactivar(int id);
        IList<Pago> ObtenerLista(int pagina, int tamanioPagina, bool incluirInactivos);
        Pago? ObtenerPorReserva(int idReserva);
    }
}