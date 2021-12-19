using System;
using System.Collections.Generic;

[Serializable]
public class ProgressData
{
	public int completedMissions;

	public int completedKid;

	public int completedHard;

	public int completedGrandpa;

	public int totalTankKills;

	public int totalTimesDefeated;

	public int totalTimesWon;

	public int totalKillsBounced;

	public int[] highestWaves;

	public int totalSurvivalKills;

	public int totalRevivesPerformed;

	public int[] ColorKills;

	public List<int> SecretMissions = new List<int>();

	public ProgressData(GameMaster GM)
	{
	}
}
