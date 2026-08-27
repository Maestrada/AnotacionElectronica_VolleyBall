using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations;

/// <summary>Agrega únicamente los catálogos introducidos después del calendario y las reglas del partido.</summary>
public partial class AgregarTablasFormularios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
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
            constraints: table => table.PrimaryKey("PK_Arbitros", x => x.Id));

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
            constraints: table => table.PrimaryKey("PK_Competiciones", x => x.Id));

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
            constraints: table => table.PrimaryKey("PK_PerfilesReglamento", x => x.Id));

        migrationBuilder.CreateIndex(name: "IX_PerfilesReglamento_CodigoReglamento", table: "PerfilesReglamento", column: "CodigoReglamento", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Arbitros");
        migrationBuilder.DropTable(name: "Competiciones");
        migrationBuilder.DropTable(name: "PerfilesReglamento");
    }
}
