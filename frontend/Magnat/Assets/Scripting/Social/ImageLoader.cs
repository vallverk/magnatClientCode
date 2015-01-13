using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ImageLoader : Singleton<ImageLoader> 
{
	private Dictionary<string, Texture2D> cash = new Dictionary<string, Texture2D>();
	private Dictionary<string, Action<Texture2D>> queue = new Dictionary<string, Action<Texture2D>>();
	public Texture2D DefaultTexture = null;

	void Awake()
	{
		StartCoroutine("Loader");
	}

	void OnDestroy()
	{
		StopCoroutine("Loader");
		queue.Clear();
		try
		{
		foreach (var pair in cash)
			cash[pair.Key]=null;
		} catch {}
		cash.Clear();
	}

	private void Write(string s)
	{
		//Debug.Log("ImageLoader-> "+s);
	}

	IEnumerator Loader()
	{
		while (true)
		{
			if (queue.Count>0)
			{
				var e = queue.GetEnumerator();
				e.MoveNext();
				string url = e.Current.Key;
				Write("Start load image '"+url+"'");
				if (cash.ContainsKey(url))
				{
					Write("Returnet from cash load image '"+url+"'");
					queue[url](cash[url]);
					queue.Remove(url);
				}
				else
				{
					//WWW w = WWW.LoadFromCacheOrDownload(WWW.EscapeURL(url),1);
					WWW w = new WWW(url);
					yield return w;
					if (string.IsNullOrEmpty(w.error))
					{
						Write("Finished loading image '"+url+"'");
						cash.Add(url,w.texture);
						queue[url](w.texture);
						queue.Remove(url);
					} else
					{
						Write("Can't loading image '"+url+"', return default");
						queue[url](DefaultTexture);
						queue.Remove(url);
					}
				}
			}
			yield return new WaitForFixedUpdate();
		}
	}

	public void LoadAvatar(string URL, Action<Texture2D> OnLoaded)
	{
		if (cash.ContainsKey(URL))
			OnLoaded(cash[URL]);
		else
		{
			if (queue.ContainsKey(URL))
				queue[URL]+=OnLoaded;
			else
				queue.Add(URL,OnLoaded);
		}
	}
}
