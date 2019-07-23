using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using SA.Android;
using SA.Android.Vending.Billing;
using SA.CrossPlatform.InApp;
using SA.Foundation.Templates;


namespace SA.CrossPlatform.Tests.InApp
{   
    public class InAppPurchasesTest
    {
        private TestTransactionObserver m_Observer;
        
        private class TestTransactionObserver : UM_iTransactionObserver
        {
            public event Action<UM_iTransaction> OnTransaction = delegate { };  
            public void OnTransactionUpdated(UM_iTransaction transaction)
            {
                PrintTransactionInfo(transaction);
                OnTransaction.Invoke(transaction);
            }

            public void OnRestoreTransactionsComplete(SA_Result result)
            {
               
            }
        }
        
        [OneTimeSetUp]
        public void SetUp()
        {
            var settings = AN_Settings.Instance;
            settings.RSAPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAonqY2kxgUKeAioN2tnMB2jtS1tBVwm0RHvsrFkDewHfzMGyBZvHsg9UN47H1MO6omXtNvsVuOnACV02MWIY16w7TPnttYTY7e2pULARafq7GwPuh9F7gLDdGluIoi/dJGjhaCTzvY6TpslI/FegJ/tDXVsNZh7urAxO1pWP4vrs412lANAjN8O6KF2dxF0VSThejyjzhyL0QWVtXtB6mJ9Ulsw16+0ndY4/Y4gL0BYSiJ4Qa+y7Ron6IXEGOnimixvGWasQQSKZHtEOLrh593ssp4a9PKMLQHWP7Pu2AYDmzhfR/ZkR1ZupKattjsviPnz5fTpsZ3oggSK+7IDBWQwIDAQAB";
            settings.InAppProducts.Clear();
            
            var p0 = new AN_Product(UM_InAppService.TEST_ITEM_PURCHASED, AN_ProductType.inapp);
            p0.Title = "Android Purchased";
            p0.IsConsumable = false;
            settings.InAppProducts.Add(p0);
            
            var p1 = new AN_Product(UM_InAppService.TEST_ITEM_UNAVAILABLE, AN_ProductType.inapp);
            p1.Title = "Android Unavailable";
            p1.IsConsumable = false;
            settings.InAppProducts.Add(p1);
            
            m_Observer = new TestTransactionObserver();
            UM_InAppService.Client.SetTransactionObserver(m_Observer);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            
        }

        [UnityTest, Order(1)]
        public IEnumerator  Connect()
        {
            var @lock = new CallbackLock();
            UM_InAppService.Client.Connect((connectionResult) => {
                @lock.Unlock();
                Assert.IsTrue(connectionResult.IsSucceeded);
            });

            yield return @lock.WaitToUnlock();
        }


        private CallbackLock m_transactionLock;

        [UnityTest, Order(2)]
        public IEnumerator SuccessfulPurchase()
        {
            if (Application.isEditor)
            {
                m_transactionLock = new CallbackLock();
                m_Observer.OnTransaction += OnSuccessfuTransaction;
                
                UM_InAppService.Client.AddPayment(UM_InAppService.TEST_ITEM_PURCHASED);
                yield return m_transactionLock.WaitToUnlock();
                m_Observer.OnTransaction -= OnSuccessfuTransaction;
            }

            yield return null;
        }

        private void OnSuccessfuTransaction(UM_iTransaction transaction)
        {
            Assert.IsTrue(transaction.State == UM_TransactionState.Purchased);
            m_transactionLock.Unlock();
        }

        [UnityTest, Order(3)] 
        public IEnumerator FailedPurchase()
        {
            //  if (Application.isEditor)
            //  {

            m_transactionLock = new CallbackLock();
            m_Observer.OnTransaction += OnFailedTransaction;
           
            UM_InAppService.Client.AddPayment(UM_InAppService.TEST_ITEM_UNAVAILABLE);
            yield return m_transactionLock.WaitToUnlock();
            m_Observer.OnTransaction -= OnFailedTransaction;

            // }

            yield return null;
        }

        private void OnFailedTransaction(UM_iTransaction transaction) {
            Assert.IsTrue(transaction.State == UM_TransactionState.Failed);
            m_transactionLock.Unlock();
        }

        private static void PrintTransactionInfo(UM_iTransaction transaction)
        {
            Debug.Log("transaction.Id: " + transaction.Id);
            Debug.Log("transaction.State: " + transaction.State);
            Debug.Log("transaction.ProductId: " + transaction.ProductId);
            Debug.Log("transaction.Timestamp: " + transaction.Timestamp);
        }
        
    }
}