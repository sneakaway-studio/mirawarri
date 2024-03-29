////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
//////////////////////////////////////////////////////////////////////////////// 
using System;
using UnityEngine;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.AVFoundation.Internal
{
    internal interface ISN_iAVAPI
    {

        //--------------------------------------
        // ISN_AVCaptureDevice
        //--------------------------------------

        ISN_AVAuthorizationStatus GetAuthorizationStatus(ISN_AVMediaType type);
        void RequestAccess(ISN_AVMediaType type, Action<ISN_AVAuthorizationStatus> callback);

        //--------------------------------------
        // ISN_AVAssetImageGenerator
        //--------------------------------------

        Texture2D CopyCGImageAtTime(string movieUrl, double seconds);

        //--------------------------------------
        // ISN_AVAudioSession
        //--------------------------------------

        SA_iEvent<ISN_AVAudioSessionRouteChangeReason> OnAudioSessionRouteChange { get; }
        ISN_AVAudioSessionCategory AudioSessionCategory { get; }
        SA_Result AudioSessionSetCategory(ISN_AVAudioSessionCategory category);
        SA_Result AudioSessionSetActive(bool isActive);




        int AudioSessionGetRecordPermission();
        void AudioSessionRequestRecordPermission(Action<bool> callback);
    }
}
