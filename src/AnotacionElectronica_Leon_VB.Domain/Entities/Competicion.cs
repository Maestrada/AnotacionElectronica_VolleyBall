namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Competicion
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Edicion { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty; // Ej: Mayor, U19, Libre
    public string Rama { get; private set; } = string.Empty; // Ej: Femenil, Varonil, Mixto
    public string? Organizador { get; private set; }
    public string? SedePrincipal { get; private set; }

    private Competicion() { }

    public Competicion(string nombre, string edicion, string categoria, string rama, string? organizador = null, string? sedePrincipal = null)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(edicion))
            throw new ArgumentException("El nombre y edición de la competición son obligatorios.");

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        Edicion = edicion.Trim();
        Categoria = string.IsNullOrWhiteSpace(categoria) ? "Mayor" : categoria.Trim();
        Rama = string.IsNullOrWhiteSpace(rama) ? "Mixto" : rama.Trim();
        Organizador = organizador?.Trim();
        SedePrincipal = sedePrincipal?.Trim();
    }
}
