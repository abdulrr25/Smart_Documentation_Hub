using MailKit.Net.Smtp;
using MimeKit;

namespace Backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendResetEmail(string toEmail, string resetLink)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                "Smart Documentation Hub",
                _config["EmailSettings:Email"]
            ));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = "Reset Your Password";

            email.Body = new TextPart("html")
            {
                Text = $@"
                    <h3>Password Reset</h3>
                    <p>Click below to reset your password:</p>
                    <a href='{resetLink}'>Reset Password</a>
                    <p>This link expires in 15 minutes.</p>
                "
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["EmailSettings:Host"],
                int.Parse(_config["EmailSettings:Port"]),
                false
            );

            await smtp.AuthenticateAsync(
                _config["EmailSettings:Email"],
                _config["EmailSettings:AppPassword"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
