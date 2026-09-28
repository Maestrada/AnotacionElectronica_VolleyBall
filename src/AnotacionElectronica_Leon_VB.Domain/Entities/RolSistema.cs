namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class RolSistema
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activo { get; private set; } = true;
    public ICollection<RolPermiso> Permisos { get; private set; } = new List<RolPermiso>();
    private RolSistema() { }
    public RolSistema(string nombre, string? descripcion = null) => Actualizar(nombre, descripcion);
    public void Actualizar(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre del rol es obligatorio.");
        Nombre = nombre.Trim();
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
    }
}
