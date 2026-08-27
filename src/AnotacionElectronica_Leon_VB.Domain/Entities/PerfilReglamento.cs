using AnotacionElectronica_Leon_VB.Domain.ValueObjects;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class PerfilReglamento
{
    public Guid Id { get; private set; }
    public string CodigoReglamento { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public int MaximoSets { get; private set; }
    public int SetsParaGanar { get; private set; }
    public int PuntosSetRegular { get; private set; }
    public int PuntosSetDecisivo { get; private set; }
    public int DiferenciaMinima { get; private set; }
    public int PuntoCambioCanchaSetDecisivo { get; private set; }

    private PerfilReglamento() { }

    public PerfilReglamento(
        string codigoReglamento,
        string nombre,
        string? descripcion,
        int maximoSets,
        int setsParaGanar,
        int puntosSetRegular,
        int puntosSetDecisivo,
        int diferenciaMinima,
        int puntoCambioCanchaSetDecisivo)
    {
        var vo = new ConfiguracionReglamentoPartido(
            codigoReglamento,
            maximoSets,
            setsParaGanar,
            puntosSetRegular,
            puntosSetDecisivo,
            diferenciaMinima,
            puntoCambioCanchaSetDecisivo);
        vo.Validar();

        Id = Guid.NewGuid();
        CodigoReglamento = codigoReglamento.Trim();
        Nombre = string.IsNullOrWhiteSpace(nombre) ? codigoReglamento.Trim() : nombre.Trim();
        Descripcion = descripcion?.Trim();
        MaximoSets = maximoSets;
        SetsParaGanar = setsParaGanar;
        PuntosSetRegular = puntosSetRegular;
        PuntosSetDecisivo = puntosSetDecisivo;
        DiferenciaMinima = diferenciaMinima;
        PuntoCambioCanchaSetDecisivo = puntoCambioCanchaSetDecisivo;
    }

    public ConfiguracionReglamentoPartido ToValueObject() =>
        new(CodigoReglamento, MaximoSets, SetsParaGanar, PuntosSetRegular, PuntosSetDecisivo, DiferenciaMinima, PuntoCambioCanchaSetDecisivo);
}
