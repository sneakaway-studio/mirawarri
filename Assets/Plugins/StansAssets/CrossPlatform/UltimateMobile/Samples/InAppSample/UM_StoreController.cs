using System.Collections.Generic;
using SA.CrossPlatform.InApp;
using SA.CrossPlatform.UI;
using UnityEngine;
using UnityEngine.UI;

public class UM_StoreController : MonoBehaviour
{
    [SerializeField, Header("Buttons")] 
    private Button m_ConnectButton = null;
    [SerializeField]
    private Button m_ClearPlayerPrefs = null;
    [SerializeField]
    private Button m_RestorePurchases = null;
    [SerializeField]
    private  List<UM_PurchaseButton> m_PurchaseButtons = null;

    [SerializeField, Header("Status Bar")] 
    private Text m_CoinsBar = null;
    [SerializeField]
    private Text m_GoldStatusBar = null;

    private void Awake()
    {
        UpdateStatusBar();
        var observer = new UM_TransactionObserverExample();
        observer.OnProductUnlock += UpdateStatusBar;
        
        UM_InAppService.Client.SetTransactionObserver(observer);
        
        m_ConnectButton.onClick.AddListener(() =>
        {
            m_ConnectButton.interactable = false;
            UM_InAppService.Client.Connect(result =>
            {
                if (result.IsSucceeded)
                {
                    foreach (var purchaseButton in m_PurchaseButtons)
                    {
                        purchaseButton.UpdateButtonView();
                    }
                }
                else
                {
                    m_ConnectButton.interactable = true;
                    UM_DialogsUtility.ShowMessage("Error", result.Error.FullMessage);
                }
            });
        });
        
        m_ClearPlayerPrefs.onClick.AddListener(() =>
        {
            UM_RewardManager.Reset();
            UpdateStatusBar();
        });
        
        
        m_RestorePurchases.onClick.AddListener(UM_InAppService.Client.RestoreCompletedTransactions);
        m_RestorePurchases.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer ||
                                                Application.isEditor);

        foreach (var purchaseButton in m_PurchaseButtons)
        {
            purchaseButton.Button.onClick.AddListener(() =>
            {
                UM_InAppService.Client.AddPayment(purchaseButton.ProductId);
            });
        }
    }

    private void UpdateStatusBar()
    {
        m_CoinsBar.text = "Coins: " + UM_RewardManager.Coins;
        m_GoldStatusBar.text = "Gold: " + UM_RewardManager.HasGoldStatus;
    }
}