using TMPro;
using UnityEngine;

public class SetShadowText : MonoBehaviour
{
	public bool isRunning;

	private TextMeshProUGUI myText;

	private TextMeshProUGUI shadowText;

	private void Start()
	{
		myText = GetComponent<TextMeshProUGUI>();
		shadowText = base.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		isRunning = true;
		if ((bool)shadowText)
		{
			shadowText.text = myText.text;
		}
	}
}
