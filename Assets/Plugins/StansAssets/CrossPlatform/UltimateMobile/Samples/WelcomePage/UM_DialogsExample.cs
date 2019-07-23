using System;
using UnityEngine;
using UnityEngine.UI;
using SA.Foundation.Async;
using SA.CrossPlatform.UI;
using System.Collections.Generic;

public class UM_DialogsExample : MonoBehaviour 
{

    [SerializeField] Button m_Message = null;
    [SerializeField] Button m_Dialog = null;
    [SerializeField] Button m_DestructiveDialog = null;
    [SerializeField] Button m_ComplexDialog = null;
    [SerializeField] Button m_Preloader = null;
    [SerializeField] Button m_RateUs = null;
    [SerializeField] Button m_Calendar = null;
    [SerializeField] Button m_WheelPicker = null;
    
    void Start() 
    {
        m_Message.onClick.AddListener(Message);
        m_Dialog.onClick.AddListener(Dialog);
        m_DestructiveDialog.onClick.AddListener(DestructiveDialog);
        m_ComplexDialog.onClick.AddListener(ComplexDialog);
        m_Preloader.onClick.AddListener(Preloader);
        m_RateUs.onClick.AddListener(RateUsDialog);
        m_Calendar.onClick.AddListener(PickDate);
        m_WheelPicker.onClick.AddListener(WheelPicker);
    }

    private void WheelPicker()
    {
        var values = new List<string>{"Test 1", "Test 2", "Test 3"};
        var picker = new UM_WheelPickerDialog(values);
        picker.Show(result => 
        {
            if(result.IsSucceeded)
            {
                Debug.Log("User picked - " + result.Value);
            }
            else
            {
                Debug.Log("Failed to pick value - " + result.Error.FullMessage);
            }
        });
    }

    private void PickDate() 
    {
        int year = DateTime.Now.Year;
        var picker = new UM_DatePickerDialog(year);
        picker.Show(result => 
        {
            if (result.IsSucceeded) 
            {
                Debug.Log("Date picked result.Year: " + result.Date.Year);
                Debug.Log("Date picked result.Month: " + result.Date.Month);
                Debug.Log("Date picked result.Day: " + result.Date.Day);
            } 
            else 
            {
                Debug.Log("Failed to pick a date: " + result.Error.FullMessage);
            }
        });
    }

    private void RateUsDialog() 
    {
        UM_ReviewController.RequestReview();
    }

    private void Preloader() 
    {
        UM_Preloader.LockScreen();
        SA_Coroutine.WaitForSeconds(2f, UM_Preloader.UnlockScreen);
    }

    private void Message() 
    {
        var title = "Congrats";
        var message = "Your account has been verified";
        var builder = new UM_NativeDialogBuilder(title, message);
        builder.SetPositiveButton("Okay", () => 
        {
            Debug.Log("Okay button pressed");
        });

        var dialog = builder.Build();
        dialog.Show();
        SA_Coroutine.WaitForSeconds(2f, dialog.Hide);
    }

    private void Dialog() 
    {
        var title = "Save";
        var message = "Do you want to save your progress?";

        var builder = new UM_NativeDialogBuilder(title, message);
        builder.SetPositiveButton("Yes", () => 
        {
            Debug.Log("Yes button pressed");
        });

        builder.SetNegativeButton("No", () => 
        {
            Debug.Log("No button pressed");
        });
        var dialog = builder.Build();
        dialog.Show();
    }

    private void DestructiveDialog() 
    {
        var title = "Confirmation ";
        var message = "Do you want to delete this item?";
        var builder = new UM_NativeDialogBuilder(title, message);
        builder.SetNegativeButton("Cancel", () => 
        {
            Debug.Log("Cancel button pressed");
        });

        builder.SetDestructiveButton("Delete", () => 
        {
            Debug.Log("Delete button pressed");
        });

        var dialog = builder.Build();
        dialog.Show();
    }

    private void ComplexDialog() 
    {
        var title = "Save";
        var message = "Do you want to save your progress?";
        var builder = new UM_NativeDialogBuilder(title, message);
        builder.SetPositiveButton("Yes", () => 
        {
            Debug.Log("Yes button pressed");
        });
        builder.SetNegativeButton("No", () => 
        {
            Debug.Log("No button pressed");
        });
        builder.SetNeutralButton("Later", () => 
        {
            Debug.Log("Later button pressed");
        });
        var dialog = builder.Build();
        dialog.Show();
    }
}
