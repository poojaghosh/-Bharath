using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnquiriesMadeApp.Models
{
    public class ProductDetailsModel
    {
        public string ProductLink { get; set; }
        public byte[] ProductImage { get; set; }
        public string ProductSpecification { get; set; }

        public string Price { get; set; }
    }
}