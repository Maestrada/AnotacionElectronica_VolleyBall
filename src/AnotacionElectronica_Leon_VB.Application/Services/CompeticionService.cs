using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public class CompeticionService : ICompeticionService
{
    private readonly IRepository<Competicion> _competiciones;
    private readonly IUnitOfWork _unitOfWork;

    public CompeticionService(IRepository<Competicion> competiciones, IUnitOfWork unitOfWork)
    {
        _competiciones = competiciones;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CompeticionDto>> ObtenerCompeticionesAsync()
    {
        var comps = await _competiciones.GetAllAsync();

        return comps
            .OrderBy(c => c.Nombre)
            .ThenBy(c => c.Edicion)
            .Select(MapToCompeticionDto);
    }

    public async Task<CompeticionDto?> ObtenerPorIdAsync(Guid id)
    {
        var comp = await _competiciones.GetByIdAsync(id);
        return comp is null ? null : MapToCompeticionDto(comp);
    }

    public async Task<CompeticionDto> CrearCompeticionAsync(CrearCompeticionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Edicion))
            throw new ArgumentException("El nombre y la edición de la competición son obligatorios.");

        var comp = new Competicion(
            dto.Nombre.Trim(),
            dto.Edicion.Trim(),
            dto.Categoria.Trim(),
            dto.Rama.Trim(),
            dto.Organizador?.Trim(),
            dto.SedePrincipal?.Trim());

        await _competiciones.AddAsync(comp);
        await _unitOfWork.SaveChangesAsync();

        return MapToCompeticionDto(comp);
    }

    private static CompeticionDto MapToCompeticionDto(Competicion c)
    {
        return new CompeticionDto(
            c.Id,
            c.Nombre,
            c.Edicion,
            c.Categoria,
            c.Rama,
            c.Organizador,
            c.SedePrincipal);
    }
}
