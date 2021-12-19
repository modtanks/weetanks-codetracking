using TMPro;
using UnityEngine;

public class SetCursorText : MonoBehaviour
{
	[TextArea]
	public string TextToShow = "";

	public void MouseEntered()
	{
		Debug.Log("ENTERED");
		if ((bool)GameMaster.instance.CursorText)
		{
			GameMaster.instance.CursorText.GetComponent<TextMeshProUGUI>().text = TextToShow;
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
