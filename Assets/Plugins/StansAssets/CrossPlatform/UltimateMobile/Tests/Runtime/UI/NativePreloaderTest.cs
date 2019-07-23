using System.Collections;
using SA.CrossPlatform.UI;
using UnityEngine.TestTools;

namespace SA.CrossPlatform.Tests.UI
{
	public class NativePreloaderTest
	{
		[UnityTest]
		public IEnumerator LockScreen()
		{
			UM_Preloader.LockScreen();
			yield return null;
			UM_Preloader.UnlockScreen();
		}
	}
}