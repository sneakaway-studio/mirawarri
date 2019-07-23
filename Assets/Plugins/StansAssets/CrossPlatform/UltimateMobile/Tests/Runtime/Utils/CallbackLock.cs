using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace SA.CrossPlatform.Tests
{
    public class CallbackLock
    {
        private bool m_IsLocked;
        private DateTime m_CreationTime;
            
        public CallbackLock()
        {
            m_IsLocked = true;
            m_CreationTime = DateTime.Now;
        }

        public void Unlock()
        {
            m_IsLocked = false;
        }

        public IEnumerator  WaitToUnlock()
        {
            while (m_IsLocked)
            {   
                yield return null; 
            }
        }


        /// <summary>
        /// This methods aren't used, since Unit test implementation
        /// will have own timouts
        /// </summary>
        private void CheckTimeout()
        {
            var timeDifference = DateTime.Now.Subtract(m_CreationTime);
            if (timeDifference.TotalSeconds > 5)
            {
               throw new TimeoutException();
            }
        }
    }
}