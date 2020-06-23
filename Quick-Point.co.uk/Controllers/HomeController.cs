using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

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

        [HttpPost]
        public ActionResult FFA(HttpPostedFileBase file, FormCollection form)
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




                ViewBag.Message = "Thank you, your file has been uploaded sucessfully. We will be in contact shortly.";
                return View();
            }
            catch
            {
                ViewBag.Message = "File Uploaded Unsucessfully, please try again, filling in all fields. If the error persists, please contact us.";
                return View();
            }

            

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