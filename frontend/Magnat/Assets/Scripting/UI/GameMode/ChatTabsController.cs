using UnityEngine;
using System.Collections;

public class ChatTabsController : MonoBehaviour 
{
	public UIButton MainChatButton;
	public UIButton[] PrivatChatButtons;
	private string[] uids;
	private int[] newMessagesCount;
	public GameObject[] ChatsLists;
	private UILabel[] privateChatWigets;
	private UITextList[] privateChatLists;

	public Color BlueChatColor;
	public Color RedChatColor;
	public Color GreenChatColor;
	public Color PurpleChatColor;
	public Color OrangeChatColor;

	private int ActiveChat = 0;
	private int chatCount;

	public System.Action<string,string> OnChatSubmint;
	public UIInput ChatInput;

	public void OnSubmint()
	{
		if (ChatInput!=null)
		{
			OnChatSubmint(uids[ActiveChat],ChatInput.value);
			ChatInput.value = "";
		}
	}

	void Start()
	{
		privateChatLists = new UITextList[ChatsLists.Length];
		privateChatWigets = new UILabel[ChatsLists.Length];
		for (int i =0;i<ChatsLists.Length;i++)
		{
			privateChatLists[i] = ChatsLists[i].GetComponent<UITextList>();
			privateChatWigets[i] = ChatsLists[i].GetComponent<UILabel>();
		}
	}

	public void OnMainClick()
	{
		SetActiveChat(0);
	}

	public void OnFirstClick()
	{
		SetActiveChat(1);
	}

	public void OnSecondClick()
	{
		SetActiveChat(2);
	}

	public void OnThirdClick()
	{
		SetActiveChat(3);
	}

	public void OnFrouthClick()
	{
		SetActiveChat(4);
	}

	public string MakeColoredBB(GameField.Owners Owner, string Text)
	{
		Color c = GetOwnerColor(Owner);
		return "["+ColorTools.ColorToHex(c)+"]"+Text+"[-]";
	}

	public void AddMessage(int ChatID,string Message)
	{
		privateChatLists[ChatID].Add(Message);
		if (ActiveChat != ChatID) 
		{
			newMessagesCount[ChatID]++;
			UpdateChatNames();
		}
	}

	public void AddMessageByChatName(string ChatName,string Message)
	{
		int id = 0;
		for (int i=0;i<uids.Length;i++)
			if (uids[i] == ChatName)
		{
			id = i;
			break;
		}
		AddMessage(id,Message);
	}

	private void SetActiveChat(int ChatNum)
	{
		if (ChatNum>chatCount) return;
		newMessagesCount[ChatNum] = 0;
		UpdateChatNames();
		ActiveChat = ChatNum;
		for (int i=0;i<PrivatChatButtons.Length;i++)
		{
			PrivatChatButtons[i].enabled = true;
			PrivatChatButtons[i].SetState(UIButtonColor.State.Normal,true);
		}
		foreach (var wig in privateChatWigets)
			if (wig!=null)
				wig.gameObject.SetActive(false);

		if (ChatNum>0)
		{
			PrivatChatButtons[ChatNum-1].enabled = false;
			PrivatChatButtons[ChatNum-1].SetState(UIButtonColor.State.Disabled,true);
		}
		privateChatWigets[ChatNum].gameObject.SetActive(true);
	}

	private Color MulColor(Color col, float num)
	{
		col*=num;
		col.a = 1;
		return col;
	}

	private void UpdateChatNames()
	{
		for (int i=1;i<uids.Length;i++)
		{
			UILabel l = PrivatChatButtons[i-1].transform.GetComponentInChildren<UILabel>();
			if (l==null) throw new System.Exception("Не найден компонент UILabel id = "+i.ToString());
#if UNITY_EDITOR
			string name = "Игрок"+i.ToString();
#else
			if (!SocialManager.Instance.SocialData.ContainsKey(uids[i]))
				throw new System.Exception("Не найден id = '"+uids[i]+"' в базе пользователей");
			string name = SocialManager.Instance.SocialData[uids[i]].FirstName;
#endif
			if (newMessagesCount[i]>0)
				name += " ("+newMessagesCount[i]+")";
			l.text = name;
		}

	}

	public void Init(Player[] Ps)
	{
		chatCount = Ps.Length;
		uids = new string[Ps.Length+1];
		newMessagesCount = new int[Ps.Length+1];
		for (int i =0;i<newMessagesCount.Length;i++)
			newMessagesCount[i]=0;
		uids[0] = "Main";

		for (int i=0;i<4;i++)
		{
			if (i<Ps.Length)
			{
				UILabel l =	PrivatChatButtons[i].transform.GetComponentInChildren<UILabel>();
				Color col = GetOwnerColor(Ps[i].OwnerID);
				l.text = Ps[i].FirstName;
				PrivatChatButtons[i].hover = MulColor(col,1.1f);
				PrivatChatButtons[i].disabledColor = MulColor(col,0.7f);
				PrivatChatButtons[i].pressed = col;
				PrivatChatButtons[i].defaultColor = col;
				PrivatChatButtons[i].UpdateColor(true);
				uids[i+1]=Ps[i].SocialID;
			} else
				PrivatChatButtons[i].gameObject.SetActive(false);
		}

		SetActiveChat(0);
	}

	private Color GetOwnerColor(GameField.Owners Owner)
	{
		switch (Owner)
		{
		case GameField.Owners.Blue:
			return BlueChatColor;
		case GameField.Owners.Green:
			return GreenChatColor;
		case GameField.Owners.Orange:
			return OrangeChatColor;
		case GameField.Owners.Purple:
			return PurpleChatColor;
		case GameField.Owners.Red:
			return RedChatColor;
		default:
			return Color.white;
		}
	}
}
