using AnotacionElectronica_Leon_VB.Domain.Enums;
namespace AnotacionElectronica_Leon_VB.Application.DTOs;
public record CrearFaseDto(Guid CompeticionId, string Nombre, int Orden, string CodigoReglamento);
public record CrearRecintoDto(string Nombre, string Ciudad, string? Direccion);
public record CrearDesignacionArbitralDto(Guid PartidoId, Guid ArbitroId, RolArbitro Rol);
