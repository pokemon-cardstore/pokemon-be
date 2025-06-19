using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Pokemon.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) {
                    return BadRequest("Email cannot be empty");
                }
                if (string.IsNullOrEmpty(password)) { 
                    return BadRequest("Password cannot be empty");
                }
                IActionResult response = Unauthorized();
                IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
                string AdminEmail = configuration["Account:AdminAccount:email"];
                string AdminPassword = configuration["Account:AdminAccount:password"];
                var hashedPassword = await HashPassword(password);
                if (AdminEmail.Equals(email) && AdminPassword.Equals(hashedPassword))
                {
                    var accessToken = await _authService.GenerateAccessTokenForAdmin();
                    response = Ok(new { accessToken = accessToken });
                    return response;
                }
                var customer = await _authService.AuthenticateCustomer(email, hashedPassword);
                if (customer != null)
                {
                    var accessToken = await _authService.GenerateAccessTokenForCustomer(customer);
                    response = Ok(new { accessToken = accessToken });
                    return response;
                }
                return NotFound("Invalid email or password");
            }
            catch (Exception ex) { 
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private async Task<string> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        stringBuilder.Append(hashBytes[i].ToString("x2"));
                    }

                    return await Task.FromResult(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
