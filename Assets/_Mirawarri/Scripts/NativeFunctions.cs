using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.CrossPlatform.UI;

public class NativeFunctions : MonoBehaviour
{



    // show a simple "OK" dialog
    // documentation: https://unionassets.com/ultimate-mobile-pro/native-dialogs-722
    public void ShowDialog(string title, string message)
    {
        // string title = "Congrats";
        // string message = "Your account has been verified";
        var builder = new UM_NativeDialogBuilder(title, message);
        builder.SetPositiveButton("Okay", () =>
        {
            Debug.Log("Okay button pressed");
        });
        var dialog = builder.Build();
        dialog.Show();
    }




}
