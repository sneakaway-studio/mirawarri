//
//  NatCamRenderDispatch.mm
//  NatCam Render Pipeline
//
//  Created by Yusuf Olokoba on 2/26/17.
//  Copyright © 2017 Yusuf Olokoba. All rights reserved.
//

#import "NatCamRenderDispatch.h"
#import "UnityAppController.h"

static RenderDispatch* sharedInstance;
IUnityInterfaces* UnityInterfaces;
IUnityGraphics* UnityGraphics;
IUnityGraphicsMetal* UnityMetalGraphics;

@interface RenderDispatch ()
@property NSMutableArray *queue;
@end

@implementation RenderDispatch

@synthesize queue;

+ (void) initialize {
    sharedInstance = [[RenderDispatch alloc] init];
    sharedInstance.queue = [[NSMutableArray alloc] init];
}

+ (void) dispatch:(RenderDelegate) delegate {
    @synchronized (sharedInstance) {
        [sharedInstance.queue addObject:delegate];
    }
}

- (void) invoke {
    NSArray* executing;
    @synchronized (self) {
        executing = [NSArray arrayWithArray:queue];
        [queue removeAllObjects];
    }
    for (int i = 0; i < executing.count; i++) ((RenderDelegate)executing[i])();
}
@end


#pragma mark --Unity Interop--

static void UNITY_INTERFACE_API OnGraphicsEvent (UnityGfxDeviceEventType eventType) {}

static void UNITY_INTERFACE_API OnRenderDispatch (int eventID) {
    [sharedInstance invoke];
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API OnLoadNatCam (IUnityInterfaces* unityInterfaces) {
    UnityInterfaces = unityInterfaces;
    UnityGraphics = unityInterfaces->Get<IUnityGraphics>();
    UnityMetalGraphics = unityInterfaces->Get<IUnityGraphicsMetal>();
    UnityGraphics->RegisterDeviceEventCallback(OnGraphicsEvent);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API OnUnloadNatCam () {
    UnityGraphics->UnregisterDeviceEventCallback(OnGraphicsEvent);
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API NatCamRenderDelegate () {
    return OnRenderDispatch;
}

@interface NatCamRenderDispatchAppController : UnityAppController {}
- (void) shouldAttachRenderDelegate;
@end

@implementation NatCamRenderDispatchAppController
- (void) shouldAttachRenderDelegate {
    UnityRegisterRenderingPluginV5(&OnLoadNatCam, &OnUnloadNatCam);
}
@end
IMPL_APP_CONTROLLER_SUBCLASS(NatCamRenderDispatchAppController)
