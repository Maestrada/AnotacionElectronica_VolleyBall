using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JugadoresController : ControllerBase
{
    private readonly IEquipoService _equipoService;
    private readonly IWebHostEnvironment _environment;

    public JugadoresController(IEquipoService equipoService, IWebHostEnvironment environment)
    {
        _equipoService = equipoService;
        _environment = environment;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var jugador = await _equipoService.ObtenerJugadorPorIdAsync(id);
        return jugador is null ? NotFound(new { mensaje = "Jugador no encontrado." }) : Ok(jugador);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarJugadorDto dto)
    {
        try { return Ok(await _equipoService.ActualizarJugadorAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("{id:guid}/documentacion")]
    public async Task<IActionResult> ActualizarDocumentacion(Guid id, [FromBody] ActualizarDocumentacionJugadorDto dto)
    {
        try { return Ok(await _equipoService.ActualizarDocumentacionJugadorAsync(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPost("{id:guid}/foto")]
    public Task<IActionResult> SubirFoto(Guid id, IFormFile archivo) => SubirArchivo(id, archivo, "foto", [".jpg", ".jpeg", ".png", ".webp"], true);

    [HttpPost("{id:guid}/documento-identidad")]
    public Task<IActionResult> SubirDocumentoIdentidad(Guid id, IFormFile archivo) => SubirArchivo(id, archivo, "identidad", [".pdf", ".jpg", ".jpeg", ".png"], false);

    [HttpPost("{id:guid}/aval-foraneo")]
    public Task<IActionResult> SubirAvalForaneo(Guid id, IFormFile archivo) => SubirArchivo(id, archivo, "aval", [".pdf", ".jpg", ".jpeg", ".png"], false);

    private async Task<IActionResult> SubirArchivo(Guid id, IFormFile? archivo, string tipo, string[] extensionesPermitidas, bool esFoto)
    {
        if (archivo is null || archivo.Length == 0) return BadRequest(new { mensaje = "Debes seleccionar un archivo." });
        if (archivo.Length > 10 * 1024 * 1024) return BadRequest(new { mensaje = "El archivo no puede superar 10 MB." });
        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (!extensionesPermitidas.Contains(extension)) return BadRequest(new { mensaje = "Tipo de archivo no permitido." });

        var folder = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "jugadores", id.ToString());
        Directory.CreateDirectory(folder);
        var filename = $"{tipo}-{Guid.NewGuid():N}{extension}";
        await using (var stream = System.IO.File.Create(Path.Combine(folder, filename))) await archivo.CopyToAsync(stream);
        var url = $"{Request.Scheme}://{Request.Host}/uploads/jugadores/{id}/{filename}";
        try
        {
            var actual = await _equipoService.ObtenerJugadorPorIdAsync(id);
            if (actual is null) return NotFound(new { mensaje = "Jugador no encontrado." });
            var jugador = esFoto
                ? await _equipoService.ActualizarImagenJugadorAsync(id, url)
                : await _equipoService.ActualizarDocumentacionJugadorAsync(id, new ActualizarDocumentacionJugadorDto(
                    actual.EsJugadorForaneo,
                    tipo == "identidad" ? url : actual.DocumentoIdentidadUrl,
                    tipo == "aval" ? url : actual.DocumentoAvalForaneoUrl));
            return Ok(jugador);
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        try { await _equipoService.EliminarJugadorAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
    }
}
