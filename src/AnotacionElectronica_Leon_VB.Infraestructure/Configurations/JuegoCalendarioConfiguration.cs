using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public sealed class JuegoCalendarioConfiguration : IEntityTypeConfiguration<JuegoCalendario>
{
    public void Configure(EntityTypeBuilder<JuegoCalendario> builder)
    {
        builder.ToTable("JuegosCalendario");
        builder.HasKey(j => j.Id);
        builder.Property(j => j.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(j => j.Codigo).IsUnique();
        builder.Property(j => j.Recinto).HasMaxLength(150).IsRequired();
        builder.Property(j => j.Estado).HasConversion<int>();
        builder.HasIndex(j => j.FechaHoraProgramada);
        builder.HasIndex(j => j.PartidoId).IsUnique().HasFilter("[PartidoId] IS NOT NULL");
    }
}
