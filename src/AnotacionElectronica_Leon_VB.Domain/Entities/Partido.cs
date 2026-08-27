using AnotacionElectronica_Leon_VB.Domain.Enums;
using AnotacionElectronica_Leon_VB.Domain.ValueObjects;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Partido
{
    public Guid Id { get; private set; }
    public Guid EquipoLocalId { get; private set; }
    public Equipo EquipoLocal { get; private set; } = null!;
    public Guid EquipoVisitanteId { get; private set; }
    public Equipo EquipoVisitante { get; private set; } = null!;
    
    public DateTime FechaProgramada { get; private set; }
    public string Lugar { get; private set; } = string.Empty;
    public EstadoPartido Estado { get; private set; }
    
    public int SetsGanadosLocal { get; private set; }
    public int SetsGanadosVisitante { get; private set; }
    public Guid? EquipoGanadorId { get; private set; }
    public string CodigoReglamento { get; private set; } = string.Empty;
    public int MaximoSets { get; private set; }
    public int SetsParaGanar { get; private set; }
    public int PuntosSetRegular { get; private set; }
    public int PuntosSetDecisivo { get; private set; }
    public int DiferenciaMinima { get; private set; }
    public int PuntoCambioCanchaSetDecisivo { get; private set; }

    private readonly List<Set> _sets = new();
    public IReadOnlyCollection<Set> Sets => _sets.AsReadOnly();

    private Partido() { }

    public Partido(Guid equipoLocalId, Guid equipoVisitanteId, DateTime fechaProgramada, string lugar,
        ConfiguracionReglamentoPartido? reglamento = null)
    {
        reglamento ??= ConfiguracionReglamentoPartido.Fivb2025_2028;
        reglamento.Validar();
        Id = Guid.NewGuid();
        EquipoLocalId = equipoLocalId;
        EquipoVisitanteId = equipoVisitanteId;
        FechaProgramada = fechaProgramada;
        Lugar = lugar;
        Estado = EstadoPartido.Programado;
        SetsGanadosLocal = 0;
        SetsGanadosVisitante = 0;
        CodigoReglamento = reglamento.CodigoReglamento;
        MaximoSets = reglamento.MaximoSets;
        SetsParaGanar = reglamento.SetsParaGanar;
        PuntosSetRegular = reglamento.PuntosSetRegular;
        PuntosSetDecisivo = reglamento.PuntosSetDecisivo;
        DiferenciaMinima = reglamento.DiferenciaMinima;
        PuntoCambioCanchaSetDecisivo = reglamento.PuntoCambioCanchaSetDecisivo;
    }

    public void IniciarPartido()
    {
        if (Estado != EstadoPartido.Programado)
            throw new InvalidOperationException("El partido solo puede iniciarse si está en estado Programado.");

        Estado = EstadoPartido.EnProgreso;
        _sets.Add(CrearSet(1));
    }

    public Set IniciarSiguienteSet()
    {
        if (Estado != EstadoPartido.EnProgreso)
            throw new InvalidOperationException("El partido debe estar en curso para iniciar un set.");

        var ultimoSet = _sets.OrderByDescending(s => s.NumeroSet).FirstOrDefault();
        if (ultimoSet is not null && !ultimoSet.Finalizado)
            throw new InvalidOperationException("No puede iniciarse un set mientras el anterior siga abierto.");
        if (SetsGanadosLocal == SetsParaGanar || SetsGanadosVisitante == SetsParaGanar)
            throw new InvalidOperationException("El partido ya ha finalizado.");

        var set = CrearSet(_sets.Count + 1);
        _sets.Add(set);
        return set;
    }

    public void RegistrarResultadoSet(Set set)
    {
        if (!set.Finalizado || set.PartidoId != Id)
            throw new InvalidOperationException("Solo puede registrarse un set finalizado de este partido.");

        if (set.EquipoGanadorId == EquipoLocalId)
            SetsGanadosLocal++;
        else if (set.EquipoGanadorId == EquipoVisitanteId)
            SetsGanadosVisitante++;
        else
            throw new InvalidOperationException("El ganador del set no pertenece al partido.");

        if (SetsGanadosLocal == SetsParaGanar || SetsGanadosVisitante == SetsParaGanar)
        {
            Estado = EstadoPartido.Finalizado;
            EquipoGanadorId = SetsGanadosLocal == 3 ? EquipoLocalId : EquipoVisitanteId;
        }
    }

    private Set CrearSet(int numeroSet) => new(Id, numeroSet, numeroSet == MaximoSets,
        numeroSet == MaximoSets ? PuntosSetDecisivo : PuntosSetRegular, DiferenciaMinima,
        PuntoCambioCanchaSetDecisivo);
}
