using UnityEngine;
using System.Collections;

public class ScriptsInitiator : MonoBehaviour 
{
	void Awake()
	{
		if (FindObjectOfType<ServerInfo>()==null)
			ServerInfo.Instance.Init();
	}
}
