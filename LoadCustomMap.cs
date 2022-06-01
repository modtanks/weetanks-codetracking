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
		MapEditorData mapEditorData = SavingMapEditorData.LoadData(OptionsMainMenu.instance.MapEditorMapName);
		if (mapEditorData == null)
		{
			SceneManager.LoadScene(0);
		}
		int missionAmount = mapEditorData.missionAmount;
		if (missionAmount < 1)
		{
			SceneManager.LoadScene(0);
		}
		GameMaster.instance.NightLevels = mapEditorData.nightMissions;
		List<string> list = mapEditorData.missionNames.ToList();
		GameMaster.instance.Lives = mapEditorData.StartingLives;
		if (mapEditorData.VersionCreated == "v0.7.9" || mapEditorData.VersionCreated == "v0.7.8")
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
			MapEditorMaster.instance.PlayerSpeed = mapEditorData.PTS;
			MapEditorMaster.instance.PlayerMaxBullets = mapEditorData.PMB;
			MapEditorMaster.instance.PlayerCanLayMines = mapEditorData.PCLM;
			MapEditorMaster.instance.PlayerArmourPoints = mapEditorData.PAP;
			MapEditorMaster.instance.PlayerBulletType = mapEditorData.PBT;
			MapEditorMaster.instance.PlayerAmountBounces = mapEditorData.PAB;
			if (mapEditorData.TeamColorsShowing != null)
			{
				for (int k = 0; k < mapEditorData.TeamColorsShowing.Length; k++)
				{
					MapEditorMaster.instance.TeamColorEnabled[k] = mapEditorData.TeamColorsShowing[k];
				}
			}
		}
		MapEditorMaster.instance.SetCustomTankData(mapEditorData);
		GameObject item = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(item);
		int j;
		for (j = 0; j < missionAmount; j++)
		{
			SingleMapEditorData singleMapEditorData = new SingleMapEditorData(null, null);
			singleMapEditorData.MissionDataProps = mapEditorData.MissionDataProps.FindAll((MapPiecesClass x) => x.missionNumber == j);
			singleMapEditorData.MissionMessage = mapEditorData.MissionMessages[j];
			for (int l = 0; l < OptionsMainMenu.instance.MapSize; l++)
			{
				singleMapEditorData.MissionDataProps[l].ID = l;
			}
			GameMaster.instance.MissionNames.Add("Level " + MapEditorMaster.instance.Levels.Count);
			MapEditorMaster.instance.Levels.Add(singleMapEditorData);
			MapEditorMaster.instance.CreateNewLevel(isBrandNew: false);
			if (list.ElementAtOrDefault(j) != null)
			{
				GameMaster.instance.MissionNames[j] = list[j];
			}
			else
			{
				GameMaster.instance.MissionNames[j] = "no name";
			}
		}
		bool oldVersion = false;
		if (mapEditorData.VersionCreated == "v0.7.9" || mapEditorData.VersionCreated == "v0.7.8" || mapEditorData.VersionCreated == "v0.7.10" || mapEditorData.VersionCreated == "v0.7.11" || mapEditorData.VersionCreated == "v0.7.12" || mapEditorData.VersionCreated == "v0.8.0a" || mapEditorData.VersionCreated == "v0.8.0b" || mapEditorData.VersionCreated == "v0.8.0c")
		{
			oldVersion = true;
		}
		MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlaceAllProps(MapEditorMaster.instance.Levels[0].MissionDataProps, oldVersion, 0));
		if (mapEditorData.WeatherTypes != null)
		{
			if (mapEditorData.WeatherTypes.Length != 0)
			{
				if (mapEditorData.WeatherTypes != null && mapEditorData.WeatherTypes.Length != 0)
				{
					for (int m = 0; m < mapEditorData.WeatherTypes.Length; m++)
					{
						int num = mapEditorData.WeatherTypes[m];
						if (num > 0)
						{
							MapEditorMaster.MissionProperties missionProperties = new MapEditorMaster.MissionProperties();
							missionProperties.MissionFloorTexture = 0;
							missionProperties.WeatherType = num;
							missionProperties.MissionNumber = m;
							MapEditorMaster.instance.Properties.Add(missionProperties);
						}
					}
				}
				if (mapEditorData.MissionFloorTextures != null && mapEditorData.MissionFloorTextures.Length != 0)
				{
					int i;
					for (i = 0; i < mapEditorData.MissionFloorTextures.Length; i++)
					{
						int num2 = mapEditorData.MissionFloorTextures[i];
						if (num2 > 0)
						{
							MapEditorMaster.MissionProperties missionProperties2 = MapEditorMaster.instance.Properties.Find((MapEditorMaster.MissionProperties x) => x.MissionNumber == i);
							if (missionProperties2 != null)
							{
								missionProperties2.MissionFloorTexture = num2;
								continue;
							}
							MapEditorMaster.MissionProperties missionProperties3 = new MapEditorMaster.MissionProperties();
							missionProperties3.MissionFloorTexture = num2;
							missionProperties3.WeatherType = 0;
							missionProperties3.MissionNumber = i;
							MapEditorMaster.instance.Properties.Add(missionProperties3);
						}
					}
				}
			}
			Debug.Log(mapEditorData.WeatherTypes[0]);
		}
		else
		{
			Debug.Log("NEW WAY FOUND!");
			if (mapEditorData.Properties != null)
			{
				for (int n = 0; n < mapEditorData.Properties.Count; n++)
				{
					MapEditorMaster.MissionProperties missionProperties4 = new MapEditorMaster.MissionProperties();
					missionProperties4.CurrentFloorName = mapEditorData.Properties[n].CurrentFloorName;
					missionProperties4.WeatherType = mapEditorData.Properties[n].WeatherType;
					missionProperties4.MissionNumber = n;
					MapEditorMaster.instance.Properties.Add(missionProperties4);
				}
				if (MapEditorMaster.instance.Properties.Find((MapEditorMaster.MissionProperties x) => x.MissionNumber == 0) != null)
				{
					GameMaster.instance.floor.GetComponent<MeshRenderer>().material = GlobalAssets.instance.TheFloors.Find((GlobalAssets.Floors x) => x.FloorName == MapEditorMaster.instance.Properties.Find((MapEditorMaster.MissionProperties x) => x.MissionNumber == 0).CurrentFloorName).FloorTexture;
				}
			}
		}
		if (mapEditorData.NoBordersMissions != null)
		{
			MapEditorMaster.instance.NoBordersMissions = mapEditorData.NoBordersMissions;
		}
		GameMaster.instance.Levels[0].SetActive(value: false);
		StartCoroutine(DisableTheGame());
		GameMaster.instance.CheckGameState();
	}

	private IEnumerator DisableTheGame()
	{
		yield return new WaitForSeconds(1f);
		GameMaster.instance.DisableGame();
	}

	private IEnumerator PlaceAllProps(List<MapPiecesClass> allPropData, bool oldVersion)
	{
		yield return new WaitForSeconds(0.2f);
		foreach (GameObject level in GameMaster.instance.Levels)
		{
			level.SetActive(value: true);
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("MapeditorField");
		foreach (GameObject gameObject in array)
		{
			MapEditorGridPiece MEGP = gameObject.GetComponent<MapEditorGridPiece>();
			MapPiecesClass mapPiecesClass = allPropData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			if (mapPiecesClass == null)
			{
				continue;
			}
			if (mapPiecesClass.propID.Length < 3 || oldVersion)
			{
				int num = Convert.ToInt32(mapPiecesClass.propID);
				if (num == -1)
				{
					continue;
				}
				int direction = Convert.ToInt32(mapPiecesClass.propRotation);
				int num2 = Convert.ToInt32(mapPiecesClass.TeamColor);
				if (num > -1)
				{
					MEGP.MyTeamNumber = num2;
					switch (num)
					{
					case 41:
						MEGP.SpawnInProps(0, direction, num2, 0, 0);
						MEGP.SpawnInProps(41, direction, num2, 1, 0);
						break;
					case 42:
						MEGP.SpawnInProps(1, direction, num2, 0, 0);
						MEGP.SpawnInProps(42, direction, num2, 1, 0);
						break;
					case 43:
						MEGP.SpawnInProps(1, direction, num2, 0, 0);
						MEGP.SpawnInProps(1, direction, num2, 1, 0);
						break;
					case 44:
						MEGP.SpawnInProps(0, direction, num2, 0, 0);
						MEGP.SpawnInProps(0, direction, num2, 1, 0);
						break;
					default:
						MEGP.SpawnInProps(num, direction, num2, 0, 0);
						break;
					}
				}
				continue;
			}
			for (int j = 0; j < 5; j++)
			{
				if (mapPiecesClass.propID[j] > -1)
				{
					MEGP.MyTeamNumber = mapPiecesClass.TeamColor[j];
					MEGP.SpawnDifficulty = mapPiecesClass.SpawnDifficulty;
					MEGP.SpawnInProps(mapPiecesClass.propID[j], mapPiecesClass.propRotation[j], mapPiecesClass.TeamColor[j], j, mapPiecesClass.SpawnDifficulty);
				}
			}
		}
		foreach (GameObject level2 in GameMaster.instance.Levels)
		{
			level2.SetActive(value: false);
		}
		GameMaster.instance.CurrentMission = 0;
	}

	public void CreateNewLevel()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(gameObject);
		if (GameMaster.instance.Levels.Count > 0)
		{
			gameObject.name = "Custom Level " + GameMaster.instance.Levels.Count;
			GameMaster.instance.Levels[GameMaster.instance.Levels.Count - 1].SetActive(value: false);
			GameMaster.instance.CurrentMission = GameMaster.instance.Levels.Count - 1;
		}
		else
		{
			gameObject.name = "Custom Level 1";
			GameMaster.instance.Levels[0].SetActive(value: false);
			GameMaster.instance.CurrentMission = 0;
		}
		gameObject.GetComponentInChildren<MapEditorGridGenerator>().GenerateFields();
	}
}
