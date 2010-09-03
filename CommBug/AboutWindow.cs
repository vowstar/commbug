using System;
namespace CommBug
{
	public partial class AboutWindow : Gtk.Dialog
	{
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}


		public AboutWindow ()
		{
			this.Build ();
			labelInfo.Text = labelInfo.Text.Replace ("{VERSION}", System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ());
			labelInfo.Text = labelInfo.Text.Replace ("{YEAR}", DateTime.Now.Year.ToString ());
			labelInfo.Text = labelInfo.Text.Replace ("{MONTH}", DateTime.Now.Month.ToString ());
			labelInfo.UseMarkup = true;
			labelAuthor.UseMarkup = true;
			labelTranslation.UseMarkup = true;
		}
	}
}

