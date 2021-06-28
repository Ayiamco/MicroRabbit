using Laundromat.SharedKernel.Core;
using System;
using System.Collections.Generic;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using System.Linq;
using System.IO;

namespace Laundromat.EmailService.ConsoleApp
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public int SendEmail(EmailMessage message, bool IsHTML = false)
        {
            try
            {
                MimeMessage emailMessage;
                emailMessage = CreateEmailMessage(message, IsHTML: IsHTML);
                Send(emailMessage);
                return 1;
            }
            catch
            {
                return 0;
            }
            
        }

        public async Task SendEmailAsync(EmailMessage message, bool IsHTML = false)
        {
            MimeMessage emailMessage;
            emailMessage = CreateEmailMessage(message, IsHTML: IsHTML);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message, bool IsHTML = false)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
            emailMessage.To.AddRange(GetMailboxAddress(message.To));
            emailMessage.Subject = message.Subject;
            BodyBuilder bodyBuilder;
            if (IsHTML)
                bodyBuilder = new BodyBuilder { HtmlBody = message.Content };
            else
                bodyBuilder = new BodyBuilder { TextBody = message.Content };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();


            return emailMessage;
        }

        private List<MailboxAddress> GetMailboxAddress(List<string> emails)
        {
            var mailAddresses = new List<MailboxAddress>();
            foreach(var email in emails)
                mailAddresses.Add(MailboxAddress.Parse(email));
            return mailAddresses;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }


        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
