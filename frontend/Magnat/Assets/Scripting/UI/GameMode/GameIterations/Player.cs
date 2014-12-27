public class Player
{
	[Newtonsoft.Json.JsonIgnore] public string FirstName {get;set;}
	[Newtonsoft.Json.JsonIgnore] public string LastName {get;set;}
	[Newtonsoft.Json.JsonIgnore] public string PhotoURL {get;set;}
	[Newtonsoft.Json.JsonIgnore] public string LongName { get { return string.Format("{0} {1}",FirstName,LastName); } }

	public string SocialID {get;set;}
	public GameField.Owners OwnerID {get;set;}
	public bool Bankrout {get;set;}
	public int TablePosition;
	
	[Newtonsoft.Json.JsonIgnore] public System.Action<string> OnBankrotStart = (a) => {};
	private int cash;
	public int Cash
	{
		get { return cash; }
		set
		{
			cash = value;
			if (cash<0)
			{
				Bankrout = true;
				OnBankrotStart(SocialID);
			}
		}
	}
	
	public int Capital {get;set;}
	
	
	public override string ToString ()
	{
		return string.Format ("[Player: SocialID={0}, FirstName={1}, LastName={2}, PhotoURL={3}, " +
		                      "OwnerID={4}, Bankrout={5}, Cash={6}, Capital={7}, LongName={8}]", SocialID, FirstName, 
		                      LastName, PhotoURL, OwnerID, Bankrout, Cash, Capital, LongName);
	}
}