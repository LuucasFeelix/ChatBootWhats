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
            string token = "EAAPkLZBJSP9kBO8mnlsiJ4C51mMbeykKa9frNRmq7LAfkraWR2gVAy3AE7uO5fOtSSaRtblN6ZCDA9ZChAaFZAyZAoOERiJ3GF8d5z54gyNcAxvu6B6GnnVSltzKBiBZBlL4JFy6p5kOfW0nBaDr9eSg86o9LWNk40zvRBZCQgcxLouwpJLIq87hFFlrGneBNdlrQZDZD";

            string idTelefone = "502112759653572";

            string telefone = "5516993837839";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v21.0/" + idTelefone + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Content = new StringContent("{\"messaging_product\": \"whatsapp\",\"recipient_type\": \"individual\",\"to\": \"" + telefone + "\",\"type\": \"text\",\"text\": {\"body\": \"Esta Funcionando\"}}");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
