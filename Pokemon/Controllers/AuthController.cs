using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ModelView;
using Services.Services.Interface;

namespace Pokemon.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email cannot be empty");
                }
                if (string.IsNullOrEmpty(password))
                {
                    return BadRequest("Password cannot be empty");
                }

                IActionResult response = Unauthorized();

                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                string AdminEmail = configuration["Account:AdminAccount:email"];
                string AdminPassword = configuration["Account:AdminAccount:password"];

                // So sánh trực tiếp không hash nữa
                if (AdminEmail.Equals(email) && AdminPassword.Equals(password))
                {
                    var accessToken = await _authService.GenerateAccessTokenForAdmin();
                    return Ok(new { accessToken });
                }

                // Gọi service với password plain
                var customer = await _authService.AuthenticateCustomer(email, password);
                if (customer != null)
                {
                    var accessToken = await _authService.GenerateAccessTokenForCustomer(customer);
                    return Ok(new { accessToken });
                }

                return NotFound("Invalid email or password");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdToken))
                    return BadRequest("IdToken is required");

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

                var email = payload.Email;
                var name = payload.Name;
                var picture = payload.Picture;

                var customer = await _authService.GetCustomerByEmail(email);
                if (customer == null)
                {
                    customer = await _authService.CreateCustomerFromGoogle(email, name, picture);
                }

                var accessToken = await _authService.GenerateAccessTokenForCustomer(customer);
                return Ok(new { accessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Google login failed: {ex.Message}");
            }
        }
    }
}
