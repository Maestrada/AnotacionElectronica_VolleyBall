namespace AnotacionElectronica_Leon_VB.Domain.Entities;

/// <summary>Catálogo reutilizable de categorías deportivas (Mayor, U19, Infantil, etc.).</summary>
public class Categoria
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activa { get; private set; } = true;

    private Categoria() { }

    public Categoria(string nombre, string? descripcion = null)
    {
        Actualizar(nombre, descripcion);
    }

    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.");

        Nombre = nombre.Trim();
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
    }

    public void Activar() => Activa = true;
    public void Desactivar() => Activa = false;
}
