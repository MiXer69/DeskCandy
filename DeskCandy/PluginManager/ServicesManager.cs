using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
namespace DeskCandy.PluginManager
{
    public class ServicesManager
    {
        System.Timers.Timer t = new System.Timers.Timer();
        private WallpaperSourceServices avPanelServices = new WallpaperSourceServices();
       
        public ServicesManager()
        {
            avPanelServices.RegisterInternalPLugin(typeof(InternalPlugins.Zerochan));
        }
        public void ChangeTime(int minutes)
        {

        }
    }
}
