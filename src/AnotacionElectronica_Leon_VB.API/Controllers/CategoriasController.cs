using AnotacionElectronica_Leon_VB.Application.DTOs;
using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AnotacionElectronica_Leon_VB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IRepository<Categoria> _categorias;
    private readonly IUnitOfWork _unitOfWork;

    public CategoriasController(IRepository<Categoria> categorias, IUnitOfWork unitOfWork)
        => (_categorias, _unitOfWork) = (categorias, unitOfWork);

    [HttpGet]
    public async Task<IActionResult> ObtenerTodas([FromQuery] bool incluirInactivas = true)
    {
        var categorias = await _categorias.GetAllAsync();
        return Ok(categorias
            .Where(x => incluirInactivas || x.Activa)
            .OrderBy(x => x.Nombre));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearCategoriaDto dto)
    {
        try
        {
            var existentes = await _categorias.GetAllAsync();
            if (existentes.Any(x => string.Equals(x.Nombre, dto.Nombre?.Trim(), StringComparison.OrdinalIgnoreCase)))
                return Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

            var categoria = new Categoria(dto.Nombre, dto.Descripcion);
            await _categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(ObtenerTodas), new { id = categoria.Id }, categoria);
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, ActualizarCategoriaDto dto)
    {
        var categoria = await _categorias.GetByIdAsync(id);
        if (categoria is null) return NotFound(new { mensaje = "Categoría no encontrada." });

        var existentes = await _categorias.GetAllAsync();
        if (existentes.Any(x => x.Id != id && string.Equals(x.Nombre, dto.Nombre?.Trim(), StringComparison.OrdinalIgnoreCase)))
            return Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

        try
        {
            categoria.Actualizar(dto.Nombre, dto.Descripcion);
            if (dto.Activa) categoria.Activar(); else categoria.Desactivar();
            _categorias.Update(categoria);
            await _unitOfWork.SaveChangesAsync();
            return Ok(categoria);
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Desactivar(Guid id)
    {
        var categoria = await _categorias.GetByIdAsync(id);
        if (categoria is null) return NotFound(new { mensaje = "Categoría no encontrada." });

        categoria.Desactivar();
        _categorias.Update(categoria);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}
