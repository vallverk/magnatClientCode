using UnityEngine;
using System.Collections;

public class FlashingButton : UIButton 
{
	public float flashTime = 0.3f;
	public string mormalSprite2;
	public UnityEngine.Sprite mormalSprite22D;
	private bool canFlash;

	protected override void OnInit ()
	{
		base.OnInit ();
		canFlash = true;
		StartCoroutine(Flashing());
	}

	IEnumerator Flashing()
	{
		while (state == State.Normal)
		{
			SetSprite(normalSprite);
			yield return new WaitForSeconds(flashTime);
			if (state == State.Normal)
				SetSprite(mormalSprite2);
			yield return new WaitForSeconds(flashTime);
		}
	}

	protected override void OnDisable ()
	{
		canFlash = false;
		base.OnDisable ();
	}

	protected override void OnEnable ()
	{
		canFlash = true;
		base.OnEnable ();
	}

	public override void SetState (State state, bool immediate)
	{
		base.SetState (state, immediate);
		if (canFlash && state == State.Normal)
		{
			StopCoroutine("Flashing");
			StartCoroutine("Flashing");
		}
	}
}
