using AnotacionElectronica_Leon_VB.Domain.Enums;
using AnotacionElectronica_Leon_VB.Domain.ValueObjects;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

/// <summary>Programación previa; un partido puede existir sin un registro de calendario.</summary>
public class JuegoCalendario
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string? Competicion { get; private set; }
    public string? Edicion { get; private set; }
    public string? Fase { get; private set; }
    public Guid EquipoLocalId { get; private set; }
    public Guid EquipoVisitanteId { get; private set; }
    public DateTime FechaHoraProgramada { get; private set; }
    public string Recinto { get; private set; } = string.Empty;
    public EstadoJuegoCalendario Estado { get; private set; }
    public Guid? PartidoId { get; private set; }
    public string CodigoReglamento { get; private set; } = string.Empty;
    public int MaximoSets { get; private set; }
    public int SetsParaGanar { get; private set; }
    public int PuntosSetRegular { get; private set; }
    public int PuntosSetDecisivo { get; private set; }
    public int DiferenciaMinima { get; private set; }
    public int PuntoCambioCanchaSetDecisivo { get; private set; }

    private JuegoCalendario() { }

    public JuegoCalendario(string codigo, Guid equipoLocalId, Guid equipoVisitanteId, DateTime fechaHoraProgramada,
        string recinto, string? competicion, string? edicion, string? fase, ConfiguracionReglamentoPartido? reglamento = null)
    {
        if (equipoLocalId == equipoVisitanteId)
            throw new ArgumentException("Los equipos del juego programado deben ser distintos.");
        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(recinto))
            throw new ArgumentException("El código y recinto del juego programado son obligatorios.");

        reglamento ??= ConfiguracionReglamentoPartido.Fivb2025_2028;
        reglamento.Validar();
        Id = Guid.NewGuid();
        Codigo = codigo.Trim();
        EquipoLocalId = equipoLocalId;
        EquipoVisitanteId = equipoVisitanteId;
        FechaHoraProgramada = fechaHoraProgramada;
        Recinto = recinto.Trim();
        Competicion = competicion?.Trim();
        Edicion = edicion?.Trim();
        Fase = fase?.Trim();
        Estado = EstadoJuegoCalendario.Programado;
        CodigoReglamento = reglamento.CodigoReglamento;
        MaximoSets = reglamento.MaximoSets;
        SetsParaGanar = reglamento.SetsParaGanar;
        PuntosSetRegular = reglamento.PuntosSetRegular;
        PuntosSetDecisivo = reglamento.PuntosSetDecisivo;
        DiferenciaMinima = reglamento.DiferenciaMinima;
        PuntoCambioCanchaSetDecisivo = reglamento.PuntoCambioCanchaSetDecisivo;
    }

    public ConfiguracionReglamentoPartido ObtenerReglamento() => new(CodigoReglamento, MaximoSets, SetsParaGanar,
        PuntosSetRegular, PuntosSetDecisivo, DiferenciaMinima, PuntoCambioCanchaSetDecisivo);

    public void VincularPartido(Guid partidoId)
    {
        if (Estado is EstadoJuegoCalendario.Cancelado or EstadoJuegoCalendario.ConvertidoEnPartido)
            throw new InvalidOperationException("El juego programado no está disponible para crear un partido.");
        PartidoId = partidoId;
        Estado = EstadoJuegoCalendario.ConvertidoEnPartido;
    }
}
