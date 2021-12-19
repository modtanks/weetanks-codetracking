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

	public GameObject[] SpawnPoints;

	public GameObject[] Tanks;

	public GameObject[] BossTanks;

	public int[] TanksSpawnRound;

	public MusicHandler musicScript;

	public AudioClip WinClip;

	public GameObject MainTankPrefab;

	public GameObject SecondTankPrefab;

	public GameObject MagicPoof;

	public AudioClip MagicSound;

	public Vector3 playerLocation;

	public Vector3 player2Location;

	public CountDownScript counterScript;

	public TutorialCanvas tutorialCanvas;

	private static ZombieTankSpawner _instance;

	public bool timerRunning;

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
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		StartCoroutine(Spawner());
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
		if (spawned < spawnAmount && GameMaster.instance.GameHasStarted && GameMaster.instance.AmountEnemyTanks < maxSpawnedAmount)
		{
			float num2 = 0.4f;
			bool flag = false;
			if (Wave >= 14 && Random.value < 0.04f)
			{
				SpawnTank(BossTanks[0]);
				spawned++;
				amountEnemies++;
			}
			for (int i = 0; i < Tanks.Length; i++)
			{
				if (TanksSpawnRound[i] <= Wave)
				{
					if (Random.value > num2 && spawned < spawnAmount)
					{
						flag = true;
						SpawnTank(Tanks[i]);
						spawned++;
						amountEnemies++;
					}
					else if (num2 > 0.2f)
					{
						num2 -= 0.1f;
					}
				}
			}
			if (!flag && spawned < spawnAmount)
			{
				SpawnTank(Tanks[0]);
				spawned++;
				amountEnemies++;
			}
		}
		if (!timerRunning && GameMaster.instance.GameHasStarted && GameMaster.instance.AmountEnemyTanks < 1 && spawned == spawnAmount)
		{
			if (Wave > GameMaster.instance.highestWaves[GameMaster.instance.CurrentMission])
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

	private void SpawnTank(GameObject tankPrefab)
	{
		int num = Random.Range(0, SpawnPoints.Length);
		Object.Instantiate(tankPrefab, SpawnPoints[num].transform.position, base.transform.rotation);
		GameMaster.instance.AmountEnemyTanks++;
	}

	private IEnumerator Timer()
	{
		timerRunning = true;
		yield return new WaitForSeconds(1f);
		if (GameObject.Find("Main_Tank_FBX_Survival") == null && GameObject.Find("Main_Tank_FBX_Survival(Clone)") == null)
		{
			Debug.LogWarning("BLUE TANK SPAWNED!");
			GameMaster.instance.AmountGoodTanks++;
			Object.Instantiate(MainTankPrefab, playerLocation, Quaternion.identity);
			GameObject obj = Object.Instantiate(MagicPoof, playerLocation, Quaternion.identity);
			ParticleSystem component = obj.GetComponent<ParticleSystem>();
			obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
			component.Play();
			Play2DClipAtPoint(MagicSound);
		}
		if (GameObject.Find("Second_Tank_FBX_Survival") == null && GameObject.Find("Second_Tank_FBX_Survival(Clone)") == null)
		{
			Debug.LogWarning("RED TANK SPAWNED!");
			GameMaster.instance.AmountGoodTanks++;
			Object.Instantiate(SecondTankPrefab, player2Location, Quaternion.identity);
			GameObject obj2 = Object.Instantiate(MagicPoof, player2Location, Quaternion.identity);
			ParticleSystem component2 = obj2.GetComponent<ParticleSystem>();
			obj2.transform.Rotate(new Vector3(-90f, 0f, 0f));
			component2.Play();
			Play2DClipAtPoint(MagicSound);
		}
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
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
