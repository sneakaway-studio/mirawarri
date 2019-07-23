using SA.CrossPlatform.InApp;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UM_PurchaseButton : MonoBehaviour
{
    [SerializeField] 
    private string m_ProductId = string.Empty;
    private Button m_Button;

    private void Awake()
    {
        UpdateButtonView();
    }

    public Button Button
    {
        get {
            if (m_Button == null)
            {
                m_Button = GetComponent<Button>();
            }

            return m_Button;
        }
    }
    
    public string ProductId
    {
        get { return m_ProductId; }
    }

    public void UpdateButtonView()
    {
        var buttonLabel = m_Button.GetComponentInChildren<Text>();
        var product = UM_InAppService.Client.GetProductById(m_ProductId);

        if (product != null)
        {
            m_Button.interactable = true;
            buttonLabel.text = product.Title + " - " + product.Price + "(" +  product.PriceCurrencyCode + ")";
        }
        else
        {
            m_Button.interactable = false;
            buttonLabel.text = "Product not available";
        }
    }
}
