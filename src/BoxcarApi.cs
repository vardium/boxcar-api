using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace Boxcar.Notification.Api
{
    public class BoxcarApi
    {
        private const string USERAGENT = "Boxcar .NET Api Client";
        private const string URLFORMAT = "http://boxcar.io/devices/providers/{0}/notifications/{1}";

        private static string _ApiKey;
        private static string _ApiSecret;

        private enum ApiOperation
        {
            SUBSCRIBE, CREATE, BROADCAST
        }

        public BoxcarApi(string apikey, string secret)
        {
            _ApiKey = apikey;
            _ApiSecret = secret;
        }

        /// <summary>
        /// Subscribe user to you provider
        /// </summary>
        /// <param name="email">email address of the user</param>
        /// <returns>BoxcarApiResult object</returns>
        public BoxcarApiResult Subscribe(string email)
        {
            return MakeRequest(ApiOperation.SUBSCRIBE, email, null, null, null, null, null);
        }

        /// <summary>
        /// Sends user a notification
        /// </summary>
        /// <param name="email">email address of the user</param>
        /// <param name="message">message to post</param>
        /// <returns>BoxcarApiResult object</returns>
        public BoxcarApiResult Notify(string email, string message)
        {
            return MakeRequest(ApiOperation.CREATE, email, null, message, null, null, null);
        }

        /// <summary>
        /// Sends user a notification
        /// </summary>
        /// <param name="email">email address of the user</param>
        /// <param name="fromName">[optional] name of the sender</param>
        /// <param name="message">message to post</param>
        /// <param name="id">[optional] An integer value that will uniquely identify the notification, 
        ///                  and prevent duplicate notifications about the same event from being created. 
        ///                  This is an optional field, but it is strongly recommended that you use it.</param>
        /// <param name="sourceUrl">[optional] This is the URL the user will be taken to when they open your message</param>
        /// <param name="iconUrl">[optional] This is the URL of the icon that will be shown to the user. Standard size is 57x57</param>
        /// <returns>BoxcarApiResult object</returns>
        public BoxcarApiResult Notify(string email, string fromName, string message, string id, string sourceUrl, string iconUrl)
        {
            return MakeRequest(ApiOperation.CREATE, email, fromName, message, id, sourceUrl, iconUrl);
        }

        /// <summary>
        /// Broadcasts the message to all users of your provider
        /// </summary>
        /// <param name="message">message to broadcast</param>
        /// <returns></returns>
        public BoxcarApiResult Broadcast(string message)
        {
            return MakeRequest(ApiOperation.BROADCAST, null, null, message, null, null, null);
        }

        /// <summary>
        /// Broadcasts the message to all users of your provider
        /// </summary>
        /// <param name="fromName">[optional] name of the sender</param>
        /// <param name="message">message to post</param>
        /// <param name="id">[optional] An integer value that will uniquely identify the notification, 
        ///                  and prevent duplicate notifications about the same event from being created. 
        ///                  This is an optional field, but it is strongly recommended that you use it.</param>
        /// <param name="sourceUrl">[optional] This is the URL the user will be taken to when they open your message</param>
        /// <param name="iconUrl">[optional] This is the URL of the icon that will be shown to the user. Standard size is 57x57</param>
        /// <returns>BoxcarApiResult object</returns>
        public BoxcarApiResult Broadcast(string fromName, string message, string id, string sourceUrl, string iconUrl)
        {
            return MakeRequest(ApiOperation.BROADCAST, null, fromName, message, id, sourceUrl, iconUrl);
        }

        private BoxcarApiResult MakeRequest(ApiOperation operation, string email, string fromName, string message, string id, string sourceUrl, string iconUrl)
        {
            BoxcarApiResult result = new BoxcarApiResult();

            try
            {
                string url = string.Empty;
                switch (operation)
                {
                    case ApiOperation.SUBSCRIBE: url = string.Format(URLFORMAT, _ApiKey, "subscribe"); break;
                    case ApiOperation.CREATE: url = string.Format(URLFORMAT, _ApiKey, ""); break;
                    case ApiOperation.BROADCAST: url = string.Format(URLFORMAT, _ApiKey, "broadcast"); break;
                }

                NameValueCollection data = new NameValueCollection();
                data.Add("token", _ApiKey);
                data.Add("secret", _ApiSecret);
                if (!string.IsNullOrEmpty(email)) { data.Add("email", email); }
                if (!string.IsNullOrEmpty(message)) { data.Add("notification[message]", message); }
                if (!string.IsNullOrEmpty(fromName)) { data.Add("notification[from_screen_name]", fromName); }
                if (!string.IsNullOrEmpty(id)) { data.Add("notification[from_remote_service_id]", id); }
                if (!string.IsNullOrEmpty(sourceUrl)) { data.Add("notification[source_url]", sourceUrl); }
                if (!string.IsNullOrEmpty(iconUrl)) { data.Add("notification[icon_url]", iconUrl); }

                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.UserAgent, USERAGENT);
                byte[] response = client.UploadValues(url, "POST", data);
                string responseStr = System.Text.Encoding.UTF8.GetString(response);

                result.Success = true;
                result.Response = responseStr;
            }
            catch (System.Net.WebException webex)
            {
                result.Success = false;

                result.Code = Convert.ToInt32(webex.Response.Headers["Status"]);

                switch (result.Code)
                {
                    case 400: result.Description = "Incorrect parameters passed"; break;
                    case 401: result.Description = "Request failed (possible causes: invalid token, or user has not added the service, or notification id sent twice)"; break;
                    case 403: result.Description = "Request failed (General)"; break;
                    case 404: result.Description = "User not found"; break;
                    default: result.Description = "Unknown response"; break;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Code = -1;
                result.Description = "Unknown Error : " + ex.Message;
            }

            return result;
        }
    }

    public class BoxcarApiResult
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Description { get; set; }
        public string Response { get; set; }
    }
}
