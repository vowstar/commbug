using System;
namespace CommBug
{
	public partial class NumericalModelingDialog : Gtk.Dialog
	{
		public NumericalModelingDialog ()
		{
			this.Build ();
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}
		
	}
}

