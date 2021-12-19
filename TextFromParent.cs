using TMPro;
using UnityEngine;

public class TextFromParent : MonoBehaviour
{
	public TextMeshProUGUI Other_Text;

	private TextMeshProUGUI my_Text;

	public string MissionName = "Mission 500";

	private void Start()
	{
		my_Text = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		my_Text.text = MissionName;
		Other_Text.text = MissionName;
	}
}
