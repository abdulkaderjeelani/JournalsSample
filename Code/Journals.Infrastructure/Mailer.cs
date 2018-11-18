using Journals.Infrastructure.Interface;
using System;
using System.Net.Mail;

namespace Journals.Infrastructure
{
    /// <summary>
    /// A simple mailer class, that sends mail synchronours
    /// </summary>
    public class Mailer : Journals.Infrastructure.Interface.IMailer
    {
        private readonly IExceptionHandler _exHanlder = null;

        public Mailer(IExceptionHandler exHanlder)
        {
            _exHanlder = exHanlder;
        }

        public void SendMail(string from, string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from);
                mailMessage.To.Add(new MailAddress(to.Trim()));
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                //TODO:.. wrap the ex, verify in exhandler and  put it into some queue and process these later on !!!!IMPORTANT!!!
                _exHanlder.HandleException(ex);
            }
        }
    }
}