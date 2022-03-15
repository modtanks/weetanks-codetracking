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
			TMP_Dropdown component = GetComponent<TMP_Dropdown>();
			if ((bool)component)
			{
				component.ClearOptions();
				List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
				string[] textList = TextList;
				foreach (string varName in textList)
				{
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
					optionData.text = LocalizationMaster.instance.GetText(varName);
					list.Add(optionData);
				}
				component.AddOptions(list);
				return;
			}
		}
		if (IsMapEditorInstructions)
		{
			string elementIdentifierName = ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName;
			MyText.text = "Left click -> Add / Remove<br>Right click -> Rotate<br>" + elementIdentifierName + " -> Edit properties<br>Shift -> Straight lines<br>'+' -> Layer Up<br>'-' -> Layer Down<br>1...5 -> Select Layer<br>0 -> Toggle All Layers";
			return;
		}
		MyText.text = TextAddBefore + LocalizationMaster.instance.GetText(TextAssetID) + TextAddAfter;
		AmountCharacters = MyText.text.Length;
		int num = AmountCharacters - OriginalCharacters;
		if (num > 2 && OriginalCharacters > 10)
		{
			Debug.Log("INCREASE!!" + num + " AND " + OriginalCharacters);
			float num2 = (float)num / (float)OriginalCharacters;
			Debug.Log("percentage!!" + num2);
			float num3 = 1f - num2 / 2f;
			if (num3 < 0.1f)
			{
				num3 = 0.1f;
			}
			float num4 = OriginalFontSize * num3;
			if (num4 < OriginalFontSize / 1.6f)
			{
				num4 = OriginalFontSize / 1.6f;
			}
			MyText.fontSize = num4;
			Debug.Log(num3 + num2 + " and new font size is : " + MyText.fontSize);
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
