using AnotacionElectronica_Leon_VB.Application.DTOs;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public interface IEquipoService
{
    Task<IEnumerable<EquipoDto>> ObtenerEquiposAsync();
    Task<EquipoDto?> ObtenerEquipoPorIdAsync(Guid id);
    Task<EquipoDto> CrearEquipoAsync(CrearEquipoDto dto);
    Task<JugadorDto> AgregarJugadorAsync(CrearJugadorDto dto);
    Task<IEnumerable<JugadorDto>> ObtenerJugadoresAsync(Guid? equipoId = null);
}
