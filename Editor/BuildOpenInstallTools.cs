using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor.iOS.Xcode;

namespace PluginSet.OpenInstall.Editor
{
    [BuildTools]
    public static class BuildOpenInstallTools
    {
        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            var buildParams = context.BuildChannels.Get<BuildOpenInstallParams>("OpenInstall");
            if (!buildParams.Enable)
                return;
            
            context.Symbols.Add("ENABLE_OPEN_INSTALL");
            
            context.AddLinkAssembly("PluginSet.OpenInstall");
            Global.CopyDependenciesInLib("com.pluginset.openinstall");
        }

        [iOSXCodeProjectModify]
        public static void OnXcodeProjectModify(BuildProcessorContext context, PBXProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildOpenInstallParams>("OpenInstall");
            projectManager.PlistDocument.AddXcodeURLType("openSdk", "Editor", buildParams.OpenInstallAppKey);
            projectManager.AddAssociatedDomainsApplinks($"{buildParams.OpenInstallAppKey}.openinstall.io");
        }
    }
}