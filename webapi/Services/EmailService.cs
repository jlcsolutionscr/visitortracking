using System;
using System.Net.Mail;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using jlcsolutionscr.com.visitortracking.webapi.customclasses;

namespace jlcsolutionscr.com.visitortracking.webapi.services
{
    public class EmailService
    {
        private readonly AppSettings _settings;

        public EmailService(AppSettings settings)
        {
            _settings = settings;
        }

        public void SendEmail(string[] emailTo, string[] ccTo, string subject, string body, bool isBodyHtml, JArray strAttachments)
        {
            if (emailTo == null || emailTo.Length == 0)
            {
                throw new Exception("El valor del campo 'Enviar a:' no debe ser nulo o estar en blanco.");
            }
            if (subject == null || subject.Length == 0)
            {
                throw new Exception("El valor del campo 'Asunto:' no debe ser nulo o estar en blanco.");
            }
            if (body == null || body.Length == 0)
            {
                throw new Exception("El valor del campo 'Mensaje:' no debe ser nulo o estar en blanco.");
            }
            SmtpClient smtpClient = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };
            if (_settings.SSLHost == "S")
                smtpClient.EnableSsl = true;
            if (_settings.MailUser != "" & _settings.MailPassword != "")
                smtpClient.Credentials = new NetworkCredential(_settings.MailUser, _settings.MailPassword);
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(_settings.MailUser);
                message.Subject = subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.Body = body;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = isBodyHtml;
                foreach (string email in emailTo)
                {
                    message.To.Add(email);
                }
                if (ccTo != null && ccTo.Length > 0)
                {
                    foreach (string emailCc in ccTo)
                    {
                        message.CC.Add(emailCc);
                    }
                }
                Attachment attachment;
                foreach (JObject attachmentItem in strAttachments)
                {
                    string strAttachmentName = attachmentItem.Property("nombre").Value.ToString();
                    byte[] content = Convert.FromBase64String(attachmentItem.Property("contenido").Value.ToString());
                    attachment = new Attachment(new MemoryStream(content), strAttachmentName);
                    message.Attachments.Add(attachment);
                }
                try
                {
                    smtpClient.Send(message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            };
        }
    }
}