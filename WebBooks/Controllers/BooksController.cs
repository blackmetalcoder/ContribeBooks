using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBooks.Models;

namespace WebBooks.Controllers
{
    public class BooksController : Controller
    {
        // GET: Books
        public ActionResult Index()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            var V = client.GetBooks();
            List<Books> B = new List<Books>();
            foreach(var item in V)
            {
                Books b = new Books();
                b.Author = item.Author;
                b.InStock = item.InStock;
                b.Price = item.Price;
                b.Title = item.Title;
                B.Add(b);
            }
            return View(B);
        }
        public ActionResult AddToCart(string author, string title, double price)
        {
            string sessionID = Session["id"].ToString();
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            client.AddToChart(sessionID, author, title, price.ToString());
            return RedirectToAction("Index");
        }
        public ActionResult getBasket()
        {
            string sessionID = Session["id"].ToString();
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            var chart = client.getBasket(sessionID);
            List<basket> B = new List<basket>();
            foreach(var item in chart)
            {
                basket b = new basket();
                b.Author = item.Author;
                b.Price = item.Price;
                b.SessionID = item.SessionID;
                b.Title = item.Title;
                B.Add(b);
            }
            return PartialView("_basket",B);
        }
        [HttpPost]
        public ActionResult search(FormCollection collection)
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            string s = collection["filter"];
            var V = client.GetBook(collection["filter"]);
            List<Books> B = new List<Books>();
            foreach (var item in V)
            {
                Books b = new Books();
                b.Author = item.Author;
                b.InStock = item.InStock;
                b.Price = item.Price;
                b.Title = item.Title;
                B.Add(b);
            }
            return View("Index",B);
        }
    }
}