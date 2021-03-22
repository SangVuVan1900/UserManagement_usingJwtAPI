using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement_usingJwt1.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail);
    }

    public class SendGridMailService : IMailService
    {
        private IConfiguration _configuration;

        public SendGridMailService(IConfiguration configuration)
        {
            _configuration = configuration;
        } 

        public async Task SendEmailAsync(string toEmail)
        {
            var apiKey = _configuration["Lambda"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test11@gmail.com", "user 132");
            var to = new EmailAddress(toEmail);
            var subject = "run plzsssssss123zzz plzzz";
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with Csharpp</strong>";
            var msg = MailHelper
                .CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
