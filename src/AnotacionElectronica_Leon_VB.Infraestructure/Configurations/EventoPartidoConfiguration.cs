using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public sealed class EventoPartidoConfiguration : IEntityTypeConfiguration<EventoPartido>
{
    public void Configure(EntityTypeBuilder<EventoPartido> builder)
    {
        builder.ToTable("EventosPartido");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.DatosJson).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(e => e.Tipo).HasConversion<int>().IsRequired();
        builder.HasIndex(e => new { e.PartidoId, e.Secuencia }).IsUnique();
        builder.HasIndex(e => e.SetId);
    }
}
