using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class PartidoConfiguration : IEntityTypeConfiguration<Partido>
{
    public void Configure(EntityTypeBuilder<Partido> builder)
    {
        builder.ToTable("Partidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Lugar)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Estado)
            .IsRequired()
            .HasConversion<int>();

        // Relaciones con Equipo Local y Visitante
        builder.HasOne(p => p.EquipoLocal)
            .WithMany()
            .HasForeignKey(p => p.EquipoLocalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.EquipoVisitante)
            .WithMany()
            .HasForeignKey(p => p.EquipoVisitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Sets)
            .WithOne()
            .HasForeignKey(s => s.PartidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Sets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}