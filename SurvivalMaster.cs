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

	public bool PlayerAlive;

	public Animator canvasAnimator;

	public GameObject TheCanvas;

	public MusicHandler musicScript;

	public bool Player2Mode;

	public bool FriendlyFire;

	public GameObject OptionsMainMenuBackup;

	public GameObject Sun;

	public bool GameHasStarted;

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

	public bool isPlayingWithController;

	private static SurvivalMaster _instance;

	public string[] currentControllers;

	public float controllerCheckTimer = 2f;

	public float controllerCheckTimerOG = 2f;

	public int numberOfControllers;

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
		GameObject[] array = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		foreach (GameObject gameObject in array)
		{
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			gameObject.GetComponents<EnemyTargetingSystem>();
			MonoBehaviour[] array2 = components;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].enabled = true;
			}
			AudioSource[] components2 = gameObject.GetComponents<AudioSource>();
			for (int j = 0; j < components2.Length; j++)
			{
				components2[j].enabled = true;
			}
		}
	}

	private IEnumerator Spawner()
	{
		float num = 0.5f - (float)(spawnAmount / 100);
		if (num < 0.1f)
		{
			yield return new WaitForSeconds(0.1f);
		}
		else
		{
			yield return new WaitForSeconds(num);
		}
		if (spawned < spawnAmount && GameHasStarted)
		{
			int num2 = UnityEngine.Random.Range(0, SpawnPoints.Length);
			if (UnityEngine.Random.Range(0, 4) == 2)
			{
				UnityEngine.Object.Instantiate(Tank1, SpawnPoints[num2].transform.position, base.transform.rotation);
			}
			else
			{
				UnityEngine.Object.Instantiate(Tank2, SpawnPoints[num2].transform.position, base.transform.rotation);
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
		GameObject[] array = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		foreach (GameObject gameObject in array)
		{
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			for (int j = 0; j < components.Length; j++)
			{
				components[j].enabled = false;
			}
			AudioSource[] components2 = gameObject.GetComponents<AudioSource>();
			for (int j = 0; j < components2.Length; j++)
			{
				components2[j].enabled = false;
			}
		}
	}

	public void ControllerCheck()
	{
		Array.Clear(currentControllers, 0, currentControllers.Length);
		Array.Resize(ref currentControllers, Input.GetJoystickNames().Length);
		int num = 0;
		for (int i = 0; i < Input.GetJoystickNames().Length; i++)
		{
			currentControllers[i] = Input.GetJoystickNames()[i].ToLower();
			if (currentControllers[i] == "controller (xbox 360 for windows)" || currentControllers[i] == "controller (xbox 360 wireless receiver for windows)" || currentControllers[i] == "controller (xbox one for windows)")
			{
				num++;
			}
			else if (currentControllers[i] == "wireless controller")
			{
				num++;
			}
			else
			{
				_ = currentControllers[i] == "";
			}
		}
		numberOfControllers = num;
		_ = numberOfControllers;
		_ = 1;
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
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		UnityEngine.Object.Destroy(obj, clip.length);
	}
}
