using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChatBootWhatsapp.Controllers;

namespace ChatBootWhatsapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnviarController : ControllerBase
    {
        private readonly WhatsappService _whatsappService;

        public EnviarController(WhatsappService whatsappService)
        {
            _whatsappService = whatsappService;
        }

        [HttpPost("enviarMensagem")]
        public async Task<IActionResult> EnviarMensagem([FromBody] EnviarMensagemRequest request)
        {
   
            if (string.IsNullOrEmpty(request.Telefone) || string.IsNullOrEmpty(request.Mensagem))
            {
                return BadRequest("O número de telefone e a mensagem são obrigatórios.");
            }

            bool sucesso = await _whatsappService.EnviarMensagemAsync(request.Telefone, request.Mensagem);

            if (sucesso)
            {
                return Ok("Mensagem enviada com sucesso.");
            }
            else
            {
                return StatusCode(500, "Falha ao enviar a mensagem.");
            }
        }
    }

    public class EnviarMensagemRequest
    {
        public string Telefone { get; set; }
        public string Mensagem { get; set; }
    }
}