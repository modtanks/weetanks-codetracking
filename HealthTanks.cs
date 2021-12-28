using System.Collections;
using UnityEngine;

public class HealthTanks : MonoBehaviour
{
	public int health = 1;

	public int[] additionalDifficultyHealth;

	public int amountRevivesLeft = 1;

	public AudioClip Deathsound;

	public AudioClip DeathsoundLastAlive;

	public AudioClip GameOverSound;

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

	public bool isSpawnedIn;

	public bool canGetHurt = true;

	private bool ChargeDying;

	private bool isDestroying;

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
		}
		if ((bool)GameMaster.instance.CM && isMainTank)
		{
			maxHealth = 99999;
			health = maxHealth;
		}
	}

	private void Awake()
	{
		if ((bool)brokenParticles)
		{
			brokenParticles.gameObject.SetActive(value: false);
		}
		maxHealth = health;
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
		if (isSpawnedIn)
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
		if ((base.transform.tag == "Player" || base.transform.tag == "Enemy") && Armour != null && health > 0)
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
			if (base.transform.tag == "Enemy" || isGary)
			{
				num = -1f;
			}
			if ((float)health > 1f + num)
			{
				Armour.SetActive(value: true);
				if ((float)health == 2f + num)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: false);
					Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				}
				if ((float)health == 3f + num)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: true);
					Armour.transform.GetChild(2).gameObject.SetActive(value: false);
				}
				if ((float)health >= 4f + num)
				{
					Armour.transform.GetChild(0).gameObject.SetActive(value: true);
					Armour.transform.GetChild(1).gameObject.SetActive(value: true);
					Armour.transform.GetChild(2).gameObject.SetActive(value: true);
				}
				if ((float)health >= 5f + num && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: false);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				if ((float)health >= 6f + num && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
				{
					Armour.transform.GetChild(3).gameObject.SetActive(value: true);
					Armour.transform.GetChild(4).gameObject.SetActive(value: true);
					Armour.transform.GetChild(5).gameObject.SetActive(value: false);
				}
				if ((float)health >= 7f + num && GameMaster.instance.isZombieMode && base.transform.tag == "Player")
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
		if (!canGetHurt || GameMaster.instance.inTankeyTown)
		{
			if (GameMaster.instance.inTankeyTown)
			{
				health = 999;
			}
			return;
		}
		if (!GameMaster.instance.GameHasStarted && isSpawnedIn)
		{
			health = -999;
			EnemyTankDeath();
			return;
		}
		if (GameMaster.instance.CurrentMission == 99 && isSpawnedIn)
		{
			if (GameMaster.instance.Bosses.Length < 1)
			{
				health = -999;
				EnemyTankDeath();
				return;
			}
			if (GameMaster.instance.Bosses[0] == null)
			{
				health = -999;
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
		}
		if (!isMainTank && Input.GetKey(KeyCode.K) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.RightControl))
		{
			health = 0;
		}
		if (isMainTank && Input.GetKey(KeyCode.K) && GameMaster.instance.PlayerDied[0] && GameMaster.instance.PlayerModeWithAI[1] == 1 && (bool)GetComponent<EnemyAI>())
		{
			if ((bool)MapEditorMaster.instance)
			{
				if (GameMaster.instance.PlayerTeamColor[0] == GameMaster.instance.PlayerTeamColor[1] && GameMaster.instance.PlayerTeamColor[1] != 0)
				{
					health = -199;
				}
			}
			else
			{
				health = -199;
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
		if (isGary && GameMaster.instance.AmountEnemyTanks < 2 && health > 0 && GameMaster.instance.CurrentMission == 43)
		{
			GameMaster.instance.NAS.extraWait = 2f;
			GameMaster.instance.AmountEnemyTanks = 0;
		}
		if (GameMaster.instance.inMenuMode && GameObject.FindGameObjectsWithTag("Enemy").Length < 2)
		{
			health = -1;
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
		GameMaster.instance.Play2DClipAtPoint(DeathLoadingSound, 1f);
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
		isDestroying = true;
		if (isSpawnedIn && GameMaster.instance.AmountCalledInTanks > 0)
		{
			GameMaster.instance.AmountCalledInTanks--;
		}
		EnemyAI component = GetComponent<EnemyAI>();
		Explosion();
		CameraShake component2 = Camera.main.GetComponent<CameraShake>();
		if ((bool)component2)
		{
			component2.StartCoroutine(component2.Shake(0.1f, 0.06f));
		}
		if (GameMaster.instance != null)
		{
			if (GameMaster.instance.AmountEnemyTanks > 0 && !isSpawnedIn)
			{
				GameMaster.instance.AmountEnemyTanks--;
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
			AccountMaster.instance.SaveCloudData(0, EnemyID, 0, bounceKill: false);
		}
		else if (!GameMaster.instance.inMenuMode && !GameMaster.instance.inMapEditor)
		{
			if ((bool)component && component.isShiny && (bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[34] != 1)
			{
				AchievementsTracker.instance.completeAchievement(34);
			}
			if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[15] != 1)
			{
				Debug.Log("ACHIEVEMENT 15!");
				AchievementsTracker.instance.completeAchievement(15);
			}
			if ((bool)component)
			{
				if (!component.isLevel10Boss && !component.isLevel30Boss && !component.isLevel50Boss && !component.isLevel70Boss && !component.isLevel100Boss && EnemyID != -1)
				{
					GameMaster.instance.TankColorKilled[EnemyID]++;
				}
				if (component.isLevel10Boss || component.isLevel30Boss)
				{
					if ((bool)AchievementsTracker.instance && OptionsMainMenu.instance.AM[23] != 1)
					{
						AchievementsTracker.instance.completeAchievement(23);
					}
				}
				else if (component.isLevel50Boss)
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
				else if (component.isLevel70Boss && (bool)AchievementsTracker.instance && !AchievementsTracker.instance.HasBeenStunnedByBoss && OptionsMainMenu.instance.AM[11] != 1)
				{
					AchievementsTracker.instance.completeAchievement(11);
				}
				if ((component.isLevel10Boss || component.isLevel30Boss || component.isLevel50Boss || component.isLevel70Boss || component.isLevel100Boss) && !AchievementsTracker.instance.HasShootedThisRound && OptionsMainMenu.instance.AM[16] != 1)
				{
					AchievementsTracker.instance.completeAchievement(16);
				}
			}
			GameMaster.instance.totalKills++;
		}
		if (GetComponent<NewAIagent>() != null)
		{
			NewAIagent component3 = GetComponent<NewAIagent>();
			if (component3.source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
				component3.source.volume = 0.5f;
				component3.source.clip = null;
				component3.source.loop = false;
				component3.source.Stop();
			}
		}
		if (GetComponent<EnemyAI>() != null)
		{
			EnemyAI component4 = GetComponent<EnemyAI>();
			if (component4.source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
				component4.source.volume = 0.5f;
				component4.source.clip = null;
				component4.source.loop = false;
				component4.source.Stop();
			}
		}
		SkidMarkCreator.transform.parent = null;
		SkidMarkCreator.Stop();
		Play2DClipAtPoint(Deathsound);
		GameObject gameObject = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		EnemyAI component5 = GetComponent<EnemyAI>();
		if ((bool)component5 && (component5.isLevel10Boss || component5.isLevel30Boss || component5.isLevel50Boss || component5.isLevel70Boss || component5.isLevel100Boss))
		{
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
		if (GameMaster.instance.CurrentMission == 99)
		{
			GameMaster.instance.Enemies = GameObject.FindGameObjectsWithTag("Enemy");
			if (GameMaster.instance.Enemies.Length == 1)
			{
				GameMaster.instance.Enemies = null;
				GameMaster.instance.AmountEnemyTanks = 1;
			}
			else
			{
				GameMaster.instance.AmountEnemyTanks = GameMaster.instance.Enemies.Length;
			}
		}
		if ((bool)component5)
		{
			if (component5.isLevel100Boss)
			{
				GameMaster.instance.totalWins++;
				AccountMaster.instance.SaveCloudData(1, 1, 0, bounceKill: false);
				GameMaster.instance.SaveData(skipCloud: false);
				GameMaster.instance.GameHasStarted = false;
				KingTankScript component6 = base.transform.parent.gameObject.GetComponent<KingTankScript>();
				if ((bool)component6)
				{
					component6.StartCoroutine(component6.DestroyedBoss());
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
		if (GameMaster.instance.PlayerModeWithAI[1] != 1)
		{
			GameMaster.instance.totalRevivesPerformed++;
		}
		GameMaster.instance.AmountPlayersThatNeedRevive--;
		brokenParticles.gameObject.SetActive(value: false);
		health = 1;
		Play2DClipAtPoint(reviveSound);
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
		Play2DClipAtPoint(Deathsound);
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
		Object.Destroy(base.transform.parent.gameObject);
	}

	private void Explosion()
	{
		GameObject gameObject = Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity);
		gameObject.GetComponent<ParticleSystem>().Play();
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
		if (OptionsMainMenu.instance.BloodMode && (bool)GlobalHealthTanks.instance)
		{
			Object.Instantiate(GlobalHealthTanks.instance.BloodSplatters[Random.Range(0, GlobalHealthTanks.instance.BloodSplatters.Length)], base.transform.position + new Vector3(0f, 0.06f, 0f), Quaternion.identity, null);
			if (Random.Range(0, 3) == 1)
			{
				GameMaster.instance.Play2DClipAtPoint(GlobalHealthTanks.instance.InPain[Random.Range(0, GlobalHealthTanks.instance.InPain.Length)], 1f);
			}
		}
		EnemyAI component = GetComponent<EnemyAI>();
		if ((bool)component)
		{
			if (component.isAggro && (bool)component.AngerVein && component.AngerVein.activeSelf)
			{
				GameMaster.instance.musicScript.Orchestra.RagingCherries--;
			}
			if (EnemyID == 4 && !IsCustom && (bool)component.ETSN)
			{
				component.ETSN.PlaceMine(deathmine: true);
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
		if (array.Length != 0 && flag)
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
		if (!instadie && amountRevivesLeft > 0)
		{
			Play2DClipAtPoint(Buzz);
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
		}
		if (GameMaster.instance != null)
		{
			if (GameMaster.instance.AmountGoodTanks <= 1)
			{
				if (GameMaster.instance.Lives > 1)
				{
					Play2DClipAtPoint(DeathsoundLastAlive);
				}
				else
				{
					Play2DClipAtPoint(GameOverSound);
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
					Play2DClipAtPoint(Deathsound);
				}
				GameMaster.instance.AmountGoodTanks--;
			}
		}
		GameObject obj = Object.Instantiate(deathCross, base.transform.position + new Vector3(0f, 0.04f, 0f), Quaternion.identity);
		DeathCrossScript component3 = obj.GetComponent<DeathCrossScript>();
		if (MTS != null)
		{
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
			if (!MTS.isPlayer2)
			{
				GameMaster.instance.PKU.ResetAll(1);
			}
			else
			{
				GameMaster.instance.PKU.ResetAll(2);
			}
			MTS.Upgrades[0] = 0;
			MTS.Upgrades[1] = 0;
			MTS.Upgrades[2] = 0;
			MTS.Upgrades[3] = 0;
		}
		obj.transform.parent = null;
		if (GameMaster.instance.CurrentMission == 99 && GameMaster.instance.Players.Count < 2)
		{
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

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}