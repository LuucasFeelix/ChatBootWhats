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
            string token = "EAAWRxZBz2YGQBAGlneZCWwJgwkP2ZBYEk5PoHDL9gsZCgxY3C0IpMTgHHW9cAi5PSApuzRSgUu5flKlTGgZCl1uxL6sjCqixRmkhldhOvVOYYIVOZBOyQ6WV3bm3jZCmWlfsdGb9gU8ZCddrHmFNfJ2FyhjJ43G9VgOtNuVTcZAoZAYoyZA1uYZBZB0ICPwDgDcpGscug9L2USOZAKgAZDZD";

            string idTelefono = "111290641852610";

            string telefono = "527121122441";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v15.0/" + idTelefono + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);
            request.Content = new StringContent("{\"messaging_product\": \"whatsapp\",\"recipient_type\": \"individual\",\"to\": \"" + telefono + "\",\"type\": \"text\",\"text\": {\"body\": \"prueba\"}}");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();
        }
    }
}
