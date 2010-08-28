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
	public static string BytesToString (byte[] buffer)
	{
		return System.Text.Encoding.GetEncoding (Charset).GetString (buffer);
	}
	public static string BytesToHexString (byte[] buffer)
	{
		int i;
		string result = "";
		for (i = 0; i < buffer.Length; i++) {
			result = result + buffer[i].ToString ("X") + SplitString;
			// 转换为16进制
		}
		return result;
	}
	public static string BytesToDecString (byte[] buffer)
	{
		int i;
		string result = "";
		for (i = 0; i < buffer.Length; i++) {
			result = result + buffer[i].ToString () + SplitString;
		}
		return result;
	}
	public static byte[] StringToBytes (string str)
	{
		return System.Text.Encoding.GetEncoding (Charset).GetBytes (str);
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
	
}


