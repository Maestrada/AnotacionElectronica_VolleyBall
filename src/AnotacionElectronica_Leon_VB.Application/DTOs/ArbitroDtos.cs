using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearArbitroDto(
    string Nombre,
    string Apellidos,
    RolArbitro Rol,
    string? NumeroLicencia = null,
    string? Federacion = null);

public record ArbitroDto(
    Guid Id,
    string Nombre,
    string Apellidos,
    string NombreCompleto,
    RolArbitro Rol,
    string RolTexto,
    string? NumeroLicencia,
    string? Federacion);
