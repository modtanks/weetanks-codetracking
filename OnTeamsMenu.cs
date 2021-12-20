using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnTeamsMenu : MonoBehaviour
{
	public int CurrentTeamNumber;

	private int PrevNumber;

	public int CurrentDifficulty;

	public Toggle[] ColorToggles;

	public MapEditorProp SelectedMEP;

	public TMP_Dropdown DifficultyPick;

	public bool PointOnMe;

	public float DisableMenuAtDistance = 100f;

	private void Start()
	{
		for (int i = 0; i < 4; i++)
		{
		}
	}

	public void OnPointerEnter()
	{
		if (!GameMaster.instance.GameHasPaused)
		{
			UpdateMenu();
			PointOnMe = true;
		}
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
		ColorToggles[CurrentTeamNumber].isOn = true;
	}

	public void OnPointerExit()
	{
		Debug.Log("Exiting Teams menu!");
		MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: false);
		MapEditorMaster.instance.OnTeamsMenu = false;
		PointOnMe = false;
	}

	private void Update()
	{
		if (GameMaster.instance.GameHasPaused && PointOnMe)
		{
			OnPointerExit();
		}
		if (PrevNumber != CurrentTeamNumber)
		{
			UpdateMenu();
			Debug.Log("Team Number changed, now updated!");
			MapEditorMaster.instance.TeamsCursorMenu.SetActive(value: true);
		}
		if (PointOnMe && Vector3.Distance(base.transform.position, Input.mousePosition) > DisableMenuAtDistance)
		{
			OnPointerExit();
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
	}
}
