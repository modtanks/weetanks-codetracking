using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCustomMap : MonoBehaviour
{
	public GameObject LevelPrefab;

	private void Start()
	{
		LoadData();
	}

	public void LoadData()
	{
		MapEditorData theData = SavingMapEditorData.LoadData(OptionsMainMenu.instance.MapEditorMapName);
		if (theData == null)
		{
			SceneManager.LoadScene(0);
		}
		int missions = theData.missionAmount;
		if (missions < 1)
		{
			SceneManager.LoadScene(0);
		}
		GameMaster.instance.NightLevels = theData.nightMissions;
		List<string> thenames = theData.missionNames.ToList();
		List<MapPiecesClass> allPropData = theData.MissionDataProps;
		GameMaster.instance.Lives = theData.StartingLives;
		if (theData.VersionCreated == "v0.7.9" || theData.VersionCreated == "v0.7.8")
		{
			MapEditorMaster.instance.PlayerSpeed = 65;
			MapEditorMaster.instance.PlayerMaxBullets = 5;
			MapEditorMaster.instance.PlayerCanLayMines = true;
			MapEditorMaster.instance.PlayerArmourPoints = 0;
			MapEditorMaster.instance.PlayerBulletType = 0;
			MapEditorMaster.instance.PlayerAmountBounces = 1;
		}
		else
		{
			MapEditorMaster.instance.PlayerSpeed = theData.PTS;
			MapEditorMaster.instance.PlayerMaxBullets = theData.PMB;
			MapEditorMaster.instance.PlayerCanLayMines = theData.PCLM;
			MapEditorMaster.instance.PlayerArmourPoints = theData.PAP;
			MapEditorMaster.instance.PlayerBulletType = theData.PBT;
			MapEditorMaster.instance.PlayerAmountBounces = theData.PAB;
			if (theData.TeamColorsShowing != null)
			{
				for (int j = 0; j < theData.TeamColorsShowing.Length; j++)
				{
					MapEditorMaster.instance.TeamColorEnabled[j] = theData.TeamColorsShowing[j];
				}
			}
		}
		MapEditorMaster.instance.SetCustomTankData(theData);
		GameObject newLevel = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(newLevel);
		int i;
		for (i = 0; i < missions; i++)
		{
			SingleMapEditorData newSMED = new SingleMapEditorData(null, null);
			newSMED.MissionDataProps = theData.MissionDataProps.FindAll((MapPiecesClass x) => x.missionNumber == i);
			newSMED.MissionMessage = theData.MissionMessages[i];
			for (int k = 0; k < OptionsMainMenu.instance.MapSize; k++)
			{
				newSMED.MissionDataProps[k].ID = k;
			}
			GameMaster.instance.MissionNames.Add("Level " + MapEditorMaster.instance.Levels.Count);
			MapEditorMaster.instance.Levels.Add(newSMED);
			MapEditorMaster.instance.CreateNewLevel(isBrandNew: false);
			if (thenames.ElementAtOrDefault(i) != null)
			{
				GameMaster.instance.MissionNames[i] = thenames[i];
			}
			else
			{
				GameMaster.instance.MissionNames[i] = "no name";
			}
		}
		bool oldVersion = false;
		if (theData.VersionCreated == "v0.7.9" || theData.VersionCreated == "v0.7.8" || theData.VersionCreated == "v0.7.10" || theData.VersionCreated == "v0.7.11" || theData.VersionCreated == "v0.7.12" || theData.VersionCreated == "v0.8.0a" || theData.VersionCreated == "v0.8.0b" || theData.VersionCreated == "v0.8.0c")
		{
			oldVersion = true;
		}
		MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlaceAllProps(MapEditorMaster.instance.Levels[0].MissionDataProps, oldVersion, 0));
		if (theData.WeatherTypes != null && theData.WeatherTypes.Length != 0)
		{
			MapEditorMaster.instance.WeatherTypes = theData.WeatherTypes;
		}
		if (theData.MissionFloorTextures != null && theData.MissionFloorTextures.Length != 0)
		{
			MapEditorMaster.instance.MissionFloorTextures = theData.MissionFloorTextures;
		}
		if (theData.NoBordersMissions != null)
		{
			MapEditorMaster.instance.NoBordersMissions = theData.NoBordersMissions;
		}
		GameMaster.instance.Levels[0].SetActive(value: false);
		StartCoroutine(DisableTheGame());
	}

	private IEnumerator DisableTheGame()
	{
		yield return new WaitForSeconds(1f);
		GameMaster.instance.DisableGame();
	}

	private IEnumerator PlaceAllProps(List<MapPiecesClass> allPropData, bool oldVersion)
	{
		yield return new WaitForSeconds(0.2f);
		foreach (GameObject level2 in GameMaster.instance.Levels)
		{
			level2.SetActive(value: true);
		}
		GameObject[] allGridPieces = GameObject.FindGameObjectsWithTag("MapeditorField");
		GameObject[] array = allGridPieces;
		foreach (GameObject piece in array)
		{
			MapEditorGridPiece MEGP = piece.GetComponent<MapEditorGridPiece>();
			MapPiecesClass myClass = allPropData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			if (myClass == null)
			{
				continue;
			}
			if (myClass.propID.Length < 3 || oldVersion)
			{
				int propID = Convert.ToInt32(myClass.propID);
				if (propID == -1)
				{
					continue;
				}
				int propRotation = Convert.ToInt32(myClass.propRotation);
				int propTeam = Convert.ToInt32(myClass.TeamColor);
				if (propID > -1)
				{
					MEGP.MyTeamNumber = propTeam;
					switch (propID)
					{
					case 41:
						MEGP.SpawnInProps(0, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(41, propRotation, propTeam, 1, 0);
						break;
					case 42:
						MEGP.SpawnInProps(1, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(42, propRotation, propTeam, 1, 0);
						break;
					case 43:
						MEGP.SpawnInProps(1, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(1, propRotation, propTeam, 1, 0);
						break;
					case 44:
						MEGP.SpawnInProps(0, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(0, propRotation, propTeam, 1, 0);
						break;
					default:
						MEGP.SpawnInProps(propID, propRotation, propTeam, 0, 0);
						break;
					}
				}
				continue;
			}
			for (int i = 0; i < 5; i++)
			{
				if (myClass.propID[i] > -1)
				{
					MEGP.MyTeamNumber = myClass.TeamColor[i];
					MEGP.SpawnDifficulty = myClass.SpawnDifficulty;
					MEGP.SpawnInProps(myClass.propID[i], myClass.propRotation[i], myClass.TeamColor[i], i, myClass.SpawnDifficulty);
				}
			}
		}
		foreach (GameObject level in GameMaster.instance.Levels)
		{
			level.SetActive(value: false);
		}
		GameMaster.instance.CurrentMission = 0;
	}

	public void CreateNewLevel()
	{
		GameObject newLevel = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(newLevel);
		if (GameMaster.instance.Levels.Count > 0)
		{
			newLevel.name = "Custom Level " + GameMaster.instance.Levels.Count;
			GameMaster.instance.Levels[GameMaster.instance.Levels.Count - 1].SetActive(value: false);
			GameMaster.instance.CurrentMission = GameMaster.instance.Levels.Count - 1;
		}
		else
		{
			newLevel.name = "Custom Level 1";
			GameMaster.instance.Levels[0].SetActive(value: false);
			GameMaster.instance.CurrentMission = 0;
		}
		MapEditorGridGenerator MEGG = newLevel.GetComponentInChildren<MapEditorGridGenerator>();
		MEGG.GenerateFields();
	}
}
