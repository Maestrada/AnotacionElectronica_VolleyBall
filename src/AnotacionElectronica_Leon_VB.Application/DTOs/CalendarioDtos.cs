namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearJuegoCalendarioDto(string Codigo, Guid EquipoLocalId, Guid EquipoVisitanteId,
    DateTime FechaHoraProgramada, string Recinto, string? Competicion, string? Edicion, string? Fase,
    ConfiguracionReglamentoPartidoDto? Reglamento = null);

public record JuegoCalendarioDto(Guid Id, string Codigo, string? Competicion, string? Edicion, string? Fase,
    Guid EquipoLocalId, Guid EquipoVisitanteId, DateTime FechaHoraProgramada, string Recinto, string Estado,
    Guid? PartidoId, ConfiguracionReglamentoPartidoDto Reglamento);
