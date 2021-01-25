using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quick_Point.co.uk.LetterGeneratorLogic;

namespace Quick_Point.co.uk.Controllers
{
    public class LetterGeneratorController : Controller
    {
        // GET: LetterGenerator
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FlexibleFurlough()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FlexFurl(FormCollection form)
        {
            var letgen = new LetterGen(form);
            letgen.CreateDoc("FlexibleFurlough");
          
            return View();
        }
    }
}