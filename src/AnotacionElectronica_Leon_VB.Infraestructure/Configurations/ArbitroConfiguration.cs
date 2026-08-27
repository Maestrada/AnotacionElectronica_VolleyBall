using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class ArbitroConfiguration : IEntityTypeConfiguration<Arbitro>
{
    public void Configure(EntityTypeBuilder<Arbitro> builder)
    {
        builder.ToTable("Arbitros");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Apellidos)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Rol)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.NumeroLicencia)
            .HasMaxLength(50);

        builder.Property(a => a.Federacion)
            .HasMaxLength(100);
    }
}
