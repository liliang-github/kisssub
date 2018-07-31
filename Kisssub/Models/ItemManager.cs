using HtmlAgilityPack;
using Kisssub.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kisssub.Models
{
    public class ItemManager
    {
        private const string uri = "http://www.kisssub.org/{0}.html";
        private const string contentUri = "http://www.kisssub.org/show-{0}.html";
        public async static Task GetAllItems(ObservableCollection<ItemModel> items, int page)
        {
            //接收返回的html代码
            string resultHtml = await HttpManager.GetHtml(String.Format(uri,page));

            resultHtml = resultHtml.Replace("\n"," ");

            //正则匹配
            string reg = "alt.*?now.*?>(.*?)<.*?show-(.*?).html.*?>(.*?)</a>.*?<td>(.*?)</td>.*?href.*?>(.*?)<";

            MatchCollection matchs = Regex.Matches(resultHtml, reg);

            foreach (Match match in matchs)
            {
                if (match.Success)
                {
                    ItemModel item = new ItemModel();
                    item.Date = match.Groups[1].Value.Trim();
                    item.UrlId = match.Groups[2].Value.Trim();
                    item.Name = match.Groups[3].Value.Trim();
                    item.Size = match.Groups[4].Value.Trim();
                    item.Author = match.Groups[5].Value.Trim();
                    items.Add(item);
                }
            }

        }


        public async static Task<string> GetHtmlContent(string uriId)
        {
            string resultHtml = await HttpManager.GetHtml(String.Format(contentUri,uriId));

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(resultHtml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class='intro']");
            if (node == null)
            {
                return "";
            }
            return node.InnerHtml;
        }

    }
}
