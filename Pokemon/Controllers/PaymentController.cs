using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ModelView;
using Services.Services.Interface;

namespace Pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> CreatePayment([FromQuery] PaymentRequest parameters)
        {
            try
            {
                string appScheme = "pokemonshop";

                if (parameters.vnp_BankTranNo == null)
                {
                    string redirectUrl = $"{appScheme}://payment-failed?orderId={parameters.vnp_TxnRef}";

                    return Redirect(redirectUrl);
                }
                var result = await _paymentService.CreatePayment(parameters);

                if (result != null)
                {
                    string redirectUrl = $"{appScheme}://payment-success?status={result.TransactionStatus}&orderId={result.OrderId}";

                    return Redirect(redirectUrl);
                }
                else
                {
                    return NotFound("Order does not created");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

