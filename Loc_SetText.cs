using Rewired;
using TMPro;
using UnityEngine;

public class Loc_SetText : MonoBehaviour
{
	[TextArea]
	public string Text_English;

	[Header("Custom Texts with inputs")]
	public bool IsMapEditorInstructions;

	private TextMeshProUGUI MyText;

	private void Start()
	{
		MyText = GetComponent<TextMeshProUGUI>();
		if ((bool)MyText)
		{
			SetText();
		}
	}

	public void SetText()
	{
		if (IsMapEditorInstructions)
		{
			string elementIdentifierName = ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName;
			MyText.text = "Left click -> Add / Remove<br>Right click -> Rotate<br>" + elementIdentifierName + " -> Edit properties<br>Shift -> Straight lines<br>'+' -> Layer Up<br>'-' -> Layer Down<br>1...5 -> Select Layer<br>0 -> Toggle All Layers";
		}
	}

	private void Update()
	{
	}
}
