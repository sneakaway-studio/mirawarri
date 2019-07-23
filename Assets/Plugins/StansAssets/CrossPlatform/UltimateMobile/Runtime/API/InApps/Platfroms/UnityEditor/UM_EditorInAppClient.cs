using System;
using System.Collections.Generic;
using SA.iOS;
using SA.Android;
using SA.CrossPlatform.UI;
using SA.Foundation.Templates;
using UnityEngine;
using UnityEngine.Assertions;

namespace SA.CrossPlatform.InApp
{
    internal class TransactionsList
    {
        public List<string> productIds = new List<string>();
    }
    
    internal class UM_EditorInAppClient : UM_AbstractInAppClient, UM_iInAppClient
    {
        private const string k_TransactionsKey = "um_editor_inapp_transactions";

        private TransactionsList m_ActiveTransactions;
        
        
        //--------------------------------------
        //  UM_AbstractInAppClient
        //--------------------------------------

        protected override void ConnectToService(Action<SA_Result> callback) {
            UM_EditorAPIEmulator.WaitForNetwork(() => 
            {
                callback.Invoke(new SA_Result());
            });
        }

        protected override Dictionary<string, UM_iProduct> GetServerProductsInfo() 
        {

            var products = new Dictionary<string, UM_iProduct>();
#if UNITY_EDITOR

            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget) {
                case UnityEditor.BuildTarget.Android:
                    foreach (var product in AN_Settings.Instance.InAppProducts) {
                        UM_AndroidProduct p = new UM_AndroidProduct();
                        p.Override(product);
                        products.Add(p.Id, p);
                    }
                    break;
                default:
                    foreach (var product in ISN_Settings.Instance.InAppProducts) {
                        UM_IOSProduct p = new UM_IOSProduct();
                        p.Override(product);
                        products.Add(p.Id, p);
                    }
                    break;
            }
#endif

            return products;
        }

        protected override void ObserveTransactions() {
            var transactionsList = GetTransactionsList();
            foreach (var productId in transactionsList.productIds)
            {
                UM_EditorAPIEmulator.WaitForNetwork(() =>
                {
                    var transaction = new UM_EditorTransaction(productId, UM_TransactionState.Purchased);
                    UpdateTransaction(transaction);
                });
            }
        }

        //--------------------------------------
        //  UM_iInAppClient
        //--------------------------------------

        public void AddPayment(string productId)
        {
            var transactionsList = GetTransactionsList();
            if (transactionsList.productIds.Contains(productId))
            {
                UM_DialogsUtility.ShowMessage("Restored", "Product with id " + productId + " has been already purchased.");
                var transaction = new UM_EditorTransaction(productId, UM_TransactionState.Purchased);
                UpdateTransaction(transaction);
                return;
            }
            
            
            AddPendingTransaction(productId);
            UM_EditorAPIEmulator.WaitForNetwork(() =>
            {
                UM_iTransaction transaction;
                if (productId.Equals(UM_InAppService.TEST_ITEM_UNAVAILABLE))
                {
                    transaction = new UM_EditorTransaction(productId, UM_TransactionState.Failed);
                }
                else
                {
                    transaction = new UM_EditorTransaction(productId, UM_TransactionState.Purchased);
                }
               
                UpdateTransaction(transaction);
            });
        }

        public void FinishTransaction(UM_iTransaction transaction) {
            Assert.IsNotNull(transaction);
            var transactionsList = GetTransactionsList();
            transactionsList.productIds.Remove(transaction.ProductId);
            SaveTransactionsList(transactionsList);
        }

        public void RestoreCompletedTransactions() {
           if(UM_Settings.Instance.TestRestoreProducts.Count > 0) {
                foreach(var productsId in UM_Settings.Instance.TestRestoreProducts) {
                    var product = GetProductById(productsId);
                    if(product != null && product.Type == UM_ProductType.NonConsumable) {
                        var transaction = new UM_EditorTransaction(product.Id, UM_TransactionState.Restored);
                        UpdateTransaction(transaction);
                    }
                }
           } else {
                foreach(var product in Products) {
                    if(product.Type == UM_ProductType.NonConsumable) {
                        var transaction = new UM_EditorTransaction(product.Id, UM_TransactionState.Restored);
                        UpdateTransaction(transaction);
                    }
                }
           }
        }

        private void AddPendingTransaction(string productId)
        {
            var transactionsList = GetTransactionsList();
            transactionsList.productIds.Add(productId);
            SaveTransactionsList(transactionsList);
        }

        private TransactionsList GetTransactionsList()
        {
            if (PlayerPrefs.HasKey(k_TransactionsKey))
            {
                return JsonUtility.FromJson<TransactionsList>(PlayerPrefs.GetString(k_TransactionsKey));
            }
            
            return  new TransactionsList();
        }

        public void SaveTransactionsList(TransactionsList transactionsList)
        {
            PlayerPrefs.SetString(k_TransactionsKey, JsonUtility.ToJson(transactionsList));
        }

    }
}