using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMouseEvents : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public int Place = 0;

	public bool IsRadioButton = false;

	public bool TurnChildTextGreen = false;

	public Color TextSelectedColor;

	public Color TextNormalColor;

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

	public bool IsEnabled = false;

	public RawImage CheckMarkImage;

	public bool mouseOnMe = false;

	public ScrollRect ParentSR;

	public NewMenuControl NMC;

	private TMP_Dropdown MyDropdown;

	[HideInInspector]
	public int CustomTankID = 0;

	private void Start()
	{
		if (myImage == null)
		{
			myImage = GetComponent<RawImage>();
		}
		if (myImage_img == null)
		{
			myImage_img = GetComponent<Image>();
		}
		GameObject N = GameObject.Find("Canvas");
		if ((bool)N)
		{
			NMC = N.GetComponent<NewMenuControl>();
		}
		SetSpriteTexture(NotSelected, NotSelected_sprite);
		MyDropdown = GetComponent<TMP_Dropdown>();
		if ((bool)base.transform.parent && (bool)base.transform.parent.transform.parent && (bool)base.transform.parent.transform.parent.transform.parent)
		{
			ParentSR = base.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<ScrollRect>();
		}
	}

	public void DeselectButton()
	{
		SetSpriteTexture(NotSelected, NotSelected_sprite);
		if (!IsRadioButton)
		{
			return;
		}
		CheckMarkImage.gameObject.SetActive(value: false);
		IsEnabled = false;
		if (!TurnChildTextGreen)
		{
			return;
		}
		foreach (Transform t in base.transform)
		{
			TextMeshProUGUI ChildText = t.GetComponent<TextMeshProUGUI>();
			if ((bool)ChildText)
			{
				ChildText.color = TextNormalColor;
			}
		}
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
		if ((bool)NMC && NMC.player != null)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				Debug.LogError("NOT ACTIVE IN HIERARCHY!!");
				return;
			}
			if (NMC.Selection == Place && NMC.CanDoSomething && (NMC.player.GetButtonUp("Menu Use") || NMC.player.GetButtonUp("Use")) && (bool)MyDropdown)
			{
				if (MyDropdown.options.Count == MyDropdown.value + 1)
				{
					MyDropdown.value = 0;
				}
				else
				{
					MyDropdown.value += 1;
				}
			}
		}
		if (!NMC || !(NMC != null))
		{
			return;
		}
		if (NMC.Selection != Place || NMC.IsUsingMouse)
		{
		}
		if (NMC.player != null && NMC.Selection == Place)
		{
			if (!NMC.IsUsingMouse)
			{
				OnMouseOver(null);
			}
			if (NMC.player.GetAxis("Move Vertically") > 0f)
			{
				if (NMC.Selection > 0 && NMC.CanMove)
				{
					NMC.StartCoroutine(NMC.MoveSelection(up: true));
				}
			}
			else if (NMC.player.GetAxis("Move Vertically") < 0f && NMC.CanMove)
			{
				NMC.StartCoroutine(NMC.MoveSelection(up: false));
			}
		}
		else if (NMC.Selection != Place && !NMC.IsUsingMouse)
		{
			OnMouseExit(null);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (IsRadioButton && IsEnabled)
		{
			return;
		}
		if ((bool)Selected)
		{
			SetSpriteTexture(Selected, Selected_sprite);
			StartCoroutine(AutoDeselectButton());
		}
		if (TurnChildTextGreen)
		{
			foreach (Transform t in base.transform)
			{
				TextMeshProUGUI ChildText = t.GetComponent<TextMeshProUGUI>();
				if ((bool)ChildText)
				{
					ChildText.color = TextSelectedColor;
				}
			}
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
