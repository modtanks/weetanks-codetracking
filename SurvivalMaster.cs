using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SurvivalMaster : MonoBehaviour
{
	public int Wave;

	public int spawnAmount;

	public int spawned;

	public int amountEnemies;

	public int amountGood;

	public GameObject[] Players;

	public Vector3 playerLocation;

	public Vector3 player2Location;

	public GameObject[] EnemyScripts;

	public GameObject[] Enemies;

	public GameObject[] Bosses;

	public GameObject[] Levels;

	public bool PlayerAlive = false;

	public Animator canvasAnimator;

	public GameObject TheCanvas;

	public MusicHandler musicScript;

	public bool Player2Mode = false;

	public bool FriendlyFire = false;

	public GameObject OptionsMainMenuBackup;

	public GameObject Sun;

	public bool GameHasStarted = false;

	public GameObject[] SpawnPoints;

	public GameObject Tank1;

	public GameObject Tank2;

	public float MoneyP1;

	public float MoneyP2;

	public GameObject MainTankPrefab;

	public GameObject SecondTankPrefab;

	public GameObject MagicPoof;

	public AudioClip MagicSound;

	public AudioClip WinClip;

	public CountDownScript counterScript;

	public int Player1Kills;

	public int Player2Kills;

	public bool isPlayingWithController = false;

	private static SurvivalMaster _instance;

	public string[] currentControllers;

	public float controllerCheckTimer = 2f;

	public float controllerCheckTimerOG = 2f;

	public int numberOfControllers = 0;

	public static SurvivalMaster instance => _instance;

	private void Awake()
	{
		DisableGame();
		if (OptionsMainMenu.instance == null)
		{
			OptionsMainMenuBackup.SetActive(value: true);
		}
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		playerLocation = GameObject.Find("Main_Tank_FBX_Survival").transform.position;
		Player2Mode = OptionsMainMenu.instance.StartPlayer2Mode;
		if (!Player2Mode)
		{
			UnityEngine.Object.Destroy(GameObject.Find("Second_Tank_FBX_Survival"));
		}
		if (Player2Mode)
		{
			player2Location = GameObject.Find("Second_Tank_FBX_Survival").transform.position;
		}
		FriendlyFire = OptionsMainMenu.instance.FriendlyFire;
		musicScript.StartSurvivalMusic();
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		StartCoroutine(Spawner());
	}

	public void StartGame()
	{
		GameHasStarted = true;
		Players = GameObject.FindGameObjectsWithTag("Player");
		Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] all = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		GameObject[] array = all;
		foreach (GameObject enemy in array)
		{
			MonoBehaviour[] enemyscripts = enemy.GetComponents<MonoBehaviour>();
			EnemyTargetingSystem[] targetscripts = enemy.GetComponents<EnemyTargetingSystem>();
			MonoBehaviour[] array2 = enemyscripts;
			foreach (MonoBehaviour script in array2)
			{
				script.enabled = true;
			}
			AudioSource[] sources2 = enemy.GetComponents<AudioSource>();
			AudioSource[] array3 = sources2;
			foreach (AudioSource source in array3)
			{
				source.enabled = true;
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
		if (spawned < spawnAmount && GameHasStarted)
		{
			int chosenPoint = UnityEngine.Random.Range(0, SpawnPoints.Length);
			int whichTank = UnityEngine.Random.Range(0, 4);
			if (whichTank == 2)
			{
				UnityEngine.Object.Instantiate(Tank1, SpawnPoints[chosenPoint].transform.position, base.transform.rotation);
			}
			else
			{
				UnityEngine.Object.Instantiate(Tank2, SpawnPoints[chosenPoint].transform.position, base.transform.rotation);
			}
			spawned++;
			amountEnemies++;
		}
		else if (GameHasStarted && amountEnemies < 1 && spawned == spawnAmount)
		{
			musicScript.MusicSource.Stop();
			musicScript.MusicSource.loop = false;
			musicScript.MusicSource.clip = null;
			musicScript.CanStartMusic = false;
			musicScript.canStartMainPart = false;
			Play2DClipAtPoint(WinClip);
			GameHasStarted = false;
			StartCoroutine("Timer");
		}
		StartCoroutine(Spawner());
	}

	private IEnumerator Timer()
	{
		yield return new WaitForSeconds(1f);
		if (GameObject.Find("Main_Tank_FBX_Survival") == null && GameObject.Find("Main_Tank_FBX_Survival(Clone)") == null && Player2Mode)
		{
			UnityEngine.Object.Instantiate(MainTankPrefab, playerLocation, Quaternion.identity);
			UnityEngine.Object.Instantiate(MagicPoof, playerLocation, Quaternion.identity);
			Play2DClipAtPoint(MagicSound);
		}
		if (GameObject.Find("Second_Tank_FBX_Survival") == null && GameObject.Find("Second_Tank_FBX_Survival(Clone)") == null && Player2Mode)
		{
			UnityEngine.Object.Instantiate(SecondTankPrefab, player2Location, Quaternion.identity);
			UnityEngine.Object.Instantiate(MagicPoof, player2Location, Quaternion.identity);
			Play2DClipAtPoint(MagicSound);
		}
		if (!counterScript.start)
		{
			counterScript.count = -5f;
			counterScript.start = true;
		}
	}

	public void NewRound()
	{
		Wave++;
		amountEnemies = 0;
		spawned = 0;
		spawnAmount = Wave * 2;
		GameHasStarted = true;
	}

	public void DisableGame()
	{
		Players = GameObject.FindGameObjectsWithTag("Player");
		Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] all = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		GameObject[] array = all;
		foreach (GameObject enemy in array)
		{
			MonoBehaviour[] enemyscripts = enemy.GetComponents<MonoBehaviour>();
			MonoBehaviour[] array2 = enemyscripts;
			foreach (MonoBehaviour script in array2)
			{
				script.enabled = false;
			}
			AudioSource[] sources2 = enemy.GetComponents<AudioSource>();
			AudioSource[] array3 = sources2;
			foreach (AudioSource source in array3)
			{
				source.enabled = false;
			}
		}
	}

	public void ControllerCheck()
	{
		Array.Clear(currentControllers, 0, currentControllers.Length);
		Array.Resize(ref currentControllers, Input.GetJoystickNames().Length);
		int amount = 0;
		for (int i = 0; i < Input.GetJoystickNames().Length; i++)
		{
			currentControllers[i] = Input.GetJoystickNames()[i].ToLower();
			if (currentControllers[i] == "controller (xbox 360 for windows)" || currentControllers[i] == "controller (xbox 360 wireless receiver for windows)" || currentControllers[i] == "controller (xbox one for windows)")
			{
				amount++;
			}
			else if (currentControllers[i] == "wireless controller")
			{
				amount++;
			}
			else if (!(currentControllers[i] == ""))
			{
			}
		}
		numberOfControllers = amount;
		if (numberOfControllers >= 1)
		{
		}
	}

	public void defeated()
	{
		musicScript.Defeat();
		canvasAnimator.SetBool("Defeat", value: true);
		DisableGame();
	}

	private void Update()
	{
		amountGood = GameObject.FindGameObjectsWithTag("Player").Length;
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		UnityEngine.Object.Destroy(tempAudioSource, clip.length);
	}
}
