using UnityEngine;
using System.Collections;

public class SoundSwitcher : MonoBehaviour 
{
	public GameObject VolumeOnGO;
	public GameObject VolumeOffGO;

	public void SwitchOn()
	{
		VolumeOnGO.SetActive(false);
		VolumeOffGO.SetActive(true);
	}

	public void SwitchOff()
	{
		VolumeOnGO.SetActive(true);
		VolumeOffGO.SetActive(false);
	}
}
