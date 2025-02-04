using ChatBootWhatsapp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RiveScript;

namespace ChatBootWhatsapp.Controllers
{
    public class RecebeController : ControllerBase
    {
        private readonly WhatsappService _whatsappService;
        private readonly RiveScript.RiveScript _bot;

        public RecebeController(WhatsappService whatsappService)
        {
            _whatsappService = whatsappService;
            _bot = new RiveScript.RiveScript(true);
            _bot.loadFile("restaurante.rive");
            _bot.sortReplies();
        }

        [HttpGet]
        [Route("webhook")]
        public string Webhook(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            return verify_token.Equals("oi") ? challenge : "";
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<dynamic> Dados([FromBody] WebHookResponseModel entry)
        {
            string mensagemRecebida = ObterValorDaMensagem(entry, m => m.text?.body);
            string idWhatsapp = ObterValorDaMensagem(entry, m => m.id);
            string telefoneWhatsapp = ObterValorDaMensagem(entry, m => m.from);

            if (string.IsNullOrEmpty(mensagemRecebida))
            {
                Console.WriteLine("Mensagem recebida está nula ou vazia.");
                return new { status = "Erro", mensagem = "Mensagem inválida." };
            }

            string resposta;
            try
            {
                resposta = _bot.reply("local-user", mensagemRecebida) ?? "Desculpe, não entendi sua mensagem.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar resposta do bot: {ex.Message}");
                return new { status = "Erro", mensagem = "Falha ao processar a resposta do bot." };
            }

            try
            {
                DadosModel dados = new DadosModel();
                dados.insert(mensagemRecebida, resposta, idWhatsapp, telefoneWhatsapp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar dados no banco: {ex.Message}");
                return new { status = "Erro", mensagem = "Falha ao salvar dados no banco." };
            }

            try
            {
                await _whatsappService.EnviarMensagemAsync(telefoneWhatsapp, resposta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
            }

            return new { status = "Sucesso", resposta };
        }

        private static string ObterValorDaMensagem(WebHookResponseModel entry, Func<Messages, string> selector)
        {
            return entry?.entry?.FirstOrDefault()?.changes?.FirstOrDefault()?.value?.messages?.FirstOrDefault() is Messages msg ? selector(msg): null;
        }
    }
}
