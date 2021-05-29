using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnquiriesMadeApp.Models
{
    public class EnquiriesMadeModel
    {
        public UserModel userModel {get;set;}
        public ProductModel productModel { get; set; }
        public IEnumerable<ProductDetailsModel> detailsModel { get; set; }
    }
}