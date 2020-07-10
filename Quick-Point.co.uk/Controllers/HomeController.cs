using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using MongoDB.Bson;
using System.Web.Mvc;
using MongoDB.Driver;
using System.Security;
using System.Net.Mail;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Quick_Point.co.uk.Helpers;
using Quick_Point.co.uk.ViewModels;
using Utils = Quick_Point.co.uk.Helpers.Utils;

namespace Quick_Point.co.uk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult oldResources()
        {
            return View();
        }
        public ActionResult Index()
        {

            var baseAddress = string.Format("{0}://{1}{2}{3}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"), "Home/Article?id=");

            return View(new Resource() { BaseUrl = baseAddress });

        }

        public ActionResult Resource()
        {
            return View();
        }


        public async Task<JsonResult> GetToken()
        {
            var secret = Utils.GetConfigSetting("secret");

            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://directline.botframework.com/v3/directline/tokens/generate");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secret);

            var userId = $"dl_{Guid.NewGuid()}";

            request.Content = new StringContent(
                JsonConvert.SerializeObject(
                    new { User = new { Id = userId } }),
                    Encoding.UTF8,
                    "application/json");

            var response = await client.SendAsync(request);
            string tokenVal = String.Empty;

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                tokenVal = JsonConvert.DeserializeObject<DirectLineToken>(body).token;
               // var config = new ChatConfig() { Token = token, UserId = userId };
                // return String.Format("{0}*****{1}",token, userId);
                return Json(new
                {
                    success = true,
                    Token = tokenVal,
                    User = userId,
                   // Email = model.Email
                },
                JsonRequestBehavior.AllowGet);
            }
            else
            {
                throw new Exception("Error getting bearer token");
            }
        }
            



        public ActionResult FF()
        {

            return View();
        }

               

        [HttpPost]
        public ActionResult FF(HttpPostedFileBase file, System.Web.Mvc.FormCollection form1, FFAC model)
        {
            var MDclient = new MongoClient("mongodb+srv://fred:" + MongoDBPW() + "@freefinancial-tubyw.azure.mongodb.net/QuickPoint?retryWrites=true&w=majority");
            var db = MDclient.GetDatabase("QuickPoint");
            var collec = db.GetCollection<BsonDocument>("FreeFinancial");
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

                    var document = new BsonDocument
                {
                {"Name", (form1["name"].ToString()) },
                {"Email", (form1["email"].ToString()) },
                {"Phone", (form1["phone"].ToString()) },
                {"Staff Number", (form1["staffno"].ToString()) },
                {"Turnover", (form1["turnover"].ToString()) },
                {"Date", dt },
                };

                    collec.InsertOneAsync(document);

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

            var MDclient = new MongoClient("mongodb+srv://fred:" + MongoDBPW() + "@freefinancial-tubyw.azure.mongodb.net/QuickPoint?retryWrites=true&w=majority");
            var db = MDclient.GetDatabase("QuickPoint");
            var collec = db.GetCollection<BsonDocument>("Contact");
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

                var document = new BsonDocument
                {
                {"Name", (form["name"].ToString()) },
                {"Email", (form["email"].ToString()) },
                {"Message", (form["message"].ToString()) },
                {"Date", dt },
                };

                collec.InsertOneAsync(document);

                ViewBag.Message = "Email sent";
                  return View("Contact"); 
              }
              catch
              {
                  
                  return View();
              }

          }



        [HttpPost]
        public ActionResult SubscribePopup(String name, string emailS)
        {
            var MDclient = new MongoClient("mongodb+srv://fred:" + MongoDBPW() + "@freefinancial-tubyw.azure.mongodb.net/QuickPoint?retryWrites=true&w=majority");
            var db = MDclient.GetDatabase("QuickPoint");
            var collec = db.GetCollection<BsonDocument>("PopSubscribe");

            try
            {


                var username = escapeCharacters(name);
                var email = escapeCharacters(emailS);
                var dt = DateTime.Now.ToString();


                var mailMessage = new MailMessage();
                mailMessage.From = new
                   MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
                mailMessage.To.Add(new
                   MailAddress(Utils.GetConfigSetting("Ludaemail")));
                   //MailAddress(Utils.GetConfigSetting("Fredemail")));
                mailMessage.CC.Add(Utils.GetConfigSetting("Fredemail"));
               mailMessage.CC.Add(Utils.GetConfigSetting("Andrewemail"));
                mailMessage.Subject = "Subscribe" + " " + email;
                mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + username + "\n" + "\n" + "Email:" + "\n" + email;
                mailMessage.IsBodyHtml = false;
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("fpw"));
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.EnableSsl = true;
                client.Send(mailMessage);
                ViewBag.Message = "Subscribed";

                var document = new BsonDocument
                {
                {"Name", name },
                {"Email", emailS },
                {"Date", dt },
                };

                collec.InsertOneAsync(document);

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
            var MDclient = new MongoClient("mongodb+srv://fred:" + MongoDBPW() + "@freefinancial-tubyw.azure.mongodb.net/QuickPoint?retryWrites=true&w=majority");
            var db = MDclient.GetDatabase("QuickPoint");
            var collec = db.GetCollection<BsonDocument>("Subscribe");

           
            try
            {

                var dt = DateTime.Now.ToString();
                var _email = escapeCharacters(email);
                var _name = escapeCharacters(name);



                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
                mailMessage.To.Add(new MailAddress(Utils.GetConfigSetting("Ludaemail")));
                //mailMessage.To.Add(new MailAddress(Utils.GetConfigSetting("Fredemail")));
                mailMessage.CC.Add(Utils.GetConfigSetting("Fredemail"));
                mailMessage.CC.Add(Utils.GetConfigSetting("Andrewemail"));
                mailMessage.Subject = "Subscribe" + " " + email;
                mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + _name + "\n" + "\n" + "Email:" + "\n" + _email;
                mailMessage.IsBodyHtml = false;
                SmtpClient client = new SmtpClient();
                client.Credentials = new NetworkCredential(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("fpw"));
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.EnableSsl = true;
                client.Send(mailMessage);
                ViewBag.Message = "Subscribed";

                var document = new BsonDocument
                {
                {"Name", name },
                {"Email", email },
                {"Date", dt },
                };

                collec.InsertOneAsync(document);

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

        public string MongoDBPW()
        {
            var P = Utils.GetConfigSetting("MongoDBPW");
            return P;
        }

    }

    public class ChatConfig
    {
        public string Token { get; set; }
        public string UserId { get; set; }
    }

    public class DirectLineToken
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public int expires_in { get; set; }
    }

}