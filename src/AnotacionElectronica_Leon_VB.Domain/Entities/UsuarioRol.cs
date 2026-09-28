namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class UsuarioRol
{
    public Guid UsuarioId { get; private set; }
    public Guid RolId { get; private set; }
    public UsuarioSistema Usuario { get; private set; } = null!;
    public RolSistema Rol { get; private set; } = null!;
    private UsuarioRol() { }
    public UsuarioRol(Guid usuarioId, Guid rolId) { UsuarioId = usuarioId; RolId = rolId; }
}
