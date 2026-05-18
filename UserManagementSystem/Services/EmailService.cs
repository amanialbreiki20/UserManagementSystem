using MailKit.Net.Smtp;
using MimeKit;

namespace UserManagementSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendResetPasswordEmail(string toEmail, string resetLink)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("My Dashboard", "noreply@mydashboard.com"));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = "Reset Your Password";

            email.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Reset Your Password</h2>
                    <p>Click the link below to reset your password:</p>
                    <a href='{resetLink}' 
                       style='background-color:#0d6efd; color:white; padding:10px 20px; 
                              text-decoration:none; border-radius:5px;'>
                        Reset Password
                    </a>
                    <p>This link will expire in 1 hour.</p>
                "
            };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _configuration["EmailSettings:Host"],
                int.Parse(_configuration["EmailSettings:Port"]),
                false
            );
            smtp.Authenticate(
                _configuration["EmailSettings:Username"],
                _configuration["EmailSettings:Password"]
            );
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}