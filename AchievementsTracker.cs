using UnityEngine;

public class AchievementsTracker : MonoBehaviour
{
	private static AchievementsTracker _instance;

	private bool resetted = false;

	public AudioClip AchievementCompleted;

	public bool StartedFromBegin = false;

	public bool Moved = false;

	public bool KilledWithoutBounce = false;

	public bool HasShooted = false;

	public bool HasShootedThisRound = false;

	public bool HasShotBoss = false;

	public bool HasMissed = false;

	public bool BoughtUpgrade = false;

	public bool HasBeenHit = false;

	public bool GaryKilled = false;

	public bool HasBeenStunnedByBoss = false;

	public GameObject AchievementNotification;

	private GameObject AchievementNotificationParent;

	public int[] MissionUnlocked;

	public string[] UnlockedMessage;

	public int selectedDifficulty = 0;

	public static AchievementsTracker instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		InvokeRepeating("SearchParent", 2f, 2f);
		if (OptionsMainMenu.instance.StartLevel < 9)
		{
			StartedFromBegin = true;
		}
	}

	private void SearchParent()
	{
		if (AchievementNotificationParent == null)
		{
			AchievementNotificationParent = GameObject.FindGameObjectWithTag("AchievementNotificationParent");
		}
		bool failed = false;
		for (int i = 0; i < 34; i++)
		{
			if (OptionsMainMenu.instance.AM[i] != 1 && i != 14)
			{
				failed = true;
			}
		}
		if (!failed && OptionsMainMenu.instance.AM[14] != 1)
		{
			completeAlwaysAchievement(14);
		}
	}

	public void ResetVariables()
	{
		resetted = true;
		Moved = false;
		KilledWithoutBounce = false;
		HasShooted = false;
		HasShotBoss = false;
		HasMissed = false;
		StartedFromBegin = false;
		BoughtUpgrade = false;
		HasBeenHit = false;
		HasBeenStunnedByBoss = false;
		HasShootedThisRound = false;
		GaryKilled = false;
	}

	private void Update()
	{
		if (!GameMaster.instance)
		{
			return;
		}
		if (!GameMaster.instance.GameHasStarted && HasShootedThisRound && GameMaster.instance.AmountEnemyTanks < 1 && GameMaster.instance.AmountGoodTanks < 1)
		{
			HasShootedThisRound = false;
		}
		if (OptionsMainMenu.instance.StartLevel < 4 && GameMaster.instance.CurrentMission < 4)
		{
			StartedFromBegin = true;
		}
		if (GameMaster.instance.inMenuMode && !resetted)
		{
			ResetVariables();
		}
		else if (resetted)
		{
			resetted = false;
		}
		if (GameMaster.instance.PlayerModeWithAI[1] != 1 && !GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode && MapEditorMaster.instance == null)
		{
			if (GameMaster.instance.CurrentMission > 9 && !Moved && StartedFromBegin && OptionsMainMenu.instance.AM[1] != 1)
			{
				completeAchievement(1);
			}
			else if (GameMaster.instance.CurrentMission > 29 && StartedFromBegin && !KilledWithoutBounce && OptionsMainMenu.instance.AM[2] != 1)
			{
				completeAchievement(2);
			}
			else if (GameMaster.instance.CurrentMission > 9 && StartedFromBegin && !HasShooted && OptionsMainMenu.instance.AM[3] != 1)
			{
				completeAchievement(3);
			}
			else if (GameMaster.instance.CurrentMission > 49 && StartedFromBegin && !HasBeenHit && OptionsMainMenu.instance.AM[6] != 1)
			{
				completeAchievement(6);
			}
			else if (GameMaster.instance.counter < 1200f && StartedFromBegin && GameMaster.instance.CurrentMission > 19 && OptionsMainMenu.instance.AM[5] != 1)
			{
				completeAchievement(5);
			}
			else if (GameMaster.instance.CurrentMission > 9 && StartedFromBegin && !HasMissed && OptionsMainMenu.instance.AM[17] != 1)
			{
				completeAchievement(17);
			}
			else if (GameMaster.instance.totalKills > 999 && OptionsMainMenu.instance.AM[24] != 1)
			{
				completeAchievement(24);
			}
			else if (GameMaster.instance.totalKillsBounce > 99 && OptionsMainMenu.instance.AM[18] != 1)
			{
				completeAchievement(18);
			}
			else if (GameMaster.instance.totalRevivesPerformed > 49 && OptionsMainMenu.instance.AM[21] != 1)
			{
				completeAchievement(21);
			}
			else if (GameMaster.instance.totalKillsBounce > 249 && OptionsMainMenu.instance.AM[29] != 1)
			{
				completeAchievement(29);
			}
			else if (GameMaster.instance.TankColorKilled[12] > 99 && OptionsMainMenu.instance.AM[22] != 1)
			{
				completeAchievement(22);
			}
			else if (OptionsMainMenu.instance.CompletedCustomCampaigns > 19 && OptionsMainMenu.instance.AM[9] != 1)
			{
				completeAchievement(9);
			}
			else if (GameMaster.instance.CurrentMission == 44 && !GaryKilled && OptionsMainMenu.instance.AM[31] != 1)
			{
				completeAchievement(31);
			}
		}
		else if (GameMaster.instance.PlayerModeWithAI[1] != 1 && !GameMaster.instance.inMapEditor && GameMaster.instance.isZombieMode)
		{
			if ((bool)ZombieTankSpawner.instance)
			{
				if (ZombieTankSpawner.instance.Wave > 9 && !BoughtUpgrade && OptionsMainMenu.instance.AM[28] != 1)
				{
					completeOtherAchievement(28);
				}
				else if (ZombieTankSpawner.instance.Wave > 19 && OptionsMainMenu.instance.AM[7] != 1)
				{
					completeOtherAchievement(7);
				}
			}
		}
		else if (GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode)
		{
			if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.Levels.Count > 9 && OptionsMainMenu.instance.AM[8] != 1)
			{
				completeOtherAchievement(8);
			}
		}
		else if (!GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode && MapEditorMaster.instance == null && GameMaster.instance.totalRevivesPerformed > 49 && OptionsMainMenu.instance.AM[21] != 1)
		{
			completeAchievementWithAI(21);
		}
	}

	public void ShowUnlockMessage(int ID, int count)
	{
		if (GameMaster.instance.inMapEditor || GameMaster.instance.isZombieMode)
		{
			return;
		}
		GameObject Notification = Object.Instantiate(AchievementNotification);
		GameObject[] Fireworks = GameObject.FindGameObjectsWithTag("Firework");
		GameMaster.instance.NAS.extraWait = 2f;
		GameMaster.instance.NAS.ResetExtraWait();
		GameObject[] array = Fireworks;
		foreach (GameObject Firework in array)
		{
			AchievementFireWork AFW = Firework.GetComponent<AchievementFireWork>();
			if ((bool)AFW)
			{
				AFW.NewAchievement();
			}
		}
		Notification.transform.parent = AchievementNotificationParent.transform;
		Notification.transform.localPosition = new Vector3(0f, -350f, 0f);
		AchievementItemScript AIS = Notification.GetComponent<AchievementItemScript>();
		SFXManager.instance.PlaySFX(AchievementCompleted, 1f, null);
		if ((bool)AIS)
		{
			AIS.message = UnlockedMessage[count];
			AIS.AMID = count;
		}
	}

	public void completeAchievement(int ID)
	{
		if (GameMaster.instance.PlayerModeWithAI[1] == 1 || GameMaster.instance.PlayerModeWithAI[2] == 1 || GameMaster.instance.PlayerModeWithAI[3] == 1 || GameMaster.instance.inMapEditor || GameMaster.instance.isZombieMode)
		{
			Debug.Log("Achievement " + ID);
		}
		else
		{
			if (OptionsMainMenu.instance.AM[ID] == 1)
			{
				return;
			}
			Debug.Log("COMPLETING: " + ID);
			OptionsMainMenu.instance.AM[ID] = 1;
			OptionsMainMenu.instance.SaveNewData();
			AccountMaster.instance.SaveCloudData(3, ID, 0, bounceKill: false);
			GameObject Notification = Object.Instantiate(AchievementNotification);
			GameObject[] Fireworks = GameObject.FindGameObjectsWithTag("Firework");
			if ((bool)GameMaster.instance.NAS)
			{
				GameMaster.instance.NAS.extraWait = 2f;
				GameMaster.instance.NAS.ResetExtraWait();
			}
			GameObject[] array = Fireworks;
			foreach (GameObject Firework in array)
			{
				AchievementFireWork AFW = Firework.GetComponent<AchievementFireWork>();
				if ((bool)AFW)
				{
					AFW.NewAchievement();
				}
			}
			Notification.transform.parent = AchievementNotificationParent.transform;
			Notification.transform.localPosition = new Vector3(0f, -350f, 0f);
			AchievementItemScript AIS = Notification.GetComponent<AchievementItemScript>();
			SFXManager.instance.PlaySFX(AchievementCompleted, 1f, null);
			if (AccountMaster.instance.isSignedIn)
			{
				AccountMaster.instance.IncreaseMarbles(OptionsMainMenu.instance.AM_marbles[ID]);
			}
			if ((bool)AIS)
			{
				AIS.AMID = ID;
			}
		}
	}

	public void completeAchievementWithAI(int ID)
	{
		if (GameMaster.instance.inMapEditor || GameMaster.instance.isZombieMode)
		{
			Debug.Log("AI.. Achievement " + ID);
		}
		else
		{
			if (OptionsMainMenu.instance.AM[ID] == 1 || !AccountMaster.instance.isSignedIn)
			{
				return;
			}
			Debug.Log("AI COMPLETING: " + ID);
			AccountMaster.instance.SaveCloudData(3, ID, 0, bounceKill: false);
			OptionsMainMenu.instance.AM[ID] = 1;
			OptionsMainMenu.instance.SaveNewData();
			GameObject Notification = Object.Instantiate(AchievementNotification);
			GameObject[] Fireworks = GameObject.FindGameObjectsWithTag("Firework");
			GameMaster.instance.NAS.extraWait = 2f;
			GameMaster.instance.NAS.ResetExtraWait();
			GameObject[] array = Fireworks;
			foreach (GameObject Firework in array)
			{
				AchievementFireWork AFW = Firework.GetComponent<AchievementFireWork>();
				if ((bool)AFW)
				{
					AFW.NewAchievement();
				}
			}
			Notification.transform.parent = AchievementNotificationParent.transform;
			Notification.transform.localPosition = new Vector3(0f, -350f, 0f);
			AchievementItemScript AIS = Notification.GetComponent<AchievementItemScript>();
			SFXManager.instance.PlaySFX(AchievementCompleted, 1f, null);
			if ((bool)AIS)
			{
				AIS.AMID = ID;
			}
		}
	}

	public void completeOtherAchievement(int ID)
	{
		if (GameMaster.instance.PlayerModeWithAI[1] == 1)
		{
			Debug.Log("Other Achievement " + ID);
		}
		else if (OptionsMainMenu.instance.AM[ID] != 1 && AccountMaster.instance.isSignedIn)
		{
			Debug.Log("OTHER COMPLETING: " + ID);
			AccountMaster.instance.SaveCloudData(3, ID, 0, bounceKill: false);
			OptionsMainMenu.instance.AM[ID] = 1;
			GameObject Notification = Object.Instantiate(AchievementNotification);
			Notification.transform.parent = AchievementNotificationParent.transform;
			Notification.transform.localPosition = new Vector3(0f, -350f, 0f);
			SFXManager.instance.PlaySFX(AchievementCompleted, 1f, null);
			AchievementItemScript AIS = Notification.GetComponent<AchievementItemScript>();
			if ((bool)AIS)
			{
				Debug.Log("SETTINGS ID!!" + ID);
				AIS.AMID = ID;
			}
		}
	}

	public void completeAlwaysAchievement(int ID)
	{
		if (OptionsMainMenu.instance.AM[ID] == 1)
		{
			return;
		}
		Debug.Log("ALWAYS COMPLETING: " + ID);
		if (AccountMaster.instance.isSignedIn)
		{
			AccountMaster.instance.SaveCloudData(3, ID, 0, bounceKill: false);
			OptionsMainMenu.instance.AM[ID] = 1;
			GameObject Notification = Object.Instantiate(AchievementNotification);
			Notification.transform.parent = AchievementNotificationParent.transform;
			Notification.transform.localPosition = new Vector3(0f, -350f, 0f);
			SFXManager.instance.PlaySFX(AchievementCompleted, 1f, null);
			AchievementItemScript AIS = Notification.GetComponent<AchievementItemScript>();
			if ((bool)AIS)
			{
				Debug.Log("SETTINGS ID!!" + ID);
				AIS.AMID = ID;
			}
		}
	}
}
