using TMPro;
using UnityEngine;

public class TextMeshProUnderlineBlinking : MonoBehaviour
{
	private TextMeshProUGUI myText;

	private void Start()
	{
		myText = GetComponent<TextMeshProUGUI>();
		InvokeRepeating("BlinkingText", 0.5f, 0.5f);
	}

	private void BlinkingText()
	{
		bool isBold = (myText.fontStyle & FontStyles.Underline) != 0;
		if (!GameMaster.instance.GameHasStarted)
		{
			if (isBold)
			{
				myText.fontStyle ^= FontStyles.Underline;
			}
			else
			{
				myText.fontStyle = FontStyles.Underline;
			}
		}
	}
}
