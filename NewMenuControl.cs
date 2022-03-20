using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rewired;
using Rewired.UI.ControlMapper;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NewMenuControl : MonoBehaviour
{
	private Vector2 input;

	public int currentMenu;

	public List<int> menuAmountOptions = new List<int>();

	public int Selection;

	[Header("Temp Selection")]
	public int Temp_scene;

	public MainMenuButtons Temp_MMB;

	public int Temp_startingLevel;

	[Header("Audio")]
	public AudioClip errorSound;

	public AudioClip MenuSwitch;

	public AudioClip MarkerSound;

	public AudioClip MenuChange;

	public AudioClip[] Jingles;

	public AudioClip SecondSoundStart;

	private MainMenuButtons currentScript;

	public bool CanDoSomething = true;

	public Animator camAnimator;

	public TextMeshProUGUI Recordmission;

	public TextMeshProUGUI RecordmissionPage2;

	public TextMeshProUGUI ExtrasNotification;

	public TextMeshProUGUI SignedInText;

	[Header("Online")]
	public TMP_InputField Create_AccountNameInput;

	public TMP_InputField Create_PasswordInput;

	public TMP_InputField Create_PasswordInputCheck;

	public TMP_InputField Login_AccountNameInput;

	public TMP_InputField Login_PasswordInput;

	public TextMeshProUGUI CenterText;

	public TextMeshProUGUI SignInNotificationText;

	public TextMeshProUGUI CreateAccountNotificationText;

	public GameObject OnlineMapPrefab;

	public GameObject OnlineMyMapPrefab;

	public GameObject OnlineMapParent;

	public GameObject OnlineMyMapParent;

	public GameObject OnlineButton;

	public TMP_InputField Lobby_Code;

	public TextMeshProUGUI LobbyCodeText;

	public TextMeshProUGUI Player1LobbyText;

	public TextMeshProUGUI Player2LobbyText;

	public GameObject StartLobbyGameButton;

	public GameObject StartLobbyGameButtonSlider;

	[Header("Settings Texts")]
	public TMP_Dropdown Graphics_list;

	public TMP_Dropdown Resolution_list;

	public ButtonMouseEvents Fullscreen_toggle;

	public TMP_Dropdown FPS_list;

	public TMP_Dropdown MasterVolume_list;

	public TMP_Dropdown MusicVolume_list;

	public TMP_Dropdown SFXVolume_list;

	public ButtonMouseEvents FriendlyFire_toggle;

	public TextMeshProUGUI AICompaniontext;

	public ButtonMouseEvents SnowMode_toggle;

	public ButtonMouseEvents MarkedTanks_toggle;

	public ButtonMouseEvents XRAYBULLETS_toggle;

	public TextMeshProUGUI GoreModeText;

	public GameObject GoreModeObject;

	public TextMeshProUGUI NoMapsText;

	public TextMeshProUGUI NoKillsText;

	public TMP_Dropdown Difficulty_list;

	public TMP_Dropdown Difficulty_list_campaign;

	public TextMeshProUGUI DifficultyExplainText;

	public ButtonMouseEvents vsync_toggle;

	public TextMeshProUGUI SurvivalGamesText_shadow;

	public TextMeshProUGUI TransferAccountText;

	public GameObject[] TransferButtons;

	public Color activeTextColor;

	public TextMeshProUGUI VideoOptionstext;

	public GameObject MapFilePrefab;

	public GameObject MapFileView;

	public bool MapLoading;

	public GameObject AchievementPrefab;

	public GameObject AchievementParent;

	public GameObject UnlockablePrefab;

	public GameObject UnlockableParent;

	public GameObject InventoryItemsParent;

	public GameObject RebindKeyPrefab;

	public GameObject RebindKeyParent;

	public GameObject TankKillItemPrefab;

	public GameObject TankKillItemParent;

	public int StatisticsOpenMenu;

	public int ControlsOpenMenu;

	public RebindKeyScript selectedRKS;

	public GameObject[] Menus;

	public GameObject OptionsMenu;

	public GameObject NewGameMenu;

	public GameObject ContinueMenu;

	public int[] lastKnownPlaces;

	public ToolTipLoading TTL;

	public LoadingIconScript LIS;

	public GameObject LIS_parent;

	public KeyCode[] AssignedCodes;

	public MainMenuButtons[] MMBlevels;

	public TextMeshProUGUI CheckPointTitle;

	public int PreviousMenu;

	public GetPublicMaps GPM;

	private List<GameObject> MapObjects = new List<GameObject>();

	public Player player;

	public int AmountMaps;

	public GameObject DemoObject;

	public TextAsset EditClassicMapID;

	public bool HoldingShift;

	public string EnteredCode = "";

	private KeyCode[] keyCodes = new KeyCode[10]
	{
		KeyCode.Alpha0,
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9
	};

	public bool CanMove = true;

	public bool IsUsingMouse = true;

	private int SelectedCheckpoint;

	private bool prevRequestIsHere = true;

	public float waitingTimeBetweenRequests = 0.8f;

	public PlayerInputsMenu PIM;

	private IEnumerator PlayJingle()
	{
		yield return new WaitForSeconds(0.4f);
		int num = UnityEngine.Random.Range(0, Jingles.Length);
		SFXManager.instance.PlaySFX(Jingles[num]);
	}

	private void Awake()
	{
		Time.timeScale = 1f;
	}

	public void GetMapFiles()
	{
		foreach (GameObject mapObject in MapObjects)
		{
			UnityEngine.Object.Destroy(mapObject);
		}
		MapObjects.Clear();
		string text = Application.persistentDataPath + "/";
		text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text += "/My Games/Wee Tanks/";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		new FileInfo(text);
		FileInfo[] files = new DirectoryInfo(text).GetFiles("*.campaign");
		if (files.Length != 0)
		{
			Debug.LogError("MapFiles FOUND!" + files.Length);
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				string mapname = fileInfo.Name.Replace(".campaign", "");
				int MapSize = 285;
				if (mapname.Length < 1 || mapname.Length > 25)
				{
					continue;
				}
				MapEditorData mapEditorData = SavingMapEditorData.LoadData(mapname);
				if (mapEditorData == null)
				{
					continue;
				}
				if (NoMapsText != null)
				{
					NoMapsText.gameObject.SetActive(value: false);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(MapFilePrefab);
				MapObjects.Add(gameObject);
				gameObject.transform.SetParent(MapFileView.transform, worldPositionStays: false);
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = mapname;
				CampaignItemScript campaignItemScript = null;
				if (mapEditorData.VersionCreated != "v0.7.12" && mapEditorData.VersionCreated != "v0.7.11" && mapEditorData.VersionCreated != "v0.7.10" && mapEditorData.VersionCreated != "v0.8.0e" && mapEditorData.VersionCreated != "v0.8.0d" && mapEditorData.VersionCreated != "v0.8.0c" && mapEditorData.VersionCreated != "v0.8.0b")
				{
					GameObject obj = UnityEngine.Object.Instantiate(OnlineMyMapPrefab);
					obj.transform.SetParent(OnlineMyMapParent.transform, worldPositionStays: false);
					campaignItemScript = obj.GetComponent<CampaignItemScript>();
					campaignItemScript.campaignName = mapname;
					campaignItemScript.campaignVersion = mapEditorData.VersionCreated;
					campaignItemScript.NMC = this;
					campaignItemScript.map_size = mapEditorData.MapSize;
					campaignItemScript.amount_missions = mapEditorData.missionAmount;
					campaignItemScript.campaign_difficulty = mapEditorData.difficulty;
					AmountMaps++;
					if (mapEditorData.PID > 0)
					{
						campaignItemScript.isPublished = mapEditorData.isPublished;
						campaignItemScript.campaignID = mapEditorData.PID;
					}
				}
				campaignItemScript = gameObject.GetComponent<CampaignItemScript>();
				_ = (bool)campaignItemScript;
				_ = campaignItemScript.isMainMenuCampaign;
				if ((bool)campaignItemScript && campaignItemScript.isMainMenuCampaign)
				{
					if (mapEditorData.missionAmount > 0)
					{
						campaignItemScript.text_amountmissions.text = mapEditorData.missionAmount.ToString();
					}
					string text2 = ((mapEditorData.signedName != "") ? mapEditorData.signedName : "unknown");
					if (OptionsMainMenu.instance.CurrentVersion != mapEditorData.VersionCreated)
					{
						campaignItemScript.text_subtitle.color = Color.red;
						if (mapEditorData.VersionCreated != null)
						{
							campaignItemScript.text_subtitle.text = "WARNING! Created in " + mapEditorData.VersionCreated;
						}
						else
						{
							campaignItemScript.text_subtitle.text = "WARNING! Unknown version";
						}
					}
					else
					{
						campaignItemScript.text_subtitle.text = "";
						campaignItemScript.text_subtitle.color = Color.grey;
						if (text2 != "")
						{
							campaignItemScript.text_subtitle.text = "Created by " + text2;
						}
					}
				}
				gameObject.GetComponent<EventTrigger>();
				try
				{
					MapSize = mapEditorData.MapSize;
					if (MapSize == 0)
					{
						MapSize = 285;
					}
				}
				catch (Exception)
				{
					Debug.Log("no map size found");
				}
				EventTrigger component = gameObject.gameObject.GetComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate
				{
					OnMapClick(mapname, MapSize);
				});
				component.triggers.Add(entry);
			}
		}
		else if (NoMapsText != null)
		{
			Debug.LogError("NO MAPS FOUND! " + text);
			NoMapsText.gameObject.SetActive(value: true);
		}
	}

	public void EnableTransferMenu()
	{
		TransferAccountText.text = "Hey! Wee Tanks detected you have a Steam account.\n\n Would you like to connect <color=#0000FF>" + AccountMaster.instance.Username + "</color> to your Steam (<color=#0000FF>" + SteamTest.instance.username + "</color>) account?";
		enableMenu(24);
	}

	private IEnumerator CheckData()
	{
		if (!AccountMaster.instance.hasDoneSignInCheck)
		{
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(CheckData());
		}
		else if (AccountMaster.instance.isSignedIn)
		{
			Debug.Log("SIGNED IN!");
			if (SteamTest.instance.SteamAccountID > 1000 && AccountMaster.instance.SteamUserID < 1000)
			{
				EnableTransferMenu();
			}
		}
	}

	private void Start()
	{
		OptionsMainMenu.instance.MapSize = 285;
		OptionsMainMenu.instance.StartLevel = 0;
		StartCoroutine(PlayJingle());
		StartCoroutine(CheckData());
		if ((bool)OptionsMainMenu.instance)
		{
			OptionsMainMenu.instance.AMUS.Clear();
			if (OptionsMainMenu.instance.IsDemo)
			{
				DemoObject.SetActive(value: true);
			}
			else
			{
				DemoObject.SetActive(value: false);
			}
		}
		GetMapFiles();
		GameObject[] menus = Menus;
		foreach (GameObject gameObject in menus)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			int num = 0;
			int childCount = gameObject.transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				if (gameObject.transform.GetChild(j).GetComponent<MainMenuButtons>() != null)
				{
					num++;
				}
			}
			menuAmountOptions.Add(num - 1);
		}
		Fullscreen_toggle.IsEnabled = OptionsMainMenu.instance.isFullscreen;
		FriendlyFire_toggle.IsEnabled = OptionsMainMenu.instance.FriendlyFire;
		SnowMode_toggle.IsEnabled = OptionsMainMenu.instance.SnowMode;
		XRAYBULLETS_toggle.IsEnabled = OptionsMainMenu.instance.showxraybullets;
		MarkedTanks_toggle.IsEnabled = OptionsMainMenu.instance.MarkedTanks;
		vsync_toggle.IsEnabled = OptionsMainMenu.instance.vsync;
		for (int k = 0; k < 50; k++)
		{
			if (OptionsMainMenu.instance.AMnames[k] != "")
			{
				GameObject obj = UnityEngine.Object.Instantiate(AchievementPrefab);
				obj.transform.SetParent(AchievementParent.transform);
				obj.GetComponent<AchievementItemScript>().AMID = k;
			}
		}
		for (int l = 0; l < OptionsMainMenu.instance.AM.Length; l++)
		{
			GameObject obj2 = UnityEngine.Object.Instantiate(UnlockablePrefab);
			obj2.transform.SetParent(UnlockableParent.transform);
			obj2.GetComponent<UnlockableScript>().ULID = l;
		}
		MusicVolume_list.SetValueWithoutNotify(OptionsMainMenu.instance.musicVolumeLvl);
		MasterVolume_list.SetValueWithoutNotify(OptionsMainMenu.instance.masterVolumeLvl);
		SFXVolume_list.SetValueWithoutNotify(OptionsMainMenu.instance.sfxVolumeLvl);
		SetAudioText();
		SetDifficultyText();
		SetGraphicsText();
		SetResolutionText();
		SetFPSText();
		enableMenu(0);
		UpdateDifficultyText();
		StartCoroutine(LateStart());
	}

	private void SetGameplayToggles()
	{
		FriendlyFire_toggle.IsEnabled = OptionsMainMenu.instance.FriendlyFire;
		SnowMode_toggle.IsEnabled = OptionsMainMenu.instance.SnowMode;
		XRAYBULLETS_toggle.IsEnabled = OptionsMainMenu.instance.showxraybullets;
		MarkedTanks_toggle.IsEnabled = OptionsMainMenu.instance.MarkedTanks;
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.2f);
		int num = 0;
		for (int i = 0; i < GameMaster.instance.TankColorKilled.Count; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(TankKillItemPrefab);
			obj.transform.SetParent(TankKillItemParent.transform);
			obj.GetComponent<TankStatsItem>().myMenu = 1;
			obj.GetComponent<TankStatsItem>().myStatID = i;
			obj.GetComponent<TankStatsItem>().NMC = this;
			obj.GetComponent<TankStatsItem>().originalParent = TankKillItemParent.transform;
			if (GameMaster.instance.TankColorKilled[i] > 0)
			{
				num++;
			}
		}
		_ = 0;
		for (int j = 0; j < 5; j++)
		{
			GameObject obj2 = UnityEngine.Object.Instantiate(TankKillItemPrefab);
			obj2.transform.SetParent(TankKillItemParent.transform);
			obj2.GetComponent<TankStatsItem>().myMenu = 0;
			obj2.GetComponent<TankStatsItem>().myStatID = j;
			obj2.GetComponent<TankStatsItem>().NMC = this;
			obj2.GetComponent<TankStatsItem>().originalParent = TankKillItemParent.transform;
		}
		for (int k = 0; k < 8; k++)
		{
			GameObject obj3 = UnityEngine.Object.Instantiate(TankKillItemPrefab);
			obj3.transform.SetParent(TankKillItemParent.transform);
			obj3.GetComponent<TankStatsItem>().myMenu = 2;
			obj3.GetComponent<TankStatsItem>().myStatID = k;
			obj3.GetComponent<TankStatsItem>().NMC = this;
			obj3.GetComponent<TankStatsItem>().originalParent = TankKillItemParent.transform;
		}
		UpdateMenuSignedInText();
		yield return new WaitForSeconds(2f);
		bool assignedInventory = false;
		if (AccountMaster.instance.Inventory.InventoryItems != null)
		{
			AssignInventory();
			assignedInventory = true;
		}
		yield return new WaitForSeconds(2f);
		if (!assignedInventory)
		{
			AssignInventory();
		}
		UpdateMenuSignedInText();
	}

	private void AssignInventory()
	{
		for (int i = 0; i < AccountMaster.instance.Inventory.InventoryItems.Length; i++)
		{
			foreach (TankeyTownStockItem item in GlobalAssets.instance.StockDatabase)
			{
				if (item.ItemID == AccountMaster.instance.Inventory.InventoryItems[i] && !item.IsMapEditorObject)
				{
					GameObject obj = UnityEngine.Object.Instantiate(UnlockablePrefab);
					obj.transform.SetParent(InventoryItemsParent.transform);
					obj.GetComponent<UnlockableScript>().isTankeyTownItem = true;
					obj.GetComponent<UnlockableScript>().ULID = item.ItemID + 1000;
					obj.GetComponent<UnlockableScript>().UnlockableTitle.text = item.ItemName;
					obj.GetComponent<UnlockableScript>().UnlockableRequire.text = "";
					obj.GetComponent<UnlockableScript>().isBoost = item.isBoost;
					obj.GetComponent<UnlockableScript>().isBullet = item.isBullet;
					obj.GetComponent<UnlockableScript>().isHitmarker = item.isHitmarker;
					obj.GetComponent<UnlockableScript>().isMine = item.isMine;
					obj.GetComponent<UnlockableScript>().isSkin = item.isSkin;
					obj.GetComponent<UnlockableScript>().isSkidmarks = item.isSkidmarks;
				}
			}
		}
	}

	private void OnMapStart()
	{
		if (MapLoading)
		{
			StartCoroutine(LoadYourAsyncScene(3));
		}
		else
		{
			StartCoroutine(LoadYourAsyncScene(4));
		}
	}

	private void OnMapClick(string mapname, int mapsize)
	{
		Temp_MMB = null;
		Temp_scene = 100;
		Temp_startingLevel = 0;
		PIM.CanPlayWithAI = true;
		PIM.SetControllers();
		enableMenu(25);
		PIM.EnableDifficultySetter();
		StartCoroutine(doing());
		if (mapname == "CLASSICMAP")
		{
			OptionsMainMenu.instance.ClassicMap = EditClassicMapID;
			OptionsMainMenu.instance.MapSize = 285;
		}
		else
		{
			OptionsMainMenu.instance.ClassicMap = null;
			OptionsMainMenu.instance.MapEditorMapName = mapname;
			OptionsMainMenu.instance.MapSize = mapsize;
		}
	}

	public void UpdateMenuSignedInText()
	{
		if (AccountMaster.instance.isSignedIn)
		{
			SignedInText.text = LocalizationMaster.instance.GetText("Account_signed_in_status") + "<br>" + AccountMaster.instance.Username;
			SignedInText.color = Color.black;
		}
		else
		{
			SignedInText.text = "You are not signed in!";
			SignedInText.color = Color.red;
		}
	}

	private void Update()
	{
		this.player = ReInput.players.GetPlayer(0);
		bool flag = false;
		for (int i = 0; i < ReInput.players.playerCount; i++)
		{
			Player player = ReInput.players.GetPlayer(i);
			if (player.isPlaying)
			{
				input.x = player.GetAxis("Move Horizontal");
				input.y = player.GetAxis("Move Vertically");
				flag = player.GetButtonUp("Menu Use");
				if (input.y < 0f || input.y > 0f || flag)
				{
					break;
				}
			}
		}
		if (Input.GetAxisRaw("Mouse X") < 0f || Input.GetAxisRaw("Mouse Y") > 0f)
		{
			IsUsingMouse = true;
		}
		if (NoKillsText != null)
		{
			if (StatisticsOpenMenu == 1 && !NoKillsText.gameObject.activeSelf)
			{
				NoKillsText.gameObject.SetActive(value: true);
			}
			else if (StatisticsOpenMenu != 1 && NoKillsText.gameObject.activeSelf)
			{
				NoKillsText.gameObject.SetActive(value: false);
			}
		}
		if ((bool)GoreModeObject)
		{
			if (GoreModeObject.activeSelf && !OptionsMainMenu.instance.AMselected.Contains(58))
			{
				OptionsMainMenu.instance.BloodMode = false;
				GoreModeObject.SetActive(value: false);
			}
			else if (!GoreModeObject.activeSelf && OptionsMainMenu.instance.AMselected.Contains(58))
			{
				OptionsMainMenu.instance.BloodMode = true;
				GoreModeObject.SetActive(value: true);
			}
		}
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.L) && !HoldingShift)
		{
			HoldingShift = true;
		}
		else if (HoldingShift && (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.L)))
		{
			HoldingShift = false;
			int num = int.Parse(EnteredCode);
			if (num > 0 && num < 101)
			{
				OptionsMainMenu.instance.StartLevel = num - 1;
				StartCoroutine(LoadYourAsyncScene(1));
			}
			else if (num > 100)
			{
				OptionsMainMenu.instance.StartLevel = num - 101;
				TTL.Survival = true;
				StartCoroutine(LoadYourAsyncScene(2));
			}
			Debug.Log(EnteredCode);
			EnteredCode = "";
		}
		if (HoldingShift)
		{
			for (int j = 0; j < keyCodes.Length; j++)
			{
				if (Input.GetKeyDown(keyCodes[j]))
				{
					EnteredCode += j;
				}
			}
		}
		if (CanDoSomething)
		{
			if (input.y < -0.5f)
			{
				GameMaster.instance.isPlayingWithController = true;
			}
			else if (input.y > 0.5f)
			{
				GameMaster.instance.isPlayingWithController = true;
			}
			if (flag)
			{
				GameMaster.instance.isPlayingWithController = true;
				doButton(currentScript);
			}
		}
	}

	public void UpdateDifficultyText()
	{
	}

	public IEnumerator doing()
	{
		CanDoSomething = false;
		SFXManager.instance.PlaySFX(MarkerSound, 0.8f);
		yield return new WaitForSeconds(0.2f);
		CanDoSomething = true;
	}

	public IEnumerator MoveSelection(bool up)
	{
		if (CanMove)
		{
			IsUsingMouse = false;
			CanMove = false;
			if (up)
			{
				Selection--;
			}
			else
			{
				Selection++;
			}
			yield return new WaitForSeconds(0.3f);
			CanMove = true;
		}
	}

	private void deselectButton(MainMenuButtons MMB)
	{
		MMB.Selected = false;
		MMB.startTime = Time.time;
		MMB.RightClicked = false;
	}

	public void doRightButton(MainMenuButtons MMB)
	{
		if (MMB.IsContinue && MMB.canBeSelected)
		{
			for (int i = 0; i < MMBlevels.Length; i++)
			{
				MMBlevels[i].ContinueLevel = MMB.ContinueLevel - 10 + i;
				if ((bool)MMBlevels[i].ButtonTitle)
				{
					MMBlevels[i].ButtonTitle.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (MMB.ContinueLevel - 9 + i);
					continue;
				}
				MMBlevels[i].thisText = MMBlevels[i].GetComponent<TextMeshProUGUI>();
				MMBlevels[i].thisText.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (MMB.ContinueLevel - 9 + i);
			}
			SelectedCheckpoint = MMB.ContinueLevel;
			deselectButton(MMB);
			enableMenu(17);
			StartCoroutine(doing());
		}
		else
		{
			deselectButton(MMB);
			StartCoroutine(doing());
		}
	}

	public void ChangeFile()
	{
	}

	private IEnumerator SignMeOut(string url, string name, string key, string userid)
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", key);
		wWWForm.AddField("userid", userid);
		wWWForm.AddField("username", name);
		wWWForm.AddField("authKey", text);
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			AccountMaster.instance.isSignedIn = false;
			UpdateMenuSignedInText();
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		Menus[currentMenu].SetActive(value: false);
		CenterText.text = "You are now signed out.";
		CenterText.gameObject.SetActive(value: true);
		AccountMaster.instance.SignOut(manual: true);
		yield return new WaitForSeconds(2f);
		CenterText.gameObject.SetActive(value: false);
		currentMenu = 18;
		Menus[currentMenu].SetActive(value: true);
		SignInNotificationText.gameObject.SetActive(value: false);
		UpdateMenuSignedInText();
	}

	private IEnumerator CreateAccount(string url, string username, string password)
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", username);
		wWWForm.AddField("password", password);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("userData", JsonUtility.ToJson(GameMaster.instance.CurrentData));
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		uwr.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		uwr.SetRequestHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
		uwr.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
		uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");
		uwr.chunkedTransfer = false;
		CenterText.text = "Creating account...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = "Creation failed";
				GameMaster.instance.CurrentData.marbles = 0;
				GameMaster.instance.CurrentData.accountname = null;
			}
			else if (uwr.downloadHandler.text.Contains("EXISTS"))
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = "Name already exists";
				GameMaster.instance.CurrentData.marbles = 0;
				GameMaster.instance.CurrentData.accountname = null;
			}
			else
			{
				AccountMaster.instance.isSignedIn = false;
				CenterText.text = "Account created!";
				CenterText.gameObject.SetActive(value: true);
				yield return new WaitForSeconds(2f);
				CenterText.gameObject.SetActive(value: false);
				enableMenu(18);
				Create_AccountNameInput.text = null;
				Create_PasswordInput.text = null;
				Create_PasswordInputCheck.text = null;
				CreateAccountNotificationText.gameObject.SetActive(value: false);
				UpdateMenuSignedInText();
			}
		}
		CenterText.gameObject.SetActive(value: false);
		Menus[currentMenu].SetActive(value: true);
	}

	private IEnumerator SignIn(string url, string username, string password)
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Network error";
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		Debug.Log("GOT KEY REQUEST: " + keyRequest.downloadHandler.text);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", username);
		wWWForm.AddField("password", password);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("fromGame", "true");
		if (GameMaster.instance.CurrentData.marbles == 0)
		{
			int num = 0;
			for (int i = 0; i < OptionsMainMenu.instance.AM.Length; i++)
			{
				if (OptionsMainMenu.instance.AM[i] == 1)
				{
					num += OptionsMainMenu.instance.AM_marbles[i];
				}
			}
			GameMaster.instance.CurrentData.marbles += num;
			GameMaster.instance.CurrentData.accountname = username;
			wWWForm.AddField("userData", JsonUtility.ToJson(GameMaster.instance.CurrentData));
		}
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		uwr.chunkedTransfer = false;
		CenterText.text = "Signing in...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				SignInNotificationText.gameObject.SetActive(value: true);
				SignInNotificationText.text = "Sign in failed";
				AccountMaster.instance.isSignedIn = false;
				UpdateMenuSignedInText();
			}
			else
			{
				CenterText.text = "You are now signed in.";
				CenterText.gameObject.SetActive(value: true);
				string[] splitArray = uwr.downloadHandler.text.Split(char.Parse("/"));
				AccountMaster.instance.UserID = splitArray[1];
				GameMaster.instance.AccountID = int.Parse(splitArray[1]);
				AccountMaster.instance.Key = splitArray[0];
				AccountMaster.instance.Username = Login_AccountNameInput.text;
				AccountMaster.instance.isSignedIn = true;
				AccountMaster.instance.SteamUserID = 0uL;
				AccountMaster.instance.SaveCredentials();
				yield return new WaitForSeconds(1f);
				int num2 = int.Parse(splitArray[2]);
				if (num2 > 0)
				{
					AccountMaster.instance.PDO.marbles = num2;
					GameMaster.instance.CurrentData.marbles = num2;
					AccountMaster.instance.ShowMarbleNotification(num2);
				}
				yield return new WaitForSeconds(1f);
				AccountMaster.instance.StartCoroutine(AccountMaster.instance.LoadCloudData());
				CenterText.gameObject.SetActive(value: false);
				enableMenu(18);
				SignInNotificationText.gameObject.SetActive(value: false);
				UpdateMenuSignedInText();
			}
		}
		CenterText.gameObject.SetActive(value: false);
		Menus[currentMenu].SetActive(value: true);
	}

	private IEnumerator GetLobbyInfo()
	{
		if (!prevRequestIsHere)
		{
			yield break;
		}
		prevRequestIsHere = false;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", AccountMaster.instance.Username);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("lobbyid", AccountMaster.instance.LobbyID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/get_lobby_info.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		prevRequestIsHere = true;
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			yield return new WaitForSeconds(waitingTimeBetweenRequests);
			StartCoroutine(GetLobbyInfo());
		}
		else if (uwr.downloadHandler.text.Contains("NO_PLAYER_2"))
		{
			Player2LobbyText.text = "Awaiting player 2..";
			yield return new WaitForSeconds(waitingTimeBetweenRequests);
			StartCoroutine(GetLobbyInfo());
		}
		else if (uwr.downloadHandler.text.Contains("STARTED"))
		{
			if (!LobbyMaster.instance.LobbyStarted)
			{
				StartCoroutine(LoadYourAsyncScene(6));
			}
		}
		else if (uwr.downloadHandler.text.Contains("STOPPED"))
		{
			CenterText.text = "Lobby stopped!";
			CenterText.gameObject.SetActive(value: true);
			AccountMaster.instance.LobbyID = 0;
			AccountMaster.instance.LobbyCode = null;
			yield return new WaitForSeconds(waitingTimeBetweenRequests);
			CenterText.gameObject.SetActive(value: false);
			currentMenu = 18;
			Menus[currentMenu].SetActive(value: true);
			SignInNotificationText.gameObject.SetActive(value: false);
		}
		else
		{
			if (LobbyMaster.instance.MyPlayerID == 1)
			{
				LobbyMaster.instance.Player1Name = uwr.downloadHandler.text;
				LobbyMaster.instance.Player2Name = AccountMaster.instance.Username;
			}
			else
			{
				LobbyMaster.instance.Player2Name = uwr.downloadHandler.text;
				LobbyMaster.instance.Player1Name = AccountMaster.instance.Username;
			}
			Player1LobbyText.text = ((LobbyMaster.instance.Player1Name.Length > 2) ? LobbyMaster.instance.Player1Name : "Awaiting player 1..");
			Player2LobbyText.text = ((LobbyMaster.instance.Player2Name.Length > 2) ? LobbyMaster.instance.Player2Name : "Awaiting player 2..");
			yield return new WaitForSeconds(waitingTimeBetweenRequests);
			StartCoroutine(GetLobbyInfo());
		}
	}

	private IEnumerator LeaveLobby()
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", AccountMaster.instance.Username);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("lobbyid", AccountMaster.instance.LobbyID);
		wWWForm.AddField("action", 2);
		wWWForm.AddField("authKey", text);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/send_lobby_data.php", wWWForm);
		uwr.chunkedTransfer = false;
		CenterText.text = "Stopping lobby...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!uwr.downloadHandler.text.Contains("FAILED"))
			{
				CenterText.text = "Lobby stopped!";
				CenterText.gameObject.SetActive(value: true);
				AccountMaster.instance.LobbyID = 0;
				AccountMaster.instance.LobbyCode = null;
				yield return new WaitForSeconds(1f);
				CenterText.gameObject.SetActive(value: false);
				currentMenu = 18;
				Menus[currentMenu].SetActive(value: true);
				SignInNotificationText.gameObject.SetActive(value: false);
			}
		}
		CenterText.gameObject.SetActive(value: false);
		Menus[currentMenu].SetActive(value: true);
	}

	private IEnumerator JoinLobby()
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text.Contains("WAIT"))
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", AccountMaster.instance.Username);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("lobbyCODE", Lobby_Code.text);
		Debug.Log(Lobby_Code.text);
		wWWForm.AddField("action", 1);
		wWWForm.AddField("authKey", text);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/create_lobby_data.php", wWWForm);
		uwr.chunkedTransfer = false;
		CenterText.text = "Joining lobby...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!uwr.downloadHandler.text.Contains("FAILED"))
			{
				CenterText.text = "Lobby joined!";
				CenterText.gameObject.SetActive(value: true);
				LobbyMaster.instance.MyPlayerID = 1;
				AccountMaster.instance.LobbyID = int.Parse(uwr.downloadHandler.text);
				LobbyCodeText.text = "";
				StartLobbyGameButton.SetActive(value: false);
				StartLobbyGameButtonSlider.SetActive(value: false);
				yield return new WaitForSeconds(1f);
				CenterText.gameObject.SetActive(value: false);
				currentMenu = 23;
				Menus[currentMenu].SetActive(value: true);
				SignInNotificationText.gameObject.SetActive(value: false);
				StartCoroutine(GetLobbyInfo());
			}
		}
		CenterText.gameObject.SetActive(value: false);
		Menus[currentMenu].SetActive(value: true);
	}

	private IEnumerator StartLobby()
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", AccountMaster.instance.Username);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("lobbyCODE", Lobby_Code.text);
		wWWForm.AddField("lobbyid", AccountMaster.instance.LobbyID);
		wWWForm.AddField("action", 0);
		wWWForm.AddField("authKey", text);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/send_lobby_data.php", wWWForm);
		uwr.chunkedTransfer = false;
		CenterText.text = "Starting lobby...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!uwr.downloadHandler.text.Contains("FAILED"))
			{
				CenterText.text = "Lobby started!";
				CenterText.gameObject.SetActive(value: true);
				LobbyCodeText.text = "";
				StartLobbyGameButton.SetActive(value: false);
				StartLobbyGameButtonSlider.SetActive(value: false);
				yield return new WaitForSeconds(1f);
				StartCoroutine(LoadYourAsyncScene(6));
			}
		}
		CenterText.gameObject.SetActive(value: false);
		Menus[currentMenu].SetActive(value: true);
	}

	private IEnumerator CreateLobby()
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			CreateAccountNotificationText.gameObject.SetActive(value: true);
			CreateAccountNotificationText.text = "Please wait before request";
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", AccountMaster.instance.Username);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("action", 0);
		wWWForm.AddField("authKey", text);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/create_lobby_data.php", wWWForm);
		uwr.chunkedTransfer = false;
		CenterText.text = "Creating lobby...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!(uwr.downloadHandler.text == "FAILED"))
			{
				string[] array = uwr.downloadHandler.text.Split(char.Parse("/"));
				CenterText.text = "Lobby created!";
				CenterText.gameObject.SetActive(value: true);
				LobbyMaster.instance.MyPlayerID = 0;
				LobbyMaster.instance.Player1Name = AccountMaster.instance.Username;
				LobbyCodeText.text = "Lobby code: " + array[1];
				Player1LobbyText.text = LobbyMaster.instance.Player1Name;
				Player2LobbyText.text = "Awaiting player 2...";
				AccountMaster.instance.LobbyID = int.Parse(array[0]);
				AccountMaster.instance.LobbyCode = array[1];
				StartLobbyGameButton.SetActive(value: true);
				StartLobbyGameButtonSlider.SetActive(value: true);
				yield return new WaitForSeconds(1f);
				CenterText.gameObject.SetActive(value: false);
				currentMenu = 23;
				Menus[currentMenu].SetActive(value: true);
				SignInNotificationText.gameObject.SetActive(value: false);
				StartCoroutine(GetLobbyInfo());
			}
		}
		CenterText.gameObject.SetActive(value: false);
		Menus[currentMenu].SetActive(value: true);
	}

	public void UploadCampaign(int ID)
	{
	}

	public void DownloadCampaign(int ID)
	{
	}

	public IEnumerator FailedTransferSteam(string custommessage)
	{
		if (custommessage != null)
		{
			TransferAccountText.text = custommessage;
		}
		else
		{
			TransferAccountText.text = "Failed!";
		}
		yield return new WaitForSeconds(2f);
		enableMenu(0);
	}

	public IEnumerator SuccesTransferSteam()
	{
		TransferAccountText.text = "Succes!";
		yield return new WaitForSeconds(2f);
		AccountMaster.instance.SteamUserID = SteamTest.instance.SteamAccountID;
		AccountMaster.instance.SaveCredentials();
		enableMenu(0);
	}

	public void doButton(MainMenuButtons MMB)
	{
		if (MMB == null)
		{
			return;
		}
		if (MMB.ChangeAchievementDifficulty)
		{
			if ((bool)AchievementsTracker.instance)
			{
				AchievementsTracker.instance.selectedDifficulty = MMB.AchievementDifficulty;
			}
		}
		else if (MMB.ChangeControlsMenu)
		{
			ControlsOpenMenu = MMB.ControlsMenu;
		}
		else if (MMB.ChangeStatisticScreen)
		{
			StatisticsOpenMenu = MMB.StatisticScreen;
		}
		else if (MMB.IsCreateAccount)
		{
			if (Create_AccountNameInput.text.Length < 3)
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = "Name has to be atleast 3 long";
				Debug.LogError("Name has to be atleast 3 long");
			}
			else if (Create_PasswordInput.text.Length < 6)
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = "Password has to be at least 6 long";
			}
			else if (Create_PasswordInput.text.Contains("/") || Create_PasswordInput.text.Contains("$") || Create_PasswordInput.text.Contains("{") || Create_PasswordInput.text.Contains("}") || Create_PasswordInput.text.Contains(",") || Create_PasswordInput.text.Contains(".") || Create_PasswordInput.text.Contains("'"))
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = "Name has invalid characters";
			}
			else if (Create_PasswordInput.text != Create_PasswordInputCheck.text)
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = "Passwords not the same";
				Debug.LogError("Check not positive");
			}
			else
			{
				Debug.LogError("Creating account");
				StartCoroutine(CreateAccount("https://www.weetanks.com/create_account.php", Create_AccountNameInput.text, Create_PasswordInput.text));
			}
		}
		else if (MMB.IsSignIn)
		{
			if (Login_AccountNameInput.text.Length < 3)
			{
				SignInNotificationText.gameObject.SetActive(value: true);
				SignInNotificationText.text = "Error: Name has to be at least 3 long";
			}
			else if (Login_PasswordInput.text.Length < 5)
			{
				SignInNotificationText.gameObject.SetActive(value: true);
				SignInNotificationText.text = "Error: Password has to be at least 6 long";
			}
			else
			{
				Debug.LogError("Loggin in account");
				StartCoroutine(SignIn("https://www.weetanks.com/signin_account.php", Login_AccountNameInput.text, Login_PasswordInput.text));
			}
		}
		else if (MMB.IsSignOut)
		{
			StartCoroutine(SignMeOut("https://www.weetanks.com/signout_account.php", AccountMaster.instance.Username, AccountMaster.instance.Key, AccountMaster.instance.UserID));
		}
		else if (MMB.IsOpenCampaignsMenu)
		{
			deselectButton(MMB);
			enableMenu(21);
			StartCoroutine(doing());
		}
		else if (MMB.IsOptions)
		{
			deselectButton(MMB);
			enableMenu(1);
			StartCoroutine(doing());
		}
		else if (MMB.IsStats)
		{
			deselectButton(MMB);
			enableMenu(7);
			StartCoroutine(doing());
		}
		else if (MMB.IsCredits)
		{
			Debug.LogWarning("CREDITS TIME!");
			deselectButton(MMB);
			StartCoroutine(LoadYourAsyncScene(5));
		}
		else if (MMB.IsContinueClassicCampaign)
		{
			deselectButton(MMB);
			enableMenu(3);
			StartCoroutine(doing());
		}
		else if (MMB.IsClassicCampaign)
		{
			deselectButton(MMB);
			enableMenu(2);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsNewClassicCampaign)
		{
			Temp_MMB = MMB;
			Temp_scene = 1;
			Temp_startingLevel = 0;
			PIM.CanPlayWithAI = true;
			PIM.SetControllers();
			PIM.LoadData();
			deselectButton(MMB);
			enableMenu(25);
			PIM.DisableDifficultySetter();
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsAcceptTransferAccount)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				GameObject[] transferButtons = TransferButtons;
				for (int i = 0; i < transferButtons.Length; i++)
				{
					transferButtons[i].SetActive(value: false);
				}
				TransferAccountText.text = "transferring...";
				AccountMaster.instance.StartCoroutine(AccountMaster.instance.TransferAccountToSteam("https://www.weetanks.com/transfer_to_steam_account.php"));
			}
		}
		else if (MMB.IsGoToLobbyMenu)
		{
			deselectButton(MMB);
			enableMenu(22);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsCreateLobby)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				StartCoroutine(CreateLobby());
			}
		}
		else if (MMB.IsJoinLobby)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				StartCoroutine(JoinLobby());
			}
		}
		else if (MMB.IsStartLobby)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				StartCoroutine(StartLobby());
			}
		}
		else if (MMB.IsLeaveLobby)
		{
			StartCoroutine(LeaveLobby());
		}
		else if (MMB.IsDifficultyDown)
		{
			OptionsMainMenu.instance.ChangeDifficulty(-1);
			UpdateDifficultyText();
		}
		else if (MMB.IsDifficultyUp)
		{
			OptionsMainMenu.instance.ChangeDifficulty(1);
			UpdateDifficultyText();
		}
		else if (MMB.IsMapEditor)
		{
			deselectButton(MMB);
			enableMenu(9);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsSelectPlayerController)
		{
			Debug.Log("Value is now at : " + PIM.Dropdowns[MMB.PlayerNumber].value);
			if (PIM.Dropdowns[MMB.PlayerNumber].value < PIM.Dropdowns[MMB.PlayerNumber].options.Count - 1)
			{
				PIM.Dropdowns[MMB.PlayerNumber].value = ++PIM.Dropdowns[MMB.PlayerNumber].value;
			}
			else
			{
				PIM.Dropdowns[MMB.PlayerNumber].value = 0;
			}
			PIM.Dropdowns[MMB.PlayerNumber].RefreshShownValue();
		}
		else if (MMB.IsRefreshButton)
		{
			if (currentMenu == 25)
			{
				PIM.SetControllers();
				return;
			}
			GetMapFiles();
			deselectButton(MMB);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsUnlockables)
		{
			if (!AccountMaster.instance.isSignedIn)
			{
				StartCoroutine(DisplayExtrasNotification("You need to be signed in"));
				return;
			}
			deselectButton(MMB);
			enableMenu(13);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsAchievements)
		{
			if (!AccountMaster.instance.isSignedIn)
			{
				StartCoroutine(DisplayExtrasNotification("You need to be signed in"));
				return;
			}
			deselectButton(MMB);
			enableMenu(14);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsStatistics)
		{
			if (!AccountMaster.instance.isSignedIn)
			{
				StartCoroutine(DisplayExtrasNotification("You need to be signed in"));
				return;
			}
			deselectButton(MMB);
			enableMenu(15);
			StartCoroutine(doing());
			MMB.Selected = false;
		}
		else if (MMB.IsBack)
		{
			deselectButton(MMB);
			enableMenu(0);
			StartCoroutine(doing());
		}
		else if (MMB.IsBackPrevMenu)
		{
			deselectButton(MMB);
			enableMenu(PreviousMenu);
			StartCoroutine(doing());
		}
		else if (MMB.IsBackCustomMenu)
		{
			deselectButton(MMB);
			enableMenu(MMB.menuNumber);
			StartCoroutine(doing());
		}
		else if (MMB.IsExit)
		{
			Debug.LogError("buh bye");
			Application.Quit();
		}
		else if (MMB.IsVideo && !OptionsMainMenu.instance.inAndroid)
		{
			deselectButton(MMB);
			enableMenu(4);
			StartCoroutine(doing());
		}
		else if (MMB.IsGamePlay)
		{
			deselectButton(MMB);
			enableMenu(5);
			StartCoroutine(doing());
		}
		else if (MMB.IsControls)
		{
			ControlMapper component = GameObject.Find("ControlMapper").GetComponent<ControlMapper>();
			if ((bool)component)
			{
				component.Open();
			}
		}
		else if (MMB.IsAudio)
		{
			deselectButton(MMB);
			enableMenu(6);
			StartCoroutine(doing());
		}
		else if (MMB.IsBack2Menu)
		{
			deselectButton(MMB);
			enableMenu(1);
			StartCoroutine(doing());
		}
		else if (MMB.IsGoreMode)
		{
			PlayMenuChangeSound();
			if (!OptionsMainMenu.instance.BloodMode)
			{
				OptionsMainMenu.instance.BloodMode = true;
				GoreModeText.text = "(x)";
				OptionsMainMenu.instance.SaveNewData();
			}
			else
			{
				OptionsMainMenu.instance.BloodMode = false;
				GoreModeText.text = "( )";
				OptionsMainMenu.instance.SaveNewData();
			}
		}
		else if (MMB.IsExtraLivesDown)
		{
			PlayMenuChangeSound();
			TextMeshProUGUI component2 = GameObject.Find("AmountLives").GetComponent<TextMeshProUGUI>();
			if (OptionsMainMenu.instance.ExtraLives > 0)
			{
				OptionsMainMenu.instance.ExtraLives--;
				component2.text = OptionsMainMenu.instance.ExtraLives.ToString();
			}
		}
		else if (MMB.IsExtraLivesUp)
		{
			PlayMenuChangeSound();
			TextMeshProUGUI component3 = GameObject.Find("AmountLives").GetComponent<TextMeshProUGUI>();
			if (OptionsMainMenu.instance.ExtraLives < 9)
			{
				OptionsMainMenu.instance.ExtraLives++;
				component3.text = OptionsMainMenu.instance.ExtraLives.ToString();
			}
		}
		else if (MMB.IsGraphicsDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeGraphics(-1);
			SetGraphicsText();
		}
		else if (MMB.IsGraphicsUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeGraphics(1);
			SetGraphicsText();
		}
		else if (MMB.IsResolutionDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeResolution(-1, OptionsMainMenu.instance.isFullscreen);
			SetResolutionText();
		}
		else if (MMB.IsResolutionUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeResolution(1, OptionsMainMenu.instance.isFullscreen);
			SetResolutionText();
		}
		else if (MMB.IsFPSDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeFPS(-1);
			SetFPSText();
		}
		else if (MMB.IsFPSUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeFPS(1);
			SetFPSText();
		}
		else if (MMB.IsFullscreen)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeFullscreen();
			SetResolutionText();
		}
		else if (MMB.IsVsync)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.Vsync();
		}
		else if (MMB.IsPlayMap)
		{
			MapLoading = false;
			deselectButton(MMB);
			enableMenu(10);
			StartCoroutine(doing());
		}
		else if (MMB.IsCreateMap)
		{
			deselectButton(MMB);
			enableMenu(16);
			StartCoroutine(doing());
		}
		else if (MMB.IsSmallMap)
		{
			OptionsMainMenu.instance.MapEditorMapName = null;
			OptionsMainMenu.instance.MapSize = 180;
			TTL.CreateMap = true;
			StartCoroutine(LoadYourAsyncScene(3));
		}
		else if (MMB.IsNormalMap)
		{
			OptionsMainMenu.instance.MapEditorMapName = null;
			OptionsMainMenu.instance.MapSize = 285;
			TTL.CreateMap = true;
			StartCoroutine(LoadYourAsyncScene(3));
		}
		else if (MMB.IsBigMap)
		{
			OptionsMainMenu.instance.MapEditorMapName = null;
			OptionsMainMenu.instance.MapSize = 374;
			TTL.CreateMap = true;
			StartCoroutine(LoadYourAsyncScene(3));
		}
		else if (MMB.IsLargeMap)
		{
			OptionsMainMenu.instance.MapEditorMapName = null;
			OptionsMainMenu.instance.MapSize = 475;
			TTL.CreateMap = true;
			StartCoroutine(LoadYourAsyncScene(3));
		}
		else if (MMB.IsLoadMap)
		{
			MapLoading = true;
			deselectButton(MMB);
			enableMenu(10);
			StartCoroutine(doing());
		}
		else if (MMB.IsToTankeyTown)
		{
			TutorialMaster.instance.ShowTutorial("Tankey town has been disabled for now!");
			SFXManager.instance.PlaySFX(errorSound);
		}
		else if (MMB.IsSurvivalMode)
		{
			deselectButton(MMB);
			enableMenu(8);
			StartCoroutine(doing());
		}
		else if (MMB.IsSurvivalMap)
		{
			if (!MMB.canBeSelected)
			{
				SFXManager.instance.PlaySFX(errorSound);
				return;
			}
			Temp_MMB = MMB;
			Temp_scene = 2;
			Temp_startingLevel = MMB.SurvivalMapNumber;
			PIM.CanPlayWithAI = false;
			PIM.SetControllers();
			deselectButton(MMB);
			enableMenu(25);
			PIM.DisableDifficultySetter();
			StartCoroutine(doing());
		}
		else if (MMB.IsContinue && MMB.canBeSelected)
		{
			Temp_MMB = MMB;
			Temp_scene = 1;
			Temp_startingLevel = MMB.ContinueLevel;
			PIM.CanPlayWithAI = true;
			PIM.SetControllers();
			PIM.LoadData();
			PIM.DisableDifficultySetter();
			deselectButton(MMB);
			enableMenu(25);
			StartCoroutine(doing());
		}
		else
		{
			if (!MMB.StartMatchButton)
			{
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				bool flag = false;
				for (int k = 0; k < ReInput.controllers.GetControllers(ControllerType.Joystick).Length; k++)
				{
					if (PIM.Dropdowns[j].captionText.text == ReInput.controllers.GetController(ControllerType.Joystick, k).name)
					{
						Debug.Log("FOUND ONE!!!: " + ReInput.controllers.GetController(ControllerType.Joystick, k).name);
						ReInput.players.GetPlayer(j).controllers.AddController(ReInput.controllers.GetController(ControllerType.Joystick, k), removeFromOtherPlayers: true);
						flag = true;
						OptionsMainMenu.instance.PlayerJoined[j] = true;
					}
				}
				if (!flag)
				{
					if (j == 0)
					{
						ReInput.players.GetPlayer(j).controllers.ClearAllControllers();
						ReInput.players.GetPlayer(j).controllers.AddController(ReInput.controllers.GetController(ControllerType.Keyboard, 0), removeFromOtherPlayers: true);
						ReInput.players.GetPlayer(j).controllers.AddController(ReInput.controllers.GetController(ControllerType.Mouse, 0), removeFromOtherPlayers: true);
						OptionsMainMenu.instance.PlayerJoined[j] = true;
					}
					else if (PIM.Dropdowns[j].captionText.text.Contains("AI"))
					{
						OptionsMainMenu.instance.AIcompanion[j] = true;
						OptionsMainMenu.instance.PlayerJoined[j] = false;
					}
					else
					{
						OptionsMainMenu.instance.PlayerJoined[j] = false;
					}
				}
			}
			if (Temp_scene == 100)
			{
				OnMapStart();
			}
			else if (Temp_scene == 1)
			{
				PIM.CanPlayWithAI = true;
				if (Temp_startingLevel == 0 && Temp_scene == 1)
				{
					OptionsMainMenu.instance.StartLevel = 0;
					StartCoroutine(LoadYourAsyncScene(1));
					TTL.ContinueCheckpoint = 0;
					TTL.ClassicCampaign = true;
				}
				else
				{
					LoadLevel(Temp_scene, Temp_MMB, Temp_startingLevel);
				}
			}
			else if (Temp_scene == 2)
			{
				PIM.CanPlayWithAI = false;
				LoadLevel(Temp_scene, Temp_MMB, Temp_startingLevel);
			}
		}
	}

	private void PlayMenuChangeSound()
	{
		SFXManager.instance.PlaySFX(MenuChange);
	}

	private void LoadLevel(int scene, MainMenuButtons MMB, int startingLevel)
	{
		OptionsMainMenu.instance.StartLevel = startingLevel;
		StartCoroutine(LoadYourAsyncScene(scene));
	}

	public IEnumerator LoadYourAsyncScene(int lvlNumber)
	{
		GameObject[] menus = Menus;
		foreach (GameObject gameObject in menus)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		LIS_parent.gameObject.SetActive(value: true);
		LIS.gameObject.SetActive(value: true);
		LIS.Play = true;
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(lvlNumber);
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

	private IEnumerator DisplayExtrasNotification(string text)
	{
		ExtrasNotification.text = text;
		ExtrasNotification.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(2f);
		ExtrasNotification.text = "";
		ExtrasNotification.gameObject.SetActive(value: false);
	}

	public void SetGraphicsText()
	{
		Graphics_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentGraphicSettings);
	}

	public void SetDifficultyText()
	{
		Debug.Log("SETTING DA TING " + OptionsMainMenu.instance.currentDifficulty);
		Difficulty_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
		Difficulty_list_campaign.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
	}

	public void SetGraphics()
	{
		Debug.LogError(Graphics_list.value);
		OptionsMainMenu.instance.ChangeGraphics(Graphics_list.value);
	}

	public void SetDifficulty()
	{
		OptionsMainMenu.instance.ChangeDifficulty(Difficulty_list.value);
		Difficulty_list_campaign.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
	}

	public void SetDifficultyFromCampaign()
	{
		OptionsMainMenu.instance.ChangeDifficulty(Difficulty_list_campaign.value);
		Difficulty_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
	}

	public void SetVSYNC()
	{
		OptionsMainMenu.instance.Vsync();
	}

	public void SetDisableSnow()
	{
		OptionsMainMenu.instance.SnowMode = !OptionsMainMenu.instance.SnowMode;
		OptionsMainMenu.instance.SaveNewData();
	}

	public void SetFriendlyFire()
	{
		OptionsMainMenu.instance.FriendlyFire = !OptionsMainMenu.instance.FriendlyFire;
		OptionsMainMenu.instance.SaveNewData();
	}

	public void SetMarkedTanks()
	{
		OptionsMainMenu.instance.MarkedTanks = !OptionsMainMenu.instance.MarkedTanks;
		OptionsMainMenu.instance.SaveNewData();
	}

	public void SetXray()
	{
		OptionsMainMenu.instance.showxraybullets = !OptionsMainMenu.instance.showxraybullets;
		OptionsMainMenu.instance.SaveNewData();
	}

	public void SetFullscreen()
	{
		OptionsMainMenu.instance.ChangeFullscreen();
	}

	public void SetResolutionText()
	{
		Debug.Log("SETTED RESOLUTION");
		Resolution_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentResolutionSettings);
	}

	public void SetAudio(TMP_Dropdown List)
	{
		if (List.name == "0")
		{
			OptionsMainMenu.instance.masterVolumeLvl = List.value;
		}
		else if (List.name == "2")
		{
			OptionsMainMenu.instance.musicVolumeLvl = List.value;
		}
		else if (List.name == "1")
		{
			OptionsMainMenu.instance.sfxVolumeLvl = List.value;
		}
		OptionsMainMenu.instance.SaveNewData();
	}

	public void SetAudioText()
	{
		MasterVolume_list.SetValueWithoutNotify(OptionsMainMenu.instance.masterVolumeLvl);
		MusicVolume_list.SetValueWithoutNotify(OptionsMainMenu.instance.musicVolumeLvl);
		SFXVolume_list.SetValueWithoutNotify(OptionsMainMenu.instance.sfxVolumeLvl);
	}

	public void SetFPS()
	{
		OptionsMainMenu.instance.ChangeFPS(FPS_list.value);
	}

	public void SetResolution()
	{
		OptionsMainMenu.instance.ChangeResolution(Resolution_list.value, OptionsMainMenu.instance.isFullscreen);
	}

	public void SetFPSText()
	{
		FPS_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentFPSSettings);
	}

	public void enableMenu(int menunumber)
	{
		CreateAccountNotificationText.gameObject.SetActive(value: false);
		SignInNotificationText.gameObject.SetActive(value: false);
		string text = ((OptionsMainMenu.instance.currentDifficulty == 0) ? "Toddler" : ((OptionsMainMenu.instance.currentDifficulty == 1) ? "Kid" : ((OptionsMainMenu.instance.currentDifficulty == 2) ? "Adult" : "Grandpa")));
		DifficultyExplainText.text = "You are playing on the " + text + " difficulty";
		StartCoroutine(MenuTransition(menunumber));
	}

	private IEnumerator MenuTransition(int menunumber)
	{
		Selection = 0;
		SFXManager.instance.PlaySFX(MenuSwitch);
		float t2 = 0f;
		CanvasGroup CG2 = Menus[currentMenu].GetComponent<CanvasGroup>();
		MainMenuButtons[] MMBs2 = Menus[currentMenu].GetComponentsInChildren<MainMenuButtons>();
		MainMenuButtons[] array = MMBs2;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SwitchedMenu();
		}
		if ((bool)CG2)
		{
			while (t2 < 1f)
			{
				t2 += Time.deltaTime * 5f;
				CG2.alpha = 1f - t2;
				yield return null;
			}
		}
		array = MMBs2;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SwitchedMenu();
		}
		GameObject[] menus = Menus;
		foreach (GameObject gameObject in menus)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		lastKnownPlaces[currentMenu] = Selection;
		if (currentMenu != menunumber)
		{
			PreviousMenu = currentMenu;
		}
		currentMenu = menunumber;
		Selection = lastKnownPlaces[menunumber];
		Menus[menunumber].SetActive(value: true);
		MMBs2 = Menus[currentMenu].GetComponentsInChildren<MainMenuButtons>();
		array = MMBs2;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LoadButton();
		}
		t2 = 0f;
		CG2 = Menus[currentMenu].GetComponent<CanvasGroup>();
		if ((bool)CG2)
		{
			while (t2 < 1f)
			{
				t2 = (CG2.alpha = t2 + Time.deltaTime * 5f);
				yield return null;
			}
			CG2.alpha = 1f;
		}
		if (menunumber == 2 || menunumber == 25)
		{
			SetDifficultyText();
		}
	}
}
