using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBooks.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["id"] = HttpContext.Session.SessionID;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Books for sale!.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}