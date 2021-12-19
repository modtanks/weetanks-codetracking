using TMPro;
using UnityEngine;

public class ChangeTextBasedOnInput : MonoBehaviour
{
	[TextArea]
	public string ControllerText;

	[TextArea]
	public string PCText;

	private TextMeshProUGUI myText;

	private void Start()
	{
		myText = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if ((bool)myText)
		{
			if (myText.text != ControllerText && GameMaster.instance.isPlayingWithController)
			{
				myText.text = ControllerText;
			}
			else if (myText.text != PCText && !GameMaster.instance.isPlayingWithController)
			{
				myText.text = PCText;
			}
		}
	}
}
