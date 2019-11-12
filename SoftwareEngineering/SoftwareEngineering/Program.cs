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
            bool incorrectInput = false;
            int userSelection = -1;

            do
            {
                Console.Clear();
                SearchSpotify();
                Console.WriteLine("==*****   SPOTIFY PLAYLIST FROM NEWS CREATOR THING   *****==\n\n\n");
                Console.WriteLine(" * Please select an option from belows\n");
                Console.WriteLine(" 1 > Create playlist from news category\n 2 > Create playlist from country-specific news\n 3 > Random news selection");
                Console.Write("\n > ");

                
                try
                {
                    userSelection = Convert.ToInt16(Console.ReadLine());
                    if (userSelection < 1 || userSelection > 3)
                    {
                        Console.WriteLine("\n * Incorrect Input - Press Enter to Continue.");
                        incorrectInput = true;
                    }

                }
                catch
                {
                    userSelection = -1;
                    Console.WriteLine("\n * Incorrect Input Format - Press Enter to Continue.");
                    incorrectInput = true;
                }

                Console.ReadLine();
            } while (incorrectInput == true);
            Console.Clear();

            switch (userSelection)
            {
                case 1:
                    //Do things/call function for option 1
                    break;
                case 2:
                    //Do things/call function for option 2
                    break;
                case 3:
                    //Do things/call function for option 3
                    break;
                default:
                    break;
            }

            Console.ReadLine();
        }


        static void ConnectToNews()
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


        static void SearchSpotify()
        {

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