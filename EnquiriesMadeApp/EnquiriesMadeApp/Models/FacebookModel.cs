using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnquiriesMadeApp.Models
{
    public class FacebookModel
    {
        public List<FacebookPostDetailsModel> data { get; set; }
    }


    public class FacebookPostDetailsModel
    {
        public string id { get; set; }
        public string message { get; set; }
        public DateTime created_time { get; set; }
    }


}