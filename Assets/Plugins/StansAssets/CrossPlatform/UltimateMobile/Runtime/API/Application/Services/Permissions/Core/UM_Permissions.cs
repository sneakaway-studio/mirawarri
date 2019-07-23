namespace SA.CrossPlatform.App
{
	/// <summary>
	/// Main entry point for the Permissions Services APIs. 
	/// This class provides APIs and interfaces to access the permission functionality.
	/// </summary>
	public static class UM_Permissions
	{
		/// <summary>
		/// Contacts Permissions Client.
		/// Requires Contacts framework on IOS. 
		/// </summary>
		public static UM_IPermission Contacts {
			get { return new UM_ContactsPermission();}
		}
		
		/// <summary>
		/// Notifications Permissions Client.
		/// Requires UserNotifications framework on IOS.
		/// </summary>
		public static UM_IPermission Notifications {
			get { return new UM_NotificationsPermission();}
		}
		
		/// <summary>
		/// Camera Permissions Client.
		/// </summary>
		public static UM_IPermission Camera {
			get { return new UM_CameraPermission();}
		}
		
		/// <summary>
		/// Photo Gallery Permissions Client.
		/// Requires Photos framework on IOS. 
		/// </summary>
		public static UM_IPermission Photos {
			get { return new UM_PhotosPermission();}
		}
	}

}