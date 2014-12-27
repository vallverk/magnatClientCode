using System.Collections.Generic;
using System;
using Newtonsoft.Json;

public class Query
{
	public string UserID;
	public string AuthKey;
	public string ViewerID;
	public string Type;
	public List<object> Args;

	[JsonIgnore]
	public System.Net.Sockets.SocketError Error;

	[JsonIgnore]
	public bool UseUREncode = true;

	public Query()
	{
		UserID = "";
		AuthKey = "";
		ViewerID = "";
		Type = "";
		Error = System.Net.Sockets.SocketError.Success;
		Args = new List<object>();
	}

	public Query(Query Original, System.Net.Sockets.SocketError ErrorState)
	{
		UserID = Original.UserID;
		AuthKey = Original.AuthKey;
		ViewerID = Original.ViewerID;
		Type = Original.Type;
		Error = ErrorState;
		Args = Original.Args;
	}

	public string GetJSON()
	{
		return JSONSerializer.Serialize(this);
	}

	public static Query Parse(string JSON)
	{
		return JSONSerializer.Deserialize<Query>(JSON);
	}

	public static bool TryParse(string JSON, out Query Q)
	{
		try 
		{
			Q = JSONSerializer.Deserialize<Query>(JSON);
			return true;
		} 
		catch (Exception e)
		{
			UnityEngine.Debug.Log("Error: Can't parse query; "+e.Message);
			Q = null;
			return false;
		}
	}

	public override bool Equals (object obj)
	{
		if (obj == null || GetType() != obj.GetType()) return false;
		Query q = (Query)obj;
		return q.Type.Equals(this.Type);
	}
}
