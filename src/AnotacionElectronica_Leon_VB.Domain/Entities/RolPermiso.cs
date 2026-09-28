namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class RolPermiso
{
    public Guid RolId { get; private set; }
    public Guid PermisoId { get; private set; }
    public RolSistema Rol { get; private set; } = null!;
    public PermisoSistema Permiso { get; private set; } = null!;
    private RolPermiso() { }
    public RolPermiso(Guid rolId, Guid permisoId) { RolId = rolId; PermisoId = permisoId; }
}
