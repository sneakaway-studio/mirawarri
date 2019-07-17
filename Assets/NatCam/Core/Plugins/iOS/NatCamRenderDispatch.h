//
//  NatCamRenderDispatch.h
//  NatCam Render Pipeline
//
//  Created by Yusuf Olokoba on 2/26/17.
//  Copyright © 2017 Yusuf Olokoba. All rights reserved.
//

#import <Foundation/Foundation.h>
#include "IUnityInterface.h"
#include "IUnityGraphics.h"
#include "IUnityGraphicsMetal.h"

typedef void (^RenderDelegate) ();

@interface RenderDispatch : NSObject
+ (void) dispatch:(RenderDelegate) delegate;
@end

extern IUnityInterfaces* UnityInterfaces;
extern IUnityGraphics* UnityGraphics;
extern IUnityGraphicsMetal* UnityMetalGraphics;
