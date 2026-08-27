using AnotacionElectronica_Leon_VB.API.DTOs;
using AnotacionElectronica_Leon_VB.API.Hubs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnotacionController : ControllerBase
{
    private readonly IPartidoService _partidoService;
    private readonly IPartidoRepository _partidoRepository;
    private readonly IHubContext<PartidoHub, IPartidoHubClient> _hubContext;

    public AnotacionController(
        IPartidoService partidoService,
        IPartidoRepository partidoRepository,
        IHubContext<PartidoHub, IPartidoHubClient> hubContext)
    {
        _partidoService = partidoService;
        _partidoRepository = partidoRepository;
        _hubContext = hubContext;
    }

    [HttpPost("puntos/registrar")]
    // Usamos la ruta explícita hacia Application.DTOs para evitar la ambigüedad (Error CS0104)
    public async Task<IActionResult> RegistrarPunto([FromBody] AnotacionElectronica_Leon_VB.Application.DTOs.RegistrarPuntoDto dto)
    {
        try
        {
            var marcador = await _partidoService.RegistrarPuntoAsync(dto);

            var partido = await _partidoRepository.GetPartidoConDetallesAsync(dto.PartidoId);
            if (partido is null) return NotFound();

            var setActual = partido.Sets.OrderByDescending(s => s.NumeroSet).FirstOrDefault();

            var payloadMarcador = new MarcadorEnVivoDto(
                partido.Id,
                setActual?.NumeroSet ?? 1,
                setActual?.PuntosLocal ?? 0,
                setActual?.PuntosVisitante ?? 0,
                partido.SetsGanadosLocal,
                partido.SetsGanadosVisitante,
                marcador.UltimoEquipoAlSaqueId
            );

            await _hubContext.Clients
                .Group(dto.PartidoId.ToString())
                .RecibirMarcadorActualizado(payloadMarcador);

            return Ok(new
            {
                mensaje = marcador.PendienteCambioCancha ? "Cambio de cancha pendiente de confirmación."
                    : marcador.PendienteConfirmacionCierre ? "Set pendiente de confirmación." : "Punto registrado y transmitido.",
                marcador = payloadMarcador,
                pendienteConfirmacionCierre = marcador.PendienteConfirmacionCierre,
                pendienteCambioCancha = marcador.PendienteCambioCancha
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("partidos/{partidoId:guid}/deshacer")]
    public async Task<IActionResult> DeshacerUltimoPunto(Guid partidoId)
    {
        try
        {
            return Ok(await _partidoService.DeshacerUltimoPuntoAsync(partidoId));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("partidos/{partidoId:guid}/sets/confirmar-cierre")]
    public async Task<IActionResult> ConfirmarCierreSet(Guid partidoId)
    {
        try
        {
            return Ok(await _partidoService.ConfirmarCierreSetAsync(partidoId));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("partidos/{partidoId:guid}/sets/confirmar-cambio-cancha")]
    public async Task<IActionResult> ConfirmarCambioCancha(Guid partidoId)
    {
        try
        {
            return Ok(await _partidoService.ConfirmarCambioCanchaAsync(partidoId));
        }
        catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("partidos/{partidoId:guid}/eventos")]
    public async Task<IActionResult> ObtenerEventos(Guid partidoId) =>
        Ok(await _partidoService.ObtenerEventosAsync(partidoId));
}
