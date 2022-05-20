using System;
using System.Collections.Generic;

[Serializable]
public class ProgressDataOnline
{
	[Serializable]
	public class kills
	{
		public int ID_10;

		public int ID_11;

		public int ID_12;

		public int ID_13;

		public int ID_14;

		public int ID_15;

		public int ID_110;
	}

	[Serializable]
	public class kills_others
	{
		public int ID_110;

		public int ID_130;

		public int ID_150;

		public int ID_170;

		public int ID_200;
	}

	public int maxMission0;

	public int maxMission1;

	public int maxMission2;

	public int maxMission3;

	public int totalKills;

	public int totalDefeats;

	public int totalWins;

	public int survivalTanksKilled;

	public int totalKillsBounce;

	public int totalRevivesPerformed;

	public List<int> killed = new List<int>();

	public int[] AM = new int[100];

	public List<int> ActivatedAM = new List<int>();

	public int[] hW = new int[10];

	public int marbles;

	public int accountid;

	public string accountname;

	public int TimePlayed;

	public float[] SpeedrunTimes = new float[9];

	public List<int> FoundMissions = new List<int>();

	public List<int> CC;

	public kills killed_s;

	public kills_others killed_others;
}
