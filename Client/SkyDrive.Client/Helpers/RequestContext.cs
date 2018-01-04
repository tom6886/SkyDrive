using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;

namespace SkyDrive.Client
{
    public class RequestContext
    {
        private static string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        private static Dictionary<string, string> urls = StaticMenus.GetInstance().List;

        public static RequestResult Get(string key, string paramsStr)
        {
            if (!urls.ContainsKey(key)) { return null; }

            string url = baseUrl + urls[key] + paramsStr;

            WebClient wCient = new WebClient();

            string returnStr = Encoding.UTF8.GetString(wCient.DownloadData(url));

            return JsonConvert.DeserializeObject<RequestResult>(returnStr);
        }

        public static RequestResult Post(string key, string paramsStr)
        {
            if (!urls.ContainsKey(key)) { return null; }

            string url = baseUrl + urls[key];

            byte[] postData = Encoding.UTF8.GetBytes(paramsStr);

            WebClient wCient = new WebClient();

            wCient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可 

            byte[] responseData = wCient.UploadData(url, "POST", postData);//得到返回字符流  

            string returnStr = Encoding.UTF8.GetString(responseData);//解码  

            return JsonConvert.DeserializeObject<RequestResult>(returnStr);
        }

        public static RequestResult PostFile(string key, string header, string fileID, byte[] data)
        {
            if (!urls.ContainsKey(key)) { return null; }

            string url = baseUrl + urls[key] + "?id=" + fileID;

            WebClient wCient = new WebClient();

            wCient.Headers.Add(HttpRequestHeader.ContentRange, header);

            byte[] responseData = wCient.UploadData(url, "POST", data);//得到返回字符流  

            string returnStr = Encoding.UTF8.GetString(responseData);//解码  

            return JsonConvert.DeserializeObject<RequestResult>(returnStr);
        }
    }
}
