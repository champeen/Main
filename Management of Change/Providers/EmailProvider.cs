using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Management_of_Change.Provider
{
    public class EmailProvider
    {
        //private readonly SmtpClient _smtpClient;
        private string _emailUrl;
        private string _emailUser;
        private string _emailPassword;
        private int _emailPort;
        private string _emailFrom;
        //private string _emailProcurementTo;
        //private string _emailSalesTo;
        //private string _emailGeneralTo;
        public EmailProvider(string emailUrl, string emailUser, string emailPassword, int emailPort, string emailFrom)
        {
            if (emailUrl == null)
                throw new ArgumentNullException("Error, emailUrl argument cannot be null.");
            if (emailUser == null)
                throw new ArgumentNullException("Error, emailUser argument cannot be null.");
            if (emailPassword == null)
                throw new ArgumentNullException("Error, emailPassword argument cannot be null.");
            if (emailPort == null)
                throw new ArgumentNullException("Error, emailPort argument cannot be null.");
            _emailUrl = emailUrl;
            _emailUser = emailUser;
            _emailPassword = emailPassword;
            _emailPort = emailPort;
            _emailFrom = emailFrom;
            //_emailProcurementTo = emailProcurementTo;
            //_emailSalesTo = emailSalesTo;
            //_emailGeneralTo = emailGeneralTo;
        }

        public async Task<bool> SendMessage(string subject, string body, string to, string cc, string bcc/*, string department, IFormFile? fileAttachment*/)
        {
            var smtpClient = new SmtpClient();
            smtpClient.Host = _emailUrl;
            smtpClient.Port = _emailPort;
            smtpClient.Credentials = new NetworkCredential(_emailUser, _emailPassword);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailFrom);
            if (to == null)
                throw new Exception("To Email Address Cannot Be Null");

            //switch (department)
            //{
            //    case "Sales":
            //        to = to + ";" + _emailSalesTo;
            //        break;
            //    case "Procurement":
            //        to = to + ";" + _emailProcurementTo;
            //        break;
            //    case "General":
            //        to = to + ";" + _emailGeneralTo;
            //        break;
            //    default:
            //        to = to + ";" + "Michael.Wilson@sksiltron.com";   // use myself for fallback temporarily
            //        break;
            //}

            foreach (var curr_address in to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MailAddress mytoAddress = new MailAddress(curr_address);
                mailMessage.To.Add(mytoAddress);
            }

            if (cc != null)
            {
                foreach (var curr_address in cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    MailAddress mytoAddress = new MailAddress(curr_address);
                    mailMessage.CC.Add(mytoAddress);
                }
            }

            if (bcc != null)
            {
                foreach (var curr_address in bcc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    MailAddress mytoAddress = new MailAddress(curr_address);
                    mailMessage.Bcc.Add(mytoAddress);
                }
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            //if (fileAttachment != null)
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        fileAttachment.CopyTo(ms);
            //        var fileBytes = ms.ToArray();
            //        mailMessage.Attachments.Add(new Attachment(new MemoryStream(fileBytes), fileAttachment.FileName, fileAttachment.ContentType));
            //        smtpClient.Send(mailMessage);
            //        //ms.Position = 0;
            //        //model.ContactUs_Sales.Attachments.Name = Utilities.Utilities.AddDateToFileName(fileAttachment.FileName);
            //    }
            //}
            //else
            //    smtpClient.Send(mailMessage);

            await smtpClient.SendMailAsync(mailMessage);

            return true;
        }
    }
}
