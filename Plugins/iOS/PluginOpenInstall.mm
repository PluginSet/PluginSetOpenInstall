//
//  PluginTest.m
//  basketball-mobile
//
//  Created by macbook on 2021/10/9.
//

#import "OpenInstallSDK.h"

#import "PluginListener.h"
#import "PluginUtils.h"

#import "NSDictionary+SafeUseDictionary.h"

@interface PluginOpenInstall : NSObject<PluginListener, OpenInstallDelegate>
@property(nonatomic, strong) NSURL * notify_url;
@property(nonatomic, strong) NSUserActivity * notify_activity;
@property BOOL sdkInited;
@property BOOL hasUserRegister;
@end

REGISTER_PLUGIN_LISTENER(PluginOpenInstall)

@implementation PluginOpenInstall


- (void) onContinueUserActivity:(NSNotification *)notification
{
    NSDictionary* notifData = notification.userInfo;
    if (![notifData hasKey:@"activity"]) return;
    NSUserActivity * activity = notifData[@"activity"];
    if (self.sdkInited) {
        [OpenInstallSDK continueUserActivity:activity];
        self.notify_activity = nil;
    } else {
        self.notify_activity = activity;
    }
}

- (void) onOpenURL:(NSNotification *)notification
{
    NSDictionary* notifData = notification.userInfo;
    if (![notifData hasKey:@"url"]) return;
    NSURL * url = notifData[@"url"];
    if (self.sdkInited) {
        [OpenInstallSDK handLinkURL:url];
        self.notify_url = nil;
    } else {
        self.notify_url = url;
    }
}

- (void) onUserReigster
{
    self.hasUserRegister = YES;
    if (self.sdkInited) {
        [OpenInstallSDK reportRegister];
    }
}

- (void) initOpenInstall
{
    self.sdkInited = YES;
    [OpenInstallSDK initWithDelegate:self];
    if (self.notify_url) {
        [OpenInstallSDK handLinkURL:self.notify_url];
        self.notify_url = nil;
    }
    
    if (self.notify_activity) {
        [OpenInstallSDK continueUserActivity:self.notify_activity];
        self.notify_activity = nil;
    }
    
    if (self.hasUserRegister) {
        [OpenInstallSDK reportRegister];
    }
}

- (void) setAdid:(NSString*)adid
{
    [OpenInstallSDK initWithDelegate:self advertisingId:adid];
}

- (bool) getInstallParams:(NSString*)gameObjectName
{
    if (!self.sdkInited) return false;
    
    [[OpenInstallSDK defaultManager] getInstallParmsCompleted:^(OpeninstallData * _Nullable appData) {
        dispatch_async(dispatch_get_main_queue(), ^(){
            NSString *channelID = @"";
            NSString *datas = @"";
            if (appData && appData.data) {
                datas = [PluginUtils dictToJson:appData.data];
            }
            if (appData && appData.channelCode) {
                channelID = appData.channelCode;
            }
            NSDictionary *installDicResult = @{@"channelCode":channelID,@"bindData":datas};
            NSString *installJsonStr = [PluginUtils dictToJson:installDicResult];
            UnitySendMessage([gameObjectName UTF8String], "OnGetInstallParams", [installJsonStr UTF8String]);
        });
    }];

    return true;
}

- (void) reportEventPoint:(NSString *)eventName withValue:(long)vaule
{
    if (!self.sdkInited) return;
    [[OpenInstallSDK defaultManager] reportEffectPoint:eventName effectValue:vaule];
}


#if defined (__cplusplus)
extern "C" {
#endif

    void Plugin_OpenInstall_Init()
    {
        [[PluginOpenInstall GetInstance] initOpenInstall];
    }
    
    void Plugin_OpenInstall_ReportRegister()
    {
        [[PluginOpenInstall GetInstance] onUserReigster];
    }
    
    void Plugin_OpenInstall_SetUserAdid(const char * adid)
    {
        [[PluginOpenInstall GetInstance] setAdid:[PluginUtils createNSString:adid]];
    }
    
    bool Plugin_OpenInstall_GetInstallParams(const char * gameObjectName)
    {
        return [[PluginOpenInstall GetInstance] getInstallParams:[PluginUtils createNSString:gameObjectName]];
    }

    void Plugin_OpenInstall_ReportEffectPoint(const char * pointId, long pointValue)
    {
        [[PluginOpenInstall GetInstance] reportEventPoint:[PluginUtils createNSString:pointId] withValue:pointValue];
    }
    
#if defined (__cplusplus)
}
#endif

@end
