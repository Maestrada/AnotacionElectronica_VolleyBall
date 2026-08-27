using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreEntrenador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreAsistente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jugadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumeroCamiseta = table.Column<int>(type: "int", nullable: false),
                    Posicion = table.Column<int>(type: "int", nullable: false),
                    EsCapitan = table.Column<bool>(type: "bit", nullable: false),
                    EquipoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jugadores_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipoLocalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipoVisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lugar = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    SetsGanadosLocal = table.Column<int>(type: "int", nullable: false),
                    SetsGanadosVisitante = table.Column<int>(type: "int", nullable: false),
                    EquipoGanadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoLocalId",
                        column: x => x.EquipoLocalId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Equipos_EquipoVisitanteId",
                        column: x => x.EquipoVisitanteId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroSet = table.Column<int>(type: "int", nullable: false),
                    PuntosLocal = table.Column<int>(type: "int", nullable: false),
                    PuntosVisitante = table.Column<int>(type: "int", nullable: false),
                    EquipoGanadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Finalizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sets_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Puntos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipoAnotadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JugadorAnotadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PuntosEquipoLocalActual = table.Column<int>(type: "int", nullable: false),
                    PuntosEquipoVisitanteActual = table.Column<int>(type: "int", nullable: false),
                    TipoAccion = table.Column<int>(type: "int", nullable: false),
                    HoraRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puntos_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rotaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion3Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion4Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion5Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Posicion6Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rotaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rotaciones_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_EquipoId_NumeroCamiseta",
                table: "Jugadores",
                columns: new[] { "EquipoId", "NumeroCamiseta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EquipoLocalId",
                table: "Partidos",
                column: "EquipoLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_EquipoVisitanteId",
                table: "Partidos",
                column: "EquipoVisitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Puntos_SetId",
                table: "Puntos",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotaciones_SetId",
                table: "Rotaciones",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_PartidoId",
                table: "Sets",
                column: "PartidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jugadores");

            migrationBuilder.DropTable(
                name: "Puntos");

            migrationBuilder.DropTable(
                name: "Rotaciones");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "Partidos");

            migrationBuilder.DropTable(
                name: "Equipos");
        }
    }
}
