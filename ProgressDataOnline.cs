using System;
using System.Collections.Generic;

[Serializable]
public class ProgressDataOnline
{
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

	public List<int> foundSecretMissions = new List<int>();
}
