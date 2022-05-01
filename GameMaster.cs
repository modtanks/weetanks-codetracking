using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Rewired;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
	[Serializable]
	public class PressurePlateSpawnpoints
	{
		public Vector3 position;

		public int MissionNumber;

		public GameObject SecretMission;

		public string MissionName;

		public bool IsNightMission;

		public int WeatherType;

		public bool HasFloor;

		public bool HasWalls;
	}

	public bool DevModeActive;

	public bool inDemoMode;

	public bool isZombieMode;

	public bool inMenuMode;

	public bool inMapEditor;

	public bool inOnlineMode;

	public bool isOfficialCampaign;

	public bool inTankeyTown;

	[Header("Stats")]
	public int Lives = 3;

	public int CurrentMission;

	public int AmountEnemyTanks;

	public int AmountGoodTanks;

	public int AmountMinesPlaced;

	public int[] AmountTeamTanks;

	public ProgressDataOnline CurrentData;

	public int AmountCalledInTanks;

	public List<int> Playerkills = new List<int>();

	public int TotalKillsThisSession;

	public List<int> TankColorKilled = new List<int>();

	public int totalKills;

	public int totalKillsBounce;

	public int totalRevivesPerformed;

	public int totalDefeats;

	public int totalWins;

	public int maxMissionReached;

	public int maxMissionReachedKid;

	public int maxMissionReachedHard;

	public int maxMissionReachedGrandpa;

	[HideInInspector]
	public bool HasGotten100Checkpoint;

	[Header("UI")]
	public TextMeshProUGUI Mission_Text;

	public TextMeshProUGUI MissionName_Text;

	public TextMeshProUGUI Tanks_Text;

	public TextMeshProUGUI TanksLeft_Text;

	public GameObject[] ObjectsEnablingAtStart;

	[Header("Spawning Map Loading")]
	public List<GameObject> Players = new List<GameObject>();

	public GameObject PlayerPrefab;

	public GameObject Player2Prefab;

	public GameObject Player3Prefab;

	public GameObject Player4Prefab;

	public GameObject AIPlayer2Prefab;

	public GameObject AIPlayer3Prefab;

	public GameObject AIPlayer4Prefab;

	public List<Vector3> playerLocation = new List<Vector3>();

	public GameObject[] EnemyScripts;

	public List<GameObject> Enemies;

	public GameObject[] Bosses;

	public List<string> MissionNames = new List<string>();

	public List<string> MissionTutorials = new List<string>();

	public UnityEngine.Object[] SMED;

	public List<GameObject> Levels = new List<GameObject>();

	public List<TextAsset> LevelsText = new List<TextAsset>();

	public List<int> NightLevels = new List<int>();

	public List<int> TrainLevels = new List<int>();

	public List<int> FoundSecretMissions = new List<int>();

	public GameObject PressurePlate;

	public List<PressurePlateSpawnpoints> CampaignPressurePlates = new List<PressurePlateSpawnpoints>();

	public int SecretMissionCounter;

	public int lastMissionNumber = 55;

	[Header("Others")]
	public Animator canvasAnimator;

	public NewAnimationsScript NAS;

	public GameObject TheCanvas;

	public GameObject floor;

	public GameObject MapCornerObject;

	public MusicHandler musicScript;

	public AudioClip ExtraLiveSound;

	public AudioClip LostLiveSound;

	public AudioClip TankTracksCarpetSound;

	public AudioClip TankTracksGrassSound;

	public AudioClip TankTracksNormalSound;

	public bool Victory;

	public bool PlayerAlive;

	public bool levelIsLoaded;

	public int[] PlayerModeWithAI = new int[4];

	public bool AssassinTankAlive;

	public List<bool> PlayerJoining = new List<bool>();

	public List<bool> PlayerJoined = new List<bool>();

	public bool FriendlyFire;

	public bool PlayerLeftCooldown;

	public bool OnlyCompanionLeft;

	public GameObject mapBorders;

	public GenerateNavMeshSurface GNMS;

	public FinalEndCanvasScript FinalScript;

	public PlayerKillsUI PKU;

	public int EnemyTankTracksAudio;

	public int maxEnemyTankTracks = 4;

	public GameObject currentLoadedLevel;

	private Vector3 lvlPos;

	public Camera MainCamera;

	public GameObject CursorText;

	public List<bool> PlayerDown = new List<bool>();

	public List<bool> PlayerDied = new List<bool>();

	public GameObject SpawningPlayerParticles;

	public float counter;

	[HideInInspector]
	public int AmountPlayersThatNeedRevive;

	public bool isResettingTestLevel;

	private static GameMaster _instance;

	[Header("Game status")]
	public bool restartGame;

	public bool GameHasStarted;

	public bool GameHasPaused;

	public GameObject OptionsMainMenuBackup;

	public GameObject[] Sun;

	public GameObject Moon;

	public GameObject[] TrainLevelBlocks;

	public GameObject[] TrainLevel87Blocks;

	public GameObject PauseMenu;

	public List<Vector3> BreakableBlocksLocations = new List<Vector3>();

	public List<Vector3> StoneBlocksLocations = new List<Vector3>();

	public List<Vector3> TNTLocations = new List<Vector3>();

	public List<Vector3> BreakableHalfBlocksLocations = new List<Vector3>();

	public GameObject CorkBlock;

	public GameObject TNTBlock;

	public GameObject HalfCorkBlock;

	public GameObject StoneBlock;

	public bool isReplay;

	public TutorialCanvas tutorialCanvas;

	public Color ambientCLR;

	public Color ambientCLRlvl50;

	private bool activatedBackup;

	public bool isPlayingWithController;

	[Header("Tutorial Things")]
	public bool mission1HasShooted;

	public bool mission2HasBoosted;

	public bool mission3HasMined;

	[Header("Zombie stats")]
	public int[] highestWaves = new int[10];

	public int survivalTanksKilled;

	public int[] TurretsPlaced;

	public CreditsMaster CM;

	public List<int> PlayerTeamColor = new List<int>();

	public int AccountID = -1;

	public int numberOfControllers;

	public static GameMaster instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
		if (!inMenuMode && !inMapEditor)
		{
			if (TheCanvas != null)
			{
				TheCanvas.SetActive(value: true);
			}
			if (OptionsMainMenu.instance == null)
			{
				OptionsMainMenuBackup.SetActive(value: true);
				activatedBackup = true;
			}
		}
		CurrentData = new ProgressDataOnline();
	}

	private void Start()
	{
		if ((bool)CursorText)
		{
			CursorText.GetComponent<TextMeshProUGUI>().text = "";
		}
		AudioListener.volume = 1f;
		LoadData();
		if (ObjectsEnablingAtStart != null)
		{
			GameObject[] objectsEnablingAtStart = ObjectsEnablingAtStart;
			for (int i = 0; i < objectsEnablingAtStart.Length; i++)
			{
				objectsEnablingAtStart[i].SetActive(value: true);
			}
		}
		MainCamera = Camera.main;
		ambientCLR = RenderSettings.ambientLight;
		InvokeRepeating("ControllerCheck", 0.5f, 0.5f);
		InvokeRepeating("PlayerListCheck", 0.5f, 0.5f);
		Debug.LogError("now here in start from GameMaster!");
		if (GNMS != null)
		{
			InvokeRepeating("GenerateNavMesh", 2f, 2f);
		}
		if (!inMenuMode && !inMapEditor)
		{
			if (MapEditorMaster.instance == null)
			{
				PlayerJoined[0] = true;
			}
			if (OptionsMainMenu.instance.AIcompanion[1] && !isZombieMode)
			{
				PlayerModeWithAI[1] = 1;
				if (MapEditorMaster.instance == null)
				{
					PlayerJoined[1] = true;
				}
				if (OptionsMainMenu.instance.AIcompanion[2])
				{
					PlayerModeWithAI[2] = 1;
					if (MapEditorMaster.instance == null)
					{
						PlayerJoined[2] = true;
					}
				}
				if (OptionsMainMenu.instance.AIcompanion[3])
				{
					PlayerModeWithAI[3] = 1;
					if (MapEditorMaster.instance == null)
					{
						PlayerJoined[3] = true;
					}
				}
			}
			CurrentMission = OptionsMainMenu.instance.StartLevel;
			if (!isZombieMode && !inMapEditor)
			{
				Mission_Text.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (CurrentMission + 1);
				if (isOfficialCampaign)
				{
					MissionName_Text.text = "'" + LocalizationMaster.instance.GetText("Mission_" + (CurrentMission + 1)) + "'";
				}
				else
				{
					MissionName_Text.text = "'" + MissionNames[CurrentMission] + "'";
				}
				Tanks_Text.text = "x " + AmountEnemyTanks;
				if (TanksLeft_Text != null)
				{
					TanksLeft_Text.text = "Tanks left: " + Lives;
				}
			}
			if (!isZombieMode)
			{
				Lives += OptionsMainMenu.instance.ExtraLives;
			}
			FriendlyFire = OptionsMainMenu.instance.FriendlyFire;
			if ((isZombieMode || inMapEditor || !(MapEditorMaster.instance == null)) && isZombieMode)
			{
				Debug.Log("added a good tank");
				AmountGoodTanks++;
			}
		}
		else if (!inMapEditor)
		{
			lvlPos = currentLoadedLevel.transform.position;
			UnityEngine.Object.Destroy(currentLoadedLevel);
		}
		for (int j = 0; j < 4; j++)
		{
			PlayerJoined[j] = OptionsMainMenu.instance.PlayerJoined[j];
		}
		Debug.LogError("End of start!");
	}

	private void GenerateNavMesh()
	{
	}

	private void PlayerListCheck()
	{
		if (Players.Count > 0)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject player in Players)
			{
				if (player == null)
				{
					list.Add(player);
				}
			}
			foreach (GameObject item in list)
			{
				Players.Remove(item);
			}
		}
		GameObject.FindGameObjectsWithTag("Enemy");
	}

	public void SetPlayerPosition(string p1tankName, string p2tankName, string p3tankName, string p4tankName)
	{
		if (currentLoadedLevel != null)
		{
			Transform transform = currentLoadedLevel.transform.Find(p1tankName);
			if (transform != null)
			{
				playerLocation[0] = transform.position;
			}
			Transform transform2 = currentLoadedLevel.transform.Find(p2tankName);
			if (transform2 != null)
			{
				playerLocation[1] = transform2.position;
			}
			else
			{
				playerLocation[1] = new Vector3(0f, 0f, 0f);
			}
			Transform transform3 = currentLoadedLevel.transform.Find(p3tankName);
			if (transform3 != null)
			{
				playerLocation[2] = transform3.position;
			}
			else
			{
				playerLocation[2] = new Vector3(0f, 0f, 0f);
			}
			Transform transform4 = currentLoadedLevel.transform.Find(p4tankName);
			if (transform4 != null)
			{
				playerLocation[3] = transform4.position;
			}
			else
			{
				playerLocation[3] = new Vector3(0f, 0f, 0f);
			}
		}
	}

	public IEnumerator PlayerleftCooldownTimer()
	{
		PlayerLeftCooldown = true;
		yield return new WaitForSeconds(1f);
		PlayerLeftCooldown = false;
	}

	public void ResetMapEditorMap(bool skip)
	{
		isResettingTestLevel = true;
		MapEditorMaster.instance.TeleportationFields.SetActive(value: false);
		StartCoroutine(ResetTestLevel(skip));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.U))
		{
			Input.GetKey(KeyCode.LeftShift);
		}
		if (inOnlineMode || (bool)CM)
		{
			return;
		}
		if (!inMenuMode)
		{
			if (numberOfControllers > 0)
			{
				int num = ((!isPlayingWithController) ? 1 : 0);
				if (PlayerModeWithAI[1] == 1)
				{
					num++;
				}
				if (PlayerModeWithAI[2] == 1)
				{
					num++;
				}
				if (PlayerModeWithAI[3] == 1)
				{
					num++;
				}
				int num2 = numberOfControllers + num;
				if (num2 > 4)
				{
					num2 = 4;
				}
				for (int i = 0; i < num2; i++)
				{
					if (PlayerJoined[i] || PlayerJoining[i] || PlayerLeftCooldown || !ReInput.players.GetPlayer(i).GetButtonDown("Escape"))
					{
						continue;
					}
					Debug.Log("CHANGING STATE!");
					if ((i != 1 || PlayerModeWithAI[1] != 1) && (i != 2 || PlayerModeWithAI[2] != 1) && (i != 3 || PlayerModeWithAI[3] != 1))
					{
						if (GameHasStarted && !GameHasPaused)
						{
							PlayerJoining[i] = true;
						}
						else if (!GameHasPaused && !GameHasStarted && PlayerModeWithAI[1] != 1)
						{
							PlayerJoined[i] = true;
							Debug.Log("added a good tank , player joined");
							AmountGoodTanks++;
						}
					}
				}
			}
			if (Players.Count < 1 && !MapEditorMaster.instance && PlayerAlive && (GameHasStarted || isZombieMode))
			{
				Debug.LogWarning("PlayerDead!!");
				if (inMapEditor && !isResettingTestLevel)
				{
					ResetMapEditorMap(skip: false);
					return;
				}
				if ((AmountEnemyTanks < 1 || CurrentMission == 99) && !Victory && levelIsLoaded && !isZombieMode && !inMapEditor)
				{
					if (Bosses.Length != 0 && CurrentMission == 99 && Bosses[0] != null)
					{
						if (Lives > 1)
						{
							CameraFollowPlayer component = Camera.main.transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
							PlaneMaster component2 = GetComponent<PlaneMaster>();
							if ((bool)component2)
							{
								component2.SpawnInOrder.Clear();
								component2.PS.isFlying = false;
								component2.PS.transform.position = Vector3.zero;
							}
							GameObject gameObject = GameObject.Find("LEVEL_100(Clone)");
							if ((bool)gameObject)
							{
								MissionHundredController component3 = gameObject.GetComponent<MissionHundredController>();
								if ((bool)component3)
								{
									component3.PlayerDied();
								}
							}
							component.enabled = true;
							totalDefeats++;
							AccountMaster.instance.SaveCloudData(2, 1, 0, bounceKill: false);
							SaveData(skipCloud: false);
							PlayerAlive = false;
							musicScript.Defeat();
							GameHasStarted = false;
							ResetLevel();
							StartCoroutine(startTheGame());
							Lives--;
						}
						else
						{
							PlayerAlive = false;
							defeated();
						}
					}
					else
					{
						CheckSecretMission();
						totalWins++;
						AccountMaster.instance.SaveCloudData(1, 1, 0, bounceKill: false);
						SaveData(skipCloud: false);
						EndTheGame();
					}
					return;
				}
				PlayerAlive = false;
				defeated();
			}
			if (inTankeyTown)
			{
				FindPlayers();
				return;
			}
			if (!isZombieMode && !inMapEditor && !inTankeyTown)
			{
				if (GameHasStarted)
				{
					counter += Time.deltaTime;
				}
				Tanks_Text.text = "x " + AmountEnemyTanks;
				if (TanksLeft_Text != null)
				{
					TanksLeft_Text.text = "Tanks left: " + Lives;
				}
			}
			else if (isZombieMode)
			{
				Mission_Text.text = "Wave " + ZombieTankSpawner.instance.Wave;
				MissionName_Text.text = "";
				Tanks_Text.text = "x " + AmountEnemyTanks;
				NewOrchestra component4 = GameObject.FindGameObjectWithTag("Orchestra").GetComponent<NewOrchestra>();
				if (component4 != null)
				{
					if (component4.isPlaying && !GameHasStarted)
					{
						component4.StopPlaying();
					}
					else if (!component4.isPlaying && GameHasStarted)
					{
						component4.StartPlaying();
					}
				}
			}
			if (AmountEnemyTanks < 1 && !Victory && levelIsLoaded && !inTankeyTown && !isZombieMode && !inMapEditor && !MapEditorMaster.instance)
			{
				if (CurrentMission < 99)
				{
					CheckSecretMission();
					totalWins++;
					AccountMaster.instance.SaveCloudData(1, 1, 0, bounceKill: false);
					SaveData(skipCloud: false);
					EndTheGame();
				}
				return;
			}
			if (!Victory && levelIsLoaded && GameHasStarted && (bool)MapEditorMaster.instance && !inMapEditor && !inTankeyTown)
			{
				int num3 = 0;
				int[] array = new int[5];
				for (int j = 0; j < AmountTeamTanks.Length; j++)
				{
					if (AmountTeamTanks[j] > 0)
					{
						num3++;
						array[j] = 1;
					}
					if (AmountTeamTanks[0] > 1)
					{
						num3 = 900;
						array[j] = 1;
					}
				}
				int num4 = 0;
				for (int k = 0; k < PlayerJoined.Count; k++)
				{
					if (PlayerJoined[k])
					{
						num4++;
						if (PlayerDied[k] && (AmountTeamTanks[PlayerTeamColor[k]] < 1 || PlayerTeamColor[k] == 0))
						{
							num4--;
						}
					}
				}
				if (num4 < 1)
				{
					PlayerAlive = false;
					defeated();
					return;
				}
				if (num3 < 2)
				{
					EndTheGame();
					return;
				}
			}
			if (!isZombieMode && inMapEditor && GameHasStarted && !isResettingTestLevel)
			{
				int num5 = 0;
				for (int l = 0; l < AmountTeamTanks.Length; l++)
				{
					if (AmountTeamTanks[l] > 0)
					{
						num5++;
					}
					if (AmountTeamTanks[0] > 1)
					{
						num5 = 900;
					}
				}
				if (num5 < 2)
				{
					isResettingTestLevel = true;
					MapEditorMaster.instance.TeleportationFields.SetActive(value: false);
					ResetMapEditorMap(skip: false);
					return;
				}
			}
			if (AmountEnemyTanks < 1 && !levelIsLoaded && !isZombieMode && !inMapEditor && !NAS.playingAnimation)
			{
				NAS.NextRound();
			}
			if (ZombieTankSpawner.instance != null && AmountEnemyTanks < 1 && ZombieTankSpawner.instance.Wave < 1 && isZombieMode && !NAS.playingAnimation)
			{
				Debug.LogError("Loading first lvl");
				NAS.NextRound();
			}
		}
		else
		{
			if (!inMenuMode || GameObject.FindGameObjectsWithTag("Enemy").Length >= 1)
			{
				return;
			}
			UnityEngine.Object.Destroy(currentLoadedLevel);
			GameObject[] first = GameObject.FindGameObjectsWithTag("Temp");
			GameObject[] second = GameObject.FindGameObjectsWithTag("Bullet");
			GameObject[] array2 = Enumerable.Concat(second: GameObject.FindGameObjectsWithTag("Mine"), first: first.Concat(second)).ToArray();
			for (int m = 0; m < array2.Length; m++)
			{
				UnityEngine.Object.Destroy(array2[m]);
			}
			int num6 = 0;
			int num7 = maxMissionReached;
			if (num7 < maxMissionReachedHard)
			{
				num7 = maxMissionReachedHard;
			}
			if (num7 < maxMissionReachedKid)
			{
				num7 = maxMissionReachedKid;
			}
			int num8 = Mathf.CeilToInt((float)num7 / 10f) * 10;
			if (maxMissionReached < 1)
			{
				_ = 10;
			}
			else
				_ = num8;
			_ = 1;
			num6 = UnityEngine.Random.Range(0, Levels.Count);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(Levels[num6], lvlPos, Quaternion.identity);
			gameObject2.SetActive(value: true);
			currentLoadedLevel = gameObject2;
			array2 = GameObject.FindGameObjectsWithTag("Player");
			for (int m = 0; m < array2.Length; m++)
			{
				UnityEngine.Object.Destroy(array2[m].transform.parent.gameObject);
			}
			array2 = GameObject.FindGameObjectsWithTag("Enemy");
			for (int m = 0; m < array2.Length; m++)
			{
				EnemyTargetingSystemNew component5 = array2[m].transform.parent.GetChild(1).GetChild(0).GetComponent<EnemyTargetingSystemNew>();
				if ((bool)component5)
				{
					component5.isHuntingEnemies = true;
				}
			}
		}
	}

	public void EndTheGame()
	{
		Victory = true;
		DisableGame();
		endGame();
	}

	private IEnumerator SetAnimatorBoolFalse()
	{
		yield return new WaitForSeconds(1f);
		canvasAnimator.SetBool("NextMission", value: false);
	}

	public Vector3 GetValidLocation(bool CheckForDist, float minimalDist, Vector3 CallerPos, bool TargetPlayer)
	{
		Vector3 vector = Vector3.zero;
		int num = 0;
		int maxExclusive = 0;
		bool flag = false;
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			num = 14;
			maxExclusive = 11;
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			num = 18;
			maxExclusive = 14;
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			num = 21;
			maxExclusive = 16;
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			num = 24;
			maxExclusive = 18;
		}
		if (MapCornerObject == null)
		{
			MapCornerObject = GameObject.Find("ALL_FIELDS");
		}
		for (int i = 0; i < 140; i++)
		{
			int num2 = UnityEngine.Random.Range(0, num);
			int num3 = UnityEngine.Random.Range(0, maxExclusive);
			bool flag2 = false;
			if (MapCornerObject == null)
			{
				Debug.Log("NO ALL FIELDS");
				flag = false;
				break;
			}
			vector = MapCornerObject.transform.GetChild(0).position + new Vector3(num2 * 2, 0f, -num3 * 2);
			Collider[] array;
			if (minimalDist > 9999f && !TargetPlayer)
			{
				array = Physics.OverlapSphere(vector, 0.5f);
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].tag == "Solid")
					{
						flag2 = true;
						break;
					}
				}
			}
			else if (!TargetPlayer)
			{
				array = Physics.OverlapSphere(vector, 1f);
				foreach (Collider collider in array)
				{
					if (collider.tag == "Solid" || collider.tag == "MapBorder")
					{
						flag2 = true;
						break;
					}
				}
			}
			int index = num3 * (num + 1) + num2;
			PathfindingBlock component = MapCornerObject.transform.GetChild(index).GetComponent<PathfindingBlock>();
			if ((bool)component && component.SolidInMe)
			{
				flag2 = true;
			}
			if (flag2)
			{
				continue;
			}
			array = Physics.OverlapSphere(vector, 3f);
			foreach (Collider collider2 in array)
			{
				if (collider2.tag == "Player" && TargetPlayer)
				{
					flag2 = false;
					flag = true;
					break;
				}
				if (collider2.tag == "Player" || collider2.tag == "Boss" || collider2.tag == "Mine" || collider2.tag == "Bullet")
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				continue;
			}
			if (!TargetPlayer)
			{
				if (!CheckForDist)
				{
					flag = true;
					break;
				}
				if (!(Vector3.Distance(CallerPos, vector) < minimalDist))
				{
					flag = true;
					break;
				}
				flag = false;
			}
			else if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			return vector;
		}
		Debug.Log("FOUND NO SPOT");
		return Vector3.zero;
	}

	public Vector3 GetValidLocation(Transform[] targets)
	{
		if (targets.Length < 1)
		{
			return Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < 100; i++)
		{
			bool flag = false;
			Transform transform = targets[UnityEngine.Random.Range(0, targets.Length)];
			PathfindingBlock myPB = transform.GetComponent<DrawGizmos>().myPB;
			if (transform.GetComponent<DrawGizmos>().Picked)
			{
				flag = true;
			}
			if ((bool)myPB && myPB.SolidInMe)
			{
				flag = true;
			}
			if (!flag && !flag)
			{
				vector = transform.position;
				transform.GetComponent<DrawGizmos>().GotPicked();
				break;
			}
		}
		if (vector == Vector3.zero)
		{
			Debug.LogError("GOT A ZERO LOCATION ERROR!");
		}
		return vector;
	}

	public void LoadSurvivalMap()
	{
		Levels[OptionsMainMenu.instance.StartLevel].SetActive(value: true);
		Players.Clear();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in array)
		{
			Players.Add(item);
		}
	}

	private void CheckSecretMission()
	{
		SecretMissionCounter--;
		if (SecretMissionCounter == 1)
		{
			if (!FoundSecretMissions.Contains(CurrentMission))
			{
				FoundSecretMissions.Add(CurrentMission);
			}
			if (FoundSecretMissions.Count > 10 && (bool)AchievementsTracker.instance)
			{
				AchievementsTracker.instance.completeAchievement(33);
			}
			Debug.Log("CHECK ESCROTR");
			SaveData(skipCloud: false);
		}
	}

	public void endGame()
	{
		AmountPlayersThatNeedRevive = 0;
		if (CurrentMission < 1)
		{
			AchievementsTracker.instance.ResetVariables();
		}
		CurrentMission++;
		bool flag = false;
		OnlyCompanionLeft = false;
		if (CurrentMission == lastMissionNumber)
		{
			musicScript.StopMusic();
			flag = true;
		}
		else if (MapEditorMaster.instance != null)
		{
			if (MapEditorMaster.instance.inPlayingMode)
			{
				if (CurrentMission < MapEditorMaster.instance.Levels.Count)
				{
					musicScript.Victory();
				}
				else
				{
					musicScript.StopMusic();
					flag = true;
				}
			}
			else
			{
				musicScript.Victory();
			}
		}
		else
		{
			musicScript.Victory();
			if (!inMapEditor && !isZombieMode)
			{
				for (int i = 0; i < AchievementsTracker.instance.MissionUnlocked.Length; i++)
				{
					int num = AchievementsTracker.instance.MissionUnlocked[i] + 1;
					if (maxMissionReached <= num && maxMissionReachedHard <= num && (maxMissionReached == num || maxMissionReachedHard == num) && !OptionsMainMenu.instance.MapEditorTankMessagesReceived[i])
					{
						OptionsMainMenu.instance.MapEditorTankMessagesReceived[i] = true;
						OptionsMainMenu.instance.SaveNewData();
						AchievementsTracker.instance.ShowUnlockMessage(AchievementsTracker.instance.MissionUnlocked[i], i);
					}
				}
			}
		}
		if (!flag)
		{
			if (CurrentMission % 5 == 0 && CurrentMission != 0 && CurrentMission % 10 != 0)
			{
				NAS.newTank = true;
			}
			else if (CurrentMission != 0 && CurrentMission % 10 == 0 && MapEditorMaster.instance == null)
			{
				NAS.checkPoint = true;
				NAS.newTank = true;
			}
			NAS.NextRound();
		}
		else
		{
			NAS.FinishGame();
		}
	}

	public void AddBonusTank()
	{
		Lives++;
		SFXManager.instance.PlaySFX(ExtraLiveSound, 1f, null);
	}

	public IEnumerator GetTankTeamData(bool fast)
	{
		float seconds = (fast ? 0.01f : 0.1f);
		yield return new WaitForSeconds(seconds);
		for (int i = 0; i < AmountTeamTanks.Length; i++)
		{
			AmountTeamTanks[i] = 0;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		for (int j = 0; j < array.Length; j++)
		{
			EnemyAI component = array[j].GetComponent<EnemyAI>();
			if ((bool)component)
			{
				AmountTeamTanks[component.MyTeam]++;
			}
		}
		array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject gameObject in array)
		{
			MoveTankScript component2 = gameObject.GetComponent<MoveTankScript>();
			if ((bool)component2)
			{
				AmountTeamTanks[component2.MyTeam]++;
				PlayerTeamColor[component2.playerId] = component2.MyTeam;
				continue;
			}
			EnemyAI component3 = gameObject.GetComponent<EnemyAI>();
			if ((bool)component3)
			{
				AmountTeamTanks[component3.MyTeam]++;
				PlayerTeamColor[1] = component3.MyTeam;
			}
		}
		if (!(MapEditorMaster.instance != null))
		{
			yield break;
		}
		AmountEnemyTanks = 0;
		for (int k = 0; k < AmountTeamTanks.Length; k++)
		{
			if (k != PlayerTeamColor[0] || PlayerTeamColor[0] == 0)
			{
				AmountEnemyTanks += AmountTeamTanks[k];
			}
		}
	}

	private void HandlePlayerQueue()
	{
		for (int i = 0; i < PlayerJoined.Count; i++)
		{
			PlayerDied[i] = false;
			if (PlayerJoining[i])
			{
				PlayerJoined[i] = true;
				PlayerJoining[i] = false;
			}
		}
	}

	public void nextLevel()
	{
		Debug.Log("Start new level!");
		AmountCalledInTanks = 0;
		HandlePlayerQueue();
		OnlyCompanionLeft = false;
		SaveData(skipCloud: false);
		AmountPlayersThatNeedRevive = 0;
		AmountEnemyTanks = 0;
		for (int i = 0; i < PlayerDown.Count; i++)
		{
			PlayerDown[i] = false;
		}
		for (int j = 0; j < AmountTeamTanks.Length; j++)
		{
			AmountTeamTanks[j] = 0;
		}
		EnemyTankTracksAudio = 0;
		if ((bool)currentLoadedLevel && !MapEditorMaster.instance)
		{
			currentLoadedLevel.SetActive(value: false);
			UnityEngine.Object.Destroy(currentLoadedLevel);
		}
		levelIsLoaded = false;
		RenderSettings.skybox = CloudGeneration.instance.SkyBoxClear;
		CloudGeneration.instance.GlobalVolume.profile = CloudGeneration.instance.PPPs[0];
		if ((CurrentMission == 49 || CurrentMission == 29) && MapEditorMaster.instance == null)
		{
			if (!MapEditorMaster.instance)
			{
				mapBorders.SetActive(value: false);
			}
			if (floor != null)
			{
				floor.SetActive(value: false);
			}
			if (CurrentMission == 29)
			{
				RenderSettings.skybox = CloudGeneration.instance.SkyBoxDark;
				CloudGeneration.instance.GlobalVolume.profile = CloudGeneration.instance.PPPs[5];
			}
		}
		else
		{
			if (!MapEditorMaster.instance)
			{
				mapBorders.SetActive(value: true);
			}
			if (floor != null)
			{
				floor.SetActive(value: true);
			}
		}
		if (TrainLevelBlocks.Length != 0)
		{
			GameObject[] trainLevelBlocks = TrainLevelBlocks;
			foreach (GameObject gameObject in trainLevelBlocks)
			{
				gameObject.SetActive(value: true);
				if (gameObject.transform.tag == "Snow" && !OptionsMainMenu.instance.SnowMode)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
		bool flag = false;
		foreach (int nightLevel in NightLevels)
		{
			if (CurrentMission == nightLevel)
			{
				flag = true;
				CloudGeneration.instance.MakeItDark();
				break;
			}
		}
		if ((bool)MapEditorMaster.instance)
		{
			CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(MapEditorMaster.instance.WeatherTypes[CurrentMission], force: false));
		}
		if (!flag)
		{
			CloudGeneration.instance.MakeItDay();
		}
		foreach (int trainLevel in TrainLevels)
		{
			if (CurrentMission == trainLevel)
			{
				GameObject[] trainLevelBlocks = TrainLevelBlocks;
				for (int k = 0; k < trainLevelBlocks.Length; k++)
				{
					trainLevelBlocks[k].SetActive(value: false);
				}
				break;
			}
		}
		if (isOfficialCampaign && MissionTutorials.Count > CurrentMission && MissionTutorials[CurrentMission] != null && MissionTutorials[CurrentMission] != "")
		{
			TutorialMaster.instance.ShowTutorial(MissionTutorials[CurrentMission]);
		}
		if (CurrentMission == 49 && MapEditorMaster.instance == null)
		{
			RenderSettings.ambientLight = ambientCLRlvl50;
		}
		DestroyTemps();
		Victory = false;
		Debug.Log("Time to load map!");
		if ((bool)MapEditorMaster.instance)
		{
			if (MapEditorMaster.instance.inPlayingMode)
			{
				MapEditorMaster.instance.RemoveCurrentObjects();
				MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlaceAllProps(MapEditorMaster.instance.Levels[CurrentMission].MissionDataProps, oldVersion: false, CurrentMission));
				currentLoadedLevel = Levels[0];
				if ((bool)currentLoadedLevel)
				{
					SetPlayerPosition("Main_Tank_Prop(Clone)", "Second_Tank_Prop(Clone)", "Third_Tank_Prop(Clone)", "Fourth_Tank_Prop(Clone)");
				}
			}
		}
		else if (Levels[CurrentMission] == null)
		{
			Debug.Log("Loading custom map!");
			Stream stream = new MemoryStream(LevelsText[CurrentMission].bytes);
			SingleMapEditorData sMED = new BinaryFormatter().Deserialize(stream) as SingleMapEditorData;
			stream.Close();
			currentLoadedLevel = GetComponent<LoadCampaignMap>().LoadMap(sMED, CurrentMission);
			if ((bool)currentLoadedLevel)
			{
				SetPlayerPosition("Main_Tank_FBX(Clone)", "Second_Tank_FBX(Clone)", "Third_Tank_FBX(Clone)", "Fourth_Tank_FBX(Clone)");
			}
		}
		else if (!Levels[CurrentMission].name.Contains("Custom"))
		{
			currentLoadedLevel = UnityEngine.Object.Instantiate(Levels[CurrentMission], new Vector3(-9f, 0f, 1f), Quaternion.identity);
			currentLoadedLevel.SetActive(value: true);
			if ((bool)currentLoadedLevel)
			{
				SetPlayerPosition("Main_Tank_FBX", "Second_Tank_FBX", "Third_Tank_FBX", "Fourth_Tank_FBX");
			}
		}
		else
		{
			Debug.Log("yes custom");
			if (CurrentMission > 0 && Levels[CurrentMission - 1] != null)
			{
				UnityEngine.Object.Destroy(Levels[CurrentMission - 1]);
			}
			Levels[CurrentMission].SetActive(value: true);
			currentLoadedLevel = Levels[CurrentMission];
			if ((bool)currentLoadedLevel)
			{
				SetPlayerPosition("Main_Tank_Prop(Clone)", "Second_Tank_Prop(Clone)", "Third_Tank_Prop(Clone)", "Fourth_Tank_Prop(Clone)");
			}
		}
		for (int l = 0; l < CampaignPressurePlates.Count; l++)
		{
			if (CampaignPressurePlates[l].MissionNumber == CurrentMission + 1)
			{
				Debug.Log("PRESSURE PLATE SPAWNED");
				UnityEngine.Object.Instantiate(PressurePlate, CampaignPressurePlates[l].position, Quaternion.Euler(0f, 0f, 90f)).transform.SetParent(currentLoadedLevel.transform);
			}
		}
		FindPlayers();
		if (!MapEditorMaster.instance)
		{
			AmountGoodTanks = Players.Count();
		}
		StartCoroutine(GetTankTeamData(fast: false));
		if ((bool)MapEditorMaster.instance)
		{
			StartCoroutine(LateUpdatePlayerToAI());
		}
		else
		{
			UpdatePlayerToAI();
		}
		FindPlayers();
		if (MapEditorMaster.instance == null)
		{
			AccountMaster.instance.UpdateServerStatus(CurrentMission);
		}
		AmountEnemyTanks = GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("Boss").Length;
		levelIsLoaded = true;
		BreakableBlocksLocations.Clear();
		StoneBlocksLocations.Clear();
		TNTLocations.Clear();
		BreakableHalfBlocksLocations.Clear();
		GetBreakableBlocksLocations(currentLoadedLevel);
		Debug.Log("ja lets go enxt LEVEL");
		if (CurrentMission == 99 && MapEditorMaster.instance == null)
		{
			mapBorders.SetActive(value: false);
			CameraFollowPlayer component = Camera.main.transform.parent.GetComponent<CameraFollowPlayer>();
			if ((bool)component)
			{
				component.enabled = true;
			}
		}
		if (MapEditorMaster.instance != null)
		{
			StartCoroutine(DisableGameDelay());
			if (MapEditorMaster.instance.MissionFloorTextures.Length != 0)
			{
				LoadModTexture component2 = instance.floor.GetComponent<LoadModTexture>();
				if ((bool)component2 && component2.IsModded)
				{
					return;
				}
				floor.GetComponent<MeshRenderer>().material = MapEditorMaster.instance.FloorMaterials[MapEditorMaster.instance.MissionFloorTextures[CurrentMission]];
			}
			if (MapEditorMaster.instance.NoBordersMissions.Count > 0)
			{
				MapEditorMaster.instance.UpdateMapBorder(CurrentMission);
			}
		}
		else
		{
			DisableGame();
		}
		Mission_Text.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (CurrentMission + 1);
		if (isOfficialCampaign)
		{
			MissionName_Text.text = "'" + LocalizationMaster.instance.GetText("Mission_" + (CurrentMission + 1)) + "'";
		}
		else
		{
			MissionName_Text.text = "'" + MissionNames[CurrentMission] + "'";
		}
	}

	private IEnumerator LateUpdatePlayerToAI()
	{
		yield return new WaitForSeconds(0.5f);
		UpdatePlayerToAI();
	}

	private void UpdatePlayerToAI()
	{
		FindPlayers();
		if (Players.Count <= 1)
		{
			return;
		}
		foreach (GameObject player in Players)
		{
			Debug.Log("Current player before change:" + player.name);
		}
		if ((!OptionsMainMenu.instance.AIcompanion[1] && !OptionsMainMenu.instance.AIcompanion[2] && !OptionsMainMenu.instance.AIcompanion[3]) || (bool)CM)
		{
			return;
		}
		for (int i = 1; i < OptionsMainMenu.instance.AIcompanion.Length; i++)
		{
			if (!OptionsMainMenu.instance.AIcompanion[i])
			{
				continue;
			}
			Debug.Log("CHANGING COMPANION" + i);
			PlayerModeWithAI[i] = 1;
			for (int j = 0; j < Players.Count; j++)
			{
				if (Players[j] != null && !Players[j].name.Contains("AI") && Players[j].GetComponent<MoveTankScript>().playerId == i)
				{
					Debug.Log("YES CHANGE " + Players[j].name + " NOW" + i);
					ChangePlayerToAI(j, i);
				}
			}
		}
	}

	private void ChangePlayerToAI(int playerIndex, int playerType)
	{
		Vector3 position = Players[playerIndex].transform.position;
		Quaternion rotation = Players[playerIndex].transform.rotation;
		GameObject gameObject = UnityEngine.Object.Instantiate(playerType switch
		{
			2 => AIPlayer3Prefab, 
			1 => AIPlayer2Prefab, 
			_ => AIPlayer4Prefab, 
		}, position, rotation, Players[playerIndex].transform.parent.transform.parent);
		if ((bool)MapEditorMaster.instance)
		{
			MapEditorGridPiece myMEGP = Players[playerIndex].GetComponent<MapEditorProp>().myMEGP;
			MapEditorProp mapEditorProp = gameObject.transform.GetChild(0).gameObject.AddComponent<MapEditorProp>();
			gameObject.transform.GetChild(0).gameObject.GetComponent<EnemyAI>().MyTeam = Players[playerIndex].GetComponent<MapEditorProp>().TeamNumber;
			mapEditorProp.TeamNumber = Players[playerIndex].GetComponent<MapEditorProp>().TeamNumber;
			if (playerType == 1)
			{
				mapEditorProp.isPlayerTwo = true;
			}
			if (playerType == 2)
			{
				mapEditorProp.isPlayerThree = true;
			}
			if (playerType == 3)
			{
				mapEditorProp.isPlayerFour = true;
			}
			Renderer[] array = (mapEditorProp.myRends = new MeshRenderer[1]);
			mapEditorProp.myRends[0] = mapEditorProp.transform.Find("Cube.003").GetComponent<MeshRenderer>();
			mapEditorProp.OriginalBodyColor = mapEditorProp.myRends[0].materials[0].color;
			mapEditorProp.TeamNumber = myMEGP.MyTeamNumber;
		}
		Debug.Log("Destroying player: " + Players[playerIndex].transform.parent.gameObject.name);
		UnityEngine.Object.Destroy(Players[playerIndex].transform.parent.gameObject);
		if (CurrentMission == 99 && !MapEditorMaster.instance)
		{
			UnityEngine.Object.Destroy(gameObject);
			AmountGoodTanks = 1;
		}
		DisableGame();
		disableTheGame();
	}

	private IEnumerator DisableGameDelay()
	{
		yield return new WaitForSeconds(0.01f);
		DisableGame();
	}

	public void TestLevel()
	{
		AmountCalledInTanks = 0;
		for (int i = 0; i < PlayerJoined.Count; i++)
		{
			PlayerDied[i] = false;
		}
		foreach (int nightLevel in NightLevels)
		{
			if (CurrentMission == nightLevel)
			{
				CloudGeneration.instance.MakeItDark();
				break;
			}
		}
		if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.isTesting)
		{
			CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(MapEditorMaster.instance.WeatherTypes[CurrentMission], force: false));
		}
		DestroyTemps();
		AmountEnemyTanks = GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("Boss").Length;
		StartCoroutine(GetTankTeamData(fast: false));
		Debug.LogError("Lets start the test!");
		Camera.main.GetComponent<AudioSource>().volume = 0f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		AmountGoodTanks = PlayerJoined.Count;
		SetPlayerPosition("Main_Tank_Prop(Clone)", "Second_Tank_Prop(Clone)", "Third_Tank_Prop(Clone)", "Fourth_Tank_Prop(Clone)");
		UpdatePlayerToAI();
		FindPlayers();
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		foreach (GameObject enemy in Enemies)
		{
			MapEditorProp component = enemy.GetComponent<MapEditorProp>();
			if ((bool)component && component.MyDifficultySpawn > OptionsMainMenu.instance.currentDifficulty)
			{
				enemy.transform.parent.transform.gameObject.SetActive(value: false);
			}
		}
		levelIsLoaded = true;
		BreakableBlocksLocations.Clear();
		BreakableHalfBlocksLocations.Clear();
		StoneBlocksLocations.Clear();
		TNTLocations.Clear();
		GetBreakableBlocksLocations(Levels[CurrentMission]);
		DisableGame();
		MapEditorMaster.instance.CDS.start = true;
	}

	private IEnumerator ResetTestLevel(bool skip)
	{
		DestroyTemps();
		if (!skip)
		{
			yield return new WaitForSeconds(1f);
		}
		Debug.Log("Reset test level");
		CloudGeneration.instance.MakeItDay();
		CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(0, force: true));
		for (int i = 0; i < PlayerDown.Count; i++)
		{
			PlayerDown[i] = false;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Mine");
		for (int j = 0; j < array.Length; j++)
		{
			UnityEngine.Object.Destroy(array[j]);
		}
		Camera.main.GetComponent<AudioSource>().volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		MapEditorMaster.instance.isTesting = false;
		Animator component = Camera.main.GetComponent<Animator>();
		CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(0, force: true));
		component.SetBool("CameraUp", value: true);
		component.SetBool("CameraDownEditor", value: false);
		component.SetBool("CameraDown", value: false);
		GetComponent<AudioSource>().Play();
		MapEditorMaster.instance.EditingCanvas.SetActive(value: true);
		GameHasStarted = false;
		AmountPlayersThatNeedRevive = 0;
		MapEditorMaster.instance.ErrorField.transform.localPosition = MapEditorMaster.instance.AnimatorStartHeight;
		for (int k = 0; k < MapEditorMaster.instance.ColorsTeamsPlaced.Length; k++)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[k] = 0;
		}
		MapEditorMaster.instance.enemyTanksPlaced[CurrentMission] = 0;
		MapEditorMaster.instance.playerOnePlaced[CurrentMission] = 0;
		MapEditorMaster.instance.playerTwoPlaced[CurrentMission] = 0;
		MapEditorMaster.instance.canPlaceProp = false;
		MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlacePropTimer(2f));
		array = GameObject.FindGameObjectsWithTag("MapeditorField");
		for (int j = 0; j < array.Length; j++)
		{
			MapEditorGridPiece component2 = array[j].GetComponent<MapEditorGridPiece>();
			for (int l = 0; l < 5; l++)
			{
				if (component2.propOnMe[l])
				{
					int team = -1;
					if (component2.MyTeamNumber > -1)
					{
						team = component2.MyTeamNumber;
					}
					UnityEngine.Object.Destroy(component2.myProp[l]);
					component2.propOnMe[l] = false;
					component2.SpawnInProps(component2.myPropID[l], component2.rotationDirection[l], team, l, component2.SpawnDifficulty);
					if (component2.myPropID[l] > 5 && component2.myPropID[l] < 40)
					{
						MapEditorMaster.instance.enemyTanksPlaced[CurrentMission]++;
					}
				}
			}
		}
		DestroyTemps();
		StartCoroutine(TempsDestroy());
		EnemyTankTracksAudio = 0;
		musicScript.StopMusic();
		MapEditorMaster.instance.CDS.gameObject.SetActive(value: false);
	}

	private IEnumerator TempsDestroy()
	{
		yield return new WaitForSeconds(1f);
		DestroyTemps();
	}

	public void GetBreakableBlocksLocations(GameObject searchPlace)
	{
		for (int i = 0; i < searchPlace.transform.childCount; i++)
		{
			DestroyableWall component = searchPlace.transform.GetChild(i).GetComponent<DestroyableWall>();
			if (component != null)
			{
				if (!component.isHalfSlab)
				{
					BreakableBlocksLocations.Add(searchPlace.transform.GetChild(i).transform.position);
				}
				else if (component.IsStone)
				{
					StoneBlocksLocations.Add(searchPlace.transform.GetChild(i).transform.position);
				}
				else
				{
					BreakableHalfBlocksLocations.Add(searchPlace.transform.GetChild(i).transform.position);
				}
			}
			if ((bool)searchPlace.transform.GetChild(i).GetComponent<ExplosiveBlock>())
			{
				TNTLocations.Add(searchPlace.transform.GetChild(i).transform.position);
			}
			GetBreakableBlocksLocations(searchPlace.transform.GetChild(i).gameObject);
		}
	}

	public void DestroyTemps()
	{
		GameObject[] first = GameObject.FindGameObjectsWithTag("Temp");
		GameObject[] second = GameObject.FindGameObjectsWithTag("Mine");
		GameObject[] array = first.Concat(second).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.parent == null)
			{
				UnityEngine.Object.Destroy(array[i]);
			}
		}
	}

	public void DisableGame()
	{
		AmountMinesPlaced = 0;
		GameHasStarted = false;
		restartGame = false;
		FindPlayers();
		if (Enemies != null)
		{
			Enemies.Clear();
		}
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] array = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject enemy in Enemies)
		{
			if ((bool)enemy)
			{
				EnemyAI component = enemy.GetComponent<EnemyAI>();
				if (component != null && component.MyTeam != 2 && Enemies.Contains(component.gameObject) && MapEditorMaster.instance == null)
				{
					list.Add(component.gameObject);
				}
			}
		}
		foreach (GameObject item in list)
		{
			Enemies.Remove(item);
		}
		for (int num = Enemies.Count - 1; num >= 0; num--)
		{
			if (Enemies[num] == null)
			{
				Enemies.RemoveAt(num);
			}
		}
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
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

	private void LoadData()
	{
		if (AccountMaster.instance.isSignedIn)
		{
			maxMissionReached = AccountMaster.instance.PDO.maxMission0;
			maxMissionReachedHard = AccountMaster.instance.PDO.maxMission2;
			maxMissionReachedKid = AccountMaster.instance.PDO.maxMission1;
		}
		else if (SavingData.ExistData())
		{
			ProgressDataNew progressDataNew = SavingData.LoadData();
			maxMissionReached = progressDataNew.cM;
			maxMissionReachedHard = progressDataNew.cH;
			maxMissionReachedKid = progressDataNew.cK;
		}
		else
		{
			Debug.Log("NO SAVE FILE FOUND");
			totalKills = 0;
			totalWins = 0;
			totalDefeats = 0;
			maxMissionReached = 0;
			maxMissionReachedHard = 0;
			maxMissionReachedKid = 0;
		}
	}

	public void ResetData()
	{
		totalKills = 0;
		totalWins = 0;
		totalDefeats = 0;
		maxMissionReached = 0;
		for (int i = 0; i < highestWaves.Length; i++)
		{
			highestWaves[i] = 0;
		}
		for (int j = 0; j < TankColorKilled.Count; j++)
		{
			TankColorKilled[j] = 0;
		}
		survivalTanksKilled = 0;
		maxMissionReachedHard = 0;
		maxMissionReachedKid = 0;
		totalKillsBounce = 0;
		totalRevivesPerformed = 0;
	}

	public void SaveData(bool skipCloud)
	{
		if (MapEditorMaster.instance != null)
		{
			Debug.LogError("cant save because in map editor mode");
		}
		else
		{
			if (activatedBackup || !(MapEditorMaster.instance == null))
			{
				return;
			}
			if (OptionsMainMenu.instance.currentDifficulty == 0)
			{
				if (CurrentMission > 0 && CurrentMission >= maxMissionReached)
				{
					maxMissionReached = CurrentMission + 1;
				}
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 1)
			{
				if (CurrentMission > 0 && CurrentMission >= maxMissionReachedKid)
				{
					maxMissionReachedKid = CurrentMission + 1;
					if (maxMissionReached < maxMissionReachedKid)
					{
						maxMissionReached = maxMissionReachedKid;
					}
				}
				else if (CurrentMission > 0 && CurrentMission >= maxMissionReached)
				{
					maxMissionReached = CurrentMission + 1;
				}
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 2)
			{
				if (CurrentMission > 0 && CurrentMission >= maxMissionReachedHard)
				{
					maxMissionReachedHard = CurrentMission + 1;
					if (maxMissionReachedKid < maxMissionReachedHard)
					{
						maxMissionReachedKid = maxMissionReachedHard;
					}
					if (maxMissionReached < maxMissionReachedHard)
					{
						maxMissionReached = maxMissionReachedHard;
					}
				}
				else if (CurrentMission > 0 && CurrentMission >= maxMissionReachedKid)
				{
					maxMissionReachedKid = CurrentMission + 1;
					if (maxMissionReached < maxMissionReachedKid)
					{
						maxMissionReached = maxMissionReachedKid;
					}
				}
				else if (CurrentMission > 0 && CurrentMission >= maxMissionReached)
				{
					maxMissionReached = CurrentMission + 1;
				}
			}
			if (!AccountMaster.instance.isSignedIn)
			{
				SavingData.SaveData(this, "tank_progress");
			}
			else if (!isZombieMode && MapEditorMaster.instance == null)
			{
				AccountMaster.instance.SaveCloudData(7, -1, 0, bounceKill: false);
			}
		}
	}

	public void defeated()
	{
		if (!inMapEditor)
		{
			if (!isZombieMode)
			{
				totalDefeats++;
				AccountMaster.instance.SaveCloudData(2, 1, 0, bounceKill: false);
				Debug.Log("DEFEATION");
				SaveData(skipCloud: false);
			}
			PlayerAlive = false;
			musicScript.Defeat();
			NAS.NextRound();
			NAS.restart = true;
			DisableGame();
		}
	}

	public void playLostLive()
	{
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.lostLife, 1f, null);
	}

	private void Mission99SpawnPoof(Vector3 PlayerPosition)
	{
		if (CurrentMission == 99)
		{
			Debug.Log("CREATING PARTICLES");
			GameObject obj = UnityEngine.Object.Instantiate(SpawningPlayerParticles, PlayerPosition, Quaternion.identity, currentLoadedLevel.transform);
			ParticleSystem component = obj.GetComponent<ParticleSystem>();
			obj.GetComponent<Play2DClipOnce>().overrideGameStarted = true;
			component.Play();
			obj.transform.Rotate(new Vector3(-90f, 0f, 0f));
		}
	}

	public void ResetLevel()
	{
		Debug.LogWarning("Reset level");
		AmountCalledInTanks = 0;
		HandlePlayerQueue();
		DestroyTemps();
		OnlyCompanionLeft = false;
		AmountPlayersThatNeedRevive = 0;
		for (int i = 0; i < PlayerDown.Count; i++)
		{
			PlayerDown[i] = false;
		}
		GameObject[] array;
		if (PlayerJoined.Count > 1 && !MapEditorMaster.instance)
		{
			Debug.LogWarning("Reset players" + PlayerJoined.Count);
			for (int j = 0; j < PlayerJoined.Count; j++)
			{
				if ((!PlayerJoined[j] && PlayerModeWithAI[j] == 0) || (CurrentMission == 99 && PlayerModeWithAI[j] == 1 && MapEditorMaster.instance == null))
				{
					continue;
				}
				switch (j)
				{
				case 0:
					Debug.LogWarning(" players 1");
					if (currentLoadedLevel.transform.Find("Main_Tank_FBX") == null)
					{
						Debug.LogWarning(" players 1 spawned");
						UnityEngine.Object.Instantiate(PlayerPrefab, playerLocation[0], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[0];
						Mission99SpawnPoof(playerLocation[0]);
					}
					else
					{
						Debug.LogWarning(" players 1 already here", Levels[CurrentMission].transform.Find("Main_Tank_FBX").gameObject);
					}
					continue;
				case 1:
					if (PlayerModeWithAI[j] == 1)
					{
						if (!currentLoadedLevel.transform.Find("AI_Tank_FBX"))
						{
							UnityEngine.Object.Instantiate(AIPlayer2Prefab, playerLocation[1], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[1];
							Mission99SpawnPoof(playerLocation[1]);
						}
					}
					else if (!currentLoadedLevel.transform.Find("Second_Tank_FBX"))
					{
						UnityEngine.Object.Instantiate(Player2Prefab, playerLocation[1], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[1];
						Mission99SpawnPoof(playerLocation[1]);
					}
					continue;
				case 2:
					if (PlayerModeWithAI[j] == 1)
					{
						if (!currentLoadedLevel.transform.Find("AI_Tank_FBX_third"))
						{
							UnityEngine.Object.Instantiate(AIPlayer3Prefab, playerLocation[2], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[2];
							Mission99SpawnPoof(playerLocation[2]);
						}
					}
					else if (!currentLoadedLevel.transform.Find("Third_Tank_FBX"))
					{
						UnityEngine.Object.Instantiate(Player3Prefab, playerLocation[2], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[2];
						Mission99SpawnPoof(playerLocation[2]);
					}
					continue;
				}
				if (PlayerModeWithAI[j] == 1)
				{
					if (!currentLoadedLevel.transform.Find("AI_Tank_FBX_fourth"))
					{
						UnityEngine.Object.Instantiate(AIPlayer4Prefab, playerLocation[3], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[3];
						Mission99SpawnPoof(playerLocation[3]);
					}
				}
				else if (!currentLoadedLevel.transform.Find("Fourth_Tank_FBX"))
				{
					GameObject obj = UnityEngine.Object.Instantiate(Player4Prefab, playerLocation[3], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
					obj.transform.parent = currentLoadedLevel.transform;
					obj.transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[3];
					Mission99SpawnPoof(playerLocation[3]);
				}
			}
		}
		else if ((bool)MapEditorMaster.instance && (bool)MapEditorMaster.instance)
		{
			Debug.Log("RESETTING MAP PIECES THINGIES");
			array = GameObject.FindGameObjectsWithTag("MapeditorField");
			for (int k = 0; k < array.Length; k++)
			{
				MapEditorGridPiece component = array[k].GetComponent<MapEditorGridPiece>();
				for (int l = 0; l < 5; l++)
				{
					if (component.propOnMe[l] && component.mission == CurrentMission && (component.myPropID[l] == 4 || component.myPropID[l] == 5 || component.myPropID[l] == 28 || component.myPropID[l] == 29))
					{
						if ((component.myPropID[l] != 5 || PlayerTeamColor[0] == PlayerTeamColor[1]) && (component.myPropID[l] != 28 || PlayerTeamColor[0] == PlayerTeamColor[2]) && (component.myPropID[l] != 29 || PlayerTeamColor[0] == PlayerTeamColor[3]))
						{
							int team = -1;
							if (component.MyTeamNumber > -1)
							{
								team = component.MyTeamNumber;
							}
							UnityEngine.Object.Destroy(component.myProp[l]);
							component.propOnMe[l] = false;
							component.SpawnInProps(component.myPropID[l], component.rotationDirection[l], team, l, component.SpawnDifficulty);
						}
					}
					else if (component.propOnMe[l] && component.mission == CurrentMission && component.myProp[l] == null && (component.myPropID[l] == 40 || component.myPropID[l] == 45 || component.myPropID[l] == 2 || component.myPropID[l] == 49))
					{
						component.propOnMe[l] = false;
						component.SpawnInProps(component.myPropID[l], component.rotationDirection[l], 0, l, component.SpawnDifficulty);
					}
				}
			}
		}
		UpdatePlayerToAI();
		if (!MapEditorMaster.instance)
		{
			for (int m = 0; m < BreakableBlocksLocations.Count; m++)
			{
				int num = 0;
				Collider[] array2 = Physics.OverlapSphere(BreakableBlocksLocations[m], 0.2f);
				foreach (Collider collider in array2)
				{
					if (collider.tag == "Solid" && collider.GetComponent<DestroyableWall>() != null)
					{
						num = 1;
					}
				}
				if (num == 0)
				{
					UnityEngine.Object.Instantiate(CorkBlock, BreakableBlocksLocations[m], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int n = 0; n < StoneBlocksLocations.Count; n++)
			{
				int num2 = 0;
				Collider[] array2 = Physics.OverlapSphere(StoneBlocksLocations[n], 0.2f);
				foreach (Collider collider2 in array2)
				{
					if (collider2.tag == "Solid" && collider2.GetComponent<DestroyableWall>() != null)
					{
						num2 = 1;
					}
				}
				if (num2 == 0)
				{
					UnityEngine.Object.Instantiate(StoneBlock, StoneBlocksLocations[n], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int num3 = 0; num3 < TNTLocations.Count; num3++)
			{
				int num4 = 0;
				Collider[] array2 = Physics.OverlapSphere(TNTLocations[num3], 0.2f);
				foreach (Collider collider3 in array2)
				{
					if (collider3.tag == "Solid" && collider3.GetComponent<ExplosiveBlock>() != null)
					{
						num4 = 1;
					}
				}
				if (num4 == 0)
				{
					UnityEngine.Object.Instantiate(TNTBlock, TNTLocations[num3], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int num5 = 0; num5 < BreakableHalfBlocksLocations.Count; num5++)
			{
				int num6 = 0;
				Collider[] array2 = Physics.OverlapSphere(BreakableHalfBlocksLocations[num5], 0.2f);
				foreach (Collider collider4 in array2)
				{
					if (collider4.tag == "Solid" && collider4.GetComponent<DestroyableWall>() != null)
					{
						num6 = 1;
					}
				}
				if (num6 == 0)
				{
					UnityEngine.Object.Instantiate(HalfCorkBlock, BreakableHalfBlocksLocations[num5], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
		}
		restartGame = true;
		EnemyTankTracksAudio = 0;
		FindPlayers();
		StartCoroutine(GetTankTeamData(fast: false));
		FindPlayers();
		if (Enemies != null)
		{
			Enemies.Clear();
		}
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		AmountGoodTanks = Players.Count;
		AmountEnemyTanks = 0;
		if ((bool)MapEditorMaster.instance)
		{
			if (AmountGoodTanks == 0)
			{
				AmountEnemyTanks = Enemies.Concat(Bosses).ToArray().Length;
			}
			else if (AmountGoodTanks > 0)
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject enemy in Enemies)
				{
					if (!enemy)
					{
						continue;
					}
					EnemyAI component2 = enemy.GetComponent<EnemyAI>();
					if ((bool)component2)
					{
						if (component2.MyTeam != 2 && Enemies.Contains(component2.gameObject) && MapEditorMaster.instance == null)
						{
							list.Add(component2.gameObject);
						}
						MoveTankScript component3 = Players[0].GetComponent<MoveTankScript>();
						if ((bool)component3 && (component2.MyTeam != component3.MyTeam || component3.MyTeam == 0))
						{
							AmountEnemyTanks++;
						}
					}
				}
				foreach (GameObject item in list)
				{
					Enemies.Remove(item);
				}
				for (int num7 = Enemies.Count - 1; num7 >= 0; num7--)
				{
					if (Enemies[num7] == null)
					{
						Enemies.RemoveAt(num7);
					}
				}
			}
		}
		else
		{
			AmountEnemyTanks = Enemies.Concat(Bosses).ToArray().Length;
		}
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		array = Enemies.Concat(Bosses).Concat(EnemyScripts).ToArray();
		for (int k = 0; k < array.Length; k++)
		{
			MonoBehaviour[] components = array[k].GetComponents<MonoBehaviour>();
			for (int num8 = 0; num8 < components.Length; num8++)
			{
				components[num8].enabled = true;
			}
		}
		StartCoroutine(disableTheGame());
	}

	private IEnumerator disableTheGame()
	{
		yield return new WaitForSeconds(0.25f);
		DisableGame();
	}

	public void FindPlayers()
	{
		Players.Clear();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in array)
		{
			Players.Add(item);
		}
	}

	public void FindEnemies()
	{
		if (Enemies != null)
		{
			Enemies.Clear();
		}
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject enemy in Enemies)
		{
			if ((bool)enemy)
			{
				EnemyAI component = enemy.GetComponent<EnemyAI>();
				if (component.MyTeam != 2 && Enemies.Contains(component.gameObject) && MapEditorMaster.instance == null)
				{
					list.Add(component.gameObject);
				}
			}
		}
		foreach (GameObject item in list)
		{
			Enemies.Remove(item);
		}
		for (int num = Enemies.Count - 1; num >= 0; num--)
		{
			if (Enemies[num] == null)
			{
				Enemies.RemoveAt(num);
			}
		}
		AmountEnemyTanks = Enemies.Concat(Bosses).ToArray().Length;
	}

	private IEnumerator startTheGame()
	{
		yield return new WaitForSeconds(0.5f);
		StartGame();
	}

	public void StartGame()
	{
		GameHasStarted = true;
		Players.Clear();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in array)
		{
			Players.Add(item);
		}
		if (Enemies != null)
		{
			Enemies.Clear();
		}
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] array2 = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		EnemyTankTracksAudio = 0;
		if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.Levels[CurrentMission].MissionMessage != null)
		{
			TutorialMaster.instance.ShowTutorial(MapEditorMaster.instance.Levels[CurrentMission].MissionMessage);
		}
		if (CurrentMission == 0)
		{
			counter = 0f;
		}
		array = array2;
		foreach (GameObject gameObject in array)
		{
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			EnemyTargetingSystem[] components2 = gameObject.GetComponents<EnemyTargetingSystem>();
			MonoBehaviour[] array3 = components;
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j].enabled = true;
			}
			AudioSource[] components3 = gameObject.GetComponents<AudioSource>();
			for (int j = 0; j < components3.Length; j++)
			{
				components3[j].enabled = true;
			}
			EnemyTargetingSystem[] array4 = components2;
			for (int j = 0; j < array4.Length; j++)
			{
				_ = array4[j];
			}
		}
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject enemy in Enemies)
		{
			if ((bool)enemy)
			{
				EnemyAI component = enemy.GetComponent<EnemyAI>();
				if (component.MyTeam != 2 && Enemies.Contains(component.gameObject) && MapEditorMaster.instance == null)
				{
					list.Add(component.gameObject);
				}
			}
		}
		foreach (GameObject item2 in list)
		{
			Enemies.Remove(item2);
		}
		for (int num = Enemies.Count - 1; num >= 0; num--)
		{
			if (Enemies[num] == null)
			{
				Enemies.RemoveAt(num);
			}
		}
		array2 = Enemies.Concat(Bosses).ToArray();
		AmountEnemyTanks = array2.Length;
	}

	public void ControllerCheck()
	{
		if (isPlayingWithController)
		{
			numberOfControllers = ReInput.controllers.joystickCount - 1;
		}
		else
		{
			numberOfControllers = ReInput.controllers.joystickCount;
		}
	}

	public void NewRound()
	{
		if (isZombieMode)
		{
			Debug.LogError("NEW ROUND!");
			ZombieTankSpawner component = GetComponent<ZombieTankSpawner>();
			component.Wave++;
			AccountMaster.instance.UpdateServerStatus(component.Wave + 100);
			GameObject[] sun = Sun;
			for (int i = 0; i < sun.Length; i++)
			{
				sun[i].SetActive(value: true);
			}
			component.amountEnemies = 0;
			component.spawned = 0;
			component.spawnAmount = Mathf.RoundToInt((float)(component.Wave * 2 + Mathf.RoundToInt(UnityEngine.Random.Range(-(component.Wave / 2), component.Wave / 2))) * component.multiplier);
			if (component.spawnAmount < 1)
			{
				component.spawnAmount = 2;
			}
			GameHasStarted = true;
			Players.Clear();
			AccountMaster.instance.SaveCloudData(6, component.Wave, 0, bounceKill: false);
			sun = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject item in sun)
			{
				Players.Add(item);
			}
		}
	}
}
