using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArbitrosController : ControllerBase
{
    private readonly IArbitroService _arbitroService;

    public ArbitrosController(IArbitroService arbitroService)
    {
        _arbitroService = arbitroService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() =>
        Ok(await _arbitroService.ObtenerArbitrosAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var arbitro = await _arbitroService.ObtenerPorIdAsync(id);
        if (arbitro is null)
            return NotFound(new { mensaje = $"Árbitro con ID {id} no encontrado." });

        return Ok(arbitro);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearArbitroDto dto)
    {
        try
        {
            var nuevo = await _arbitroService.CrearArbitroAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevo.Id }, nuevo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarArbitroDto dto)
    {
        try { return Ok(await _arbitroService.ActualizarArbitroAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        try { await _arbitroService.EliminarArbitroAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
    }
}
