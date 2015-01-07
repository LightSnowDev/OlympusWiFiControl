using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DebugShellTest
{
    class webClass
    {
        public string POST(string cgi, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://192.168.0.10/" + cgi);
                var data = Encoding.ASCII.GetBytes(postData);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public string GET(string cgi)
        {
            try
            {
                WebClient webClient = new WebClient();
                string result = webClient.DownloadString("http://192.168.0.10/" + cgi);
                return result;
            }
            catch(Exception ex)
            {
                return "error" + ex.Message;
            }
        }
    }
}
