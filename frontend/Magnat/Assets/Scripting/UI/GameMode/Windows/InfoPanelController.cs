using UnityEngine;
using System.Collections;

public class InfoPanelController : MonoBehaviour 
{
	public GameDataManager DataManager;
	public GameObject CityInfoGO;
	public GameObject CompanyInfoGO;
	public float ShowTimeout = 1;
	public float HideTimeout = 0.2f;
	public float ShowDuration = 0.7f;
	public float THSize = 530;
	public static bool CanShow;

	private InfoPanel cityInfo;
	private InfoPanel companyInfo;
	private bool isStarted = false;
	private bool isShowed = false;
	private UIPanel panel;

	private float targetAlpha = 0;
	private float cAlpha = 0;
	private bool animate = false;

	void Start()
	{
		CanShow = true;
		cityInfo = CityInfoGO.GetComponent<InfoPanel>();
		companyInfo = CompanyInfoGO.GetComponent<InfoPanel>();
		isStarted = true;

		panel = GetComponent<UIPanel>();
		panel.alpha = 0;
		StartCoroutine("Loop");
	}

	void OnDestroy()
	{
		StopCoroutine("Loop");
	}

	public void Show()
	{
		if (isStarted && CanShow)
		{
			FieldData data = DataManager.GetFieldData(UIButton.current.transform.parent.GetComponent<GameField>());
			bool city = DataManager.IsCity(data);
			CityInfoGO.SetActive(city);
			CompanyInfoGO.SetActive(!city);
			if (city)
			{
				UpdateInfo(cityInfo,data);
				UpdatePosition(UIButton.current.transform.GetComponent<UIWidget>(),CityInfoGO.GetComponent<UIWidget>());
			}
			else
			{
				UpdateInfo(companyInfo,data);
				UpdatePosition(UIButton.current.transform.GetComponent<UIWidget>(),CompanyInfoGO.GetComponent<UIWidget>());
			}
			UpdateAnim(1);
		}
	}

	public void Hide()
	{ 
		if (isStarted)
			UpdateAnim(0);
	}

	private void UpdatePosition(UIWidget Field, UIWidget Panel)
	{
		Transform table = Field.panel.transform;
		Vector3 fieldPos = table.InverseTransformPoint(Field.transform.position);
		float x = fieldPos.x;
		if (Mathf.Abs(fieldPos.x) + Panel.width / 2 > THSize)
			x = (THSize - Panel.width/2) * Mathf.Sign(fieldPos.x);
		float y = fieldPos.y;
		if (Mathf.Abs(fieldPos.y) +  Panel.height/2 > THSize)
			y = (THSize - Panel.height/2) * Mathf.Sign(fieldPos.y);
		Panel.transform.position = table.TransformPoint(x,y,0);
	}

	private void UpdateInfo(InfoPanel ipanel, FieldData data)
	{
		if (ipanel.FieldName != null)
			ipanel.FieldName.text = data.FieldName.ToString();
		if (ipanel.BuyPrice != null)
			ipanel.BuyPrice.text = data.BuyPrice.ToString("$### ### ##0");
		if (ipanel.MislayPrice != null)
			ipanel.MislayPrice.text = data.MislayPrice.ToString("$### ### ##0");
		if (ipanel.BuyOutPrice != null)
			ipanel.BuyOutPrice.text = data.BuyOutPrice.ToString("$### ### ##0");
		if (ipanel.BuildPrice != null)
			ipanel.BuildPrice.text = data.BuildPrice.ToString("$### ### ##0");
		if (ipanel.MBuildPrice != null)
			ipanel.MBuildPrice.text = data.MBuildPrice.ToString("$### ### ##0");
		if (ipanel.BasePrice != null)
			ipanel.BasePrice.text = data.BasePrice.ToString("$### ### ##0");
		if (ipanel.MonopolyCost != null)
			ipanel.MonopolyCost.text = data.MonopolyCost.ToString("$### ### ##0");
		if (ipanel.Branch1Cost != null)
			ipanel.Branch1Cost.text = data.Branch1Cost.ToString("$### ### ##0");
		if (ipanel.Branch2Cost != null)
			ipanel.Branch2Cost.text = data.Branch2Cost.ToString("$### ### ##0");
		if (ipanel.Branch3Cost != null)
			ipanel.Branch3Cost.text = data.Branch3Cost.ToString("$### ### ##0");
		if (ipanel.Branch4Cost != null)
			ipanel.Branch4Cost.text = data.Branch4Cost.ToString("$### ### ##0");
		if (ipanel.HoldingCost != null)
			ipanel.HoldingCost.text = data.HoldingCost.ToString("$### ### ##0");
	}

	private void UpdateAnim(float targetA)
	{
		StopCoroutine("Anim");
		animate = false;
		if (targetA == 1 && targetAlpha != 0)
			animate = true;
		else
			StartCoroutine("Anim");
		targetAlpha = targetA; 
	}

	private IEnumerator Anim()
	{
		yield return new WaitForSeconds(targetAlpha == 0? HideTimeout : ShowTimeout);
		animate = true;
	}

	private IEnumerator Loop()
	{
		while(true)
		{
			if (animate)
			{
				float way = targetAlpha - cAlpha;
				if (way!=0)
				{
					cAlpha += Mathf.Sign(way)*(1.0f/ShowDuration)*Time.fixedDeltaTime;
					cAlpha = Mathf.Clamp01(cAlpha);
					panel.alpha = cAlpha;
				} 
			}
			yield return new WaitForFixedUpdate();
		}
	}
}
