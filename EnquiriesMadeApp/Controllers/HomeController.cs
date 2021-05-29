using EnquiriesMadeApp.Models;
using EnquiriesMadeApp.Services;
using EnquiriesMadeApp.Views.Home;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EnquiriesMadeApp.Controllers
{
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Enquiry()
        {
            return View();
        }

        public JsonResult GetSearchValue(string search)
        {
            EnquiriesMadeEntities db = new EnquiriesMadeEntities();
            List<ProductModel> allsearch = db.Products.Where(x => x.ProductName.Contains(search)).Select(x => new ProductModel
            {
                Id = x.ProductId,
                Name = x.ProductName
            }).ToList();
            return new JsonResult { Data = allsearch, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public async Task<JsonResult> PostData(string name,string email,string phone,string emailnotification,string whatsappnotification, string smsnotification, string facebooknotification)
        {
            //, bool whatsappnotification, bool smsnotification, bool facebooknotification
            UserModel urs = new UserModel();
            urs.FirstName = name;
            urs.Email = email;
            urs.Phone = phone;
            EnquiriesMadeEntities db = new EnquiriesMadeEntities();
            bool isEmailSent = false;
            string searchedText = Session["SearchedTest"].ToString();
            //save uer data to db
            bool isAdded = AddUser(urs);
            int id = db.Products
                          .Where(x => x.ProductName == searchedText).Select(x => x.ProductId).FirstOrDefault();
            //Fetch Retailers list
            List<RetailersModel> retailerList = new List<RetailersModel>();
            retailerList = GetRetailers(id);
            
            //Notify Retailers
            if (retailerList != null && retailerList.Count > 0)
                isEmailSent = await this.NotifyRetailers(retailerList, urs, searchedText);
           
            //Send Email notification
            if (emailnotification == "1")
            {
                List<ProductDetailsModel> prdData = GetProductDetails(id);
                isEmailSent = await this.SendEmailAsync(prdData, urs.Email);
            }

            if (smsnotification == "1")
               SendSMS(searchedText, urs.Phone);

            if (whatsappnotification == "1")
                SendWhatsAppNotification(searchedText, urs.Phone);

            return Json(urs);
        }

        [HttpPost]
        public async Task<ActionResult> Index(UserModel user)
        {
            EnquiriesMadeEntities db = new EnquiriesMadeEntities();
            bool isEmailSent = false;
            string searchedText = Session["SearchedTest"].ToString();
            //save uer data to db
            bool isAdded = AddUser(user);
            //if(isAdded)
            //{
                //get product Id
                int id = db.Products
                            .Where(x => x.ProductName == searchedText).Select(x => x.ProductId).FirstOrDefault();
                //Fetch Retailers list
                List<RetailersModel> retailerList = new List<RetailersModel>();
                retailerList = GetRetailers(id);
                //Notify Retailers
                if(retailerList != null && retailerList.Count > 0)
                    isEmailSent =  await this.NotifyRetailers(retailerList,user,searchedText);
                
               
                
                //Send Email notification
                if (user.EmailNotification)
                {
                    List<ProductDetailsModel> prdData = GetProductDetails(id);
                    isEmailSent = await this.SendEmailAsync(prdData,user.Email);
                }
                   
                if (user.SMSNotification)
                    SendSMS(searchedText,user.Phone);
                if (user.WhatsappNotification)
                    SendWhatsAppNotification(searchedText, user.Phone);
            //}
           
            return View();
        }

        private List<RetailersModel> GetRetailers(int searchedProductId)
        {
            using (var db = new EnquiriesMadeEntities())
            {
                var query = (from c in db.Retailers
                        where c.productId == searchedProductId
                        select new RetailersModel { 
                            Name = c.Name,
                            Email = c.Email
                        }).ToList();

                return query;
            }
            
        }

        [HttpPost]
        public JsonResult SetSession(string name)
        {
            Session["SearchedTest"] = name;
            ProductModel prdModel = new ProductModel()
            {
                Name = Session["SearchedTest"].ToString()
            };
            return Json(prdModel);
        }
        public async Task<bool> NotifyRetailers(List<RetailersModel> retailers,UserModel user,string searchedProduct)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                foreach(RetailersModel retailer in retailers)
                {
                    // Initialization.  
                    var message = new MailMessage();

                    // Add retailsers ids.  
                    message.To.Add(new MailAddress(retailer.Email));
                    message.From = new MailAddress(ConfigurationManager.AppSettings["NetworkCredential_UserName"].ToString());
                    message.Subject = "Enquiries Made";

                    string htmlBody = "User with Email <b>" + user.Email +
                                       "</b> and Phone No. <b>" + user.Phone
                                       + "</b> has shown interest in the Product-<b>" + searchedProduct
                                       + "</b> of your area / domain and we are sending the requested relevant details to him.";

                    message.Body = htmlBody;
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        // Settings.  
                        var credential = new NetworkCredential
                        {
                            UserName = ConfigurationManager.AppSettings["NetworkCredential_UserName"].ToString(),
                            Password = ConfigurationManager.AppSettings["NetworkCredential_Password"].ToString()
                        };

                        // Settings.  
                        smtp.Credentials = credential;
                        smtp.Host = ConfigurationManager.AppSettings["SMTP_Server"].ToString();
                        smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_Server_Port"].ToString());
                        smtp.EnableSsl = true;

                        // Sending  
                        await smtp.SendMailAsync(message);
                    }
                

                    // Settings.  
                    isSend = true;
                }
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
            return isSend;


        }

        private bool AddUser(UserModel user)
        {
            bool isAdded = false;
            string constr = ConfigurationManager.AppSettings["conn"].ToString();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (var command = new SqlCommand("sp_InsertUser", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    

                    command.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = user.FirstName;
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = user.Email;
                    command.Parameters.Add("@Phone", SqlDbType.VarChar).Value = user.Phone;

                    command.Connection = con;
                    con.Open();
                    command.ExecuteNonQuery();
                }
            
            }
            
                    
            
            
            
           
            return isAdded;
        }

        private void SendSMS(string searchText,string phoneNumber)
        {
            var textMessage = "Dear Customer, Kindly check your email for more details!";
            bool IsSendSms = SMSManager.SendSms(phoneNumber, textMessage).Result;

            //phoneNumber = "+919975710054";
            //string fromNumber = "+919657555921";
            //TwilioClient.Init(ConfigurationManager.AppSettings["accountSid"].ToString(), ConfigurationManager.AppSettings["authToken"].ToString());

            //var message = MessageResource.Create(
            //    body: "Dear Customer, Kinldy check your email for more details!",//"Product -< b >"+ searchText + "</ b > details you searched have been verified and relevant information already sent to your all modes of communication with your consent..",
            //    from: new Twilio.Types.PhoneNumber(fromNumber),
            //    to: new Twilio.Types.PhoneNumber(phoneNumber)
            //);            
        }

        private void SendWhatsAppNotification(string searchText,string phoneNumber)
        {
            TwilioClient.Init(ConfigurationManager.AppSettings["accountSid"].ToString(), ConfigurationManager.AppSettings["authToken"].ToString());

            //var message = MessageResource.Create(
            //               from: new PhoneNumber("whatsapp:####"),
            //               to: new PhoneNumber("whatsapp:" + phoneNumber + ""),
            //               body: "Product -< b >" + searchText + "</ b > details you searched have been verified and relevant information already sent to your all modes of communication with your consent.."
            //           );

            var messageOptions = new CreateMessageOptions(
            new PhoneNumber($"whatsapp:+91{phoneNumber}"));
            messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
            messageOptions.Body = "Product -" + searchText + " details you searched have been verified and relevant information already sent to your all modes of communication with your consent..";

            var message = MessageResource.Create(messageOptions);
        }

        private List<ProductDetailsModel> GetProductDetails(int id)
        {
            EnquiriesMadeEntities db = new EnquiriesMadeEntities();
            string query = "SELECT * FROM ProductDetails where productId=" + id;
            List<ProductDetailsModel> prd = new List<ProductDetailsModel>();
            string constr = ConfigurationManager.AppSettings["conn"].ToString();
            //var customers = from prd in db.ProductDetails.AsEnumerable()
           

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            prd.Add(new ProductDetailsModel
                            {

                                ProductLink = sdr["ProductLink"].ToString(),
                                ProductImage = (byte[])sdr["ProductImage"],
                                ProductSpecification = sdr["ProductSpecification"].ToString(),
                                Price = sdr["Price"].ToString()

                            }); ; ;
                        }
                    }
                    con.Close();
                }

                return prd;
            }
        }

        public async Task<bool> SendEmailAsync(List<ProductDetailsModel> prdData,string email)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                // Initialization.  
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress(ConfigurationManager.AppSettings["NetworkCredential_UserName"].ToString());
                message.Subject = "Enquiries Made";
                
                string htmlBody = "<html><table border=1 ><tr style=background-color:yellow;><th>Product Image</th><th>Product description</th></tr>";
                List<LinkedResource> resources = new List<LinkedResource>();
                foreach (var prd in prdData)
                {
                    htmlBody = htmlBody + "<tr><td>";

                    var image = new LinkedResource(new MemoryStream(prd.ProductImage));   //se.SignatureFile);
                    image.ContentId = Guid.NewGuid().ToString();


                    //String body = string.Format(@" <img src=""cid:{0}"" />", image.ContentId);
                    htmlBody = htmlBody+ string.Format(@" <img src=""cid:{0}"" />", image.ContentId);
                    AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
                    view.LinkedResources.Add(image);
                    message.AlternateViews.Add(view);
                    htmlBody = htmlBody + "</td><td>" +
                        "Specification: " +prd.ProductSpecification +
                        "<br/>Price: " + prd.Price +
                        "<br/>Buy Now: " + prd.ProductLink                        
                        + "</td></tr>";
                }
                
                htmlBody = htmlBody + "</table></html>";

                 message.Body = htmlBody;
                message.IsBodyHtml = false;

                using (var smtp = new SmtpClient())
                {
                    // Settings.  
                    var credential = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["NetworkCredential_UserName"].ToString(),
                        Password = ConfigurationManager.AppSettings["NetworkCredential_Password"].ToString()
                    };

                    // Settings.  
                    smtp.Credentials = credential;
                    smtp.Host = ConfigurationManager.AppSettings["SMTP_Server"].ToString();
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_Server_Port"].ToString());
                    smtp.EnableSsl = true;

                    // Sending  
                    await smtp.SendMailAsync(message);

                    // Settings.  
                    isSend = true;
                }
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }

            // info.  
            return isSend;
        }
    }
}