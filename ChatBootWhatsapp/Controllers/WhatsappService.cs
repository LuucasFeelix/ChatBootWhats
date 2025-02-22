using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace ChatBootWhatsapp.Controllers
{
    public class WhatsappService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _idTelefone;

        public WhatsappService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;


            _token = configuration["Whatsapp:Token"];
            _idTelefone = configuration["Whatsapp:IdTelefone"];
        }

        public async Task<bool> EnviarMensagemAsync(string telefone, string mensagem)
        {
            telefone = FormatarNumero(telefone);
            mensagem = FormatarMensagem(mensagem);

            var message = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = telefone,
                type = "text",
                text = new { body = mensagem }
            };

            var json = JsonConvert.SerializeObject(message);

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.facebook.com/v22.0/{_idTelefone}/messages")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private string FormatarNumero(string telefone)
        {
            telefone = Regex.Replace(telefone, @"\D", "");
            return telefone.StartsWith("55") ? telefone : "55" + telefone;
        }

        private string FormatarMensagem(string mensagem)
        {
            return mensagem;
        }
    }
}