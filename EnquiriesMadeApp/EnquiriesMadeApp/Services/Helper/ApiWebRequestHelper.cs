using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace EnquiriesMadeApp.Services.Helper
{
    public class ApiWebRequestHelper
    {
        /// <summary>
        /// Gets a request from an external JSON formatted API and returns a deserialized object of data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public static T GetJsonRequest<T>(string requestUrl, string postType)
        {
            try
            {
                WebRequest apiRequest = WebRequest.Create(requestUrl);
                apiRequest.Method = postType.ToUpper();
                HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

                if (apiResponse.StatusCode == HttpStatusCode.OK)
                {
                    string jsonOutput;
                    using (StreamReader sr = new StreamReader(apiResponse.GetResponseStream()))
                        jsonOutput = sr.ReadToEnd();

                    var jsResult = JsonConvert.DeserializeObject<T>(jsonOutput);

                    if (jsResult != null)
                        return jsResult;
                    else
                        return default(T);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                // Log error here.

                return default(T);
            }
        }

    }
}