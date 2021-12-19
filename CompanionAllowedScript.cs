using TMPro;
using UnityEngine;

public class CompanionAllowedScript : MonoBehaviour
{
	public void MouseEntered()
	{
		Debug.Log("ENTERED");
		if ((bool)GameMaster.instance.CursorText)
		{
			GameMaster.instance.CursorText.GetComponent<TextMeshProUGUI>().text = "Companion Allowed";
		}
	}

	public void MouseExit()
	{
		Debug.Log("EXITED");
		if ((bool)GameMaster.instance.CursorText)
		{
			GameMaster.instance.CursorText.GetComponent<TextMeshProUGUI>().text = "";
		}
	}
}
