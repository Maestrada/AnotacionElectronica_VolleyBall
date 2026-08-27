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
        migrationBuilder.AddColumn<string>(name: "CodigoReglamento", table: "Partidos", type: "nvarchar(max)", nullable: false, defaultValue: "FIVB-2025-2028");
        migrationBuilder.AddColumn<int>(name: "MaximoSets", table: "Partidos", type: "int", nullable: false, defaultValue: 5);
        migrationBuilder.AddColumn<int>(name: "SetsParaGanar", table: "Partidos", type: "int", nullable: false, defaultValue: 3);
        migrationBuilder.AddColumn<int>(name: "PuntosSetRegular", table: "Partidos", type: "int", nullable: false, defaultValue: 25);
        migrationBuilder.AddColumn<int>(name: "PuntosSetDecisivo", table: "Partidos", type: "int", nullable: false, defaultValue: 15);
        migrationBuilder.AddColumn<int>(name: "DiferenciaMinima", table: "Partidos", type: "int", nullable: false, defaultValue: 2);
        migrationBuilder.AddColumn<int>(name: "PuntoCambioCanchaSetDecisivo", table: "Partidos", type: "int", nullable: false, defaultValue: 8);

        migrationBuilder.AddColumn<bool>(name: "EsSetDecisivo", table: "Sets", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<int>(name: "PuntosParaGanar", table: "Sets", type: "int", nullable: false, defaultValue: 25);
        migrationBuilder.AddColumn<int>(name: "DiferenciaMinima", table: "Sets", type: "int", nullable: false, defaultValue: 2);
        migrationBuilder.AddColumn<int>(name: "PuntoCambioCancha", table: "Sets", type: "int", nullable: false, defaultValue: 8);
        migrationBuilder.AddColumn<bool>(name: "PendienteCambioCancha", table: "Sets", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "CambioCanchaConfirmado", table: "Sets", type: "bit", nullable: false, defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var columna in new[] { "CodigoReglamento", "MaximoSets", "SetsParaGanar", "PuntosSetRegular", "PuntosSetDecisivo", "DiferenciaMinima", "PuntoCambioCanchaSetDecisivo" })
            migrationBuilder.DropColumn(name: columna, table: "Partidos");
        foreach (var columna in new[] { "EsSetDecisivo", "PuntosParaGanar", "DiferenciaMinima", "PuntoCambioCancha", "PendienteCambioCancha", "CambioCanchaConfirmado" })
            migrationBuilder.DropColumn(name: columna, table: "Sets");
    }
}
