namespace AnotacionElectronica_Leon_VB.Application.DTOs;

public record CrearUsuarioSistemaDto(string NombreUsuario, string NombreCompleto, string CorreoElectronico, string Contrasena, IReadOnlyCollection<Guid>? RolesIds, Guid? ArbitroId = null);
public record ActualizarUsuarioSistemaDto(string NombreUsuario, string NombreCompleto, string CorreoElectronico, bool Activo, IReadOnlyCollection<Guid>? RolesIds);
public record CrearRolSistemaDto(string Nombre, string? Descripcion, IReadOnlyCollection<Guid>? PermisosIds);
public record ActualizarRolSistemaDto(string Nombre, string? Descripcion, IReadOnlyCollection<Guid>? PermisosIds);
public record CrearPermisoSistemaDto(string Codigo, string Descripcion);
public record VincularArbitroUsuarioDto(Guid? ArbitroId);
