using System;
using System.Collections.Generic;

[Serializable]
public class ProgressDataNew
{
	public int cM;

	public int cK;

	public int cH;

	public int cG;

	public int tTK;

	public int tTD;

	public int tTW;

	public int tKB;

	public int[] hW;

	public int tSK;

	public int tRP;

	public List<int> CK;

	public List<int> SM = new List<int>();

	public int accountID = -1;

	public ProgressDataNew(GameMaster GM, ProgressData old)
	{
		if (old != null)
		{
			cM = old.completedMissions;
			cK = old.completedKid;
			cH = old.completedHard;
			cG = old.completedGrandpa;
			tTK = old.totalTankKills;
			tTD = old.totalTimesDefeated;
			tTW = old.totalTimesWon;
			hW = old.highestWaves;
			tSK = old.totalSurvivalKills;
			tKB = old.totalKillsBounced;
			tRP = old.totalRevivesPerformed;
			CK = old.ColorKills;
			SM = old.SecretMissions;
		}
		else
		{
			cM = GM.maxMissionReached;
			cK = GM.maxMissionReachedKid;
			cH = GM.maxMissionReachedHard;
			cG = GM.maxMissionReachedGrandpa;
			tTK = GM.totalKills;
			tTD = GM.totalDefeats;
			tTW = GM.totalWins;
			hW = GM.highestWaves;
			tSK = GM.survivalTanksKilled;
			tKB = GM.totalKillsBounce;
			tRP = GM.totalRevivesPerformed;
			CK = GM.TankColorKilled;
			SM = GM.FoundSecretMissions;
		}
	}
}
