using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthTanks : MonoBehaviour
{
	public int health = 1;

	public int health_armour;

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

	public bool dying;

	public bool dyingBlinkingText;

	public AudioClip reviveSound;

	public GameObject reviveText;

	public bool immuneToExplosion;

	public int EnemyID = -1;

	public bool IsCustom;

	public bool IsArmoured;

	public bool isGary;

	public ShieldScript ShieldFade;

	public int maxHealth;

	public int maxArmour;

	public bool IsAirdropped;

	public bool canGetHurt = true;

	public bool DoAreaDamageWhenDie;

	public float AreaDamageRange = 6f;

	public bool IsHitByBullet;

	private bool ChargeDying;

	private float TimeAlive;

	private bool isDestroying;

	[Command("SetPlayerHealth", Platform.AllPlatforms, MonoTargetType.Single)]
	public void SetPlayerHealth(int playerId, int Amount)
	{
		if (isMainTank)
		{
			MoveTankScript component = GetComponent<MoveTankScript>();
			if ((bool)component)
			{
				_ = component.playerId;
			}
		}
	}

	private void Start()
	{
		if (isMainTank)
		{
			InvokeRepeating("BlinkingReviveText", 0.2f, 0.2f);
		}
		if (additionalDifficultyHealth.Length != 0)
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
		if (!canGetHurt)
		{
			return;
		}
		for (int i = 0; i < amount; i++)
		{
			if (health_armour > 0)
			{
				health_armour--;
				int maxExclusive = GlobalAssets.instance.AudioDB.ArmourHits.Length;
				int num = Random.Range(0, maxExclusive);
				SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.ArmourHits[num], 1f, null);
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
			float num = 0f;
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
			if ((float)health_armour > 0f + num)
			{
				Armour.SetActive(value: true);
				if ((float)health_armour == 1f + num)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: false);
					Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				}
				if ((float)health_armour == 2f + num)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: true);
					Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				}
				if ((float)health_armour >= 3f + num)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: true);
					Armour.transform.GetChild(2).gameObject.SetActive(value: true);
				}
				if ((float)health_armour >= 4f + num && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: false);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				if ((float)health_armour >= 5f + num && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: true);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				if ((float)health_armour >= 6f + num && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
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
		if (GameMaster.instance.CurrentMission >= 99 && GameMaster.instance.isOfficialCampaign && isMainTank)
		{
			if (GameMaster.instance.Bosses.Length < 1)
			{
				health = 999;
				return;
			}
			if (GameMaster.instance.Bosses[0] == null)
			{
				health = 999;
				return;
			}
		}
		if (!GameMaster.instance.GameHasStarted && IsAirdropped)
		{
			DamageMe(999);
			EnemyTankDeath();
			return;
		}
		if (GameMaster.instance.CurrentMission >= 99 && IsAirdropped && !MapEditorMaster.instance && GameMaster.instance.isOfficialCampaign)
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
			EnemyAI component = GetComponent<EnemyAI>();
			if ((bool)component)
			{
				if (component.isLevel70Boss)
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
		if (isGary && GameMaster.instance.AmountEnemyTanks < 2 && health > 0 && GameMaster.instance.CurrentMission == 43 && !MapEditorMaster.instance)
		{
			GameMaster.instance.NAS.extraWait = 2f;
			GameMaster.instance.AmountEnemyTanks = 0;
		}
		if (GameMaster.instance.inMenuMode && GameObject.FindGameObjectsWithTag("Enemy").Length < 2)
		{
			DamageMe(50);
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
			GameObject gameObject = GameMaster.instance.Players[i];
			if (!gameObject)
			{
				continue;
			}
			HealthTanks component2 = gameObject.GetComponent<HealthTanks>();
			if (Vector3.Distance(base.transform.position, gameObject.transform.position) <= 2f && component2.health > 0)
			{
				reviveMe();
				MoveTankScript component3 = GetComponent<MoveTankScript>();
				EnemyAI component4 = gameObject.GetComponent<EnemyAI>();
				if ((bool)component4)
				{
					component4.DownedPlayer = null;
				}
				if ((bool)component3)
				{
					GameMaster.instance.PlayerDown[component3.playerId] = false;
				}
				else
				{
					GameMaster.instance.PlayerDown[component4.CompanionID] = false;
				}
			}
		}
	}

	private IEnumerator DelayExplosion()
	{
		GameMaster.instance.musicScript.Orchestra.SetSongsVolumes(0);
		SFXManager.instance.PlaySFX(DeathLoadingSound, 1f, null);
		EnemyAI component = GetComponent<EnemyAI>();
		component.ETSN.enabled = false;
		component.enabled = false;
		ParticleSystem[] deathLoadingParticles = DeathLoadingParticles;
		for (int i = 0; i < deathLoadingParticles.Length; i++)
		{
			deathLoadingParticles[i].Play();
		}
		yield return new WaitForSeconds(3f);
		EnemyTankDeath();
	}

	private IEnumerator canHurtAgain(float sec)
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
		BossVoiceLines component = GetComponent<BossVoiceLines>();
		if ((bool)component)
		{
			component.PlayDeathSound();
		}
		isDestroying = true;
		if (IsAirdropped && GameMaster.instance.AmountCalledInTanks > 0)
		{
			GameMaster.instance.AmountCalledInTanks--;
		}
		EnemyAI component2 = GetComponent<EnemyAI>();
		Explosion();
		if (DoAreaDamageWhenDie)
		{
			AreaDamageEnemies(base.transform.position, AreaDamageRange, 1f);
		}
		CameraShake component3 = Camera.main.GetComponent<CameraShake>();
		if (GetComponent<NukeTankAI>() != null)
		{
			if (GetComponent<NukeTankAI>().IsArmed)
			{
				if ((bool)component3)
				{
					component3.StartCoroutine(component3.Shake(0.5f, 1f));
				}
			}
			else if ((bool)component3)
			{
				component3.StartCoroutine(component3.Shake(0.1f, 0.06f));
			}
		}
		else if ((bool)component3)
		{
			component3.StartCoroutine(component3.Shake(0.1f, 0.06f));
		}
		if (GameMaster.instance != null)
		{
			if (GameMaster.instance.AmountEnemyTanks > 0 && !IsAirdropped)
			{
				GameMaster.instance.AmountEnemyTanks--;
			}
			if ((bool)component2 && component2.MyTeam == 1 && !MapEditorMaster.instance && !component2.IsCompanion)
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
			int num = ((EnemyID != -10) ? ((EnemyID == -11) ? 1 : ((EnemyID == -12) ? 2 : ((EnemyID == -13) ? 3 : ((EnemyID == -14) ? 4 : ((EnemyID == -15) ? 5 : ((EnemyID == -110) ? 9 : 0)))))) : 0);
			ZombieTankSpawner.instance.CurrentAmountOfEnemyTypes[num]--;
			AccountMaster.instance.SaveCloudData(0, EnemyID, 0, bounceKill: false, 0f);
		}
		else if (!GameMaster.instance.inMenuMode && !GameMaster.instance.inMapEditor)
		{
			if ((bool)component2 && component2.isShiny && (bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[34] != 1)
			{
				AchievementsTracker.instance.completeAchievementWithAI(34);
			}
			if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[15] != 1)
			{
				Debug.Log("ACHIEVEMENT 15!");
				AchievementsTracker.instance.completeAchievement(15);
			}
			if ((bool)component2)
			{
				if (!component2.isLevel10Boss && !component2.isLevel30Boss && !component2.isLevel50Boss && !component2.isLevel70Boss && !component2.isLevel100Boss && EnemyID != -1)
				{
					GameMaster.instance.TankColorKilled[EnemyID]++;
				}
				if (component2.isLevel10Boss || component2.isLevel30Boss)
				{
					if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[23] != 1)
					{
						AchievementsTracker.instance.completeAchievement(23);
					}
				}
				else if (component2.isLevel50Boss)
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
				else if (component2.isLevel70Boss && (bool)AchievementsTracker.instance && !AchievementsTracker.instance.HasBeenStunnedByBoss && OptionsMainMenu.instance.AM[11] != 1)
				{
					AchievementsTracker.instance.completeAchievement(11);
				}
				if ((component2.isLevel10Boss || component2.isLevel30Boss || component2.isLevel50Boss || component2.isLevel70Boss || component2.isLevel100Boss) && !IsHitByBullet && OptionsMainMenu.instance.AM[16] != 1)
				{
					AchievementsTracker.instance.completeAchievement(16);
				}
				if (component2.isLevel100Boss && GameMaster.instance.isOfficialCampaign)
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
			NewAIagent component4 = GetComponent<NewAIagent>();
			if (component4.source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
				component4.source.volume = 0.5f;
				component4.source.clip = null;
				component4.source.loop = false;
				component4.source.Stop();
			}
		}
		if (GetComponent<EnemyAI>() != null)
		{
			EnemyAI component5 = GetComponent<EnemyAI>();
			if (component5.source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
				component5.source.volume = 0.5f;
				component5.source.clip = null;
				component5.source.loop = false;
				component5.source.Stop();
			}
		}
		SkidMarkCreator.transform.parent = null;
		SkidMarkCreator.Stop();
		float t = ((OptionsMainMenu.instance.currentGraphicSettings == 0) ? 15f : ((OptionsMainMenu.instance.currentGraphicSettings == 1) ? 30f : ((OptionsMainMenu.instance.currentGraphicSettings == 2) ? 60f : ((OptionsMainMenu.instance.currentGraphicSettings == 3) ? 120f : ((OptionsMainMenu.instance.currentGraphicSettings == 4) ? 240f : 300f)))));
		Object.Destroy(SkidMarkCreator, t);
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDie);
		Collider[] array = Physics.OverlapSphere(base.transform.position, 4f);
		foreach (Collider collider in array)
		{
			Rigidbody component6 = collider.GetComponent<Rigidbody>();
			if (component6 != null && (collider.tag == "Player" || collider.tag == "Enemy"))
			{
				float num2 = Vector3.Distance(component6.transform.position, base.transform.position);
				float num3 = (7f - num2) * 2f;
				Vector3 vector = component6.transform.position - base.transform.position;
				component6.AddForce(vector * num3, ForceMode.Impulse);
			}
		}
		GameObject gameObject = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		EnemyAI component7 = GetComponent<EnemyAI>();
		if ((bool)component7 && (component7.isLevel10Boss || component7.isLevel30Boss || component7.isLevel50Boss || component7.isLevel70Boss || component7.isLevel100Boss))
		{
			SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.BossDie);
			gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		}
		if (!GameMaster.instance.CM)
		{
			gameObject.transform.parent = null;
		}
		else
		{
			gameObject.transform.parent = GameMaster.instance.CM.CanvasPaper.transform;
		}
		if ((bool)MapEditorMaster.instance)
		{
			GameMaster.instance.StartCoroutine(GameMaster.instance.GetTankTeamData(fast: false));
		}
		if (GameMaster.instance.CurrentMission == 99 && !MapEditorMaster.instance)
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
		if ((bool)component7)
		{
			if (component7.isLevel100Boss && GameMaster.instance.isOfficialCampaign)
			{
				GameMaster.instance.totalWins++;
				AccountMaster.instance.SaveCloudData(1, 1, 0, bounceKill: false, 0.5f);
				GameMaster.instance.SaveData(skipCloud: false);
				int amount = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 10 : ((OptionsMainMenu.instance.currentDifficulty == 1) ? 20 : ((OptionsMainMenu.instance.currentDifficulty == 2) ? 30 : 40)));
				AccountMaster.instance.IncreaseMarbles(amount);
				GameMaster.instance.GameHasStarted = false;
				KingTankScript component8 = base.transform.parent.gameObject.GetComponent<KingTankScript>();
				if ((bool)component8)
				{
					component8.StartCoroutine(component8.DestroyedBoss());
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
		GameMaster.instance.CheckGameState();
	}

	private void reviveMe()
	{
		canGetHurt = false;
		StartCoroutine(canHurtAgain(0.5f));
		reviveText.SetActive(value: false);
		dying = false;
		MoveTankScript component = GetComponent<MoveTankScript>();
		if ((bool)component)
		{
			GameMaster.instance.PlayerDown[component.playerId] = false;
		}
		else
		{
			EnemyAI component2 = GetComponent<EnemyAI>();
			if ((bool)component2)
			{
				GameMaster.instance.PlayerDown[component2.CompanionID] = false;
			}
		}
		GameMaster.instance.totalRevivesPerformed++;
		GameMaster.instance.AmountPlayersThatNeedRevive--;
		brokenParticles.gameObject.SetActive(value: false);
		health = 1;
		SFXManager.instance.PlaySFX(reviveSound);
		Object.Destroy(Object.Instantiate(revivePrefab, base.transform.position, Quaternion.identity), 2f);
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
		float t = ((OptionsMainMenu.instance.currentGraphicSettings == 0) ? 15f : ((OptionsMainMenu.instance.currentGraphicSettings == 1) ? 30f : ((OptionsMainMenu.instance.currentGraphicSettings == 2) ? 60f : ((OptionsMainMenu.instance.currentGraphicSettings == 3) ? 120f : ((OptionsMainMenu.instance.currentGraphicSettings == 4) ? 240f : 300f)))));
		Object.Destroy(SkidMarkCreator, t);
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.TankDie);
		CameraShake component = Camera.main.GetComponent<CameraShake>();
		if ((bool)component)
		{
			component.StartCoroutine(component.Shake(0.1f, 0.15f));
		}
		GameMaster.instance.AmountGoodTanks--;
		GameObject obj = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		DeathCrossScript component2 = obj.GetComponent<DeathCrossScript>();
		if (!GetComponent<MoveTankScript>().isPlayer2)
		{
			component2.IsBlue();
		}
		else
		{
			component2.IsRed();
		}
		obj.transform.parent = null;
		GameMaster.instance.CheckGameState();
		Object.Destroy(base.transform.parent.gameObject);
	}

	private void Explosion()
	{
		NukeTankAI component = GetComponent<NukeTankAI>();
		GameObject gameObject = null;
		gameObject = ((!component) ? Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity) : ((!component.IsArmed) ? Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity) : Object.Instantiate(component.NukeParticles, base.transform.position, Quaternion.identity)));
		if (gameObject.GetComponent<ParticleSystem>() != null)
		{
			gameObject.GetComponent<ParticleSystem>().Play();
		}
		ParticleSystemRenderer particleSystemRenderer = null;
		ParticleSystemRenderer particleSystemRenderer2 = null;
		foreach (Transform item in gameObject.transform)
		{
			if (item.name == "Schrapnel")
			{
				particleSystemRenderer = item.GetComponent<ParticleSystemRenderer>();
			}
			if (item.name == "Schrapnel2")
			{
				particleSystemRenderer2 = item.GetComponent<ParticleSystemRenderer>();
			}
		}
		if (OptionsMainMenu.instance.AMselected.Contains(58) && (bool)GlobalHealthTanks.instance)
		{
			Object.Instantiate(GlobalHealthTanks.instance.BloodSplatters[Random.Range(0, GlobalHealthTanks.instance.BloodSplatters.Length)], base.transform.position + new Vector3(0f, 0.06f, 0f), Quaternion.identity, null);
			if (Random.Range(0, 3) == 1)
			{
				SFXManager.instance.PlaySFX(GlobalHealthTanks.instance.InPain[Random.Range(0, GlobalHealthTanks.instance.InPain.Length)], 1f, null);
			}
		}
		EnemyAI component2 = GetComponent<EnemyAI>();
		if ((bool)component2)
		{
			if (component2.isAggro && (bool)component2.AngerVein && component2.AngerVein.activeSelf)
			{
				GameMaster.instance.musicScript.Orchestra.RagingCherries--;
			}
			if (EnemyID == 4 && !IsCustom && (bool)component2.ETSN)
			{
				component2.ETSN.PlaceMine(deathmine: true);
			}
		}
		Material[] array = new Material[1];
		bool flag = false;
		foreach (Transform item2 in base.transform)
		{
			if (item2.name == "Cube.003")
			{
				array = item2.GetComponent<MeshRenderer>().materials;
				flag = true;
			}
		}
		if (array.Length > 1 && flag && (bool)particleSystemRenderer)
		{
			if (array[2] != null)
			{
				particleSystemRenderer.material = array[2];
				if ((bool)particleSystemRenderer2)
				{
					particleSystemRenderer2.material = array[2];
				}
			}
			else if (array[1] != null)
			{
				particleSystemRenderer.material = array[1];
				if ((bool)particleSystemRenderer2)
				{
					particleSystemRenderer2.material = array[2];
				}
			}
		}
		else if (flag && particleSystemRenderer != null)
		{
			particleSystemRenderer.material.color = array[0].color;
			if (particleSystemRenderer2 != null)
			{
				particleSystemRenderer2.material.color = array[0].color;
			}
		}
		Object.Destroy(gameObject.gameObject, 5f);
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
		int num = ((!MTS) ? AIscript.MyTeam : MTS.MyTeam);
		Debug.Log("dying!" + health + instadie + num);
		if (num != 0 && !instadie)
		{
			bool flag = false;
			foreach (GameObject player in GameMaster.instance.Players)
			{
				if (player == base.gameObject)
				{
					continue;
				}
				MoveTankScript component = player.GetComponent<MoveTankScript>();
				if ((bool)component)
				{
					if (component.MyTeam == num)
					{
						instadie = false;
						flag = true;
					}
					continue;
				}
				EnemyAI component2 = player.GetComponent<EnemyAI>();
				if ((bool)component2 && component2.MyTeam == num)
				{
					instadie = false;
					flag = true;
				}
			}
			if (!flag)
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
		if (!instadie && amountRevivesLeft > 0 && base.transform.position.y > -30f)
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
				GameMaster.instance.CheckGameState();
				yield break;
			}
		}
		Explosion();
		if ((bool)SkidMarkCreator)
		{
			SkidMarkCreator.transform.parent = null;
			SkidMarkCreator.Stop();
			float t = ((OptionsMainMenu.instance.currentGraphicSettings == 0) ? 15f : ((OptionsMainMenu.instance.currentGraphicSettings == 1) ? 30f : ((OptionsMainMenu.instance.currentGraphicSettings == 2) ? 60f : ((OptionsMainMenu.instance.currentGraphicSettings == 3) ? 120f : ((OptionsMainMenu.instance.currentGraphicSettings == 4) ? 240f : 300f)))));
			Object.Destroy(SkidMarkCreator, t);
		}
		if (GameMaster.instance != null)
		{
			if (GameMaster.instance.Players.Count <= 1)
			{
				if (GameMaster.instance.Lives <= 1)
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
		GameObject gameObject = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		DeathCrossScript component3 = gameObject.GetComponent<DeathCrossScript>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, 4f);
		foreach (Collider collider in array)
		{
			Rigidbody component4 = collider.GetComponent<Rigidbody>();
			if (component4 != null && (collider.tag == "Player" || collider.tag == "Enemy"))
			{
				float num2 = Vector3.Distance(component4.transform.position, base.transform.position);
				float num3 = (7f - num2) * 2f;
				Vector3 vector = component4.transform.position - base.transform.position;
				component4.AddForce(vector * num3, ForceMode.Impulse);
			}
		}
		if (MTS != null)
		{
			if ((bool)TankeyTownMaster.instance)
			{
				Camera.main.gameObject.AddComponent<AudioListener>();
			}
			if (!instadie && dying)
			{
				GameMaster.instance.AmountPlayersThatNeedRevive--;
			}
			if (MTS.playerId == 0)
			{
				component3.IsBlue();
				GameMaster.instance.PlayerDied[0] = true;
				GameMaster.instance.PlayerDown[0] = false;
			}
			else if (MTS.playerId == 1)
			{
				component3.IsRed();
				GameMaster.instance.PlayerDied[1] = true;
				GameMaster.instance.PlayerDown[1] = false;
			}
			else if (MTS.playerId == 2)
			{
				component3.IsGreen();
				GameMaster.instance.PlayerDied[2] = true;
				GameMaster.instance.PlayerDown[2] = false;
			}
			else if (MTS.playerId == 3)
			{
				component3.IsPurple();
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
					component3.IsRed();
				}
				else if (AIscript.CompanionID == 2)
				{
					component3.IsGreen();
				}
				else if (AIscript.CompanionID == 3)
				{
					component3.IsPurple();
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
		gameObject.transform.parent = null;
		if (GameMaster.instance.CurrentMission >= 99 && !MapEditorMaster.instance && GameMaster.instance.Players.Count < 2)
		{
			if (MissionHundredController.instance != null && (bool)MissionHundredController.instance.KTS)
			{
				HealthTanks hT = MissionHundredController.instance.KTS.HT;
				if ((bool)hT && hT.health <= 11 && OptionsMainMenu.instance.AM[26] != 1)
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
		if ((bool)TankeyTownMaster.instance)
		{
			Object.Destroy(base.transform.parent.gameObject);
			yield return new WaitForSeconds(1f);
			SceneManager.LoadScene(0);
		}
		else
		{
			GameMaster.instance.CheckGameState();
			Object.Destroy(base.transform.parent.gameObject);
		}
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		NukeTankAI component = GetComponent<NukeTankAI>();
		if ((bool)component && component.IsArmed)
		{
			radius *= 2.5f;
		}
		Collider[] array;
		if ((bool)component)
		{
			if (component.IsArmed)
			{
				array = Physics.OverlapSphere(location, radius * 2f);
				foreach (Collider collider in array)
				{
					PlayerBulletScript component2 = collider.GetComponent<PlayerBulletScript>();
					EnemyBulletScript component3 = collider.GetComponent<EnemyBulletScript>();
					MineScript component4 = collider.GetComponent<MineScript>();
					DestroyableWall component5 = collider.GetComponent<DestroyableWall>();
					collider.GetComponent<WeeTurret>();
					ExplosiveBlock component6 = collider.GetComponent<ExplosiveBlock>();
					BombSackScript component7 = collider.GetComponent<BombSackScript>();
					HealthTanks component8 = collider.GetComponent<HealthTanks>();
					if ((bool)component6)
					{
						if (!component6.isExploding)
						{
							component6.StartCoroutine(component6.Death());
						}
					}
					else if (component2 != null)
					{
						if (!component2.isSilver)
						{
							component2.TimesBounced = 999;
						}
					}
					else if (component8 != null)
					{
						if (GameMaster.instance.isZombieMode && component8.health > 0 && GameMaster.instance.GameHasStarted && component8.Buzz != null)
						{
							SFXManager.instance.PlaySFX(component8.Buzz);
						}
						int num = 0;
						if (component8.ShieldFade != null)
						{
							num = component8.ShieldFade.ShieldHealth;
						}
						if (GameMaster.instance.GameHasStarted)
						{
							if (num > 0)
							{
								component8.ShieldFade.ShieldHealth = 0;
							}
							else
							{
								component8.DamageMe(999);
							}
						}
					}
					else if (component3 != null)
					{
						if (!component3.isElectric)
						{
							component3.BounceAmount = 999;
						}
					}
					else if (component4 != null)
					{
						component4.DetinationTime = 0f;
					}
					else if (component5 != null)
					{
						component5.StartCoroutine(component5.destroy());
					}
					else if (component7 != null)
					{
						component7.FlyBack();
					}
					else if (collider.name.Contains("Wood") && Vector3.Distance(base.transform.position, collider.transform.position) < 6f)
					{
						Object.Destroy(collider.gameObject);
					}
				}
				return;
			}
		}
		else
		{
			array = Physics.OverlapSphere(location, radius);
			foreach (Collider obj in array)
			{
				HealthTanks component9 = obj.GetComponent<HealthTanks>();
				PlayerBulletScript component10 = obj.GetComponent<PlayerBulletScript>();
				EnemyBulletScript component11 = obj.GetComponent<EnemyBulletScript>();
				MineScript component12 = obj.GetComponent<MineScript>();
				DestroyableWall component13 = obj.GetComponent<DestroyableWall>();
				WeeTurret component14 = obj.GetComponent<WeeTurret>();
				ExplosiveBlock component15 = obj.GetComponent<ExplosiveBlock>();
				BombSackScript component16 = obj.GetComponent<BombSackScript>();
				if ((bool)component15)
				{
					if (!component15.isExploding)
					{
						component15.StartCoroutine(component15.Death());
					}
				}
				else if (component14 != null)
				{
					if (GameMaster.instance.GameHasStarted)
					{
						component14.Health--;
					}
				}
				else if (component9 != null)
				{
					if (component9.immuneToExplosion)
					{
						continue;
					}
					if (GameMaster.instance.isZombieMode && component9.health > 0 && GameMaster.instance.GameHasStarted && component9.Buzz != null)
					{
						SFXManager.instance.PlaySFX(component9.Buzz);
					}
					int num2 = 0;
					if (component9.ShieldFade != null)
					{
						num2 = component9.ShieldFade.ShieldHealth;
					}
					if (GameMaster.instance.GameHasStarted)
					{
						if (num2 > 0)
						{
							component9.ShieldFade.ShieldHealth = 0;
						}
						else
						{
							component9.DamageMe(1);
						}
					}
				}
				else if (component10 != null)
				{
					if (!component10.isSilver)
					{
						component10.TimesBounced = 999;
					}
				}
				else if (component11 != null)
				{
					if (!component11.isElectric)
					{
						component11.BounceAmount = 999;
					}
				}
				else if (component12 != null)
				{
					component12.DetinationTime = 0f;
				}
				else if (component13 != null)
				{
					component13.StartCoroutine(component13.destroy());
				}
				else if (component16 != null)
				{
					component16.FlyBack();
				}
			}
		}
		array = Physics.OverlapSphere(location, radius * 2.75f);
		foreach (Collider collider2 in array)
		{
			Rigidbody component17 = collider2.GetComponent<Rigidbody>();
			if (component17 != null && (collider2.tag == "Player" || collider2.tag == "Enemy"))
			{
				float num3 = Vector3.Distance(component17.transform.position, base.transform.position);
				float num4 = (radius * 2.75f - num3) * 2.2f;
				Vector3 vector = component17.transform.position - base.transform.position;
				if (num4 > 16f)
				{
					num4 = 16f;
				}
				component17.AddForce(vector * num4, ForceMode.Impulse);
			}
		}
	}
}
