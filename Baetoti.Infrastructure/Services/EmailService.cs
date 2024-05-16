using System.Threading.Tasks;
using System;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Request.Email;
using System.Net.Mail;
using System.Net;
using Baetoti.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Baetoti.Core.Interface.Base;
using Baetoti.Core.Entites;

namespace Baetoti.Infrastructure.Services
{
    public class EmailService : IEmailService
    {

        private readonly BaetotiDbContext _dbContext;
        private readonly IAppLogger<AppException> _logger;

        public EmailService(BaetotiDbContext dbContext, IAppLogger<AppException> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(EmailRequest request)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                var EmailConfiguration = await _dbContext.EmailConfigurations.FirstOrDefaultAsync();
                if (EmailConfiguration == null)
                    return false;

                string MailAddress = EmailConfiguration.MailAddress;
                string MailPass = EmailConfiguration.MailPass;
                string MailSmtp = EmailConfiguration.MailSmtp;
                int Port = EmailConfiguration.Port;

                MailAddress senderadress = new MailAddress(MailAddress);
                MailMessage theMailMessage = new MailMessage(MailAddress, request.Receiver);

                theMailMessage.Body = request.Body;
                theMailMessage.Subject = request.Subject;
                SmtpClient theClient = new SmtpClient(MailSmtp);
                theClient.UseDefaultCredentials = false;

                NetworkCredential theCredential = new NetworkCredential(MailAddress, MailPass);
                theClient.Credentials = theCredential;
                theClient.Timeout = 10000000;
                theClient.Port = Port;

                theClient.EnableSsl = EmailConfiguration.EnableSsl;
                theMailMessage.IsBodyHtml = EmailConfiguration.IsBodyHtml;
                theMailMessage.Priority = request.MailPriority;
                theMailMessage.ReplyToList.Add(senderadress);
                theMailMessage.Sender = senderadress;

                string cc = request.Cc;
                if (!string.IsNullOrEmpty(cc))
                {
                    if (cc.IndexOf(',') > 0)
                    {
                        foreach (string CCAddress in cc.Split(','))
                        {
                            if (CCAddress != "")
                                theMailMessage.CC.Add(CCAddress);
                        }
                    }
                    else
                        theMailMessage.CC.Add(cc);
                }

                string bcc = request.Bcc;
                if (!string.IsNullOrEmpty(bcc))
                {
                    if (bcc.IndexOf(',') > 0)
                    {
                        foreach (string CCAddress in bcc.Split(','))
                        {
                            if (CCAddress != "")
                                theMailMessage.Bcc.Add(CCAddress);
                        }
                    }
                    else
                        theMailMessage.Bcc.Add(bcc);
                }

                theClient.Send(theMailMessage);
                theMailMessage.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex, request.API, request.UserID);
                return false;
            }
        }

    }
}
