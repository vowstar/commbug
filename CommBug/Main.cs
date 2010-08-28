using System;
using Gtk;

namespace CommBug
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			if(!GLib.Thread.Supported)
				GLib.Thread.Init();
			Gdk.Threads.Init();
			// Gtk线程更新界面的无奈解决方法
			Gdk.Threads.Enter();
			MainWindow win = new MainWindow ();			
			win.Show ();
			Gdk.Threads.Leave();
			Application.Run ();
			
		}
	}
}

