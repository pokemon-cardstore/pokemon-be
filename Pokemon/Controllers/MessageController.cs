using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ModelView;
using Services.Services.Interface;

namespace Pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageDtoRequest request)
        {
            try
            {
                await _messageService.SendMessage(request);
                return Ok(new { success = true, message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal Server Error: {ex.Message}" });
            }
        }

        [HttpPost("admin")]
        public async Task<IActionResult> SendMessageAdmin([FromBody] MessageDtoRequest request)
        {
            try
            {
                await _messageService.SendMessageAdmin(request);
                return Ok(new { success = true, message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal Server Error: {ex.Message}" });
            }
        }

        [HttpGet("history/{CustomerId}")]
        public async Task<IActionResult> GetChatHistoryByCustomerId(int CustomerId)
        {
            try
            {
                var response = await _messageService.GetChatHistoryByCustomerId(CustomerId);
                if (response.CustomerName == null && response.response == null)
                {
                    return BadRequest("Customer does not exist");
                }
                else
                {
                    return Ok(new { customerName = response.CustomerName, messageHistory = response.response });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("get-chatbox/admin")]
        public async Task<IActionResult> GetChatBoxListForAdmin()
        {
            try
            {
                var response = await _messageService.GetChatBoxList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("get-chatbox/{CustomerId}")]
        public async Task<IActionResult> GetChatBoxByCustomerId(int CustomerId)
        {
            try
            {
                var response = await _messageService.GetChatBoxByCustomerId(CustomerId);
                if (response == null)
                {
                    return BadRequest("No chat");
                }
                else
                {
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

