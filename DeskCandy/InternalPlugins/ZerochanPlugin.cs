using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DeskCandyLib;
using HtmlAgilityPack;

namespace DeskCandy.InternalPlugins
{
    /* ZerochanParser - Parser for zerochan
     * Remember to credit me after using this in your app: 
     * Piotr "MiXer" Mikstacki => ja.to.mixer@gmail.com
     * */
    public class Zerochan : IWallpaperSource
    {
        public List<string> AnimeNumbers = new List<string>();
        public string SearchedQuery;
        int page = 1;
        /// <summary>
        /// Opis
        /// </summary>
        public string Description
        {
            get
            {
                return "Zerochan Wallpaper Downloader. Please fill in \"query\" field for service to actually work. Well, you need at least something to search for!. Be careful! default query is Boku no Pico. Nah, just kidding";
            }
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return "Zerochan";
            }
        }

        public string Query
        {
            get
            {
                return SearchedQuery;
            }

            set
            {
                SearchedQuery = value;
            }
        }
        public string Url
        {
            get
            {
                return "Zerochan.net";
            }
        }
        /// <summary>
        /// Called on init
        /// </summary>
        /// <param name="query"></param>
        public void Init(string query)
        {
            Query = query;
            ParseAnimeNumber(query);
        }
        /// <summary>
        /// Gets image 
        /// </summary>
        /// <param name="AnimeName">Name of the anime</param>
        /// <param name="number">anime number</param>
        public void GetImage(string AnimeName, string number)
        {
            WebClient webClient = new WebClient();
            Directory.CreateDirectory(STATIC.ImagesPaths + "\\" + AnimeName);
            webClient.DownloadFileAsync(new Uri(string.Format("http://static.zerochan.net/{0}.full.{1}.jpg", AnimeName, number)), STATIC.ImagesPaths + "\\" + AnimeName + "\\" + number + ".jpg");
            Console.WriteLine("Downloading to " + STATIC.ImagesPaths + "\\" + AnimeName);
        }
        /// <summary>
        /// Refreshes Images
        /// </summary>
        public void RefreshImages()
        {
            Console.WriteLine("Refreshing Images...");

            System.IO.DirectoryInfo di = new DirectoryInfo(STATIC.ImagesPaths + "\\" + Query);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            page++;
            ParseAnimeNumber(Query, page);
            Console.WriteLine("Done Refreshing Images!");

        }
        /// <summary>
        /// Parses Page for Animes
        /// </summary>
        /// <param name="AnimeName">Name of the Anime/manga</param>
        public async void ParseAnimeNumber(string AnimeName)
        {
            Console.WriteLine("Parsing...");
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(new Uri(string.Format("http://zerochan.net/{0}", AnimeName.Replace(' ', '+'))));
                String source = Encoding.GetEncoding("UTF-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(source);
                HtmlNode ul = resultat.GetElementbyId("thumbs2");
                HtmlNodeCollection childList = ul.ChildNodes;
                foreach (HtmlNode n in childList)
                {
                    HtmlNodeCollection il = n.ChildNodes;
                    foreach (HtmlNode l in il)
                    {
                        if (l.HasAttributes)
                        {
                            List<char> chars = l.Attributes[0].Value.ToString().ToList<char>();
                            chars.RemoveAt(0);
                            string s = new string(chars.ToArray());
                            AnimeNumbers.Add(s);
                            GetImage(AnimeName, s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Anime not found");
            }
        }
        public async void ParseAnimeNumber(string AnimeName, int page)
        {
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(new Uri(string.Format("http://zerochan.net/{0}?p={1}", AnimeName.Replace(' ', '+'), page.ToString())));
                String source = Encoding.GetEncoding("UTF-8").GetString(response, 0, response.Length - 1);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(source);
                HtmlNode ul = resultat.GetElementbyId("thumbs2");
                HtmlNodeCollection childList = ul.ChildNodes;
                foreach (HtmlNode n in childList)
                {
                    HtmlNodeCollection il = n.ChildNodes;
                    foreach (HtmlNode l in il)
                    {
                        if (l.HasAttributes)
                        {
                            List<char> chars = l.Attributes[0].Value.ToString().ToList<char>();
                            chars.RemoveAt(0);
                            string s = new string(chars.ToArray());
                            AnimeNumbers.Add(s);
                            GetImage(AnimeName, s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Page Limit probably overloaded");
            }


        }
    }
}
