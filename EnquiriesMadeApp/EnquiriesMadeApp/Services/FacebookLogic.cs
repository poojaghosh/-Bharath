using EnquiriesMadeApp.Models;
using EnquiriesMadeApp.Services.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace EnquiriesMadeApp.Services
{
    public class FacebookLogic
    {

        private string _accessToken;

        /// <summary>
        /// Uses default Client ID and Secret as set in the web.config.
        /// </summary>
        public FacebookLogic()
        {
            _accessToken = "EAAFZACJwVBg4BAGbbkHtSFAPg1Ra7gbBWeEEYQCSsGmE4IZA0ZA7YJyJd00mFc7yqlLjAknGhZBOZCx0VJ47ZAx7ZCGmT0BUb4iRFwFnaIOykb6HduQ3VMlllUOkFHruktfNFfJDkYKpYP9XO0iaQdVGzGWHBReN85bvTAxyMTmpr4P8O82XeZApccrCgV8anokIeyIkqruUo5K5Kf4pZCuZC1";
        }

        /// <summary>
        /// Gets post details by Facebook's Page ID.
        /// </summary>
        /// <param name="fbPageId"></param>
        /// <returns></returns>
        public FacebookModel GetPagePostDetails(string fbPageId)
        {
            return ApiWebRequestHelper.GetJsonRequest<FacebookModel>($"https://graph.facebook.com/v9.0/{fbPageId}/posts?access_token={_accessToken}", "GET");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fbPostId"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public FacebookModel GetPostDetails(string fbPostId, string searchText)
        {
            return ApiWebRequestHelper.GetJsonRequest<FacebookModel>($"https://graph.facebook.com/v9.0/{fbPostId}/comments?message={searchText}&access_token={_accessToken}", "POST");
        }
        ///Summary
        //https://graph.facebook.com/v9.0/{pageid}/comments?message={text}&access_token=accesstoken


    }
}