using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kisssub.Service
{
    public class HttpManager
    {
        private static HttpClient httpClient = new HttpClient();
        public async static Task<string> GetHtml(string url)
        {
            string result = await httpClient.GetStringAsync(url);

            return result;
        }
    }
}
