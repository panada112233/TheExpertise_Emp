using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string body)
    {
        try
        {
            //var smtpClient = new SmtpClient
            //{
            //    Host = _configuration["EmailSettings:SmtpServer"],
            //    Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            //    Credentials = new NetworkCredential(
            //        _configuration["EmailSettings:SenderEmail"],
            //        _configuration["EmailSettings:SenderPassword"]
            //    ),
            //    EnableSsl = false
            //};

            //var mailMessage = new MailMessage
            //{
            //    From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
            //    Subject = subject,
            //    Body = body,
            //    IsBodyHtml = true
            //};

            //mailMessage.To.Add(email);

            //await smtpClient.SendMailAsync(mailMessage);

            //Console.WriteLine($"Email sent successfully to: {email}");

            // Gmail SMTP settings
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            // Sender's credentials
            string emailFrom = "expertise.emps@gmail.com";
            string password = "kximrvpcchrpkqtm"; // Use app-specific password if 2FA is enabled
            string emailTo = email;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true; // Set to true if the body contains HTML

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                    Console.WriteLine("Email sent successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
