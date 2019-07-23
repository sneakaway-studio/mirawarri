using SA.CrossPlatform.Social;
using SA.CrossPlatform.UI;
using SA.Foundation.Templates;
using SA.Foundation.Utility;
using UnityEngine;
using UnityEngine.UI;

public class UM_SharingExample : MonoBehaviour {

    [Header("Native Sharing")]
    [SerializeField] Button m_SystemSharingDialog  = null;
    [SerializeField] Button m_SendMailDialog = null;
    
    
    [Header("Social Media")]
    [SerializeField] Button m_Facebook = null;
    [SerializeField] Button m_Instagram = null;
    [SerializeField] Button m_Twitter = null;
    [SerializeField] Button m_Whatsapp = null;

    
    private void Start()
    {
        //Native Sharing
        m_SystemSharingDialog.onClick.AddListener(() =>
        {
            var client = UM_SocialService.SharingClient;
            client.SystemSharingDialog(MakeSharingBuilder(), PrintSharingResult);
        });
        
        m_SendMailDialog.onClick.AddListener(() =>
        {
            var client = UM_SocialService.SharingClient;
            var dialog = new UM_EmailDialogBuilder();

            dialog.SetSubject("Subject");
            dialog.SetText("Hello World!");
            dialog.SetUrl("https://stansassets.com/");

            //Juts generating simple red texture with 32x32 resolution
            var sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
            dialog.AddImage(sampleRedTexture);
            dialog.AddRecipient("support@stansassets.com");

            client.ShowSendMailDialog(dialog, PrintSharingResult);
        });
        
        
        //Sharing to Social Media
        m_Facebook.onClick.AddListener(() =>
        {
            var client = UM_SocialService.SharingClient;
            client.ShareToFacebook(MakeSharingBuilder(), PrintSharingResult);
        });
        
        
        m_Instagram.onClick.AddListener(() =>
        {
            var client = UM_SocialService.SharingClient;
            client.ShareToInstagram(MakeSharingBuilder(), PrintSharingResult);
        });
        
        
        m_Twitter.onClick.AddListener(() =>
        {
            var client = UM_SocialService.SharingClient;
            client.ShareToTwitter(MakeSharingBuilder(), PrintSharingResult);
        });
        
        m_Whatsapp.onClick.AddListener(() =>
        {
            var client = UM_SocialService.SharingClient;
            client.ShareToWhatsapp(MakeSharingBuilder(), PrintSharingResult);
        });
    }
    
    private UM_ShareDialogBuilder MakeSharingBuilder()
    {
        var builder = new UM_ShareDialogBuilder();
        builder.SetText("Hello world!");
        builder.SetUrl("https://stansassets.com/");

        //Juts generating simple red texture with 32x32 resolution
        var sampleRedTexture = SA_IconManager.GetIcon(Color.red, 32, 32);
        builder.AddImage(sampleRedTexture);

        return builder;
    }

    public static void PrintSharingResult(SA_Result result)
    {
        if(result.IsSucceeded) {
            UM_DialogsUtility.ShowMessage("Result", "Sharing Completed.");
            Debug.Log("Sharing Completed.");
        } else {
            UM_DialogsUtility.ShowMessage("Result", "Failed to share: " + result.Error.FullMessage);
            Debug.Log("Failed to share: " + result.Error.FullMessage);
        }
    }

}
