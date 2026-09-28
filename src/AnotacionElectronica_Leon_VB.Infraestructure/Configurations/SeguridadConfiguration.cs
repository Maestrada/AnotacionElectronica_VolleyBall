using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Configurations;

public class UsuarioSistemaConfiguration : IEntityTypeConfiguration<UsuarioSistema>
{
    public void Configure(EntityTypeBuilder<UsuarioSistema> builder)
    {
        builder.ToTable("UsuariosSistema"); builder.HasKey(x => x.Id);
        builder.Property(x => x.NombreUsuario).HasMaxLength(80).IsRequired(); builder.HasIndex(x => x.NombreUsuario).IsUnique();
        builder.Property(x => x.NombreCompleto).HasMaxLength(160).IsRequired();
        builder.Property(x => x.CorreoElectronico).HasMaxLength(160).IsRequired(); builder.HasIndex(x => x.CorreoElectronico).IsUnique();
        builder.Property(x => x.HashContrasena).HasMaxLength(500).IsRequired();
        builder.HasMany(x => x.Roles).WithOne(x => x.Usuario).HasForeignKey(x => x.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Arbitro).WithOne(x => x.UsuarioSistema).HasForeignKey<UsuarioSistema>(x => x.ArbitroId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class RolSistemaConfiguration : IEntityTypeConfiguration<RolSistema>
{
    public void Configure(EntityTypeBuilder<RolSistema> builder)
    {
        builder.ToTable("RolesSistema"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(80).IsRequired(); builder.HasIndex(x => x.Nombre).IsUnique();
        builder.Property(x => x.Descripcion).HasMaxLength(300);
        builder.HasMany(x => x.Permisos).WithOne(x => x.Rol).HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PermisoSistemaConfiguration : IEntityTypeConfiguration<PermisoSistema>
{
    public void Configure(EntityTypeBuilder<PermisoSistema> builder)
    {
        builder.ToTable("PermisosSistema"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Codigo).HasMaxLength(120).IsRequired(); builder.HasIndex(x => x.Codigo).IsUnique();
        builder.Property(x => x.Descripcion).HasMaxLength(300).IsRequired();
    }
}

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("UsuariosRoles"); builder.HasKey(x => new { x.UsuarioId, x.RolId });
        builder.HasOne(x => x.Rol).WithMany().HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
{
    public void Configure(EntityTypeBuilder<RolPermiso> builder)
    {
        builder.ToTable("RolesPermisos"); builder.HasKey(x => new { x.RolId, x.PermisoId });
        builder.HasOne(x => x.Permiso).WithMany().HasForeignKey(x => x.PermisoId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class BitacoraAccionConfiguration : IEntityTypeConfiguration<BitacoraAccion>
{
    public void Configure(EntityTypeBuilder<BitacoraAccion> builder)
    {
        builder.ToTable("BitacoraAcciones"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Usuario).HasMaxLength(160).IsRequired(); builder.Property(x => x.Accion).HasMaxLength(160).IsRequired();
        builder.Property(x => x.MetodoHttp).HasMaxLength(10).IsRequired(); builder.Property(x => x.Ruta).HasMaxLength(300).IsRequired();
        builder.Property(x => x.DireccionIp).HasMaxLength(64); builder.Property(x => x.DetalleJson).HasMaxLength(2000);
        builder.HasIndex(x => x.FechaUtc); builder.HasIndex(x => x.UsuarioId);
    }
}
