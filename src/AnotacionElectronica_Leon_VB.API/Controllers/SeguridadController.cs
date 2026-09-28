using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/seguridad")]
public class SeguridadController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<UsuarioSistema> _passwordHasher;
    public SeguridadController(ApplicationDbContext db, IPasswordHasher<UsuarioSistema> passwordHasher) => (_db, _passwordHasher) = (db, passwordHasher);

    [HttpGet("usuarios")]
    public async Task<IActionResult> Usuarios() => Ok(await _db.UsuariosSistema.AsNoTracking().OrderBy(x => x.NombreUsuario)
        .Select(x => new { x.Id, x.NombreUsuario, x.NombreCompleto, x.CorreoElectronico, x.Activo, x.FechaCreacionUtc, x.ArbitroId, Roles = x.Roles.Select(r => new { r.RolId, r.Rol.Nombre }) }).ToListAsync());

    [HttpPost("usuarios")]
    public async Task<IActionResult> CrearUsuario(CrearUsuarioSistemaDto dto)
    {
        if (await UsuarioDuplicado(dto.NombreUsuario, dto.CorreoElectronico)) return Conflict(new { mensaje = "El usuario o correo ya existe." });
        try
        {
            var usuario = new UsuarioSistema(dto.NombreUsuario, dto.NombreCompleto, dto.CorreoElectronico, "pendiente");
            usuario.EstablecerHashContrasena(_passwordHasher.HashPassword(usuario, dto.Contrasena));
            await ValidarArbitroDisponible(dto.ArbitroId); if (dto.ArbitroId.HasValue) usuario.VincularArbitro(dto.ArbitroId.Value);
            await AsignarRoles(usuario, dto.RolesIds);
            _db.UsuariosSistema.Add(usuario); await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Usuarios), new { id = usuario.Id }, new { usuario.Id, usuario.NombreUsuario, usuario.NombreCompleto, usuario.CorreoElectronico, usuario.Activo, usuario.ArbitroId });
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("usuarios/{id:guid}")]
    public async Task<IActionResult> ActualizarUsuario(Guid id, ActualizarUsuarioSistemaDto dto)
    {
        var usuario = await _db.UsuariosSistema.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
        if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });
        if (await UsuarioDuplicado(dto.NombreUsuario, dto.CorreoElectronico, id)) return Conflict(new { mensaje = "El usuario o correo ya existe." });
        try
        {
            usuario.Actualizar(dto.NombreUsuario, dto.NombreCompleto, dto.CorreoElectronico); if (dto.Activo) usuario.Activar(); else usuario.Desactivar();
            _db.UsuariosRoles.RemoveRange(usuario.Roles); await AsignarRoles(usuario, dto.RolesIds);
            await _db.SaveChangesAsync(); return Ok(new { usuario.Id, usuario.NombreUsuario, usuario.NombreCompleto, usuario.CorreoElectronico, usuario.Activo, usuario.ArbitroId });
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("usuarios/{id:guid}/contrasena")]
    public async Task<IActionResult> CambiarContrasena(Guid id, [FromBody] string contrasena)
    {
        var usuario = await _db.UsuariosSistema.FindAsync(id); if (usuario is null) return NotFound();
        try { usuario.EstablecerHashContrasena(_passwordHasher.HashPassword(usuario, contrasena)); await _db.SaveChangesAsync(); return NoContent(); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("usuarios/{id:guid}/arbitro")]
    public async Task<IActionResult> VincularArbitro(Guid id, VincularArbitroUsuarioDto dto)
    {
        var usuario = await _db.UsuariosSistema.FindAsync(id); if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });
        try
        {
            await ValidarArbitroDisponible(dto.ArbitroId, id);
            if (dto.ArbitroId.HasValue) usuario.VincularArbitro(dto.ArbitroId.Value); else usuario.DesvincularArbitro();
            await _db.SaveChangesAsync(); return Ok(new { usuario.Id, usuario.ArbitroId });
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpGet("roles")]
    public async Task<IActionResult> Roles() => Ok(await _db.RolesSistema.AsNoTracking().OrderBy(x => x.Nombre)
        .Select(x => new { x.Id, x.Nombre, x.Descripcion, x.Activo, Permisos = x.Permisos.Select(p => new { p.PermisoId, p.Permiso.Codigo, p.Permiso.Descripcion }) }).ToListAsync());

    [HttpPost("roles")]
    public async Task<IActionResult> CrearRol(CrearRolSistemaDto dto)
    {
        if (await _db.RolesSistema.AnyAsync(x => x.Nombre == dto.Nombre.Trim())) return Conflict(new { mensaje = "El rol ya existe." });
        try { var rol = new RolSistema(dto.Nombre, dto.Descripcion); await AsignarPermisos(rol, dto.PermisosIds); _db.RolesSistema.Add(rol); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(Roles), new { id = rol.Id }, rol); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("roles/{id:guid}")]
    public async Task<IActionResult> ActualizarRol(Guid id, ActualizarRolSistemaDto dto)
    {
        var rol = await _db.RolesSistema.Include(x => x.Permisos).FirstOrDefaultAsync(x => x.Id == id); if (rol is null) return NotFound();
        if (await _db.RolesSistema.AnyAsync(x => x.Id != id && x.Nombre == dto.Nombre.Trim())) return Conflict(new { mensaje = "El rol ya existe." });
        try { rol.Actualizar(dto.Nombre, dto.Descripcion); _db.RolesPermisos.RemoveRange(rol.Permisos); await AsignarPermisos(rol, dto.PermisosIds); await _db.SaveChangesAsync(); return Ok(rol); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpGet("permisos")]
    public async Task<IActionResult> Permisos() => Ok(await _db.PermisosSistema.AsNoTracking().OrderBy(x => x.Codigo).ToListAsync());

    [HttpPost("permisos")]
    public async Task<IActionResult> CrearPermiso(CrearPermisoSistemaDto dto)
    {
        try { var permiso = new PermisoSistema(dto.Codigo, dto.Descripcion); if (await _db.PermisosSistema.AnyAsync(x => x.Codigo == permiso.Codigo)) return Conflict(new { mensaje = "El permiso ya existe." }); _db.PermisosSistema.Add(permiso); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(Permisos), new { id = permiso.Id }, permiso); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpGet("bitacora")]
    public async Task<IActionResult> Bitacora([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] Guid? usuarioId, [FromQuery] int limite = 100)
    {
        var query = _db.BitacoraAcciones.AsNoTracking().AsQueryable();
        if (desde.HasValue) query = query.Where(x => x.FechaUtc >= desde.Value.ToUniversalTime());
        if (hasta.HasValue) query = query.Where(x => x.FechaUtc <= hasta.Value.ToUniversalTime());
        if (usuarioId.HasValue) query = query.Where(x => x.UsuarioId == usuarioId.Value);
        return Ok(await query.OrderByDescending(x => x.FechaUtc).Take(Math.Clamp(limite, 1, 500)).ToListAsync());
    }

    private async Task<bool> UsuarioDuplicado(string usuario, string correo, Guid? ignorarId = null) => await _db.UsuariosSistema.AnyAsync(x => x.Id != ignorarId && (x.NombreUsuario == usuario.Trim() || x.CorreoElectronico == correo.Trim().ToLower()));
    private async Task AsignarRoles(UsuarioSistema usuario, IReadOnlyCollection<Guid>? rolesIds)
    {
        var ids = rolesIds?.Distinct().ToArray() ?? []; if (ids.Length == 0) return;
        if (await _db.RolesSistema.CountAsync(x => ids.Contains(x.Id)) != ids.Length) throw new ArgumentException("Uno o más roles no existen.");
        foreach (var id in ids) usuario.Roles.Add(new UsuarioRol(usuario.Id, id));
    }
    private async Task AsignarPermisos(RolSistema rol, IReadOnlyCollection<Guid>? permisosIds)
    {
        var ids = permisosIds?.Distinct().ToArray() ?? []; if (ids.Length == 0) return;
        if (await _db.PermisosSistema.CountAsync(x => ids.Contains(x.Id)) != ids.Length) throw new ArgumentException("Uno o más permisos no existen.");
        foreach (var id in ids) rol.Permisos.Add(new RolPermiso(rol.Id, id));
    }
    private async Task ValidarArbitroDisponible(Guid? arbitroId, Guid? usuarioId = null)
    {
        if (!arbitroId.HasValue) return;
        if (!await _db.Arbitros.AnyAsync(x => x.Id == arbitroId.Value)) throw new ArgumentException("Árbitro no encontrado.");
        if (await _db.UsuariosSistema.AnyAsync(x => x.ArbitroId == arbitroId.Value && x.Id != usuarioId)) throw new ArgumentException("El árbitro ya está vinculado a otro usuario.");
    }
}
