using Microsoft.AspNetCore.SignalR;

namespace AnotacionElectronica_Leon_VB.API.Hubs;

public class PartidoHub : Hub<IPartidoHubClient>
{
    // El frontend se suscribe al canal específico de este partido
    public async Task UnirseAlPartido(string partidoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, partidoId);
    }

    // El frontend abandona la visualización del partido
    public async Task SalirDelPartido(string partidoId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, partidoId);
    }
}