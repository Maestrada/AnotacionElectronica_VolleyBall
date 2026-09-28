using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class VincularUsuariosConArbitros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArbitroId",
                table: "UsuariosSistema",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSistema_ArbitroId",
                table: "UsuariosSistema",
                column: "ArbitroId",
                unique: true,
                filter: "[ArbitroId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosSistema_Arbitros_ArbitroId",
                table: "UsuariosSistema",
                column: "ArbitroId",
                principalTable: "Arbitros",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosSistema_Arbitros_ArbitroId",
                table: "UsuariosSistema");

            migrationBuilder.DropIndex(
                name: "IX_UsuariosSistema_ArbitroId",
                table: "UsuariosSistema");

            migrationBuilder.DropColumn(
                name: "ArbitroId",
                table: "UsuariosSistema");
        }
    }
}
