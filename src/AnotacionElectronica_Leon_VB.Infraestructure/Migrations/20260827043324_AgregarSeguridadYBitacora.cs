using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSeguridadYBitacora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentoAvalForaneoUrl",
                table: "Jugadores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentoIdentidadUrl",
                table: "Jugadores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsJugadorForaneo",
                table: "Jugadores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Equipos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Competiciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BitacoraAcciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Usuario = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    MetodoHttp = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CodigoRespuesta = table.Column<int>(type: "int", nullable: false),
                    DireccionIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DetalleJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BitacoraAcciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Federaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Federaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermisosSistema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesSistema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosSistema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    HashContrasena = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacionUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesPermisos",
                columns: table => new
                {
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermisoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesPermisos", x => new { x.RolId, x.PermisoId });
                    table.ForeignKey(
                        name: "FK_RolesPermisos_PermisosSistema_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "PermisosSistema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolesPermisos_RolesSistema_RolId",
                        column: x => x.RolId,
                        principalTable: "RolesSistema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_RolesSistema_RolId",
                        column: x => x.RolId,
                        principalTable: "RolesSistema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_UsuariosSistema_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuariosSistema",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PermisosSistema",
                columns: new[] { "Id", "Codigo", "Descripcion" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "SEGURIDAD.GESTIONAR", "Administrar usuarios, roles y permisos." },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "COMPETICIONES.GESTIONAR", "Administrar competiciones y fases." },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "CALENDARIO.GESTIONAR", "Programar, reprogramar y cancelar juegos." },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "PARTIDOS.ANOTAR", "Registrar puntos e incidencias de partido." },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "PARTIDOS.CERRAR", "Confirmar cierres de sets y partidos." },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "ACTAS.EXPORTAR", "Consultar y exportar actas y reportes." },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "CATALOGOS.GESTIONAR", "Administrar equipos, jugadores, árbitros, recintos y categorías." },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "BITACORA.CONSULTAR", "Consultar la bitácora de acciones." }
                });

            migrationBuilder.InsertData(
                table: "RolesSistema",
                columns: new[] { "Id", "Nombre", "Descripcion", "Activo" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Administrador", "Control total del sistema.", true },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Organizador", "Gestiona la competencia, calendario y catálogos.", true },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Anotador", "Opera la mesa y confirma acciones del partido.", true },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Consulta", "Consulta y exportación de información.", true }
                });

            migrationBuilder.InsertData(
                table: "RolesPermisos",
                columns: new[] { "RolId", "PermisoId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000004") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000005") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000006") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000007") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000008") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000006") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000007") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000004") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000005") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000006") },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000006") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraAcciones_FechaUtc",
                table: "BitacoraAcciones",
                column: "FechaUtc");

            migrationBuilder.CreateIndex(
                name: "IX_BitacoraAcciones_UsuarioId",
                table: "BitacoraAcciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermisosSistema_Codigo",
                table: "PermisosSistema",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolesPermisos_PermisoId",
                table: "RolesPermisos",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_RolesSistema_Nombre",
                table: "RolesSistema",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_RolId",
                table: "UsuariosRoles",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSistema_CorreoElectronico",
                table: "UsuariosSistema",
                column: "CorreoElectronico",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSistema_NombreUsuario",
                table: "UsuariosSistema",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BitacoraAcciones");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Federaciones");

            migrationBuilder.DropTable(
                name: "RolesPermisos");

            migrationBuilder.DropTable(
                name: "UsuariosRoles");

            migrationBuilder.DropTable(
                name: "PermisosSistema");

            migrationBuilder.DropTable(
                name: "RolesSistema");

            migrationBuilder.DropTable(
                name: "UsuariosSistema");

            migrationBuilder.DropColumn(
                name: "DocumentoAvalForaneoUrl",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "DocumentoIdentidadUrl",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "EsJugadorForaneo",
                table: "Jugadores");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Competiciones");
        }
    }
}
