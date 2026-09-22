namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {
        int Reactivar(int id);
        int ObtenerCantidad(bool incluirInactivos);
        IList<Pago> ObtenerLista(int pagina, int tamanioPagina, bool incluirInactivos);
        Pago? ObtenerPorReserva(int idReserva);
        IList<Pago> ObtenerListaPorReserva(int idReserva);
    }
}