using System;
using Gtk;

public static class GtkMessages
{
		public static void ShowMessage (String messageString)
		{
				Window parentWindow = new Window ("提示");
				MessageDialog messageDialog = new MessageDialog (parentWindow, 
				                                               DialogFlags.Modal,
				                                               MessageType.Info, 
				                                               ButtonsType.Close, messageString);
				messageDialog.Run ();
				messageDialog.Destroy ();
				parentWindow.Dispose ();
		}
}


