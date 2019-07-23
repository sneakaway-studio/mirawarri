using System.Collections.Generic;
using SA.CrossPlatform.Social;
using SA.CrossPlatform.UI;
using SA.Foundation.Templates;
using UnityEngine;
using UnityEngine.UI;

using SA.GIF;
using SA.iOS.Social;

public class UM_GIF_Samples : MonoBehaviour
{

    [Header("Recorder")]
    [SerializeField] private Button m_StartRecord = null;
    [SerializeField] private Button m_StopRecord = null;
    [SerializeField] private Button m_SaveGif = null;
    
    [Header("Player")]
    [SerializeField] private Button m_PlayButton = null;
    //[SerializeField] private Button m_PauseButton = null;
    [SerializeField] private Image m_PlayerView = null;
    [SerializeField] private RawImage m_CameraView = null;
    [SerializeField] private Text m_StatusBarText = null;
    
    
    [Header("Share  Buttons")]
    [SerializeField] private Button m_SystemShareDialog = null;
    [SerializeField] private Button m_ShareWithFacebook = null;
    [SerializeField] private Button m_ShareWithTwitter = null;
    
    [Header("Recorder Settings")]
    public int Width = 320;
    public int Height = 200;
    
    public bool AutoAspect = true;
    public ThreadPriority WorkerPriority = ThreadPriority.BelowNormal;

    [Range(1, 30)]
    public int FramesPerSecond = 15;
    
    public float BufferSize = 3f;

    public int Repeat = 0;

    [Range(1, 100)]
    public int CompressionQuality = 15;

    public Camera Camera;
    
    [Header("Sharing Settings")]
    public string GiphyApiKey = "dc6zaTOxFJmzC";

    private SG_Recorder m_Recorder;
    private SG_Player m_Player;
    private string m_SharableUrl;

    private Dictionary<SG_Recorder.RecorderState, List<Button>> m_StateAvailability =
        new Dictionary<SG_Recorder.RecorderState, List<Button>>();
    
    private List<Button> m_SharingButtons = new List<Button>();
    
    // Start is called before the first frame update
    void Start()
    {
        MakeNewRecorder();
        m_StartRecord.onClick.AddListener(() =>
        {
            MakeNewRecorder();
            m_Recorder.Record();
        });
        
        m_StopRecord.onClick.AddListener(m_Recorder.Stop);
        m_SaveGif.onClick.AddListener(m_Recorder.Save);
        
        m_PlayButton.onClick.AddListener(m_Player.Play);
        AddFitter(m_CameraView.gameObject);
        AddFitter(m_PlayerView.gameObject);
        
        var client = UM_SocialService.SharingClient;
        m_SystemShareDialog.onClick.AddListener(() =>
        {
           client.SystemSharingDialog(GetSharingContent(), UM_SharingExample.PrintSharingResult);
        });
        
        m_ShareWithFacebook.onClick.AddListener(() =>
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                UM_DialogsUtility.ShowMessage("Text & Url Sharing is denied.", 
                    "You need to use Facebook SDK for sharing. As Facebook will not allow sharing through Intent");
            }
            else
            {
                client.ShareToFacebook(GetSharingContent(), UM_SharingExample.PrintSharingResult);
            }
        });
        
        m_ShareWithTwitter.onClick.AddListener(() =>
        {
            client.ShareToTwitter(GetSharingContent(), UM_SharingExample.PrintSharingResult);
        });

        FillStates();
        OnRecorderStateChanged(m_Recorder.State);
        SetSharingButtonsInteractable(false);
    }

    private void MakeNewRecorder()
    {
        m_Recorder = new SG_Recorder(Camera);
        m_Player = new SG_Player(m_Recorder, m_PlayerView);
        m_Recorder.OnFileSaved += OnFileSaved;
        m_Recorder.OnRecorderStateChanged += OnRecorderStateChanged;
        m_Recorder.Setup(AutoAspect, Width, Height, FramesPerSecond, BufferSize, Repeat, CompressionQuality);
    }
    
    private void FillStates()
    {
        
        m_SharingButtons = new List<Button>
        {
            m_SystemShareDialog,
            m_ShareWithFacebook,
            m_ShareWithTwitter,
        };
        
        m_StateAvailability.Add(SG_Recorder.RecorderState.Paused, new List<Button>
        {
            m_StartRecord
        });   
        
        m_StateAvailability.Add(SG_Recorder.RecorderState.Stopped, new List<Button>
        {
            m_StartRecord,
            m_SaveGif,
            m_PlayButton,
        });   
        
        m_StateAvailability.Add(SG_Recorder.RecorderState.Recording, new List<Button>
        {
            m_StopRecord
        });   
        
        m_StateAvailability.Add(SG_Recorder.RecorderState.PreProcessing, new List<Button>());   
    }

    private void SetSharingButtonsInteractable(bool interactable)
    {
        foreach (var button in m_SharingButtons)
        {
            button.interactable = interactable;
        }
    }
    
    private void OnRecorderStateChanged(SG_Recorder.RecorderState state)
    {
        foreach (var kvp in m_StateAvailability)
        {
            foreach (var button in kvp.Value)
            {
                button.interactable = false;
            }
        }

        if (m_StateAvailability.ContainsKey(state))
        {
            var buttons = m_StateAvailability[state];
            foreach (var button in buttons)
            {
                button.interactable = true;
            }
        }

        m_StatusBarText.text = "Gif Recorder Status: " + state;
    }

    private UM_ShareDialogBuilder GetSharingContent()
    {
        var builder = new UM_ShareDialogBuilder();
        builder.SetText("Hello world GIF");
        builder.SetUrl(m_SharableUrl);

        return builder;
    }

    private void OnFileSaved(int workerId, string path)
    {
        m_StatusBarText.text = "File Saved: " + path;
        var uploader = new SG_GiphyUploader(GiphyApiKey);
        uploader.Upload(path, (result) =>
        {
            if (result.IsSucceeded)
            {
                m_SharableUrl = result.ShareUrl;
                m_StatusBarText.text = "GIF uploaded url: " + result.ShareUrl;
                SetSharingButtonsInteractable(true);
            }
            else
            {
                m_StatusBarText.text = "Upload Failed: " + result.meta.msg;
                UM_DialogsUtility.ShowMessage("Upload Failed", result.meta.msg);
            }
        });
    }
    
    
    private void AddFitter(GameObject go) 
    {
        var fitter = go.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        var aspectRatio =  (float)Screen.width / (float)Screen.height;
        fitter.aspectRatio = aspectRatio;
    }

}
