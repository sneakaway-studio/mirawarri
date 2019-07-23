using UnityEngine;
using System;
using SA.Android.App.Utils;
using SA.iOS.Foundation;

namespace SA.CrossPlatform.App
{
	/// <summary>
	/// Information about linguistic, cultural, and technological conventions 
	/// for use in formatting data for presentation.
	/// </summary>
	[Serializable]
	public class UM_Locale  : AN_Locale
	{

		/// <summary>
		/// Returns The locale is formed from the settings for the current user’s chosen system locale 
		/// overlaid with any custom settings the user has specified.
		/// </summary>
		public static UM_Locale GetCurrentLocale()
		{
			var un_locale = new UM_Locale();
			
			switch (Application.platform)
			{
				case RuntimePlatform.Android:
					var locale = GetDefault();
					un_locale.m_CountryCode = locale.CountryCode;
					un_locale.m_LanguageCode = locale.LanguageCode;
					un_locale.m_CurrencySymbol = locale.CurrencySymbol;
					un_locale.m_CurrencyCode = locale.CurrencyCode;
					break;
				case RuntimePlatform.IPhonePlayer:
					var isn_locale = ISN_NSLocale.CurrentLocale;
					un_locale.m_CountryCode = isn_locale.CountryCode;
					un_locale.m_LanguageCode = isn_locale.LanguageCode;
					un_locale.m_CurrencySymbol = isn_locale.CurrencySymbol;
					un_locale.m_CurrencyCode = isn_locale.CurrencyCode;
					break;
				default:
					un_locale.m_CountryCode = "US";
					un_locale.m_LanguageCode = "ENG";
					un_locale.m_CurrencySymbol = "$";
					un_locale.m_CurrencyCode = "USD";
					break;
			}

			return un_locale;
		}
	}
}