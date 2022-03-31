using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnTeamsMenu : MonoBehaviour
{
	public int CurrentTeamNumber = 0;

	private int PrevNumber = 0;

	public int MenuType = 0;

	public GameObject[] Menus;

	public int CurrentDifficulty = 0;

	public int PreviousDifficulty = -1;

	public Toggle[] ColorToggles;

	public MapEditorProp SelectedMEP;

	public TMP_Dropdown DifficultyPick;

	public bool PointOnMe = false;

	public float DisableMenuAtDistance = 100f;

	public bool IsOpen = false;

	private Animator myAnimator;

	public Image FCP_mat;

	public FlexibleColorPicker FCP;

	private void Start()
	{
		myAnimator = GetComponent<Animator>();
		for (int i = 0; i < 4; i++)
		{
		}
	}

	public void OnPointerEnter()
	{
		if (GameMaster.instance.GameHasPaused)
		{
		}
	}

	public void OnOpenMenu(int MenuType, MapEditorProp Selected)
	{
		Debug.Log("Opening animatino Teams menu!");
		myAnimator.SetBool("ShowMenu", value: true);
		GameObject[] menus = Menus;
		foreach (GameObject Menu in menus)
		{
			Menu.SetActive(value: false);
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
		foreach (Toggle toggl in colorToggles)
		{
			toggl.isOn = false;
		}
		ColorToggles[CurrentTeamNumber].isOn = true;
	}

	public void OnCloseMenu()
	{
		Debug.Log("Exiting Teams menu!");
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
		MapEditorMaster.instance.SelectedParticles.transform.position = new Vector3(9999f, 9999f, 9999f);
		SelectedMEP = null;
	}

	private void OnDisable()
	{
		if (SelectedMEP != null && SelectedMEP.myMEGP != null)
		{
			SelectedMEP.myMEGP.FieldSelected = false;
		}
		MapEditorMaster.instance.SelectedParticles.transform.position = new Vector3(9999f, 9999f, 9999f);
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
				Color newColor = FCP_mat.material.GetColor("_Color1");
				SelectedMEP.SetMaterials(newColor, reset: false);
				MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[SelectedMEP.myMEGP.ID].CustomColor[SelectedMEP.LayerNumber].Color = newColor;
			}
		}
		if (!GameMaster.instance.GameHasPaused || PointOnMe)
		{
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
