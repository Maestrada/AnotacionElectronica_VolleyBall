namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record RegistrarSustitucionDto(
    Guid SetId,
    Guid EquipoId,
    Guid JugadorSaleId,
    Guid JugadorEntraId
);