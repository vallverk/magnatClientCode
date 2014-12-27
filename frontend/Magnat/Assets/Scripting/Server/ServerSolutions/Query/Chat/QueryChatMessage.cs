using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QueryChatMessage
{
	[JsonProperty]
	public string SenderID {set; get;}

	[JsonProperty]
	public string RoomName {set; get;}

	[JsonProperty]
	public string Message {set; get;}

	[JsonProperty]
	public int count {set; get;}
}
