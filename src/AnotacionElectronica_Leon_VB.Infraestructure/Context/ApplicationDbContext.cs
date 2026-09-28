using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainSet = AnotacionElectronica_Leon_VB.Domain.Entities.Set;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Jugador> Jugadores => Set<Jugador>();
    public DbSet<Partido> Partidos => Set<Partido>();
    public DbSet<DomainSet> Sets => Set<DomainSet>();
    public DbSet<Punto> Puntos => Set<Punto>();
    public DbSet<Rotacion> Rotaciones => Set<Rotacion>();
    public DbSet<EventoPartido> EventosPartido => Set<EventoPartido>();
    public DbSet<JuegoCalendario> JuegosCalendario => Set<JuegoCalendario>();
    public DbSet<FaseCompeticion> FasesCompeticion => Set<FaseCompeticion>();
    public DbSet<Recinto> Recintos => Set<Recinto>();
    public DbSet<DesignacionArbitral> DesignacionesArbitrales => Set<DesignacionArbitral>();
    public DbSet<AlineacionSet> AlineacionesSets => Set<AlineacionSet>();
    public DbSet<SustitucionReglamentaria> SustitucionesReglamentarias => Set<SustitucionReglamentaria>();
    public DbSet<Arbitro> Arbitros => Set<Arbitro>();
    public DbSet<Competicion> Competiciones => Set<Competicion>();
    public DbSet<PerfilReglamento> PerfilesReglamento => Set<PerfilReglamento>();
    public DbSet<Federacion> Federaciones => Set<Federacion>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<UsuarioSistema> UsuariosSistema => Set<UsuarioSistema>();
    public DbSet<RolSistema> RolesSistema => Set<RolSistema>();
    public DbSet<PermisoSistema> PermisosSistema => Set<PermisoSistema>();
    public DbSet<UsuarioRol> UsuariosRoles => Set<UsuarioRol>();
    public DbSet<RolPermiso> RolesPermisos => Set<RolPermiso>();
    public DbSet<BitacoraAccion> BitacoraAcciones => Set<BitacoraAccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica automáticamente todas las configuraciones IEntityTypeConfiguration creadas en el ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
