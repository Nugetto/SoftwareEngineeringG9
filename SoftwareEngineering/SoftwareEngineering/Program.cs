using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using Newtonsoft.Json;


namespace SoftwareEngineering
{
    class Program
    {
        //Class structure of the recived Json string
        public class News
        {
            public class Rootobject
            {
                public string status { get; set; }
                public int totalResults { get; set; }
                public Article[] articles { get; set; }
            }

            public class Article
            {
                public Source source { get; set; }
                public string author { get; set; }
                public string title { get; set; }
                public string description { get; set; }
                public string url { get; set; }
                public string urlToImage { get; set; }
                public DateTime publishedAt { get; set; }
                public string content { get; set; }
            }

            public class Source
            {
                public string id { get; set; }
                public string name { get; set; }
            }
        }

        static void Main(string[] args)
        {
            //Get the news, change the url to get different news
            var url = "https://newsapi.org/v2/top-headlines?" + "country=us&" + "apiKey=2cdba516b7024c7eb765e9f0b186c0eb";
            var json = new WebClient().DownloadString(url);

            //Using Newtonsoft.Json to deserialise the Json as a News.Rootobject Object 
            News.Rootobject deserializedNews = JsonConvert.DeserializeObject<News.Rootobject>(json);

            //The string can now be used for stuff like this (the titles of all the headline articles)
            for (int i = 0; i < deserializedNews.articles.Length; i++)
            {
                Console.WriteLine(deserializedNews.articles[i].title);
            }
            Console.ReadLine();
            
        }

    }
}