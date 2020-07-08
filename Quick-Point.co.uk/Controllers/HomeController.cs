using System;
using System.IO;
using System.Web;
using System.Net;
using MongoDB.Bson;
using System.Web.Mvc;
using MongoDB.Driver;
using System.Security;
using System.Net.Mail;
using Newtonsoft.Json.Linq;
using Quick_Point.co.uk.Helpers;
using Quick_Point.co.uk.ViewModels;
using Utils = Quick_Point.co.uk.Helpers.Utils;

namespace Quick_Point.co.uk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult FF()
        {

            return View();
        }

               

        [HttpPost]
        public ActionResult FF(HttpPostedFileBase file, System.Web.Mvc.FormCollection form1, FFAC model)
        {

            var dt = DateTime.Now.ToString();
            var username = escapeCharacters((form1["name"].ToString()));
            var email = escapeCharacters((form1["email"].ToString()));
            var phone = escapeCharacters((form1["phone"].ToString()));
            var staffno = escapeCharacters((form1["staffno"].ToString()));
            var turnover = escapeCharacters((form1["turnover"].ToString()));
            var selected = model.Business;
            var bookkeeping = model.Bookkeeping;
            var payroll = model.Payroll;
            var CompaniesHouseReturns = model.CompaniesHouseReturns;
            var SelfAssessment = model.SelfAssessment;
            var VATReturns = model.VATReturns;
            var AccountsManagement = model.AccountsManagement;
            var BusinessConsultation = model.BusinessConsultation;
            var TaxationAdvice = model.TaxationAdvice;

            try
            {
                byte[] data;
                using (Stream inputStream = file.InputStream)
                using (MemoryStream ms = new MemoryStream())
                using (var client = new SmtpClient("smtp.office365.com", 587))
                using (var message = new MailMessage(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("LudaEmail")))
                {
                    if (file != null)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                        memoryStream.Position = 0;
                        message.Attachments.Add(new Attachment(memoryStream, file.FileName, file.ContentType));
                    }

                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Octet);
                    message.Subject = "New Financial Request from " + username;
                    message.Body = "Name:" + "\n" + username + "\n" + "\n" + "Time" + "\n" + dt + "\n" + "\n" + "Email:" + "\n" + email + "\n" + "\n" + "Phone:" + "\n" + phone + "\n" + "\n" + "Business Type:" + "\n" + "\n" + selected + "\n" + "\n" + "Turnover:" + "\n" + "\n" + turnover + "\n" + "\n" + "Number of Staff:" + "\n" + staffno + "\n" + "\n" + "Required Services:" + "\n" + "\n" + "Bookkeeping: " + bookkeeping
                   + "\n" + "Payroll: " + payroll + "\n" + "Companies House Returns: " + CompaniesHouseReturns + "\n" + "Self Assessment: " + SelfAssessment + "\n" +
                   "VAT Returns: " + VATReturns + "\n" + "Accounts Management: " + AccountsManagement + "\n" + "Business Consultation: " + BusinessConsultation + "\n" + "Taxation Advice: " + TaxationAdvice + "\n";


                    message.IsBodyHtml = false;

                    client.Credentials = new NetworkCredential(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("fpw"));

                    message.CC.Add(Utils.GetConfigSetting("Fredemail"));
                    message.CC.Add(Utils.GetConfigSetting("Andrewemail"));
                    client.EnableSsl = true;
                    client.Send(message);
                }
                ViewBag.Message = "Email sent";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Email not sent" + "\n" + ex;
                return View();
            }
        }
        private string getHomeAddress()
        {
            return string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"), "Home/Index");
        }

        public ActionResult Resources()
        {

            var baseAddress = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"), "Home/Article?id=");

            return View(new Resource() { BaseUrl = baseAddress });
        }

        [HttpPost]
        public ActionResult RequestArticle(string firstName, string lastName, string company, string email, string articleID)
        {
            Session["articleID"] = articleID;
            return RedirectToAction("Article");
        }

        public ActionResult Article(string id)
        {

            if (!String.IsNullOrEmpty(id))
            {
                return View(new Article() { Description = ArticleHelpers.GetTitle(id), ID = id });
            }
            else
            {
                Response.Redirect(getHomeAddress());
                return View();
            }
        }

        public ActionResult RequestArticle(string id)
        {
            if (id != null)
            {
                //Session["ArticleID"] = id;
                return View(new Article() { Description = ArticleHelpers.GetTitle(id), ID = id });
            }
            else
            {
                Response.Redirect(getHomeAddress());
                return View();
            }

        }


        public ActionResult Index()
        {

            var baseAddress = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"), "Home/Article?id=");

            return View(new Resource() { BaseUrl = baseAddress });

        }

        public ActionResult Contact()
        {
            return View();
        }


        public ActionResult Subscribe()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        [HttpPost]
         public ActionResult Contact(System.Web.Mvc.FormCollection form)
         {
          try
              {

                  var username = escapeCharacters((form["name"].ToString()));
                  var email = escapeCharacters((form["email"].ToString()));
                  var message = escapeCharacters((form["message"].ToString()));
                  var dt = DateTime.Now.ToString();


                  var mailMessage = new MailMessage();
                  mailMessage.From = new
                     MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
                  mailMessage.To.Add(new
                       MailAddress(Utils.GetConfigSetting("Ludaemail")));
                       //MailAddress(Utils.GetConfigSetting("Fredemail")));
                  mailMessage.CC.Add(Utils.GetConfigSetting("Fredemail"));
                  mailMessage.CC.Add(Utils.GetConfigSetting("Andrewemail"));
                  mailMessage.Subject = "New Contact Request from " + username; ;
                  mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + username + "\n" + "\n" + "Email:" + "\n" + email + "\n" + "\n" + "Message:" + "\n" + message;
                  mailMessage.IsBodyHtml = false;
                  SmtpClient client = new SmtpClient();
                  client.Credentials = new NetworkCredential(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("fpw"));
                  client.Port = 587;
                  client.Host = "smtp.office365.com";
                  client.EnableSsl = true;
                  client.Send(mailMessage);
                  ViewBag.Message = "Email sent";
                  return View("Contact"); 
              }
              catch
              {
                  
                  return View();
              }

          }



        [HttpPost]
        public ActionResult SubscribePopup(String name, string email)
        {

            var t = 9;


            try
            {



                //var fileName = Path.GetFileName(file.FileName);
                //var username = escapeCharacters((form["name"].ToString()));
                //var email = escapeCharacters((form["email"].ToString()));
                //var dt = DateTime.Now.ToString();

                //var t = 9;


                //var mailMessage = new MailMessage();
                //mailMessage.From = new
                //   MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
                //mailMessage.To.Add(new
                //   MailAddress(Utils.GetConfigSetting("Ludaemail")));
                //mailMessage.CC.Add(Utils.GetConfigSetting("Fredemail"));
                //mailMessage.CC.Add(Utils.GetConfigSetting("Andrewemail"));
                //mailMessage.Subject = "Subscribe" + " " + email;
                //mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + username + "\n" + "\n" + "Email:" + "\n" + email;
                //mailMessage.IsBodyHtml = false;
                //SmtpClient client = new SmtpClient();
                //client.Credentials = new NetworkCredential(Utils.GetConfigSetting("QuickPointemail"), Utils.GetConfigSetting("fpw"));
                //client.Port = 587;
                //client.Host = "smtp.office365.com";
                //client.EnableSsl = true;
                //client.Send(mailMessage);
                //ViewBag.Message = "Subscribed";
                return View();
            }
            catch
            {
                ViewBag.Message = "";
                return View();
            }
        }



        [HttpPost]
        public ActionResult Subscribe(String name, string email)
        {
            try
            {

                var dt = DateTime.Now.ToString();




                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
                mailMessage.To.Add(new MailAddress(Utils.GetConfigSetting("Ludaemail")));
                mailMessage.CC.Add(Utils.GetConfigSetting("Fredemail"));
                mailMessage.CC.Add(Utils.GetConfigSetting("Andrewemail"));
                mailMessage.Subject = "Subscribe" + " " + email;
                mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + name + "\n" + "\n" + "Email:" + "\n" + email;
                mailMessage.IsBodyHtml = false;
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential(Utils.GetConfigSetting("QuickPointemail"), Utils.GetConfigSetting("fpw"));
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.EnableSsl = true;
                client.Send(mailMessage);
                ViewBag.Message = "Subscribed";
                return View();
            }
            catch
            {
                ViewBag.Message = "";
                return View();
            }
        }



        public ActionResult FAQ()
        {
            return View();
        }

   

        public static SecureString GetSecurePassword()
        {
            var s = new SecureString();
            s.AppendChar('Q');
            s.AppendChar('u');
            s.AppendChar('1');
            s.AppendChar('c');
            s.AppendChar('k');
            s.AppendChar('P');
            s.AppendChar('0');
            s.AppendChar('i');
            s.AppendChar('n');
            s.AppendChar('t');
            s.AppendChar('7');
            s.AppendChar('7');
            s.AppendChar('7');
            return s;
        }

        public class Uploader
        {
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string date { get; set; }

            public string fileName { get; set; }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Meet the team";

            return View();
        }

        public string escapeCharacters(string msg)
        {
            msg = msg.Replace(';', ' ');
            msg = msg.Replace('\\', ' ');
            msg = msg.Replace('=', ' ');
            return msg;
        }

    }
}