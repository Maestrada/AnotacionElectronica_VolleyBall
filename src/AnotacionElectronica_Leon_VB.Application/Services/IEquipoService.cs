using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface IEquipoService
{
    Task<IEnumerable<EquipoDto>> ObtenerEquiposAsync();
    Task<EquipoDto?> ObtenerEquipoPorIdAsync(Guid id);
    Task<EquipoDto> CrearEquipoAsync(CrearEquipoDto dto);
    Task<EquipoDto> ActualizarEquipoAsync(Guid id, ActualizarEquipoDto dto);
    Task EliminarEquipoAsync(Guid id);
    Task<JugadorDto> AgregarJugadorAsync(CrearJugadorDto dto);
    Task<IEnumerable<JugadorDto>> ObtenerJugadoresAsync(Guid? equipoId = null);
    Task<JugadorDto?> ObtenerJugadorPorIdAsync(Guid id);
    Task<JugadorDto> ActualizarJugadorAsync(Guid id, ActualizarJugadorDto dto);
    Task<JugadorDto> ActualizarDocumentacionJugadorAsync(Guid id, ActualizarDocumentacionJugadorDto dto);
    Task<JugadorDto> ActualizarImagenJugadorAsync(Guid id, string? imagenUrl);
    Task EliminarJugadorAsync(Guid id);
}
