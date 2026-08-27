namespace AnotacionElectronica_Leon_VB.API.DTOs;

public record MarcadorEnVivoDto(
    Guid PartidoId,
    int NumeroSetActual,
    int PuntosLocal,
    int PuntosVisitante,
    int SetsGanadosLocal,
    int SetsGanadosVisitante,
    Guid? EquipoAlSaqueId
);

public record RotacionEnVivoDto(
    Guid PartidoId,
    Guid EquipoId,
    int[] Posiciones // Arreglo de 6 posiciones [I, II, III, IV, V, VI] con dorsales o IDs de jugadores
);