#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "ISN_Foundation.h"
#import "ISN_UICommunication.h"


static NSMutableDictionary* alerts = [[NSMutableDictionary alloc] init];
static  UIActivityIndicatorView *spinner;

extern "C" {
    
    
    void _ISN_UI_PresentUIAlertController(char * data) {
        
        
        [ISN_Logger LogNativeMethodInvoke:"_ISN_UI_PresentUIAlertController" data:data];
        
        NSError *jsonError;
        ISN_UIAlertController *request = [[ISN_UIAlertController alloc] initWithChar:data error:&jsonError];
        if (jsonError) {
            [ISN_Logger LogError:@"_ISN_UI_PresentUIAlertController JSON parsing error: %@", jsonError.description];
            return;
        }
        
        UIAlertController* alert = [UIAlertController alertControllerWithTitle:request.m_title  message:request.m_message  preferredStyle:request.m_preferredStyle];
        for(ISN_UIAlertAction* actionRequest in request.m_actions) {
            UIAlertAction* uiAction = [UIAlertAction actionWithTitle:actionRequest.m_title style:actionRequest.m_style handler:^(UIAlertAction * _Nonnull action) {
                
                ISN_UIAlertActionId *actionId = [[ISN_UIAlertActionId alloc] init];
                [actionId setM_alertId:request.m_id];
                [actionId setM_actionId:actionRequest.m_id];
                
                ISN_SendMessage(UNITY_UI_LISTENER, "OnUIAlertAction", [actionId toJSONString]);
            }];
            
            if(actionRequest.m_image != NULL && actionRequest.m_image.length > 0) {
                NSData *imageData = [[NSData alloc] initWithBase64EncodedString:actionRequest.m_image options:NSDataBase64DecodingIgnoreUnknownCharacters];
                UIImage *image = [[UIImage alloc] initWithData:imageData];
                [uiAction setValue:[image imageWithRenderingMode:UIImageRenderingModeAlwaysOriginal] forKey:@"image"];
                
                
            }
            
            
            [alert addAction:uiAction];
            
            //Those properties has to be set only on actions that already sitting inside the alert actions array
            if(!actionRequest.m_enabled) {
                uiAction.enabled = actionRequest.m_enabled;
            }
            
            if(actionRequest.m_preffered) {
                alert.preferredAction = uiAction;
            }
            
            
        }
        
        
        UIViewController *vc =  UnityGetGLViewController();
        [vc presentViewController:alert animated:YES completion:nil];
        
        NSString* key = [NSString stringWithFormat:@"%d",request.m_id];
        
        [alerts setObject:alert forKey:key];
    }
    
    void _ISN_UI_DismissUIAlertController(int alertId) {
        NSString* key = [NSString stringWithFormat:@"%d", alertId];
        UIAlertController* alert = [alerts objectForKey:key];
        if(alert != NULL) {
            [alert dismissViewControllerAnimated:true completion:^{}];
        }
    }
    
    
    
    void _ISN_UI_PreloaderLockScreen() {
        [[UIApplication sharedApplication] beginIgnoringInteractionEvents];
        
        if(spinner != nil) {
            return;
        }
        
        UIViewController *vc =  UnityGetGLViewController();
        
        #if !TARGET_OS_TV
        spinner = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhiteLarge];
        [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
        #endif
        
        NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
        if ([[vComp objectAtIndex:0] intValue] >= 8) {
            [spinner setFrame:CGRectMake(0,0, vc.view.frame.size.width, vc.view.frame.size.height)];
        }
        
        spinner.opaque = NO;
        spinner.backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
        
        [UIView animateWithDuration:0.8 animations:^{
            spinner.backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
        }];
        
        
        [vc.view addSubview:spinner];
        [spinner startAnimating];
    }
    
    void _ISN_UI_PreloaderUnlockScreen() {
        [[UIApplication sharedApplication] endIgnoringInteractionEvents];
        
        if(spinner != nil) {
            spinner.backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.5f];
            [UIView animateWithDuration:0.8 animations:^{
                spinner.backgroundColor = [UIColor colorWithWhite:0.0f alpha:0.0f];
                
            } completion:^(BOOL finished) {
                [spinner removeFromSuperview];
                spinner = nil;
            }];
        }
    }
    
    
}

