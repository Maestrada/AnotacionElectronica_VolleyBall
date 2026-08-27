using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class SustitucionReglamentaria
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AlineacionSetId { get; private set; }
    public Guid JugadorSaleId { get; private set; }
    public Guid JugadorEntraId { get; private set; }
    public PosicionCancha Posicion { get; private set; }
    public int PuntosLocalEnMomento { get; private set; }
    public int PuntosVisitanteEnMomento { get; private set; }
    public DateTime FechaHora { get; private set; } = DateTime.UtcNow;

    protected SustitucionReglamentaria() { }

    public SustitucionReglamentaria(
        Guid alineacionSetId,
        Guid saleId,
        Guid entraId,
        PosicionCancha posicion,
        int puntosLocal,
        int puntosVisitante)
    {
        AlineacionSetId = alineacionSetId;
        JugadorSaleId = saleId;
        JugadorEntraId = entraId;
        Posicion = posicion;
        PuntosLocalEnMomento = puntosLocal;
        PuntosVisitanteEnMomento = puntosVisitante;
    }
}