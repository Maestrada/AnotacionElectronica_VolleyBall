using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260826130000_AgregarConfiguracionReglamentariaYCambioCancha")]
public partial class AgregarConfiguracionReglamentariaYCambioCancha : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF COL_LENGTH(N'dbo.Partidos', N'CodigoReglamento') IS NULL ALTER TABLE [Partidos] ADD [CodigoReglamento] nvarchar(max) NOT NULL DEFAULT N'FIVB-2025-2028';
            IF COL_LENGTH(N'dbo.Partidos', N'MaximoSets') IS NULL ALTER TABLE [Partidos] ADD [MaximoSets] int NOT NULL DEFAULT 5;
            IF COL_LENGTH(N'dbo.Partidos', N'SetsParaGanar') IS NULL ALTER TABLE [Partidos] ADD [SetsParaGanar] int NOT NULL DEFAULT 3;
            IF COL_LENGTH(N'dbo.Partidos', N'PuntosSetRegular') IS NULL ALTER TABLE [Partidos] ADD [PuntosSetRegular] int NOT NULL DEFAULT 25;
            IF COL_LENGTH(N'dbo.Partidos', N'PuntosSetDecisivo') IS NULL ALTER TABLE [Partidos] ADD [PuntosSetDecisivo] int NOT NULL DEFAULT 15;
            IF COL_LENGTH(N'dbo.Partidos', N'DiferenciaMinima') IS NULL ALTER TABLE [Partidos] ADD [DiferenciaMinima] int NOT NULL DEFAULT 2;
            IF COL_LENGTH(N'dbo.Partidos', N'PuntoCambioCanchaSetDecisivo') IS NULL ALTER TABLE [Partidos] ADD [PuntoCambioCanchaSetDecisivo] int NOT NULL DEFAULT 8;
            IF COL_LENGTH(N'dbo.Sets', N'EsSetDecisivo') IS NULL ALTER TABLE [Sets] ADD [EsSetDecisivo] bit NOT NULL DEFAULT CAST(0 AS bit);
            IF COL_LENGTH(N'dbo.Sets', N'PuntosParaGanar') IS NULL ALTER TABLE [Sets] ADD [PuntosParaGanar] int NOT NULL DEFAULT 25;
            IF COL_LENGTH(N'dbo.Sets', N'DiferenciaMinima') IS NULL ALTER TABLE [Sets] ADD [DiferenciaMinima] int NOT NULL DEFAULT 2;
            IF COL_LENGTH(N'dbo.Sets', N'PuntoCambioCancha') IS NULL ALTER TABLE [Sets] ADD [PuntoCambioCancha] int NOT NULL DEFAULT 8;
            IF COL_LENGTH(N'dbo.Sets', N'PendienteCambioCancha') IS NULL ALTER TABLE [Sets] ADD [PendienteCambioCancha] bit NOT NULL DEFAULT CAST(0 AS bit);
            IF COL_LENGTH(N'dbo.Sets', N'CambioCanchaConfirmado') IS NULL ALTER TABLE [Sets] ADD [CambioCanchaConfirmado] bit NOT NULL DEFAULT CAST(0 AS bit);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var columna in new[] { "CodigoReglamento", "MaximoSets", "SetsParaGanar", "PuntosSetRegular", "PuntosSetDecisivo", "DiferenciaMinima", "PuntoCambioCanchaSetDecisivo" })
            migrationBuilder.DropColumn(name: columna, table: "Partidos");
        foreach (var columna in new[] { "EsSetDecisivo", "PuntosParaGanar", "DiferenciaMinima", "PuntoCambioCancha", "PendienteCambioCancha", "CambioCanchaConfirmado" })
            migrationBuilder.DropColumn(name: columna, table: "Sets");
    }
}
