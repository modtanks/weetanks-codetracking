using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeHappinessButton : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public BugReportingMenu BPM;

	public int ButtonID;

	public Texture NotSelected;

	public Texture Hovering;

	public Texture Selected;

	public RawImage myImage;

	public bool IsSelected;

	private void Start()
	{
		myImage = GetComponent<RawImage>();
		myImage.texture = NotSelected;
	}

	public void DeselectButton()
	{
		IsSelected = false;
		myImage.texture = NotSelected;
	}

	public void OnSelect(BaseEventData eventData)
	{
		BPM.SelectedButton(ButtonID);
		IsSelected = true;
		myImage.texture = Selected;
	}

	public void OnMouseOver(BaseEventData eventData)
	{
		if (!IsSelected)
		{
			myImage.texture = Hovering;
		}
	}

	public void OnMouseExit(BaseEventData eventData)
	{
		if (!IsSelected)
		{
			myImage.texture = NotSelected;
		}
	}
}
