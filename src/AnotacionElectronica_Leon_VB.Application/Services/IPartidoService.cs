using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface IPartidoService
{
    Task<PartidoResponseDto> CrearPartidoAsync(CrearPartidoDto dto);
    Task<PartidoResponseDto?> GetPartidoByIdAsync(Guid partidoId);
    Task IniciarPartidoAsync(Guid partidoId);
    Task<PartidoResumenDto> ObtenerResumenPartidoAsync(Guid partidoId);
    Task<SetDetalleDto> IniciarSetAsync(Guid partidoId, int numeroSet);
    Task<SetDetalleDto?> ObtenerSetActualAsync(Guid partidoId);
    Task<MarcadorLiveDto> RegistrarPuntoAsync(RegistrarPuntoDto dto);
    Task<MarcadorLiveDto> DeshacerUltimoPuntoAsync(Guid partidoId);
    Task<MarcadorLiveDto> ConfirmarCierreSetAsync(Guid partidoId);
    Task<MarcadorLiveDto> ConfirmarCambioCanchaAsync(Guid partidoId);
    Task<IReadOnlyList<EventoPartidoDto>> ObtenerEventosAsync(Guid partidoId);
}
