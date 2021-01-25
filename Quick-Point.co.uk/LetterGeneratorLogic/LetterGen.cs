using Quick_Point.co.uk.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DocumentGeneratorLibrary;

namespace Quick_Point.co.uk.LetterGeneratorLogic
{
    public class LetterGen
    {
        private FormCollection _form;

        public byte[] Filestream { get; set; }
        public string LetterName { get; set; }

        public LetterGen(FormCollection form)
        {
            _form = form;
        }

        public void CreateDoc(string Type)
        {
            if (Type == "FlexibleFurlough") { FlexFurlGen(); LetterName = "Flexible Furlough Letter.Pdf"; };
            if (Type == "FurloughLeave") { FurlLeaveGen(); LetterName = "Furlough Leave Letter.Pdf"; };
            if (Type == "ReturnToWork") { RetToWorkGen(); LetterName = "Return To Work Letter.Pdf"; };
            if (Type == "TemporaryFurlough") { TempFurl(); LetterName = "Temporary Furlough Letter.Pdf"; };

            //...

            var mailMessage = new MailMessage();
            mailMessage.From = new
               MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
            mailMessage.To.Add(new
                 MailAddress(_form["Email"].ToString()));
            mailMessage.Subject = "Quick Point Generated Letter";
            mailMessage.Body = "Here is the letter you have just generated through www.quick-point.co.uk";
            mailMessage.IsBodyHtml = false;
            Attachment att = new Attachment(new MemoryStream(Filestream), LetterName);
            mailMessage.Attachments.Add(att);
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("fpw"));
            client.Port = 587;
            client.Host = "smtp.office365.com";
            client.EnableSsl = true;
            client.Send(mailMessage);
        }
        public void FlexFurlGen()
        {
            var docgen = new DocGen();
            Filestream = docgen.CreateFlexFurlDocument(_form);
        }

         public void FurlLeaveGen()
        {
            var docgen = new DocGen();
            Filestream = docgen.CreateFurlLeaveDocument(_form);
        }
         public void RetToWorkGen()
        {
            var docgen = new DocGen();
            Filestream = docgen.CreateRetToWorkDocument(_form);
        }
        
         public void TempFurl()
        {
            var docgen = new DocGen();
            Filestream = docgen.CreateTempFurlDocument(_form);
        }


    }
}