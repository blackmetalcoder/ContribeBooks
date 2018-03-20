using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestBooks
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddToChart()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            string svar = client.AddToChart("11", "Nisse", "Musjakt", "365,50");
        }
        [TestMethod]
        public void getBooks()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            var Bocker = client.GetBooks();
        }
        [TestMethod]
        public void getTheBook()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            var Boken = client.GetBook("Second Author");
        }
        [TestMethod]
        public void getBasket()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            var Basket = client.getBasket("11");
            
        }
        [TestMethod]
        public void AddOrder()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            string svar = client.OrderHead("Nisse", "Mit i gatan 1", "11122", "Nisse@nisse.se", "Staden", "11");
        }
        [TestMethod]
        public void sendMEmail()
        {
            ServiceBooks.BookStoreServiceClient client = new ServiceBooks.BookStoreServiceClient();
            string svar = client.senOrderMail("peter@horja.se");
            
        }
        
    }
}
