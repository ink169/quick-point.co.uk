using System;
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

namespace Quick_Point.co.uk.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FFA()
        {
            return View();
        }

        public ActionResult Resources()
        {
            return View();
        }

        public ActionResult Article()
        {
            return View();
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
        public ActionResult FFA(HttpPostedFileBase file, System.Web.Mvc.FormCollection form)
        {


            var client = new MongoClient("mongodb+srv://fred:ASdfJI43619%21@freefinancial-tubyw.azure.mongodb.net/QuickPoint?retryWrites=true&w=majority");
            var db = client.GetDatabase("QuickPoint");
            var collec = db.GetCollection<BsonDocument>("FFA Uploaders");

           

          try
           {
                
                    var fileName = Path.GetFileName(file.FileName);
                    var username = (form["name"].ToString());
                    var email = (form["email"].ToString());
                    var phone = (form["phone"].ToString());
                    var date = (form["date"].ToString());
                    fileName = username + date + fileName;
                    string uploadDetails = username + ',' + email + ',' + phone + ',' + date + ',' + fileName;
                    var path = Path.Combine(Server.MapPath("~/Files/"), fileName);

                    var document = new BsonDocument
                {
                {"Name", username},
                {"Email", email },
                {"Phone", phone },
                {"Date", date },
                {"File Name", fileName }
                };

                    collec.InsertOneAsync(document);

                MemoryStream target = new MemoryStream();
                file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();


                string relativeUrl = "Shared Documents/FFA";
                var website = "https://netorgft6692843.sharepoint.com/sites/QuickPointDocuments";

                using (var clientContext = new ClientContext(website))
                {
                   
                        clientContext.Credentials = new SharePointOnlineCredentials("qpadmin@quick-point.co.uk", GetSecurePassword());
                        Web web = clientContext.Web;
                        Folder folder = web.GetFolderByServerRelativeUrl(relativeUrl);
                        clientContext.Load(folder);
                        clientContext.ExecuteQuery();

                        folder.Files.Add(new FileCreationInformation
                        {
                            Overwrite = true,
                            Content = data,
                            Url = folder.ServerRelativeUrl + "/" + fileName
                        }); ;
                        clientContext.ExecuteQuery();
                   
               }


                ViewBag.Message = "Thank you, your file has been uploaded sucessfully. We will be in contact shortly.";
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
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Meet the team";

            return View();
        }

    }
}