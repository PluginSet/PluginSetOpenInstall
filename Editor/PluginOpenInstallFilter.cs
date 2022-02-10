using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.OpenInstall.Editor
{
    [InitializeOnLoad]
    public static class PluginOpenInstallFilter
    {
        static PluginOpenInstallFilter()
        {
            var filter = PluginFilter.IsBuildParamsEnable<BuildOpenInstallParams>();
            PluginFilter.RegisterFilter("com.pluginset.openinstall/Plugins/iOS", filter);
        }
    }
}