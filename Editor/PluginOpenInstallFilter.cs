using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.OpenInstall.Editor
{
    [InitializeOnLoad]
    public static class PluginOpenInstallFilter
    {
        static PluginOpenInstallFilter()
        {
            PluginFilter.RegisterFilterExpress<BuildOpenInstallParams>("com.pluginset.openinstall", "Plugins/iOS");
//            PluginFilter.RegisterFilterExpress<BuildOpenInstallParams>("com.pluginset.openinstall", "Plugins/Android");
        }
    }
}