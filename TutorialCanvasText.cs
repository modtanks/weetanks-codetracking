using TMPro;
using UnityEngine;

public class TutorialCanvasText : MonoBehaviour
{
	public bool isMineText;

	public bool isBoostText;

	private void Start()
	{
		if (isMineText)
		{
			string text = ((OptionsMainMenu.instance.keys["mineKey"].ToString() == "Mouse1") ? "Right Click" : OptionsMainMenu.instance.keys["mineKey"].ToString());
			GetComponent<TextMeshProUGUI>().text = "Press " + text + " to place a mine";
		}
		else if (isBoostText)
		{
			string text2 = ((OptionsMainMenu.instance.keys["boostKey"].ToString() == "Mouse1") ? "Right Click" : OptionsMainMenu.instance.keys["boostKey"].ToString());
			GetComponent<TextMeshProUGUI>().text = "Press " + text2 + " to use the boost";
		}
	}

	private void Update()
	{
	}
}
