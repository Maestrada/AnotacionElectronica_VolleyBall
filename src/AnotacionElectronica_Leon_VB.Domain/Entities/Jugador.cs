using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Jugador
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public int NumeroCamiseta { get; private set; }
    public PosicionJugador Posicion { get; private set; }
    public bool EsCapitan { get; private set; }
    
    // Foreign Key
    public Guid EquipoId { get; private set; }
    public Equipo Equipo { get; private set; } = null!;

    private Jugador() { } // Requerido por EF Core

    public Jugador(string nombre, string apellidos, int numeroCamiseta, PosicionJugador posicion, Guid equipoId, bool esCapitan = false)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        Apellidos = apellidos;
        NumeroCamiseta = numeroCamiseta;
        Posicion = posicion;
        EquipoId = equipoId;
        EsCapitan = esCapitan;
    }
}