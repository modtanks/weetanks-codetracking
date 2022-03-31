using TMPro;
using UnityEngine;

public class ToolTipLoading : MonoBehaviour
{
	public bool ClassicCampaign = false;

	public bool PlayCustomCampaign = false;

	public bool CreateMap = false;

	public bool Survival = false;

	public int ContinueCheckpoint = 0;

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
			int index = ContinueCheckpoint + 10;
			string text4 = "";
			do
			{
				text4 = ClassicCampaignTexts[Random.Range(ContinueCheckpoint, index)];
			}
			while (text4 == "");
			ToolTipText.text = LocalizationMaster.instance.GetText(text4);
		}
		else if (PlayCustomCampaign)
		{
			string text3 = "";
			do
			{
				text3 = PlayCustomCampaigns[Random.Range(0, PlayCustomCampaigns.Length)];
			}
			while (text3 == "");
			ToolTipText.text = LocalizationMaster.instance.GetText(text3);
		}
		else if (CreateMap)
		{
			string text2 = "";
			do
			{
				text2 = CreateMapEditor[Random.Range(0, CreateMapEditor.Length)];
			}
			while (text2 == "");
			ToolTipText.text = LocalizationMaster.instance.GetText(text2);
		}
		else if (Survival)
		{
			string text = "";
			do
			{
				text = SurvivalTexts[Random.Range(0, SurvivalTexts.Length)];
			}
			while (text == "");
			ToolTipText.text = LocalizationMaster.instance.GetText(text);
		}
	}
}
