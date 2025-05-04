using System.Net;
using System.Net.Mail;

namespace Hirafeyat.EmailServices
{
    public class EmailSender : IEmailSender
    {
     
            private readonly IConfiguration _configuration;

            public EmailSender(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task SendEmailAsync(string toEmail, string subject, string message)
            {
                var email = _configuration["EmailSettings:Email"];
                var password = _configuration["EmailSettings:Password"];

                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }

            //public async Task SendEmailAsync(string toEmail, string subject, string message)
            //{
            //    var email = _configuration["EmailSettings:Email"];
            //    var password = _configuration["EmailSettings:Password"];

            //    var client = new SmtpClient("smtp.gmail.com", 587)
            //    {
            //        Credentials = new NetworkCredential(email, password),
            //        EnableSsl = true
            //    };

            //    var mailMessage = new MailMessage
            //    {
            //        From = new MailAddress(email),
            //        Subject = subject,
            //        Body = message,
            //        IsBodyHtml = true,
            //    };
            //    mailMessage.To.Add(toEmail);

            //    await client.SendMailAsync(mailMessage);
            //}
        

    }
}
