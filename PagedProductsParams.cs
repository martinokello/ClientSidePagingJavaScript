using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MartinLayooInc.Models
{
    public class PagedProductsParams
    {
        public int viewStartPage { get; set; }
        public int numberPerPage { get; set; }
        public int pagesToShow { get; set; }
        public string direction { get; set; }
    }
}