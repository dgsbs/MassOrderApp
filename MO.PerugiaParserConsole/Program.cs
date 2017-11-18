using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;

namespace MO.PerugiaParserConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri url = new Uri("http://perugia.pl/");
            WebClient wc = new WebClient();
            string htmlText = wc.DownloadString(url);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlText);

            List<Product> products = new List<Product>();
            foreach (var product in doc.QuerySelectorAll("#makarony .post"))
            {
                Product p = new Product();
                p.Group = "Makaron";
                p.Name = product.QuerySelector("p").InnerText;

                if (product.QuerySelector("h3") != null)
                {
                    p.Price = product.QuerySelector("h3").InnerText;
                }
                products.Add(p);
            }

            foreach (var product in doc.QuerySelectorAll("#pizza .post"))
            {
                Product p = new Product();
                p.Group = "pizza";
                p.Name = product.QuerySelector("p").InnerText;

                if (product.QuerySelector("h3") != null)
                {
                    p.Price = product.QuerySelector("h3").InnerText;
                }
                products.Add(p);
            }

            foreach (var product in doc.QuerySelectorAll("#salatki .post"))
            {
                Product p = new Product();
                p.Group = "sałatka";
                p.Name = product.QuerySelector("p").InnerText;

                if (product.QuerySelector("h3") != null)
                {
                    p.Price = product.QuerySelector("h3").InnerText;
                }
                products.Add(p);
            }
            
            string json = JsonConvert.SerializeObject(products);
            File.WriteAllText("produkty.json", json);

            Console.WriteLine("Hello World!");
        }
    }

    public class Product
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
