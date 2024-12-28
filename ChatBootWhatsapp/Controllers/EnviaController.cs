using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ChatBootWhatsapp.Controllers
{
    public class EnviaController
    {
        [HttpGet]

        [Route("envia")]

        public async Task enviaAsync()
        {
            string token = "EAAPkLZBJSP9kBO3InVrYhgZCZANTiCod0JZB0OmH42SWx2isgdJrx9yZAQyc6sJ3GbVQweEQn6QjvrDIVM7vGXYlZBYFZCzXrEFwvzZBalhVX1ehBB1z9khp3lT1oyJGELFIzjtxLZAZBn4T2zZC9ZCZCFTw4UYa9DTOm0vVpjefuCoxQihWZBoVT1ajjq8gMOU0Tp9LfrZBdCvzLkvcb5EflcUByZA6MkN6Oz0Cqor07r0uf1czziIZD";

            string idTelefone = "502112759653572";

            string telefone = "5516993837839";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v21.0/" + idTelefone + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Content = new StringContent("{\"messaging_product\": \"whatsapp\",\"recipient_type\": \"individual\",\"to\": \"" + telefone + "\",\"type\": \"text\",\"text\": {\"body\": \"prueba\"}}");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
