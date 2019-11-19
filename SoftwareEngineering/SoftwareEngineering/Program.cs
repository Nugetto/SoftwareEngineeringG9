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
using System.Text.RegularExpressions;

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
                Console.WriteLine("==*****   SPOTIFY PLAYLIST FROM NEWS CREATOR THING   *****==\n\n\n");
                Console.WriteLine(" * Please select an option from belows\n");
                Console.WriteLine(" 1 > Create playlist from news category\n 2 > Create playlist from your countries news\n 3 > Random news selection");
                Console.Write("\n > ");

                try
                {
                    userSelection = Convert.ToInt16(Console.ReadLine());
                    if (userSelection < 1 || userSelection > 3)
                    {
                        Console.WriteLine("\n * Incorrect Input - Press Enter to Continue.");
                        incorrectInput = true;
                        Console.ReadLine();
                    }
                    else
                    {
                        incorrectInput = false;
                    }
                }
                catch
                {
                    userSelection = -1;
                    Console.WriteLine("\n * Incorrect Input Format - Press Enter to Continue.");
                    incorrectInput = true;
                    Console.ReadLine();
                }

            } while (incorrectInput == true);
            Console.Clear();

            switch (userSelection)
            {
                case 1: //News category specific
                    ConnectToNews("category=" + getNewsCategory() + "&country=gb");
                    break;
                case 2: //Country specific case
                    ConnectToNews("country=" + GetCountryCode());
                    break;
                case 3: //Random news selection?

                    break;
                default:
                    break;
            }

            Console.ReadLine();
        }


        static void ConnectToNews(string searchPerameters)
        {
            //Get the news, change the url to get different news
            var url = "https://newsapi.org/v2/top-headlines?" + searchPerameters + "&" + "apiKey=2cdba516b7024c7eb765e9f0b186c0eb";
            var json = new WebClient().DownloadString(url);

            //Using Newtonsoft.Json to deserialise the Json as a News.Rootobject Object 
            News.Rootobject deserializedNews = JsonConvert.DeserializeObject<News.Rootobject>(json);

            //The string can now be used for stuff like this (the titles of all the headline articles)
            for (int i = 0; i < deserializedNews.articles.Length; i++)
            {
                Console.WriteLine(deserializedNews.articles[i].title);
            }
            SearchSpotify(deserializedNews);
        }

        //Asks the user how long they want their playlist to be with exception handling
        static int getListLength()
        {
            bool incorrectInput = false;
            int userSelection;

            do
            {
                Console.Clear();
                Console.WriteLine(" * How many songs would you like in the playlist? (5-100)");
                Console.Write(" > ");
                try
                {
                    userSelection = Convert.ToInt16(Console.ReadLine());
                    if (userSelection < 5 || userSelection > 100) //Playlist must be longer than 5 (need to use top 5 keywords) and less than 100 items
                    {
                        Console.WriteLine("\n * Incorrect Input - Press Enter to Continue.");
                        incorrectInput = true;
                        Console.ReadLine();
                    }
                    else
                    {
                        incorrectInput = false;
                    }
                }
                catch
                {
                    userSelection = -1;
                    Console.WriteLine("\n * Incorrect Input Format - Press Enter to Continue.");
                    incorrectInput = true;
                    Console.ReadLine();
                }

            } while (incorrectInput == true);
            return userSelection;
        }

        //User selects the category they want their news from with exception handling
        public static string getNewsCategory()
        {
            bool incorrectInput = false;
            int userSelection = -1;
            string[] possibleCategories = { "Business", "Entertainment", "General", "Health", "Science", "Sports", "Technology" };

            do
            {
                Console.Clear();
                Console.WriteLine(" * Please select a news category from the options below\n");
                for (int i = 0; i < possibleCategories.Length; i++)
                {
                    Console.WriteLine(i + 1 + " > " + possibleCategories[i]);
                }
                Console.Write("\n > ");

                try
                {
                    userSelection = Convert.ToInt16(Console.ReadLine());
                    if (userSelection < 1 || userSelection > possibleCategories.Length)
                    {
                        Console.WriteLine("\n * Incorrect Input - Press Enter to Continue.");
                        incorrectInput = true;
                        Console.ReadLine();
                    }
                    else
                    {
                        incorrectInput = false;
                    }
                }
                catch
                {
                    userSelection = -1;
                    Console.WriteLine("\n * Incorrect Input Format - Press Enter to Continue.");
                    incorrectInput = true;
                    Console.ReadLine();
                }

            } while (incorrectInput == true);
            Console.Clear();

            return possibleCategories[userSelection-1].ToLower(); //Returns the users selection from possibleCategories
        }

        static void SearchSpotify(News.Rootobject news)
        {
            getSpotify();
            Console.WriteLine("Enter to start search");
            Console.ReadLine();

            Dictionary<string, int> wordFreq = new Dictionary<string, int>();
            string[] desiredWords = { "fury", "rage", "outrage", "sex", "sexy", "fraud", "row", "attack", "football", "sport", "rugby", "pool", "roar", "cash", "kick", "stab", "punch", "hit", "suspect", "gunman", "extremist", "genocide", "death", "killed", "award", "medal", "funny", "royal", "queen", "prince", "king", "climate", "fined", "surprise", "labour", "conservative", "green", "brexit", "animal", "politics", "premier", "league", "celebrity", "firefighter", "policeman", "drama", "outbreak", "angrily", "netflix", "facebook", "google", "snapchat", "twitter", "dad", "mum", "father", "grandfather", "grandmother", "mother", "healthy", "genetics", "fundraising", "plastic"};
            string[] undesiredWords = { "expresscouk", "a", "and", "or", "if", "i", "in", "claims", "expert", "by", "of", "it", "to", "news", "mail", "telegraph", "the", "also", "up", "down", "left", "right", "yes", "no", "from", "on", "off", "under", "with", "till", "than", "any", "every", "other", "some", "such", "come", "get", "give", "go", "keep", "let", "make", "put", "seem", "take", "do", "have", "say", "but", "though", "when", "where", "how", "why", "who", "far", "forward", "near", "now", "uk", "against", ".com", ".co.uk", "pro", "before", ",", ".", " ", "want", "me", "gone", "will", "only", "leave", "my", "you", "took", "your",  "that", "he", "be", "new", "deal", "at", "had", "she", "today", "its", "may", "is", "out", "general", "are", "both", "an", "what", "into", "has", "his", "for", "told", "was", "her", "after", "not", "says", "said", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "as", "could", "been", "else", "someone", "partner", "watch", "one", "two", "three", "four", "five", "weeks", "spent", "-", "", "once", "twice", "mother", "first", "second", "while", "chars", "image", "whats", "us", "caption", "citys", "unveiled", "guardian", "bbc", "live"};

            for (int i = 0; i < news.articles.Length; i++)
            {
                Regex rgx = new Regex("[^a-zA-Z -]"); //Gets rid of all special characters
                if (news.articles[i].content == null)
                {
                    news.articles[i].content = rgx.Replace(news.articles[i].title ?? "", "");

                } else
                {
                    news.articles[i].content = rgx.Replace(news.articles[i].content ?? "", "");

                }
                foreach (string word in (news.articles[i].content).Split(' '))
                {
                    string wordL = (word).ToLower(); //puts everything in lower case
                    if (!undesiredWords.Contains(wordL) && wordFreq.ContainsKey(wordL)) //checks to see if word is in 'undesired words' list
                    {
                        wordFreq[wordL]++; //adds one to freq if already in list
                    }
                    else if (!undesiredWords.Contains(wordL))
                    {
                        wordFreq.Add(wordL, 1); //if not in the list, add it
                    }
                }
            }

            var sortedDict = from entry in wordFreq orderby entry.Value descending select entry; //sorts dictionary list
            Dictionary<string, int> top5words = new Dictionary<string, int>(); //creates new dictionary

            int wordCount = 0;
            foreach (KeyValuePair<string, int> kvp in sortedDict)
            {
                if (wordCount < 5) //gets top 5 frequent words
                {
                    top5words.Add(kvp.Key, kvp.Value);
                    wordCount++;
                }

                Console.WriteLine(string.Format("Word = {0} | Freq = {1}", kvp.Key, kvp.Value));
            }

            Console.ReadLine();

            int playlistLength = getListLength();



            //string[,] createdPlaylist = new string[userSelection,2];
            //Dictionary<string, string> createdPlaylist = new Dictionary<string, string>();
            var createdPlaylist = new List<Song>(); //Creates new list of class song

			int playlistRemainder = playlistLength % 5;
			int wordNumber = 0;
			foreach (KeyValuePair<string, int> kvp in top5words)
            {
				int extraSong = 0;
				wordNumber++;
				if (wordNumber <= playlistRemainder)
				{
					extraSong = 1;
				}
                Console.WriteLine(string.Format("Word = {0} | Freq = {1}", kvp.Key, kvp.Value));
                SearchItem item = _spotify.SearchItems(kvp.Key, SearchType.Track);
                for (int i = 0; i < (playlistLength/5)+extraSong; i++) //goes through each top word and gets the userselection/5 (equally distributed songs/word)
                {
                    Song tempSong = new Song(); //creates new instance of the song class
                    tempSong.SongName = item.Tracks.Items[i].Name; //sets name
                    tempSong.Artist = Convert.ToString(item.Tracks.Items[i].Artists[0].Name); //sets artist
                    createdPlaylist.Add(tempSong); //adds song to playlist
                }
				
            }


            //freaquency analysis on all headlines retrieved
            //Loop spotify search for the top 5(?) words
            //Total ammount of songs/ 5 rounded up is the ammount of songs needed for each word
            //Might need to remove some random songs

            Console.WriteLine("\n");
            foreach (Song song in createdPlaylist) //outputs all songs
            {
                Console.WriteLine(string.Format("Song = {0}\nArtist = {1}\n\n", song.SongName, song.Artist));
            }

        }

        public class Song
        {
            public string SongName { get; set; }
            public string Artist { get; set; }
        }

        public static string GetCountryCode()
        {

            string country = System.Globalization.RegionInfo.CurrentRegion.EnglishName;

            switch (country)
            {
                case "United Kingdom":
                    return "gb";

            }

            return null;
        }

    }
}