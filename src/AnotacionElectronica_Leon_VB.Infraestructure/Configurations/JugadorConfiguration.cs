using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class JugadorConfiguration : IEntityTypeConfiguration<Jugador>
{
    public void Configure(EntityTypeBuilder<Jugador> builder)
    {
        builder.ToTable("Jugadores");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.Apellidos)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.NumeroCamiseta)
            .IsRequired();

        builder.Property(j => j.Posicion)
            .IsRequired()
            .HasConversion<int>();

        builder.HasIndex(j => new { j.EquipoId, j.NumeroCamiseta })
            .IsUnique();
    }
}