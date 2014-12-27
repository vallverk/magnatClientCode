using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ButtonEx : UIButton
{
	public Action<ButtonEx> EventNormal = (a) => {};
	public Action<ButtonEx> EventHover = (a) => {};
	public Action<ButtonEx> EventClick = (a) => {};

	protected override void OnClick ()
	{
		base.OnClick ();
		EventClick(this);
	}

	public override void SetState (State state, bool immediate)
	{
		if (state != mState && state == State.Normal)
			EventNormal(this);
		if (state != mState && state == State.Hover)
			EventHover(this);

		if (!mInitDone)
		{
			mInitDone = true;
			OnInit();
		}
		
		if (mState != state)
		{
			mState = state;
		}

		//base.SetState(state,immediate);
	}

	public void UpdateSprite(State state)
	{
		switch (state)
		{
			case State.Hover: SetSprite(hoverSprite); break;
			case State.Pressed: SetSprite(pressedSprite); break;
			case State.Disabled: SetSprite(disabledSprite); break;
		}
	}

}
