using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations;

public partial class AgregarCalendarioJuegos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "JuegosCalendario",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Competicion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Edicion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Fase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                EquipoLocalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                EquipoVisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FechaHoraProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                Recinto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                Estado = table.Column<int>(type: "int", nullable: false),
                PartidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CodigoReglamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MaximoSets = table.Column<int>(type: "int", nullable: false),
                SetsParaGanar = table.Column<int>(type: "int", nullable: false),
                PuntosSetRegular = table.Column<int>(type: "int", nullable: false),
                PuntosSetDecisivo = table.Column<int>(type: "int", nullable: false),
                DiferenciaMinima = table.Column<int>(type: "int", nullable: false),
                PuntoCambioCanchaSetDecisivo = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_JuegosCalendario", x => x.Id));

        migrationBuilder.CreateIndex(name: "IX_JuegosCalendario_Codigo", table: "JuegosCalendario", column: "Codigo", unique: true);
        migrationBuilder.CreateIndex(name: "IX_JuegosCalendario_FechaHoraProgramada", table: "JuegosCalendario", column: "FechaHoraProgramada");
        migrationBuilder.CreateIndex(name: "IX_JuegosCalendario_PartidoId", table: "JuegosCalendario", column: "PartidoId", unique: true, filter: "[PartidoId] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "JuegosCalendario");
}
