using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface IReglamentoService
{
    Task<IEnumerable<PerfilReglamentoDto>> ObtenerReglamentosAsync();
    Task<PerfilReglamentoDto?> ObtenerPorCodigoAsync(string codigo);
    Task<PerfilReglamentoDto> CrearReglamentoAsync(CrearPerfilReglamentoDto dto);
    Task AsegurarReglamentosPorDefectoAsync();
}
