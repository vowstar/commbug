using System;
using Gtk;

namespace CommBug
{
	class MainClass
	{
		public static void Main (string[] args)
		{			
			if (!GLib.Thread.Supported)
				GLib.Thread.Init ();
			Application.Init ();
			Gdk.Threads.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Gdk.Threads.Enter ();
			try {
				Application.Run ();
			} finally {
				Gdk.Threads.Leave ();
			}	
		}
	}
}

