using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.OpenInstall.Editor
{
    [BuildChannelsParams("OpenInstall", "OpenInstall的SDK参数")]
    public class BuildOpenInstallParams: ScriptableObject
    {
        [Tooltip("是否启用OpenInstall")]
        public bool Enable;
        
        [Tooltip("OpenInstall的SDK提供的AppKey参数")]
        public string OpenInstallAppKey;
    }
}