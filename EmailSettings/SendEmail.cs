using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace OnlineHouseRentManagementSystem.EmailSettings
{
    public class SendEmail
    {
        public static void Send(string email,string emailBody)
        {
            try
            {
                MailMessage mailMessage = new MailMessage("vigneshwaranmicrosoft.akv@gmail.com", email);
                mailMessage.Subject = "Property Alert";
                mailMessage.Body = emailBody;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = "vigneshwaranmicrosoft.akv@gmail.com",
                    Password = "Letsrocky!!"
                };
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}