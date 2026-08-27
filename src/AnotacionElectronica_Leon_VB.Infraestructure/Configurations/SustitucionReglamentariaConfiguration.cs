using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infrastructure.Persistence.Configurations;

public class SustitucionReglamentariaConfiguration : IEntityTypeConfiguration<SustitucionReglamentaria>
{
    public void Configure(EntityTypeBuilder<SustitucionReglamentaria> builder)
    {
        builder.ToTable("SustitucionesReglamentarias");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Posicion)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.FechaHora)
            .IsRequired();
    }
}