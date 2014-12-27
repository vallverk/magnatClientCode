using Newtonsoft.Json;

public static class JSONSerializer 
{
	public static string Serialize(object Obj)
	{
		return JsonConvert.SerializeObject(Obj);
	}

	public static T Deserialize<T>(string JSON)
	{
		return JsonConvert.DeserializeObject<T>(JSON);
	}
}
