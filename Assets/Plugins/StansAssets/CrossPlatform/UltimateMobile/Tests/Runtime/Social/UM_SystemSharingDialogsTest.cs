using UnityEngine;
using System.Collections;
using NUnit.Framework;
using SA.CrossPlatform.Social;
using UnityEngine.TestTools;
using SA.Foundation.Utility;

namespace SA.CrossPlatform.Tests.Social
{
    public class UM_SystemSharingDialogsTest
    {
        [UnityTest]
        public IEnumerator SystemShareDialog() {

            var @lock = new CallbackLock();
            var client = UM_SocialService.SharingClient;
            var builder = new UM_ShareDialogBuilder();
            builder.SetText("Hello world!");
            builder.SetUrl("https://stansassets.com/");

            //Juts generating smaple red tuxture with 32x32 resolution
            Texture2D sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            builder.AddImage(sampleRedTexture);

            client.SystemSharingDialog(builder, (result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }

        [UnityTest]
        public IEnumerator MailDialog() {

            var @lock = new CallbackLock();
            var client = UM_SocialService.SharingClient;
            var dialog = new UM_EmailDialogBuilder();

            dialog.SetSubject("Subject");
            dialog.SetText("Hello World!");
            dialog.SetUrl("https://stansassets.com/");

            //Juts generating smaple red tuxture with 32x32 resolution
            Texture2D sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            dialog.AddImage(sampleRedTexture);
            dialog.AddRecipient("support@stansassets.com");

            client.ShowSendMailDialog(dialog, (result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }
    }
}