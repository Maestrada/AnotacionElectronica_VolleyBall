using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearEquipoDto(
    string Nombre,
    string NombreEntrenador,
    string Categoria,
    string? NombreAsistente = null);
public record ActualizarEquipoDto(string Nombre, string NombreEntrenador, string Categoria, string? NombreAsistente = null);

public record JugadorDto(
    Guid Id,
    string Nombre,
    string Apellidos,
    int NumeroCamiseta,
    PosicionJugador Posicion,
    string PosicionTexto,
    bool EsCapitan,
    Guid EquipoId,
    string? NombreEquipo,
    string? ImagenUrl,
    bool EsJugadorForaneo,
    string? DocumentoIdentidadUrl,
    string? DocumentoAvalForaneoUrl);

public record CrearJugadorDto(
    string Nombre,
    string Apellidos,
    int NumeroCamiseta,
    PosicionJugador Posicion,
    Guid EquipoId,
    bool EsCapitan = false);

public record ActualizarJugadorDto(string Nombre, string Apellidos, int NumeroCamiseta,
    PosicionJugador Posicion, bool EsCapitan = false);
public record ActualizarDocumentacionJugadorDto(bool EsJugadorForaneo, string? DocumentoIdentidadUrl, string? DocumentoAvalForaneoUrl);

public record EquipoDto(
    Guid Id,
    string Nombre,
    string NombreEntrenador,
    string? NombreAsistente,
    string Categoria,
    int TotalJugadores,
    IEnumerable<JugadorDto> Jugadores);
