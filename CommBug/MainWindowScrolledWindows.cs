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
using System.IO.Ports;
using Gtk;

public class ScrolledTextViewMover
{
		private int top;
		private int bottom;
		private int windowSize = 3600;
		private int stepSize = 96;
		// 平移窗口大小
		public int WindowSize {
				set {
						if (value > 0) {
								windowSize = value;
						} else {
								windowSize = 1;
						}
				}
				get {
						return windowSize;
				}
		}
		// 步进增量
		public int StepSize {
				set {
						if (value > 0) {
								stepSize = value;
						} else {
								stepSize = 1;
						}
				}
				get {
						return stepSize;
				}
		}

		public String Text = "";
		public Gtk.TextView TextView = null;

		public void Append (String str)
		{
				Text = Text + str;
		}

		public void Clear ()
		{
				if (TextView != null) {
						TextView.Buffer.Clear ();
				}
				Text = "";
				top = 0;
				bottom = 0;
		}
		// 向下平移文本
		public void MoveDown ()
		{
				int bottomToMove = bottom + StepSize;
				if (bottomToMove > Text.Length) {
						bottomToMove = Text.Length;
				}
				
				int topToMove = bottomToMove - WindowSize;
				if (topToMove < 0) {
						topToMove = 0;
				}

				int bottomAddBytes = bottomToMove - bottom;
				int topDeleteBytes = topToMove - top;
				if (bottomAddBytes > 0) {
						TextIter iter = TextView.Buffer.EndIter;
						//Console.WriteLine (Text);
						//Console.WriteLine ("MOVEDOWN:"+bottom.ToString () + ":" + Text.Substring (bottom, bottomAddBytes));
						TextView.Buffer.Insert (ref iter, Text.Substring (bottom, bottomAddBytes));
						//TextView.Buffer.Text += Text.Substring (bottom, bottomAddBytes);
				}
				if (topDeleteBytes > 0) {
						TextIter iterStart = TextView.Buffer.StartIter;
						TextIter iterEnd = TextView.Buffer.StartIter;
						iterEnd.ForwardChars (topDeleteBytes);
						TextView.Buffer.Delete (ref iterStart, ref iterEnd);
				}
				
				bottom = bottomToMove;
				top = topToMove;
		}
		// 向上平移文本
		public void MoveUp ()
		{
				int topToMove = top - StepSize;
				if (topToMove < 0) {
						topToMove = 0;
				}
				int bottomToMove = topToMove + WindowSize;
				if (bottomToMove > Text.Length) {
						bottomToMove = Text.Length;
				}

				int topAddBytes = top - topToMove;
				int bottomDeleteBytes = bottom - bottomToMove;

				if (topAddBytes > 0) {
						TextIter iter = TextView.Buffer.StartIter;
						//Console.WriteLine (Text);
						//Console.WriteLine ("MOVEUP:" + top.ToString () + ":" + Text.Substring (topToMove, topAddBytes));
						TextView.Buffer.Insert (ref iter, Text.Substring (topToMove, topAddBytes));
						//TextView.Buffer.Text += Text.Substring (bottom, bottomAddBytes);
				}
				if (bottomDeleteBytes > 0) {
						TextIter iterStart = TextView.Buffer.EndIter;
						TextIter iterEnd = TextView.Buffer.EndIter;
						iterStart.BackwardChars (bottomDeleteBytes);
						TextView.Buffer.Delete (ref iterStart, ref iterEnd);
				}
				top = topToMove;
				bottom = bottomToMove;
		}
}

public partial class MainWindow : Gtk.Window
{
		//     TextView DataCounter
		//		private int textViewDataBytesMaxCount = 1200;
		//		private int textviewTextOffset = 0;
		//		private int textviewHexOffset = 0;
		//		private int textviewDecOffset = 0;
		//		private String textviewTextString = "";
		//		private String textviewHexString = "";
		//		private String textviewDecString = "";
		private ScrolledTextViewMover moverText = new ScrolledTextViewMover ();
		private ScrolledTextViewMover moverHex = new ScrolledTextViewMover ();
		private ScrolledTextViewMover moverDec = new ScrolledTextViewMover ();
		private System.Timers.Timer scrolledTextViewTimer;

		private void processScrollEvent (ConvertMode convertMode, Boolean isSend)
		{
//				String textviewString = null;
				Gtk.ScrolledWindow scrolledWindow = null;
				Gtk.TextView textView = null;
				ScrolledTextViewMover mover = null;
//				int offset = 0;
				if (isSend) {
						switch (convertMode) {
						case ConvertMode.Text:
								scrolledWindow = GtkScrolledWindowTextS;
								textView = textviewTextS;
								break;
						case ConvertMode.Hex:
								scrolledWindow = GtkScrolledWindowHexS;
								textView = textviewHexS;
								break;
						case ConvertMode.Dec:
								scrolledWindow = GtkScrolledWindowDecS;
								textView = textviewDecS;
								break;
						}
				} else {
						switch (convertMode) {
						case ConvertMode.Text:
								scrolledWindow = GtkScrolledWindowText;
								textView = textviewText;
//								offset = textviewTextOffset;
//								textviewString = textviewTextString;
								mover = moverText;
								break;
						case ConvertMode.Hex:
								scrolledWindow = GtkScrolledWindowHex;
								textView = textviewHex;
//								offset = textviewHexOffset;
//								textviewString = textviewHexString;
								mover = moverHex;
								break;
						case ConvertMode.Dec:
								scrolledWindow = GtkScrolledWindowDec;
								textView = textviewDec;
//								offset = textviewDecOffset;
//								textviewString = textviewDecString;
								mover = moverDec;
								break;
						}
				}
				double vadjustmentLower = scrolledWindow.Vadjustment.Lower;
				double vadjustmentUpper = scrolledWindow.Vadjustment.Upper;
				double vadjustmentValue = scrolledWindow.Vadjustment.Value;
				double vadjustmentPageSize = scrolledWindow.Vadjustment.PageSize;
				double vadjustmentPosition = (vadjustmentValue - vadjustmentLower) / (vadjustmentUpper - vadjustmentPageSize - vadjustmentLower);
				//Console.WriteLine ("VADJ:" + vadjustmentPosition);
						
//				if (textviewString.Length < 3 * textViewDataBytesMaxCount) {
//						int forwardBytes = textviewString.Length - offset;
//						if (forwardBytes > 0) {						
//								TextIter iter = textView.Buffer.EndIter;
//								textView.Buffer.Insert (ref iter, textviewString.Substring (offset, forwardBytes));
//								offset += forwardBytes;
//								//						if (textviewText.Buffer.CharCount > 2 * textViewDataBytesMaxCount) {
//								//								TextIter iterStart = textviewText.Buffer.StartIter;
//								//								TextIter iterEnd = textviewText.Buffer.StartIter;
//								//								if (iterEnd.ForwardChars (textviewText.Buffer.CharCount - textViewDataBytesMaxCount)) {
//								//										textviewText.Buffer.Delete (ref iterStart, ref iterEnd);
//								//										textviewTextOffset += textviewText.Buffer.CharCount - textViewDataBytesMaxCount;
//								//								}
//								//						}
//						}
//
//				} else {
//						if (vadjustmentPosition > 0.9) {
//								Console.WriteLine ("Count:" + textView.Buffer.CharCount.ToString ());
//								int forwardBytes = textviewString.Length - offset;
//								if (forwardBytes > textViewDataBytesMaxCount) {
//										forwardBytes = textViewDataBytesMaxCount;
//								}
//								if (forwardBytes > 0) {						
//										TextIter iter = textView.Buffer.EndIter;
//										textView.Buffer.Insert (ref iter, textviewString.Substring (offset, forwardBytes));
//										offset += forwardBytes;
//										int deleteBytes = textView.Buffer.CharCount - 3 * textViewDataBytesMaxCount;
//										
//										if (deleteBytes > 0) {
//												Console.WriteLine ("Delete:" + deleteBytes.ToString ());
//												TextIter iterStart = textView.Buffer.StartIter;
//												TextIter iterEnd = textView.Buffer.StartIter;												
//												if (iterEnd.ForwardChars (deleteBytes)) {
//														textView.Buffer.Delete (ref iterStart, ref iterEnd);
//												}
//										}
//								}
//						}
//						if (vadjustmentPosition < 0.1) {
//								Console.WriteLine ("Delete:" + textView.Buffer.CharCount.ToString ());
//								if (offset >= textViewDataBytesMaxCount * 3) {
//										int offsetEnd = offset - textViewDataBytesMaxCount * 3;
//										int offsetStart = offsetEnd - textViewDataBytesMaxCount;
//										if (offsetStart < 0) {
//												offsetStart = 0;
//										}
//										int backwardBytes = offsetEnd - offsetStart;
//										if (backwardBytes > 0) {
//												TextIter iter = textView.Buffer.StartIter;
//												textView.Buffer.Insert (ref iter, textviewString.Substring (offsetStart, backwardBytes));
//												offset -= backwardBytes;
//										}
//								}
//						}
//				}
//				if (isSend) {
//						switch (convertMode) {
//						case ConvertMode.Text:
//								break;
//						case ConvertMode.Hex:
//								break;
//						case ConvertMode.Dec:
//								break;
//						}
//				} else {
//						switch (convertMode) {
//						case ConvertMode.Text:
//								break;
//						case ConvertMode.Hex:
//								//textviewHexOffset = offset;
//								break;
//						case ConvertMode.Dec:
//								//textviewDecOffset = offset;
//								break;
//						}
//				}

				if (checkbuttonAutoScrollReceive.Active) {
						if (scrolledWindow.VScrollbar.Visible == false || vadjustmentPosition > 0.98) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 10;
										mover.MoveDown ();
										TextIter iter = textView.Buffer.EndIter;
										textView.Buffer.CreateMark ("EndMark", iter, false);
										textView.ScrollToMark (textView.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
										textView.Buffer.DeleteMark ("EndMark");
								}		

						} else
						if (vadjustmentPosition < 0.02) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 10;
										mover.MoveUp ();
										TextIter iter = textView.Buffer.StartIter;
										textView.Buffer.CreateMark ("StartMark", iter, false);
										textView.ScrollToMark (textView.Buffer.CreateMark ("StartMark", iter, false), 0, false, 0, 0);
										textView.Buffer.DeleteMark ("StartMark");
								}		

						} else
						if (vadjustmentPosition > 0.7) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 100;
										mover.MoveDown ();
										TextIter iter = textView.Buffer.EndIter;
								}
						
						} else
						if (vadjustmentPosition < 0.3) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 100;
										mover.MoveUp ();
										TextIter iter = textView.Buffer.StartIter;
								}
						} else
						if (vadjustmentPosition > 0.55) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 300;
										mover.MoveDown ();
										TextIter iter = textView.Buffer.EndIter;
								}

						} else
						if (vadjustmentPosition < 0.45) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 300;
										mover.MoveUp ();
										TextIter iter = textView.Buffer.StartIter;
								}
						} 

				} else {
						if (vadjustmentPosition > 0.9) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 100;
										mover.MoveDown ();
								}
						} else
						if (vadjustmentPosition < 0.1) {
								if (mover != null) {
										mover.StepSize = mover.WindowSize / 100;
										mover.MoveUp ();
								}
						} 
				}
				
		}
}
