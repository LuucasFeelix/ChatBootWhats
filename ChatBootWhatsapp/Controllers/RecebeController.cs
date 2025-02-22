using ChatBootWhatsapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using RiveScript;
using Newtonsoft.Json;

namespace ChatBootWhatsapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecebeController : ControllerBase
    {
        private readonly WhatsappService _whatsappService;
        private readonly RiveScript.RiveScript _bot;
        private readonly ILogger<RecebeController> _logger;

        public RecebeController(WhatsappService whatsappService, ILogger<RecebeController> logger)
        {
            _whatsappService = whatsappService;
            _logger = logger;
            _bot = new RiveScript.RiveScript(true);

            try
            {
                string filePath = "restaurante.rive";
                if (System.IO.File.Exists(filePath))
                {
                    _bot.loadFile(filePath);
                    _bot.sortReplies();
                    _logger.LogInformation("Arquivo RiveScript carregado com sucesso.");
                }
                else
                {
                    _logger.LogError($"Erro: Arquivo {filePath} não encontrado!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar arquivo RiveScript: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("webhook")]
        public IActionResult Webhook(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.challenge")] string challenge,
            [FromQuery(Name = "hub.verify_token")] string verify_token)
        {
            if (verify_token == "oi")
            {
                return Ok(challenge);
            }
            _logger.LogWarning("Tentativa de verificação com token inválido.");
            return Unauthorized();
        }

        [HttpPost]
        [Route("webhook")]
        public async Task<IActionResult> Dados([FromBody] WebHookResponseModel entry)
        {
            try
            {
                _logger.LogInformation($"JSON recebido: {JsonConvert.SerializeObject(entry)}");


                if (entry == null || entry.entry == null || entry.entry.FirstOrDefault()?.changes?.FirstOrDefault()?.value?.messages == null)
                {
                    _logger.LogWarning("Campo 'messages' ausente ou nulo no JSON recebido.");
                    return BadRequest(new { status = "Erro", mensagem = "Campo 'messages' ausente ou nulo." });
                }

                string mensagemRecebida = ObterValorDaMensagem(entry, m => m.text?.body);
                string idWhatsapp = ObterValorDaMensagem(entry, m => m.id);
                string telefoneWhatsapp = ObterValorDaMensagem(entry, m => m.from);

                if (string.IsNullOrEmpty(mensagemRecebida))
                {
                    _logger.LogWarning("Mensagem recebida está nula ou vazia.");
                    return BadRequest(new { status = "Erro", mensagem = "Mensagem inválida." });
                }

                string resposta;
                try
                {
                    resposta = _bot.reply("local-user", mensagemRecebida) ?? "Desculpe, não entendi sua mensagem.";
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao processar resposta do bot: {ex.Message}. StackTrace: {ex.StackTrace}");
                    return StatusCode(500, new { status = "Erro", mensagem = "Falha ao processar a resposta do bot." });
                }

                try
                {
                    DadosModel dados = new DadosModel();

                    bool salvo = dados.Insert(mensagemRecebida, resposta.Replace("\\n", "\n"), idWhatsapp, telefoneWhatsapp);

                    if (salvo)
                    {
                        _logger.LogInformation($"Mensagem salva no banco: Recebida: {mensagemRecebida}, Enviada: {resposta}, ID: {idWhatsapp}, Telefone: {telefoneWhatsapp}");
                    }
                    else
                    {
                        _logger.LogError("Falha ao salvar mensagem no banco, mas continuando com o envio da resposta.");
                    }

                    string respostaFormatada = resposta.Trim();

                    await _whatsappService.EnviarMensagemAsync(telefoneWhatsapp, respostaFormatada);
                    _logger.LogInformation($"Mensagem enviada para {telefoneWhatsapp} com sucesso.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao salvar dados no banco ou enviar mensagem: {ex.Message}. StackTrace: {ex.StackTrace}");
                    return StatusCode(500, new { status = "Erro", mensagem = "Erro ao salvar no banco ou enviar mensagem." });
                }

                return Ok(new { status = "Sucesso", resposta, idWhatsapp, telefoneWhatsapp });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro inesperado: {ex.Message}. StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { status = "Erro", mensagem = "Erro interno no servidor." });
            }
        }

        private static string ObterValorDaMensagem(WebHookResponseModel entry, Func<Messages, string> selector)
        {
            try
            {
                if (entry?.entry?.FirstOrDefault()?.changes?.FirstOrDefault()?.value?.messages != null)
                {
                    var msg = entry.entry[0].changes[0].value.messages.FirstOrDefault();
                    if (msg != null)
                    {
                        return selector(msg);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter valor da mensagem: {ex.Message}");
                return null;
            }
        }
    }
}
