namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioTipoInmueble : IRepositorio<tipoInmueble>
    {
        public IList<tipoInmueble> Buscar(string term);
    }

}