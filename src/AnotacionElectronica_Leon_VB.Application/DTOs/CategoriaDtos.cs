namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearCategoriaDto(string Nombre, string? Descripcion);
public record ActualizarCategoriaDto(string Nombre, string? Descripcion, bool Activa);
