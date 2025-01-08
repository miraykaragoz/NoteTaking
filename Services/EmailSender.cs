using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace NoteTakingApp.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    
    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");
        string server = smtpSettings["Server"];
        int port = int.Parse(smtpSettings["Port"]);
        string senderEmail = smtpSettings["SenderEmail"];
        string username = smtpSettings["Username"];
        string password = smtpSettings["Password"];

        using (var client = new SmtpClient(server, port))
        {
            client.Credentials = new NetworkCredential(username, password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}