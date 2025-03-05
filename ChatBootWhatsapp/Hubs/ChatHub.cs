using Microsoft.AspNetCore.SignalR;

namespace ChatBootWhatsapp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NotifyNewMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceberMenssagem", message);
        }
    }
}
