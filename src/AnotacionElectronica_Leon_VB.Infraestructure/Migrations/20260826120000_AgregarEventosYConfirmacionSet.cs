using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;

#nullable disable

namespace AnotacionElectronica_Leon_VB.Infraestructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260826120000_AgregarEventosYConfirmacionSet")]
public partial class AgregarEventosYConfirmacionSet : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // La primera versión se desplegó en algunas instalaciones sin registrar esta migración.
        // Las comprobaciones permiten adoptar esas bases sin volver a crear columnas o tablas existentes.
        migrationBuilder.Sql("""
            IF COL_LENGTH(N'dbo.Sets', N'PendienteConfirmacionCierre') IS NULL
                ALTER TABLE [Sets] ADD [PendienteConfirmacionCierre] bit NOT NULL CONSTRAINT [DF_Sets_PendienteConfirmacionCierre] DEFAULT CAST(0 AS bit);

            IF OBJECT_ID(N'dbo.EventosPartido', N'U') IS NULL
            BEGIN
                CREATE TABLE [EventosPartido] (
                    [Id] uniqueidentifier NOT NULL,
                    [PartidoId] uniqueidentifier NOT NULL,
                    [SetId] uniqueidentifier NULL,
                    [Secuencia] int NOT NULL,
                    [Tipo] int NOT NULL,
                    [DatosJson] nvarchar(max) NOT NULL,
                    [OcurrioEnUtc] datetime2 NOT NULL,
                    CONSTRAINT [PK_EventosPartido] PRIMARY KEY ([Id])
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EventosPartido_PartidoId_Secuencia' AND object_id = OBJECT_ID(N'dbo.EventosPartido'))
                CREATE UNIQUE INDEX [IX_EventosPartido_PartidoId_Secuencia] ON [EventosPartido] ([PartidoId], [Secuencia]);
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EventosPartido_SetId' AND object_id = OBJECT_ID(N'dbo.EventosPartido'))
                CREATE INDEX [IX_EventosPartido_SetId] ON [EventosPartido] ([SetId]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EventosPartido");
        migrationBuilder.DropColumn(name: "PendienteConfirmacionCierre", table: "Sets");
    }
}
