using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ModelView;
using Services.Services.Interface;

namespace Pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("/api/v1/createOrder")]
        public async Task<IActionResult> CreateOrder(List<OrderProductDto> cartItems)
        {
            try
            {
                if (!cartItems.Any())
                {
                    return BadRequest("No item to order");
                }
                var checkItem = await _orderService.ValidateItemInCart(cartItems);
                if (checkItem == -1)
                {
                    return BadRequest("Some items that not valid in your cart");
                }
                else if (checkItem == -2)
                {
                    return BadRequest("Some items that are not available now");
                }
                else if (checkItem == -3)
                {
                    return BadRequest("Some items that have higher quantity than quantity in stock");
                }
                else
                {
                    var url = await _orderService.CreateOrder(cartItems);
                    return Ok(new { url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/admin")]
        public async Task<IActionResult> GetAllOrdersForAdmin()
        {
            try
            {
                var response = await _orderService.GetAllOrders();
                if (!response.Any())
                {
                    return NotFound("No order in history");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/{CustomerId}")]
        public async Task<IActionResult> GetOrdersByCustomerId(int CustomerId)
        {
            try
            {
                var response = await _orderService.GetOrdersByCustomerId(CustomerId);
                if (!response.Any())
                {
                    return NotFound("No order in history");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("/api/v1/detail/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var response = await _orderService.GetOrderById(id);
                if (response == null)
                {
                    return NotFound("No order in history");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("/api/v1/updateStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromQuery] int id, int status)
        {
            try
            {
                var order = await _orderService._getOrderById(id);
                if (order == null)
                {
                    return NotFound("Order not found");
                }
                order.Status = status;
                await _orderService.UpdateOrderStatus(order);
                return Ok("Update success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
