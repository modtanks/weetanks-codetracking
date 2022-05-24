using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnTeamsMenu : MonoBehaviour
{
	public int CurrentTeamNumber;

	private int PrevNumber;

	public int MenuType;

	public GameObject[] Menus;

	public int CurrentDifficulty;

	public int PreviousDifficulty = -1;

	public Toggle[] ColorToggles;

	public MapEditorProp SelectedMEP;

	public TMP_Dropdown DifficultyPick;

	public bool PointOnMe;

	public float DisableMenuAtDistance = 100f;

	public bool IsOpen;

	private Animator myAnimator;

	public Image FCP_mat;

	public FlexibleColorPicker FCP;

	[Header("Lighthouse Menu")]
	public Slider RotationSpeed_lighthouse;

	public Slider AmountLights_lighthouse;

	public Image FCP_mat_lighthouse;

	public FlexibleColorPicker FCP_lighthouse;

	private void Start()
	{
		myAnimator = GetComponent<Animator>();
		for (int i = 0; i < 4; i++)
		{
		}
	}

	public void OnPointerEnter()
	{
		_ = GameMaster.instance.GameHasPaused;
	}

	public void OnOpenMenu(int MenuType, MapEditorProp Selected)
	{
		Debug.Log("Opening animatino Teams menu!");
		myAnimator.SetBool("ShowMenu", value: true);
		GameObject[] menus = Menus;
		for (int i = 0; i < menus.Length; i++)
		{
			menus[i].SetActive(value: false);
		}
		Menus[MenuType].SetActive(value: true);
		DifficultyPick.SetValueWithoutNotify(CurrentDifficulty);
		SelectedMEP = Selected;
	}

	private void UpdateMenu()
	{
		MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: true);
		MapEditorMaster.instance.OnTeamsMenu = true;
		PrevNumber = CurrentTeamNumber;
		DifficultyPick.value = CurrentDifficulty;
		Toggle[] colorToggles = ColorToggles;
		for (int i = 0; i < colorToggles.Length; i++)
		{
			colorToggles[i].isOn = false;
		}
		Debug.Log("Team number = " + CurrentTeamNumber);
		if (CurrentTeamNumber > -1)
		{
			ColorToggles[CurrentTeamNumber].isOn = true;
		}
	}

	public void OnCloseMenu()
	{
		MapEditorMaster.instance.OnTeamsMenu = false;
		PointOnMe = false;
		IsOpen = false;
		if (SelectedMEP != null && SelectedMEP.myMEGP != null)
		{
			SelectedMEP.myMEGP.FieldSelected = false;
		}
		if ((bool)myAnimator)
		{
			myAnimator.SetBool("ShowMenu", value: false);
		}
		if (MapEditorMaster.instance.SelectedParticles != null)
		{
			MapEditorMaster.instance.SelectedParticles.transform.position = new Vector3(999f, 999f, 999f);
		}
		SelectedMEP = null;
	}

	private void OnDisable()
	{
		if (SelectedMEP != null && SelectedMEP.myMEGP != null)
		{
			SelectedMEP.myMEGP.FieldSelected = false;
		}
		if (MapEditorMaster.instance.SelectedParticles != null)
		{
			MapEditorMaster.instance.SelectedParticles.transform.position = new Vector3(999f, 999f, 999f);
		}
		SelectedMEP = null;
	}

	private void Update()
	{
		if (SelectedMEP != null)
		{
			if ((bool)SelectedMEP.myMEGP)
			{
				SelectedMEP.myMEGP.FieldSelected = true;
			}
			if (SelectedMEP.CanBeColored)
			{
				Color color = FCP_mat.material.GetColor("_Color1");
				SelectedMEP.SetMaterials(color, reset: false);
				MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor[SelectedMEP.LayerNumber].Color = color;
			}
			else if ((bool)SelectedMEP.CP)
			{
				if (SelectedMEP.CP.IsLighthouse)
				{
					Color color2 = FCP_mat_lighthouse.material.GetColor("_Color1");
					if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor == null)
					{
						MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor = new SerializableColor[5];
					}
					if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor[SelectedMEP.LayerNumber] == null)
					{
						MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor[SelectedMEP.LayerNumber] = new SerializableColor();
					}
					MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor[SelectedMEP.LayerNumber].Color = color2;
					if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].F1 == null)
					{
						MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].F1 = new float[5];
					}
					MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].F1[SelectedMEP.LayerNumber] = RotationSpeed_lighthouse.value;
					if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].I1 == null)
					{
						MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].I1 = new int[5];
					}
					MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].I1[SelectedMEP.LayerNumber] = Mathf.RoundToInt(AmountLights_lighthouse.value);
				}
				SelectedMEP.CP.CheckCustomProp();
			}
		}
		if (GameMaster.instance.GameHasPaused)
		{
			_ = PointOnMe;
		}
		if (PrevNumber != CurrentTeamNumber)
		{
			UpdateMenu();
			Debug.Log("Team Number changed, now updated!");
			MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: true);
		}
	}

	public void ChangeTeam(int ThisNumber)
	{
		if (CurrentTeamNumber > -1)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[CurrentTeamNumber]--;
			MapEditorMaster.instance.ColorsTeamsPlaced[ThisNumber]++;
		}
		CurrentTeamNumber = ThisNumber;
		Debug.Log(ThisNumber);
		for (int i = 0; i < ColorToggles.Length; i++)
		{
			ColorToggles[i].isOn = false;
		}
		ColorToggles[ThisNumber].isOn = true;
		MapEditorMaster.instance.LastPlacedColor = ThisNumber;
		Debug.Log("Changed Team!" + ThisNumber);
		SelectedMEP.TeamNumber = ThisNumber;
		SelectedMEP.myMEGP.MyTeamNumber = ThisNumber;
		SelectedMEP.myMEGP.SetGridPieceColor();
	}

	public void ChangeDifficultySpawn()
	{
		Debug.Log("Changed difficulty!" + DifficultyPick.value);
		CurrentDifficulty = DifficultyPick.value;
		SelectedMEP.MyDifficultySpawn = DifficultyPick.value;
		SelectedMEP.myMEGP.SpawnDifficulty = DifficultyPick.value;
		PreviousDifficulty = CurrentDifficulty;
	}
}
