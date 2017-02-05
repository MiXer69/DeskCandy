using System;
using System.IO;
using System.Reflection;
using DeskCandyLib;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
namespace DeskCandy.PluginManager
{
	public class WallpaperSourceServices
	{
		public WallpaperSourceServices()
		{
		}
		private Types.AvailableWallpaperSource colAvailablePlugins = new Types.AvailableWallpaperSource();
		public Types.AvailableWallpaperSource AvailablePlugins {
			get { return colAvailablePlugins; }
			set { colAvailablePlugins = value; }
		}
		public void FindPlugins()
		{
			FindPlugins(AppDomain.CurrentDomain.BaseDirectory + "\\rsc\\Plugins");
		}
		/// <summary>
		/// Searches the passed Path for Plugins
		/// </summary>
		/// <param name="Path">Directory to search for Plugins in</param>
		public void FindPlugins(string Path)
		{
			//First empty the collection, we're reloading them all
			colAvailablePlugins.Clear();

			//Go through all the files in the plugin directory
			foreach (string fileOn in Directory.GetFiles(Path)) {
				FileInfo file = new FileInfo(fileOn);

				//Preliminary check, must be .dll
				if (file.Extension.Equals(".dll")) {
					//Add the 'plugin'
					this.AddPlugin(fileOn);
				}
			}
		}
		public void RegisterInternalPLugin(Type type)
		{
			Types.AvailablePlugin newPlugin = new Types.AvailablePlugin(type);

			//Add the new plugin to our collection here
			this.colAvailablePlugins.Add(newPlugin);
		}
			private void AddPlugin(string FileName)
		{

			//Create a new assembly from the plugin file we're adding..
			Assembly pluginAssembly = Assembly.LoadFrom(FileName);
			try {
				//Next we'll loop through all the Types found in the assembly
				foreach (Type pluginType in pluginAssembly.GetTypes()) {
					if (pluginType.IsPublic) { //Only look at public types
						if (!pluginType.IsAbstract) {  //Only look at non-abstract types
							//Gets a type object of the interface we need the plugins to match
							Type typeInterface = pluginType.GetInterface("DeskCandyLib.IWallpaperSource", true);

							//Make sure the interface we want to use actually exists
							if (typeInterface != null) {
								//Create a new available plugin since the type implements the PanelElement interface
								Types.AvailablePlugin newPlugin = new Types.AvailablePlugin(pluginAssembly.GetType(pluginType.ToString()));

								//Add the new plugin to our collection here
								this.colAvailablePlugins.Add(newPlugin);

								return;
							}
						}
					}
				}
			} catch (ReflectionTypeLoadException ex) {
				MessageBox.Show(ex.LoaderExceptions[0].ToString());
			}
			MessageBox.Show("Not a plugin: " + FileName);
		}
	}
	namespace Types
	{
		public class AvailableWallpaperSource : System.Collections.CollectionBase
		{
			public void Add(Types.AvailablePlugin pluginToAdd)
			{
				this.List.Add(pluginToAdd);
			}
			public void Remove(Types.AvailablePlugin pluginToRemove)
			{
				this.List.Remove(pluginToRemove);
			}
		}
		public class AvailablePlugin
		{
			private Type type;
			public Type Type { get { return type; } }

			public AvailablePlugin(Type t)
			{
				type = t;
			}

			public IWallpaperSource MakeInstance(string Query)
			{
                var s = (IWallpaperSource)Activator.CreateInstance(type);
                s.Query = Query;
                return s;
			}
            public IWallpaperSource MakeInstance()
            {
                var s = (IWallpaperSource)Activator.CreateInstance(type);
                return s;
            }

        }
	}
}

