using AnotacionElectronica_Leon_VB.Application.Interfaces;
using AnotacionElectronica_Leon_VB.Infraestructure.Context;
using AnotacionElectronica_Leon_VB.Infraestructure.Repositories;

namespace AnotacionElectronica_Leon_VB.Infraestructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IPartidoRepository? _partidos;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IPartidoRepository Partidos => _partidos ??= new PartidoRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}