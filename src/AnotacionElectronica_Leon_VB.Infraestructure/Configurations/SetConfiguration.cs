using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class SetConfiguration : IEntityTypeConfiguration<Set>
{
    public void Configure(EntityTypeBuilder<Set> builder)
    {
        builder.ToTable("Sets");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.HasMany(s => s.Puntos)
            .WithOne()
            .HasForeignKey(p => p.SetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Puntos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.Rotaciones)
            .WithOne()
            .HasForeignKey(r => r.SetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Rotaciones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}