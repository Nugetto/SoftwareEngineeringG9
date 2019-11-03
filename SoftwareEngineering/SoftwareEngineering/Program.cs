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

using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Auth;


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

        private static SpotifyWebAPI _spotify;
        public static async void getSpotify()
        {
            //ClientID and SecretID
            CredentialsAuth auth = new CredentialsAuth("088f576b5164473c99d0f31d261d1501", "d42f8386acbe4f6b8894ef9ccae9ef0a");
            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
        }
        public static void  Main(string[] args)
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

            getSpotify();
            Console.ReadLine();

            SearchItem item = _spotify.SearchItems("news", SearchType.Track);
            for (int i = 0; i < item.Tracks.Items.Count; i++)
            {
                Console.WriteLine(item.Tracks.Items[i].Name);
            }
            
            Console.ReadLine();
        }

    }
}