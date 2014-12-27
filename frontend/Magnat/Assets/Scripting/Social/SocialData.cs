public class SocialData
{
	public string ViewerId { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Photo { get; set; }
	
	private string _formatNameCache;
	
	public string FormatName
	{
		get
		{
			if (string.IsNullOrEmpty(_formatNameCache))
			{
				_formatNameCache = FirstName + " " + LastName;
				if (_formatNameCache.Length > 20)
				{
					_formatNameCache = _formatNameCache.Remove(17) + "...";
				}
			}
			
			return _formatNameCache;
		}
	}
}