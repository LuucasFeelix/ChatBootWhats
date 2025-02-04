using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ChatBootWhatsapp.Controllers
{
    public class WhatsappService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token = "SEU_TOKEN_AQUI"; // Melhor definir isso via configuração
        private readonly string _idTelefone = "502112759653572";

        public WhatsappService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> EnviarMensagemAsync(string telefone, string mensagem)
        {
            telefone = FormatarNumero(telefone);
            mensagem = mensagem.Replace("\r\n", "\\n").Replace("\n", "\\n");

            var json = $"{{\"messaging_product\": \"whatsapp\",\"recipient_type\": \"individual\",\"to\": \"{telefone}\",\"type\": \"text\",\"text\": {{\"body\": \"{mensagem}\"}}}}";

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://graph.facebook.com/v21.0/{_idTelefone}/messages")
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
    }
}
