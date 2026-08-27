using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public class EquipoService : IEquipoService
{
    private readonly IRepository<Equipo> _equipos;
    private readonly IRepository<Jugador> _jugadores;
    private readonly IUnitOfWork _unitOfWork;

    public EquipoService(IRepository<Equipo> equipos, IRepository<Jugador> jugadores, IUnitOfWork unitOfWork)
    {
        _equipos = equipos;
        _jugadores = jugadores;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<EquipoDto>> ObtenerEquiposAsync()
    {
        var todosEquipos = await _equipos.GetAllAsync();
        var todosJugadores = await _jugadores.GetAllAsync();

        var jugadoresPorEquipo = todosJugadores.GroupBy(j => j.EquipoId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return todosEquipos
            .OrderBy(e => e.Nombre)
            .Select(e =>
            {
                var jugadores = jugadoresPorEquipo.TryGetValue(e.Id, out var jList)
                    ? jList.OrderBy(j => j.NumeroCamiseta).Select(j => MapToJugadorDto(j, e.Nombre)).ToList()
                    : new List<JugadorDto>();

                return new EquipoDto(
                    e.Id,
                    e.Nombre,
                    e.NombreEntrenador,
                    e.NombreAsistente,
                    e.Categoria,
                    jugadores.Count,
                    jugadores);
            });
    }

    public async Task<EquipoDto?> ObtenerEquipoPorIdAsync(Guid id)
    {
        var equipo = await _equipos.GetByIdAsync(id);
        if (equipo is null) return null;

        var todosJugadores = await _jugadores.GetAllAsync();
        var jugadores = todosJugadores
            .Where(j => j.EquipoId == id)
            .OrderBy(j => j.NumeroCamiseta)
            .Select(j => MapToJugadorDto(j, equipo.Nombre))
            .ToList();

        return new EquipoDto(
            equipo.Id,
            equipo.Nombre,
            equipo.NombreEntrenador,
            equipo.NombreAsistente,
            equipo.Categoria,
            jugadores.Count,
            jugadores);
    }

    public async Task<EquipoDto> CrearEquipoAsync(CrearEquipoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.NombreEntrenador))
            throw new ArgumentException("El nombre del equipo y el nombre del entrenador son obligatorios.");

        var equipo = new Equipo(
            dto.Nombre.Trim(),
            dto.NombreEntrenador.Trim(),
            string.IsNullOrWhiteSpace(dto.Categoria) ? "Mayor" : dto.Categoria.Trim(),
            dto.NombreAsistente?.Trim());

        await _equipos.AddAsync(equipo);
        await _unitOfWork.SaveChangesAsync();

        return new EquipoDto(
            equipo.Id,
            equipo.Nombre,
            equipo.NombreEntrenador,
            equipo.NombreAsistente,
            equipo.Categoria,
            0,
            Array.Empty<JugadorDto>());
    }

    public async Task<JugadorDto> AgregarJugadorAsync(CrearJugadorDto dto)
    {
        var equipo = await _equipos.GetByIdAsync(dto.EquipoId)
            ?? throw new KeyNotFoundException($"No se encontró el equipo con ID {dto.EquipoId}.");

        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellidos))
            throw new ArgumentException("El nombre y apellidos del jugador son obligatorios.");

        if (dto.NumeroCamiseta <= 0)
            throw new ArgumentException("El número de camiseta debe ser mayor a 0.");

        var todosJugadores = await _jugadores.GetAllAsync();
        var yaExiste = todosJugadores.Any(j => j.EquipoId == dto.EquipoId && j.NumeroCamiseta == dto.NumeroCamiseta);
        if (yaExiste)
            throw new InvalidOperationException($"Ya existe un jugador con la camiseta #{dto.NumeroCamiseta} en este equipo.");

        var jugador = new Jugador(
            dto.Nombre.Trim(),
            dto.Apellidos.Trim(),
            dto.NumeroCamiseta,
            dto.Posicion,
            dto.EquipoId,
            dto.EsCapitan);

        await _jugadores.AddAsync(jugador);
        await _unitOfWork.SaveChangesAsync();

        return MapToJugadorDto(jugador, equipo.Nombre);
    }

    public async Task<IEnumerable<JugadorDto>> ObtenerJugadoresAsync(Guid? equipoId = null)
    {
        var todosEquipos = (await _equipos.GetAllAsync()).ToDictionary(e => e.Id, e => e.Nombre);
        var todosJugadores = await _jugadores.GetAllAsync();

        if (equipoId.HasValue && equipoId.Value != Guid.Empty)
        {
            todosJugadores = todosJugadores.Where(j => j.EquipoId == equipoId.Value);
        }

        return todosJugadores
            .OrderBy(j => todosEquipos.TryGetValue(j.EquipoId, out var nom) ? nom : "")
            .ThenBy(j => j.NumeroCamiseta)
            .Select(j => MapToJugadorDto(j, todosEquipos.TryGetValue(j.EquipoId, out var nom) ? nom : null));
    }

    private static JugadorDto MapToJugadorDto(Jugador j, string? nombreEquipo)
    {
        return new JugadorDto(
            j.Id,
            j.Nombre,
            j.Apellidos,
            j.NumeroCamiseta,
            j.Posicion,
            ObtenerTextoPosicion(j.Posicion),
            j.EsCapitan,
            j.EquipoId,
            nombreEquipo);
    }

    private static string ObtenerTextoPosicion(PosicionJugador pos) => pos switch
    {
        PosicionJugador.Colocador => "Colocador / Armador",
        PosicionJugador.RematadorConvocado => "Rematador / Punta",
        PosicionJugador.Central => "Central / Bloqueador",
        PosicionJugador.Opuesto => "Opuesto",
        PosicionJugador.Libero => "Líbero",
        _ => pos.ToString()
    };
}
