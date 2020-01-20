using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using CventRegManager.Interfaces;
namespace CventRegManager.Helpers
{
    public class JangoEmail : IEmail


    {

        private string MailServer;
        
        public string Subject { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }


        public JangoEmail()
        { 
        
        MailServer= "Relay.xxxxxx.net";
        FromEmail = "webmaster@xxxxx.com";
        FromName = "Webmaster";
        Subject = "APAP Post to ProTech Failed";
        }

        public void UpdateSubject(string subject)
        {
        
        Subject = subject;
        }

        public bool Send(string ToEmail, string Body)
        {

           
            SmtpClient smtpClient = new SmtpClient(MailServer, 25);
            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(FromEmail, FromName );
            mail.To.Add(new MailAddress(ToEmail));
            mail.Body = Body;
            mail.Subject = Subject;
            try
            {
                smtpClient.Send(mail);
                return true;    
            }
            catch (Exception e)
            {
                return false;
            }
            
        
        }
    }
}