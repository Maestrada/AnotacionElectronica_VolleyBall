using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;
using AnotacionElectronica_Leon_VB.Domain.ValueObjects;
using System.Text.Json;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public sealed class PartidoService : IPartidoService
{
    private readonly IPartidoRepository _partidos;
    private readonly IUnitOfWork _unitOfWork;

    public PartidoService(IPartidoRepository partidos, IUnitOfWork unitOfWork)
    {
        _partidos = partidos;
        _unitOfWork = unitOfWork;
    }

    public async Task<PartidoResponseDto> CrearPartidoAsync(CrearPartidoDto dto)
    {
        if (dto.EquipoLocalId == dto.EquipoVisitanteId)
            throw new ArgumentException("Los equipos local y visitante deben ser distintos.");
        if (string.IsNullOrWhiteSpace(dto.Lugar))
            throw new ArgumentException("El lugar del partido es obligatorio.");

        var partido = new Partido(dto.EquipoLocalId, dto.EquipoVisitanteId, dto.FechaProgramada, dto.Lugar.Trim(),
            dto.Reglamento is null ? null : new ConfiguracionReglamentoPartido(dto.Reglamento.CodigoReglamento,
                dto.Reglamento.MaximoSets, dto.Reglamento.SetsParaGanar, dto.Reglamento.PuntosSetRegular,
                dto.Reglamento.PuntosSetDecisivo, dto.Reglamento.DiferenciaMinima,
                dto.Reglamento.PuntoCambioCanchaSetDecisivo));
        await _partidos.AddAsync(partido);
        await _unitOfWork.SaveChangesAsync();
        return Map(partido);
    }

    public async Task<PartidoResponseDto?> GetPartidoByIdAsync(Guid partidoId)
    {
        var partido = await _partidos.GetPartidoConDetallesAsync(partidoId);
        return partido is null ? null : Map(partido);
    }

    public async Task IniciarPartidoAsync(Guid partidoId)
    {
        var partido = await ObtenerPartido(partidoId);
        partido.IniciarPartido();
        await RegistrarEvento(partido.Id, null, TipoEventoPartido.PartidoIniciado, new { });
        var primerSet = partido.Sets.Single();
        await RegistrarEvento(partido.Id, primerSet.Id, TipoEventoPartido.SetIniciado, new { primerSet.NumeroSet });
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PartidoResumenDto> ObtenerResumenPartidoAsync(Guid partidoId)
    {
        var partido = await ObtenerPartido(partidoId);
        return new PartidoResumenDto(partido.Id, partido.EquipoLocalId, partido.EquipoVisitanteId,
            partido.SetsGanadosLocal, partido.SetsGanadosVisitante,
            partido.Estado == Domain.Enums.EstadoPartido.Finalizado, partido.EquipoGanadorId,
            partido.Sets.OrderBy(s => s.NumeroSet).Select(Map));
    }

    public async Task<SetDetalleDto> IniciarSetAsync(Guid partidoId, int numeroSet)
    {
        var partido = await ObtenerPartido(partidoId);
        var esperado = partido.Sets.Count + 1;
        if (numeroSet != esperado)
            throw new InvalidOperationException($"El siguiente set debe ser el número {esperado}.");

        var set = partido.IniciarSiguienteSet();
        await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.SetIniciado, new { set.NumeroSet });
        await _unitOfWork.SaveChangesAsync();
        return Map(set);
    }

    public async Task<SetDetalleDto?> ObtenerSetActualAsync(Guid partidoId)
    {
        var partido = await _partidos.GetPartidoConDetallesAsync(partidoId);
        return partido?.Sets.OrderByDescending(s => s.NumeroSet).Select(Map).FirstOrDefault();
    }

    public async Task<MarcadorLiveDto> RegistrarPuntoAsync(RegistrarPuntoDto dto)
    {
        var partido = await ObtenerPartido(dto.PartidoId);
        var set = partido.Sets.OrderByDescending(s => s.NumeroSet).FirstOrDefault()
            ?? throw new InvalidOperationException("El partido todavía no tiene un set activo.");
        if (dto.EquipoAnotadorId != partido.EquipoLocalId && dto.EquipoAnotadorId != partido.EquipoVisitanteId)
            throw new ArgumentException("El equipo anotador no pertenece a este partido.");

        set.AnotarPunto(dto.EquipoAnotadorId, partido.EquipoLocalId, partido.EquipoVisitanteId,
            dto.TipoAccion, dto.JugadorAnotadorId);
        await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.PuntoRegistrado, new
        {
            dto.EquipoAnotadorId,
            dto.TipoAccion,
            dto.JugadorAnotadorId,
            set.PuntosLocal,
            set.PuntosVisitante
        });
        if (set.PendienteConfirmacionCierre)
            await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.SetPendienteConfirmacion, new
            {
                set.NumeroSet,
                set.PuntosLocal,
                set.PuntosVisitante
            });
        if (set.PendienteCambioCancha)
            await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.CambioCanchaPendiente, new
            {
                set.NumeroSet,
                set.PuntosLocal,
                set.PuntosVisitante,
                set.PuntoCambioCancha
            });

        await _unitOfWork.SaveChangesAsync();
        return new MarcadorLiveDto(set.Id, set.PuntosLocal, set.PuntosVisitante,
            set.UltimoEquipoAlSaqueId, set.Finalizado, set.PendienteConfirmacionCierre, set.PendienteCambioCancha,
            partido.Estado == Domain.Enums.EstadoPartido.Finalizado);
    }

    public async Task<MarcadorLiveDto> DeshacerUltimoPuntoAsync(Guid partidoId)
    {
        var partido = await ObtenerPartido(partidoId);
        var set = ObtenerSetActual(partido);
        var punto = set.DeshacerUltimoPunto();
        await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.PuntoDeshecho, new
        {
            punto.Id,
            punto.EquipoAnotadorId,
            set.PuntosLocal,
            set.PuntosVisitante
        });
        await _unitOfWork.SaveChangesAsync();
        return new MarcadorLiveDto(set.Id, set.PuntosLocal, set.PuntosVisitante, set.UltimoEquipoAlSaqueId,
            set.Finalizado, set.PendienteConfirmacionCierre, set.PendienteCambioCancha, false);
    }

    public async Task<MarcadorLiveDto> ConfirmarCierreSetAsync(Guid partidoId)
    {
        var partido = await ObtenerPartido(partidoId);
        var set = ObtenerSetActual(partido);
        set.ConfirmarCierre(partido.EquipoLocalId, partido.EquipoVisitanteId);
        partido.RegistrarResultadoSet(set);
        await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.SetConfirmado, new
        {
            set.NumeroSet,
            set.PuntosLocal,
            set.PuntosVisitante,
            set.EquipoGanadorId,
            partido.SetsGanadosLocal,
            partido.SetsGanadosVisitante
        });
        await _unitOfWork.SaveChangesAsync();
        return new MarcadorLiveDto(set.Id, set.PuntosLocal, set.PuntosVisitante, set.UltimoEquipoAlSaqueId,
            true, false, false, partido.Estado == Domain.Enums.EstadoPartido.Finalizado);
    }

    public async Task<MarcadorLiveDto> ConfirmarCambioCanchaAsync(Guid partidoId)
    {
        var partido = await ObtenerPartido(partidoId);
        var set = ObtenerSetActual(partido);
        set.ConfirmarCambioCancha();
        await RegistrarEvento(partido.Id, set.Id, TipoEventoPartido.CambioCanchaConfirmado, new
        {
            set.NumeroSet,
            set.PuntosLocal,
            set.PuntosVisitante
        });
        await _unitOfWork.SaveChangesAsync();
        return new MarcadorLiveDto(set.Id, set.PuntosLocal, set.PuntosVisitante, set.UltimoEquipoAlSaqueId,
            set.Finalizado, set.PendienteConfirmacionCierre, false,
            partido.Estado == Domain.Enums.EstadoPartido.Finalizado);
    }

    public async Task<IReadOnlyList<EventoPartidoDto>> ObtenerEventosAsync(Guid partidoId) =>
        (await _partidos.ObtenerEventosAsync(partidoId)).Select(e => new EventoPartidoDto(e.Id, e.PartidoId,
            e.SetId, e.Secuencia, e.Tipo.ToString(), e.DatosJson, e.OcurrioEnUtc)).ToList();

    private async Task<Partido> ObtenerPartido(Guid partidoId) =>
        await _partidos.GetPartidoConDetallesAsync(partidoId)
        ?? throw new KeyNotFoundException($"No se encontró el partido con ID {partidoId}.");

    private static PartidoResponseDto Map(Partido partido) => new(partido.Id, partido.EquipoLocalId,
        partido.EquipoVisitanteId, partido.FechaProgramada, partido.Lugar, partido.SetsGanadosLocal,
        partido.SetsGanadosVisitante, partido.Estado == Domain.Enums.EstadoPartido.Finalizado, partido.EquipoGanadorId,
        MapReglamento(partido));

    private static SetDetalleDto Map(Set set) => new(set.Id, set.PartidoId, set.NumeroSet, set.PuntosLocal,
        set.PuntosVisitante, set.Finalizado, set.PendienteConfirmacionCierre, set.PendienteCambioCancha,
        set.CambioCanchaConfirmado, set.EquipoGanadorId);

    private static ConfiguracionReglamentoPartidoDto MapReglamento(Partido partido) => new(partido.CodigoReglamento,
        partido.MaximoSets, partido.SetsParaGanar, partido.PuntosSetRegular, partido.PuntosSetDecisivo,
        partido.DiferenciaMinima, partido.PuntoCambioCanchaSetDecisivo);

    private static Set ObtenerSetActual(Partido partido) => partido.Sets.OrderByDescending(s => s.NumeroSet)
        .FirstOrDefault() ?? throw new InvalidOperationException("El partido todavía no tiene un set activo.");

    private async Task RegistrarEvento(Guid partidoId, Guid? setId, TipoEventoPartido tipo, object datos) =>
        await _partidos.AgregarEventoAsync(new EventoPartido(partidoId, setId,
            await _partidos.ObtenerSiguienteSecuenciaEventoAsync(partidoId), tipo, JsonSerializer.Serialize(datos)));
}
