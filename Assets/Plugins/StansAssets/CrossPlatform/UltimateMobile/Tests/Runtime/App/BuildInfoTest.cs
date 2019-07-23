#if UNITY_2018_1_OR_NEWER

using NUnit.Framework;
using SA.CrossPlatform.App;

namespace SA.CrossPlatform.Tests.App
{
	public class BuildInfoTest  {
	
		[Test]
		public void UM_iBuildInfo() {
			var buildInfo = UM_Build.Info;
			Assert.False(string.IsNullOrEmpty(buildInfo.Version));
			Assert.False(string.IsNullOrEmpty(buildInfo.Identifier));
		}
	}
}

#endif