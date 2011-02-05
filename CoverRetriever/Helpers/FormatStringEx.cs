using System;
using System.Globalization;

public static class FormatStringEx
{
	/// <summary>
	/// Invariant inline string format
	/// </summary>
	/// <param name="str"></param>
	/// <param name="param"></param>
	/// <returns></returns>
	public static string FormatString(this string str, params object[] param)
	{
		return String.Format(CultureInfo.InvariantCulture, str, param);
	}

	/// <summary>
	/// Inline string format
	/// </summary>
	/// <param name="str"></param>
	/// <param name="param"></param>
	/// <returns></returns>
	public static string FormatUIString(this string str, params object[] param)
	{
		return String.Format(CultureInfo.CurrentUICulture, str, param);
	}

}