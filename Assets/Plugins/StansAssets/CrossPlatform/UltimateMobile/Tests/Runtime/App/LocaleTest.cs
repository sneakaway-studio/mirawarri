#if UNITY_2018_1_OR_NEWER

using NUnit.Framework;
using SA.CrossPlatform.App;

namespace SA.CrossPlatform.Tests.App
{
	public class LocaleTest  {
	
		[Test]
		public void GetCurrentLocale() {
			var currentLocale = UM_Locale.GetCurrentLocale();
			Assert.False(string.IsNullOrEmpty(currentLocale.CountryCode));
			Assert.False(string.IsNullOrEmpty(currentLocale.CurrencyCode));
			Assert.False(string.IsNullOrEmpty(currentLocale.LanguageCode));
			Assert.False(string.IsNullOrEmpty(currentLocale.CurrencySymbol));
		}
	}
}

#endif