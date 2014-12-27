using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class SocketClient : MonoBehaviour 
{
	void Start()
	{
		//Client();
	}

	public static Socket Connect(string Host, int Port)
	{
		Socket sender = null;
		// Connect to a remote device.
		try {
			// Establish the remote endpoint for the socket.
			// This example uses port 11000 on the local computer.
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress,Port);
			
			// Create a TCP/IP  socket.
			sender = new Socket(AddressFamily.InterNetwork, 
			                           SocketType.Stream, ProtocolType.Tcp );
			sender.Connect(remoteEP);
			
		} catch {
			return null;
		}
		return sender;
	}

	public static IPAddress GetIP(string Host)
	{
		try
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Host);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			return ipAddress;
		} catch
		{
			return null;
		}
	}

	public static Socket Connect(IPAddress ipAddress, int Port)
	{
		Socket sender = null;
		// Connect to a remote device.
		try {
			IPEndPoint remoteEP = new IPEndPoint(ipAddress,Port);
			
			// Create a TCP/IP  socket.
			sender = new Socket(AddressFamily.InterNetwork, 
			                    SocketType.Stream, ProtocolType.Tcp );
			sender.Connect(remoteEP);
			
		} catch (System.Exception e) {
			print(e.Message + " at port "+Port);
			return null;
		}
		return sender;
	}

	public static void CloseConnection(Socket S)
	{
		if (S!=null)
		{
			try
			{
			if (S.Connected)
				S.Shutdown(SocketShutdown.Both);
			} catch {}
			S.Close();
		}
	}

	public static String Receive(Socket S)
	{
		try
		{
			byte[] buffer = new byte[10240]; 
			int bytesRec = S.Receive(buffer);
			string res = DataTransfer.GetString(buffer,bytesRec);
			return res;
		} catch {return "";}
	}

	public static bool Send(Socket S, string Text)
	{
		int len = -1;
		byte[] msg = DataTransfer.GetBytes(Text);
		try {
			if (S==null) throw new System.Exception("Сокет нулевой...");
			len = S.Send(msg);
		} catch (ArgumentNullException ane) {
			Debug.Log(string.Format("ArgumentNullException : {0}",ane.ToString()));
		} catch (SocketException se) {
			Debug.Log(string.Format("SocketException : {0}",se.ToString()));
		} catch (Exception e) {
			Debug.Log(string.Format("Unexpected exception : {0}", e.ToString()));
		}
		return len == msg.Length;
	}

	public static bool Send(Socket S, byte[] msg)
	{
		int len = -1;
		try {
			if (S==null) throw new System.Exception("Сокет нулевой...");
			len = S.Send(msg);
		} catch (ArgumentNullException ane) {
			Debug.Log(string.Format("ArgumentNullException : {0}",ane.ToString()));
		} catch (SocketException se) {
			Debug.Log(string.Format("SocketException : {0}",se.ToString()));
		} catch (Exception e) {
			Debug.Log(string.Format("Unexpected exception : {0}", e.ToString()));
		}
		return len == msg.Length;
	}
}
