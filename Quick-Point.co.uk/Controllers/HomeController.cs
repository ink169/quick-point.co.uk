using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.Security;
using Quick_Point.co.uk.ViewModels;
using Quick_Point.co.uk.Helpers;
using System.Reflection;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
using System.Web.Configuration;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Storage.Blob;

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
            var username = (form1["name"].ToString());
            var email = (form1["email"].ToString());
            var phone = (form1["phone"].ToString());
            var staffno = (form1["staffno"].ToString());
            var turnover = (form1["turnover"].ToString());
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
               


                var mailMessage = new MailMessage();
                mailMessage.From = new
                   MailAddress("freddie.kemp@cybercom.media");
                mailMessage.To.Add(new
                   MailAddress("FreddieK_02@hotmail.co.uk"));
                mailMessage.Subject = username;
                mailMessage.Body = "Name:" + "\n" + username + "\n" + "\n" + "Time" + "\n" + dt +"\n" + "\n" + "Email:" + "\n" + email + "\n" + "\n" + "Phone:" + "\n" + phone + "\n" + "\n" + "Business Type:" + "\n" + "\n" + selected + "\n" + "\n" + "Turnover:" + "\n" + "\n" + turnover + "\n" + "\n" + "Number of Staff:" + "\n" + staffno + "\n" + "\n" + "Required Services:" + "\n" + "\n" + "Bookkeeping: " + bookkeeping
                   + "\n" + "Payroll: " + payroll + "\n" + "Companies House Returns: " + CompaniesHouseReturns + "\n" + "Self Assessment: " + SelfAssessment + "\n" + 
                   "VAT Returns: " + VATReturns + "\n" + "Accounts Management: " + AccountsManagement + "\n" + "Business Consultation: " + BusinessConsultation + "\n" + "Taxation Advice: " + TaxationAdvice +"\n" ;

                mailMessage.IsBodyHtml = false;
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential("freddie.kemp@cybercom.media", pw());
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.EnableSsl = true;
                client.Send(mailMessage);
                ViewBag.Message = "Email sent";
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Message = "Email not sent" + "\n" + ex;
                return View();
            }
  
        }

        public string pw()
        {
            string userName = WebConfigurationManager.AppSettings["pw"];
            return userName;
        }

        public string email()
        {
            string userName = WebConfigurationManager.AppSettings["email"];
            return userName;
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
        public ActionResult RequestArticle(string firstName, string lastName, string company, string email, string articleID )
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
                return View(new Article() { Description = ArticleHelpers.GetTitle(id), ID=id });
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

        public ActionResult Test()
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

        public ActionResult Contact(System.Web.Mvc.FormCollection form)
        {

            try
            {

                //var fileName = Path.GetFileName(file.FileName);
                var username = (form["name"].ToString());
                var email = (form["email"].ToString());
                var message = (form["message"].ToString());
                var dt = DateTime.Now.ToString();


                var mailMessage = new MailMessage();
                mailMessage.From = new
                   MailAddress("freddie.kemp@cybercom.media");
                mailMessage.To.Add(new
                   MailAddress("sales@sterling-beanland.co.uk"));
                mailMessage.Subject = username;
                mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + username + "\n" + "\n" + "Email:" + "\n" + email + "\n" + "\n" + "Message:" + "\n" + message;
                mailMessage.IsBodyHtml = false;
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential("freddie.kemp@cybercom.media", "169ajx*!.L");
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.EnableSsl = true;
                client.Send(mailMessage);
                ViewBag.Message = "Email sent";
                return View();
            }
            catch
            {
                ViewBag.Message = "Email not sent";
                return View();
            }

        }       

        

        public ActionResult Subscribe()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FFA(HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, System.Web.Mvc.FormCollection form)
        {


            var client = new MongoClient("mongodb+srv://fred:ASdfJI43619%21@freefinancial-tubyw.azure.mongodb.net/QuickPoint?retryWrites=true&w=majority");
            var db = client.GetDatabase("QuickPoint");
            var collec = db.GetCollection<BsonDocument>("FFA Uploaders");

           

          try
           {

                //var fileName = Path.GetFileName(file.FileName);
                var username = (form["name"].ToString());
                var email = (form["email"].ToString());
                var phone = (form["phone"].ToString());
                var business = (form["Business type"].ToString());
                    var turnover = (form["turnover"].ToString());
                    var staff = (form["staff"].ToString());
                    var bookkeeping = (form["Bookkeeping"]);
                var Payroll = (form["Payroll"]);
                var Companieshousereturns = (form["Companieshousereturns"]);
                var SelfAssessment = (form["Self-Assessment"]);
                var VAT = (form["VAT"]);
                var AccountsManagement = (form["AccountsManagement"]);
                var BusinessConsultations = (form["BusinessConsultations"]);
                var TaxationAdvice = (form["TaxationAdvice"]);

                    var document = new BsonDocument
                {
                {"Name", username},
                {"Email", email },
                {"Phone", phone },
                {"Business", business },
                {"Turnover", turnover },
                {"No. Staff", staff },
                //NEEDS FIDDLING WITH BECAUSE THEY AREN'T UPLOADING
               /* {"Request Bookkeeping?", bookkeeping },
                {"Request Payroll?", Payroll },
                {"Request Companies House Returns?", Companieshousereturns },
                {"Request Self-Assessment?", SelfAssessment },
                {"Request VAT?", VAT },
                {"Request Accoutns management?", AccountsManagement },
                {"Request Business Consultations?", BusinessConsultations },
                {"Request Taxation Advice?", TaxationAdvice },*/
                };

                    collec.InsertOneAsync(document);



                ViewBag.Message = "Thank you kindly, and we will be in touch soon! ";
                return View();
            }
            catch
            {
                ViewBag.Message = "File Uploaded Unsucessfully, please try again, filling in all fields. If the error persists, please contact us.";
                return View();
            }
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

    }
}