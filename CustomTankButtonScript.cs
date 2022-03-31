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
			TextMeshProUGUI Textchild2 = GetComponentInChildren<TextMeshProUGUI>();
			if (Textchild2.color != Selected)
			{
				Textchild2.color = Selected;
			}
		}
		else
		{
			TextMeshProUGUI Textchild = GetComponentInChildren<TextMeshProUGUI>();
			if (Textchild.color != NotSelected)
			{
				Textchild.color = NotSelected;
			}
		}
	}
}
