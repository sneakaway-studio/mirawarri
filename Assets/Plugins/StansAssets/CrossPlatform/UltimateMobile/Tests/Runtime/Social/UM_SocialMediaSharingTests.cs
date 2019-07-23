using System.Collections;
using NUnit.Framework;
using SA.CrossPlatform.Social;
using SA.Foundation.Utility;
using UnityEngine;
using UnityEngine.TestTools;


namespace SA.CrossPlatform.Tests.Social
{
    public class UM_SocialMediaSharingTests
    {
        [UnityTest]
        public IEnumerator ShareToFacebook() {

            var @lock = new CallbackLock();
            var client = UM_SocialService.SharingClient;
            var builder = new UM_ShareDialogBuilder();

            //Juts generating smaple red tuxture with 32x32 resolution
            Texture2D sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            builder.AddImage(sampleRedTexture);

            client.ShareToFacebook(builder, (result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }


        [UnityTest]
        public IEnumerator ShareToTwitter() {

            var @lock = new CallbackLock();
            var client = UM_SocialService.SharingClient;
            var builder = new UM_ShareDialogBuilder();

            //Juts generating smaple red tuxture with 32x32 resolution
            Texture2D sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            builder.AddImage(sampleRedTexture);

            client.ShareToTwitter(builder, (result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }

        [UnityTest]
        public IEnumerator ShareToInstagram() {

            var @lock = new CallbackLock();
            var client = UM_SocialService.SharingClient;
            var builder = new UM_ShareDialogBuilder();

            //Juts generating smaple red tuxture with 32x32 resolution
            Texture2D sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            builder.AddImage(sampleRedTexture);

            client.ShareToInstagram(builder, (result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }

        [UnityTest]
        public IEnumerator ShareToWhatsapp() {

            var @lock = new CallbackLock();
            var client = UM_SocialService.SharingClient;
            var builder = new UM_ShareDialogBuilder();

            //Juts generating smaple red tuxture with 32x32 resolution
            Texture2D sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            builder.AddImage(sampleRedTexture);

            client.ShareToWhatsapp(builder, (result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }
    }
}