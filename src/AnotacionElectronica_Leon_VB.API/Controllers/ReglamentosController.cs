using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReglamentosController : ControllerBase
{
    private readonly IReglamentoService _reglamentoService;

    public ReglamentosController(IReglamentoService reglamentoService)
    {
        _reglamentoService = reglamentoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() =>
        Ok(await _reglamentoService.ObtenerReglamentosAsync());

    [HttpGet("{codigo}")]
    public async Task<IActionResult> ObtenerPorCodigo(string codigo)
    {
        var reg = await _reglamentoService.ObtenerPorCodigoAsync(codigo);
        if (reg is null)
            return NotFound(new { mensaje = $"Reglamento '{codigo}' no encontrado." });

        return Ok(reg);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearPerfilReglamentoDto dto)
    {
        try
        {
            var nuevo = await _reglamentoService.CrearReglamentoAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorCodigo), new { codigo = nuevo.CodigoReglamento }, nuevo);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
