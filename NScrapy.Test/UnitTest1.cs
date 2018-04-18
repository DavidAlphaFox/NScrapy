using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScrapy.Infra;
using NScrapy.Infra.Attributes.SpiderAttributes;
using NScrapy.Shell;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NScrapy.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EngineTest()
        {
            NScrapy.Shell.NScrapy scrapy = NScrapy.Shell.NScrapy.GetInstance();
            HttpRequest request = new HttpRequest()
            {
                URL = "http://www.baidu.com"
            };
            scrapy.Context.CurrentEngine.ProcessRequest(request);
        }

        [TestMethod]
        public void ConstructDownloader()
        {
            var sw=Stopwatch.StartNew();
            sw.Start();
            //init context
            NScrapy.Shell.NScrapy scrapy = NScrapy.Shell.NScrapy.GetInstance();
            var request = new HttpRequest()
            {
                URL = "http://www.sina.com"
            };
            var request2 = new HttpRequest()
            {
                URL = "http://www.sina.com"
            };
            var request3 = new HttpRequest()
            {
                URL = "http://www.sina.com"
            };
            var request4 = new HttpRequest()
            {
                URL = "http://www.sina.com"
            };
            var request5 = new HttpRequest()
            {
                URL = "http://www.sina.com"
            };
            var response = Downloader.Downloader.SendRequestAsync(request);
            var response2 = Downloader.Downloader.SendRequestAsync(request2);
            var response3 = Downloader.Downloader.SendRequestAsync(request3);
            var response4 = Downloader.Downloader.SendRequestAsync(request4);
            var response5 = Downloader.Downloader.SendRequestAsync(request5);
            var result = response5.Result.ReponsePlanText;
            //var html= Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(result.ResponseMessage.Content.ReadAsStringAsync().Result));
            sw.Stop();
            var timeCost = sw.ElapsedMilliseconds;
            return;
        }

        [TestMethod]
        public void SpiderTest()
        {
            Shell.NScrapy.GetInstance().Crawl("JobSpider");
        }
    }

    [Name(Name = "JobSpider")]
    [URL("https://www.liepin.com/zhaopin/?d_sfrom=search_fp_nvbar&init=1")]
    public class JobSpider : Spider.Spider
    {
        public override IResponse ResponseHandler(IResponse response)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(response.ReponsePlanText);
            var result = doc.DocumentNode.Descendants("input");
            return null;
            //var parser = new HtmlParser();
            //var result=parser.Parse(response.ReponsePlanText);
        }
    }
}