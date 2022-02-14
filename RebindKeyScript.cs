using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebindKeyScript : MonoBehaviour
{
	public bool IsSelected;

	public bool IsXbox;

	public bool IsMoveUp;

	public bool IsMoveDown;

	public bool IsMoveLeft;

	public bool IsMoveRight;

	public bool IsKeyChangeMine;

	public bool IsKeyChangeBoost;

	public bool IsKeyChangeShoot;

	public bool IsHUD;

	public TextMeshProUGUI KeyBindKey;

	public TextMeshProUGUI KeyBindText;

	public TextMeshProUGUI KeyBindDescription;

	private Event keyEvent;

	public NewMenuControl NMC;

	public Button ThisButton;

	private KeyCode newKey;

	public AudioClip ErrorSound;

	public AudioClip SuccesSound;

	public Transform originalParent;

	public int myMenu;

	public Vector3 scale;

	private void Start()
	{
		base.transform.localScale = scale;
		base.transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (originalParent == null)
		{
			originalParent = base.transform.parent;
		}
		base.transform.SetParent(null);
		ThisButton = GetComponent<Button>();
		ThisButton.onClick.AddListener(ThisButtonClicked);
		GetKeyData();
		if (IsKeyChangeMine)
		{
			SetText("Lay Mine", "Laying mines key");
		}
		else if (IsKeyChangeBoost)
		{
			SetText("Boost", "Boost the tank");
		}
		else if (IsKeyChangeShoot)
		{
			SetText("Fire", "Fire the tank");
		}
		else if (IsMoveUp)
		{
			SetText("Move Up", "Key to press to move up");
		}
		else if (IsMoveRight)
		{
			SetText("Move Right", "Key to press to move right");
		}
		else if (IsMoveDown)
		{
			SetText("Move Down", "Key to press to move down");
		}
		else if (IsMoveLeft)
		{
			SetText("Move Left", "Key to press to move left");
		}
		else if (IsHUD)
		{
			SetText("Toggle HUD", "Key to press toggle HUD");
		}
	}

	private void ThisButtonClicked()
	{
		if (NMC.selectedRKS == this)
		{
			NMC.selectedRKS = null;
			return;
		}
		NMC.selectedRKS = this;
		StartCoroutine(blinkingMarker());
	}

	private IEnumerator blinkingMarker()
	{
		KeyBindKey.text = "___";
		yield return new WaitForSeconds(0.25f);
		KeyBindKey.text = "";
		yield return new WaitForSeconds(0.25f);
		KeyBindKey.text = "___";
		if (NMC.selectedRKS == this)
		{
			StartCoroutine(blinkingMarker());
		}
		else
		{
			GetKeyData();
		}
	}

	private void GetKeyData()
	{
		if (IsKeyChangeMine)
		{
			if (IsXbox)
			{
				string text = OptionsMainMenu.instance.keys["mineKey"].ToString();
				KeyBindKey.text = "(" + text + ")";
			}
			else
			{
				string text2 = OptionsMainMenu.instance.keys["mineKey"].ToString();
				KeyBindKey.text = "(" + text2 + ")";
			}
		}
		else if (IsKeyChangeBoost)
		{
			if (IsXbox)
			{
				string text3 = OptionsMainMenu.instance.keys["boostKey"].ToString();
				KeyBindKey.text = "(" + text3 + ")";
			}
			else
			{
				string text4 = OptionsMainMenu.instance.keys["boostKey"].ToString();
				KeyBindKey.text = "(" + text4 + ")";
			}
		}
		else if (IsKeyChangeShoot)
		{
			if (IsXbox)
			{
				string text5 = OptionsMainMenu.instance.keys["shootKey"].ToString();
				KeyBindKey.text = "(" + text5 + ")";
			}
			else
			{
				string text6 = OptionsMainMenu.instance.keys["shootKey"].ToString();
				KeyBindKey.text = "(" + text6 + ")";
			}
		}
		else if (IsMoveDown)
		{
			string text7 = OptionsMainMenu.instance.keys["downKey"].ToString();
			KeyBindKey.text = "(" + text7 + ")";
		}
		else if (IsMoveUp)
		{
			string text8 = OptionsMainMenu.instance.keys["upKey"].ToString();
			KeyBindKey.text = "(" + text8 + ")";
		}
		else if (IsMoveLeft)
		{
			string text9 = OptionsMainMenu.instance.keys["leftKey"].ToString();
			KeyBindKey.text = "(" + text9 + ")";
		}
		else if (IsMoveRight)
		{
			string text10 = OptionsMainMenu.instance.keys["rightKey"].ToString();
			KeyBindKey.text = "(" + text10 + ")";
		}
		else if (IsHUD)
		{
			string text11 = OptionsMainMenu.instance.keys["hudKey"].ToString();
			KeyBindKey.text = "(" + text11 + ")";
		}
	}

	private void SetText(string text, string description)
	{
		KeyBindText.text = text;
		KeyBindDescription.text = description;
	}

	private void Update()
	{
		if (NMC.ControlsOpenMenu == myMenu)
		{
			base.transform.SetParent(originalParent);
			base.transform.localScale = scale;
		}
		else if (base.transform.parent != null)
		{
			base.transform.SetParent(null);
			base.transform.localScale = scale;
		}
	}

	private void SetKey()
	{
	}

	private void OnGUI()
	{
		keyEvent = Event.current;
		if ((!keyEvent.isKey && !Input.anyKey && !keyEvent.shift && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(0)) || !(NMC.selectedRKS == this))
		{
			return;
		}
		if ((bool)NMC)
		{
			NMC.selectedRKS = null;
		}
		newKey = keyEvent.keyCode;
		foreach (KeyValuePair<string, KeyCode> key in OptionsMainMenu.instance.keys)
		{
			if (Input.GetMouseButtonDown(0) && key.Value == KeyCode.Mouse0)
			{
				SFXManager.instance.PlaySFX(ErrorSound, 1f, null);
				return;
			}
			if (Input.GetMouseButtonDown(1) && key.Value == KeyCode.Mouse1)
			{
				SFXManager.instance.PlaySFX(ErrorSound, 1f, null);
				return;
			}
			if (Input.GetKey(KeyCode.LeftShift) && key.Value == KeyCode.LeftShift)
			{
				SFXManager.instance.PlaySFX(ErrorSound, 1f, null);
				return;
			}
			if (Input.GetKey(KeyCode.RightShift) && key.Value == KeyCode.RightShift)
			{
				SFXManager.instance.PlaySFX(ErrorSound, 1f, null);
				return;
			}
			if (key.Value == newKey)
			{
				SFXManager.instance.PlaySFX(ErrorSound, 1f, null);
				return;
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			newKey = KeyCode.Mouse0;
			Debug.LogWarning("left click!");
		}
		else if (Input.GetMouseButtonDown(1))
		{
			newKey = KeyCode.Mouse1;
			Debug.LogWarning("right click!");
		}
		else if (Input.GetKey(KeyCode.LeftShift))
		{
			newKey = KeyCode.LeftShift;
			Debug.LogWarning("LSHIFT.");
		}
		else if (Input.GetKey(KeyCode.RightShift))
		{
			newKey = KeyCode.RightShift;
			Debug.LogWarning("RSHIFT.");
		}
		if (!IsXbox)
		{
			_ = newKey;
			if (IsKeyChangeMine)
			{
				OptionsMainMenu.instance.keys["mineKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsKeyChangeBoost)
			{
				OptionsMainMenu.instance.keys["boostKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsKeyChangeShoot)
			{
				OptionsMainMenu.instance.keys["shootKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsMoveDown)
			{
				OptionsMainMenu.instance.keys["downKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsMoveUp)
			{
				OptionsMainMenu.instance.keys["upKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsMoveLeft)
			{
				OptionsMainMenu.instance.keys["leftKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsMoveRight)
			{
				OptionsMainMenu.instance.keys["rightKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			else if (IsHUD)
			{
				OptionsMainMenu.instance.keys["hudKey"] = newKey;
				KeyBindKey.text = "(" + newKey.ToString() + ")";
			}
			SFXManager.instance.PlaySFX(SuccesSound, 1f, null);
			OptionsMainMenu.instance.SaveNewData();
		}
	}
}
