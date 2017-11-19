using EyexAAC.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace EyexAAC.ViewModel.Utils
{
   
    class HttpManager
    {
        private static readonly string LOG_SERVER_URL = "http://127.0.0.1:5000/log";
        public static bool Send(List<ActivityLogEntry> activityLog)
        {
            string activityLogAsJson = JsonConvert.SerializeObject(activityLog);
            string result = "";
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                try
                {
                    result = client.UploadString(LOG_SERVER_URL, "POST", activityLogAsJson);
                    if (result == "200-OK")
                    {
                        return true;
                    }
                    return false;
                }
                catch (WebException)
                {
                    return false;
                }
            }
        }
    }
}
