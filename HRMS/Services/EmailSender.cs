using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HRMS.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient();
        var basicCredential = new NetworkCredential(_configuration["EmailConfiguration:Email"], _configuration["EmailConfiguration:Password"]);
        var mailMessage = new MailMessage();
        var fromAddress = new MailAddress(_configuration["EmailConfiguration:Email"]);
        smtpClient.Host = _configuration["EmailConfiguration:Host"];
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = basicCredential;
        smtpClient.EnableSsl = bool.Parse(_configuration["EmailConfiguration:EnableSsl"]);
        smtpClient.Port = Convert.ToInt16(_configuration["EmailConfiguration:Port"]);
        string cc = _configuration["EmailConfiguration:CC"];
        if (cc != "")
        {
            mailMessage.CC.Add(cc);
        }
        mailMessage.From = fromAddress;
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = BodyContent(htmlMessage);
        mailMessage.To.Add(email);
        await smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage, string name)
    {
        var smtpClient = new SmtpClient();
        var basicCredential = new NetworkCredential(_configuration["EmailConfiguration:Email"], _configuration["EmailConfiguration:Password"]);
        var mailMessage = new MailMessage();
        var fromAddress = new MailAddress(_configuration["EmailConfiguration:Email"]);
        smtpClient.Host = _configuration["EmailConfiguration:Host"];
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = basicCredential;
        smtpClient.EnableSsl = true;
        smtpClient.Port = Convert.ToInt16(_configuration["EmailConfiguration:Port"]);
        mailMessage.From = fromAddress;
        mailMessage.Subject = subject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = BodyContent(htmlMessage, name);
        mailMessage.To.Add(email);
        await smtpClient.SendMailAsync(mailMessage);
    }

    private static string BodyContent(string mainContent, string Name = "")
    {
        string content = $"<div></div>";

        return content;
    }
}
