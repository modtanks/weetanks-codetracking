using TMPro;
using UnityEngine;

public class ToolTipLoading : MonoBehaviour
{
	public bool ClassicCampaign;

	public bool PlayCustomCampaign;

	public bool CreateMap;

	public bool Survival;

	public int ContinueCheckpoint;

	[TextArea]
	public string[] ClassicCampaignTexts;

	[TextArea]
	public string[] PlayCustomCampaigns;

	[TextArea]
	public string[] CreateMapEditor;

	[TextArea]
	public string[] SurvivalTexts;

	public TextMeshProUGUI ToolTipText;

	private void Start()
	{
		ChangeToolTipText();
		InvokeRepeating("ChangeToolTipText", 2f, 2f);
	}

	private void ChangeToolTipText()
	{
		if (ClassicCampaign)
		{
			int maxExclusive = ContinueCheckpoint + 10;
			string text = "";
			do
			{
				text = ClassicCampaignTexts[Random.Range(ContinueCheckpoint, maxExclusive)];
			}
			while (text == "");
			ToolTipText.text = text;
		}
		else if (PlayCustomCampaign)
		{
			string text2 = "";
			do
			{
				text2 = PlayCustomCampaigns[Random.Range(0, PlayCustomCampaigns.Length)];
			}
			while (text2 == "");
			ToolTipText.text = text2;
		}
		else if (CreateMap)
		{
			string text3 = "";
			do
			{
				text3 = CreateMapEditor[Random.Range(0, CreateMapEditor.Length)];
			}
			while (text3 == "");
			ToolTipText.text = text3;
		}
		else if (Survival)
		{
			string text4 = "";
			do
			{
				text4 = SurvivalTexts[Random.Range(0, SurvivalTexts.Length)];
			}
			while (text4 == "");
			ToolTipText.text = text4;
		}
	}
}
