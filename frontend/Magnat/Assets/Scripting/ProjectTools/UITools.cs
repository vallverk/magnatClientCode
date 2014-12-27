using UnityEngine;
using System.Collections;

public class UITools
{
	public static void RemoveChildrens(UIGrid grid)
	{
		var list = grid.GetChildList();
		if (list.Count!=0)
		{
			foreach (var t in list)
				NGUITools.Destroy(t.gameObject);
			list.Clear();
		}
	}

	public static void RemoveChildrens(UITable table)
	{
		var list = table.GetChildList();
		if (list.Count!=0)
		{
			foreach (var t in list)
				NGUITools.Destroy(t.gameObject);
			list.Clear();
		}
	}

	public static void FadeIn(GameObject Target, float Duration)
	{
		TweenAlpha tween = NGUITools.AddMissingComponent<TweenAlpha>(Target);
		tween.enabled = false;
		tween.from = 0;
		tween.to = 1;
		tween.style = UITweener.Style.Once;
		tween.duration = Duration;
		tween.PlayForward();
	}

	public static void FadeOut(GameObject Target, float Duration)
	{
		TweenAlpha tween = NGUITools.AddMissingComponent<TweenAlpha>(Target);
		tween.enabled = false;
		tween.from = 0;
		tween.to = 1;
		tween.style = UITweener.Style.Once;
		tween.duration = Duration;
		tween.PlayReverse();
	}

	public static Vector2 ResizeTo(Texture2D Tex, int TargetSize)
	{
		if (Tex!=null)
		{
			Vector2 size = new Vector2(Tex.width,Tex.height);
			float k = Mathf.Max(size.x,size.y);
			size *= TargetSize / k;
			return size;
		}
		return Vector2.zero;
	}
}
