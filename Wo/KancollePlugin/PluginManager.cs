using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wo.Logger;

namespace Wo.KancollePlugin
{
    class PluginManager
    {
        public event Action<List<IKancollePlugin>> OnPluginsLoaded;

        public const string PluginFolder = "Plugins";

        public List<IKancollePlugin> Plugins { get; private set; } = new List<IKancollePlugin>();

        public void LoadPlugins()
        {
            LogFactory.SystemLogger.Info("开始载入插件");
            Plugins.Clear();
            DirectoryInfo pluginRootFolder = new DirectoryInfo(PluginFolder);
            if (pluginRootFolder.Exists)
            {
                var pluginLoader = new PluginLoader<IKancollePlugin>();
                var aiFolders = (from di in pluginRootFolder.GetDirectories()
                                 select di.FullName).ToList();
                var collection = pluginLoader.LoadPlugins(aiFolders);
                if (collection.Count > 0)
                {
                    foreach (var pluginDll in collection)
                    {
                        foreach (var plugin in pluginDll.PluginObjects)
                        {
                            if (plugin.Name == null || plugin.Name == "")
                            {
                                LogFactory.SystemLogger.Error($"插件文件'{pluginDll.Name}'中的类型{plugin.GetType().FullName}'载入失败，插件名'Name'不能为空");
                                continue;
                            }
                            var samePlugin = Plugins.Find(p => p.Name == plugin.Name);
                            if (samePlugin != null)
                            {
                                IKancollePlugin p;
                                if (samePlugin.Version > plugin.Version)
                                    p = samePlugin;
                                else
                                    p = plugin;
                                LogFactory.SystemLogger.Error($"存在多个插件'{plugin.Name}'，将只保留版本最高的一个[Version={p.Version}]");
                                Plugins.Remove(samePlugin);
                                Plugins.Add(p);
                            }
                            else
                            {
                                Plugins.Add(plugin);
                                LogFactory.SystemLogger.Info($"插件'{plugin.Name}'载入成功");
                            }
                        }
                    }
                }
            }

            LogFactory.SystemLogger.Info($"插件载入完毕，共载入{Plugins.Count}个插件");
            OnPluginsLoaded?.Invoke(Plugins);
        }
    }
}
