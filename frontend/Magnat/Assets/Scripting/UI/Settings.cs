using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour 
{
	public static bool SoundOn = true;

	public void SetFullScreen()
	{
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
		else
		{
			Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,true);
		}
	}

	public void UpdateSounds()
	{
		SoundOn = !SoundOn;

		SetSoundSprites(UIButton.current);

	}

	private void SetSoundSprites(UIButton button)
	{
		if (SoundOn)
		{
			button.tweenTarget.GetComponent<UISprite>().spriteName = "sound-on";
			button.normalSprite = "sound-on";
			button.hoverSprite = "sound-on_hov";
			button.pressedSprite = "sound-on_active";
		} 
		else
		{
			button.tweenTarget.GetComponent<UISprite>().spriteName = "sound-off";
			button.normalSprite = "sound-off";
			button.hoverSprite = "sound-off_hov";
			button.pressedSprite = "sound-off_active";
		}
	}

	public void OpenHelp()
	{
		Application.ExternalEval("window.open('https://vk.com/magnatgamegroup','User Profile')");
	}
}
