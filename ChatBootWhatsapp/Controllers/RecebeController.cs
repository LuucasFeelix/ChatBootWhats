using ChatBootWhatsapp.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Linq;
using RiveScript;
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
            
            string mensagem_recebida = entry.entry[0].changes[0].value.messages[0].text.body;
            
            string id_whatsapp = entry.entry[0].changes[0].value.messages[0].id;
            
            string telefone_whatsapp = entry.entry[0].changes[0].value.messages[0].from;

            var bot = new RiveScript.RiveScript(true);
            //bot.setDebug(true);
            bot.loadFile("restaurante.rive");
            bot.sortReplies();
            var resposta = bot.reply("local-user", mensagem_recebida);


            //string texto = "mensagem_recebida = " + mensagem_recebida + Environment.NewLine;
            //texto = texto + "id_whatsapp = " + id_whatsapp + Environment.NewLine;
            //texto = texto + "telefone_whatsapp = " + telefone_whatsapp + Environment.NewLine;
            //texto = texto + "resposta = " + resposta + Environment.NewLine;
            //File.WriteAllText("texto.txt", texto);

            DadosModel dados = new DadosModel();
            dados.insert(mensagem_recebida, resposta, id_whatsapp, telefone_whatsapp);
            
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return response;
        }



    }
}