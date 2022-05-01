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
			string name2 = ((OptionsMainMenu.instance.keys["mineKey"].ToString() == "Mouse1") ? "Right Click" : OptionsMainMenu.instance.keys["mineKey"].ToString());
			GetComponent<TextMeshProUGUI>().text = "Press " + name2 + " to place a mine";
		}
		else if (isBoostText)
		{
			string name = ((OptionsMainMenu.instance.keys["boostKey"].ToString() == "Mouse1") ? "Right Click" : OptionsMainMenu.instance.keys["boostKey"].ToString());
			GetComponent<TextMeshProUGUI>().text = "Press " + name + " to use the boost";
		}
	}

	private void Update()
	{
	}
}
