using System;
using System.Collections.Generic;

[Serializable]
public class MapEditorData
{
	public int missionAmount;

	public int PID;

	public int isPublished;

	public List<int> nightMissions = new List<int>();

	public List<string> missionNames = new List<string>();

	public List<MapPiecesClass> MissionDataProps;

	public string campaignName;

	public string signedName;

	public string VersionCreated = "v0.0.0";

	public int MapSize = 285;

	public bool[] TeamColorsShowing;

	public int difficulty;

	public List<SerializableColor> CTC = new List<SerializableColor>();

	public List<int> CustomTankSpeed = new List<int>();

	public List<float> CustomFireSpeed = new List<float>();

	public List<int> CustomBounces = new List<int>();

	public List<int> CustomBullets = new List<int>();

	public List<float> CustomMineSpeed = new List<float>();

	public List<int> CustomTurnHead = new List<int>();

	public List<int> CustomAccuracy = new List<int>();

	public List<bool> LayMines = new List<bool>();

	public List<int> CustomBulletType = new List<int>();

	public List<int> CustomMusic = new List<int>();

	public List<bool> CustomInvisibility = new List<bool>();

	public List<bool> CustomCalculateShots = new List<bool>();

	public List<bool> CustomArmoured = new List<bool>();

	public List<int> CustomArmourPoints = new List<int>();

	public List<float> CustomScalePoints = new List<float>();

	public List<int> NoBordersMissions = new List<int>();

	public int StartingLives;

	public int PTS;

	public int PMB;

	public int PBT;

	public int PAP;

	public int PAB;

	public bool PCLM;

	public int[] WeatherTypes;

	public int[] MissionFloorTextures;

	public MapEditorData(GameMaster GM, MapEditorMaster MEM)
	{
		if (GM == null && MEM == null)
		{
			return;
		}
		VersionCreated = OptionsMainMenu.instance.CurrentVersion;
		missionAmount = MEM.Levels.Count;
		nightMissions = GM.NightLevels;
		missionNames = GM.MissionNames;
		List<MapPiecesClass> list = new List<MapPiecesClass>();
		for (int i = 0; i < MEM.Levels.Count; i++)
		{
			for (int j = 0; j < OptionsMainMenu.instance.MapSize; j++)
			{
				MEM.Levels[i].MissionDataProps[j].missionNumber = i;
				list.Add(MEM.Levels[i].MissionDataProps[j]);
			}
		}
		MissionDataProps = list;
		campaignName = MEM.campaignName;
		signedName = MEM.signedName;
		StartingLives = MEM.StartingLives;
		CustomMusic = MEM.CustomMusic;
		MapSize = OptionsMainMenu.instance.MapSize;
		PTS = MEM.PlayerSpeed;
		PMB = MEM.PlayerMaxBullets;
		PBT = MEM.PlayerBulletType;
		PAP = MEM.PlayerArmourPoints;
		PCLM = MEM.PlayerCanLayMines;
		PAB = MEM.PlayerAmountBounces;
		TeamColorsShowing = MEM.TeamColorEnabled;
		PID = MEM.PID;
		for (int k = 0; k < 3; k++)
		{
			SerializableColor item = new SerializableColor(MEM.CustomTankColor[k]);
			CTC.Add(item);
			CustomTankSpeed.Add(MEM.CustomTankSpeed[k]);
			CustomFireSpeed.Add(MEM.CustomFireSpeed[k]);
			CustomBounces.Add(MEM.CustomBounces[k]);
			CustomBullets.Add(MEM.CustomBullets[k]);
			CustomMineSpeed.Add(MEM.CustomMineSpeed[k]);
			CustomTurnHead.Add(MEM.CustomTurnHead[k]);
			CustomAccuracy.Add(MEM.CustomAccuracy[k]);
			LayMines.Add(MEM.CustomLayMines[k]);
			CustomBulletType.Add(MEM.CustomBulletType[k]);
			CustomInvisibility.Add(MEM.CustomInvisibility[k]);
			CustomCalculateShots.Add(MEM.CustomCalculateShots[k]);
			CustomArmoured.Add(MEM.CustomArmoured[k]);
			CustomArmourPoints.Add(MEM.CustomArmourPoints[k]);
			CustomScalePoints.Add(MEM.CustomTankScale[k]);
		}
		difficulty = MEM.Difficulty;
		isPublished = MEM.IsPublished;
		WeatherTypes = MEM.WeatherTypes;
		MissionFloorTextures = MEM.MissionFloorTextures;
		NoBordersMissions = MEM.NoBordersMissions;
	}
}
