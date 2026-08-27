namespace AnotacionElectronica_Leon_VB.Domain.Entities;

public class Equipo
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string NombreEntrenador { get; private set; } = string.Empty;
    public string? NombreAsistente { get; private set; }
    public string Categoria { get; private set; } = string.Empty; // Ej: U17, U19, Mayor

    private readonly List<Jugador> _jugadores = new();
    public IReadOnlyCollection<Jugador> Jugadores => _jugadores.AsReadOnly();

    private Equipo() { }

    public Equipo(string nombre, string nombreEntrenador, string categoria, string? nombreAsistente = null)
    {
        Id = Guid.NewGuid();
        Nombre = nombre;
        NombreEntrenador = nombreEntrenador;
        Categoria = categoria;
        NombreAsistente = nombreAsistente;
    }

    public void AgregarJugador(Jugador jugador)
    {
        if (_jugadores.Any(j => j.NumeroCamiseta == jugador.NumeroCamiseta))
            throw new InvalidOperationException($"Ya existe un jugador con la camiseta #{jugador.NumeroCamiseta} en este equipo.");

        _jugadores.Add(jugador);
    }
}