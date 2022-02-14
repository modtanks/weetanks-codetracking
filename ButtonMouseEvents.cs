using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMouseEvents : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public Texture NotSelected;

	public Texture Hovering;

	public Texture Selected;

	public RawImage myImage;

	public bool IsEnabled;

	public RawImage CheckMarkImage;

	public bool mouseOnMe;

	[HideInInspector]
	public int CustomTankID;

	private void Start()
	{
		if (myImage == null)
		{
			myImage = GetComponent<RawImage>();
		}
		myImage.texture = NotSelected;
	}

	public void DeselectButton()
	{
		myImage.texture = NotSelected;
	}

	private void Update()
	{
		if ((bool)CheckMarkImage)
		{
			if (!IsEnabled)
			{
				CheckMarkImage.gameObject.SetActive(value: false);
			}
			else
			{
				CheckMarkImage.gameObject.SetActive(value: true);
			}
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if ((bool)Selected)
		{
			myImage.texture = Selected;
			StartCoroutine(AutoDeselectButton());
		}
		if ((bool)CheckMarkImage)
		{
			SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MenuClick, 1f, null);
			if (!IsEnabled)
			{
				CheckMarkImage.gameObject.SetActive(value: true);
				IsEnabled = true;
			}
			else
			{
				CheckMarkImage.gameObject.SetActive(value: false);
				IsEnabled = false;
			}
		}
	}

	private void OnEnable()
	{
		if ((bool)myImage)
		{
			myImage.texture = NotSelected;
		}
	}

	private IEnumerator AutoDeselectButton()
	{
		myImage.texture = Selected;
		yield return new WaitForSeconds(0.01f);
		myImage.texture = Selected;
		yield return new WaitForSeconds(0.05f);
		if (mouseOnMe)
		{
			myImage.texture = Hovering;
		}
		else
		{
			myImage.texture = NotSelected;
		}
	}

	public void OnMouseOver(BaseEventData eventData)
	{
		mouseOnMe = true;
		myImage.texture = Hovering;
	}

	public void OnMouseExit(BaseEventData eventData)
	{
		mouseOnMe = false;
		myImage.texture = NotSelected;
	}

	public void MapEditorOpenTankMenu()
	{
		MapEditorMaster.instance.SelectedCustomTank = CustomTankID;
		MapEditorMaster.instance.ShowMenu(8);
	}
}
