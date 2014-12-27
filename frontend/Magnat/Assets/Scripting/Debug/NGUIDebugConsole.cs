using UnityEngine;
using System.Collections;
using System.Threading;

[RequireComponent(typeof(UITextList))]
public class NGUIDebugConsole : MonoBehaviour
{
	static UITextList _label;

	public static NGUIDebugConsole Instance;

	static NGUIDebugConsole()
	{
	}

	void Awake()
	{
		Instance = this;
		_label = GetComponent<UITextList>();
	}

	void Start()
	{
		LogSystem("Start");
	}

	public static void Log(string T)
	{
		Instance.LogText(T);
	}

	public static void LogSystem(string T)
	{
		//Instance.LogText("[System] : "+T);
	}

	private void LogText(string Text)
	{
		_label.Add(Text);
	}
}
