namespace WHITELABEL.Web.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Mail;
    using System.Web;
    using Data;
    using Models;
    public class EmailHelper
    {
        public void SendUserEmail(string ToEmail, string Subject, string Message)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            try
            {
                //mail.From = new MailAddress("donotreply@simtick.com", "URWEX Team");
                mail.From = new MailAddress("iotegralpvt@gmail.com", "Iotegral Team");
                mail.To.Add(ToEmail);
                mail.Subject = Subject;
                mail.Body = Message;
                //mail.IsBodyHtml = true;
                mail.IsBodyHtml = false;
                mail.Priority = MailPriority.Normal;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("iotegralpvt@gmail.com", "Iotegral@123");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i <= ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if ((status == SmtpStatusCode.MailboxBusy) | (status == SmtpStatusCode.MailboxUnavailable))
                    {
                        System.Threading.Thread.Sleep(5000);
                        smtp.Send(mail);

                    }
                }

            }
            catch (Exception e)
            {
                e.Message.ToString();
            }

        }
        public string GetEmailTemplate(string Username, string Description, string TemplateName)
        {
            string body = string.Empty;
            string directory = HttpContext.Current.Server.MapPath("~/EmailTemplate");
            using (StreamReader reader = new StreamReader(directory + "\\" + TemplateName))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", Username);
            body = body.Replace("{Description}", Description);
            return body;
        }

        public string GetEmailTemplate(Dictionary<string, string> parameters, string TemplateName)
        {
            string body = string.Empty;
            string directory = HttpContext.Current.Server.MapPath("~/EmailTemplate");
            using (StreamReader reader = new StreamReader(directory + "\\" + TemplateName))
            {
                body = reader.ReadToEnd();
            }
            foreach (var pair in parameters)
            {
                body = body.Replace(pair.Key, pair.Value);
            }
            return body;
        }
    }
}