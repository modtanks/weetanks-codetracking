using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RebindKeyScript : MonoBehaviour
{
	public bool IsSelected = false;

	public bool IsXbox = false;

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

	public int myMenu = 0;

	public Vector3 scale;

	private void Start()
	{
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
	}

	private void SetText(string text, string description)
	{
		KeyBindText.text = text;
		KeyBindDescription.text = description;
	}

	private void Update()
	{
	}

	private void SetKey()
	{
	}

	private void OnGUI()
	{
	}
}
