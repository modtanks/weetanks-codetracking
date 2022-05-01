using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorMissionBarUI : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public int mission = 0;

	public TextMeshProUGUI myMissionNumber;

	public TextMeshProUGUI myMissionName;

	private Button mybtn;

	public GameObject myLevelObject;

	private RawImage myImage;

	public AudioClip missionCreateSound;

	public string LastKnownName;

	public Button RemoveBtn;

	public Button DuplicateBtn;

	public Button UpBtn;

	public Button DownBtn;

	public Texture NotSelected;

	public Texture Hovering;

	public Texture Selected;

	public ScrollRect SR;

	public bool isSelected = false;

	private bool MouseOnMe = false;

	private void Start()
	{
		InitializeButton();
		LastKnownName = "";
		if (mission == 0)
		{
			myLevelObject = GameMaster.instance.Levels[0];
		}
		SR = base.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<ScrollRect>();
	}

	public void InitializeButton()
	{
		mybtn = GetComponent<Button>();
		mybtn.onClick.AddListener(OnClickBtn);
		RemoveBtn.onClick.AddListener(OnRemoveBtn);
		DuplicateBtn.onClick.AddListener(OnDuplicateBtn);
		UpBtn.onClick.AddListener(OnUpBtn);
		DownBtn.onClick.AddListener(OnDownBtn);
		myImage = GetComponent<RawImage>();
	}

	public void OnScroll()
	{
		SR.verticalNormalizedPosition += Input.mouseScrollDelta.y / 16f;
	}

	private void Update()
	{
		if (GameMaster.instance.CurrentMission == mission)
		{
			myImage.texture = Selected;
			isSelected = true;
		}
		else
		{
			myImage.color = mybtn.colors.normalColor;
			if (!MouseOnMe)
			{
				myImage.texture = NotSelected;
			}
			isSelected = false;
		}
		if (mission == 0)
		{
			DownBtn.gameObject.SetActive(value: false);
			if (MapEditorMaster.instance.Levels.Count == 1)
			{
				UpBtn.gameObject.SetActive(value: false);
			}
			else
			{
				UpBtn.gameObject.SetActive(value: true);
			}
		}
		else if (!DownBtn.gameObject.activeSelf)
		{
			DownBtn.gameObject.SetActive(value: true);
		}
		else if (mission == MapEditorMaster.instance.Levels.Count - 1)
		{
			UpBtn.gameObject.SetActive(value: false);
		}
		else if (!UpBtn.gameObject.activeSelf)
		{
			UpBtn.gameObject.SetActive(value: true);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
	}

	public void OnMouseOver(BaseEventData eventData)
	{
		MouseOnMe = true;
		if (!isSelected)
		{
			myImage.texture = Hovering;
		}
	}

	public void OnMouseExit(BaseEventData eventData)
	{
		MouseOnMe = false;
		if (!isSelected)
		{
			myImage.texture = NotSelected;
		}
	}

	private void OnClickBtn()
	{
		Debug.LogWarning("Map button clicked!", base.gameObject);
		MapEditorMaster.instance.SwitchLevel(mission);
		SFXManager.instance.PlaySFX(missionCreateSound);
	}

	private void OnRemoveBtn()
	{
		if (MapEditorMaster.instance.canDoButton)
		{
			MapEditorMaster.instance.RemoveLevel(mission);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	private void OnDuplicateBtn()
	{
		if (MapEditorMaster.instance.canDoButton)
		{
			MapEditorMaster.instance.DuplicateLevel(mission);
			SFXManager.instance.PlaySFX(missionCreateSound);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	private void OnUpBtn()
	{
		if (MapEditorMaster.instance.canDoButton)
		{
			MapEditorMaster.instance.MoveLevel(up: false, mission);
			SFXManager.instance.PlaySFX(missionCreateSound);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	private void OnDownBtn()
	{
		if (MapEditorMaster.instance.canDoButton)
		{
			MapEditorMaster.instance.MoveLevel(up: true, mission);
			SFXManager.instance.PlaySFX(missionCreateSound);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	public void CheckMaster()
	{
		mission = base.transform.GetSiblingIndex() - 1;
		myMissionNumber.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (mission + 1);
		if (myMissionName.text != GameMaster.instance.MissionNames[mission] && LastKnownName != GameMaster.instance.MissionNames[mission])
		{
			myMissionName.text = GameMaster.instance.MissionNames[mission];
			LastKnownName = GameMaster.instance.MissionNames[mission];
		}
	}
}
