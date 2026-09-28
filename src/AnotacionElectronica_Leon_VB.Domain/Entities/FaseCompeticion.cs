namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class FaseCompeticion
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid CompeticionId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public int Orden { get; private set; }
    public string CodigoReglamento { get; private set; } = string.Empty;
    private FaseCompeticion() { }
    public FaseCompeticion(Guid competicionId, string nombre, int orden, string codigoReglamento)
    { if (competicionId == Guid.Empty || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(codigoReglamento)) throw new ArgumentException("Competición, nombre y reglamento son obligatorios."); CompeticionId = competicionId; Nombre = nombre.Trim(); Orden = orden; CodigoReglamento = codigoReglamento.Trim(); }
}
