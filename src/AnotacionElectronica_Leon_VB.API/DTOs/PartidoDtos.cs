// src/AnotacionElectronica_Leon_VB.API/DTOs/PartidoDtos.cs
using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.API.DTOs;

public record CrearPartidoDto(
    int EquipoLocalId,
    int EquipoVisitanteId,
    string Categoria,
    string Rama, // "Femenino", "Masculino"
    string Sede,
    DateTime FechaHora
);

public record RegistrarPuntoDto(
    int SetId,
    int EquipoPuntoId,
    int? JugadorAnotadorId,
    string TipoAccion, // "Ataque", "Bloqueo", "Ace", "ErrorRival"
    int? JugadorErrorId,
    string? TipoError  // "Red", "Fuera", "ToqueMalla", "FaltaRotacion"
);

public record IniciarSetDto(
    int PartidoId,
    int NumeroSet,
    int[] FormacionInicialLocalIds,     // 6 IDs de jugadores (posiciones 1 a 6)
    int[] FormacionInicialVisitanteIds
);

public record CrearPartidoRequestDto(
    Guid EquipoLocalId,
    Guid EquipoVisitanteId,
    DateTime FechaProgramada,
    string Lugar
);

public record RegistrarPuntoRequestDto(
    Guid PartidoId,
    Guid EquipoAnotadorId,
    TipoAccionPunto TipoAccion,
    Guid? JugadorAnotadorId
);