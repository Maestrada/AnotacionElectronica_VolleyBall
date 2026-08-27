using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class PerfilReglamentoConfiguration : IEntityTypeConfiguration<PerfilReglamento>
{
    public void Configure(EntityTypeBuilder<PerfilReglamento> builder)
    {
        builder.ToTable("PerfilesReglamento");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.CodigoReglamento)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(r => r.CodigoReglamento)
            .IsUnique();

        builder.Property(r => r.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.Descripcion)
            .HasMaxLength(300);
    }
}
