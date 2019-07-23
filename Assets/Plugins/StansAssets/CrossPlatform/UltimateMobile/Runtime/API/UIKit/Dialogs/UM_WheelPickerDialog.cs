using UnityEngine;
using System;
using System.Collections.Generic;
using SA.Android.App;
using SA.iOS.UIKit;

namespace SA.CrossPlatform.UI
{
    /// <summary>
    /// A simple dialog contaning an Wheel picker
    /// </summary>
    public class UM_WheelPickerDialog
    {
        [SerializeField] List<string> m_Values;

        /// <summary>
        /// Create a new wheel picker dialog for the specified values.
        /// </summary>
        /// <param name="values">list of the elements to choose from.</param>
        public UM_WheelPickerDialog(List<string> values)
        {
            this.m_Values = values;
        }


        /// <summary>
        /// Start of the dialog display in on screen.
        /// </summary>
        public void Show(Action<UM_WheelPickerResult> callback)
        {
            if(Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog(
                    "Not supported",
                    "The wheel picker view emulation is not supported inside the Editor.\n" +
                    "First value of the list will be returned as dialog result.",
                    "Okay");
                UM_WheelPickerResult result;
                if(m_Values != null && m_Values.Count > 0)
                {
                    result = new UM_WheelPickerResult(m_Values[0]);
                }
                else
                {
                    result = new UM_WheelPickerResult("Null");
                }
                callback.Invoke(result);
#endif          
            }
            else
            {
                switch(Application.platform)
                {
                    case RuntimePlatform.Android:
                        AN_WheelPickerDialog picker = new AN_WheelPickerDialog(m_Values);
                        picker.Show((pickerResult) =>
                        {
                            UM_WheelPickerResult result;
                            if(pickerResult.IsSucceeded)
                            {
                                var pickerValue = pickerResult.Value;
                                result = new UM_WheelPickerResult(pickerValue);
                            }
                            else
                            {
                                result = new UM_WheelPickerResult(pickerResult.Error);
                            }
                            callback.Invoke(result);
                        });
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        ISN_UIWheelPickerController pickerController = new ISN_UIWheelPickerController(m_Values);
                        pickerController.Show((pickerResult) =>
                        {
                            UM_WheelPickerResult result;
                            if(pickerResult.IsSucceeded) 
                            {       
                                var pickerValue = pickerResult.Value;
                                result = new UM_WheelPickerResult(pickerValue);
                            } 
                            else
                            {
                                result = new UM_WheelPickerResult(pickerResult.Error);
                            }
                            callback.Invoke(result);
                        });
                        break;
                }
            }
        }
    }
}