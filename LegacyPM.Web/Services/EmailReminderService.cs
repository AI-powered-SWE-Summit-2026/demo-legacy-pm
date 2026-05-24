using System;
using System.Globalization;
using System.Net.Mail;
using System.Threading;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.Extensions.Configuration;

namespace LegacyPM.Web.Services
{
    public class EmailReminderService
    {
        private readonly IConfiguration _configuration;
        private readonly ProjectDbContext _dbContext;

        public EmailReminderService(IConfiguration configuration, ProjectDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public void SendReminder(Resource resource, string subject, string body, string culture)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            var fromAddress = _configuration["Smtp:From"] ?? "legacy-pm@example.com";
            var smtpHost = _configuration["Smtp:Host"] ?? "localhost";
            var smtpPort = int.TryParse(_configuration["Smtp:Port"], out var configuredPort) ? configuredPort : 25;

            var log = new NotificationLog
            {
                RecipientEmail = resource.Email,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now,
                Status = "Queued",
                Culture = culture
            };

            try
            {
                using var message = new MailMessage(fromAddress, resource.Email, subject, body);
                using var smtp = new SmtpClient(smtpHost, smtpPort);
                smtp.EnableSsl = true;
                smtp.Send(message);
                log.Status = "Sent";
            }
            catch (Exception ex)
            {
                log.Status = ex.Message;
            }

            _dbContext.NotificationLogs.Add(log);
            _dbContext.SaveChangesAsync().Wait();
        }
    }
}