using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.OpenInstall.Editor
{
    [BuildTools]
    public static class BuildOpenInstallTools
    {
        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            if (context.BuildTarget != BuildTarget.Android && context.BuildTarget != BuildTarget.iOS)
                return;
                
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
            if (!buildParams.Enable)
                return;
            
            projectManager.PlistDocument.SetPlistValue("com.openinstall.APP_KEY", buildParams.OpenInstallAppKey);
            projectManager.PlistDocument.AddXcodeURLType("openSdk", "Editor", buildParams.OpenInstallAppKey);
            projectManager.AddAssociatedDomainsApplinks($"{buildParams.OpenInstallAppKey}.openinstall.io");
        }
    }
}