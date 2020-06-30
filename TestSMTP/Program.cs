using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TestSMTP
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateTestMessage2();


        }

        public static void CreateTestMessage2()
        {
            string to = "andrew.ingpen@gmail.com";
            string from = "admin@quick-point.co.uk";
            string subject = "Using the new SMTP client.";
            string body = @"Using this new feature, you can send an email message from an application very easily.";

            MailMessage message = new MailMessage(from, to, subject, body);
            message.Subject = "Using the new SMTP client.";
            message.Body = @"Using this new feature, you can send an email message from an application very easily.";
            SmtpClient client = new SmtpClient("outlook.office365.com", 587);
            // Credentials are necessary if the server requires the client
            // to authenticate before it will send email on the client's behalf.
            client.Credentials = new System.Net.NetworkCredential("admin@quick-point.co.uk", "Qu1ckP0int777");

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }
        }




    }
}
