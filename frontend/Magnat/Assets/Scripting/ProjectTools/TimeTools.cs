using UnityEngine;
using System.Collections;
using System;

public class TimeTools
{
	public static double GetUTCTimeStamp()
	{
		return ((TimeSpan)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
	}

	public static double GetLinuxTimeStamp()
	{
		return ((TimeSpan)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalMilliseconds;
	}

	public static double GetLinuxTimeStamp(DateTime Data)
	{
		return ((TimeSpan)(Data - new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalMilliseconds;
	}

	public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
	{
		// Unix timestamp is seconds past epoch
		System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
		// если делать по-нормальному, то long привосходит ограничения...
		dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
		return dtDateTime;
	}

	public static string FormatUTSTime(double UTS)
	{
		return UnixTimeStampToDateTime(UTS/1000.0).ToString("dd.MM.yyyy HH:mm:ss");
	}
}
