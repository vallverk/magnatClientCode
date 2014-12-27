using UnityEngine;
using System.Collections;
using System.Linq;

public class AtlasLoader : MonoBehaviour 
{
	[System.Serializable]
	public struct ToLoad
	{
		[SerializeField] public string ImageURL;
		[SerializeField] public Material MaterialToSet;

		private AtlasLoader aloader;

		public void UpdateTex(Texture tex)
		{
			aloader.Loaded++;
			MaterialToSet.SetTexture("_MainTex",tex);
		}

		public void Init(AtlasLoader loader)
		{
			aloader = loader;
			ImageLoader.Instance.LoadAvatar(ImageURL,UpdateTex);
		}
	}

	private int loaded = 0;

	public int Loaded
	{
		get { return loaded;}
		set { loaded = value; }
	}

	public ToLoad[] LoadingData;
	public UIProgressBar Progress;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
		foreach (var o in LoadingData)
			o.Init(this);
	}

	private bool loaderScene = true;

	// быдлокод, конечно, тут ставить, но времени не много на фиксы :(
	void LoadScene()
	{
		ServerInfo.Instance.GetGameAnnounceList((gis)=>{

			GameInfo game = gis.FirstOrDefault(g =>
			                                   g.UserList.Contains(SocialManager.Instance.ViewerID) && 
			                                   g.Status == 1 && 
			                                   !ServerData.IsGameAtBlackList(g.GUID.ToString()));

			if (game == null)
				Application.LoadLevel(1);
			else
				StartCoroutine(LoadUsersAndStartGame(game));
		});
	}

	IEnumerator LoadUsersAndStartGame(GameInfo g)
	{
		// ready to start!... maybe....

		#if !UNITY_EDITOR
		bool canLoad = true;
		for (int i=0;i<g.UserList.Count;i++)
			while (SocialManager.GetUserData(g.UserList[i]) == null)
				yield return new WaitForSeconds(0.1f);

		//  rly ready to start!

		#endif

		PlayerPrefs.SetString("LoadGame",JSONSerializer.Serialize(g));
		Application.LoadLevel(2);
		yield break;
	}

	void Update()
	{
		if (Loaded == this.LoadingData.Length && loaderScene && ServerDataLoader.isLoaded)
		{
			loaderScene = false;
			enabled = false;
			LoadScene();
		} else
			Progress.value = (loaded*1.0f) / (LoadingData.Length*1.0f);
	}
}
