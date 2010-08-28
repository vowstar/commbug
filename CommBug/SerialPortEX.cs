using System;
using System.IO.Ports;
using System.Threading;
// --------------------------------------------------------------------------------------
// Class		:	SerialPortEx
// Author		:	HuangRui(vowstar@gmail.com),Lanzhou University
// Date			:	2010-08-04
// Description	:	Easy 2 control serialports under mono.net,designed 4 Chinese^-^
// 					优化了对中文的处理
// --------------------------------------------------------------------------------------
public class SerialPortEx : IDisposable
{
	#region 属性与变量们
	// ^-^..by蝶晓梦，vowstar@gmail.com
	private SerialPort TheSerialPort;
	// 用来使用的串口
	private Thread ReceiveThread;
	// 用于监听串口的线程
	public event SerialDataReceivedEventHandler DataReceived;
	// 接收到数据的事件

	public int Wait = 100;
	// 扫描周期，越小实时性越好，但是CPU占用率会上升
	public bool EnableOutput = true;
	// 是否允许调试输出
	private bool _Listening = false;
	// 内部变量，禁止更改
	public bool Listening {
		get { return _Listening; }
		set {
			if (ReceiveThread == null) {
				ReceiveThread = new Thread (new ThreadStart (Worker));
				OutPut ("New thread");
			}
			// 这里启用线程监视串口，弥补Mono不能在Ubuntu10.04下很好响应DataReceived的缺陷
			if (value) {
				_Listening = true;
				if (!ReceiveThread.IsAlive)
					ReceiveThread.Start ();
			} else
				_Listening = false;
		}
	}
	// 为真则监视线程运行
	public string PortName {
		get { return TheSerialPort.PortName; }
		set { TheSerialPort.PortName = value; }
	}
	// 串口名称，例如COM1(Windows)，/dev/Usbtty0(Linux)，rfcomm1..
	public int BaudRate {
		get { return TheSerialPort.BaudRate; }
		set { TheSerialPort.BaudRate = value; }
	}
	// 波特率
	public System.IO.Ports.Parity Parity {
		get { return TheSerialPort.Parity; }
		set { TheSerialPort.Parity = value; }
	}
	// 奇偶校验
	public int DataBits {
		get { return TheSerialPort.DataBits; }
		set { TheSerialPort.DataBits = value; }
	}
	// 数据位，一般为8
	public System.Text.Encoding Encoding {
		get { return TheSerialPort.Encoding; }
		set { TheSerialPort.Encoding = value; }
	}
	// 对接收到的数据进行编码的编码器
	public string Charset {
		get { return TheSerialPort.Encoding.BodyName; }
		// 这里会返回如“GB2312”的东西，而EncodingName会出现全称
		set { TheSerialPort.Encoding = System.Text.Encoding.GetEncoding (value); }
	}
	// 字符集，例如GB2312
	public int CodePage {
		// 代码页设定，一般为简体中文， 936
		get { return TheSerialPort.Encoding.CodePage; }
		set { TheSerialPort.Encoding = System.Text.Encoding.GetEncoding (value); }
	}
	// 代码页

	public System.IO.Ports.StopBits StopBits {
		get { return TheSerialPort.StopBits; }
		set { TheSerialPort.StopBits = value; }
	}
	// 停止位
	public int BytesToRead {
		get { return TheSerialPort.BytesToRead; }
	}
	// 在缓冲区中的字节数
	public string NewLine {
		get { return TheSerialPort.NewLine; }
		set { TheSerialPort.NewLine = value; }
	}
	// 行分隔标记

	#endregion
	#region 构造函数及重载们
	public SerialPortEx ()
	{
		Init ("", 1200, Parity.None, 8, StopBits.One);
	}
	public SerialPortEx (string portName)
	{
		Init (portName, 1200, Parity.None, 8, StopBits.One);
	}
	public SerialPortEx (string portName, int baudRate)
	{
		Init (portName, baudRate, Parity.None, 8, StopBits.One);
	}
	public SerialPortEx (string portName, int baudRate, System.IO.Ports.Parity parity)
	{
		Init (portName, baudRate, parity, 8, StopBits.One);
	}
	public SerialPortEx (string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits)
	{
		Init (portName, baudRate, parity, dataBits, StopBits.One);
	}

	public SerialPortEx (string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits, System.IO.Ports.StopBits stopBits)
	{
		Init (portName, baudRate, parity, dataBits, stopBits);
	}
	#endregion
	#region 方法们
	private void Init (string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits, System.IO.Ports.StopBits stopBits)
	{
		int index;
		index = SerialPort.GetPortNames ().Length;
		if (portName == "") {
			if (index > 0) {
				portName = SerialPort.GetPortNames ()[index - 1];
				OutPut ("Automatic mode. Catch :" + portName);
				// 自动选择串口
			} else
				OutPut ("Can't find any serialport");
		}
		OutPut ("Try to use :" + portName);
		#region 初始化串口
		TheSerialPort = new SerialPort ();
		try {
			TheSerialPort.PortName = portName;
			OutPut ("Set PortName->" + portName);
			TheSerialPort.BaudRate = baudRate;
			OutPut ("Set BaudRate->" + baudRate.ToString ());
			TheSerialPort.Parity = parity;
			OutPut ("Set Parity ->" + parity.ToString ());
			TheSerialPort.DataBits = dataBits;
			OutPut ("Set DataBits ->" + dataBits.ToString ());
			TheSerialPort.StopBits = stopBits;
			OutPut ("Set StopBits ->" + stopBits.ToString ());
			TheSerialPort.Handshake = Handshake.None;
			TheSerialPort.ReadTimeout = 1;
			TheSerialPort.Encoding = System.Text.Encoding.GetEncoding ("gb2312");
			
		} catch (Exception e) {
			OutPut (e.Message);
		}
		
		#endregion
		
		Listening = true;
		
	}
	public void Dispose ()
	{
		OutPut ("Beging disposing");
		Listening = false;
		while (ReceiveThread.IsAlive)
			Thread.Sleep (Wait);
		if (TheSerialPort.IsOpen) {
			TheSerialPort.Close ();
		}
		TheSerialPort.Dispose ();
		OutPut ("SerialPort released");
		OutPut ("Disposed");
	}
	public bool Open ()
	{
		try {
			TheSerialPort.Open ();
			if (TheSerialPort.IsOpen) {
				OutPut ("SerialPort opened");
				return true;
			} else {
				OutPut ("SerialPort not opened");
				return false;
			}
		} catch (Exception e) {
			OutPut (e.Message);
		}
		return false;
	}

	public bool Close ()
	{
		if (!TheSerialPort.IsOpen)
			return true;
		else {
			try {
				TheSerialPort.Close ();
				if (!TheSerialPort.IsOpen) {
					OutPut ("SerialPort closed");
					return true;
				} else {
					OutPut ("SerialPort not closed");
					return false;
				}
			} catch (Exception e) {
				OutPut (e.Message);
			}
			return false;
		}
	}

	public string ReadExisting ()
	{
		try {
			if (TheSerialPort.IsOpen) {
				return TheSerialPort.ReadExisting ();
			} else {
				return "";
			}
		} catch (Exception ex) {
			OutPut (ex.Message);
			return "";
		}
	}
	public bool IsOpen {
		get { return TheSerialPort.IsOpen; }
		set {
			if (value) {
				Open ();
			} else {
				Close ();
			}
		}
	}


	public int Read (char[] Buffer, int DateTimeOffset, int count)
	{
		return TheSerialPort.Read (Buffer, DateTimeOffset, count);
	}
	public int Read (byte[] Buffer, int DateTimeOffset, int count)
	{
		return TheSerialPort.Read (Buffer, DateTimeOffset, count);
	}
	public int ReadByte ()
	{
		return TheSerialPort.ReadByte ();
	}
	public int ReadChar ()
	{
		return TheSerialPort.ReadChar ();
	}
	public string ReadLine ()
	{
		return TheSerialPort.ReadLine ();
	}
	public string ReadTo (string value)
	{
		return TheSerialPort.ReadTo (value);
	}
	public void Write (byte[] buffer, int offset, int count)
	{
		TheSerialPort.Write (buffer, offset, count);
	}
	public void Write (char[] buffer, int offset, int count)
	{
		TheSerialPort.Write (buffer, offset, count);
	}
	public void Write (string str)
	{
		TheSerialPort.Write (str);
	}
	public void WriteLine (string str)
	{
		TheSerialPort.WriteLine (str);
	}
	public void DiscardInBuffer ()
	{
		TheSerialPort.DiscardInBuffer ();
	}
	public void DiscardOutBuffer ()
	{
		TheSerialPort.DiscardOutBuffer ();
	}
	public bool Send (string str)
	{
		try {
			if (TheSerialPort.IsOpen) {
				TheSerialPort.Write (str);
				OutPut ("Send OK:" + str);
				return true;
			} else {
				OutPut ("Send failed:" + str);
				return false;
			}
		} catch (Exception ex) {
			OutPut (ex.Message);
			return false;
		}
	}
	public bool Send (byte value)
	{
		try {
			if (TheSerialPort.IsOpen) {
				byte[] temp = new byte[1];
				temp[0] = value;
				TheSerialPort.Write (temp, 0, 1);
				OutPut ("Send OK:" + value.ToString ());
				return true;
			} else {
				OutPut ("Send failed:" + value.ToString ());
				return false;
			}
		} catch (Exception ex) {
			OutPut (ex.Message);
			return false;
		}
	}

	public bool Send (char value)
	{
		try {
			if (TheSerialPort.IsOpen) {
				char[] temp = new char[1];
				temp[0] = value;
				TheSerialPort.Write (temp, 0, 1);
				OutPut ("Send OK:" + value.ToString ());
				return true;
			} else {
				OutPut ("Send failed:" + value.ToString ());
				return false;
			}
		} catch (Exception ex) {
			OutPut (ex.Message);
			return false;
		}
	}
	private bool ReceiveChecker ()
	{
		try {
			if (Listening) {
				if (TheSerialPort.IsOpen) {
					if (TheSerialPort.BytesToRead > 0) {
						if (DataReceived != null) {
							OutPut ("Data received active");
							DataReceived (this, null);
						}
					}
					
					return true;
				} else {
					return false;
				}
			} else
				return false;
		} catch (Exception ex) {
			OutPut (ex.Message);
			return false;
		}
	}
	private void Worker ()
	{
		OutPut ("ThreadStart");
		while (Listening) {
			ReceiveChecker ();
			Thread.Sleep (Wait);
		}
		OutPut ("ThreadEnd");
	}
	private void OutPut (string s)
	{
		if (EnableOutput)
			Console.WriteLine (this.ToString () + ">>" + s);
	}
	
	
	
	#endregion
}


