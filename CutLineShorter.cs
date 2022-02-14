using TMPro;
using UnityEngine;

public class CutLineShorter : MonoBehaviour
{
	private TextMeshProUGUI myText;

	private void Start()
	{
		myText = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (myText.text.Length > 32)
		{
			Debug.Log("Chaning the text, it has length of:" + myText.text.Length);
			myText.text = myText.text.Substring(0, 13);
			myText.text += "..";
			Debug.Log("And now of:" + myText.text.Length);
		}
	}
}
