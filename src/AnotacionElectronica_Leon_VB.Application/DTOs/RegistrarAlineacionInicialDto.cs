namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record RegistrarAlineacionInicialDto(
    Guid SetId,
    Guid EquipoId,
    Guid JugadorPosicion1Id, // Zaguero derecho (al saque inicial si corresponde)
    Guid JugadorPosicion2Id, // Delantero derecho
    Guid JugadorPosicion3Id, // Delantero centro
    Guid JugadorPosicion4Id, // Delantero izquierdo
    Guid JugadorPosicion5Id, // Zaguero izquierdo
    Guid JugadorPosicion6Id  // Zaguero centro
);