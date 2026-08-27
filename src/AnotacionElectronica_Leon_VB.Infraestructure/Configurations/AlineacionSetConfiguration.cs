using System.Text.Json;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infrastructure.Persistence.Configurations;

public class AlineacionSetConfiguration : IEntityTypeConfiguration<AlineacionSet>
{
    public void Configure(EntityTypeBuilder<AlineacionSet> builder)
    {
        builder.ToTable("AlineacionesSets");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.SetId)
            .IsRequired();

        builder.Property(a => a.EquipoId)
            .IsRequired();

        builder.Property(a => a.PosicionInicial1Id).IsRequired();
        builder.Property(a => a.PosicionInicial2Id).IsRequired();
        builder.Property(a => a.PosicionInicial3Id).IsRequired();
        builder.Property(a => a.PosicionInicial4Id).IsRequired();
        builder.Property(a => a.PosicionInicial5Id).IsRequired();
        builder.Property(a => a.PosicionInicial6Id).IsRequired();

        builder.Property(a => a.SustitucionesRealizadas)
            .IsRequired();

        // Opciones de serialización JSON para Enums como Keys
        var jsonOptions = new JsonSerializerOptions();

        // 1. Mapeo del Dictionary PosicionesActuales a columna NVARCHAR(MAX) como JSON
        var valueComparerPosiciones = new ValueComparer<Dictionary<PosicionCancha, Guid>>(
            (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any(),
            c => c.Aggregate(0, (a, p) => HashCode.Combine(a, p.Key.GetHashCode(), p.Value.GetHashCode())),
            c => new Dictionary<PosicionCancha, Guid>(c));

        builder.Property<Dictionary<PosicionCancha, Guid>>("_posicionesActuales")
            .HasColumnName("PosicionesActualesJson")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                d => JsonSerializer.Serialize(d, jsonOptions),
                json => JsonSerializer.Deserialize<Dictionary<PosicionCancha, Guid>>(json, jsonOptions) ?? new Dictionary<PosicionCancha, Guid>()
            )
            .Metadata.SetValueComparer(valueComparerPosiciones);

        // 2. Mapeo del Dictionary _jugadorReemplazadoPorLibero a columna JSON
        var valueComparerLibero = new ValueComparer<Dictionary<PosicionCancha, Guid>>(
            (c1, c2) => c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any(),
            c => c.Aggregate(0, (a, p) => HashCode.Combine(a, p.Key.GetHashCode(), p.Value.GetHashCode())),
            c => new Dictionary<PosicionCancha, Guid>(c));

        builder.Property<Dictionary<PosicionCancha, Guid>>("_jugadorReemplazadoPorLibero")
            .HasColumnName("ReemplazosLiberoJson")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                d => JsonSerializer.Serialize(d, jsonOptions),
                json => JsonSerializer.Deserialize<Dictionary<PosicionCancha, Guid>>(json, jsonOptions) ?? new Dictionary<PosicionCancha, Guid>()
            )
            .Metadata.SetValueComparer(valueComparerLibero);

        // 3. Mapeo de la colección de navegaciones de Sustituciones (Backing Field _sustituciones)
        builder.HasMany(a => a.Sustituciones)
            .WithOne()
            .HasForeignKey(s => s.AlineacionSetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Sustituciones)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}