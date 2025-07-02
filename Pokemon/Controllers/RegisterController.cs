using Microsoft.AspNetCore.Mvc;
using Services.ModelView;
using Services.Services.Implement;
using Services.Services.Interface;

namespace Pokemon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    { 
        private readonly IRegisterService _registerService;
        private readonly IEmailService _emailService;

        public RegisterController(IRegisterService registerService, IEmailService emailService)
        {
            _registerService = registerService;
            _emailService = emailService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var result = await _registerService.Register(registerDto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result);

            // Tạo link kích hoạt (giả sử gửi kèm user email làm token, nên mã hóa hoặc thêm mã token thực tế)
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            string activationLink = $"{baseUrl}/api/activate?email={registerDto.Email}";

            // Gửi email xác thực
            await _emailService.SendActivationEmailAsync(registerDto.Email!, activationLink);

            return Ok(new ResponseDTO("Đăng ký thành công! Vui lòng kiểm tra email để kích hoạt tài khoản.", 200, true));

        }
    }
}
