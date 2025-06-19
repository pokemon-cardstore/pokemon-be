using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.ModelView;
using Services.Service.Interface;

namespace RolexApplication_Backend.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers() {
            try
            {
                var response = await _customerService.GetAllCustomers();
                if (!response.Any())
                {
                    return NotFound("No customer in system");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var response = await _customerService.GetCustomerById(id);
                if (response == null)
                {
                    return NotFound("No customer match this id");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAccount([FromBody] CustomerDtoRequest request)
        {
            try
            {
                //if (request == null)
                //{
                //    return BadRequest("Customer information cannot empty");
                //}
                if (request.Email.IsNullOrEmpty() || request.Password.IsNullOrEmpty())
                {
                    return BadRequest("Please fill at least email and password fields");
                }
                if (await _customerService.IsDublicatedEmail(request.Email))
                {
                    return BadRequest("Email is already exist");
                }
                await _customerService.CreateCustomerAccount(request);
                return Ok("Create success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerInformation([FromBody] UpdateCustomerDtoRequest request, int id)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Customer information cannot be empty" });
                }
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Address) || string.IsNullOrEmpty(request.Phone) || request.DoB == null)
                {
                    return BadRequest(new { success = false, message = "Please fill all fields" });
                }
                await _customerService.UpdateCustomerInformation(id, request);
                return Ok(new { success = true, message = "Update information success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal Server Error: {ex.Message}" });
            }
        }


        [HttpPut("status")]
        public async Task<IActionResult> UpdateCustomerStatus([FromQuery] int id, [FromQuery] int status)
        {
            try
            {
                await _customerService.UpdateCustomerStatus(id, status);
                return Ok("Update status success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
