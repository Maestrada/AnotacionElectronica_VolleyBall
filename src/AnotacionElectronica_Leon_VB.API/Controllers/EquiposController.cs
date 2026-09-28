using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly IEquipoService _equipoService;

    public EquiposController(IEquipoService equipoService)
    {
        _equipoService = equipoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() =>
        Ok(await _equipoService.ObtenerEquiposAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var equipo = await _equipoService.ObtenerEquipoPorIdAsync(id);
        if (equipo is null)
            return NotFound(new { mensaje = $"Equipo con ID {id} no encontrado." });

        return Ok(equipo);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEquipoDto dto)
    {
        try
        {
            var nuevo = await _equipoService.CrearEquipoAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevo.Id }, nuevo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, ActualizarEquipoDto dto)
    { try { return Ok(await _equipoService.ActualizarEquipoAsync(id, dto)); } catch (KeyNotFoundException e) { return NotFound(new { mensaje = e.Message }); } catch (ArgumentException e) { return BadRequest(new { mensaje = e.Message }); } }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    { try { await _equipoService.EliminarEquipoAsync(id); return NoContent(); } catch (KeyNotFoundException e) { return NotFound(new { mensaje = e.Message }); } catch (InvalidOperationException e) { return BadRequest(new { mensaje = e.Message }); } }

    [HttpGet("{id:guid}/jugadores")]
    public async Task<IActionResult> ObtenerJugadores(Guid id) =>
        Ok(await _equipoService.ObtenerJugadoresAsync(id));

    [HttpPost("{id:guid}/jugadores")]
    public async Task<IActionResult> AgregarJugador(Guid id, [FromBody] CrearJugadorDto dto)
    {
        try
        {
            var dtoConEquipo = dto with { EquipoId = id };
            var nuevoJugador = await _equipoService.AgregarJugadorAsync(dtoConEquipo);
            return Ok(nuevoJugador);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
