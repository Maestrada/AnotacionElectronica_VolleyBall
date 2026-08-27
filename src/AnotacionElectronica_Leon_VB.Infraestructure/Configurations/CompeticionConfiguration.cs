using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class CompeticionConfiguration : IEntityTypeConfiguration<Competicion>
{
    public void Configure(EntityTypeBuilder<Competicion> builder)
    {
        builder.ToTable("Competiciones");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Edicion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Categoria)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Rama)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Organizador)
            .HasMaxLength(150);

        builder.Property(c => c.SedePrincipal)
            .HasMaxLength(150);
    }
}
