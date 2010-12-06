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
/// <summary>
/// 主界面
/// </summary>
public partial class MainWindow : Gtk.Window
{
	#region Sub windows
	private CommBug.AboutDialog aboutDialog;
	private CommBug.PreferenceDialog preferenceDialog;
	private CommBug.NumericalModelingDialog numericalModelingDialog;
	#endregion
	#region 变量与类定义
	public enum ConvertMode
	{
		Text,
		Hex,
		Dec
	}
	public string SplitString = "\t";
	public ConvertMode SendMode = ConvertMode.Text;
	public ConvertMode NowSendMode = ConvertMode.Text;
	public SerialPortEx MyPort;
	public System.IO.MemoryStream ReceiveStream = new System.IO.MemoryStream ();
	//Receive
	public System.IO.MemoryStream SendStream = new System.IO.MemoryStream ();
	//Transmit 
	private System.Timers.Timer SendTimer;
	private System.Timers.Timer PortNameTimer;
	private string portName = "";
	private int baudRate = 1200;
	private Parity parity = Parity.None;
	private int dataBits = 0;
	private StopBits stopBits = StopBits.One;


	private int portCount = 0;
	private ListStore portNameModel = new ListStore (typeof(string));

	#endregion
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		// Let scroll disappear
		// textviewTextSL.SetScrollAdjustments (GtkScrolledWindowTextSL.Hadjustment, GtkScrolledWindowTextS.Vadjustment);
		
		// GtkScrolledWindowTextSL.Vadjustment.Lower=0;
		// GtkScrolledWindowTextSL.Vadjustment.Value=0;
		// GtkScrolledWindowTextSL.Vadjustment.Upper=0;
		
		textviewSend.Buffer.Changed += HandleTextviewSendBufferChanged;
		#region 初始化串口名称
		// 初始化串口名称与comboboxentry
		comboboxentryPortName.Model = portNameModel;
		InitializationPortName ();
		SettingsSynchronization ();
		
		MyPort = new SerialPortEx (portName, baudRate, parity, dataBits, stopBits);
		MyPort.DataReceived += new SerialDataReceivedEventHandler (HandleMyPortDataReceived);
		Console.WriteLine ("Port initializated");
		#endregion
		#region 初始化发送模式
		// 初始化发送模式
		switch (SendMode) {
		case ConvertMode.Text:
			radiobuttonText.Active = true;
			break;
		case ConvertMode.Hex:
			radiobuttonHex.Active = true;
			break;
		case ConvertMode.Dec:
			radiobuttonDec.Active = true;
			break;
		}
		#endregion
		#region 初始化发送定时器
		// 初始化发送定时器
		SendTimer = new System.Timers.Timer ();
		SendTimer.Elapsed += HandleSendTimerElapsed;
		SendTimer.Interval = Convert.ToDouble (spinbuttonInterval.Text);
		SendTimer.AutoReset = true;
		SendTimer.Enabled = false;
		PortNameTimer = new System.Timers.Timer ();
		PortNameTimer.Elapsed += HandlePortNameTimerElapsed;
		PortNameTimer.Interval = 700;
		PortNameTimer.AutoReset = true;
		PortNameTimer.Enabled = true;
		// 自动复位定时器，一直能触发
		#endregion
		
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		PortNameTimer.Enabled = false;
		PortNameTimer.Dispose ();
		SendTimer.Enabled = false;
		SendTimer.Dispose ();
		MyPort.Dispose ();
		//System.Diagnostics.Process.GetCurrentProcess().Kill();
		Application.Quit ();
		a.RetVal = true;
	}

	void HandleTextviewSendBufferChanged (object sender, EventArgs e)
	{
		if (NowSendMode != ConvertMode.Text) {
			textviewSend.Buffer.Changed -= HandleTextviewSendBufferChanged;
			string tempstr = textviewSend.Buffer.Text;
			int position = textviewSend.Buffer.CursorPosition + 1;
			
			switch (NowSendMode) {
			case ConvertMode.Hex:
				tempstr = System.Text.RegularExpressions.Regex.Replace (tempstr, "(?<=([0-9A-Fa-f]{2,}))", SplitString);
				tempstr = System.Text.RegularExpressions.Regex.Replace (tempstr, "[^0-9^A-F^a-f]+", SplitString);
				break;
			case ConvertMode.Dec:
				tempstr = System.Text.RegularExpressions.Regex.Replace (tempstr, "[^0-9]+", SplitString);
				tempstr = System.Text.RegularExpressions.Regex.Replace (tempstr, "((?<=(25))(?=[6-9]))|(?<=([^0-9]{0,1}[3-9][0-9]))|(?<=([^0-9]{0,1}2[6-9]))|(?<=([0-9]{3,}))", SplitString);
				tempstr = System.Text.RegularExpressions.Regex.Replace (tempstr, "[^0-9]+", SplitString);
				break;
			}
			
			textviewSend.Buffer.Text = tempstr;
			if (position > textviewSend.Buffer.EndIter.Offset)
				position = textviewSend.Buffer.EndIter.Offset;
			textviewSend.Buffer.PlaceCursor (textviewSend.Buffer.GetIterAtOffset (position));
			textviewSend.Buffer.Changed += HandleTextviewSendBufferChanged;
		}
	}

	#region In Threading Envent
	void HandleSendTimerElapsed (object sender, System.Timers.ElapsedEventArgs e)
	{
		Gdk.Threads.Enter ();
		OnButtonSendClicked (this, null);
		Gdk.Threads.Leave ();
	}

	void HandlePortNameTimerElapsed (object sender, System.Timers.ElapsedEventArgs e)
	{
		Gdk.Threads.Enter ();
		InitializationPortName ();
		Gdk.Threads.Leave ();
	}
	void HandleMyPortDataReceived (object sender, SerialDataReceivedEventArgs e)
	{
		byte[] buffer = new byte[MyPort.BytesToRead];
		MyPort.Read (buffer, 0, buffer.Length);
		ReceiveStream.Write (buffer, 0, buffer.Length);
		labelRxStatus.Text = ReceiveStream.Length.ToString ();
		Gdk.Threads.Enter ();
		// 准备在线程中更新界面
		TextIter iter;
		iter = textviewText.Buffer.EndIter;
		textviewText.Buffer.Insert (ref iter, StringConverts.BytesToString (buffer));
		if (checkbuttonAutoScrollReceive.Active) {
			textviewText.Buffer.CreateMark ("EndMark", iter, false);
			textviewText.ScrollToMark (textviewText.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
			textviewText.Buffer.DeleteMark ("EndMark");
		}
		iter = textviewHex.Buffer.EndIter;
		textviewHex.Buffer.Insert (ref iter, StringConverts.BytesToHexString (buffer));
		if (checkbuttonAutoScrollReceive.Active) {
			textviewHex.Buffer.CreateMark ("EndMark", iter, false);
			textviewHex.ScrollToMark (textviewHex.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
			textviewHex.Buffer.DeleteMark ("EndMark");
		}
		iter = textviewDec.Buffer.EndIter;
		textviewDec.Buffer.Insert (ref iter, StringConverts.BytesToDecString (buffer));
		if (checkbuttonAutoScrollReceive.Active) {
			textviewDec.Buffer.CreateMark ("EndMark", iter, false);
			textviewDec.ScrollToMark (textviewDec.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
			textviewDec.Buffer.DeleteMark ("EndMark");
		}
		Gdk.Threads.Leave ();
	}
	#endregion


	private void InitializationPortName ()
	{
		if (System.IO.Ports.SerialPort.GetPortNames ().Length != portCount) {
			Console.WriteLine ("Detect Serial Ports Changed");
			portCount = System.IO.Ports.SerialPort.GetPortNames ().Length;
			portNameModel.Clear ();
			foreach (string Name in System.IO.Ports.SerialPort.GetPortNames ()) {
				comboboxentryPortName.AppendText (Name);
			}
			if (portCount > 0) {
				comboboxentryPortName.Active = portCount - 1;
				// 自动选择串口
				
			} else {
				// 无串口
			}
		}
	}
	public void SettingsSafeUpdate ()
	{
		if (MyPort.IsOpen) {
			MyPort.Close ();
			SettingsSynchronization ();
			MyPort.Open ();
		}
	}
	private void SettingsSynchronization ()
	{
		int index;
		portName = comboboxentryPortName.ActiveText;
		Console.WriteLine ("Synchronization\tPortName\t>>\t" + portName);
		baudRate = Convert.ToInt32 (comboboxentryBaudRate.ActiveText);
		Console.WriteLine ("Synchronization\tBaudRate\t>>\t" + baudRate.ToString ());
		index = comboboxPatity.Active;
		switch (index) {
		case 0:
			parity = Parity.None;
			break;
		case 1:
			parity = Parity.Even;
			break;
		case 2:
			parity = Parity.Odd;
			break;
		case 3:
			parity = Parity.Mark;
			break;
		case 4:
			parity = Parity.Space;
			break;
		default:
			parity = Parity.None;
			break;
		}
		Console.WriteLine ("Synchronization\tParity\t\t>>\t" + parity.ToString ());
		dataBits = Convert.ToInt32 (spinbuttonDataBits.Text);
		Console.WriteLine ("Synchronization\tDataBits\t>>\t" + dataBits.ToString ());
		index = comboboxStopBits.Active;
		switch (index) {
		case 0:
			stopBits = StopBits.None;
			break;
		case 1:
			stopBits = StopBits.One;
			break;
		case 2:
			stopBits = StopBits.OnePointFive;
			break;
		case 3:
			stopBits = StopBits.Two;
			break;
		default:
			stopBits = StopBits.One;
			break;
		}
		Console.WriteLine ("Synchronization\tStopBits\t>>\t" + stopBits.ToString ());
		if (MyPort != null) {
			MyPort.PortName = portName;
			MyPort.BaudRate = baudRate;
			MyPort.Parity = parity;
			MyPort.DataBits = dataBits;
			MyPort.StopBits = stopBits;
		}
		Console.WriteLine ("Settings synchronizated");
		
	}
	protected virtual void OntogglebuttonPortSwitchClicked (object sender, System.EventArgs e)
	{
		// 该函数为串口开关
		Console.WriteLine ("Port switch active");
		if (!MyPort.IsOpen) {
			SettingsSynchronization ();
			MyPort.Open ();
			if (MyPort.IsOpen) {
				togglebuttonPortSwitch.Label = "关闭串口(_C)";
				labelPortStatus.Text = "串口开";
				imagePortState.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CommBug.icons.port_state.port_state-on_24x24.png");
			} else {
				Console.WriteLine ("Can't open this port.");
				if (checkbuttonAutoSend.Active) {
					checkbuttonAutoSend.Active = false;
					OnCheckbuttonAutoSendClicked (this, null);
				}
				togglebuttonPortSwitch.Active = false;
			}
		} else {
			if (checkbuttonAutoSend.Active) {
				checkbuttonAutoSend.Active = false;
				OnCheckbuttonAutoSendClicked (this, null);
			}
			MyPort.Close ();
			togglebuttonPortSwitch.Label = "打开串口(_O)";
			labelPortStatus.Text = "串口关";
			imagePortState.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CommBug.icons.port_state.port_state-off_24x24.png");
			togglebuttonPortSwitch.Active = false;
		}
	}

	protected virtual void OnButtonSendClicked (object sender, System.EventArgs e)
	{
		if (!MyPort.IsOpen) {
			togglebuttonPortSwitch.Active = true;
			if (!MyPort.IsOpen)
				OntogglebuttonPortSwitchClicked (this, null);
		}
		if (MyPort.IsOpen) {
			string strSend = textviewSend.Buffer.Text;
			byte[] sendByte;
			switch (SendMode) {
			case ConvertMode.Text:
				sendByte = new byte[StringConverts.StringToBytes (strSend).Length];
				sendByte = StringConverts.StringToBytes (strSend);
				break;
			case ConvertMode.Hex:
				sendByte = new byte[StringConverts.HexStringToBytes (strSend).Length];
				sendByte = StringConverts.HexStringToBytes (strSend);
				break;
			case ConvertMode.Dec:
				sendByte = new byte[StringConverts.DecStringToBytes (strSend).Length];
				sendByte = StringConverts.DecStringToBytes (strSend);
				break;
			default:
				sendByte = new byte[StringConverts.StringToBytes (strSend).Length];
				sendByte = StringConverts.StringToBytes (strSend);
				break;
			}
			MyPort.Write (sendByte, 0, sendByte.Length);
			SendStream.Write (sendByte, 0, sendByte.Length);
			labelTxStatus.Text = SendStream.Length.ToString ();
			TextIter iter;
			iter = textviewTextS.Buffer.EndIter;
			textviewTextS.Buffer.Insert (ref iter, StringConverts.BytesToString (sendByte));
			if (checkbuttonAutoScrollSend.Active) {
				// Console.WriteLine("\nLast\t{0}",GtkScrolledWindowTextS.Vadjustment.Value);
				// Console.WriteLine("Lower\t{0}",GtkScrolledWindowTextS.Vadjustment.Lower);
				// Console.WriteLine("S\t{0}",GtkScrolledWindowTextS.Vadjustment.StepIncrement);
				// Console.WriteLine("P\t{0}",GtkScrolledWindowTextS.Vadjustment.PageIncrement);
				// GtkScrolledWindowTextS.Vadjustment.Value = GtkScrolledWindowTextS.Vadjustment.Upper;
				textviewTextS.Buffer.CreateMark ("EndMark", iter, false);
				textviewTextS.ScrollToMark (textviewTextS.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
				textviewTextS.Buffer.DeleteMark ("EndMark");
			}
			
			iter = textviewHexS.Buffer.EndIter;
			textviewHexS.Buffer.Insert (ref iter, StringConverts.BytesToHexString (sendByte));
			if (checkbuttonAutoScrollSend.Active) {
				// GtkScrolledWindowHexS.Vadjustment.Value = GtkScrolledWindowHexS.Vadjustment.Upper;
				textviewHexS.Buffer.CreateMark ("EndMark", iter, false);
				textviewHexS.ScrollToMark (textviewHexS.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
				textviewHexS.Buffer.DeleteMark ("EndMark");
			}
			iter = textviewDecS.Buffer.EndIter;
			textviewDecS.Buffer.Insert (ref iter, StringConverts.BytesToDecString (sendByte));
			if (checkbuttonAutoScrollSend.Active) {
				// GtkScrolledWindowDecS.Vadjustment.Value = GtkScrolledWindowDecS.Vadjustment.Upper;
				textviewDecS.Buffer.CreateMark ("EndMark", iter, false);
				textviewDecS.ScrollToMark (textviewDecS.Buffer.CreateMark ("EndMark", iter, false), 0, false, 0, 0);
				textviewDecS.Buffer.DeleteMark ("EndMark");
			}
			// ---------------
			// Generate Linenumber,copyright by vowstar@gmail.com,^-^
			// TextViewUtils.GenerateLineNumber(textviewTextS,textviewTextSL,lineHeight);			
		}
	}

	protected virtual void OnRadiobuttonSendActivated (object sender, System.EventArgs e)
	{
		
		if (radiobuttonText.Active) {
			NowSendMode = ConvertMode.Text;
		} else if (radiobuttonHex.Active) {
			NowSendMode = ConvertMode.Hex;
		} else if (radiobuttonDec.Active) {
			NowSendMode = ConvertMode.Dec;
		}
		if (NowSendMode != SendMode) {
			string strSend = textviewSend.Buffer.Text;
			byte[] sendByte;
			switch (SendMode) {
			case ConvertMode.Text:
				sendByte = new byte[StringConverts.StringToBytes (strSend).Length];
				sendByte = StringConverts.StringToBytes (strSend);
				break;
			case ConvertMode.Hex:
				sendByte = new byte[StringConverts.HexStringToBytes (strSend).Length];
				sendByte = StringConverts.HexStringToBytes (strSend);
				break;
			case ConvertMode.Dec:
				sendByte = new byte[StringConverts.DecStringToBytes (strSend).Length];
				sendByte = StringConverts.DecStringToBytes (strSend);
				break;
			default:
				sendByte = new byte[StringConverts.StringToBytes (strSend).Length];
				sendByte = StringConverts.StringToBytes (strSend);
				break;
			}
			switch (NowSendMode) {
			case ConvertMode.Text:
				strSend = StringConverts.BytesToString (sendByte);
				break;
			case ConvertMode.Hex:
				strSend = StringConverts.BytesToHexString (sendByte);
				break;
			case ConvertMode.Dec:
				strSend = StringConverts.BytesToDecString (sendByte);
				break;
			}
			textviewSend.Buffer.Text = strSend;
			SendMode = NowSendMode;
		}
	}

	protected virtual void OnSpinbuttonIntervalValueChanged (object sender, System.EventArgs e)
	{
		SendTimer.Interval = Convert.ToDouble (spinbuttonInterval.Text);
	}

	protected virtual void OnCheckbuttonAutoSendClicked (object sender, System.EventArgs e)
	{
		if (checkbuttonAutoSend.Active) {
			SendTimer.Enabled = true;
		} else {
			SendTimer.Enabled = false;
		}
	}

	protected virtual void OnButtonClearSendContentsClicked (object sender, System.EventArgs e)
	{
		textviewSend.Buffer.Clear ();
	}
	protected virtual void OnButtonClearReceiveAreaClicked (object sender, System.EventArgs e)
	{
		textviewText.Buffer.Clear ();
		textviewHex.Buffer.Clear ();
		textviewDec.Buffer.Clear ();
	}

	protected virtual void OnButtonClearSendAreaClicked (object sender, System.EventArgs e)
	{
		textviewTextS.Buffer.Clear ();
		textviewHexS.Buffer.Clear ();
		textviewDecS.Buffer.Clear ();
	}

	protected virtual void OnButtonClearAllClicked (object sender, System.EventArgs e)
	{
		OnButtonClearSendContentsClicked (this, null);
		OnButtonClearReceiveAreaClicked (this, null);
		OnButtonClearSendAreaClicked (this, null);
		
	}

	protected virtual void OnTextviewSendBackspace (object sender, System.EventArgs e)
	{
		if (NowSendMode != ConvertMode.Text) {
			textviewSend.Buffer.Changed -= HandleTextviewSendBufferChanged;
			textviewSend.Buffer.Text = textviewSend.Buffer.Text.TrimEnd (SplitString.ToCharArray ());
			textviewSend.Buffer.Changed += HandleTextviewSendBufferChanged;
		}
	}

	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		DeleteEventArgs a = new DeleteEventArgs ();
		OnDeleteEvent (this, a);
	}

	protected virtual void OnPreferencesActionActivated (object sender, System.EventArgs e)
	{
		if (preferenceDialog == null) {
			preferenceDialog = new CommBug.PreferenceDialog ();
		} else {
			if (!preferenceDialog.Visible)
				preferenceDialog = new CommBug.PreferenceDialog ();
		}
	}


	protected virtual void OnAboutActionActivated (object sender, System.EventArgs e)
	{
		if (aboutDialog == null) {
			aboutDialog = new CommBug.AboutDialog ();
		} else {
			if (!aboutDialog.Visible)
				aboutDialog = new CommBug.AboutDialog ();
		}
	}
	protected virtual void OnNumericalModelingActionActivated (object sender, System.EventArgs e)
	{
		
		if (numericalModelingDialog == null) {
			numericalModelingDialog = new CommBug.NumericalModelingDialog ();
		} else {
			if (!numericalModelingDialog.Visible)
				numericalModelingDialog = new CommBug.NumericalModelingDialog ();
		}
		numericalModelingDialog.ProceedModeling(this.ReceiveStream);	
		
	}


	protected virtual void OnComboboxentryBaudRateChanged (object sender, System.EventArgs e)
	{
		SettingsSafeUpdate ();
	}

	protected virtual void OnComboboxentryPortNameChanged (object sender, System.EventArgs e)
	{
		try {
			if (MyPort.IsOpen) {
				MyPort.Close ();
				MyPort.PortName = comboboxentryPortName.ActiveText;
				MyPort.Open ();
			} else {
				MyPort.PortName = comboboxentryPortName.ActiveText;
			}
			
		} catch (Exception ex) {
			Console.WriteLine (ex.Message);
		}
		
		
	}

	protected virtual void OnComboboxPatityChanged (object sender, System.EventArgs e)
	{
		SettingsSafeUpdate ();
	}

	protected virtual void OnSpinbuttonDataBitsChanged (object sender, System.EventArgs e)
	{
		try {
			int TestDataBits = Convert.ToInt32 (spinbuttonDataBits.Text);
			if (TestDataBits > 8)
				spinbuttonDataBits.Text = "8";
			if (TestDataBits < 5)
				spinbuttonDataBits.Text = "5";
			SettingsSafeUpdate ();
		} catch (Exception ex) {
			Console.WriteLine (ex.Message);
		}
		
		
	}

	protected virtual void OnComboboxStopBitsChanged (object sender, System.EventArgs e)
	{
		SettingsSafeUpdate ();
	}
	
	
	
	
	
	
	
	
	
	
}

