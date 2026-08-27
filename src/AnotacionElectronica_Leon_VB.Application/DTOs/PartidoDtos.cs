using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record ConfiguracionReglamentoPartidoDto(string CodigoReglamento, int MaximoSets, int SetsParaGanar,
    int PuntosSetRegular, int PuntosSetDecisivo, int DiferenciaMinima, int PuntoCambioCanchaSetDecisivo);

public record CrearPartidoDto(Guid EquipoLocalId, Guid EquipoVisitanteId, DateTime FechaProgramada, string Lugar,
    ConfiguracionReglamentoPartidoDto? Reglamento = null);

public record PartidoResponseDto(Guid Id, Guid EquipoLocalId, Guid EquipoVisitanteId, DateTime FechaProgramada,
    string Lugar, int SetsGanadosLocal, int SetsGanadosVisitante, bool Finalizado, Guid? EquipoGanadorId,
    ConfiguracionReglamentoPartidoDto Reglamento);

public record PartidoResumenDto(Guid PartidoId, Guid EquipoLocalId, Guid EquipoVisitanteId,
    int SetsGanadosLocal, int SetsGanadosVisitante, bool Finalizado, Guid? EquipoGanadorId,
    IEnumerable<SetDetalleDto> Sets);

public record SetDetalleDto(Guid Id, Guid PartidoId, int NumeroSet, int PuntosLocal, int PuntosVisitante,
    bool Finalizado, bool PendienteConfirmacionCierre, bool PendienteCambioCancha, bool CambioCanchaConfirmado,
    Guid? EquipoGanadorId);

public record RegistrarPuntoDto(Guid PartidoId, Guid EquipoAnotadorId, TipoAccionPunto TipoAccion,
    Guid? JugadorAnotadorId);

public record MarcadorLiveDto(Guid SetId, int PuntosLocal, int PuntosVisitante, Guid? UltimoEquipoAlSaqueId,
    bool SetFinalizado, bool PendienteConfirmacionCierre, bool PendienteCambioCancha, bool PartidoFinalizado);

public record EventoPartidoDto(Guid Id, Guid PartidoId, Guid? SetId, int Secuencia, string Tipo,
    string DatosJson, DateTime OcurrioEnUtc);
