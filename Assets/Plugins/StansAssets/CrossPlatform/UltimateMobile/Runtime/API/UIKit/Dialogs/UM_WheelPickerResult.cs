using UnityEngine;
using System;
using SA.Foundation.Templates;

namespace SA.CrossPlatform.UI
{
    /// <summary>
    /// Object that contatins a result of picked value from wheel picker
    /// using the <see cref="UM_WheelPickerDialog"/>
    /// </summary>

    public class UM_WheelPickerResult: SA_Result
    {
        private string m_Value;

        internal UM_WheelPickerResult(string value): base()
        {
            m_Value = value;
        }

        internal UM_WheelPickerResult(SA_Error error): base(error) {} 

        /// <summary>
        /// User picker value.
        /// </summary>
        public string Value
        {
            get 
            {
                return m_Value;
            }
        }
    }
}