using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Repository.UnitOfWork.Interface;
using Services.ModelView;
using Services.Services.Interface;




namespace Services.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public const string logoUrl = "https://dcassetcdn.com/design_img/3810593/741410/23472778/rj3pcb2rkbv0cby5ewr7eck7h6_image.png";


        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IUnitOfWork unitOfWork)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        private MimeMessage CreateMimeMessage(string recipientEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PokemonCardShop", _emailSettings.Sender));  
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = body };
            return message;
        }
        public Task SendActivationEmailAsync(string email, string activationLink)
        {
            Task.Run(async () =>
            {
                var message = CreateMimeMessage(email, "Account Activation", GenerateActivationEmailBody(activationLink));
                using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
                try
                {
                    await ConnectAndSendEmailAsync(smtpClient, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending the activation email to {Account}.", email);
                }
            });

            return Task.CompletedTask;
        }

        private string GenerateActivationEmailBody(string activationLink, string userName = "")
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; line-height: 1.6; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background-color: #ffffff; padding: 40px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
            <div style='text-align: center; margin-bottom: 30px;'>
                <!-- Logo shop bán Pokemon card -->
                <img src={{logoUrl}} alt='Pokemon Card Shop Logo' style='max-width: 150px;'>
            </div>
            
            <h1 style='color: #333333; font-size: 24px; margin-bottom: 20px; text-align: center;'>Chào mừng bạn đến với Pokemon Card Shop!</h1>
            
            <p style='color: #666666; font-size: 16px; margin-bottom: 20px;'>
                                 ""Xin chào khách hàng mới"" 
            </p>
            
            <p style='color: #666666; font-size: 16px; margin-bottom: 20px;'>
                Cảm ơn bạn đã đăng ký tài khoản tại cửa hàng của chúng tôi! Để hoàn tất đăng ký và bắt đầu mua sắm, vui lòng xác thực tài khoản của bạn bằng cách nhấn vào nút bên dưới:
            </p>
            
            <div style='text-align: center; margin-bottom: 30px;'>
                <a href='{{activationLink}}' 
                   style='display: inline-block; padding: 15px 30px; background-color: #ffcb05; color: #3b4cca; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold; text-transform: uppercase; transition: background-color 0.3s ease;'>
                    Kích hoạt tài khoản
                </a>
            </div>
            
            <p style='color: #666666; font-size: 16px; margin-bottom: 20px;'>
                Sau khi xác thực, bạn sẽ có thể:
            </p>
            
            <ul style='color: #666666; font-size: 16px; margin-bottom: 30px; padding-left: 20px;'>
                <li style='margin-bottom: 10px;'>🃏 Đặt mua các thẻ bài Pokémon chính hãng</li>
                <li style='margin-bottom: 10px;'>🚚 Theo dõi đơn hàng và trạng thái giao hàng</li>
                <li style='margin-bottom: 10px;'>💳 Nhận ưu đãi & khuyến mãi thành viên</li>
                <li style='margin-bottom: 10px;'>⭐ Xem lịch sử mua hàng & đánh giá sản phẩm</li>
            </ul>
            
            <p style='color: #666666; font-size: 14px; margin-bottom: 20px;'>
                Nếu bạn không thể nhấn vào nút trên, hãy sao chép và dán liên kết sau vào trình duyệt của bạn:
            </p>
            
            <p style='color: #666666; font-size: 14px; margin-bottom: 30px; word-break: break-all;'>
                {{activationLink}}
            </p>
            
            <div style='border-top: 1px solid #eeeeee; padding-top: 20px; margin-top: 20px;'>
                <p style='color: #666666; font-size: 14px; margin-bottom: 20px;'>
                    Mọi thắc mắc hoặc hỗ trợ, vui lòng liên hệ đội ngũ chăm sóc khách hàng qua email: support@pokemoncardshop.vn
                </p>
                
                <p style='color: #666666; font-size: 14px; margin-bottom: 20px;'>
                    Chúc bạn có trải nghiệm mua sắm tuyệt vời!
                </p>
            </div>

            <div style='border-top: 1px solid #eeeeee; padding-top: 20px; margin-top: 20px; text-align: center;'>
                <p style='color: #999999; font-size: 14px; margin-bottom: 10px;'>
                    Nếu bạn không thực hiện yêu cầu này, hãy bỏ qua email này.
                </p>
                <p style='color: #999999; font-size: 14px;'>
                    © 2024 PokemonCardShop. Đã đăng ký bản quyền.
                </p>
            </div>
        </div>
    </div>
</body>
</html>"


;

        }

        private async Task ConnectAndSendEmailAsync(MailKit.Net.Smtp.SmtpClient smtpClient, MimeMessage message)
        {
            // Use configuration values from appsettings.json
            await smtpClient.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
