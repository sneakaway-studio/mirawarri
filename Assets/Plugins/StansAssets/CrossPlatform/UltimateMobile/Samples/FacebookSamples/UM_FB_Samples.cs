using SA.CrossPlatform.UI;
using SA.Facebook;
using UnityEngine;
using UnityEngine.UI;

public class UM_FB_Samples : MonoBehaviour
{

    [Header("Share  Buttons"), SerializeField]
    private Button m_LogIn = null;


    private void Start()
    {
        m_LogIn.onClick.AddListener(() =>
        {
            m_LogIn.interactable = false;
            SA_FB.Login(result =>
            {
                if (result.IsSucceeded)
                {
                    
                }
                else
                {
                    m_LogIn.interactable = true;
                    UM_DialogsUtility.ShowMessage("Login Failed", result.Error.FullMessage);
                }
            });
        });
    }
}
