using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface ICalendarioService
{
    Task<JuegoCalendarioDto> CrearAsync(CrearJuegoCalendarioDto dto);
    Task<IReadOnlyList<JuegoCalendarioDto>> ObtenerAsync(DateTime? desde, DateTime? hasta);
    Task<PartidoResponseDto> CrearPartidoDesdeCalendarioAsync(Guid juegoCalendarioId);
}
