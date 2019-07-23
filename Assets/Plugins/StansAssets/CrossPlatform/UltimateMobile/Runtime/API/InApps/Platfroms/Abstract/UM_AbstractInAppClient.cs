using System;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Templates;
using SA.CrossPlatform.Analytics;

namespace SA.CrossPlatform.InApp
{
    public abstract class UM_AbstractInAppClient 
    {
        private bool m_IsConnected = false;
        private bool m_IsConnectionInProgress = false;
        private event Action<SA_Result> m_OnConnect = delegate { };
        private Dictionary<string, UM_iProduct> m_Products = new Dictionary<string, UM_iProduct>();
        private bool m_IsObserverRegistered = false;
        protected UM_iTransactionObserver m_Observer;

        //--------------------------------------
        //  Abstract
        //--------------------------------------

        protected abstract void ConnectToService(Action<SA_Result> callback);
        protected abstract void ObserveTransactions();

        /// <summary>
        /// Will update products list based on information retried from server
        /// </summary>
        protected abstract Dictionary<string, UM_iProduct> GetServerProductsInfo();


        //--------------------------------------
        //  Public Methods
        //--------------------------------------

        public void Connect(Action<SA_Result> callback) 
        {
            if (m_IsConnected) 
            {
                callback.Invoke(new SA_Result());
                return;
            }

            m_OnConnect += callback;
            if (m_IsConnectionInProgress) { return; }
            m_IsConnectionInProgress = true;

            ConnectToService(result => 
            {
                if(result.IsSucceeded) 
                {
                    m_Products = GetServerProductsInfo();
                }

                m_IsConnected = true;
                m_IsConnectionInProgress = false;
                m_OnConnect.Invoke(result);

                //Checking if we should add an observer
                //In case user added it before service was connected
                if (m_Observer != null && !m_IsObserverRegistered) 
                {
                    m_IsObserverRegistered = true;
                    ObserveTransactions();
                }

                m_OnConnect = delegate { };
            });
        }

        public void SetTransactionObserver(UM_iTransactionObserver observer) 
        {
            if(m_Observer != null) 
            {
                Debug.LogWarning("UM_AbstractInAppClient::SetTransactionObserver you can only set one Transactions Observer");
                return;
            }

            m_Observer = observer;

            // Make sure we adding actual observer only when connect to the service. 
            // Otherwise we will wait for a successful connection 
            if(IsConnected) 
            {
                m_IsObserverRegistered = true;
                ObserveTransactions();
            } 
        }

        /// <summary>
        /// Gets the product by identifier.
        /// </summary>
        /// <param name="productIdentifier">Product identifier.</param>
        public UM_iProduct GetProductById(string productIdentifier)
        {
            UM_iProduct product;
            if (m_Products.TryGetValue(productIdentifier, out product))
            {
                return product;
            }
            
            return null;
        }

        //--------------------------------------
        //  Get / Set
        //--------------------------------------

        /// <summary>
        /// Returns <c>true</c> if we are currently connected to the store services. Otherwise <c>false</c>
        /// </summary>
        public bool IsConnected 
        {
            get {
                return m_IsConnected;
            }
        }

        /// <summary>
        /// A list of products, one product for each valid product identifier provided in the original init request.
        /// only valid to use when <see cref="IsConnected"/> is <c>true</c>
        /// </summary>
        public List<UM_iProduct> Products 
        {
            get 
            {
                return new List<UM_iProduct>(m_Products.Values);
            }
        }

        //--------------------------------------
        //  Protected Methods
        //--------------------------------------

        protected void UpdateTransaction(UM_iTransaction transaction) 
        {
            if (m_Observer == null) 
            {
                Debug.LogError("UpdateTransaction has been called before m_observer is set");
                return;
            }

            UM_AnalyticsInternal.OnTransactionUpdated(transaction);
            m_Observer.OnTransactionUpdated(transaction);
        }

        protected void SetRestoreTransactionsResult(SA_Result result) 
        {
            if (m_Observer == null) {
                Debug.LogError("SetRestoreTransactionsResult has been called before m_observer is set");
                return;
            }

            UM_AnalyticsInternal.OnRestoreTransactions();
            m_Observer.OnRestoreTransactionsComplete(result);
        }
    }
}