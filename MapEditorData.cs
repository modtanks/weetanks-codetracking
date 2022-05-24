using System;
using System.Collections.Generic;

[Serializable]
public class MapEditorData
{
	[Serializable]
	public class MissionProperties
	{
		public int WeatherType;

		public int MissionFloorTexture;

		public string CurrentFloorName;
	}

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

	public List<CustomTankData> CTD = new List<CustomTankData>();

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

	public List<MissionProperties> Properties = new List<MissionProperties>();

	public int[] WeatherTypes;

	public int[] MissionFloorTextures;

	public List<string> MissionMessages = new List<string>();

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
		MissionMessages.Clear();
		for (int i = 0; i < MEM.Levels.Count; i++)
		{
			for (int j = 0; j < OptionsMainMenu.instance.MapSize; j++)
			{
				MEM.Levels[i].MissionDataProps[j].missionNumber = i;
				list.Add(MEM.Levels[i].MissionDataProps[j]);
			}
			MissionMessages.Add(MEM.Levels[i].MissionMessage);
		}
		MissionDataProps = list;
		campaignName = MEM.campaignName;
		signedName = MEM.signedName;
		StartingLives = MEM.StartingLives;
		MapSize = OptionsMainMenu.instance.MapSize;
		PTS = MEM.PlayerSpeed;
		PMB = MEM.PlayerMaxBullets;
		PBT = MEM.PlayerBulletType;
		PAP = MEM.PlayerArmourPoints;
		PCLM = MEM.PlayerCanLayMines;
		PAB = MEM.PlayerAmountBounces;
		TeamColorsShowing = MEM.TeamColorEnabled;
		PID = MEM.PID;
		CTD = MEM.CustomTankDatas;
		difficulty = MEM.Difficulty;
		isPublished = MEM.IsPublished;
		NoBordersMissions = MEM.NoBordersMissions;
		for (int k = 0; k < MEM.Properties.Count; k++)
		{
			MissionProperties item = new MissionProperties
			{
				MissionFloorTexture = MEM.Properties[k].MissionFloorTexture,
				CurrentFloorName = MEM.Properties[k].CurrentFloorName,
				WeatherType = MEM.Properties[k].WeatherType
			};
			Properties.Add(item);
		}
	}
}
