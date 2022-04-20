using System.Collections;
using UnityEngine;

public class ZombieTankSpawner : MonoBehaviour
{
	public int Wave;

	public int spawnAmount;

	public int spawned;

	public int amountEnemies;

	public int amountGood;

	public int maxSpawnedAmount = 30;

	public float[] MultipliersAmountOfPlayers;

	public float multiplier = 1f;

	[Header("Audio")]
	public AudioClip Katsjing;

	public AudioClip denied;

	[Header("Prices")]
	public int[] BuildPrices;

	public int[] SpeedPrices;

	public int[] ShieldPrices;

	public int RocketPrice;

	public int TripminePrice;

	public int TurretPrice;

	public int TurretRepairPrice;

	public int[] TurretUpgradePrices;

	[Header("Upgraded Stats")]
	public int[] UpgradedSpeeds;

	public int[] UpgradedBuildSpeeds;

	[Header("Game Setup")]
	public PlaneScript BomberPlane;

	public AudioClip AirRaid;

	public Light[] Lights;

	public GameObject[] SpawnPoints;

	public GameObject[] Tanks;

	public GameObject[] BossTanks;

	public float[] TankSpawnChance;

	public int[] TanksSpawnRound;

	public int[] EnemyLimit;

	public int[] CurrentAmountOfEnemyTypes;

	public MusicHandler musicScript;

	public AudioClip WinClip;

	public GameObject MainTankPrefab;

	public GameObject SecondTankPrefab;

	public GameObject ThirdTankPrefab;

	public GameObject FourthTankPrefab;

	public GameObject MagicPoof;

	public AudioClip MagicSound;

	public Vector3 playerLocation;

	public Vector3 player2Location;

	public Vector3 player3Location;

	public Vector3 player4Location;

	public CountDownScript counterScript;

	public TutorialCanvas tutorialCanvas;

	private static ZombieTankSpawner _instance;

	public bool timerRunning = false;

	private int WeatherCooldown = 0;

	public static ZombieTankSpawner instance => _instance;

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

	public void StartSpawn()
	{
		playerLocation = GameObject.Find("Main_Tank_FBX_Survival").transform.position;
		player2Location = GameObject.Find("Second_Tank_FBX_Survival").transform.position;
		player3Location = GameObject.Find("Third_Tank_FBX_Survival").transform.position;
		player4Location = GameObject.Find("Fourth_Tank_FBX_Survival").transform.position;
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		StartCoroutine(Spawner());
	}

	private void Start()
	{
		StartCoroutine(InitiateAirRaid());
		int amount = 0;
		foreach (bool item in GameMaster.instance.PlayerJoined)
		{
			if (item)
			{
				amount++;
			}
		}
		multiplier = MultipliersAmountOfPlayers[amount];
		maxSpawnedAmount = Mathf.RoundToInt((float)maxSpawnedAmount * multiplier);
		for (int i = 0; i < GameMaster.instance.Levels.Count; i++)
		{
			if (i != GameMaster.instance.CurrentMission)
			{
				Object.Destroy(GameMaster.instance.Levels[i]);
			}
		}
	}

	private IEnumerator Spawner()
	{
		float waitTime = 0.5f - (float)(spawnAmount / 100);
		if (waitTime < 0.1f)
		{
			yield return new WaitForSeconds(0.1f);
		}
		else
		{
			yield return new WaitForSeconds(waitTime);
		}
		if (spawned < spawnAmount && GameMaster.instance.GameHasStarted && GameMaster.instance.AmountEnemyTanks < maxSpawnedAmount)
		{
			bool TankSpawned = false;
			if (Wave >= 14)
			{
				float dicerollBoss = Random.value;
				if (dicerollBoss < 0.04f && CurrentAmountOfEnemyTypes[9] < EnemyLimit[9])
				{
					SpawnTank(BossTanks[0], 9);
					spawned++;
					amountEnemies++;
					TankSpawned = true;
				}
			}
			while (!TankSpawned)
			{
				for (int i = 0; i < Tanks.Length; i++)
				{
					float modifier = ((i > 0) ? ((float)CurrentAmountOfEnemyTypes[i] / 32f) : 0f);
					float diceRoll = Random.value + modifier;
					if (diceRoll <= TankSpawnChance[i] && TanksSpawnRound[i] <= Wave && CurrentAmountOfEnemyTypes[i] < EnemyLimit[i])
					{
						SpawnTank(Tanks[i], i);
						spawned++;
						amountEnemies++;
						TankSpawned = true;
						break;
					}
				}
			}
		}
		if (!timerRunning && GameMaster.instance.GameHasStarted && GameMaster.instance.AmountEnemyTanks < 1 && spawned == spawnAmount)
		{
			if (GameMaster.instance.highestWaves != null && GameMaster.instance.highestWaves.Length > 8 && Wave > GameMaster.instance.highestWaves[GameMaster.instance.CurrentMission])
			{
				GameMaster.instance.highestWaves[GameMaster.instance.CurrentMission] = Wave;
			}
			GameMaster.instance.SaveData(skipCloud: false);
			GameMaster.instance.GameHasStarted = false;
			if (tutorialCanvas != null)
			{
				tutorialCanvas.CheckTut();
			}
			Play2DClipAtPoint(WinClip);
			StartCoroutine("Timer");
		}
		StartCoroutine(Spawner());
	}

	private IEnumerator InitiateAirRaid()
	{
		float RandomWaitingTime = Random.Range(30f, 80f);
		yield return new WaitForSeconds(RandomWaitingTime);
		if (Wave >= 38 && GameMaster.instance.GameHasStarted)
		{
			Vector3 Location = Vector3.zero;
			if (GameMaster.instance.Players[0] != null)
			{
				Location = GameMaster.instance.Players[0].transform.position;
			}
			else if (GameMaster.instance.Players[1] != null)
			{
				Location = GameMaster.instance.Players[1].transform.position;
			}
			else if (GameMaster.instance.Players[2] != null)
			{
				Location = GameMaster.instance.Players[2].transform.position;
			}
			else if (GameMaster.instance.Players[3] != null)
			{
				Location = GameMaster.instance.Players[3].transform.position;
			}
			BomberPlane.TargetLocation = new Vector3(Location.x, 10f, Location.z);
			BomberPlane.StartFlyToTB(-1, -1);
			SFXManager.instance.PlaySFX(AirRaid);
			Light[] lights = Lights;
			foreach (Light L in lights)
			{
				StartCoroutine(AirRaidLight(L));
			}
		}
		yield return new WaitForSeconds(8f);
		StartCoroutine(InitiateAirRaid());
	}

	private IEnumerator AirRaidLight(Light L)
	{
		float t2 = 0f;
		Color OG = L.color;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime;
			L.color = Color.Lerp(OG, Color.red, t2);
			yield return null;
		}
		yield return new WaitForSeconds(4f);
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime;
			L.color = Color.Lerp(Color.red, OG, t2);
			yield return null;
		}
	}

	private void SpawnTank(GameObject tankPrefab, int type)
	{
		CurrentAmountOfEnemyTypes[type]++;
		int chosenPoint = Random.Range(0, SpawnPoints.Length);
		Object.Instantiate(tankPrefab, SpawnPoints[chosenPoint].transform.position, base.transform.rotation);
		GameMaster.instance.AmountEnemyTanks++;
	}

	private IEnumerator Timer()
	{
		timerRunning = true;
		if ((bool)CloudGeneration.instance && WeatherCooldown < 1)
		{
			int[] WeatherChances = new int[28]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 1, 1, 1, 2, 2, 3, 3
			};
			int picked = WeatherChances[Random.Range(0, WeatherChances.Length)];
			if (picked > 0 && picked != CloudGeneration.instance.CurrentWeatherType)
			{
				WeatherCooldown += picked;
				CloudGeneration.instance.MakeItDark();
				CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(picked, force: true));
			}
			else
			{
				WeatherCooldown++;
				CloudGeneration.instance.MakeItDay();
				CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(0, force: true));
			}
		}
		else if (WeatherCooldown > 0)
		{
			WeatherCooldown--;
		}
		yield return new WaitForSeconds(1f);
		if (GameObject.Find("Main_Tank_FBX_Survival") == null && GameObject.Find("Main_Tank_FBX_Survival(Clone)") == null && GameMaster.instance.PlayerJoined[0])
		{
			Debug.LogWarning("BLUE TANK SPAWNED!");
			GameMaster.instance.AmountGoodTanks++;
			Object.Instantiate(MainTankPrefab, playerLocation, Quaternion.identity);
			GameObject magic = Object.Instantiate(MagicPoof, playerLocation, Quaternion.identity);
			ParticleSystem magicSystem2 = magic.GetComponent<ParticleSystem>();
			magic.transform.Rotate(new Vector3(-90f, 0f, 0f));
			magicSystem2.Play();
			Play2DClipAtPoint(MagicSound);
		}
		if ((GameMaster.instance.PlayerDied[1] && GameMaster.instance.PlayerJoined[1]) || GameMaster.instance.PlayerJoining[1])
		{
			GameMaster.instance.PlayerDied[1] = false;
			GameMaster.instance.PlayerJoining[1] = false;
			GameMaster.instance.PlayerJoined[1] = true;
			Debug.LogWarning("RED TANK SPAWNED!");
			GameMaster.instance.AmountGoodTanks++;
			Object.Instantiate(SecondTankPrefab, player2Location, Quaternion.identity);
			GameObject magic2 = Object.Instantiate(MagicPoof, player2Location, Quaternion.identity);
			ParticleSystem magicSystem3 = magic2.GetComponent<ParticleSystem>();
			magic2.transform.Rotate(new Vector3(-90f, 0f, 0f));
			magicSystem3.Play();
			Play2DClipAtPoint(MagicSound);
		}
		if ((GameMaster.instance.PlayerDied[2] && GameMaster.instance.PlayerJoined[2]) || GameMaster.instance.PlayerJoining[2])
		{
			Debug.LogWarning("THIRD TANK SPAWNED!");
			GameMaster.instance.PlayerDied[2] = false;
			GameMaster.instance.PlayerJoining[2] = false;
			GameMaster.instance.PlayerJoined[2] = true;
			GameMaster.instance.AmountGoodTanks++;
			Object.Instantiate(ThirdTankPrefab, player3Location, Quaternion.identity);
			GameObject magic3 = Object.Instantiate(MagicPoof, player3Location, Quaternion.identity);
			ParticleSystem magicSystem4 = magic3.GetComponent<ParticleSystem>();
			magic3.transform.Rotate(new Vector3(-90f, 0f, 0f));
			magicSystem4.Play();
			Play2DClipAtPoint(MagicSound);
		}
		if ((GameMaster.instance.PlayerDied[3] && GameMaster.instance.PlayerJoined[3]) || GameMaster.instance.PlayerJoining[3])
		{
			Debug.LogWarning("FOURTH TANK SPAWNED!");
			GameMaster.instance.PlayerDied[3] = false;
			GameMaster.instance.PlayerJoining[3] = false;
			GameMaster.instance.PlayerJoined[3] = true;
			GameMaster.instance.AmountGoodTanks++;
			Object.Instantiate(FourthTankPrefab, player4Location, Quaternion.identity);
			GameObject magic4 = Object.Instantiate(MagicPoof, player4Location, Quaternion.identity);
			ParticleSystem magicSystem = magic4.GetComponent<ParticleSystem>();
			magic4.transform.Rotate(new Vector3(-90f, 0f, 0f));
			magicSystem.Play();
			Play2DClipAtPoint(MagicSound);
		}
		GameMaster.instance.FindPlayers();
		yield return new WaitForSeconds(3f);
		if (!counterScript.start)
		{
			counterScript.count = -5f;
			counterScript.start = true;
		}
		timerRunning = false;
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
