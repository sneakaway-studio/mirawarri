using System;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.StoreKit;

namespace SA.CrossPlatform.InApp
{
    [Serializable]
    public class UM_IOSProduct : UM_AbstractProduct<ISN_SKProduct>, UM_iProduct
    {

        public override void OnOverride(ISN_SKProduct productTemplate) {
            m_id = productTemplate.ProductIdentifier;
            m_price = productTemplate.LocalizedPrice;
            m_priceInMicros = productTemplate.PriceInMicros;
            
            if (!string.IsNullOrEmpty(productTemplate.PriceLocale.Identifier))
            {
                m_priceCurrencyCode = productTemplate.PriceLocale.CurrencyCode;
            }
            else
            {
                m_priceCurrencyCode = "USD";
            }
           
            m_title = productTemplate.LocalizedTitle;
            m_description = productTemplate.LocalizedDescription;
        }

    }
}