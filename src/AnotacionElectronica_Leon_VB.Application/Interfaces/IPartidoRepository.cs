using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.Interfaces;

public interface IPartidoRepository : IRepository<Partido>
{
    Task<Partido?> GetPartidoConDetallesAsync(Guid partidoId);
    Task<IEnumerable<Partido>> GetPartidosPorEstadoAsync(EstadoPartido estado);
    Task<IEnumerable<Partido>> GetPartidosPorEquipoAsync(Guid equipoId);
    //Task<AlineacionSet?> GetAlineacionPorSetYEquipoAsync(Guid setId, Guid equipoId);

    Task<int> ObtenerSiguienteSecuenciaEventoAsync(Guid partidoId);
    Task AgregarEventoAsync(EventoPartido evento);
    Task<IReadOnlyList<EventoPartido>> ObtenerEventosAsync(Guid partidoId);
}

 