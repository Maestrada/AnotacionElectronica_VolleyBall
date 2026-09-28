using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260826140000_AgregarCalendarioJuegos")]
public partial class AgregarCalendarioJuegos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'dbo.JuegosCalendario', N'U') IS NULL
            BEGIN
                CREATE TABLE [JuegosCalendario] (
                    [Id] uniqueidentifier NOT NULL, [Codigo] nvarchar(50) NOT NULL, [Competicion] nvarchar(max) NULL,
                    [Edicion] nvarchar(max) NULL, [Fase] nvarchar(max) NULL, [EquipoLocalId] uniqueidentifier NOT NULL,
                    [EquipoVisitanteId] uniqueidentifier NOT NULL, [FechaHoraProgramada] datetime2 NOT NULL,
                    [Recinto] nvarchar(150) NOT NULL, [Estado] int NOT NULL, [PartidoId] uniqueidentifier NULL,
                    [CodigoReglamento] nvarchar(max) NOT NULL, [MaximoSets] int NOT NULL, [SetsParaGanar] int NOT NULL,
                    [PuntosSetRegular] int NOT NULL, [PuntosSetDecisivo] int NOT NULL, [DiferenciaMinima] int NOT NULL,
                    [PuntoCambioCanchaSetDecisivo] int NOT NULL, CONSTRAINT [PK_JuegosCalendario] PRIMARY KEY ([Id])
                );
            END;
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JuegosCalendario_Codigo' AND object_id = OBJECT_ID(N'dbo.JuegosCalendario'))
                CREATE UNIQUE INDEX [IX_JuegosCalendario_Codigo] ON [JuegosCalendario] ([Codigo]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JuegosCalendario_FechaHoraProgramada' AND object_id = OBJECT_ID(N'dbo.JuegosCalendario'))
                CREATE INDEX [IX_JuegosCalendario_FechaHoraProgramada] ON [JuegosCalendario] ([FechaHoraProgramada]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_JuegosCalendario_PartidoId' AND object_id = OBJECT_ID(N'dbo.JuegosCalendario'))
                CREATE UNIQUE INDEX [IX_JuegosCalendario_PartidoId] ON [JuegosCalendario] ([PartidoId]) WHERE [PartidoId] IS NOT NULL;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "JuegosCalendario");
}
