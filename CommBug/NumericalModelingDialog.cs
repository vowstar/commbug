// --------------------------------------------------------------------------------------
// 本软件是自由软件,使用的是GNU通用公共许可证,许可证应使用第二版本或您所选择的更新的版本.
// 发布本软件的目的是希望它能够在一定程度上帮到您.但我们并不为它提供任何形式的担保，
// 也无法保证它可以在特定用途中得到您希望的结果.请参看GNU GPL许可中的更多细节.
// 使用本软件或者与本软件相关的代码,文档,图标之前,您必须接受本软件的协议及许可.
// 您应该在获取本代码同时获得了GNU GPL协议的副本.
// 如果您没有获得GNU GPL协议的副本的话,请给自由软件基金会写信,地址是:
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 本软件及其代码的作者是黄锐,(Email:vowstar@gmail.com),Lanzhou University
// --------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
namespace CommBug
{

	public partial class NumericalModelingDialog : Gtk.Dialog
	{

		const int ImgWidth = 500;
		const int ImgHeight = 375;
		private byte[] Rx;
		long Start = 0;
		long End = 0;
		NumericalAnalysis.Element[] StoreData;
		public NumericalModelingDialog ()
		{
			this.Build ();
			using (Bitmap CoordinateBitmap = new Bitmap (ImgWidth, ImgHeight)) {
				using (Graphics g = Graphics.FromImage (CoordinateBitmap)) {
					g.Clear (Color.White);
					using (Font theFont = new Font ("Arial", 10, FontStyle.Bold, GraphicsUnit.Millimeter)) {
						using (SolidBrush theBrush = new SolidBrush (Color.Black)) {
							g.DrawString ("   No DATA...", theFont, theBrush, 0, ImgHeight / 2);
						}
					}
				}
				imageMain.Pixbuf = Gtk.Loaders.ImageLoader.LoadImage (CoordinateBitmap);
			}
			
		}
		public virtual void ProceedModeling (MemoryStream ReceiveStream)
		{
			Rx = ReceiveStream.GetBuffer ();
			this.ProceedModeling ();
			
		}
		protected virtual void ProceedModeling ()
		{
			Start = Convert.ToInt32 (spinbuttonStart.Text);
			End = Convert.ToInt32 (spinbuttonEnd.Text);
			if (End > Rx.Length)
				End = Rx.Length;
			Console.WriteLine ("{0}>>Total data length:{1}", this.ToString (), Rx.Length);
			Console.WriteLine ("{0}>>Range:{1}~{2}", this.ToString (), Start, End);
			
			if (Rx != null) {
				if (Rx.Length > 0 && End - Start > 1 && End - Start + 1 < Rx.Length) {
					double[] A = new double[End - Start + 1];
					StoreData = new NumericalAnalysis.Element[End - Start + 1];
					int i;
					for (i = 0; i < End - Start + 1; i++) {
						A[i] = Rx[Start + i];
						StoreData[i] = new NumericalAnalysis.Element (i, A[i], 1);
					}
					
					NumericalAnalysis.Graphic.Coordinate coordinate;
					coordinate = new NumericalAnalysis.Graphic.Coordinate (A, 0, 500, ImgWidth, ImgHeight);
					imageMain.Pixbuf = Gtk.Loaders.ImageLoader.LoadImage (coordinate.CoordinateBitmap);
				}
			} else
				Console.WriteLine ("{0}>>Initialization needed.", this.ToString ());
			
		}
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}

		protected virtual void OnButtonGetDataClicked (object sender, System.EventArgs e)
		{
			this.ProceedModeling ();
		}

		protected virtual void OnButtonModelingClicked (object sender, System.EventArgs e)
		{
			if (StoreData != null)
				if (StoreData.Length > 0 && End - Start > 1 && End - Start + 1 < Rx.Length) {
					int i;
					Console.WriteLine ("{0}>> NumericalAnalysis.Analysis.", this.ToString ());

					NumericalAnalysis.Analysis.Liner liner = new NumericalAnalysis.Analysis.Liner (StoreData);
					
					double[] A = new double[End - Start + 1];
					
					for (i = 0; i < End - Start + 1; i++) {
						A[i] = liner.f (i);
					}
					NumericalAnalysis.Graphic.Coordinate coordinate;
					
					coordinate = new NumericalAnalysis.Graphic.Coordinate (A, 0, 500, ImgWidth, ImgHeight);
					imageMain.Pixbuf = Gtk.Loaders.ImageLoader.LoadImage (coordinate.CoordinateBitmap);
				}
		}
	}
}

