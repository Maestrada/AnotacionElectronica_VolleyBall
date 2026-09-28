using AnotacionElectronica_Leon_VB.API.Documents;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController, Route("api/partidos/{partidoId:guid}/exportaciones")]
public class ExportacionesController(IPartidoRepository partidos) : ControllerBase
{
    [HttpGet("acta.pdf")]
    public async Task<IActionResult> Acta(Guid partidoId)
    {
        var partido = await partidos.GetPartidoConDetallesAsync(partidoId);
        if (partido is null) return NotFound(new { mensaje = "Partido no encontrado." });
        if (partido.Estado != EstadoPartido.Finalizado) return BadRequest(new { mensaje = "El acta PDF solo puede exportarse cuando el partido esté finalizado." });
        var pdf = new ActaPartidoDocument(partido, await partidos.ObtenerEventosAsync(partidoId)).GeneratePdf();
        return File(pdf, "application/pdf", $"acta-{partido.Id}.pdf");
    }
}
