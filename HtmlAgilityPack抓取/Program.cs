using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace HtmlAgilityPack抓取
{
    class Program
    {
        const string url = "https://www.cnblogs.com";
        static HtmlWeb webClient = new HtmlWeb();
        static HtmlDocument doc = new HtmlDocument();

        static void Main(string[] args)
        {
            for (int i = 1; i < 2; i++)
            {
                var currentUrl = url + "/p" + i;

                //方法1 加载html
                //string html = PostWebRequest(currentUrl);
                //doc.LoadHtml(html);

                //方法2 直接通过插件加载DOM
                doc = webClient.Load(currentUrl);

                HtmlNodeCollection nodeList = doc.DocumentNode.SelectNodes("//div[@id='post_list']/div[@class='post_item']");
                if (nodeList != null && nodeList.Count > 0)
                {
                    foreach (HtmlNode node in nodeList)
                    {
                        var nodeElement = node.SelectSingleNode("div[@class='post_item_body']/h3/a");
                        var href = nodeElement.Attributes["href"].Value;
                        Console.WriteLine($"标题地址{nodeElement.InnerText}{href}");
                        Console.WriteLine($"==========文章内容==========");
                        doc = webClient.Load(href);
                        //var chirldNode = doc.DocumentNode.SelectSingleNode("//div[@id='post_detail']/div[1]/div[1]/div[1]");
                        var chirldNode = doc.DocumentNode.SelectSingleNode("//div[@id='cnblogs_post_body']");
                        //chirldNode.SelectSingleNode("//div[@class='postDesc']").Remove();
                        //chirldNode.RemoveChild(removeNode, true);
                        Trace.WriteLine($"内容：{chirldNode.InnerText}");
                        Console.WriteLine($"内容：{chirldNode.InnerText}");
                    }
                }
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="postUrl">URL</param>
        /// <param name="paramData">参数</param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string paramData=null)
        {
            try
            {
                if (!postUrl.StartsWith("http://") && !postUrl.StartsWith("https://"))
                    return "";

                HttpClient httpClient = new HttpClient();
                httpClient.MaxResponseContentBufferSize = 256000;
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");

                var stringContent = new StringContent(paramData == null ? "" : paramData, Encoding.UTF8);

                HttpResponseMessage response = httpClient.PostAsync(postUrl, stringContent).Result;
                var result= response.Content.ReadAsStringAsync().Result;
                Trace.WriteLine($"{postUrl},{paramData},{result}");
                httpClient.Dispose();
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

}