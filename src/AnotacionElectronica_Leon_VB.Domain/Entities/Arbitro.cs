using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Arbitro
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public RolArbitro Rol { get; private set; }
    public string? NumeroLicencia { get; private set; }
    public string? Federacion { get; private set; }

    private Arbitro() { }

    public Arbitro(string nombre, string apellidos, RolArbitro rol, string? numeroLicencia = null, string? federacion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellidos))
            throw new ArgumentException("El nombre y apellidos del árbitro son obligatorios.");

        Id = Guid.NewGuid();
        Nombre = nombre.Trim();
        Apellidos = apellidos.Trim();
        Rol = rol;
        NumeroLicencia = numeroLicencia?.Trim();
        Federacion = federacion?.Trim();
    }
}
