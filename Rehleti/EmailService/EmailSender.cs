using System.Net;
using System.Net.Mail;

namespace Rehleti.EmailService
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string body)
        {
            string MyEmail = "rehletibooking@gmail.com";
            string MyPassword = "lnojsckwktazbdri";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(MyEmail, MyPassword)
            };
            return client.SendMailAsync(
                new MailMessage(from: MyEmail, to: email, subject, body)
                );
        }
    }
}