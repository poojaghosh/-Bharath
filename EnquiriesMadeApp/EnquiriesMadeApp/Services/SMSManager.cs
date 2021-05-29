using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnquiriesMadeApp.Services
{
    public class SMSManager
    {
        static string ApiKey = ConfigurationManager.AppSettings["SMS-ApiKey"];

        /// <summary>
        /// SendSms - This meyhod used to send sms to end user.
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<bool> SendSms(String phoneNumber, String message)
        {
            bool result = true;

            using (WebClient client = new WebClient())
            {
                Uri myUri = new Uri("https://api.textlocal.in/send/", UriKind.Absolute);
                var reqparam = new NameValueCollection();
                client.Proxy = null;
                reqparam.Add("apikey", ApiKey);
                reqparam.Add("numbers", phoneNumber);
                reqparam.Add("message", message);
                reqparam.Add("sender", "");

                byte[] responseArray = await client.UploadValuesTaskAsync(myUri, "POST", reqparam);
                string response = Encoding.ASCII.GetString(responseArray);
                if(response !=null)
                {
                    result = true;
                }

            }
            return result;        
        }
    }
}