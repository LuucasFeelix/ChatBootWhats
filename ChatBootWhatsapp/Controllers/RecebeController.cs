using ChatBootWhatsapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Linq;
using RiveScript;
using System.Text.RegularExpressions;
namespace ChatBootWhatsapp.Controllers
{
    public class RecebeController
    {
        
        [HttpGet]
        
        [Route("webhook")]
       
        public string Webhook(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string verify_token
        )
        {
            
            if (verify_token.Equals("oi"))
            {
                return challenge;
            }
            else
            {
                return "";
            }
        }
        
        [HttpPost]
        
        [Route("webhook")]

        public dynamic Dados([FromBody] WebHookResponseModel entry)
        {
            string mensagem_recebida = null;
            string id_whatsapp = null;
            string telefone_whatsapp = null;

            
            if (entry?.entry != null && entry.entry.Length > 0 &&
                entry.entry[0]?.changes != null && entry.entry[0].changes.Length > 0 &&
                entry.entry[0].changes[0]?.value?.messages != null && entry.entry[0].changes[0].value.messages.Length > 0 &&
                entry.entry[0].changes[0].value.messages[0]?.text?.body != null)
            {
                mensagem_recebida = entry.entry[0].changes[0].value.messages[0].text.body;
            }
            else
            {
                Console.WriteLine("Mensagem recebida está nula ou vazia.");
                return new { status = "Erro", mensagem = "Mensagem inválida." };
            }

            
            if (entry?.entry != null && entry.entry.Length > 0 &&
                entry.entry[0]?.changes != null && entry.entry[0].changes.Length > 0 &&
                entry.entry[0].changes[0]?.value?.messages != null && entry.entry[0].changes[0].value.messages.Length > 0 &&
                entry.entry[0].changes[0].value.messages[0]?.id != null)
            {
                id_whatsapp = entry.entry[0].changes[0].value.messages[0].id;
            }
            else
            {
                Console.WriteLine("ID do WhatsApp está nulo ou vazio.");
            }

            
            if (entry?.entry != null && entry.entry.Length > 0 &&
                entry.entry[0]?.changes != null && entry.entry[0].changes.Length > 0 &&
                entry.entry[0].changes[0]?.value?.messages != null && entry.entry[0].changes[0].value.messages.Length > 0 &&
                entry.entry[0].changes[0].value.messages[0]?.from != null)
            {
                telefone_whatsapp = entry.entry[0].changes[0].value.messages[0].from;
            }
            else
            {
                Console.WriteLine("Número de telefone do WhatsApp está nulo ou vazio.");
            }

            
            string resposta = string.Empty;
            try
            {
                var bot = new RiveScript.RiveScript(true);
                bot.loadFile("restaurante.rive");
                bot.sortReplies();


                if (!string.IsNullOrEmpty(mensagem_recebida))
                {
                    resposta = bot.reply("local-user", mensagem_recebida);
                }
                else
                {
                    resposta = "Desculpe, não entendi sua mensagem.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o RiveScript ou gerar resposta: {ex.Message}");
                return new { status = "Erro", mensagem = "Falha ao processar a resposta do bot." };
            }

            try
            {
                DadosModel dados = new DadosModel();
                dados.insert(mensagem_recebida, resposta, id_whatsapp, telefone_whatsapp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar dados no banco: {ex.Message}");
                return new { status = "Erro", mensagem = "Falha ao salvar dados no banco." };
            }


            try
            {
                enviaAsync(telefone_whatsapp, resposta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
            }

            return new { status = "Sucesso", resposta };
        }

        public async Task enviaAsync(string telefone, string mensagem)
        {
            telefone = FormatarNumero(telefone); // Ajusta o número antes de enviar

            mensagem = mensagem.Replace("\r\n", "\\n").Replace("\n", "\\n");
            string token = "EAAPkLZBJSP9kBO8mnlsiJ4C51mMbeykKa9frNRmq7LAfkraWR2gVAy3AE7uO5fOtSSaRtblN6ZCDA9ZChAaFZAyZAoOERiJ3GF8d5z54gyNcAxvu6B6GnnVSltzKBiBZBlL4JFy6p5kOfW0nBaDr9eSg86o9LWNk40zvRBZCQgcxLouwpJLIq87hFFlrGneBNdlrQZDZD";

            string idTelefone = "502112759653572";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v21.0/" + idTelefone + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);

            string json = $"{{\"messaging_product\": \"whatsapp\",\"recipient_type\": \"individual\",\"to\": \"{telefone}\",\"type\": \"text\",\"text\": {{\"body\": \"{mensagem}\"}}}}";
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();
        }

        private string FormatarNumero(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return "";

            telefone = Regex.Replace(telefone, @"\D", "");

            if (!telefone.StartsWith("55"))
            {
                telefone = "55" + telefone;
            }

            return telefone;
        }

    }
}