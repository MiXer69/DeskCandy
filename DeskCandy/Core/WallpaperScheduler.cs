using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskCandyLib;
namespace DeskCandy.Core
{
    public class WallpaperScheduler
    {
        string SearchDir = DeskCandyLib.STATIC.ImagesPaths;
        List<IWallpaperSource> FoundedWallpapers = new List<IWallpaperSource>();
        int CurIndex;
        System.Timers.Timer wallpaperChange = new System.Timers.Timer();
        /// <summary>
        /// Main constructor for wallpaper scheduler
        /// </summary>
        /// <param name="Interval">Interval (in minutes)</param>
        public WallpaperScheduler(int Interval)
        {
            wallpaperChange.Interval = Interval * 60000;
        }
        public IWallpaperSource CurWallpaperSource
        {
            get
            {
                return FoundedWallpapers[CurIndex];
            }
        }
        public void NextWallpaper()
        {
            
        }
        public void PrevWallpaper()
        {

        }

    }
}
