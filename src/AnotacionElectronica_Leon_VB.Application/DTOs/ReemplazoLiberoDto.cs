using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record ReemplazoLiberoDto(
    Guid SetId,
    Guid EquipoId,
    Guid LiberoId,
    PosicionCancha Posicion // Debe ser posición zaguera (I, VI o V)
);