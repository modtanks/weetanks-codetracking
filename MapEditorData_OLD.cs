using System;
using System.Collections.Generic;

[Serializable]
public class MapEditorData_OLD
{
	public int missionAmount;

	public List<int> nightMissions = new List<int>();

	public List<string> missionNames = new List<string>();

	public List<MapPiecesClass_old> MissionDataProps;

	public string campaignName;

	public string signedName;

	public string VersionCreated = "v0.0.0";

	public int MapSize = 285;

	public bool[] TeamColorsShowing;

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

	public int StartingLives;

	public int PTS;

	public int PMB;

	public int PBT;

	public int PAP;

	public int PAB;

	public bool PCLM;

	public MapEditorData_OLD(GameMaster GM, MapEditorMaster MEM)
	{
		VersionCreated = OptionsMainMenu.instance.CurrentVersion;
		missionAmount = GM.Levels.Count;
		nightMissions = GM.NightLevels;
		missionNames = GM.MissionNames;
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
		for (int i = 0; i < 3; i++)
		{
			SerializableColor item = new SerializableColor(MEM.CustomTankColor[i]);
			CTC.Add(item);
			CustomTankSpeed.Add(MEM.CustomTankSpeed[i]);
			CustomFireSpeed.Add(MEM.CustomFireSpeed[i]);
			CustomBounces.Add(MEM.CustomBounces[i]);
			CustomBullets.Add(MEM.CustomBullets[i]);
			CustomMineSpeed.Add(MEM.CustomMineSpeed[i]);
			CustomTurnHead.Add(MEM.CustomTurnHead[i]);
			CustomAccuracy.Add(MEM.CustomAccuracy[i]);
			LayMines.Add(MEM.CustomLayMines[i]);
			CustomBulletType.Add(MEM.CustomBulletType[i]);
			CustomInvisibility.Add(MEM.CustomInvisibility[i]);
			CustomCalculateShots.Add(MEM.CustomCalculateShots[i]);
			CustomArmoured.Add(MEM.CustomArmoured[i]);
			CustomArmourPoints.Add(MEM.CustomArmourPoints[i]);
			CustomScalePoints.Add(MEM.CustomTankScale[i]);
		}
	}
}
