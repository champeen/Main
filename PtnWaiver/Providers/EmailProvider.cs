using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace PtnWaiver.Provider
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

        public async Task SendMessage(string subject, string body, string to, string cc, string bcc, string? priority = "Normal"/*, string department, IFormFile? fileAttachment*/)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_emailFrom);
                if (to == null)
                    throw new Exception("To Email Address Cannot Be Null");

                mailMessage.Subject = "TEST: " + subject;
                mailMessage.Body = "TEST: " + body;
                mailMessage.IsBodyHtml = true;
                if (priority == "High")
                {
                    mailMessage.Priority = MailPriority.High;
                    mailMessage.Subject = "HIGH PRIORITY: " + mailMessage.Subject;
                    mailMessage.Body = @"<strong style=""color:Red"">HIGH PRIORITY!</strong><br/><br/>" + mailMessage.Body;
                }

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

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _emailUrl;
                    smtpClient.Port = _emailPort;
                    smtpClient.Credentials = new NetworkCredential(_emailUser, _emailPassword);
                    smtpClient.EnableSsl = true;

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

                    smtpClient.Send(mailMessage);
                }
            }
            //return true;
        }
    }
}

/*
UPDATE

Per MSDN:

After calling SendAsync, you must wait for the e-mail transmission to complete before attempting to send another e-mail message using Send or SendAsync.

http://msdn.microsoft.com/en-us/library/x5x13z6h.aspx

So given your situation, there is almost no benefit to using SendAsync over Send. Your loop is probably stomping on something since you do not wait for the previous SendAsync to complete.

Here are a few thoughts:

SendAsync will perform almost the same as Send if you are sending a bunch of emails. Just use Send.
If you need parallel sending, use a Producer/Consumer pattern. One (or more) producing threads dump stuff into a queue to send, and multiple consuming threads each use one SmtpClient to send messages. This pattern is amazingly simple to implement with a BlockingCollection. See the example in MSDN http://msdn.microsoft.com/en-us/library/dd267312.aspx
If you use enough threads, your SMTP server will be the bottleneck. Be aware of when you are overloading it.
 */