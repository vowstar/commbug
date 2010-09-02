using System;
namespace CommBug
{
	public partial class AboutWindow : Gtk.Dialog
	{
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			
		}
		
		
		public AboutWindow ()
		{
			this.Build ();
		}
	}
}

