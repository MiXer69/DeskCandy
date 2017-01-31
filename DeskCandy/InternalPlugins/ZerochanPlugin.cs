using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        public string curNumber;
        public string SearchedQuery;
        int lastindex = -1;
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
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\" + AnimeName);
            webClient.DownloadFileAsync(new Uri(string.Format("http://static.zerochan.net/{0}.full.{1}.jpg", AnimeName, number)), STATIC.ImagesPaths + "\\" + AnimeName + "\\" + number + ".jpg");
        }
        /// <summary>
        /// Gets url of the image
        /// </summary>
        /// <returns>Image for wallpaper</returns>
        public BitmapImage GetImageUrl()
        {
            if (curNumber != AnimeNumbers[AnimeNumbers.Count - 1])
            {
                lastindex++;
                curNumber = AnimeNumbers[lastindex];
                return new BitmapImage(new Uri(STATIC.ImagesPaths + "\\" + SearchedQuery + "\\" + curNumber.ToString() + ".jpg"));
            }
            else {
                lastindex = 0;
                curNumber = AnimeNumbers[lastindex];
                return new BitmapImage(new Uri(STATIC.ImagesPaths + "\\" + SearchedQuery + "\\" + curNumber.ToString() + ".jpg"));
            }
        }
        /// <summary>
        /// Parses Page for Animes
        /// </summary>
        /// <param name="AnimeName">Name of the ANime/manga</param>
        public async void ParseAnimeNumber(string AnimeName)
        {
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
                MessageBox.Show("Anime not found");
            }
        }
    }
}
