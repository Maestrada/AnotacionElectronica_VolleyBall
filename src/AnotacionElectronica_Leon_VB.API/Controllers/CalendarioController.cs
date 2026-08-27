using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/calendario")]
public sealed class CalendarioController : ControllerBase
{
    private readonly ICalendarioService _calendario;

    public CalendarioController(ICalendarioService calendario) => _calendario = calendario;

    [HttpGet]
    public async Task<IActionResult> Obtener([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta) =>
        Ok(await _calendario.ObtenerAsync(desde, hasta));

    [HttpPost("juegos")]
    public async Task<IActionResult> Crear([FromBody] CrearJuegoCalendarioDto dto)
    {
        try
        {
            var juego = await _calendario.CrearAsync(dto);
            return CreatedAtAction(nameof(Obtener), new { }, juego);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("juegos/{id:guid}/crear-partido")]
    public async Task<IActionResult> CrearPartido(Guid id)
    {
        try
        {
            return Ok(await _calendario.CrearPartidoDesdeCalendarioAsync(id));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
