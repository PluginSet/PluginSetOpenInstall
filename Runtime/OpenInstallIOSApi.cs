#if !UNITY_EDITOR && ENABLE_OPEN_INSTALL
using System.Runtime.InteropServices;

#if UNITY_IOS
namespace PluginSet.OpenInstall
{
    public static class OpenInstallIOSApi
    {
        [DllImport("__Internal")]
        public static extern void Plugin_OpenInstall_Init();

        [DllImport("__Internal")]
        public static extern void Plugin_OpenInstall_ReportRegister();

        [DllImport("__Internal")]
        public static extern void Plugin_OpenInstall_SetUserAdid(string adid);

        [DllImport("__Internal")]
        public static extern bool Plugin_OpenInstall_GetInstallParams(string gameObjectName);

        [DllImport("__Internal")]
        public static extern void Plugin_OpenInstall_ReportEffectPoint(string name, long value);
    }
}
#endif
#endif