using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebBooks.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Confirm()
        {
            return View();
        }
        

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
                string sessionID = Session["id"].ToString();
                string ok = client.OrderHead(collection["Name"], collection["Adress"], collection["Zip"], collection["Email"], collection["City"], sessionID);
                Session["orderID"] = ok;
                
                return RedirectToAction("Confirm");
            }
            catch
            {
                return View();
            }
        }

    }
}
