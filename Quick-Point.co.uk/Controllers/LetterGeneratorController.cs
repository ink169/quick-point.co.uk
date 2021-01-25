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

        public ActionResult FurloughLeave()
        {
            return View();
        }

        public ActionResult ReturnToWork()
        {
            return View();
        }
        public ActionResult TemporaryFurlough()
        {
            return View();
        }
        public ActionResult ReFurlough()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReFurl(FormCollection form)
        {
            try
            {
                var letgen = new LetterGen(form);
                letgen.CreateDoc("ReFurlough");
                ViewBag.CompleteMessage = "Your document has been emailed to the specified address. Please check your spam if you can't locate it.";
                return View("Index");
            }
            catch
            {
                ViewBag.CompleteMessage = "Error creating document";
                return View("Index");
            }

        }
        [HttpPost]
        public ActionResult TempFurl(FormCollection form)
        {
            try
            {
                var letgen = new LetterGen(form);
                letgen.CreateDoc("TemporaryFurlough");
                ViewBag.CompleteMessage = "Your document has been emailed to the specified address. Please check your spam if you can't locate it.";
                return View("Index");
            }
            catch
            {
                ViewBag.CompleteMessage = "Error creating document";
                return View("Index");
            }

        }
        [HttpPost]
        public ActionResult RetToWork(FormCollection form)
        {
            try
            {
                var letgen = new LetterGen(form);
                letgen.CreateDoc("ReturnToWork");
                ViewBag.CompleteMessage = "Your document has been emailed to the specified address. Please check your spam if you can't locate it.";
                return View("Index");
            }
            catch
            {
                ViewBag.CompleteMessage = "Error creating document";
                return View("Index");
            }

        }

        [HttpPost]
        public ActionResult FlexFurl(FormCollection form)
        {
            try
            {
                var letgen = new LetterGen(form);
                letgen.CreateDoc("FlexibleFurlough");
                ViewBag.CompleteMessage = "Your document has been emailed to the specified address. Please check your spam if you can't locate it.";
                return View("Index");
            }
            catch
            {
                ViewBag.CompleteMessage = "Error creating document";
                return View("Index");
            }

        }

        [HttpPost]
        public ActionResult FurlLeave(FormCollection form)
        {
            try
            {
                var letgen = new LetterGen(form);
            letgen.CreateDoc("FurloughLeave");
            ViewBag.CompleteMessage = "Your document has been emailed to the specified address. Please check your spam if you can't locate it.";
            return View("Index");
        }
            catch
            {
                ViewBag.CompleteMessage = "Error creating document";
                return View("Index");
    }
}
    }
}