using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record AlineacionDetalleDto(
    Guid AlineacionId,
    Guid SetId,
    Guid EquipoId,
    int SustitucionesRealizadas,
    IReadOnlyDictionary<PosicionCancha, Guid> PosicionesActuales,
    Guid JugadorAlSaqueId
);