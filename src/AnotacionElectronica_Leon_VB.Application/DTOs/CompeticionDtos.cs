namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearCompeticionDto(
    string Nombre,
    string Edicion,
    string Categoria,
    string Rama,
    string? Organizador = null,
    string? SedePrincipal = null);

public record CompeticionDto(
    Guid Id,
    string Nombre,
    string Edicion,
    string Categoria,
    string Rama,
    string? Organizador,
    string? SedePrincipal);
