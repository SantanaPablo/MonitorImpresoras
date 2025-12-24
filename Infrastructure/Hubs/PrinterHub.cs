using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs
{
    public class PrinterHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}