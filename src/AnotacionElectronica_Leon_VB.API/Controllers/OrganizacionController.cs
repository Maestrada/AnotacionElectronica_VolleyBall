using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/organizacion")]
public class OrganizacionController : ControllerBase
{
    private readonly IRepository<FaseCompeticion> _fases;
    private readonly IRepository<Recinto> _recintos;
    private readonly IRepository<DesignacionArbitral> _designaciones;
    private readonly IRepository<Arbitro> _arbitros;
    private readonly IRepository<Partido> _partidos;
    private readonly IUnitOfWork _uow;

    public OrganizacionController(IRepository<FaseCompeticion> fases, IRepository<Recinto> recintos, IRepository<DesignacionArbitral> designaciones, IRepository<Arbitro> arbitros, IRepository<Partido> partidos, IUnitOfWork uow)
        => (_fases, _recintos, _designaciones, _arbitros, _partidos, _uow) = (fases, recintos, designaciones, arbitros, partidos, uow);

    [HttpGet("fases")] public async Task<IActionResult> Fases() => Ok(await _fases.GetAllAsync());
    [HttpPost("fases")] public async Task<IActionResult> CrearFase(CrearFaseDto dto) { try { var e = new FaseCompeticion(dto.CompeticionId, dto.Nombre, dto.Orden, dto.CodigoReglamento); await _fases.AddAsync(e); await _uow.SaveChangesAsync(); return Ok(e); } catch (ArgumentException e) { return BadRequest(new { mensaje = e.Message }); } }
    [HttpDelete("fases/{id:guid}")] public async Task<IActionResult> EliminarFase(Guid id) { var e = await _fases.GetByIdAsync(id); if (e is null) return NotFound(); _fases.Delete(e); await _uow.SaveChangesAsync(); return NoContent(); }

    [HttpGet("recintos")] public async Task<IActionResult> Recintos() => Ok(await _recintos.GetAllAsync());
    [HttpPost("recintos")] public async Task<IActionResult> CrearRecinto(CrearRecintoDto dto) { try { var e = new Recinto(dto.Nombre, dto.Ciudad, dto.Direccion); await _recintos.AddAsync(e); await _uow.SaveChangesAsync(); return Ok(e); } catch (ArgumentException e) { return BadRequest(new { mensaje = e.Message }); } }
    [HttpDelete("recintos/{id:guid}")] public async Task<IActionResult> EliminarRecinto(Guid id) { var e = await _recintos.GetByIdAsync(id); if (e is null) return NotFound(); _recintos.Delete(e); await _uow.SaveChangesAsync(); return NoContent(); }

    [HttpGet("partidos/{partidoId:guid}/designaciones")] public async Task<IActionResult> Designaciones(Guid partidoId) => Ok((await _designaciones.GetAllAsync()).Where(x => x.PartidoId == partidoId));
    [HttpPost("designaciones")] public async Task<IActionResult> CrearDesignacion(CrearDesignacionArbitralDto dto)
    { var partido = await _partidos.GetByIdAsync(dto.PartidoId); var arbitro = await _arbitros.GetByIdAsync(dto.ArbitroId); if (partido is null || arbitro is null) return NotFound(new { mensaje = "Partido o árbitro no encontrado." }); if ((await _designaciones.GetAllAsync()).Any(x => x.PartidoId == dto.PartidoId && x.Rol == dto.Rol)) return BadRequest(new { mensaje = "Ya existe una designación para ese rol." }); var e = new DesignacionArbitral(dto.PartidoId, arbitro, dto.Rol); await _designaciones.AddAsync(e); await _uow.SaveChangesAsync(); return Ok(e); }
    [HttpDelete("designaciones/{id:guid}")] public async Task<IActionResult> EliminarDesignacion(Guid id) { var e = await _designaciones.GetByIdAsync(id); if (e is null) return NotFound(); _designaciones.Delete(e); await _uow.SaveChangesAsync(); return NoContent(); }
}
