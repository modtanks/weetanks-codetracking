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
	public bool DevModeActive = false;

	public bool inDemoMode = false;

	public bool isZombieMode = false;

	public bool inMenuMode = false;

	public bool inMapEditor = false;

	public bool inOnlineMode = false;

	public bool isOfficialCampaign = false;

	public bool inTankeyTown = false;

	[Header("Stats")]
	public int Lives = 3;

	public int CurrentMission = 0;

	public int AmountEnemyTanks = 0;

	public int AmountGoodTanks = 0;

	public int AmountMinesPlaced = 0;

	public int[] AmountTeamTanks;

	public ProgressDataOnline CurrentData;

	public int AmountCalledInTanks = 0;

	public List<int> Playerkills = new List<int>();

	public int TotalKillsThisSession = 0;

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
	public bool HasGotten100Checkpoint = false;

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

	public Object[] SMED;

	public List<GameObject> Levels = new List<GameObject>();

	public List<TextAsset> LevelsText = new List<TextAsset>();

	public List<int> NightLevels = new List<int>();

	public List<int> TrainLevels = new List<int>();

	public List<int> FoundSecretMissions = new List<int>();

	public int SecretMissionCounter = 0;

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

	public bool Victory = false;

	public bool PlayerAlive = false;

	public bool levelIsLoaded = false;

	public int[] PlayerModeWithAI = new int[4];

	public bool AssassinTankAlive = false;

	public List<bool> PlayerJoining = new List<bool>();

	public List<bool> PlayerJoined = new List<bool>();

	public bool FriendlyFire = false;

	public bool PlayerLeftCooldown = false;

	public bool OnlyCompanionLeft;

	public GameObject mapBorders;

	public GenerateNavMeshSurface GNMS;

	public FinalEndCanvasScript FinalScript;

	public PlayerKillsUI PKU;

	public int EnemyTankTracksAudio = 0;

	public int maxEnemyTankTracks = 4;

	public GameObject currentLoadedLevel;

	private Vector3 lvlPos;

	public Camera MainCamera;

	public GameObject CursorText;

	public List<bool> PlayerDown = new List<bool>();

	public List<bool> PlayerDied = new List<bool>();

	public GameObject SpawningPlayerParticles;

	public float counter = 0f;

	[HideInInspector]
	public int AmountPlayersThatNeedRevive;

	public bool isResettingTestLevel = false;

	private static GameMaster _instance;

	[Header("Game status")]
	public bool restartGame = false;

	public bool GameHasStarted = false;

	public bool GameHasPaused = false;

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

	public bool isReplay = false;

	public TutorialCanvas tutorialCanvas;

	public Color ambientCLR;

	public Color ambientCLRlvl50;

	private bool activatedBackup = false;

	public bool isPlayingWithController = false;

	[Header("Tutorial Things")]
	public bool mission1HasShooted = false;

	public bool mission2HasBoosted = false;

	public bool mission3HasMined = false;

	[Header("Zombie stats")]
	public int[] highestWaves = new int[10];

	public int survivalTanksKilled;

	public int[] TurretsPlaced;

	public CreditsMaster CM;

	public List<int> PlayerTeamColor = new List<int>();

	public int AccountID = -1;

	public int numberOfControllers = 0;

	public static GameMaster instance => _instance;

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
			foreach (GameObject obj in objectsEnablingAtStart)
			{
				obj.SetActive(value: true);
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
			Object.Destroy(currentLoadedLevel);
		}
		for (int i = 0; i < 4; i++)
		{
			PlayerJoined[i] = OptionsMainMenu.instance.PlayerJoined[i];
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
			List<GameObject> PtoRemove = new List<GameObject>();
			foreach (GameObject Player in Players)
			{
				if (Player == null)
				{
					PtoRemove.Add(Player);
				}
			}
			foreach (GameObject P in PtoRemove)
			{
				Players.Remove(P);
			}
		}
		GameObject[] AllEnemies = GameObject.FindGameObjectsWithTag("Enemy");
	}

	public void SetPlayerPosition(string p1tankName, string p2tankName, string p3tankName, string p4tankName)
	{
		if (currentLoadedLevel != null)
		{
			Transform P1Object = currentLoadedLevel.transform.Find(p1tankName);
			if (P1Object != null)
			{
				playerLocation[0] = P1Object.position;
			}
			Transform P2Object = currentLoadedLevel.transform.Find(p2tankName);
			if (P2Object != null)
			{
				playerLocation[1] = P2Object.position;
			}
			else
			{
				playerLocation[1] = new Vector3(0f, 0f, 0f);
			}
			Transform P3Object = currentLoadedLevel.transform.Find(p3tankName);
			if (P3Object != null)
			{
				playerLocation[2] = P3Object.position;
			}
			else
			{
				playerLocation[2] = new Vector3(0f, 0f, 0f);
			}
			Transform P4Object = currentLoadedLevel.transform.Find(p4tankName);
			if (P4Object != null)
			{
				playerLocation[3] = P4Object.position;
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
		if (!Input.GetKeyDown(KeyCode.U) || Input.GetKey(KeyCode.LeftShift))
		{
		}
		if (inOnlineMode || (bool)CM)
		{
			return;
		}
		if (!inMenuMode)
		{
			if (numberOfControllers > 0)
			{
				int extra = ((!isPlayingWithController) ? 1 : 0);
				if (PlayerModeWithAI[1] == 1)
				{
					extra++;
				}
				if (PlayerModeWithAI[2] == 1)
				{
					extra++;
				}
				if (PlayerModeWithAI[3] == 1)
				{
					extra++;
				}
				int amount = numberOfControllers + extra;
				if (amount > 4)
				{
					amount = 4;
				}
				for (int i = 0; i < amount; i++)
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
							CameraFollowPlayer CFP = Camera.main.transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
							PlaneMaster PM = GetComponent<PlaneMaster>();
							if ((bool)PM)
							{
								PM.SpawnInOrder.Clear();
								PM.PS.isFlying = false;
								PM.PS.transform.position = Vector3.zero;
							}
							GameObject Level100 = GameObject.Find("LEVEL_100(Clone)");
							if ((bool)Level100)
							{
								MissionHundredController MHC = Level100.GetComponent<MissionHundredController>();
								if ((bool)MHC)
								{
									MHC.PlayerDied();
								}
							}
							CFP.enabled = true;
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
				NewOrchestra orchestra = GameObject.FindGameObjectWithTag("Orchestra").GetComponent<NewOrchestra>();
				if (orchestra != null)
				{
					if (orchestra.isPlaying && !GameHasStarted)
					{
						orchestra.StopPlaying();
					}
					else if (!orchestra.isPlaying && GameHasStarted)
					{
						orchestra.StartPlaying();
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
				int AmountTeamsAlive2 = 0;
				int[] TeamsThatAreAlive = new int[5];
				for (int l = 0; l < AmountTeamTanks.Length; l++)
				{
					if (AmountTeamTanks[l] > 0)
					{
						AmountTeamsAlive2++;
						TeamsThatAreAlive[l] = 1;
					}
					if (AmountTeamTanks[0] > 1)
					{
						AmountTeamsAlive2 = 900;
						TeamsThatAreAlive[l] = 1;
					}
				}
				int PlayersAlive = 0;
				for (int k = 0; k < PlayerJoined.Count; k++)
				{
					if (PlayerJoined[k])
					{
						PlayersAlive++;
						if (PlayerDied[k] && (AmountTeamTanks[PlayerTeamColor[k]] < 1 || PlayerTeamColor[k] == 0))
						{
							PlayersAlive--;
						}
					}
				}
				if (PlayersAlive < 1)
				{
					PlayerAlive = false;
					defeated();
					return;
				}
				if (AmountTeamsAlive2 < 2)
				{
					EndTheGame();
					return;
				}
			}
			if (!isZombieMode && inMapEditor && GameHasStarted && !isResettingTestLevel)
			{
				int AmountTeamsAlive = 0;
				for (int j = 0; j < AmountTeamTanks.Length; j++)
				{
					if (AmountTeamTanks[j] > 0)
					{
						AmountTeamsAlive++;
					}
					if (AmountTeamTanks[0] > 1)
					{
						AmountTeamsAlive = 900;
					}
				}
				if (AmountTeamsAlive < 2)
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
			if (!inMenuMode)
			{
				return;
			}
			GameObject[] enemieTanks = GameObject.FindGameObjectsWithTag("Enemy");
			if (enemieTanks.Length >= 1)
			{
				return;
			}
			Object.Destroy(currentLoadedLevel);
			GameObject[] Temps = GameObject.FindGameObjectsWithTag("Temp");
			GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
			GameObject[] Mines = GameObject.FindGameObjectsWithTag("Mine");
			GameObject[] all = Temps.Concat(Bullets).Concat(Mines).ToArray();
			GameObject[] array = all;
			foreach (GameObject Temp in array)
			{
				Object.Destroy(Temp);
			}
			int chosen = 0;
			int chosenLvl = 0;
			int missionReached = maxMissionReached;
			if (missionReached < maxMissionReachedHard)
			{
				missionReached = maxMissionReachedHard;
			}
			if (missionReached < maxMissionReachedKid)
			{
				missionReached = maxMissionReachedKid;
			}
			int calcMission = Mathf.CeilToInt((float)missionReached / 10f) * 10;
			int roundedMission = ((maxMissionReached < 1) ? 10 : calcMission);
			if (roundedMission < 1)
			{
				roundedMission = 0;
			}
			chosenLvl = Random.Range(0, Levels.Count);
			GameObject newLevel = Object.Instantiate(Levels[chosenLvl], lvlPos, Quaternion.identity);
			newLevel.SetActive(value: true);
			currentLoadedLevel = newLevel;
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject players in array2)
			{
				Object.Destroy(players.transform.parent.gameObject);
			}
			GameObject[] array3 = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject Enemy in array3)
			{
				EnemyTargetingSystemNew ETSN = Enemy.transform.parent.GetChild(1).GetChild(0).GetComponent<EnemyTargetingSystemNew>();
				if ((bool)ETSN)
				{
					ETSN.isHuntingEnemies = true;
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
		Vector3 PickedLocation = Vector3.zero;
		int SizeX = 0;
		int SizeY = 0;
		bool canTeleport = false;
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			SizeX = 14;
			SizeY = 11;
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			SizeX = 18;
			SizeY = 14;
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			SizeX = 21;
			SizeY = 16;
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			SizeX = 24;
			SizeY = 18;
		}
		if (MapCornerObject == null)
		{
			MapCornerObject = GameObject.Find("ALL_FIELDS");
		}
		for (int i = 0; i < 140; i++)
		{
			int RandomXPos = Random.Range(0, SizeX);
			int RandomYPos = Random.Range(0, SizeY);
			bool NextIteration = false;
			if (MapCornerObject == null)
			{
				Debug.Log("NO ALL FIELDS");
				canTeleport = false;
				break;
			}
			PickedLocation = MapCornerObject.transform.GetChild(0).position + new Vector3(RandomXPos * 2, 0f, -RandomYPos * 2);
			Collider[] objectsInRange;
			if (minimalDist > 9999f && !TargetPlayer)
			{
				objectsInRange = Physics.OverlapSphere(PickedLocation, 0.5f);
				Collider[] array = objectsInRange;
				foreach (Collider col3 in array)
				{
					if (col3.tag == "Solid")
					{
						NextIteration = true;
						break;
					}
				}
			}
			else if (!TargetPlayer)
			{
				objectsInRange = Physics.OverlapSphere(PickedLocation, 1f);
				Collider[] array2 = objectsInRange;
				foreach (Collider col2 in array2)
				{
					if (col2.tag == "Solid" || col2.tag == "MapBorder")
					{
						NextIteration = true;
						break;
					}
				}
			}
			int ChildPos = RandomYPos * (SizeX + 1) + RandomXPos;
			PathfindingBlock PB = MapCornerObject.transform.GetChild(ChildPos).GetComponent<PathfindingBlock>();
			if ((bool)PB && PB.SolidInMe)
			{
				NextIteration = true;
			}
			if (NextIteration)
			{
				continue;
			}
			objectsInRange = Physics.OverlapSphere(PickedLocation, 3f);
			Collider[] array3 = objectsInRange;
			foreach (Collider col in array3)
			{
				if (col.tag == "Player" && TargetPlayer)
				{
					NextIteration = false;
					canTeleport = true;
					break;
				}
				if (col.tag == "Player" || col.tag == "Boss" || col.tag == "Mine" || col.tag == "Bullet")
				{
					NextIteration = true;
					break;
				}
			}
			if (NextIteration)
			{
				continue;
			}
			if (!TargetPlayer)
			{
				if (!CheckForDist)
				{
					canTeleport = true;
					break;
				}
				float dist = Vector3.Distance(CallerPos, PickedLocation);
				if (!(dist < minimalDist))
				{
					canTeleport = true;
					break;
				}
				canTeleport = false;
			}
			else if (canTeleport)
			{
				break;
			}
		}
		if (canTeleport)
		{
			return PickedLocation;
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
		Vector3 PickedLocation = Vector3.zero;
		for (int i = 0; i < 100; i++)
		{
			bool NextIteration = false;
			Transform PickedTarget = targets[Random.Range(0, targets.Length)];
			PathfindingBlock PickedBlock = PickedTarget.GetComponent<DrawGizmos>().myPB;
			if (PickedTarget.GetComponent<DrawGizmos>().Picked)
			{
				NextIteration = true;
			}
			if ((bool)PickedBlock && PickedBlock.SolidInMe)
			{
				NextIteration = true;
			}
			if (!NextIteration && !NextIteration)
			{
				PickedLocation = PickedTarget.position;
				PickedTarget.GetComponent<DrawGizmos>().GotPicked();
				break;
			}
		}
		if (PickedLocation == Vector3.zero)
		{
			Debug.LogError("GOT A ZERO LOCATION ERROR!");
		}
		return PickedLocation;
	}

	public void LoadSurvivalMap()
	{
		Levels[OptionsMainMenu.instance.StartLevel].SetActive(value: true);
		Players.Clear();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject Player in array)
		{
			Players.Add(Player);
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
		bool GameFinished = false;
		OnlyCompanionLeft = false;
		if (CurrentMission == lastMissionNumber)
		{
			musicScript.StopMusic();
			GameFinished = true;
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
					GameFinished = true;
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
					int missionnumber = AchievementsTracker.instance.MissionUnlocked[i] + 1;
					if (maxMissionReached <= missionnumber && maxMissionReachedHard <= missionnumber && (maxMissionReached == missionnumber || maxMissionReachedHard == missionnumber) && !OptionsMainMenu.instance.MapEditorTankMessagesReceived[i])
					{
						OptionsMainMenu.instance.MapEditorTankMessagesReceived[i] = true;
						OptionsMainMenu.instance.SaveNewData();
						AchievementsTracker.instance.ShowUnlockMessage(AchievementsTracker.instance.MissionUnlocked[i], i);
					}
				}
			}
		}
		if (!GameFinished)
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
		float waittime = (fast ? 0.01f : 0.1f);
		yield return new WaitForSeconds(waittime);
		for (int i = 0; i < AmountTeamTanks.Length; i++)
		{
			AmountTeamTanks[i] = 0;
		}
		GameObject[] AllEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] array = AllEnemies;
		foreach (GameObject Enemy in array)
		{
			EnemyAI EA = Enemy.GetComponent<EnemyAI>();
			if ((bool)EA)
			{
				AmountTeamTanks[EA.MyTeam]++;
			}
		}
		GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = AllPlayers;
		foreach (GameObject Player in array2)
		{
			MoveTankScript MTS = Player.GetComponent<MoveTankScript>();
			if ((bool)MTS)
			{
				AmountTeamTanks[MTS.MyTeam]++;
				PlayerTeamColor[MTS.playerId] = MTS.MyTeam;
				continue;
			}
			EnemyAI EA2 = Player.GetComponent<EnemyAI>();
			if ((bool)EA2)
			{
				AmountTeamTanks[EA2.MyTeam]++;
				PlayerTeamColor[1] = EA2.MyTeam;
			}
		}
		if (!(MapEditorMaster.instance != null))
		{
			yield break;
		}
		AmountEnemyTanks = 0;
		for (int j = 0; j < AmountTeamTanks.Length; j++)
		{
			if (j != PlayerTeamColor[0] || PlayerTeamColor[0] == 0)
			{
				AmountEnemyTanks += AmountTeamTanks[j];
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
			Object.Destroy(currentLoadedLevel);
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
			foreach (GameObject TrainLevelBlock in trainLevelBlocks)
			{
				TrainLevelBlock.SetActive(value: true);
				if (TrainLevelBlock.transform.tag == "Snow" && !OptionsMainMenu.instance.SnowMode)
				{
					TrainLevelBlock.SetActive(value: false);
				}
			}
		}
		bool makeNight = false;
		foreach (int NL in NightLevels)
		{
			if (CurrentMission == NL)
			{
				makeNight = true;
				CloudGeneration.instance.MakeItDark();
				break;
			}
		}
		if ((bool)MapEditorMaster.instance)
		{
			CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(MapEditorMaster.instance.WeatherTypes[CurrentMission], force: false));
		}
		if (!makeNight)
		{
			CloudGeneration.instance.MakeItDay();
		}
		foreach (int TL in TrainLevels)
		{
			if (CurrentMission == TL)
			{
				GameObject[] trainLevelBlocks2 = TrainLevelBlocks;
				foreach (GameObject TrainLevelBlock2 in trainLevelBlocks2)
				{
					TrainLevelBlock2.SetActive(value: false);
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
			BinaryFormatter formatter = new BinaryFormatter();
			SingleMapEditorData data = formatter.Deserialize(stream) as SingleMapEditorData;
			stream.Close();
			currentLoadedLevel = GetComponent<LoadCampaignMap>().LoadMap(data, CurrentMission);
			if ((bool)currentLoadedLevel)
			{
				SetPlayerPosition("Main_Tank_FBX(Clone)", "Second_Tank_FBX(Clone)", "Third_Tank_FBX(Clone)", "Fourth_Tank_FBX(Clone)");
			}
		}
		else if (!Levels[CurrentMission].name.Contains("Custom"))
		{
			currentLoadedLevel = Object.Instantiate(Levels[CurrentMission], new Vector3(-9f, 0f, 1f), Quaternion.identity);
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
				Object.Destroy(Levels[CurrentMission - 1]);
			}
			Levels[CurrentMission].SetActive(value: true);
			currentLoadedLevel = Levels[CurrentMission];
			if ((bool)currentLoadedLevel)
			{
				SetPlayerPosition("Main_Tank_Prop(Clone)", "Second_Tank_Prop(Clone)", "Third_Tank_Prop(Clone)", "Fourth_Tank_Prop(Clone)");
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
			CameraFollowPlayer CFP = Camera.main.transform.parent.GetComponent<CameraFollowPlayer>();
			if ((bool)CFP)
			{
				CFP.enabled = true;
			}
		}
		if (MapEditorMaster.instance != null)
		{
			StartCoroutine(DisableGameDelay());
			if (MapEditorMaster.instance.MissionFloorTextures.Length != 0)
			{
				LoadModTexture LMT = instance.floor.GetComponent<LoadModTexture>();
				if ((bool)LMT && LMT.IsModded)
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
		foreach (GameObject Player in Players)
		{
			Debug.Log("Current player before change:" + Player.name);
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
		Vector3 newPos = Players[playerIndex].transform.position;
		Quaternion theRot = Players[playerIndex].transform.rotation;
		GameObject AIplayer = Object.Instantiate(playerType switch
		{
			2 => AIPlayer3Prefab, 
			1 => AIPlayer2Prefab, 
			_ => AIPlayer4Prefab, 
		}, newPos, theRot, Players[playerIndex].transform.parent.transform.parent);
		if ((bool)MapEditorMaster.instance)
		{
			MapEditorGridPiece HisMEGP = Players[playerIndex].GetComponent<MapEditorProp>().myMEGP;
			MapEditorProp MEP = AIplayer.transform.GetChild(0).gameObject.AddComponent<MapEditorProp>();
			EnemyAI EA = AIplayer.transform.GetChild(0).gameObject.GetComponent<EnemyAI>();
			EA.MyTeam = Players[playerIndex].GetComponent<MapEditorProp>().TeamNumber;
			MEP.TeamNumber = Players[playerIndex].GetComponent<MapEditorProp>().TeamNumber;
			if (playerType == 1)
			{
				MEP.isPlayerTwo = true;
			}
			if (playerType == 2)
			{
				MEP.isPlayerThree = true;
			}
			if (playerType == 3)
			{
				MEP.isPlayerFour = true;
			}
			Renderer[] array = (MEP.myRends = new MeshRenderer[1]);
			MEP.myRends[0] = MEP.transform.Find("Cube.003").GetComponent<MeshRenderer>();
			MEP.OriginalBodyColor = MEP.myRends[0].materials[0].color;
			MEP.TeamNumber = HisMEGP.MyTeamNumber;
		}
		Debug.Log("Destroying player: " + Players[playerIndex].transform.parent.gameObject.name);
		Object.Destroy(Players[playerIndex].transform.parent.gameObject);
		if (CurrentMission == 99 && !MapEditorMaster.instance)
		{
			Object.Destroy(AIplayer);
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
		foreach (int NL in NightLevels)
		{
			if (CurrentMission == NL)
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
		AudioSource mySrc = Camera.main.GetComponent<AudioSource>();
		mySrc.volume = 0f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		AmountGoodTanks = PlayerJoined.Count;
		SetPlayerPosition("Main_Tank_Prop(Clone)", "Second_Tank_Prop(Clone)", "Third_Tank_Prop(Clone)", "Fourth_Tank_Prop(Clone)");
		UpdatePlayerToAI();
		FindPlayers();
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		foreach (GameObject Enemy in Enemies)
		{
			Debug.Log("ENEMY FOUND");
			MapEditorProp MEP = Enemy.GetComponent<MapEditorProp>();
			if ((bool)MEP)
			{
				Debug.Log("Merp");
				if (MEP.MyDifficultySpawn > OptionsMainMenu.instance.currentDifficulty)
				{
					Debug.Log("Bye bye");
					Enemy.transform.parent.transform.gameObject.SetActive(value: false);
				}
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
		GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");
		GameObject[] array = mines;
		foreach (GameObject mine in array)
		{
			Object.Destroy(mine);
		}
		AudioSource mySrc = Camera.main.GetComponent<AudioSource>();
		mySrc.volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		MapEditorMaster.instance.isTesting = false;
		Animator camAnimator = Camera.main.GetComponent<Animator>();
		CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(0, force: true));
		camAnimator.SetBool("CameraUp", value: true);
		camAnimator.SetBool("CameraDownEditor", value: false);
		camAnimator.SetBool("CameraDown", value: false);
		GetComponent<AudioSource>().Play();
		MapEditorMaster.instance.EditingCanvas.SetActive(value: true);
		GameHasStarted = false;
		AmountPlayersThatNeedRevive = 0;
		MapEditorMaster.instance.ErrorField.transform.localPosition = MapEditorMaster.instance.AnimatorStartHeight;
		for (int j = 0; j < MapEditorMaster.instance.ColorsTeamsPlaced.Length; j++)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[j] = 0;
		}
		MapEditorMaster.instance.enemyTanksPlaced[CurrentMission] = 0;
		MapEditorMaster.instance.playerOnePlaced[CurrentMission] = 0;
		MapEditorMaster.instance.playerTwoPlaced[CurrentMission] = 0;
		MapEditorMaster.instance.canPlaceProp = false;
		MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlacePropTimer(2f));
		GameObject[] MapeditorGridPieces = GameObject.FindGameObjectsWithTag("MapeditorField");
		GameObject[] array2 = MapeditorGridPieces;
		foreach (GameObject GridPiece in array2)
		{
			MapEditorGridPiece MEGP = GridPiece.GetComponent<MapEditorGridPiece>();
			for (int k = 0; k < 5; k++)
			{
				if (MEGP.propOnMe[k])
				{
					int TeamNumber = -1;
					if (MEGP.MyTeamNumber > -1)
					{
						TeamNumber = MEGP.MyTeamNumber;
					}
					Object.Destroy(MEGP.myProp[k]);
					MEGP.propOnMe[k] = false;
					MEGP.SpawnInProps(MEGP.myPropID[k], MEGP.rotationDirection[k], TeamNumber, k, MEGP.SpawnDifficulty);
					if (MEGP.myPropID[k] > 5 && MEGP.myPropID[k] < 40)
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
			DestroyableWall DW = searchPlace.transform.GetChild(i).GetComponent<DestroyableWall>();
			if (DW != null)
			{
				if (!DW.isHalfSlab)
				{
					BreakableBlocksLocations.Add(searchPlace.transform.GetChild(i).transform.position);
				}
				else if (DW.IsStone)
				{
					StoneBlocksLocations.Add(searchPlace.transform.GetChild(i).transform.position);
				}
				else
				{
					BreakableHalfBlocksLocations.Add(searchPlace.transform.GetChild(i).transform.position);
				}
			}
			ExplosiveBlock EB = searchPlace.transform.GetChild(i).GetComponent<ExplosiveBlock>();
			if ((bool)EB)
			{
				TNTLocations.Add(searchPlace.transform.GetChild(i).transform.position);
			}
			GetBreakableBlocksLocations(searchPlace.transform.GetChild(i).gameObject);
		}
	}

	public void DestroyTemps()
	{
		GameObject[] others = GameObject.FindGameObjectsWithTag("Temp");
		GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");
		GameObject[] all = others.Concat(mines).ToArray();
		for (int i = 0; i < all.Length; i++)
		{
			if (all[i].transform.parent == null)
			{
				Object.Destroy(all[i]);
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
		GameObject[] all = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		List<GameObject> EnemiesToRemove = new List<GameObject>();
		foreach (GameObject Enemy in Enemies)
		{
			if ((bool)Enemy)
			{
				EnemyAI EA = Enemy.GetComponent<EnemyAI>();
				if (EA.MyTeam != 2 && Enemies.Contains(EA.gameObject) && MapEditorMaster.instance == null)
				{
					EnemiesToRemove.Add(EA.gameObject);
				}
			}
		}
		foreach (GameObject toremove in EnemiesToRemove)
		{
			Enemies.Remove(toremove);
		}
		for (int i = Enemies.Count - 1; i >= 0; i--)
		{
			if (Enemies[i] == null)
			{
				Enemies.RemoveAt(i);
			}
		}
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
			ProgressDataNew data = SavingData.LoadData();
			maxMissionReached = data.cM;
			maxMissionReachedHard = data.cH;
			maxMissionReachedKid = data.cK;
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
		for (int j = 0; j < highestWaves.Length; j++)
		{
			highestWaves[j] = 0;
		}
		for (int i = 0; i < TankColorKilled.Count; i++)
		{
			TankColorKilled[i] = 0;
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
			GameObject poof = Object.Instantiate(SpawningPlayerParticles, PlayerPosition, Quaternion.identity, currentLoadedLevel.transform);
			ParticleSystem poofie = poof.GetComponent<ParticleSystem>();
			Play2DClipOnce PCO = poof.GetComponent<Play2DClipOnce>();
			PCO.overrideGameStarted = true;
			poofie.Play();
			poof.transform.Rotate(new Vector3(-90f, 0f, 0f));
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
		for (int j = 0; j < PlayerDown.Count; j++)
		{
			PlayerDown[j] = false;
		}
		if (PlayerJoined.Count > 1 && !MapEditorMaster.instance)
		{
			Debug.LogWarning("Reset players" + PlayerJoined.Count);
			for (int k = 0; k < PlayerJoined.Count; k++)
			{
				if ((!PlayerJoined[k] && PlayerModeWithAI[k] == 0) || (CurrentMission == 99 && PlayerModeWithAI[k] == 1 && MapEditorMaster.instance == null))
				{
					continue;
				}
				switch (k)
				{
				case 0:
					Debug.LogWarning(" players 1");
					if (currentLoadedLevel.transform.Find("Main_Tank_FBX") == null)
					{
						Debug.LogWarning(" players 1 spawned");
						GameObject newPlayer = Object.Instantiate(PlayerPrefab, playerLocation[0], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
						newPlayer.transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[0];
						Mission99SpawnPoof(playerLocation[0]);
					}
					else
					{
						Debug.LogWarning(" players 1 already here", Levels[CurrentMission].transform.Find("Main_Tank_FBX").gameObject);
					}
					continue;
				case 1:
					if (PlayerModeWithAI[k] == 1)
					{
						if (!currentLoadedLevel.transform.Find("AI_Tank_FBX"))
						{
							GameObject newPlayer2 = Object.Instantiate(AIPlayer2Prefab, playerLocation[1], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
							newPlayer2.transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[1];
							Mission99SpawnPoof(playerLocation[1]);
						}
					}
					else if (!currentLoadedLevel.transform.Find("Second_Tank_FBX"))
					{
						GameObject newPlayer3 = Object.Instantiate(Player2Prefab, playerLocation[1], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
						newPlayer3.transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[1];
						Mission99SpawnPoof(playerLocation[1]);
					}
					continue;
				case 2:
					if (PlayerModeWithAI[k] == 1)
					{
						if (!currentLoadedLevel.transform.Find("AI_Tank_FBX_third"))
						{
							GameObject newPlayer4 = Object.Instantiate(AIPlayer3Prefab, playerLocation[2], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
							newPlayer4.transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[2];
							Mission99SpawnPoof(playerLocation[2]);
						}
					}
					else if (!currentLoadedLevel.transform.Find("Third_Tank_FBX"))
					{
						GameObject newPlayer6 = Object.Instantiate(Player3Prefab, playerLocation[2], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
						newPlayer6.transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[2];
						Mission99SpawnPoof(playerLocation[2]);
					}
					continue;
				}
				if (PlayerModeWithAI[k] == 1)
				{
					if (!currentLoadedLevel.transform.Find("AI_Tank_FBX_fourth"))
					{
						GameObject newPlayer5 = Object.Instantiate(AIPlayer4Prefab, playerLocation[3], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
						newPlayer5.transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[3];
						Mission99SpawnPoof(playerLocation[3]);
					}
				}
				else if (!currentLoadedLevel.transform.Find("Fourth_Tank_FBX"))
				{
					GameObject newPlayer7 = Object.Instantiate(Player4Prefab, playerLocation[3], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
					newPlayer7.transform.parent = currentLoadedLevel.transform;
					newPlayer7.transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[3];
					Mission99SpawnPoof(playerLocation[3]);
				}
			}
		}
		else if ((bool)MapEditorMaster.instance && (bool)MapEditorMaster.instance)
		{
			Debug.Log("RESETTING MAP PIECES THINGIES");
			GameObject[] MapeditorGridPieces = GameObject.FindGameObjectsWithTag("MapeditorField");
			GameObject[] array = MapeditorGridPieces;
			foreach (GameObject GridPiece in array)
			{
				MapEditorGridPiece MEGP = GridPiece.GetComponent<MapEditorGridPiece>();
				for (int j2 = 0; j2 < 5; j2++)
				{
					if (MEGP.propOnMe[j2] && MEGP.mission == CurrentMission && (MEGP.myPropID[j2] == 4 || MEGP.myPropID[j2] == 5 || MEGP.myPropID[j2] == 28 || MEGP.myPropID[j2] == 29))
					{
						if ((MEGP.myPropID[j2] != 5 || PlayerTeamColor[0] == PlayerTeamColor[1]) && (MEGP.myPropID[j2] != 28 || PlayerTeamColor[0] == PlayerTeamColor[2]) && (MEGP.myPropID[j2] != 29 || PlayerTeamColor[0] == PlayerTeamColor[3]))
						{
							int TeamNumber = -1;
							if (MEGP.MyTeamNumber > -1)
							{
								TeamNumber = MEGP.MyTeamNumber;
							}
							Object.Destroy(MEGP.myProp[j2]);
							MEGP.propOnMe[j2] = false;
							MEGP.SpawnInProps(MEGP.myPropID[j2], MEGP.rotationDirection[j2], TeamNumber, j2, MEGP.SpawnDifficulty);
						}
					}
					else if (MEGP.propOnMe[j2] && MEGP.mission == CurrentMission && MEGP.myProp[j2] == null && (MEGP.myPropID[j2] == 40 || MEGP.myPropID[j2] == 45 || MEGP.myPropID[j2] == 2 || MEGP.myPropID[j2] == 49))
					{
						MEGP.propOnMe[j2] = false;
						MEGP.SpawnInProps(MEGP.myPropID[j2], MEGP.rotationDirection[j2], 0, j2, MEGP.SpawnDifficulty);
					}
				}
			}
		}
		UpdatePlayerToAI();
		if (!MapEditorMaster.instance)
		{
			for (int i2 = 0; i2 < BreakableBlocksLocations.Count; i2++)
			{
				int foundone = 0;
				Collider[] hitColliders = Physics.OverlapSphere(BreakableBlocksLocations[i2], 0.2f);
				Collider[] array2 = hitColliders;
				foreach (Collider hitCollider in array2)
				{
					if (hitCollider.tag == "Solid" && hitCollider.GetComponent<DestroyableWall>() != null)
					{
						foundone = 1;
					}
				}
				if (foundone == 0)
				{
					GameObject NewCork = Object.Instantiate(CorkBlock, BreakableBlocksLocations[i2], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
					NewCork.transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int n = 0; n < StoneBlocksLocations.Count; n++)
			{
				int foundone2 = 0;
				Collider[] hitColliders2 = Physics.OverlapSphere(StoneBlocksLocations[n], 0.2f);
				Collider[] array3 = hitColliders2;
				foreach (Collider hitCollider2 in array3)
				{
					if (hitCollider2.tag == "Solid" && hitCollider2.GetComponent<DestroyableWall>() != null)
					{
						foundone2 = 1;
					}
				}
				if (foundone2 == 0)
				{
					GameObject NewStone = Object.Instantiate(StoneBlock, StoneBlocksLocations[n], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
					NewStone.transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int m = 0; m < TNTLocations.Count; m++)
			{
				int foundone3 = 0;
				Collider[] hitColliders3 = Physics.OverlapSphere(TNTLocations[m], 0.2f);
				Collider[] array4 = hitColliders3;
				foreach (Collider hitCollider3 in array4)
				{
					if (hitCollider3.tag == "Solid" && hitCollider3.GetComponent<ExplosiveBlock>() != null)
					{
						foundone3 = 1;
					}
				}
				if (foundone3 == 0)
				{
					GameObject NewTNT = Object.Instantiate(TNTBlock, TNTLocations[m], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
					NewTNT.transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int l = 0; l < BreakableHalfBlocksLocations.Count; l++)
			{
				int foundone4 = 0;
				Collider[] hitColliders4 = Physics.OverlapSphere(BreakableHalfBlocksLocations[l], 0.2f);
				Collider[] array5 = hitColliders4;
				foreach (Collider hitCollider4 in array5)
				{
					if (hitCollider4.tag == "Solid" && hitCollider4.GetComponent<DestroyableWall>() != null)
					{
						foundone4 = 1;
					}
				}
				if (foundone4 == 0)
				{
					GameObject NewCork2 = Object.Instantiate(HalfCorkBlock, BreakableHalfBlocksLocations[l], Quaternion.Euler(new Vector3(0f, 0f, 0f)));
					NewCork2.transform.parent = currentLoadedLevel.transform;
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
				List<GameObject> EnemiesToRemove = new List<GameObject>();
				foreach (GameObject Enemy in Enemies)
				{
					if (!Enemy)
					{
						continue;
					}
					EnemyAI EA = Enemy.GetComponent<EnemyAI>();
					if ((bool)EA)
					{
						if (EA.MyTeam != 2 && Enemies.Contains(EA.gameObject) && MapEditorMaster.instance == null)
						{
							EnemiesToRemove.Add(EA.gameObject);
						}
						MoveTankScript MTS = Players[0].GetComponent<MoveTankScript>();
						if ((bool)MTS && (EA.MyTeam != MTS.MyTeam || MTS.MyTeam == 0))
						{
							AmountEnemyTanks++;
						}
					}
				}
				foreach (GameObject toremove in EnemiesToRemove)
				{
					Enemies.Remove(toremove);
				}
				for (int i = Enemies.Count - 1; i >= 0; i--)
				{
					if (Enemies[i] == null)
					{
						Enemies.RemoveAt(i);
					}
				}
			}
		}
		else
		{
			AmountEnemyTanks = Enemies.Concat(Bosses).ToArray().Length;
		}
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] all = Enemies.Concat(Bosses).Concat(EnemyScripts).ToArray();
		GameObject[] array6 = all;
		foreach (GameObject enemy in array6)
		{
			MonoBehaviour[] enemyscripts = enemy.GetComponents<MonoBehaviour>();
			MonoBehaviour[] array7 = enemyscripts;
			foreach (MonoBehaviour script in array7)
			{
				script.enabled = true;
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
		foreach (GameObject Player in array)
		{
			Players.Add(Player);
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
		List<GameObject> EnemiesToRemove = new List<GameObject>();
		foreach (GameObject Enemy in Enemies)
		{
			if ((bool)Enemy)
			{
				EnemyAI EA = Enemy.GetComponent<EnemyAI>();
				if (EA.MyTeam != 2 && Enemies.Contains(EA.gameObject) && MapEditorMaster.instance == null)
				{
					EnemiesToRemove.Add(EA.gameObject);
				}
			}
		}
		foreach (GameObject toremove in EnemiesToRemove)
		{
			Enemies.Remove(toremove);
		}
		for (int i = Enemies.Count - 1; i >= 0; i--)
		{
			if (Enemies[i] == null)
			{
				Enemies.RemoveAt(i);
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
		foreach (GameObject Player in array)
		{
			Players.Add(Player);
		}
		if (Enemies != null)
		{
			Enemies.Clear();
		}
		Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] all = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
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
		GameObject[] array2 = all;
		foreach (GameObject enemy in array2)
		{
			MonoBehaviour[] enemyscripts = enemy.GetComponents<MonoBehaviour>();
			EnemyTargetingSystem[] targetscripts = enemy.GetComponents<EnemyTargetingSystem>();
			MonoBehaviour[] array3 = enemyscripts;
			foreach (MonoBehaviour script in array3)
			{
				script.enabled = true;
			}
			AudioSource[] sources2 = enemy.GetComponents<AudioSource>();
			AudioSource[] array4 = sources2;
			foreach (AudioSource source in array4)
			{
				source.enabled = true;
			}
			EnemyTargetingSystem[] array5 = targetscripts;
			foreach (EnemyTargetingSystem script2 in array5)
			{
			}
		}
		List<GameObject> EnemiesToRemove = new List<GameObject>();
		foreach (GameObject Enemy in Enemies)
		{
			if ((bool)Enemy)
			{
				EnemyAI EA = Enemy.GetComponent<EnemyAI>();
				if (EA.MyTeam != 2 && Enemies.Contains(EA.gameObject) && MapEditorMaster.instance == null)
				{
					EnemiesToRemove.Add(EA.gameObject);
				}
			}
		}
		foreach (GameObject toremove in EnemiesToRemove)
		{
			Enemies.Remove(toremove);
		}
		for (int i = Enemies.Count - 1; i >= 0; i--)
		{
			if (Enemies[i] == null)
			{
				Enemies.RemoveAt(i);
			}
		}
		all = Enemies.Concat(Bosses).ToArray();
		AmountEnemyTanks = all.Length;
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
			ZombieTankSpawner ZTS = GetComponent<ZombieTankSpawner>();
			ZTS.Wave++;
			AccountMaster.instance.UpdateServerStatus(ZTS.Wave + 100);
			GameObject[] sun2 = Sun;
			foreach (GameObject sun in sun2)
			{
				sun.SetActive(value: true);
			}
			ZTS.amountEnemies = 0;
			ZTS.spawned = 0;
			ZTS.spawnAmount = Mathf.RoundToInt((float)(ZTS.Wave * 2 + Mathf.RoundToInt(Random.Range(-(ZTS.Wave / 2), ZTS.Wave / 2))) * ZTS.multiplier);
			if (ZTS.spawnAmount < 1)
			{
				ZTS.spawnAmount = 2;
			}
			GameHasStarted = true;
			Players.Clear();
			AccountMaster.instance.SaveCloudData(6, ZTS.Wave, 0, bounceKill: false);
			GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject Player in array)
			{
				Players.Add(Player);
			}
		}
	}
}
