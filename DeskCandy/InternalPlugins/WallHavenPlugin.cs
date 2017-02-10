using DeskCandyLib;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeskCandy.InternalPlugins
{
    /// <summary>
    /// base URL https://alpha.wallhaven.cc/search?q=Anime&page=2
    /// link to wallpaper https://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-id.jpg
    /// </summary>
    class WallHavenPlugin
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
                return "Wallhaven Wallpaper Downloader. Please fill in \"query\" field for service to actually work.";
            }
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return "Wallhaven";
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
                return "https://alpha.wallhaven.cc/";
            }
        }
        /// <summary>
        /// Called on init
        /// </summary>
        /// <param name="query"></param>
        public void Init(string query)
        {
            Query = query;
            ParseWallpaperNumber(query, page);
        }
        /// <summary>
        /// Gets image 
        /// </summary>
        /// <param name="name">Name of the query</param>
        /// <param name="number">wallpaper number</param>
        public void GetImage(string name,string number)
        {
            WebClient webClient = new WebClient();
            Directory.CreateDirectory(STATIC.ImagesPaths + "\\" + name);
            webClient.DownloadFileAsync(new Uri(string.Format("https://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-{0}.jpg", number)), STATIC.ImagesPaths + "\\" + AnimeName + "\\" + number + ".jpg");
            Console.WriteLine("Downloading to " + STATIC.ImagesPaths + "\\" + name);
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
            ParseWallpaperNumber(Query, page);
            Console.WriteLine("Done Refreshing Images!");

        }
        /// <summary>
        /// Parses Page for Animes
        /// </summary>
        /// <param name="AnimeName">Name of the Anime/manga</param>
        public async void ParseWallpaperNumber(string Query)
        {
            Console.WriteLine("Parsing...");
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(new Uri(string.Format(" https://alpha.wallhaven.cc/search?q={0}&page={1}", Query.Replace(' ', '+'), page.ToString())));
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
                            GetImage(Query, s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Anime not found");
            }
        }
        public async void ParseWallpaperNumber(string Query, int page)
        {
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(new Uri(string.Format(" https://alpha.wallhaven.cc/search?q={0}&page={1}", Query.Replace(' ', '+'), page.ToString())));
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
                            GetImage(Query, s);
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
