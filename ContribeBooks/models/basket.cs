using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContribeBooks.models
{
    public class basket
    {
        public string SessionID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }
}