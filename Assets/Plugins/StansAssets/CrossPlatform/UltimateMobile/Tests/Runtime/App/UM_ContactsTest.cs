#if UNITY_2018_1_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SA.CrossPlatform.App;
using UnityEngine.TestTools;

namespace SA.CrossPlatform.Tests.App
{
    public class UM_ContactsTest
    {

        private List<UM_EditorContact> m_TestingContacts;

        [SetUp]
        public void SetUp() {
            m_TestingContacts = new List<UM_EditorContact>(UM_Settings.Instance.EditorTestingContacts);

            if(UM_Settings.Instance.EditorTestingContacts.Count == 0) {
                var contact = new UM_EditorContact("name", "phone", "email");
                UM_Settings.Instance.EditorTestingContacts.Add(contact);
            }
        }


        [TearDown]
        public void TearDown() {
            UM_Settings.Instance.EditorTestingContacts = m_TestingContacts;
        }

        [UnityTest]
        public IEnumerator RetrieveContacts() {

            var @lock = new CallbackLock();
            var client = UM_Application.ContactsService;
            client.Retrieve((result) => {
                @lock.Unlock();
                Assert.IsTrue(result.IsSucceeded);
                Assert.IsTrue(result.Contacts.Count > 0);

                var contact = result.Contacts[0];
                Assert.NotNull(contact.Name);
                Assert.NotNull(contact.Phone);
                Assert.NotNull(contact.Email);
            });

            yield return @lock.WaitToUnlock();
        }
    }
}

#endif