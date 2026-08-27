using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public class ReglamentoService : IReglamentoService
{
    private readonly IRepository<PerfilReglamento> _reglamentos;
    private readonly IUnitOfWork _unitOfWork;

    public ReglamentoService(IRepository<PerfilReglamento> reglamentos, IUnitOfWork unitOfWork)
    {
        _reglamentos = reglamentos;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PerfilReglamentoDto>> ObtenerReglamentosAsync()
    {
        await AsegurarReglamentosPorDefectoAsync();

        var lista = await _reglamentos.GetAllAsync();

        return lista
            .OrderBy(r => r.CodigoReglamento)
            .Select(MapToDto);
    }

    public async Task<PerfilReglamentoDto?> ObtenerPorCodigoAsync(string codigo)
    {
        await AsegurarReglamentosPorDefectoAsync();

        var lista = await _reglamentos.GetAllAsync();
        var reg = lista.FirstOrDefault(r => r.CodigoReglamento.Equals(codigo, StringComparison.OrdinalIgnoreCase));

        return reg is null ? null : MapToDto(reg);
    }

    public async Task<PerfilReglamentoDto> CrearReglamentoAsync(CrearPerfilReglamentoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CodigoReglamento))
            throw new ArgumentException("El código de reglamento es obligatorio.");

        var lista = await _reglamentos.GetAllAsync();
        var existente = lista.Any(r => r.CodigoReglamento.Equals(dto.CodigoReglamento.Trim(), StringComparison.OrdinalIgnoreCase));

        if (existente)
            throw new InvalidOperationException($"Ya existe un reglamento con el código '{dto.CodigoReglamento}'.");

        var reg = new PerfilReglamento(
            dto.CodigoReglamento.Trim(),
            string.IsNullOrWhiteSpace(dto.Nombre) ? dto.CodigoReglamento.Trim() : dto.Nombre.Trim(),
            dto.Descripcion?.Trim(),
            dto.MaximoSets,
            dto.SetsParaGanar,
            dto.PuntosSetRegular,
            dto.PuntosSetDecisivo,
            dto.DiferenciaMinima,
            dto.PuntoCambioCanchaSetDecisivo);

        await _reglamentos.AddAsync(reg);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(reg);
    }

    public async Task AsegurarReglamentosPorDefectoAsync()
    {
        var lista = await _reglamentos.GetAllAsync();
        if (!lista.Any())
        {
            var presets = new List<PerfilReglamento>
            {
                new(
                    "FIVB-2025-2028",
                    "Oficial FIVB (Mejor de 5 Sets)",
                    "Reglamento oficial estándar internacional de voleibol sala.",
                    maximoSets: 5,
                    setsParaGanar: 3,
                    puntosSetRegular: 25,
                    puntosSetDecisivo: 15,
                    diferenciaMinima: 2,
                    puntoCambioCanchaSetDecisivo: 8),
                new(
                    "LEON-REGULAR-3SETS",
                    "Liga León Regular (Mejor de 3 Sets)",
                    "Reglamento local para fase regular: gana 2 de 3 sets (25 pts reg, 15 pts dec).",
                    maximoSets: 3,
                    setsParaGanar: 2,
                    puntosSetRegular: 25,
                    puntosSetDecisivo: 15,
                    diferenciaMinima: 2,
                    puntoCambioCanchaSetDecisivo: 8),
                new(
                    "TORNEO-RAPIDO-21PTS",
                    "Torneo Rápido (3 Sets a 21 pts)",
                    "Formato dinámico para torneos relámpago con sets a 21 puntos.",
                    maximoSets: 3,
                    setsParaGanar: 2,
                    puntosSetRegular: 21,
                    puntosSetDecisivo: 15,
                    diferenciaMinima: 2,
                    puntoCambioCanchaSetDecisivo: 8)
            };

            foreach (var preset in presets)
            {
                await _reglamentos.AddAsync(preset);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private static PerfilReglamentoDto MapToDto(PerfilReglamento r) =>
        new(r.Id, r.CodigoReglamento, r.Nombre, r.Descripcion, r.MaximoSets, r.SetsParaGanar,
            r.PuntosSetRegular, r.PuntosSetDecisivo, r.DiferenciaMinima, r.PuntoCambioCanchaSetDecisivo);
}
