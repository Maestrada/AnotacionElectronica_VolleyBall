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
