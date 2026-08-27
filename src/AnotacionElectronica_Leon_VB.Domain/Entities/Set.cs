using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Set
{
    public Guid? UltimoEquipoAlSaqueId { get; set; }
    public Guid Id { get; private set; }
    public Guid PartidoId { get; private set; }
    public int NumeroSet { get; private set; } // 1, 2, 3, 4, 5
    public int PuntosLocal { get; private set; }
    public int PuntosVisitante { get; private set; }
    public Guid? EquipoGanadorId { get; private set; }
    public bool Finalizado { get; private set; }
    public bool PendienteConfirmacionCierre { get; private set; }
    public bool EsSetDecisivo { get; private set; }
    public int PuntosParaGanar { get; private set; }
    public int DiferenciaMinima { get; private set; }
    public int PuntoCambioCancha { get; private set; }
    public bool PendienteCambioCancha { get; private set; }
    public bool CambioCanchaConfirmado { get; private set; }

    private readonly List<Punto> _puntos = new();
    public IReadOnlyCollection<Punto> Puntos => _puntos.AsReadOnly();

    private readonly List<Rotacion> _rotaciones = new();
    public IReadOnlyCollection<Rotacion> Rotaciones => _rotaciones.AsReadOnly();

    private Set() { }

    public Set(Guid partidoId, int numeroSet, bool esSetDecisivo = false, int puntosParaGanar = 25,
        int diferenciaMinima = 2, int puntoCambioCancha = 8)
    {
        Id = Guid.NewGuid();
        PartidoId = partidoId;
        NumeroSet = numeroSet;
        PuntosLocal = 0;
        PuntosVisitante = 0;
        Finalizado = false;
        EsSetDecisivo = esSetDecisivo;
        PuntosParaGanar = puntosParaGanar;
        DiferenciaMinima = diferenciaMinima;
        PuntoCambioCancha = puntoCambioCancha;
    }

    public void AnotarPunto(Guid equipoAnotadorId, Guid equipoLocalId, Guid equipoVisitanteId, TipoAccionPunto tipoAccion, Guid? jugadorId = null)
    {
        if (Finalizado || PendienteConfirmacionCierre || PendienteCambioCancha)
            throw new InvalidOperationException("El set está cerrado, pendiente de cierre o de cambio de cancha.");

        if (equipoAnotadorId == equipoLocalId)
            PuntosLocal++;
        else if (equipoAnotadorId == equipoVisitanteId)
            PuntosVisitante++;

        var punto = new Punto(Id, equipoAnotadorId, PuntosLocal, PuntosVisitante, tipoAccion, jugadorId);
        _puntos.Add(punto);

        VerificarCierrePendiente();
        VerificarCambioCanchaPendiente();
    }

    public Punto DeshacerUltimoPunto()
    {
        if (Finalizado || _puntos.Count == 0)
            throw new InvalidOperationException("No hay un punto que pueda deshacerse.");

        var punto = _puntos[^1];
        _puntos.RemoveAt(_puntos.Count - 1);
        PuntosLocal = _puntos.LastOrDefault()?.PuntosEquipoLocalActual ?? 0;
        PuntosVisitante = _puntos.LastOrDefault()?.PuntosEquipoVisitanteActual ?? 0;
        UltimoEquipoAlSaqueId = _puntos.LastOrDefault()?.EquipoAnotadorId;
        PendienteConfirmacionCierre = false;
        PendienteCambioCancha = false;
        return punto;
    }

    public void ConfirmarCambioCancha()
    {
        if (!PendienteCambioCancha)
            throw new InvalidOperationException("El set no tiene un cambio de cancha pendiente.");

        PendienteCambioCancha = false;
        CambioCanchaConfirmado = true;
    }

    public void ConfirmarCierre(Guid equipoLocalId, Guid equipoVisitanteId)
    {
        if (!PendienteConfirmacionCierre)
            throw new InvalidOperationException("El set todavía no alcanza un resultado reglamentario para cerrarse.");

        Finalizado = true;
        PendienteConfirmacionCierre = false;
        EquipoGanadorId = PuntosLocal > PuntosVisitante ? equipoLocalId : equipoVisitanteId;
    }

    private void VerificarCierrePendiente()
    {
        if ((PuntosLocal >= PuntosParaGanar || PuntosVisitante >= PuntosParaGanar) && Math.Abs(PuntosLocal - PuntosVisitante) >= DiferenciaMinima)
            PendienteConfirmacionCierre = true;
    }

    private void VerificarCambioCanchaPendiente()
    {
        if (EsSetDecisivo && !CambioCanchaConfirmado &&
            Math.Max(PuntosLocal, PuntosVisitante) >= PuntoCambioCancha)
            PendienteCambioCancha = true;
    }
}
