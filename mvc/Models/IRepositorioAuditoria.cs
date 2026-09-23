using System.Security.Claims;

namespace Inmobiliaria_.Net_Core.Models
{
    public interface IRepositorioAuditoria : IRepositorio<Auditoria>
    {
        void Registrar(ClaimsPrincipal usuario, string entidad, int entidadId, string accion, string? detalle = null);
    }
}
