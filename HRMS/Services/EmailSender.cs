using HRMS.Resources;
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
        string content = $"<div><div style='margin: auto; max-width: 600px;'>" +
            $"<div><p style='font-size: 20px;'>" +
            $"<b>{Resource.Dear}</b> {Name},<br/><br/>" +
            $"{mainContent}" +
            $"</p></div>" +
            $"<div>" +
            $"<p style='font-size: 17px; margin-bottom: 0px; text-align: left;'>{Resource.BestRegards}</p>" +
            $"<p style='margin-top: 3px;text-align: left;'>{Resource.RiinvestCollege}</p>" +
            $"</div>" +
            $"<div style='margin-top: 10px;'>" +
            $"<div>" +
            $"<img src='~/ favicon.ico' alt='Riinvest logo' style='opacity: .8; margin-top: -.5rem; margin - right: .2rem; height: 33px;'/>" +
            $"<span>{Resource.RiinvestCollege}</span>" +
            $"</div>" +
            $"</div>" +
            $"</div></div>";

        return content;
    }
}
