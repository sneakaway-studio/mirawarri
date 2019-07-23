#if UNITY_2018_1_OR_NEWER

using System.Collections;
using NUnit.Framework;
using SA.CrossPlatform.App;
using UnityEngine;
using UnityEngine.TestTools;

namespace SA.CrossPlatform.Tests.App
{
    /// <summary>
    /// Test will only run in editor, and will test Editor Fake API.
    /// Reason: It will require actions from user when tested on a real dvice, 
    /// and can't be perfromed automatically. 
    /// </summary>
    public class CameraTests
    {
        [UnityTest]
        public IEnumerator CamptueVideo() {
            yield return null;

            var @lock = new CallbackLock();
            if (Application.isEditor) {
                var cameraService = UM_Application.CameraService;
                cameraService.TakeVideo(1024, (result) => {
                    @lock.Unlock();
                    ValidateResult(result, UM_MediaType.Video);
                });
            }

            yield return @lock.WaitToUnlock();
        }

        [UnityTest]
        public IEnumerator CamptureImage() {
            yield return null;

            var @lock = new CallbackLock();
            if (Application.isEditor) {
                var cameraService = UM_Application.CameraService;
                cameraService.TakePicture(1024, (result) => {
                    @lock.Unlock();
                    ValidateResult(result, UM_MediaType.Image);
                });
            }

            yield return @lock.WaitToUnlock();
        }

        private void ValidateResult(UM_MediaResult result, UM_MediaType type) {
            Assert.IsTrue(result.IsSucceeded);
            Assert.IsNotNull(result.Media.Thumbnail);
            Assert.IsTrue(result.Media.Type == type);
        }

    }
}

#endif