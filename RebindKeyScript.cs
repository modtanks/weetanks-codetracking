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
}
