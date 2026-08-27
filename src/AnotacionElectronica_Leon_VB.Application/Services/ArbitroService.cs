using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;

namespace AnotacionElectronica_Leon_VB.Application.Services;

public class ArbitroService : IArbitroService
{
    private readonly IRepository<Arbitro> _arbitros;
    private readonly IUnitOfWork _unitOfWork;

    public ArbitroService(IRepository<Arbitro> arbitros, IUnitOfWork unitOfWork)
    {
        _arbitros = arbitros;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ArbitroDto>> ObtenerArbitrosAsync()
    {
        var arbitros = await _arbitros.GetAllAsync();

        return arbitros
            .OrderBy(a => a.Apellidos)
            .ThenBy(a => a.Nombre)
            .Select(MapToArbitroDto);
    }

    public async Task<ArbitroDto?> ObtenerPorIdAsync(Guid id)
    {
        var arbitro = await _arbitros.GetByIdAsync(id);
        return arbitro is null ? null : MapToArbitroDto(arbitro);
    }

    public async Task<ArbitroDto> CrearArbitroAsync(CrearArbitroDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Apellidos))
            throw new ArgumentException("El nombre y apellidos del árbitro son obligatorios.");

        var arbitro = new Arbitro(
            dto.Nombre.Trim(),
            dto.Apellidos.Trim(),
            dto.Rol,
            dto.NumeroLicencia?.Trim(),
            dto.Federacion?.Trim());

        await _arbitros.AddAsync(arbitro);
        await _unitOfWork.SaveChangesAsync();

        return MapToArbitroDto(arbitro);
    }

    private static ArbitroDto MapToArbitroDto(Arbitro a)
    {
        return new ArbitroDto(
            a.Id,
            a.Nombre,
            a.Apellidos,
            $"{a.Nombre} {a.Apellidos}",
            a.Rol,
            ObtenerTextoRol(a.Rol),
            a.NumeroLicencia,
            a.Federacion);
    }

    private static string ObtenerTextoRol(RolArbitro rol) => rol switch
    {
        RolArbitro.PrimerArbitro => "1.º Árbitro (Principal)",
        RolArbitro.SegundoArbitro => "2.º Árbitro (Asistente)",
        RolArbitro.Anotador => "Anotador Oficial",
        RolArbitro.AsistenteAnotador => "Asistente de Anotador (Líbero)",
        RolArbitro.JuezDeLinea => "Juez de Línea",
        _ => rol.ToString()
    };
}
