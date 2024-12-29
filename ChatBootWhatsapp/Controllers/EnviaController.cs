using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ChatBootWhatsapp.Controllers
{
    public class EnviaController : ControllerBase
    {
        [HttpGet]

        [Route("envia")]

        public async Task enviaAsync()
        {
            string token = "EAAPkLZBJSP9kBOwTPLV3wbBCUPI9jRroR6kRyUiHu7X9LAZBJOvnDlzQS7AqMuaAUNOugMJayyiudmVxsGBzfxrMnhk0fY3BlXu1ur2MZAipCjjKHTIMAS7T8Vk46nGVQOLVr1GUbSrB9qnCGWeriJAKmPDLejFgemV4XZAoovG3qxvbnywOMNmDmJpFAMo8kFbpIhAsyMWiLUZByAvRtdgXhhTZC0j1yZC9uUrWgqZABQQZD";

            string idTelefone = "502112759653572";

            string telefone = "5516993837839";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v21.0/" + idTelefone + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Content = new StringContent("{\"messaging_product\": \"whatsapp\",\"recipient_type\": \"individual\",\"to\": \"" + telefone + "\",\"type\": \"text\",\"text\": {\"body\": \"Olá, Mundo!!\"}}");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
