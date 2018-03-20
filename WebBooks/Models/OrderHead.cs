using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBooks.Models
{
    public class OrderHead
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
    }
}