using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class EquipoConfiguration : IEntityTypeConfiguration<Equipo>
{
    public void Configure(EntityTypeBuilder<Equipo> builder)
    {
        builder.ToTable("Equipos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NombreEntrenador)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.NombreAsistente)
            .HasMaxLength(100);

        builder.Property(e => e.Categoria)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(e => e.Jugadores)
            .WithOne(j => j.Equipo)
            .HasForeignKey(j => j.EquipoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}