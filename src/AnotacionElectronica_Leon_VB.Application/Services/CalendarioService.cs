using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.ValueObjects;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public sealed class CalendarioService : ICalendarioService
{
    private readonly IRepository<JuegoCalendario> _juegos;
    private readonly IPartidoRepository _partidos;
    private readonly IUnitOfWork _unitOfWork;

    public CalendarioService(IRepository<JuegoCalendario> juegos, IPartidoRepository partidos, IUnitOfWork unitOfWork)
    {
        _juegos = juegos;
        _partidos = partidos;
        _unitOfWork = unitOfWork;
    }

    public async Task<JuegoCalendarioDto> CrearAsync(CrearJuegoCalendarioDto dto)
    {
        var regla = dto.Reglamento is null ? null : new ConfiguracionReglamentoPartido(dto.Reglamento.CodigoReglamento,
            dto.Reglamento.MaximoSets, dto.Reglamento.SetsParaGanar, dto.Reglamento.PuntosSetRegular,
            dto.Reglamento.PuntosSetDecisivo, dto.Reglamento.DiferenciaMinima, dto.Reglamento.PuntoCambioCanchaSetDecisivo);
        var juego = new JuegoCalendario(dto.Codigo, dto.EquipoLocalId, dto.EquipoVisitanteId, dto.FechaHoraProgramada,
            dto.Recinto, dto.Competicion, dto.Edicion, dto.Fase, regla);
        await _juegos.AddAsync(juego);
        await _unitOfWork.SaveChangesAsync();
        return Map(juego);
    }

    public async Task<IReadOnlyList<JuegoCalendarioDto>> ObtenerAsync(DateTime? desde, DateTime? hasta) =>
        (await _juegos.GetAllAsync()).Where(j => (!desde.HasValue || j.FechaHoraProgramada >= desde) &&
            (!hasta.HasValue || j.FechaHoraProgramada <= hasta)).OrderBy(j => j.FechaHoraProgramada).Select(Map).ToList();

    public async Task<PartidoResponseDto> CrearPartidoDesdeCalendarioAsync(Guid juegoCalendarioId)
    {
        var juego = await _juegos.GetByIdAsync(juegoCalendarioId)
            ?? throw new KeyNotFoundException("No se encontró el juego programado.");
        var partido = new Partido(juego.EquipoLocalId, juego.EquipoVisitanteId, juego.FechaHoraProgramada,
            juego.Recinto, juego.ObtenerReglamento());
        juego.VincularPartido(partido.Id);
        await _partidos.AddAsync(partido);
        await _unitOfWork.SaveChangesAsync();
        return new PartidoResponseDto(partido.Id, partido.EquipoLocalId, partido.EquipoVisitanteId,
            partido.FechaProgramada, partido.Lugar, 0, 0, false, null, MapReglamento(partido));
    }

    private static JuegoCalendarioDto Map(JuegoCalendario juego) => new(juego.Id, juego.Codigo, juego.Competicion,
        juego.Edicion, juego.Fase, juego.EquipoLocalId, juego.EquipoVisitanteId, juego.FechaHoraProgramada,
        juego.Recinto, juego.Estado.ToString(), juego.PartidoId, new ConfiguracionReglamentoPartidoDto(
            juego.CodigoReglamento, juego.MaximoSets, juego.SetsParaGanar, juego.PuntosSetRegular,
            juego.PuntosSetDecisivo, juego.DiferenciaMinima, juego.PuntoCambioCanchaSetDecisivo));

    private static ConfiguracionReglamentoPartidoDto MapReglamento(Partido partido) => new(partido.CodigoReglamento,
        partido.MaximoSets, partido.SetsParaGanar, partido.PuntosSetRegular, partido.PuntosSetDecisivo,
        partido.DiferenciaMinima, partido.PuntoCambioCanchaSetDecisivo);
}
