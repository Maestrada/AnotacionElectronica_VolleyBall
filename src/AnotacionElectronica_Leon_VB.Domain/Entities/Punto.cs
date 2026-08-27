using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Punto
{
    public Guid Id { get; private set; }
    public Guid SetId { get; private set; }
    public Guid EquipoAnotadorId { get; private set; }
    public Guid? JugadorAnotadorId { get; private set; }
    public int PuntosEquipoLocalActual { get; private set; }
    public int PuntosEquipoVisitanteActual { get; private set; }
    public TipoAccionPunto TipoAccion { get; private set; }
    public DateTime HoraRegistro { get; private set; }

    private Punto() { }

    public Punto(Guid setId, Guid equipoAnotadorId, int puntosLocal, int puntosVisitante, TipoAccionPunto tipoAccion, Guid? jugadorAnotadorId = null)
    {
        Id = Guid.NewGuid();
        SetId = setId;
        EquipoAnotadorId = equipoAnotadorId;
        PuntosEquipoLocalActual = puntosLocal;
        PuntosEquipoVisitanteActual = puntosVisitante;
        TipoAccion = tipoAccion;
        JugadorAnotadorId = jugadorAnotadorId;
        HoraRegistro = DateTime.UtcNow;
    }
}