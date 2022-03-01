using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMouseEvents : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	[Header("Textures, if you use RawImage")]
	public Texture NotSelected;

	public Texture Hovering;

	public Texture Selected;

	[Header("Sprites, if you use Image")]
	public Sprite NotSelected_sprite;

	public Sprite Hovering_sprite;

	public Sprite Selected_sprite;

	public RawImage myImage;

	public Image myImage_img;

	public bool IsEnabled;

	public RawImage CheckMarkImage;

	public bool mouseOnMe;

	public ScrollRect ParentSR;

	[HideInInspector]
	public int CustomTankID;

	private void Start()
	{
		if (myImage == null)
		{
			myImage = GetComponent<RawImage>();
		}
		if (myImage == null)
		{
			myImage_img = GetComponent<Image>();
		}
		SetSpriteTexture(NotSelected, NotSelected_sprite);
		if ((bool)base.transform.parent && (bool)base.transform.parent.transform.parent && (bool)base.transform.parent.transform.parent.transform.parent)
		{
			ParentSR = base.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<ScrollRect>();
		}
	}

	public void DeselectButton()
	{
		SetSpriteTexture(NotSelected, NotSelected_sprite);
	}

	public void OnScroll()
	{
		if ((bool)ParentSR)
		{
			ParentSR.verticalNormalizedPosition += Input.mouseScrollDelta.y / 16f;
		}
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
			SetSpriteTexture(Selected, Selected_sprite);
			StartCoroutine(AutoDeselectButton());
		}
		if ((bool)CheckMarkImage)
		{
			SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MenuClick);
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
		SetSpriteTexture(NotSelected, NotSelected_sprite);
	}

	private IEnumerator AutoDeselectButton()
	{
		SetSpriteTexture(Selected, Selected_sprite);
		yield return new WaitForSeconds(0.01f);
		SetSpriteTexture(Selected, Selected_sprite);
		yield return new WaitForSeconds(0.05f);
		if (mouseOnMe)
		{
			SetSpriteTexture(Hovering, Hovering_sprite);
		}
		else
		{
			SetSpriteTexture(NotSelected, NotSelected_sprite);
		}
	}

	private void SetSpriteTexture(Texture tex, Sprite spr)
	{
		if ((bool)myImage_img)
		{
			myImage_img.sprite = spr;
		}
		else if ((bool)myImage)
		{
			myImage.texture = tex;
		}
	}

	public void OnMouseOver(BaseEventData eventData)
	{
		mouseOnMe = true;
		SetSpriteTexture(Hovering, Hovering_sprite);
	}

	public void OnMouseExit(BaseEventData eventData)
	{
		mouseOnMe = false;
		SetSpriteTexture(NotSelected, NotSelected_sprite);
	}

	public void MapEditorOpenTankMenu()
	{
		MapEditorMaster.instance.SelectedCustomTank = CustomTankID;
		MapEditorMaster.instance.ShowMenu(8);
	}
}
