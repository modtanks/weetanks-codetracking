using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorMissionBarUI : MonoBehaviour
{
	public int mission;

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

	private void Start()
	{
		InitializeButton();
		LastKnownName = "";
		if (mission == 0)
		{
			myLevelObject = GameMaster.instance.Levels[0];
		}
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

	private void Update()
	{
		if (GameMaster.instance.CurrentMission == mission)
		{
			myImage.color = mybtn.colors.selectedColor;
		}
		else
		{
			myImage.color = mybtn.colors.normalColor;
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

	private void OnClickBtn()
	{
		Debug.LogWarning("Map button clicked!", base.gameObject);
		MapEditorMaster.instance.SwitchLevel(mission);
		MapEditorMaster.instance.Play2DClipAtPoint(missionCreateSound);
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
			MapEditorMaster.instance.Play2DClipAtPoint(missionCreateSound);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	private void OnUpBtn()
	{
		if (MapEditorMaster.instance.canDoButton)
		{
			MapEditorMaster.instance.MoveLevel(up: false, mission);
			MapEditorMaster.instance.Play2DClipAtPoint(missionCreateSound);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	private void OnDownBtn()
	{
		if (MapEditorMaster.instance.canDoButton)
		{
			MapEditorMaster.instance.MoveLevel(up: true, mission);
			MapEditorMaster.instance.Play2DClipAtPoint(missionCreateSound);
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.coolDown());
		}
	}

	public void CheckMaster()
	{
		mission = base.transform.GetSiblingIndex() - 1;
		Debug.Log("new mission index = " + mission);
		myMissionNumber.text = "Mission " + (mission + 1);
		if (myMissionName.text != GameMaster.instance.MissionNames[mission] && LastKnownName != GameMaster.instance.MissionNames[mission])
		{
			myMissionName.text = GameMaster.instance.MissionNames[mission];
			LastKnownName = GameMaster.instance.MissionNames[mission];
		}
	}
}
