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
// --------------------------------------------------------------------------------------
// Class		:	StringConverts
// Author		:	HuangRui(vowstar@gmail.com),Lanzhou University
// Date			:	2010-08-04
// Description	:	Convert text/hex string/dec string -> byte -> text/hex string/dec string 
// 					The default charset is GB2312
// --------------------------------------------------------------------------------------
public static class StringConverts
{
	public static string Charset = "gb2312";
	public static string SplitString = "\t";
	// 转码函数分隔标记
	public static bool EnableOutput = true;
	// 是否允许调试输出
	public static string BytesToString (byte[] buffer)
	{
		return System.Text.Encoding.GetEncoding (Charset).GetString (buffer);
	}
	public static string BytesToHexString (byte[] buffer)
	{
		int i;
		string result = "";
		for (i = 0; i < buffer.Length; i++) {
			result = result + buffer[i].ToString ("X2") + SplitString;
			// 转换为16进制
		}
		return result;
	}
	public static string BytesToDecString (byte[] buffer)
	{
		int i;
		string result = "";
		for (i = 0; i < buffer.Length; i++) {
			result = result + buffer[i].ToString ("D3") + SplitString;
		}
		return result;
	}
	public static byte[] StringToBytes (string str)
	{
		try {
			return System.Text.Encoding.GetEncoding (Charset).GetBytes (str);
		} catch (Exception ex) {
			OutPut (ex.Message);
			OutPut ("Try to use default encoding.Please install mono-i18n.");
			return System.Text.Encoding.Default.GetBytes (str);
		}
	}
	public static byte[] HexStringToBytes (string str)
	{
		str = System.Text.RegularExpressions.Regex.Replace (str, "[^0-9^A-F^a-f]+", SplitString);
		string[] splitedstrs = str.Split (SplitString.ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
		byte[] result = new byte[splitedstrs.Length];
		int i = 0;
		for (i = 0; i < result.Length; i++) {
			result[i] = Convert.ToByte (splitedstrs[i], 16);
		}
		return result;
	}
	public static byte[] DecStringToBytes (string str)
	{
		str = System.Text.RegularExpressions.Regex.Replace (str, "[^0-9]+", SplitString);
		string[] splitedstrs = str.Split (SplitString.ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
		byte[] result = new byte[splitedstrs.Length];
		int i = 0;
		for (i = 0; i < result.Length; i++) {
			result[i] = Convert.ToByte (splitedstrs[i]);
		}
		return result;
	}
	private static void OutPut (string format, params object[] arg)
	{
		if (EnableOutput) {
			Console.Write ("{0}\t>>\t", "StringConverts");
			Console.WriteLine (format, arg);
		}
	}
}


