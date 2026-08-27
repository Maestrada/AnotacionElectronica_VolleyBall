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
    public DbSet<AlineacionSet> AlineacionesSets => Set<AlineacionSet>();
    public DbSet<SustitucionReglamentaria> SustitucionesReglamentarias => Set<SustitucionReglamentaria>();
    public DbSet<Arbitro> Arbitros => Set<Arbitro>();
    public DbSet<Competicion> Competiciones => Set<Competicion>();
    public DbSet<PerfilReglamento> PerfilesReglamento => Set<PerfilReglamento>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is DomainSet)
                {
                    var origNumero = entry.Property(nameof(DomainSet.NumeroSet)).OriginalValue;
                    if (origNumero is null || origNumero.Equals(0))
                    {
                        entry.State = EntityState.Added;
                    }
                }
                else if (entry.Entity is Punto)
                {
                    var origSetId = entry.Property(nameof(Punto.SetId)).OriginalValue;
                    if (origSetId is null || origSetId.Equals(Guid.Empty))
                    {
                        entry.State = EntityState.Added;
                    }
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica automáticamente todas las configuraciones IEntityTypeConfiguration creadas en el ensamblado
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
