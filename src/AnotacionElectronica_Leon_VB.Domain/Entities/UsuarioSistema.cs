namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class UsuarioSistema
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string NombreUsuario { get; private set; } = string.Empty;
    public string NombreCompleto { get; private set; } = string.Empty;
    public string CorreoElectronico { get; private set; } = string.Empty;
    public string HashContrasena { get; private set; } = string.Empty;
    public bool Activo { get; private set; } = true;
    public DateTime FechaCreacionUtc { get; private set; } = DateTime.UtcNow;
    public Guid? ArbitroId { get; private set; }
    public Arbitro? Arbitro { get; private set; }
    public ICollection<UsuarioRol> Roles { get; private set; } = new List<UsuarioRol>();

    private UsuarioSistema() { }
    public UsuarioSistema(string nombreUsuario, string nombreCompleto, string correoElectronico, string hashContrasena)
    {
        Actualizar(nombreUsuario, nombreCompleto, correoElectronico);
        EstablecerHashContrasena(hashContrasena);
    }
    public void Actualizar(string nombreUsuario, string nombreCompleto, string correoElectronico)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(correoElectronico))
            throw new ArgumentException("Usuario, nombre completo y correo son obligatorios.");
        NombreUsuario = nombreUsuario.Trim();
        NombreCompleto = nombreCompleto.Trim();
        CorreoElectronico = correoElectronico.Trim().ToLowerInvariant();
    }
    public void EstablecerHashContrasena(string hashContrasena)
    {
        if (string.IsNullOrWhiteSpace(hashContrasena)) throw new ArgumentException("La contraseña es obligatoria.");
        HashContrasena = hashContrasena;
    }
    public void Activar() => Activo = true;
    public void Desactivar() => Activo = false;
    public void VincularArbitro(Guid arbitroId)
    {
        if (arbitroId == Guid.Empty) throw new ArgumentException("El árbitro es obligatorio.");
        ArbitroId = arbitroId;
    }
    public void DesvincularArbitro() => ArbitroId = null;
}
