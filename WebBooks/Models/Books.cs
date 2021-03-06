﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBooks.Models
{
    public class Books
    {
        public string Title { get; set; }
        public string Author { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        public int InStock { get; set; }
    }
}