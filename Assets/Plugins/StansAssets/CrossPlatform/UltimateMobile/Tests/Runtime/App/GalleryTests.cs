#if UNITY_2018_1_OR_NEWER

using System.Collections;
using NUnit.Framework;
using SA.CrossPlatform.App;
using SA.CrossPlatform.Tests;
using SA.Foundation.Utility;
using UnityEngine;
using UnityEngine.TestTools;

namespace SA.CrossPlatform.Tests.App
{
    /// <summary>
    /// Test will only run in editor, and will test Editor Fake API.
    /// Reason: It will require actions from user when tested on a real dvice, 
    /// and can't be perfromed automatically. 
    /// </summary>
    public class GalleryTests
    {

        [UnityTest]
        public IEnumerator SaveScreenshot() {
            yield return null;

            if (Application.isEditor) {
                var @lock = new CallbackLock();
                var gallery = UM_Application.GalleryService;
                gallery.SaveScreenshot("example_scene.png", (result) => {
                    Assert.IsTrue(result.IsSucceeded);
                    @lock.Unlock();
                });

                yield return @lock.WaitToUnlock();
            }
        }

        [UnityTest]
        public IEnumerator SaveImage() {
            yield return null;

            if (Application.isEditor) {
                var @lock = new CallbackLock();
                var gallery = UM_Application.GalleryService;
                var sampleBlackTexture = SA_IconManager.GetIcon(Color.black, 32, 32);
                gallery.SaveImage(sampleBlackTexture, "sample_black_image.png", (result) => {
                    Assert.IsTrue(result.IsSucceeded);
                    @lock.Unlock();
                });

                yield return @lock.WaitToUnlock();

            }
        }

        [UnityTest]
        public IEnumerator PickVideo() {
            yield return null;

            var @lock = new CallbackLock();
            if (Application.isEditor) {
                var gallery = UM_Application.GalleryService;
                gallery.PickVideo(1024, (result) => {
                    @lock.Unlock();
                    ValidateResult(result, UM_MediaType.Video);
                });
            }

            yield return @lock.WaitToUnlock();
        }


        [UnityTest]
        public IEnumerator PickImage() {
            yield return null;

            var @lock = new CallbackLock();
            if (Application.isEditor) {
                var gallery = UM_Application.GalleryService;
                gallery.PickImage(1024, (result) => {
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