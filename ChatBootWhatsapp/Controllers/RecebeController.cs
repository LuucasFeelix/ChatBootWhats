using ChatBootWhatsapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RiveScript;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using ChatBootWhatsapp.Hubs;

namespace ChatBootWhatsapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecebeController : ControllerBase
    {
        private readonly WhatsappService _whatsappService;
        private readonly RiveScript.RiveScript _bot;
        private readonly ILogger<RecebeController> _logger;
        private readonly IHubContext<ChatHub> _hubContext; // Injeção do SignalR Hub

        // HashSet para armazenar IDs de mensagens já processadas
        private static HashSet<string> _processedMessageIds = new HashSet<string>();
        private static DateTime _lastCleanup = DateTime.Now;

        public RecebeController(WhatsappService whatsappService, ILogger<RecebeController> logger, IHubContext<ChatHub> hubContext)
        {
            _whatsappService = whatsappService;
            _logger = logger;
            _hubContext = hubContext; // Inicialização do SignalR Hub
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
                _logger.LogInformation($"JSON recebido: {JsonConvert.SerializeObject(entry, Formatting.Indented)}");

                // Verifica se o campo entry está presente
                if (entry?.entry == null || !entry.entry.Any())
                {
                    _logger.LogWarning("Campo 'entry' ausente ou nulo no JSON recebido. Ignorando a requisição.");
                    return Ok();
                }

                // Limpa IDs de mensagens processadas periodicamente
                CleanupProcessedMessages();

                // Processa cada entrada
                foreach (var ent in entry.entry)
                {
                    // Verifica se o campo changes está presente
                    if (ent?.changes == null || !ent.changes.Any())
                    {
                        _logger.LogWarning("Campo 'changes' ausente ou nulo no JSON recebido. Ignorando a entrada.");
                        continue;
                    }

                    // Processa cada mudança
                    foreach (var change in ent.changes)
                    {
                        // Verifica se o campo value está presente
                        if (change?.value == null)
                        {
                            _logger.LogWarning("Campo 'value' ausente ou nulo no JSON recebido. Ignorando a mudança.");
                            continue;
                        }

                        // Verifica se é uma notificação de status
                        if (change.value.statuses != null && change.value.statuses.Any())
                        {
                            foreach (var status in change.value.statuses)
                            {
                                _logger.LogInformation($"Status recebido: {status.status} para a mensagem com ID {status.id}");
                                // Aqui você pode adicionar lógica para lidar com os status, se necessário.
                            }
                            continue; // Ignora o restante do processamento, pois é uma notificação de status
                        }

                        // Verifica se o campo messages está presente
                        var messages = change.value.messages;
                        if (messages == null || !messages.Any())
                        {
                            _logger.LogInformation("Campo 'messages' ausente ou vazio. Esta pode ser uma notificação de status ou entrega.");
                            continue;
                        }

                        // Processa cada mensagem
                        foreach (var message in messages)
                        {
                            string idWhatsapp = message.id;

                            // Verifica se a mensagem já foi processada
                            if (_processedMessageIds.Contains(idWhatsapp))
                            {
                                _logger.LogInformation($"Mensagem com ID {idWhatsapp} já foi processada. Ignorando.");
                                continue;
                            }

                            // Adiciona o ID da mensagem à lista de processados
                            _processedMessageIds.Add(idWhatsapp);

                            // Verifica se o campo 'text' está presente e não é nulo
                            if (message.text == null || string.IsNullOrEmpty(message.text.body))
                            {
                                _logger.LogWarning("Campo 'text' ausente ou nulo na mensagem. Ignorando.");
                                continue; // Ignora mensagens inválidas
                            }

                            string mensagemRecebida = message.text.body; // Agora podemos acessar message.text.body com segurança
                            string telefoneWhatsapp = message.from;

                            // Obtém a resposta do RiveScript
                            string resposta;
                            try
                            {
                                resposta = _bot.reply("local-user", mensagemRecebida) ?? "Desculpe, não entendi sua mensagem.";
                                _logger.LogInformation($"Resposta do RiveScript: {resposta}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Erro ao processar resposta do bot: {ex.Message}. StackTrace: {ex.StackTrace}");
                                continue; // Ignora erros de processamento
                            }

                            // Envia a resposta para o WhatsApp
                            try
                            {
                                bool sucesso = await _whatsappService.EnviarMensagemAsync(telefoneWhatsapp, resposta);
                                if (sucesso)
                                {
                                    _logger.LogInformation($"Mensagem enviada para {telefoneWhatsapp} com sucesso.");
                                }
                                else
                                {
                                    _logger.LogError($"Falha ao enviar mensagem para {telefoneWhatsapp}.");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Erro ao enviar mensagem para {telefoneWhatsapp}: {ex.Message}. StackTrace: {ex.StackTrace}");
                            }
                        }
                    }
                }

                return Ok(new { status = "Sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro inesperado: {ex.Message}. StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { status = "Erro", mensagem = "Erro interno no servidor." });
            }
        }

        // Método para limpar IDs de mensagens processadas periodicamente
        private void CleanupProcessedMessages()
        {
            if ((DateTime.Now - _lastCleanup).TotalHours > 1) // Limpa a cada hora
            {
                _processedMessageIds.Clear();
                _lastCleanup = DateTime.Now;
                _logger.LogInformation("IDs de mensagens processadas limpos.");
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