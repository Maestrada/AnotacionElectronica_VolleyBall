using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations;

public partial class AgregarEventosYConfirmacionSet : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "PendienteConfirmacionCierre",
            table: "Sets",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "EventosPartido",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PartidoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Secuencia = table.Column<int>(type: "int", nullable: false),
                Tipo = table.Column<int>(type: "int", nullable: false),
                DatosJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OcurrioEnUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_EventosPartido", x => x.Id));

        migrationBuilder.CreateIndex(name: "IX_EventosPartido_PartidoId_Secuencia", table: "EventosPartido",
            columns: new[] { "PartidoId", "Secuencia" }, unique: true);
        migrationBuilder.CreateIndex(name: "IX_EventosPartido_SetId", table: "EventosPartido", column: "SetId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EventosPartido");
        migrationBuilder.DropColumn(name: "PendienteConfirmacionCierre", table: "Sets");
    }
}
