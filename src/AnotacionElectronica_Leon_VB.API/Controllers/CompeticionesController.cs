using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompeticionesController : ControllerBase
{
    private readonly ICompeticionService _competicionService;

    public CompeticionesController(ICompeticionService competicionService)
    {
        _competicionService = competicionService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() =>
        Ok(await _competicionService.ObtenerCompeticionesAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var comp = await _competicionService.ObtenerPorIdAsync(id);
        if (comp is null)
            return NotFound(new { mensaje = $"Competición con ID {id} no encontrada." });

        return Ok(comp);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCompeticionDto dto)
    {
        try
        {
            var nuevo = await _competicionService.CrearCompeticionAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevo.Id }, nuevo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, ActualizarCompeticionDto dto)
    { try { return Ok(await _competicionService.ActualizarCompeticionAsync(id, dto)); } catch (KeyNotFoundException e) { return NotFound(new { mensaje = e.Message }); } catch (ArgumentException e) { return BadRequest(new { mensaje = e.Message }); } }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    { try { await _competicionService.EliminarCompeticionAsync(id); return NoContent(); } catch (KeyNotFoundException e) { return NotFound(new { mensaje = e.Message }); } }
}
