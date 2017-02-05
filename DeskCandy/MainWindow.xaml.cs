using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeskCandy.PluginManager;
using DeskCandy.Core;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using DeskCandy.PluginManager.Types;
using DeskCandyLib;

namespace DeskCandy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public static MainWindow instance;
        public WallpaperSourceServices avWallpaperSources = new WallpaperSourceServices();
        public WallpaperScheduler wallpaperScheduler = new WallpaperScheduler(1);
        public ServicesManager plMan = new ServicesManager();
        public MainWindow()
        {
            Console.WriteLine("Initializing...");
            InitializeComponent();
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Wallpapers");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\rsc\\Plugins");
            avWallpaperSources.FindPlugins();
            //wallpaperScheduler.LoadSources();
            instance = this;
            avWallpaperSources.RegisterInternalPLugin(typeof(InternalPlugins.Zerochan));
            Console.WriteLine("Done! Loading Plugins...");
            LoadPlugins();
            Console.WriteLine("Loaded plugins!");
            Console.WriteLine("Done! Loading Sources...");
            wallpaperScheduler.LoadSources();
            Console.WriteLine("Loaded sources!");
        }
        /// <summary>
        /// Loads plugins to TypeBox
        /// </summary>
        public void LoadPlugins()
        {
            TypeBox.Items.Clear();
            foreach (AvailablePlugin s in avWallpaperSources.AvailablePlugins)
            {
                TypeBox.Items.Add(s.MakeInstance().Name);
            }
            LoadSources();
        }
        /// <summary>
        /// Loads Sources to orderBox
        /// 
        /// </summary>
        public void LoadSources()
        {
            OrderBox.Items.Clear();

            if (wallpaperScheduler.WallpaperSources.Count != 0)
            {
                foreach (IWallpaperSource s in wallpaperScheduler.WallpaperSources)
                {
                    Console.WriteLine("Adding "+ s.Query +"["+s.Name+"] "+ "To Sources...");
                    OrderBox.Items.Add(s.Query + "[" + s.Name + "]");
                    Console.WriteLine("Added!");

                }
            }

        }
        public void MoveItem(int direction)
        {
            // Checking selected item
            if (OrderBox.SelectedItem == null || OrderBox.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = OrderBox.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= OrderBox.Items.Count)
                return; // Index out of range - nothing to do

            object selected = OrderBox.SelectedItem;

            // Removing removable element
            OrderBox.Items.Remove(selected);
            // Insert it in new position
            OrderBox.Items.Insert(newIndex, selected);
            // Restore selection
            OrderBox.SelectedIndex = newIndex;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddFlyout.IsOpen = true;

        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            string[] titlestring = OrderBox.SelectedItem.ToString().Trim(new char[] { ' ' }).Split(new char[] { '[' , ']' });
            wallpaperScheduler.RemoveSource(titlestring[1], titlestring[0]);
            
            LoadSources();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            MoveItem(-1);
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            MoveItem(1);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (TypeBox.SelectedItem.ToString() != null || QueryBox.Text != null)
            {
                wallpaperScheduler.AddSource(TypeBox.SelectedItem.ToString(), QueryBox.Text);
                LoadSources();
                AddFlyout.IsOpen = false;
                QueryBox.Clear();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            wallpaperScheduler.NextWallpaper();
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            wallpaperScheduler.PrevWallpaper();
        }
    }
}
