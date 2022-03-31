using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownMenu : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public Texture NotSelected;

	public Texture Hovering;

	private RawImage myImage;

	public bool IsEnabled = false;

	public bool mouseOnMe = false;

	public Texture Dropdown_active;

	public Transform Caret;

	private void Start()
	{
		myImage = GetComponent<RawImage>();
		myImage.texture = NotSelected;
	}

	public void DeselectButton()
	{
		myImage.texture = NotSelected;
	}

	public void OnSelect(BaseEventData eventData)
	{
		Caret.Rotate(180f, 0f, 0f, Space.World);
		if (!IsEnabled)
		{
			myImage.texture = Dropdown_active;
			myImage.SetNativeSize();
			IsEnabled = true;
		}
		else
		{
			myImage.texture = Hovering;
			myImage.SetNativeSize();
			IsEnabled = false;
		}
	}

	public void OnMouseOver(BaseEventData eventData)
	{
		mouseOnMe = true;
		if (!IsEnabled)
		{
			myImage.texture = Hovering;
		}
	}

	public void OnMouseExit(BaseEventData eventData)
	{
		mouseOnMe = false;
		if (!IsEnabled)
		{
			myImage.texture = NotSelected;
		}
	}
}
