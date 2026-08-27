using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablasFormularios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CambioCanchaConfirmado",
                table: "Sets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DiferenciaMinima",
                table: "Sets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EsSetDecisivo",
                table: "Sets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PendienteCambioCancha",
                table: "Sets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PuntoCambioCancha",
                table: "Sets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PuntosParaGanar",
                table: "Sets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UltimoEquipoAlSaqueId",
                table: "Sets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoReglamento",
                table: "Partidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DiferenciaMinima",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaximoSets",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PuntoCambioCanchaSetDecisivo",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PuntosSetDecisivo",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PuntosSetRegular",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetsParaGanar",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Arbitros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    NumeroLicencia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Federacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arbitros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Competiciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Edicion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rama = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Organizador = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SedePrincipal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competiciones", x => x.Id);
                });

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
                constraints: table =>
                {
                    table.PrimaryKey("PK_JuegosCalendario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfilesReglamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoReglamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MaximoSets = table.Column<int>(type: "int", nullable: false),
                    SetsParaGanar = table.Column<int>(type: "int", nullable: false),
                    PuntosSetRegular = table.Column<int>(type: "int", nullable: false),
                    PuntosSetDecisivo = table.Column<int>(type: "int", nullable: false),
                    DiferenciaMinima = table.Column<int>(type: "int", nullable: false),
                    PuntoCambioCanchaSetDecisivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesReglamento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JuegosCalendario_Codigo",
                table: "JuegosCalendario",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JuegosCalendario_FechaHoraProgramada",
                table: "JuegosCalendario",
                column: "FechaHoraProgramada");

            migrationBuilder.CreateIndex(
                name: "IX_JuegosCalendario_PartidoId",
                table: "JuegosCalendario",
                column: "PartidoId",
                unique: true,
                filter: "[PartidoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesReglamento_CodigoReglamento",
                table: "PerfilesReglamento",
                column: "CodigoReglamento",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arbitros");

            migrationBuilder.DropTable(
                name: "Competiciones");

            migrationBuilder.DropTable(
                name: "JuegosCalendario");

            migrationBuilder.DropTable(
                name: "PerfilesReglamento");

            migrationBuilder.DropColumn(
                name: "CambioCanchaConfirmado",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "DiferenciaMinima",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "EsSetDecisivo",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "PendienteCambioCancha",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "PuntoCambioCancha",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "PuntosParaGanar",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "UltimoEquipoAlSaqueId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "CodigoReglamento",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "DiferenciaMinima",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "MaximoSets",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "PuntoCambioCanchaSetDecisivo",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "PuntosSetDecisivo",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "PuntosSetRegular",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "SetsParaGanar",
                table: "Partidos");
        }
    }
}
