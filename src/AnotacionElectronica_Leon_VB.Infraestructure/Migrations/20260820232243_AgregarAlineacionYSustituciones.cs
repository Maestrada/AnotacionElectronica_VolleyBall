using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAlineacionYSustituciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlineacionesSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionInicial1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionInicial2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionInicial3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionInicial4Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionInicial5Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PosicionInicial6Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SustitucionesRealizadas = table.Column<int>(type: "int", nullable: false),
                    ReemplazosLiberoJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosicionesActualesJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlineacionesSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SustitucionesReglamentarias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlineacionSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JugadorSaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JugadorEntraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion = table.Column<int>(type: "int", nullable: false),
                    PuntosLocalEnMomento = table.Column<int>(type: "int", nullable: false),
                    PuntosVisitanteEnMomento = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SustitucionesReglamentarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SustitucionesReglamentarias_AlineacionesSets_AlineacionSetId",
                        column: x => x.AlineacionSetId,
                        principalTable: "AlineacionesSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SustitucionesReglamentarias_AlineacionSetId",
                table: "SustitucionesReglamentarias",
                column: "AlineacionSetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SustitucionesReglamentarias");

            migrationBuilder.DropTable(
                name: "AlineacionesSets");
        }
    }
}
