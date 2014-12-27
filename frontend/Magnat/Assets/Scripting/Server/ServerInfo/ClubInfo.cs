using System.Collections.Generic;

[System.Serializable]
public class ClubInfo
{
	[Newtonsoft.Json.JsonIgnore] public string ID;
	public string ClubName;
	public string CreatorID;
	public string Description;
	public int Lavel;
	[Newtonsoft.Json.JsonIgnore] public string LevelName;
	public long	Capital;
	public int Gold;
	public string Icon;
	public List<string> UserList;
	public int UsersLimit;
	public long DateOfDeath;
	public int MinEnterPrice;
	public List<object> CardList;
}
