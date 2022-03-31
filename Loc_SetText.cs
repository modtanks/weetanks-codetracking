using System.Collections.Generic;
using Rewired;
using TMPro;
using UnityEngine;

public class Loc_SetText : MonoBehaviour
{
	[TextArea]
	public string TextAssetID;

	[TextArea]
	public string TextAddBefore;

	[TextArea]
	public string TextAddAfter;

	[TextArea]
	public string[] TextList;

	[Header("Custom Texts with inputs")]
	public bool IsMapEditorInstructions;

	private TextMeshProUGUI MyText;

	private int LastKnown = -1;

	public Vector2 OriginalSize;

	public Vector2 NewSize;

	public int AmountCharacters = -1;

	public int OriginalCharacters = -1;

	public float OriginalFontSize = -1f;

	private void Start()
	{
		MyText = GetComponent<TextMeshProUGUI>();
		if ((bool)MyText)
		{
			OriginalSize = MyText.GetRenderedValues(onlyVisibleCharacters: false);
			OriginalCharacters = MyText.text.Length;
			OriginalFontSize = MyText.fontSize;
			SetText();
		}
		else if (TextList.Length != 0)
		{
			SetText();
		}
		InvokeRepeating("CheckLang", 0.5f, 0.5f);
	}

	public void SetText()
	{
		LastKnown = LocalizationMaster.instance.CurrentLang;
		if (TextList.Length != 0)
		{
			TMP_Dropdown Dropdown = GetComponent<TMP_Dropdown>();
			if ((bool)Dropdown)
			{
				Dropdown.ClearOptions();
				List<TMP_Dropdown.OptionData> NewOptions = new List<TMP_Dropdown.OptionData>();
				string[] textList = TextList;
				foreach (string add in textList)
				{
					TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
					newOption.text = LocalizationMaster.instance.GetText(add);
					NewOptions.Add(newOption);
				}
				Dropdown.AddOptions(NewOptions);
				return;
			}
		}
		if (IsMapEditorInstructions)
		{
			string key = ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName;
			MyText.text = "Left click -> Add / Remove<br>Right click -> Rotate<br>" + key + " -> Edit properties<br>Shift -> Straight lines<br>'+' -> Layer Up<br>'-' -> Layer Down<br>1...5 -> Select Layer<br>0 -> Toggle All Layers";
			return;
		}
		MyText.text = TextAddBefore + LocalizationMaster.instance.GetText(TextAssetID) + TextAddAfter;
		AmountCharacters = MyText.text.Length;
		int increase = AmountCharacters - OriginalCharacters;
		if (increase > 2 && OriginalCharacters > 10)
		{
			float Percent = (float)increase / (float)OriginalCharacters;
			float PercentageIncrease = 1f - Percent / 2f;
			if (PercentageIncrease < 0.1f)
			{
				PercentageIncrease = 0.1f;
			}
			float newSize = OriginalFontSize * PercentageIncrease;
			if (newSize < OriginalFontSize / 1.6f)
			{
				newSize = OriginalFontSize / 1.6f;
			}
			MyText.fontSize = newSize;
		}
		else
		{
			MyText.fontSize = OriginalFontSize;
		}
	}

	private void CheckLang()
	{
		if (LastKnown != LocalizationMaster.instance.CurrentLang)
		{
			SetText();
		}
	}
}
