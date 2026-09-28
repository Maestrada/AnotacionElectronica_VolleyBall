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
    public string? ImagenUrl { get; private set; }
    public string? DocumentoIdentidadUrl { get; private set; }
    public string? DocumentoAvalForaneoUrl { get; private set; }
    public bool EsJugadorForaneo { get; private set; }
    
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

    public void Actualizar(string nombre, string apellidos, int numeroCamiseta, PosicionJugador posicion, bool esCapitan)
    {
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellidos) || numeroCamiseta <= 0)
            throw new ArgumentException("Nombre, apellidos y número de camiseta válido son obligatorios.");
        Nombre = nombre.Trim();
        Apellidos = apellidos.Trim();
        NumeroCamiseta = numeroCamiseta;
        Posicion = posicion;
        EsCapitan = esCapitan;
    }

    public void ActualizarDocumentos(bool esForaneo, string? identidadUrl, string? avalForaneoUrl)
    {
        if (esForaneo && string.IsNullOrWhiteSpace(avalForaneoUrl)) throw new ArgumentException("El aval es obligatorio para un jugador foráneo.");
        EsJugadorForaneo = esForaneo; DocumentoIdentidadUrl = identidadUrl?.Trim(); DocumentoAvalForaneoUrl = avalForaneoUrl?.Trim();
    }
    public void EstablecerImagen(string? imagenUrl) => ImagenUrl = string.IsNullOrWhiteSpace(imagenUrl) ? null : imagenUrl.Trim();
}
