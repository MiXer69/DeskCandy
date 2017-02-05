using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskCandyLib;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using DeskCandy.PluginManager.Types;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;

namespace DeskCandy.Core
{
    public class WallpaperScheduler
    {
        string SearchDir = DeskCandyLib.STATIC.ImagesPaths;
        List<IWallpaperSource>  Sources = new List<IWallpaperSource>();
        List<string> AllWallpapersPaths = new List<string>();
        string curWallpaperPath;
        int CurIndex = -1;
        System.Timers.Timer wallpaperChange = new System.Timers.Timer();
        WallpaperChange.Style curScaleStyle;
        /// <summary>
        /// Returns SourcesOfWallpapers (For the sake of sources management)
        /// </summary>
        public List<IWallpaperSource> WallpaperSources
        {
            get
            {
                return Sources;
            }
        }
        /// <summary>
        /// Main constructor for wallpaper scheduler
        /// </summary>
        /// <param name="Interval">Interval (in minutes)</param>
        public WallpaperScheduler(int Interval)
        {
            var SearchTimer = new System.Timers.Timer(50000);
            SearchTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => { DirSearch(AllWallpapersPaths, STATIC.ImagesPaths); };
            SearchTimer.Start();
            wallpaperChange.Interval = Interval * 60000;
            wallpaperChange.Elapsed += WallpaperChange_Elapsed;
            wallpaperChange.Start();
            DirSearch(AllWallpapersPaths, STATIC.ImagesPaths);

        }
        /// <summary>
        /// Saves Sources
        /// </summary>
        public void SaveSources()
        {
            string json = JsonConvert.SerializeObject(Sources, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
            Properties.Settings.Default.Sources = json;
            Properties.Settings.Default.FirstRun = false;
            Properties.Settings.Default.Save();
        }
        /// <summary>
        /// Load Sources
        /// </summary>
        public void LoadSources()
        {
            //Console.WriteLine(Properties.Settings.Default.Sources );
            if (!Properties.Settings.Default.FirstRun)
            {
                Sources = JsonConvert.DeserializeObject<List<IWallpaperSource>>(Properties.Settings.Default.Sources, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
                foreach(IWallpaperSource s in Sources) { s.Init(s.Query); }
                DirSearch(AllWallpapersPaths, STATIC.ImagesPaths);
                MainWindow.instance.LoadSources();
           }
        }

        /// <summary>
        /// Adding source of wallpaper
        /// </summary>
        /// <param name="SourceName">Source</param>
        /// <param name="Query">Query to search</param>
        public void AddSource(string SourceName, string Query)
        {
            if (PluginExists(SourceName))
            {
                if (!Sources.Any(x => x.Name != SourceName && x.Query != Query))
                {
                    var Source = ReturnPlugin(SourceName, Query);
                    Source.Init(Query);
                    WallpaperSources.Add(Source);
                    SaveSources();
                }
                else
                {
                    MainWindow.instance.ShowMessageAsync("Error!", "Item with that source and query already added, find something else!", MessageDialogStyle.Affirmative, null);
                }
                    
            }
        }
        /// <summary>
        /// Remove wallaper source
        /// </summary>
        /// <param name="SourceName">Source's name to remove</param>
        /// <param name="Query">Source's query to remove</param>
        public void RemoveSource(string SourceName, string Query)
        {
            if (Sources.Count != 0)
            {
                Console.WriteLine("Removing " + Query + " with source "+SourceName);
                Sources.Remove(WallpaperSources.Find(x => x.Query == Query && x.Name == SourceName));
                SaveSources();
            }
        }

        /// <summary>
        /// Checks if plugins even exists (plugin services cannot use linq :c)
        /// </summary>
        /// <param name="name">name of plugin to "verify"</param>
        /// <returns></returns>
        public bool PluginExists(string name)
        {
            foreach (PluginManager.Types.AvailablePlugin t in MainWindow.instance.avWallpaperSources.AvailablePlugins)
            {
                var p = t.MakeInstance();
                if (p.Name == name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns plugin
        /// </summary>
        /// <param name="name">name of plugin</param>
        /// <param name="Query">Query of plugin</param>
        /// <returns></returns>
        public IWallpaperSource ReturnPlugin(string name, string Query)
        {
            foreach (PluginManager.Types.AvailablePlugin t in MainWindow.instance.avWallpaperSources.AvailablePlugins)
            {
                var p = t.MakeInstance(Query);
                if (p.Name == name)
                {
                    return p;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// Scalling style
        /// </summary>
        public WallpaperChange.Style scalingStyle
        {
            get {
                return curScaleStyle;
            }
            set {
                curScaleStyle = value;
            }
        }
        /// <summary>
        /// returns if string is null or nah ;_;
        /// </summary>
        /// <param name="s">string </param>
        /// <returns>bool</returns>
        bool isNull(string s)
        {
            if (s == "")
            {
                return true;
            }
            if (s == null)
            {
                return true;
            }
            else {
                return false;
            }
        }
        /// <summary>
        /// Void called when timer for wallpaper change is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallpaperChange_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Timer Elapsed, Searching Directory and Invoking NextWallpaper()");
            DirSearch(AllWallpapersPaths, STATIC.ImagesPaths);
            NextWallpaper();
        }
        /// <summary>
        /// Return current source of wallpaper
        /// </summary>
        public IWallpaperSource CurWallpaperSource
        {
            get
            {
                string sourceQuery = Path.GetDirectoryName(curWallpaperPath);
                return Sources.Find(x => x.Query == sourceQuery);
            }
        }
        /// <summary>
        /// Searches for next wallpaper
        /// </summary>
        public void NextWallpaper()
        {
            if (AllWallpapersPaths.Count != 0)
            {
                Console.WriteLine("Setting Wallpaper");
                DirSearch(AllWallpapersPaths, STATIC.ImagesPaths);
                if (CurIndex < AllWallpapersPaths.Count)
                {
                    CurIndex++;
                    WallpaperChange.Set(AllWallpapersPaths[CurIndex], WallpaperChange.Style.Stretched);
                    Console.WriteLine(AllWallpapersPaths[CurIndex].ToString());

                }
                else
                {
                    CurIndex = 0;
                    WallpaperChange.Set(AllWallpapersPaths[CurIndex], WallpaperChange.Style.Stretched);
                }
            }
            else
            {
                Console.WriteLine("No wallpapers, checking if there are plugins...");
                Console.WriteLine("There are no Plugins!");

            }
        }

        /// <summary>
        /// Changes for previous wallpaper
        /// </summary>
        public void PrevWallpaper()
        {
            if (AllWallpapersPaths.Count > 0)
            {
                CurIndex--;
                WallpaperChange.Set(AllWallpapersPaths[CurIndex], WallpaperChange.Style.Stretched);
            }
            else {
                CurIndex = AllWallpapersPaths.Count - 1;
                WallpaperChange.Set(AllWallpapersPaths[CurIndex], WallpaperChange.Style.Stretched);
            }
        }
        /// <summary>
        /// Refreshes source. It forces source to delete images in it's parent directory
        /// </summary>
        /// <param name="es">Source to refresh</param>
        public void RefreshSource(IWallpaperSource es)
        {
            es.RefreshImages();
        }
        /// <summary>
        /// Directory Search
        /// </summary>
        /// <param name="files"></param>
        /// <param name="startDirectory"></param>
        public void DirSearch(List<string> files, string startDirectory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(startDirectory, "*.*"))
                {
                    string extension = Path.GetExtension(file);

                    if (extension != null)
                    {

                        files.Add(file);
                    }
                }

                foreach (string directory in Directory.GetDirectories(startDirectory))
                {
                    DirSearch(files, directory);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
