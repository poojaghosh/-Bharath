using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnquiriesMadeApp.Views.Home
{
    public class RetailersModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }
        public int productId { get; set; }

        public virtual Product Product { get; set; }
    }
}