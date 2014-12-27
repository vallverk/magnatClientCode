using System.Text;

public static class DataTransfer
{
	public static byte[] GetBytes(string String) { return Encoding.UTF8.GetBytes(String); }

	public static string GetString(byte[] Bytes) { return Encoding.UTF8.GetString(Bytes); }

	public static string GetString(byte[] Bytes, int BytesRec)
	{
		return Encoding.UTF8.GetString(Bytes,0,BytesRec);
	}
}
