using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class PuntoConfiguration : IEntityTypeConfiguration<Punto>
{
    public void Configure(EntityTypeBuilder<Punto> builder)
    {
        builder.ToTable("Puntos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
    }
}
