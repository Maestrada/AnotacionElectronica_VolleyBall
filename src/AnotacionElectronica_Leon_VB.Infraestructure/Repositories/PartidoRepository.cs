using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Domain.Entities;
using AnotacionElectronica_Leon_VB.Domain.Enums;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AnotacionElectronica_Leon_VB.Infraestructure.Repositories;

public class PartidoRepository : Repository<Partido>, IPartidoRepository
{
    public PartidoRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Partido?> GetPartidoConDetallesAsync(Guid partidoId)
    {
        return await _context.Partidos
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Include(p => p.Sets)
                .ThenInclude(s => s.Puntos)
            .FirstOrDefaultAsync(p => p.Id == partidoId);
    }

    public async Task<IEnumerable<Partido>> GetPartidosPorEstadoAsync(EstadoPartido estado)
    {
        return await _context.Partidos
            .Where(p => p.Estado == estado)
            .ToListAsync();
    }

    public async Task<IEnumerable<Partido>> GetPartidosPorEquipoAsync(Guid equipoId)
    {
        return await _context.Partidos
            .Where(p => p.EquipoLocalId == equipoId || p.EquipoVisitanteId == equipoId)
            .ToListAsync();
    }

    public async Task<int> ObtenerSiguienteSecuenciaEventoAsync(Guid partidoId)
    {
        var ultimaSecuenciaPersistida = await _context.EventosPartido
            .Where(e => e.PartidoId == partidoId)
            .Select(e => (int?)e.Secuencia)
            .MaxAsync() ?? 0;
        var ultimaSecuenciaPendiente = _context.EventosPartido.Local
            .Where(e => e.PartidoId == partidoId)
            .Select(e => e.Secuencia)
            .DefaultIfEmpty(0)
            .Max();
        return Math.Max(ultimaSecuenciaPersistida, ultimaSecuenciaPendiente) + 1;
    }

    public Task AgregarEventoAsync(EventoPartido evento) => _context.EventosPartido.AddAsync(evento).AsTask();

    public async Task<IReadOnlyList<EventoPartido>> ObtenerEventosAsync(Guid partidoId) =>
        await _context.EventosPartido.Where(e => e.PartidoId == partidoId)
            .OrderBy(e => e.Secuencia).ToListAsync();
}
