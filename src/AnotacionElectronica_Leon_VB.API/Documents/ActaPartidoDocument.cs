using AnotacionElectronica_Leon_VB.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AnotacionElectronica_Leon_VB.API.Documents;

public sealed class ActaPartidoDocument(Partido partido, IReadOnlyList<EventoPartido> eventos) : IDocument
{
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public void Compose(IDocumentContainer container) => container.Page(page =>
    {
        page.Margin(22); page.Size(PageSizes.A4); page.DefaultTextStyle(x => x.FontSize(9));
        page.Header().Column(c => { c.Item().AlignCenter().Text("INTERNATIONAL SCORESHEET").Bold().FontSize(16); c.Item().AlignCenter().Text("Acta electrónica de voleibol · modelo estructural FIVB"); });
        page.Content().Column(c =>
        {
            c.Spacing(8);
            c.Item().Border(1).Padding(6).Row(r => { r.RelativeItem().Text($"Partido: {partido.Id}"); r.RelativeItem().Text($"Fecha: {partido.FechaProgramada:yyyy-MM-dd HH:mm}"); r.RelativeItem().Text($"Recinto: {partido.Lugar}"); });
            c.Item().Table(t => { t.ColumnsDefinition(x => { x.RelativeColumn(); x.ConstantColumn(55); x.ConstantColumn(55); x.RelativeColumn(); }); t.Header(h => { h.Cell().Text("EQUIPO").Bold(); h.Cell().Text("SETS").Bold(); h.Cell().Text("PUNTOS").Bold(); h.Cell().Text("EQUIPO").Bold(); }); t.Cell().Text(partido.EquipoLocal.Nombre); t.Cell().Text(partido.SetsGanadosLocal); t.Cell().Text(partido.Sets.Sum(s => s.PuntosLocal)); t.Cell().Text(partido.EquipoVisitante.Nombre); t.Cell().Text(partido.EquipoVisitante.Nombre); t.Cell().Text(partido.SetsGanadosVisitante); t.Cell().Text(partido.Sets.Sum(s => s.PuntosVisitante)); t.Cell().Text(partido.EquipoLocal.Nombre); });
            c.Item().Text("RESULTADOS POR SET").Bold();
            c.Item().Table(t => { t.ColumnsDefinition(x => { x.ConstantColumn(45); x.RelativeColumn(); x.RelativeColumn(); x.RelativeColumn(); }); t.Header(h => { h.Cell().Text("Set").Bold(); h.Cell().Text("Local").Bold(); h.Cell().Text("Visitante").Bold(); h.Cell().Text("Estado").Bold(); }); foreach (var s in partido.Sets.OrderBy(s => s.NumeroSet)) { t.Cell().Text(s.NumeroSet); t.Cell().Text(s.PuntosLocal); t.Cell().Text(s.PuntosVisitante); t.Cell().Text(s.Finalizado ? "Confirmado" : "Abierto"); } });
            c.Item().Text("INCIDENCIAS Y SECUENCIA DE EVENTOS").Bold();
            c.Item().Table(t => { t.ColumnsDefinition(x => { x.ConstantColumn(35); x.ConstantColumn(95); x.RelativeColumn(); }); t.Header(h => { h.Cell().Text("#").Bold(); h.Cell().Text("Hora UTC").Bold(); h.Cell().Text("Evento").Bold(); }); foreach (var e in eventos) { t.Cell().Text(e.Secuencia); t.Cell().Text(e.OcurrioEnUtc.ToString("HH:mm:ss")); t.Cell().Text($"{e.Tipo}: {e.DatosJson}"); } });
            c.Item().PaddingTop(20).Row(r => { r.RelativeItem().Text("Anotador: ____________________"); r.RelativeItem().Text("1.º Árbitro: ____________________"); r.RelativeItem().Text("Capitán: ____________________"); });
        });
        page.Footer().AlignCenter().Text($"Reglamento aplicado: {partido.CodigoReglamento} · Generado {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
    });
}
