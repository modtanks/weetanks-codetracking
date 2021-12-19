using TMPro;
using UnityEngine;

public class CustomTankButtonScript : MonoBehaviour
{
	public int CustomTankID;

	public Color Selected;

	public Color NotSelected;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(MapEditorMaster.instance != null))
		{
			return;
		}
		if (MapEditorMaster.instance.SelectedCustomTank == CustomTankID)
		{
			TextMeshProUGUI componentInChildren = GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren.color != Selected)
			{
				componentInChildren.color = Selected;
			}
		}
		else
		{
			TextMeshProUGUI componentInChildren2 = GetComponentInChildren<TextMeshProUGUI>();
			if (componentInChildren2.color != NotSelected)
			{
				componentInChildren2.color = NotSelected;
			}
		}
	}
}
