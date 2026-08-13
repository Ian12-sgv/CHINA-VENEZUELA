using Microsoft.AspNetCore.SignalR;

namespace ChinaVenezuela.Api.Hubs;

public sealed class ActualizacionesHub : Hub
{
    public const string DatosActualizados = "DatosActualizados";
}