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
            string token = "EAAPkLZBJSP9kBO6w6BCMKkwxZAgpmCsxC1etPKmJ54Md7nkHE2oA9riSv2PEDdn3vNwdtZCq5mIZCyvXwB4FPCZBN42SQDvTeh95tJx47EOGZAaIlA7sICZANCr1aggEIU0mDRyWuc0CURYwfZAmI5VJyJTuxYEJ4mGsDmjDlSnPa1XVGRhIVrnDYk006ZArQHB7Fhm9oIPDHLjbACNgTdoAp0PmidFCRJuyQJmkV9TZCtoI0ZD";

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
