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

	public int[] AmountTeamTanks;

	public ProgressDataOnline CurrentData;

	[HideInInspector]
	public int AmountCalledInTanks;

	public List<int> Playerkills = new List<int>();

	public int TotalKillsThisSession;

	public int[] TankColorKilled = new int[20];

	public int totalKills;

	public int totalKillsBounce;

	public int totalRevivesPerformed;

	public int totalDefeats;

	public int totalWins;

	public int maxMissionReached;

	public int maxMissionReachedKid;

	public int maxMissionReachedHard;

	[HideInInspector]
	public bool HasGotten100Checkpoint;

	[Header("UI")]
	public TextMeshProUGUI Mission_Text;

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

	public GameObject[] Enemies;

	public GameObject[] Bosses;

	public List<string> MissionNames = new List<string>();

	public Object[] SMED;

	public List<GameObject> Levels = new List<GameObject>();

	public List<TextAsset> LevelsText = new List<TextAsset>();

	public List<int> NightLevels = new List<int>();

	public List<int> TrainLevels = new List<int>();

	public List<int> FoundSecretMissions = new List<int>();

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
				Mission_Text.text = "Mission " + (CurrentMission + 1);
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
			foreach (GameObject player in Players)
			{
				if (player == null)
				{
					Players.Remove(player);
				}
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
				Mission_Text.text = "Mission " + (CurrentMission + 1);
				Tanks_Text.text = "x " + AmountEnemyTanks;
				if (TanksLeft_Text != null)
				{
					TanksLeft_Text.text = "Tanks left: " + Lives;
				}
			}
			else if (isZombieMode)
			{
				Mission_Text.text = "Wave " + ZombieTankSpawner.instance.Wave;
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
				if (CurrentMission != 99)
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
		else if (inMenuMode && GameObject.FindGameObjectsWithTag("Enemy").Length < 1)
		{
			Object.Destroy(currentLoadedLevel);
			GameObject[] first = GameObject.FindGameObjectsWithTag("Temp");
			GameObject[] second = GameObject.FindGameObjectsWithTag("Bullet");
			GameObject[] array2 = Enumerable.Concat(second: GameObject.FindGameObjectsWithTag("Mine"), first: first.Concat(second)).ToArray();
			for (int m = 0; m < array2.Length; m++)
			{
				Object.Destroy(array2[m]);
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
			num6 = Random.Range(0, Levels.Count);
			GameObject gameObject2 = Object.Instantiate(Levels[num6], lvlPos, Quaternion.identity);
			gameObject2.SetActive(value: true);
			currentLoadedLevel = gameObject2;
			array2 = GameObject.FindGameObjectsWithTag("Player");
			for (int m = 0; m < array2.Length; m++)
			{
				Object.Destroy(array2[m].transform.parent.gameObject);
			}
			array2 = GameObject.FindGameObjectsWithTag("Enemy");
			for (int m = 0; m < array2.Length; m++)
			{
				array2[m].transform.parent.GetChild(1).GetChild(0).GetComponent<EnemyTargetingSystemNew>()
					.isHuntingEnemies = true;
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

	public Vector3 GetValidLocation(bool CheckForDist, float minimalDist, Vector3 CallerPos)
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
		for (int i = 0; i < 100; i++)
		{
			int num2 = Random.Range(0, num);
			int num3 = Random.Range(0, maxExclusive);
			bool flag2 = false;
			if (MapCornerObject == null)
			{
				Debug.Log("NO ALL FIELDS");
				flag = false;
				break;
			}
			vector = MapCornerObject.transform.GetChild(0).position + new Vector3(num2 * 2, 0f, -num3 * 2);
			Collider[] array;
			if (minimalDist > 9999f)
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
			else
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
				if (collider2.tag == "Player" || collider2.tag == "Boss" || collider2.tag == "Mine" || collider2.tag == "Bullet")
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
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
		}
		if (flag)
		{
			return vector;
		}
		Debug.Log("FOUND NO SPOT");
		return Vector3.zero;
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
				if (OptionsMainMenu.instance.StartLevel < CurrentMission - 9)
				{
					switch (CurrentMission)
					{
					case 10:
						IncreaseMarbles(CalcMarbles(5));
						break;
					case 20:
						IncreaseMarbles(CalcMarbles(7));
						break;
					case 30:
						IncreaseMarbles(CalcMarbles(10));
						break;
					case 40:
						IncreaseMarbles(CalcMarbles(7));
						break;
					case 50:
						IncreaseMarbles(CalcMarbles(15));
						break;
					case 60:
						IncreaseMarbles(CalcMarbles(9));
						break;
					case 70:
						IncreaseMarbles(CalcMarbles(12));
						break;
					case 80:
						IncreaseMarbles(CalcMarbles(10));
						break;
					case 90:
						IncreaseMarbles(CalcMarbles(20));
						break;
					case 100:
						IncreaseMarbles(CalcMarbles(25));
						break;
					}
				}
			}
			NAS.NextRound();
		}
		else
		{
			NAS.FinishGame();
		}
	}

	private int CalcMarbles(int amount)
	{
		return amount + amount * OptionsMainMenu.instance.currentDifficulty;
	}

	public void IncreaseMarbles(int amount)
	{
		if (AccountMaster.instance.isSignedIn)
		{
			AccountMaster.instance.PDO.marbles += amount;
			AccountMaster.instance.SaveCloudData(5, amount, 0, bounceKill: false);
			AccountMaster.instance.ShowMarbleNotification(amount);
		}
	}

	public void AddBonusTank()
	{
		Lives++;
		Play2DClipAtPoint(ExtraLiveSound, 1f);
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
		for (int i = 0; i < AmountTeamTanks.Length; i++)
		{
			AmountTeamTanks[i] = 0;
		}
		EnemyTankTracksAudio = 0;
		if ((bool)currentLoadedLevel && !MapEditorMaster.instance)
		{
			currentLoadedLevel.SetActive(value: false);
			Object.Destroy(currentLoadedLevel);
		}
		levelIsLoaded = false;
		if (CurrentMission == 49)
		{
			if (!MapEditorMaster.instance)
			{
				mapBorders.SetActive(value: false);
			}
			if (floor != null)
			{
				floor.SetActive(value: false);
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
				for (int j = 0; j < trainLevelBlocks.Length; j++)
				{
					trainLevelBlocks[j].SetActive(value: false);
				}
				break;
			}
		}
		if (CurrentMission == 49)
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
		AmountEnemyTanks = GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("Boss").Length;
		levelIsLoaded = true;
		BreakableBlocksLocations.Clear();
		StoneBlocksLocations.Clear();
		TNTLocations.Clear();
		BreakableHalfBlocksLocations.Clear();
		GetBreakableBlocksLocations(currentLoadedLevel);
		Debug.Log("ja lets go enxt LEVEL");
		if (CurrentMission == 99)
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
		GameObject gameObject = Object.Instantiate(playerType switch
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
			mapEditorProp.myRend = mapEditorProp.transform.Find("Cube.003").GetComponent<MeshRenderer>();
			mapEditorProp.OriginalBodyColor = mapEditorProp.myRend.materials[0].color;
			mapEditorProp.TeamNumber = myMEGP.MyTeamNumber;
		}
		Debug.Log("Destroying player: " + Players[playerIndex].transform.parent.gameObject.name);
		Object.Destroy(Players[playerIndex].transform.parent.gameObject);
		if (CurrentMission == 99 && !MapEditorMaster.instance)
		{
			Object.Destroy(gameObject);
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
		GameObject[] array = GameObject.FindGameObjectsWithTag("Mine");
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
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
		for (int j = 0; j < MapEditorMaster.instance.ColorsTeamsPlaced.Length; j++)
		{
			MapEditorMaster.instance.ColorsTeamsPlaced[j] = 0;
		}
		MapEditorMaster.instance.enemyTanksPlaced[CurrentMission] = 0;
		MapEditorMaster.instance.playerOnePlaced[CurrentMission] = 0;
		MapEditorMaster.instance.playerTwoPlaced[CurrentMission] = 0;
		MapEditorMaster.instance.canPlaceProp = false;
		MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlacePropTimer(2f));
		array = GameObject.FindGameObjectsWithTag("MapeditorField");
		for (int i = 0; i < array.Length; i++)
		{
			MapEditorGridPiece component2 = array[i].GetComponent<MapEditorGridPiece>();
			for (int k = 0; k < 5; k++)
			{
				if (component2.propOnMe[k])
				{
					int team = -1;
					if (component2.MyTeamNumber > -1)
					{
						team = component2.MyTeamNumber;
					}
					Object.Destroy(component2.myProp[k]);
					component2.propOnMe[k] = false;
					component2.SpawnInProps(component2.myPropID[k], component2.rotationDirection[k], team, k, component2.SpawnDifficulty);
					if (component2.myPropID[k] > 5 && component2.myPropID[k] < 40)
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
				Object.Destroy(array[i]);
			}
		}
	}

	public void DisableGame()
	{
		Debug.LogWarning("Game disbaled.");
		GameHasStarted = false;
		restartGame = false;
		FindPlayers();
		Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] array = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		GameObject[] enemies = Enemies;
		for (int i = 0; i < enemies.Length; i++)
		{
			EnemyAI component = enemies[i].GetComponent<EnemyAI>();
			if ((bool)component && (bool)component.ETSN)
			{
				component.ETSN.enabled = false;
			}
		}
		enemies = array;
		foreach (GameObject gameObject in enemies)
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
		for (int j = 0; j < TankColorKilled.Length; j++)
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
		Play2DClipAtPoint(LostLiveSound, 1f);
	}

	public void ResetLevel()
	{
		Debug.LogWarning("Reset level");
		AmountCalledInTanks = 0;
		HandlePlayerQueue();
		DestroyTemps();
		AmountPlayersThatNeedRevive = 0;
		GameObject[] array;
		if (PlayerJoined.Count > 1 && CurrentMission < 99 && !MapEditorMaster.instance)
		{
			Debug.LogWarning("Reset players" + PlayerJoined.Count);
			for (int i = 0; i < PlayerJoined.Count; i++)
			{
				switch (i)
				{
				case 0:
					Debug.LogWarning(" players 1");
					if (currentLoadedLevel.transform.Find("Main_Tank_FBX") == null)
					{
						Debug.LogWarning(" players 1 spawned");
						Object.Instantiate(PlayerPrefab, playerLocation[0], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[0];
					}
					else
					{
						Debug.LogWarning(" players 1 already here", Levels[CurrentMission].transform.Find("Main_Tank_FBX").gameObject);
					}
					continue;
				case 1:
					if (PlayerModeWithAI[i] == 1)
					{
						if (!currentLoadedLevel.transform.Find("AI_Tank_FBX"))
						{
							Object.Instantiate(AIPlayer2Prefab, playerLocation[1], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[1];
						}
					}
					else if (!currentLoadedLevel.transform.Find("Second_Tank_FBX"))
					{
						Object.Instantiate(Player2Prefab, playerLocation[1], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[1];
					}
					continue;
				case 2:
					if (PlayerModeWithAI[i] == 1)
					{
						if (!currentLoadedLevel.transform.Find("AI_Tank_FBX_third"))
						{
							Object.Instantiate(AIPlayer3Prefab, playerLocation[2], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[2];
						}
					}
					else if (!currentLoadedLevel.transform.Find("Third_Tank_FBX"))
					{
						Object.Instantiate(Player3Prefab, playerLocation[2], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[2];
					}
					continue;
				}
				if (PlayerModeWithAI[i] == 1)
				{
					if (!currentLoadedLevel.transform.Find("AI_Tank_FBX_fourth"))
					{
						Object.Instantiate(AIPlayer4Prefab, playerLocation[3], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform).transform.GetChild(0).GetComponent<EnemyAI>().MyTeam = PlayerTeamColor[3];
					}
				}
				else if (!currentLoadedLevel.transform.Find("Fourth_Tank_FBX"))
				{
					GameObject obj = Object.Instantiate(Player4Prefab, playerLocation[3], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
					obj.transform.parent = currentLoadedLevel.transform;
					obj.transform.GetChild(0).GetComponent<MoveTankScript>().MyTeam = PlayerTeamColor[3];
				}
			}
		}
		else if (!MapEditorMaster.instance)
		{
			AmountGoodTanks = ((!MapEditorMaster.instance) ? 1 : 0);
			GameObject gameObject = Object.Instantiate(PlayerPrefab, playerLocation[0], Quaternion.Euler(0f, 0f, 0f), currentLoadedLevel.transform);
			if (CurrentMission == 99)
			{
				Debug.Log("CREATING PARTICLES");
				GameObject obj2 = Object.Instantiate(SpawningPlayerParticles, gameObject.transform.position, Quaternion.identity, currentLoadedLevel.transform);
				ParticleSystem component = obj2.GetComponent<ParticleSystem>();
				obj2.GetComponent<Play2DClipOnce>().overrideGameStarted = true;
				component.Play();
				obj2.transform.Rotate(new Vector3(-90f, 0f, 0f));
			}
		}
		else if ((bool)MapEditorMaster.instance)
		{
			Debug.Log("RESETTING MAP PIECES THINGIES");
			array = GameObject.FindGameObjectsWithTag("MapeditorField");
			for (int j = 0; j < array.Length; j++)
			{
				MapEditorGridPiece component2 = array[j].GetComponent<MapEditorGridPiece>();
				for (int k = 0; k < 5; k++)
				{
					if (component2.propOnMe[k] && component2.mission == CurrentMission && (component2.myPropID[k] == 4 || component2.myPropID[k] == 5 || component2.myPropID[k] == 28 || component2.myPropID[k] == 29))
					{
						if ((component2.myPropID[k] != 5 || PlayerTeamColor[0] == PlayerTeamColor[1]) && (component2.myPropID[k] != 28 || PlayerTeamColor[0] == PlayerTeamColor[2]) && (component2.myPropID[k] != 29 || PlayerTeamColor[0] == PlayerTeamColor[3]))
						{
							int team = -1;
							if (component2.MyTeamNumber > -1)
							{
								team = component2.MyTeamNumber;
							}
							Object.Destroy(component2.myProp[k]);
							component2.propOnMe[k] = false;
							component2.SpawnInProps(component2.myPropID[k], component2.rotationDirection[k], team, k, component2.SpawnDifficulty);
						}
					}
					else if (component2.propOnMe[k] && component2.mission == CurrentMission && component2.myProp[k] == null && (component2.myPropID[k] == 40 || component2.myPropID[k] == 45 || component2.myPropID[k] == 2 || component2.myPropID[k] == 49))
					{
						component2.propOnMe[k] = false;
						component2.SpawnInProps(component2.myPropID[k], component2.rotationDirection[k], 0, k, component2.SpawnDifficulty);
					}
				}
			}
		}
		UpdatePlayerToAI();
		if (!MapEditorMaster.instance)
		{
			for (int l = 0; l < BreakableBlocksLocations.Count; l++)
			{
				int num = 0;
				Collider[] array2 = Physics.OverlapSphere(BreakableBlocksLocations[l], 0.2f);
				foreach (Collider collider in array2)
				{
					if (collider.tag == "Solid" && collider.GetComponent<DestroyableWall>() != null)
					{
						num = 1;
					}
				}
				if (num == 0)
				{
					Object.Instantiate(CorkBlock, BreakableBlocksLocations[l], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int m = 0; m < StoneBlocksLocations.Count; m++)
			{
				int num2 = 0;
				Collider[] array2 = Physics.OverlapSphere(StoneBlocksLocations[m], 0.2f);
				foreach (Collider collider2 in array2)
				{
					if (collider2.tag == "Solid" && collider2.GetComponent<DestroyableWall>() != null)
					{
						num2 = 1;
					}
				}
				if (num2 == 0)
				{
					Object.Instantiate(StoneBlock, StoneBlocksLocations[m], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int n = 0; n < TNTLocations.Count; n++)
			{
				int num3 = 0;
				Collider[] array2 = Physics.OverlapSphere(TNTLocations[n], 0.2f);
				foreach (Collider collider3 in array2)
				{
					if (collider3.tag == "Solid" && collider3.GetComponent<ExplosiveBlock>() != null)
					{
						num3 = 1;
					}
				}
				if (num3 == 0)
				{
					Object.Instantiate(TNTBlock, TNTLocations[n], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
			for (int num4 = 0; num4 < BreakableHalfBlocksLocations.Count; num4++)
			{
				int num5 = 0;
				Collider[] array2 = Physics.OverlapSphere(BreakableHalfBlocksLocations[num4], 0.2f);
				foreach (Collider collider4 in array2)
				{
					if (collider4.tag == "Solid" && collider4.GetComponent<DestroyableWall>() != null)
					{
						num5 = 1;
					}
				}
				if (num5 == 0)
				{
					Object.Instantiate(HalfCorkBlock, BreakableHalfBlocksLocations[num4], Quaternion.Euler(new Vector3(0f, 0f, 0f))).transform.parent = currentLoadedLevel.transform;
				}
			}
		}
		restartGame = true;
		EnemyTankTracksAudio = 0;
		FindPlayers();
		StartCoroutine(GetTankTeamData(fast: false));
		FindPlayers();
		Enemies = GameObject.FindGameObjectsWithTag("Enemy");
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
				array = Enemies;
				for (int j = 0; j < array.Length; j++)
				{
					EnemyAI component3 = array[j].GetComponent<EnemyAI>();
					if ((bool)component3)
					{
						MoveTankScript component4 = Players[0].GetComponent<MoveTankScript>();
						if ((bool)component4 && (component3.MyTeam != component4.MyTeam || component4.MyTeam == 0))
						{
							AmountEnemyTanks++;
						}
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
		for (int j = 0; j < array.Length; j++)
		{
			MonoBehaviour[] components = array[j].GetComponents<MonoBehaviour>();
			for (int num6 = 0; num6 < components.Length; num6++)
			{
				components[num6].enabled = true;
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
		Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
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
		Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Bosses = GameObject.FindGameObjectsWithTag("Boss");
		EnemyScripts = GameObject.FindGameObjectsWithTag("EnemyScripting");
		GameObject[] array2 = Enemies.Concat(Bosses).Concat(EnemyScripts).Concat(Players)
			.ToArray();
		EnemyTankTracksAudio = 0;
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
		if (!isZombieMode)
		{
			return;
		}
		Debug.LogError("NEW ROUND!");
		ZombieTankSpawner component = GetComponent<ZombieTankSpawner>();
		component.Wave++;
		GameObject[] sun = Sun;
		for (int i = 0; i < sun.Length; i++)
		{
			sun[i].SetActive(value: true);
		}
		component.amountEnemies = 0;
		component.spawned = 0;
		component.spawnAmount = Mathf.RoundToInt((float)(component.Wave * 2 + Mathf.RoundToInt(Random.Range(-(component.Wave / 2), component.Wave / 2))) * component.multiplier);
		if (component.spawnAmount < 1)
		{
			component.spawnAmount = 2;
		}
		if (component.Wave > 1)
		{
			if (component.Wave < 10)
			{
				AccountMaster.instance.IncreaseMarbles(1);
			}
			else if (component.Wave < 20)
			{
				AccountMaster.instance.IncreaseMarbles(2);
			}
			else
			{
				AccountMaster.instance.IncreaseMarbles(3);
			}
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

	public void Play2DClipAtPoint(AudioClip clip, float volume)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = volume * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}