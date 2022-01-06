#if ENABLE_OPEN_INSTALL
using System;
using System.Collections.Generic;
using PluginSet.Core;
using UnityEngine;
using Logger = PluginSet.Core.Logger;

namespace PluginSet.OpenInstall
{
    [PluginRegister]
    public class PluginOpenInstall: PluginBase, IPrivacyAuthorizationCallback, IReport, ICustomPlugin, IChannel
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("OpenInstall");
        
        private struct GetOpenInstallResult
        {
            public string channelCode;
            public string bindData;

            public GetOpenInstallResult(string channelCode, string bindData)
            {
                this.channelCode = channelCode;
                this.bindData = bindData;
            }
        }
        
        public override string Name => "OpenInstall";

        private bool isPrivacyAuthorizated = false;
        private List<Action<Result>> getInstallParamsCallbacks = new List<Action<Result>>();

        private int _channelCode = -1;

        public int ChannelPriority => 1000;
        
        public string GetChannelName()
        {
            return null;
        }

        public int GetChannelId()
        {
            return _channelCode;
        }

        public void OnPrivacyAuthorization()
        {
            isPrivacyAuthorizated = true;
#if !UNITY_EDITOR && UNITY_IOS
            OpenInstallIOSApi.Plugin_OpenInstall_Init();
#endif
        }

        public void OnUserRegister()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
#elif UNITY_IOS && !UNITY_EDITOR
            OpenInstallIOSApi.Plugin_OpenInstall_ReportRegister();
#endif
        }

        public void OnUserRealName()
        {
        }
        
//        public void CustomEvent(string customEventName, Dictionary<string, object> eventData = null)
//        {
//#if UNITY_ANDROID && !UNITY_EDITOR
//#elif UNITY_IOS && !UNITY_EDITOR
//            OpenInstallIOSApi.Plugin_OpenInstall_ReportEffectPoint(customEventName, 0);
//#endif
//        }

        private void OnGetInstallParams(string jsonData)
        {
            Logger.Debug("OnGetInstallParams :" + jsonData);
            
            var getResult = JsonUtility.FromJson<GetOpenInstallResult>(jsonData);

            if (!string.IsNullOrEmpty(getResult.channelCode))
            {
                if (int.TryParse(getResult.channelCode, out _channelCode))
                {
                    Logger.Debug("OpenInstall channel set to :: " + _channelCode);
                    SendNotification(PluginConstants.NOTIFY_CHANNEL_CHANGED);
                }
                else
                {
                    _channelCode = -1;
                }
            }
            
            var result = new Result
            {
                Success = true,
                Data = getResult.bindData
            };
            
            foreach (var callback in getInstallParamsCallbacks)
            {
                callback.Invoke(result);
            }
            getInstallParamsCallbacks.Clear();
        }

        private bool CallGetInstallParams()
        {
#if !UNITY_EDITOR && UNITY_IOS
            return OpenInstallIOSApi.Plugin_OpenInstall_GetInstallParams(gameObject.name);
#else
            return false;
#endif
        }

        private void GetInstallParams(Action<Result> callback)
        {
            if (!isPrivacyAuthorizated)
            {
                callback?.Invoke(new Result
                {
                    Success = false,
                    Error = "Has not authorize privacy"
                });
                return;
            }

            if (callback != null)
                getInstallParamsCallbacks.Add(callback);

            if (!CallGetInstallParams())
            {
                callback?.Invoke(new Result
                {
                    Success = false,
                    Error = "Has not init"
                });
            }
        }
        
        public void CustomCall(string func, Action<Result> callback = null, Dictionary<string, object> param = null)
        {
            switch (func)
            {
                case "GetInstallParams":
                    GetInstallParams(callback);
                    break;
                default:
                    break;
            }
        }
    }
}
#endif