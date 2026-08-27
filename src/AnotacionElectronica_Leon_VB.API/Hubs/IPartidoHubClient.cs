using AnotacionElectronica_Leon_VB.API.DTOs;

namespace AnotacionElectronica_Leon_VB.API.Hubs;

public interface IPartidoHubClient
{
    Task RecibirMarcadorActualizado(MarcadorEnVivoDto marcador);
    Task RecibirRotacionActualizada(RotacionEnVivoDto rotacion);
    Task RecibirPartidoFinalizado(Guid partidoId, Guid equipoGanadorId);
}