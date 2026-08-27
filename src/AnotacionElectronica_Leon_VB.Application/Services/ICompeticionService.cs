using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface ICompeticionService
{
    Task<IEnumerable<CompeticionDto>> ObtenerCompeticionesAsync();
    Task<CompeticionDto?> ObtenerPorIdAsync(Guid id);
    Task<CompeticionDto> CrearCompeticionAsync(CrearCompeticionDto dto);
}
