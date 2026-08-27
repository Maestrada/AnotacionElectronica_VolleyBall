using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface IArbitroService
{
    Task<IEnumerable<ArbitroDto>> ObtenerArbitrosAsync();
    Task<ArbitroDto?> ObtenerPorIdAsync(Guid id);
    Task<ArbitroDto> CrearArbitroAsync(CrearArbitroDto dto);
}
