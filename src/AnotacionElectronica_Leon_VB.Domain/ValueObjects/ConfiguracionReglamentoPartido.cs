namespace AnotacionElectronica_Leon_VB.Domain.ValueObjects;

/// <summary>Instantánea de reglas aplicada al partido; nunca se recalcula desde la regla vigente.</summary>
public sealed record ConfiguracionReglamentoPartido(
    string CodigoReglamento,
    int MaximoSets,
    int SetsParaGanar,
    int PuntosSetRegular,
    int PuntosSetDecisivo,
    int DiferenciaMinima,
    int PuntoCambioCanchaSetDecisivo)
{
    public static ConfiguracionReglamentoPartido Fivb2025_2028 { get; } =
        new("FIVB-2025-2028", 5, 3, 25, 15, 2, 8);

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(CodigoReglamento) || MaximoSets < 1 || SetsParaGanar < 1 ||
            SetsParaGanar > (MaximoSets + 1) / 2 || PuntosSetRegular < 1 || PuntosSetDecisivo < 1 ||
            DiferenciaMinima < 1 || PuntoCambioCanchaSetDecisivo < 1)
            throw new ArgumentException("La configuración reglamentaria no es válida.");
    }
}
