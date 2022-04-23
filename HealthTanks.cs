using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;

public class HealthTanks : MonoBehaviour
{
	public int health = 1;

	public int health_armour = 0;

	public int[] additionalDifficultyHealth;

	public int amountRevivesLeft = 1;

	public AudioClip Buzz;

	public GameObject deathExplosion;

	public GameObject revivePrefab;

	public GameObject deathCross;

	public bool isMainTank;

	public GameObject Clouds;

	public GameObject Armour;

	public GameObject[] PowerUps;

	public ParticleSystem brokenParticles;

	public ParticleSystem SkidMarkCreator;

	public ParticleSystem[] DeathLoadingParticles;

	public AudioClip DeathLoadingSound;

	public Transform shootPipe;

	public bool dying = false;

	public bool dyingBlinkingText = false;

	public AudioClip reviveSound;

	public GameObject reviveText;

	public bool immuneToExplosion = false;

	public int EnemyID = -1;

	public bool IsCustom = false;

	public bool IsArmoured = false;

	public bool isGary = false;

	public ShieldScript ShieldFade;

	public int maxHealth = 0;

	public int maxArmour = 0;

	public bool IsAirdropped = false;

	public bool canGetHurt = true;

	public bool IsHitByBullet = false;

	private bool ChargeDying = false;

	private float TimeAlive = 0f;

	private bool isDestroying = false;

	[Command("SetPlayerHealth", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SetPlayerHealth(int playerId, int Amount)
	{
		if (isMainTank)
		{
			MoveTankScript MTS = GetComponent<MoveTankScript>();
			if ((bool)MTS && MTS.playerId != playerId)
			{
			}
		}
	}

	private void Start()
	{
		if (isMainTank)
		{
			InvokeRepeating("BlinkingReviveText", 0.2f, 0.2f);
		}
		if (additionalDifficultyHealth.Length != 0 && OptionsMainMenu.instance.currentDifficulty < 3)
		{
			maxHealth = health + additionalDifficultyHealth[OptionsMainMenu.instance.currentDifficulty];
			health = maxHealth;
			maxArmour = health_armour;
		}
		if ((bool)GameMaster.instance.CM && isMainTank)
		{
			maxHealth = 99999;
			health = maxHealth;
		}
	}

	public void DamageMe(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			if (health_armour > 0)
			{
				health_armour--;
				int lengthClips = GlobalAssets.instance.AudioDB.ArmourHits.Length;
				int randomPick = Random.Range(0, lengthClips);
				SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.ArmourHits[randomPick], 1f, null);
			}
			else
			{
				health--;
			}
		}
	}

	private void Awake()
	{
		if ((bool)brokenParticles)
		{
			brokenParticles.gameObject.SetActive(value: false);
		}
		maxHealth = health;
		maxArmour = health_armour;
		SetArmourPlates();
	}

	private void OnEnable()
	{
		if ((bool)brokenParticles)
		{
			brokenParticles.gameObject.SetActive(value: false);
		}
		if (base.transform.tag == "Boss")
		{
			if (OptionsMainMenu.instance.currentDifficulty == 0)
			{
				if (health <= maxHealth / 3)
				{
					health = Mathf.RoundToInt(maxHealth / 3);
				}
				else if (health <= maxHealth / 2)
				{
					health = Mathf.RoundToInt(maxHealth / 2);
				}
				else
				{
					health = maxHealth;
				}
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 1)
			{
				if (health <= Mathf.RoundToInt(maxHealth / 2))
				{
					health = Mathf.RoundToInt(maxHealth / 2);
				}
				else
				{
					health = maxHealth;
				}
			}
			else
			{
				health = maxHealth;
			}
		}
		else
		{
			health = maxHealth;
		}
	}

	private void OnDisable()
	{
		if (IsAirdropped)
		{
			health = -999;
			EnemyTankDeath();
		}
	}

	private void BlinkingReviveText()
	{
		if (dying && dyingBlinkingText)
		{
			if (reviveText.activeSelf)
			{
				reviveText.SetActive(value: false);
			}
			else
			{
				reviveText.SetActive(value: true);
			}
		}
		else if (!dying)
		{
			dyingBlinkingText = false;
		}
	}

	public void SetArmourPlates()
	{
		if ((base.transform.tag == "Player" || base.transform.tag == "Enemy") && Armour != null && health_armour > 0)
		{
			float change = 0f;
			if (IsCustom && !IsArmoured)
			{
				Armour.SetActive(value: false);
				Armour.transform.GetChild(0).gameObject.SetActive(value: false);
				Armour.transform.GetChild(1).gameObject.SetActive(value: false);
				Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				if (GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: false);
					Armour.transform.GetChild(4).gameObject.SetActive(value: false);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				return;
			}
			if ((float)health_armour > 0f + change)
			{
				Armour.SetActive(value: true);
				if ((float)health_armour == 1f + change)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: false);
					Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				}
				if ((float)health_armour == 2f + change)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: true);
					Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				}
				if ((float)health_armour >= 3f + change)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: true);
					Armour.transform.GetChild(2).gameObject.SetActive(value: true);
				}
				if ((float)health_armour >= 4f + change && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: false);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				if ((float)health_armour >= 5f + change && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: true);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				if ((float)health_armour >= 6f + change && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: true);
					Armour.transform.GetChild(5).gameObject.SetActive(value: true);
				}
			}
			else
			{
				Armour.SetActive(value: false);
				Armour.transform.GetChild(0).gameObject.SetActive(value: false);
				Armour.transform.GetChild(1).gameObject.SetActive(value: false);
				Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				if (GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: false);
					Armour.transform.GetChild(4).gameObject.SetActive(value: false);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
			}
			if (!GameMaster.instance.isZombieMode)
			{
				Armour.transform.GetChild(3).gameObject.SetActive(value: false);
				Armour.transform.GetChild(4).gameObject.SetActive(value: false);
				Armour.transform.GetChild(5).gameObject.SetActive(value: false);
			}
		}
		else if (Armour != null)
		{
			Armour.SetActive(value: false);
			Armour.transform.GetChild(0).gameObject.SetActive(value: false);
			Armour.transform.GetChild(1).gameObject.SetActive(value: false);
			Armour.transform.GetChild(2).gameObject.SetActive(value: false);
			if (GameMaster.instance.isZombieMode && base.transform.tag == "Player")
			{
				Armour.transform.GetChild(3).gameObject.SetActive(value: false);
				Armour.transform.GetChild(4).gameObject.SetActive(value: false);
				Armour.transform.GetChild(5).gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		if (GameMaster.instance.inMenuMode)
		{
			TimeAlive += Time.deltaTime;
			if (TimeAlive >= 30f)
			{
				DamageMe(999);
			}
		}
		if (!canGetHurt || GameMaster.instance.inTankeyTown)
		{
			if (GameMaster.instance.inTankeyTown)
			{
				health = 999;
			}
			return;
		}
		if (!GameMaster.instance.GameHasStarted && IsAirdropped)
		{
			DamageMe(999);
			EnemyTankDeath();
			return;
		}
		if (GameMaster.instance.CurrentMission >= 99 && IsAirdropped)
		{
			if (GameMaster.instance.Bosses.Length < 1)
			{
				DamageMe(999);
				EnemyTankDeath();
				return;
			}
			if (GameMaster.instance.Bosses[0] == null)
			{
				DamageMe(999);
				EnemyTankDeath();
				return;
			}
		}
		if (GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted)
		{
			return;
		}
		if (EnemyID == 19 && health > 0)
		{
			GameMaster.instance.AssassinTankAlive = true;
		}
		if (amountRevivesLeft < 1 && isMainTank && GameMaster.instance.isZombieMode && !GameMaster.instance.GameHasStarted)
		{
			amountRevivesLeft = 1;
		}
		if (isMainTank && GameMaster.instance.isZombieMode && !GameMaster.instance.GameHasStarted)
		{
			health = maxHealth;
			health_armour = maxArmour;
		}
		if (!isMainTank && Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl))
		{
			DamageMe(999);
		}
		if (isMainTank && Input.GetKey(KeyCode.K) && GameMaster.instance.PlayerDied[0] && GameMaster.instance.PlayerModeWithAI[1] == 1 && (bool)GetComponent<EnemyAI>())
		{
			if ((bool)MapEditorMaster.instance)
			{
				if (GameMaster.instance.PlayerTeamColor[0] == GameMaster.instance.PlayerTeamColor[1] && GameMaster.instance.PlayerTeamColor[1] != 0)
				{
					DamageMe(196);
				}
			}
			else
			{
				DamageMe(196);
			}
		}
		if (health < 1 && !isMainTank && !ChargeDying)
		{
			ChargeDying = true;
			EnemyAI EA = GetComponent<EnemyAI>();
			if ((bool)EA)
			{
				if (EA.isLevel70Boss)
				{
					StartCoroutine(DelayExplosion());
					return;
				}
				EnemyTankDeath();
			}
			else
			{
				EnemyTankDeath();
			}
		}
		else if (health < 1 && isMainTank && !dying)
		{
			if (health > -99)
			{
				StartCoroutine(dyingState(instadie: false));
			}
			else
			{
				StartCoroutine(dyingState(instadie: true));
			}
		}
		SetArmourPlates();
		if (!GameMaster.instance)
		{
			return;
		}
		if (isGary && GameMaster.instance.AmountEnemyTanks < 2 && health > 0 && GameMaster.instance.CurrentMission == 43)
		{
			GameMaster.instance.NAS.extraWait = 2f;
			GameMaster.instance.AmountEnemyTanks = 0;
		}
		if (GameMaster.instance.inMenuMode)
		{
			int amountEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
			if (amountEnemies < 2)
			{
				DamageMe(50);
			}
		}
		if (!dying || (GameMaster.instance.Players.Count <= 1 && GameMaster.instance.PlayerModeWithAI[1] != 1))
		{
			return;
		}
		if (GameMaster.instance.AmountGoodTanks < 2)
		{
			StartCoroutine(dyingState(instadie: false));
		}
		for (int i = 0; i < GameMaster.instance.Players.Count; i++)
		{
			GameObject otherPlayer = GameMaster.instance.Players[i];
			if (!otherPlayer)
			{
				continue;
			}
			HealthTanks htscriptother = otherPlayer.GetComponent<HealthTanks>();
			float distanceToPlayer = Vector3.Distance(base.transform.position, otherPlayer.transform.position);
			if (distanceToPlayer <= 2f && htscriptother.health > 0)
			{
				reviveMe();
				MoveTankScript MTS = GetComponent<MoveTankScript>();
				EnemyAI CompanionEA = otherPlayer.GetComponent<EnemyAI>();
				if ((bool)CompanionEA)
				{
					CompanionEA.DownedPlayer = null;
				}
				if ((bool)MTS)
				{
					GameMaster.instance.PlayerDown[MTS.playerId] = false;
				}
				else
				{
					GameMaster.instance.PlayerDown[CompanionEA.CompanionID] = false;
				}
			}
		}
	}

	private IEnumerator DelayExplosion()
	{
		GameMaster.instance.musicScript.Orchestra.SetSongsVolumes(0);
		SFXManager.instance.PlaySFX(DeathLoadingSound, 1f, null);
		EnemyAI EA = GetComponent<EnemyAI>();
		EA.ETSN.enabled = false;
		EA.enabled = false;
		ParticleSystem[] deathLoadingParticles = DeathLoadingParticles;
		foreach (ParticleSystem PS in deathLoadingParticles)
		{
			PS.Play();
		}
		yield return new WaitForSeconds(3f);
		EnemyTankDeath();
	}

	private IEnumerator canHurtAgain(int sec)
	{
		canGetHurt = false;
		yield return new WaitForSeconds(sec);
		canGetHurt = true;
	}

	private void EnemyTankDeath()
	{
		if (isDestroying)
		{
			return;
		}
		if (EnemyID == 19)
		{
			GameMaster.instance.AssassinTankAlive = false;
		}
		BossVoiceLines BVL = GetComponent<BossVoiceLines>();
		if ((bool)BVL)
		{
			BVL.PlayDeathSound();
		}
		isDestroying = true;
		if (IsAirdropped && GameMaster.instance.AmountCalledInTanks > 0)
		{
			GameMaster.instance.AmountCalledInTanks--;
		}
		EnemyAI EA = GetComponent<EnemyAI>();
		Explosion();
		CameraShake CS = Camera.main.GetComponent<CameraShake>();
		if ((bool)CS)
		{
			CS.StartCoroutine(CS.Shake(0.1f, 0.06f));
		}
		if (GameMaster.instance != null)
		{
			if (GameMaster.instance.AmountEnemyTanks > 0 && !IsAirdropped)
			{
				GameMaster.instance.AmountEnemyTanks--;
			}
			if ((bool)EA && EA.MyTeam == 1 && !MapEditorMaster.instance && !EA.IsCompanion)
			{
				GameMaster.instance.AmountEnemyTanks++;
			}
			if (isGary && (bool)AchievementsTracker.instance)
			{
				AchievementsTracker.instance.GaryKilled = true;
			}
		}
		if (GameMaster.instance.isZombieMode)
		{
			GameMaster.instance.survivalTanksKilled++;
			int myIndex = ((EnemyID != -10) ? ((EnemyID == -11) ? 1 : ((EnemyID == -12) ? 2 : ((EnemyID == -13) ? 3 : ((EnemyID == -14) ? 4 : ((EnemyID == -15) ? 5 : ((EnemyID == -110) ? 9 : 0)))))) : 0);
			ZombieTankSpawner.instance.CurrentAmountOfEnemyTypes[myIndex]--;
			AccountMaster.instance.SaveCloudData(0, EnemyID, 0, bounceKill: false);
		}
		else if (!GameMaster.instance.inMenuMode && !GameMaster.instance.inMapEditor)
		{
			if ((bool)EA && EA.isShiny && (bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[34] != 1)
			{
				AchievementsTracker.instance.completeAchievementWithAI(34);
			}
			if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[15] != 1)
			{
				Debug.Log("ACHIEVEMENT 15!");
				AchievementsTracker.instance.completeAchievement(15);
			}
			if ((bool)EA)
			{
				if (!EA.isLevel10Boss && !EA.isLevel30Boss && !EA.isLevel50Boss && !EA.isLevel70Boss && !EA.isLevel100Boss && EnemyID != -1)
				{
					GameMaster.instance.TankColorKilled[EnemyID]++;
				}
				if (EA.isLevel10Boss || EA.isLevel30Boss)
				{
					if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[23] != 1)
					{
						AchievementsTracker.instance.completeAchievement(23);
					}
				}
				else if (EA.isLevel50Boss)
				{
					if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[25] != 1)
					{
						AchievementsTracker.instance.completeAchievement(25);
					}
					if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[30] != 1 && OptionsMainMenu.instance.currentDifficulty > 1)
					{
						AchievementsTracker.instance.completeAchievement(30);
					}
				}
				else if (EA.isLevel70Boss && (bool)AchievementsTracker.instance && !AchievementsTracker.instance.HasBeenStunnedByBoss && OptionsMainMenu.instance.AM[11] != 1)
				{
					AchievementsTracker.instance.completeAchievement(11);
				}
				if ((EA.isLevel10Boss || EA.isLevel30Boss || EA.isLevel50Boss || EA.isLevel70Boss || EA.isLevel100Boss) && !IsHitByBullet && OptionsMainMenu.instance.AM[16] != 1)
				{
					AchievementsTracker.instance.completeAchievement(16);
				}
				if (EA.isLevel100Boss)
				{
					if (OptionsMainMenu.instance.AM[0] != 1)
					{
						AchievementsTracker.instance.completeAchievement(0);
					}
					if (OptionsMainMenu.instance.currentDifficulty >= 2 && OptionsMainMenu.instance.AM[4] != 1)
					{
						AchievementsTracker.instance.completeAchievementWithAI(4);
					}
					if (OptionsMainMenu.instance.currentDifficulty >= 1 && OptionsMainMenu.instance.AM[27] != 1 && AchievementsTracker.instance.StartedFromBegin)
					{
						AchievementsTracker.instance.completeAchievement(27);
					}
					if (OptionsMainMenu.instance.AM[10] != 1 && !AchievementsTracker.instance.HasBeenHit && AchievementsTracker.instance.StartedFromBegin)
					{
						AchievementsTracker.instance.completeAchievement(10);
					}
				}
			}
			GameMaster.instance.totalKills++;
		}
		if (GetComponent<NewAIagent>() != null)
		{
			NewAIagent agent = GetComponent<NewAIagent>();
			if (agent.source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
				agent.source.volume = 0.5f;
				agent.source.clip = null;
				agent.source.loop = false;
				agent.source.Stop();
			}
		}
		if (GetComponent<EnemyAI>() != null)
		{
			EnemyAI AI = GetComponent<EnemyAI>();
			if (AI.source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
				AI.source.volume = 0.5f;
				AI.source.clip = null;
				AI.source.loop = false;
				AI.source.Stop();
			}
		}
		SkidMarkCreator.transform.parent = null;
		SkidMarkCreator.Stop();
		float tracksRemoveTime = ((OptionsMainMenu.instance.currentGraphicSettings == 0) ? 15f : ((OptionsMainMenu.instance.currentGraphicSettings == 1) ? 30f : ((OptionsMainMenu.instance.currentGraphicSettings == 2) ? 60f : ((OptionsMainMenu.instance.currentGraphicSettings == 3) ? 120f : ((OptionsMainMenu.instance.currentGraphicSettings == 4) ? 240f : 300f)))));
		Object.Destroy(SkidMarkCreator, tracksRemoveTime);
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDie);
		Collider[] bigobjectsInRange = Physics.OverlapSphere(base.transform.position, 4f);
		Collider[] array = bigobjectsInRange;
		foreach (Collider col in array)
		{
			Rigidbody rigi = col.GetComponent<Rigidbody>();
			if (rigi != null && (col.tag == "Player" || col.tag == "Enemy"))
			{
				float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
				float force = (7f - distance) * 2f;
				Vector3 direction = rigi.transform.position - base.transform.position;
				rigi.AddForce(direction * force, ForceMode.Impulse);
			}
		}
		GameObject deathcross = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		EnemyAI AIscript = GetComponent<EnemyAI>();
		if ((bool)AIscript && (AIscript.isLevel10Boss || AIscript.isLevel30Boss || AIscript.isLevel50Boss || AIscript.isLevel70Boss || AIscript.isLevel100Boss))
		{
			deathcross.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		}
		if (!GameMaster.instance.CM)
		{
			deathcross.transform.parent = null;
		}
		else
		{
			deathcross.transform.parent = GameMaster.instance.CM.CanvasPaper.transform;
		}
		if ((bool)MapEditorMaster.instance)
		{
			GameMaster.instance.StartCoroutine(GameMaster.instance.GetTankTeamData(fast: false));
		}
		if (GameMaster.instance.CurrentMission == 99)
		{
			if (GameMaster.instance.Enemies != null)
			{
				GameMaster.instance.Enemies.Clear();
			}
			GameMaster.instance.Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
			if (GameMaster.instance.Enemies.Count == 1)
			{
				GameMaster.instance.Enemies = null;
				GameMaster.instance.AmountEnemyTanks = 1;
			}
			else
			{
				GameMaster.instance.AmountEnemyTanks = GameMaster.instance.Enemies.Count;
			}
		}
		if ((bool)AIscript)
		{
			if (AIscript.isLevel100Boss)
			{
				GameMaster.instance.totalWins++;
				AccountMaster.instance.SaveCloudData(1, 1, 0, bounceKill: false);
				GameMaster.instance.SaveData(skipCloud: false);
				int MarblesToAdd = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 10 : ((OptionsMainMenu.instance.currentDifficulty == 1) ? 20 : ((OptionsMainMenu.instance.currentDifficulty == 2) ? 30 : 40)));
				AccountMaster.instance.IncreaseMarbles(MarblesToAdd);
				GameMaster.instance.GameHasStarted = false;
				KingTankScript KTS = base.transform.parent.gameObject.GetComponent<KingTankScript>();
				if ((bool)KTS)
				{
					KTS.StartCoroutine(KTS.DestroyedBoss());
				}
			}
			else
			{
				Object.Destroy(base.transform.parent.gameObject);
			}
		}
		else
		{
			Object.Destroy(base.transform.parent.gameObject);
		}
	}

	private void reviveMe()
	{
		StartCoroutine(canHurtAgain(1));
		reviveText.SetActive(value: false);
		dying = false;
		MoveTankScript MTS = GetComponent<MoveTankScript>();
		if ((bool)MTS)
		{
			GameMaster.instance.PlayerDown[MTS.playerId] = false;
		}
		else
		{
			EnemyAI EA = GetComponent<EnemyAI>();
			if ((bool)EA)
			{
				GameMaster.instance.PlayerDown[EA.CompanionID] = false;
			}
		}
		if (GameMaster.instance.PlayerModeWithAI[1] != 1)
		{
			GameMaster.instance.totalRevivesPerformed++;
		}
		GameMaster.instance.AmountPlayersThatNeedRevive--;
		brokenParticles.gameObject.SetActive(value: false);
		health = 1;
		SFXManager.instance.PlaySFX(reviveSound);
		GameObject revive = Object.Instantiate(revivePrefab, base.transform.position, Quaternion.identity);
		Object.Destroy(revive, 2f);
		reviveText.SetActive(value: false);
	}

	private void removePlayer(int playerID)
	{
		GameMaster.instance.StartCoroutine("PlayerleftCooldownTimer");
		GameMaster.instance.PlayerJoined[playerID] = false;
		GameMaster.instance.PlayerJoined[playerID] = false;
		Explosion();
		SkidMarkCreator.transform.parent = null;
		SkidMarkCreator.Stop();
		float tracksRemoveTime = ((OptionsMainMenu.instance.currentGraphicSettings == 0) ? 15f : ((OptionsMainMenu.instance.currentGraphicSettings == 1) ? 30f : ((OptionsMainMenu.instance.currentGraphicSettings == 2) ? 60f : ((OptionsMainMenu.instance.currentGraphicSettings == 3) ? 120f : ((OptionsMainMenu.instance.currentGraphicSettings == 4) ? 240f : 300f)))));
		Object.Destroy(SkidMarkCreator, tracksRemoveTime);
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDie);
		CameraShake CS = Camera.main.GetComponent<CameraShake>();
		if ((bool)CS)
		{
			CS.StartCoroutine(CS.Shake(0.1f, 0.15f));
		}
		GameMaster.instance.AmountGoodTanks--;
		GameObject deathcross = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		DeathCrossScript DCS = deathcross.GetComponent<DeathCrossScript>();
		MoveTankScript MTS = GetComponent<MoveTankScript>();
		if (!MTS.isPlayer2)
		{
			DCS.IsBlue();
		}
		else
		{
			DCS.IsRed();
		}
		deathcross.transform.parent = null;
		Object.Destroy(base.transform.parent.gameObject);
	}

	private void Explosion()
	{
		GameObject poof = Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity);
		poof.GetComponent<ParticleSystem>().Play();
		ParticleSystemRenderer poofPS = null;
		ParticleSystemRenderer poofPS2 = null;
		foreach (Transform child2 in poof.transform)
		{
			if (child2.name == "Schrapnel")
			{
				poofPS = child2.GetComponent<ParticleSystemRenderer>();
			}
			if (child2.name == "Schrapnel2")
			{
				poofPS2 = child2.GetComponent<ParticleSystemRenderer>();
			}
		}
		if (OptionsMainMenu.instance.AMselected.Contains(58) && (bool)GlobalHealthTanks.instance)
		{
			GameObject blood = Object.Instantiate(GlobalHealthTanks.instance.BloodSplatters[Random.Range(0, GlobalHealthTanks.instance.BloodSplatters.Length)], base.transform.position + new Vector3(0f, 0.06f, 0f), Quaternion.identity, null);
			int scream = Random.Range(0, 3);
			if (scream == 1)
			{
				SFXManager.instance.PlaySFX(GlobalHealthTanks.instance.InPain[Random.Range(0, GlobalHealthTanks.instance.InPain.Length)], 1f, null);
			}
		}
		EnemyAI EA = GetComponent<EnemyAI>();
		if ((bool)EA)
		{
			if (EA.isAggro && (bool)EA.AngerVein && EA.AngerVein.activeSelf)
			{
				GameMaster.instance.musicScript.Orchestra.RagingCherries--;
			}
			if (EnemyID == 4 && !IsCustom && (bool)EA.ETSN)
			{
				EA.ETSN.PlaceMine(deathmine: true);
			}
		}
		Material[] bodyM = new Material[1];
		bool foundChild = false;
		foreach (Transform child in base.transform)
		{
			if (child.name == "Cube.003")
			{
				bodyM = child.GetComponent<MeshRenderer>().materials;
				foundChild = true;
			}
		}
		if (bodyM.Length > 1 && foundChild)
		{
			if (bodyM[2] != null)
			{
				poofPS.material = bodyM[2];
				if ((bool)poofPS2)
				{
					poofPS2.material = bodyM[2];
				}
			}
			else if (bodyM[1] != null)
			{
				poofPS.material = bodyM[1];
				if ((bool)poofPS2)
				{
					poofPS2.material = bodyM[2];
				}
			}
		}
		else if (foundChild)
		{
			poofPS.material.color = bodyM[0].color;
			if ((bool)poofPS2)
			{
				poofPS2.material.color = bodyM[0].color;
			}
		}
		Object.Destroy(poof.gameObject, 5f);
	}

	private IEnumerator activateBlinkingText()
	{
		yield return new WaitForSeconds(4f);
		if (dying)
		{
			dyingBlinkingText = true;
		}
	}

	private IEnumerator dyingState(bool instadie)
	{
		if (dying)
		{
			yield break;
		}
		if ((bool)AchievementsTracker.instance)
		{
			AchievementsTracker.instance.HasBeenHit = true;
		}
		MoveTankScript MTS = GetComponent<MoveTankScript>();
		EnemyAI AIscript = GetComponent<EnemyAI>();
		int myteamnumber = ((!MTS) ? AIscript.MyTeam : MTS.MyTeam);
		Debug.Log("dying!" + health + instadie + myteamnumber);
		if (myteamnumber != 0 && !instadie)
		{
			bool teammemberfound = false;
			foreach (GameObject Player in GameMaster.instance.Players)
			{
				if (Player == base.gameObject)
				{
					continue;
				}
				MoveTankScript otherMTS = Player.GetComponent<MoveTankScript>();
				if ((bool)otherMTS)
				{
					if (otherMTS.MyTeam == myteamnumber)
					{
						instadie = false;
						teammemberfound = true;
					}
					continue;
				}
				EnemyAI otherAIscript = Player.GetComponent<EnemyAI>();
				if ((bool)otherAIscript && otherAIscript.MyTeam == myteamnumber)
				{
					instadie = false;
					teammemberfound = true;
				}
			}
			if (!teammemberfound)
			{
				instadie = true;
			}
		}
		else
		{
			instadie = true;
		}
		if (GameMaster.instance.Players.Count < 2)
		{
			instadie = true;
		}
		if (GameMaster.instance.AmountPlayersThatNeedRevive > 0 && GameMaster.instance.Players.Count < 3)
		{
			instadie = true;
		}
		if (!instadie && amountRevivesLeft > 0)
		{
			SFXManager.instance.PlaySFX(Buzz);
			reviveText.SetActive(value: true);
			dying = true;
			GameMaster.instance.AmountPlayersThatNeedRevive++;
			if (base.name == "AI_Tank_FBX")
			{
				GameMaster.instance.PlayerDown[AIscript.CompanionID] = true;
			}
			else if ((bool)MTS)
			{
				GameMaster.instance.PlayerDown[MTS.playerId] = true;
			}
			amountRevivesLeft--;
			brokenParticles.gameObject.SetActive(value: true);
			StartCoroutine(activateBlinkingText());
			yield return new WaitForSeconds(6f);
			Debug.LogError("done dying");
			if (!dying)
			{
				if ((bool)AIscript)
				{
					AIscript.TankSpeed = AIscript.OriginalTankSpeed;
					if (AIscript.TankSpeed > 20f)
					{
						AIscript.CanMove = AIscript.couldMove;
					}
				}
				if (base.name == "AI_Tank_FBX")
				{
					GameMaster.instance.PlayerDown[AIscript.CompanionID] = false;
				}
				else if ((bool)MTS)
				{
					GameMaster.instance.PlayerDown[MTS.playerId] = false;
				}
				yield break;
			}
		}
		Explosion();
		if ((bool)SkidMarkCreator)
		{
			SkidMarkCreator.transform.parent = null;
			SkidMarkCreator.Stop();
			Object.Destroy(t: (OptionsMainMenu.instance.currentGraphicSettings == 0) ? 15f : ((OptionsMainMenu.instance.currentGraphicSettings == 1) ? 30f : ((OptionsMainMenu.instance.currentGraphicSettings == 2) ? 60f : ((OptionsMainMenu.instance.currentGraphicSettings == 3) ? 120f : ((OptionsMainMenu.instance.currentGraphicSettings == 4) ? 240f : 300f)))), obj: SkidMarkCreator);
		}
		if (GameMaster.instance != null)
		{
			if (GameMaster.instance.Players.Count <= 1)
			{
				if (GameMaster.instance.Lives > 1)
				{
					SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDieLostGame);
				}
				else
				{
					SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDie);
				}
				GameMaster.instance.AmountGoodTanks--;
				GameMaster.instance.OnlyCompanionLeft = false;
			}
			else
			{
				if (GameMaster.instance.PlayerModeWithAI[1] == 1 && base.name == "Main_Tank_FBX_body")
				{
					GameMaster.instance.OnlyCompanionLeft = true;
				}
				if (GameMaster.instance.CM == null)
				{
					SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDie);
				}
				GameMaster.instance.AmountGoodTanks--;
			}
		}
		GameObject deathcross = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		DeathCrossScript DCS = deathcross.GetComponent<DeathCrossScript>();
		Collider[] bigobjectsInRange = Physics.OverlapSphere(base.transform.position, 4f);
		Collider[] array = bigobjectsInRange;
		foreach (Collider col in array)
		{
			Rigidbody rigi = col.GetComponent<Rigidbody>();
			if (rigi != null && (col.tag == "Player" || col.tag == "Enemy"))
			{
				float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
				float force = (7f - distance) * 2f;
				Vector3 direction = rigi.transform.position - base.transform.position;
				rigi.AddForce(direction * force, ForceMode.Impulse);
			}
		}
		if (MTS != null)
		{
			if (!instadie && dying)
			{
				GameMaster.instance.AmountPlayersThatNeedRevive--;
			}
			if (MTS.playerId == 0)
			{
				DCS.IsBlue();
				GameMaster.instance.PlayerDied[0] = true;
				GameMaster.instance.PlayerDown[0] = false;
			}
			else if (MTS.playerId == 1)
			{
				DCS.IsRed();
				GameMaster.instance.PlayerDied[1] = true;
				GameMaster.instance.PlayerDown[1] = false;
			}
			else if (MTS.playerId == 2)
			{
				DCS.IsGreen();
				GameMaster.instance.PlayerDied[2] = true;
				GameMaster.instance.PlayerDown[2] = false;
			}
			else if (MTS.playerId == 3)
			{
				DCS.IsPurple();
				GameMaster.instance.PlayerDied[3] = true;
				GameMaster.instance.PlayerDown[3] = false;
			}
		}
		else
		{
			if (!instadie && dying)
			{
				GameMaster.instance.AmountPlayersThatNeedRevive--;
			}
			if (AIscript != null && AIscript.IsCompanion)
			{
				if (AIscript.CompanionID == 1)
				{
					DCS.IsRed();
				}
				else if (AIscript.CompanionID == 2)
				{
					DCS.IsGreen();
				}
				else if (AIscript.CompanionID == 3)
				{
					DCS.IsPurple();
				}
				GameMaster.instance.PlayerDied[AIscript.CompanionID] = true;
				GameMaster.instance.PlayerDown[AIscript.CompanionID] = false;
			}
		}
		if (GameMaster.instance.isZombieMode)
		{
			if (MTS.playerId == 0)
			{
				GameMaster.instance.PKU.ResetAll(1);
			}
			else if (MTS.playerId == 1)
			{
				GameMaster.instance.PKU.ResetAll(2);
			}
			else if (MTS.playerId == 2)
			{
				GameMaster.instance.PKU.ResetAll(3);
			}
			else if (MTS.playerId == 3)
			{
				GameMaster.instance.PKU.ResetAll(4);
			}
			MTS.Upgrades[0] = 0;
			MTS.Upgrades[1] = 0;
			MTS.Upgrades[2] = 0;
			MTS.Upgrades[3] = 0;
		}
		deathcross.transform.parent = null;
		if (GameMaster.instance.CurrentMission >= 99 && GameMaster.instance.Players.Count < 2)
		{
			if (MissionHundredController.instance != null && (bool)MissionHundredController.instance.KTS)
			{
				HealthTanks HT = MissionHundredController.instance.KTS.HT;
				if ((bool)HT && HT.health <= 11 && OptionsMainMenu.instance.AM[26] != 1)
				{
					AchievementsTracker.instance.completeAchievement(26);
				}
			}
			if (GameMaster.instance.Bosses.Length < 1)
			{
				GameMaster.instance.Lives = 0;
			}
			else if (GameMaster.instance.Bosses[0] == null)
			{
				GameMaster.instance.Lives = 0;
			}
		}
		if ((bool)MapEditorMaster.instance)
		{
			GameMaster.instance.StartCoroutine(GameMaster.instance.GetTankTeamData(fast: false));
		}
		Object.Destroy(base.transform.parent.gameObject);
	}
}
