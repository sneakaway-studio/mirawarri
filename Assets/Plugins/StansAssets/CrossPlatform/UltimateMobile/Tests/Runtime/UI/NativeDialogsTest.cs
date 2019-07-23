using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using SA.CrossPlatform.UI;

namespace SA.CrossPlatform.Tests.UI
{
    /// <summary>
    /// Dialogs will not be tested in editor mode, since there is no way to close editor dialog via code.
    /// </summary>
	public class NativeDialogsTest {

		[UnityTest]
		public IEnumerator Message() {

            if(Application.isEditor) {
                yield break;
            }

            const string title = "Congrats";
			const string message = "Your account has been verified";
			var builder = new UM_NativeDialogBuilder(title, message);
			builder.SetPositiveButton("Okay", () => {
				Debug.Log("Okay button pressed");
			});

			var dialog = builder.Build();
			dialog.Show();
		
			yield return null;
			
			dialog.Hide();
		}
		
		[UnityTest]
		public IEnumerator Dialog() {

            if (Application.isEditor) {
                yield break;
            }

            const string title = "Save";
			const string message = "Do you want to save your progress?";
			var builder = new UM_NativeDialogBuilder(title, message);
			builder.SetPositiveButton("Yes", () => {
				Debug.Log("Yes button pressed");
			});

			builder.SetNegativeButton("No", () => {
				Debug.Log("No button pressed");
			});

			var dialog = builder.Build();
			dialog.Show();
		
			yield return null;
			
			dialog.Hide();
		}
		
		
		[UnityTest]
		public IEnumerator DestructiveDialog() {

            if (Application.isEditor) {
                yield break;
            }

        	const string title = "Save";
			const string message = "Do you want to save your progress?";
			var builder = new UM_NativeDialogBuilder(title, message);
			builder.SetPositiveButton("Cancel", () => {
				Debug.Log("Yes button pressed");
			});

			builder.SetDestructiveButton("Delete", () => {
				Debug.Log("Delete button pressed");
			});

			var dialog = builder.Build();
			dialog.Show();
		
			yield return null;
			
			dialog.Hide();
		}
		
		public IEnumerator ComplexDialog() {

            if (Application.isEditor) {
                yield break;
            }

            const string title = "Save";
			const string message = "Do you want to save your progress?";
			var builder = new UM_NativeDialogBuilder(title, message);
			builder.SetPositiveButton("Yes", () => {
				Debug.Log("Yes button pressed");
			});

			builder.SetNegativeButton("No", () => {
				Debug.Log("Yes button pressed");
			});

			builder.SetNeutralButton("Later", () => {
				Debug.Log("Later button pressed");
			});

			var dialog = builder.Build();
			dialog.Show();
		
			yield return null;
			
			dialog.Hide();
		}
		
	}
}

