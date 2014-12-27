using UnityEngine;
using System.Collections;

public class WindowBehavoiur : MonoBehaviour 
{
	private UIPanel panel;
	private TweenAlpha panelTween;

	public static WindowBehavoiur current;

	protected virtual void Start()
	{
		panel = GetComponent<UIPanel>();
		if (panel == null)
		{
			UIWidget w = GetComponent<UIWidget>();
			if (w!=null)
				panel = w.panel;
			else
				Debug.LogError(name + " - can't find panel for window...");
		}
		if (panel != null)
		{
			panelTween = NGUITools.AddMissingComponent<TweenAlpha>(panel.gameObject);
			if (panelTween)
			{
				panelTween.enabled = false;
				panelTween.from = 0;
				panelTween.to = 1;
				panelTween.duration = 0.3f;
				panelTween.style = UITweener.Style.Once;
			}
			panel.alpha = 0;
		}
	}

	public virtual void Show()
	{
		if (panelTween && current == null)
		{
			current = this;
			panelTween.PlayForward();
		}
	}

	public virtual void ShowImmediately()
	{
		if (current!=null)
		{
			Hide();
			current = null;
		}
		Show();
	}

	public virtual void ShowAtTop()
	{
		if (panelTween)
			panelTween.PlayForward();
	}

	public virtual void Hide()
	{
		if (panelTween)
		{
			if (current == this)
				current = null;
			panelTween.PlayReverse();
		}
	}
}
