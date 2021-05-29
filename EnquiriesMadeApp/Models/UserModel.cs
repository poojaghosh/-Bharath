using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnquiriesMadeApp.Models
{
    public class UserModel
    {
        [Key]
        public int id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
       [DisplayName("Email")]
        public bool EmailNotification { get; set; }
        [DisplayName("SMS")]
        public bool SMSNotification { get; set; }

        public bool WhatsappNotification { get; set; }

        public bool facebooknotification { get; set; }
    }
}