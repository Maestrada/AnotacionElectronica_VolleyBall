namespace AnotacionElectronica_Leon_VB.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPartidoRepository Partidos { get; }
    Task<int> SaveChangesAsync();
}