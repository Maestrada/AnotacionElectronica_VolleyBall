using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartidosController : ControllerBase
{
    private readonly IPartidoRepository _partidoRepository;
    private readonly IPartidoService _partidoService;

    public PartidosController(IPartidoRepository partidoRepository, IPartidoService partidoService)
    {
        _partidoRepository = partidoRepository;
        _partidoService = partidoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var partidos = await _partidoRepository.GetAllAsync();
        return Ok(partidos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var partido = await _partidoService.GetPartidoByIdAsync(id);
        if (partido is null)
            return NotFound(new { mensaje = $"Partido con ID {id} no encontrado." });

        return Ok(partido);
    }

    [HttpPost]
    public async Task<IActionResult> CrearPartido([FromBody] CrearPartidoDto dto)
    {
        var nuevoPartido = await _partidoService.CrearPartidoAsync(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoPartido.Id }, nuevoPartido);
    }

    [HttpPost("{id:guid}/iniciar")]
    public async Task<IActionResult> IniciarPartido(Guid id)
    {
        try
        {
            await _partidoService.IniciarPartidoAsync(id);
            return Ok(new { mensaje = "Partido iniciado con éxito." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{id:guid}/acta")]
    public async Task<IActionResult> ObtenerActa(Guid id)
    {
        try
        {
            return Ok(await _partidoService.ObtenerResumenPartidoAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id:guid}/sets/{numeroSet:int}/iniciar")]
    public async Task<IActionResult> IniciarSet(Guid id, int numeroSet)
    {
        try
        {
            return Ok(await _partidoService.IniciarSetAsync(id, numeroSet));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
