using Microsoft.AspNetCore.SignalR;

namespace ChatBootWhatsapp.Hubs
{
    public class ChatHub : Hub
    {
        //Envia msg para um usuario especifico
        public async Task NotifyNewMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceberMenssagem", message);
        }

        //Envia msg para todos os usuarios conectados
        public async Task BroadcastMessage(string message)
        {
            await Clients.All.SendAsync("ReceberMenssagem", message);
        }
    }
}
