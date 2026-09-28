namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class PermisoSistema
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    private PermisoSistema() { }
    public PermisoSistema(string codigo, string descripcion)
    {
        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("Código y descripción del permiso son obligatorios.");
        Codigo = codigo.Trim().ToUpperInvariant(); Descripcion = descripcion.Trim();
    }
}
