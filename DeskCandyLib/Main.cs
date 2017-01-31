using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DeskCandyLib
{
    public interface IWallpaperSource
    {
        /// <summary>
        /// Name of the source
        /// </summary>
        string Name { get; } // Name of the source
        /// <summary>
        /// Description of the source
        /// </summary>
        string Description { get;} //Description of the source
        /// <summary>
        /// Url of parsed Site (copyright n stuff)
        /// </summary>
        string Url { get;} //Url of parsed site
        /// <summary>
        /// this is searched query. Let's for example say that you wanna boat photos only. You're getting it, right? :D
        /// </summary>
        string Query { get; set; }
        /// <summary>
        /// Function Used to return an image url
        /// </summary>
        /// <returns></returns>
        BitmapImage GetImageUrl();
        /// <summary>
        /// Called on Init
        /// </summary>
        void Init(string query);


    }
    public static class STATIC
    {
        public static string ImagesPaths = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Wallpapers";
    }
}
