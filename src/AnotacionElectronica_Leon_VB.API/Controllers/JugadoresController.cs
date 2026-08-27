using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JugadoresController : ControllerBase
{
    private readonly IEquipoService _equipoService;

    public JugadoresController(IEquipoService equipoService)
    {
        _equipoService = equipoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] Guid? equipoId) =>
        Ok(await _equipoService.ObtenerJugadoresAsync(equipoId));

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearJugadorDto dto)
    {
        try
        {
            var nuevoJugador = await _equipoService.AgregarJugadorAsync(dto);
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
