using Microsoft.AspNetCore.Mvc;
using Repository.UnitOfWork.Interface;
using System.Text;

namespace Pokemon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Activate([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return Content(("Email không hợp lệ."), "text/html", Encoding.UTF8);

            var customer = (await _unitOfWork.CustomerRepository.GetAllAsync())
                            .FirstOrDefault(c => c.Email == email);

            if (customer == null)
                return Content(("Không tìm thấy tài khoản."), "text/html", Encoding.UTF8);

            if (customer.Status == 1)
                return Content(("Tài khoản đã được kích hoạt trước đó. Vui lòng đăng nhập."), "text/html", Encoding.UTF8);

            customer.Status = 1;
            await _unitOfWork.SaveChangeAsync();

            return Content(HtmlResult("🎉 Kích hoạt tài khoản thành công!  Vui lòng đăng nhập."));
        }

        private string HtmlResult(string message)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Kích Hoạt Tài Khoản</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f7f7f7;
            text-align: center;
            padding: 50px;
        }}
        .box {{
            background-color: white;
            padding: 40px;
            border-radius: 10px;
            display: inline-block;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
      
    </style>
</head>
<body>
    <div class=""box"">
        <h2>Kích hoạt tài khoản thành công!</h2>
        <p>Vui lòng đăng nhập để tiếp tục.</p>
    </div>
</body>
</html>"
;
        }
    }
}
