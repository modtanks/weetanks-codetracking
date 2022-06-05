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

	public TextMeshProUGUI VersionNumber;

	public TextMeshProUGUI ExtrasNotification;

	public TextMeshProUGUI SignedInText;

	[Header("Online")]
	public TMP_InputField Create_AccountNameInput;

	public TMP_InputField Create_PasswordInput;

	public TMP_InputField Create_PasswordInputCheck;

	public TMP_InputField Login_AccountNameInput;

	public TMP_InputField Login_PasswordInput;

	public TMP_InputField NewPassword_PasswordInput;

	public TMP_InputField NewPassword_PasswordInputCheck;

	public TextMeshProUGUI CenterText;

	public TextMeshProUGUI SignInNotificationText;

	public TextMeshProUGUI CreateAccountNotificationText;

	public TextMeshProUGUI SetNewPasswordNotificationText;

	public GameObject DailyRewardPrefab;

	public GameObject DailyRewardParent;

	public GameObject OnlineMapPrefab;

	public GameObject OnlineMyMapPrefab;

	public GameObject RewardsButton;

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

	private bool FirstTransition = true;

	private IEnumerator PlayJingle()
	{
		yield return new WaitForSeconds(0.4f);
		int num = UnityEngine.Random.Range(0, Jingles.Length);
		SFXManager.instance.PlaySFX(Jingles[num]);
	}

	private void Awake()
	{
		Time.timeScale = 1f;
		VersionNumber.text = OptionsMainMenu.instance.CurrentVersion;
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
			Debug.Log("MapFiles FOUND!" + files.Length);
			AmountMaps = 0;
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
				if (mapEditorData.VersionCreated != "v0.7.12" && mapEditorData.VersionCreated != "v0.7.11" && mapEditorData.VersionCreated != "v0.7.10" && mapEditorData.VersionCreated != "v0.8.0e" && mapEditorData.VersionCreated != "v0.8.0d" && mapEditorData.VersionCreated != "v0.8.0c")
				{
					_ = mapEditorData.VersionCreated != "v0.8.0b";
				}
				campaignItemScript = gameObject.GetComponent<CampaignItemScript>();
				AmountMaps++;
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
				catch (Exception ex)
				{
					Debug.Log("no map size found! " + ex);
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
			Debug.Log("NO MAPS FOUND! " + text);
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
		UpdateMenuSignedInText();
		yield return new WaitForSeconds(2f);
		bool assignedInventory = false;
		if (AccountMaster.instance.Inventory != null && AccountMaster.instance.Inventory.InventoryItems != null)
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
		SetGraphicsText();
	}

	private void AssignInventory()
	{
		if (AccountMaster.instance.Inventory == null)
		{
			Debug.LogError("NO inventory object");
			return;
		}
		for (int i = 0; i < AccountMaster.instance.Inventory.InventoryItems.Count; i++)
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
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", key);
		wWWForm.AddField("userid", userid);
		wWWForm.AddField("username", name);
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			AccountMaster.instance.isSignedIn = false;
			UpdateMenuSignedInText();
		}
		else
		{
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
		uwr.Dispose();
	}

	public IEnumerator SetNewPassword(string password)
	{
		CenterText.text = "Setting password...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("password", password);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/set_new_password.php", wWWForm);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("done"))
			{
				CenterText.text = "Password set!";
				CenterText.gameObject.SetActive(value: true);
				yield return new WaitForSeconds(2f);
				CenterText.gameObject.SetActive(value: false);
				enableMenu(18);
				AccountMaster.instance.CanSetNewPassword = false;
			}
			else
			{
				CenterText.text = "Failed!";
				CenterText.gameObject.SetActive(value: true);
				yield return new WaitForSeconds(2f);
				CenterText.gameObject.SetActive(value: false);
				enableMenu(18);
			}
		}
		uwr.Dispose();
	}

	private IEnumerator CreateAccount(string url, string username, string password)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", username);
		wWWForm.AddField("password", password);
		wWWForm.AddField("userData", JsonUtility.ToJson(GameMaster.instance.CurrentData));
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		uwr.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		uwr.SetRequestHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
		uwr.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
		uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");
		CenterText.text = "Creating account...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
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
		uwr.Dispose();
	}

	private IEnumerator SignIn(string url, string username, string password)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", username);
		wWWForm.AddField("password", password);
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
		CenterText.text = "Signing in...";
		CenterText.gameObject.SetActive(value: true);
		Menus[currentMenu].SetActive(value: false);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
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
				AccountMaster.instance.SteamUserID = ulong.Parse(splitArray[3]);
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
		uwr.Dispose();
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
			if (custommessage.Contains("already"))
			{
				TransferAccountText.text = custommessage + " Would you like to overwrite?";
				TransferButtons[0].SetActive(value: true);
				TransferButtons[1].SetActive(value: true);
				TransferButtons[2].SetActive(value: true);
				TransferButtons[2].GetComponent<TextMeshProUGUI>().text = "NOTE: If you have no password set for the old account, the data will be lost.";
				yield break;
			}
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
		if (MMB == null || !CanDoSomething)
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
				CreateAccountNotificationText.text = "Error: Name has to be atleast 3 long";
				return;
			}
			string text = PasswordCheck(Create_PasswordInput.text, Create_PasswordInputCheck.text);
			if (text.Contains("Error"))
			{
				CreateAccountNotificationText.gameObject.SetActive(value: true);
				CreateAccountNotificationText.text = text;
				return;
			}
			Debug.Log("Creating account");
			StartCoroutine(CreateAccount("https://www.weetanks.com/create_account.php", Create_AccountNameInput.text, Create_PasswordInput.text));
		}
		else if (MMB.IsSignIn && CanDoSomething)
		{
			TransferButtons[2].GetComponent<TextMeshProUGUI>().text = "NOTE: Wee Tanks will auto detect your Steam account, and sync your data";
			if (Login_AccountNameInput.text.Length < 3)
			{
				SignInNotificationText.gameObject.SetActive(value: true);
				SignInNotificationText.text = "Error: Name has to be at least 3 long";
				return;
			}
			if (Login_PasswordInput.text.Length < 5)
			{
				SignInNotificationText.gameObject.SetActive(value: true);
				SignInNotificationText.text = "Error: Password has to be at least 6 long";
				return;
			}
			Debug.Log("Loggin in account");
			StartCoroutine(SignIn("https://www.weetanks.com/signin_account.php", Login_AccountNameInput.text, Login_PasswordInput.text));
		}
		else if (MMB.IsSetNewPassword && AccountMaster.instance.CanSetNewPassword)
		{
			string text2 = PasswordCheck(NewPassword_PasswordInput.text, NewPassword_PasswordInputCheck.text);
			if (text2.Contains("Error"))
			{
				SetNewPasswordNotificationText.gameObject.SetActive(value: true);
				SetNewPasswordNotificationText.text = text2;
				return;
			}
			StartCoroutine(SetNewPassword(NewPassword_PasswordInput.text));
		}
		else if (MMB.IsSignOut)
		{
			StartCoroutine(SignMeOut("https://www.weetanks.com/signout_account.php", AccountMaster.instance.Username, AccountMaster.instance.Key, AccountMaster.instance.UserID));
		}
		else if (MMB.IsOpenCampaignsMenu)
		{
			deselectButton(MMB);
			enableMenu(21);
		}
		else if (MMB.IsOptions)
		{
			deselectButton(MMB);
			enableMenu(1);
		}
		else if (MMB.IsRewards)
		{
			OptionsMainMenu.instance.LastKnownDaysInARow = AccountMaster.instance.PDO.DaysLogInARow;
			if (DailyRewardParent.transform.childCount < 20)
			{
				for (int i = 0; i < 100; i++)
				{
					GameObject obj = UnityEngine.Object.Instantiate(DailyRewardPrefab);
					obj.transform.SetParent(DailyRewardParent.transform);
					DailyRewardItem component = obj.GetComponent<DailyRewardItem>();
					MainMenuButtons component2 = component.GetComponent<MainMenuButtons>();
					component.DayTitle.text = "Day #" + (i + 1);
					float num = 0f;
					float num2 = 0f;
					switch (i)
					{
					case 9:
						num = 10f;
						num2 = 100f;
						break;
					case 29:
						num = 30f;
						num2 = 300f;
						break;
					case 49:
						num = 50f;
						num2 = 500f;
						break;
					case 69:
						num = 70f;
						num2 = 700f;
						break;
					case 99:
						num = 100f;
						num2 = 1000f;
						break;
					}
					component.MarbleText.text = "+" + Mathf.Floor(Mathf.Pow((float)(i + 1) / 0.01f, 0.3f)) + " Marbles";
					component.XPText.text = "+" + Mathf.Floor(Mathf.Pow((float)(i + 1) / 0.01f, 0.3f)) * 5f + " XP";
					if (num > 0f)
					{
						TextMeshProUGUI marbleText = component.MarbleText;
						marbleText.text = marbleText.text + "\n+" + num + " Marbles";
					}
					if (num2 > 0f)
					{
						TextMeshProUGUI xPText = component.XPText;
						xPText.text = xPText.text + "\n+" + num2 + " XP";
					}
					if (i < OptionsMainMenu.instance.LastKnownDaysInARow)
					{
						Color color = component2.AvailableColor;
						if (num > 0f)
						{
							color = Color.yellow;
						}
						component.DayTitle.color = color;
						component.MarbleText.color = color;
						component.XPText.color = color;
						component.CheckMark.SetActive(value: true);
					}
					else
					{
						Color color2 = component2.unavailableColor;
						if (num > 0f)
						{
							color2 = Color.yellow;
						}
						component.DayTitle.color = color2;
						component.MarbleText.color = component2.unavailableColor;
						component.XPText.color = component2.unavailableColor;
						component.CheckMark.SetActive(value: false);
					}
				}
			}
			OptionsMainMenu.instance.SaveNewData();
			deselectButton(MMB);
			enableMenu(28);
		}
		else if (MMB.IsStats)
		{
			deselectButton(MMB);
			enableMenu(7);
		}
		else
		{
			if (MMB.IsCredits)
			{
				Debug.LogWarning("CREDITS TIME!");
				deselectButton(MMB);
				StartCoroutine(LoadYourAsyncScene(5));
				return;
			}
			if (MMB.IsContinueClassicCampaign)
			{
				deselectButton(MMB);
				enableMenu(3);
			}
			else if (MMB.IsClassicCampaign)
			{
				deselectButton(MMB);
				enableMenu(2);
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
				MMB.Selected = false;
			}
			else if (MMB.IsAcceptTransferAccount)
			{
				if (TransferAccountText.text.Contains("overwrite"))
				{
					GameObject[] transferButtons = TransferButtons;
					for (int j = 0; j < transferButtons.Length; j++)
					{
						transferButtons[j].SetActive(value: false);
					}
					TransferAccountText.text = "setting new account data...";
					AccountMaster.instance.StartCoroutine(AccountMaster.instance.TransferAccountToSteam("https://www.weetanks.com/overwrite_to_steam_account.php"));
				}
				else if (AccountMaster.instance.isSignedIn)
				{
					GameObject[] transferButtons = TransferButtons;
					for (int j = 0; j < transferButtons.Length; j++)
					{
						transferButtons[j].SetActive(value: false);
					}
					TransferAccountText.text = "transferring...";
					AccountMaster.instance.StartCoroutine(AccountMaster.instance.TransferAccountToSteam("https://www.weetanks.com/transfer_to_steam_account.php"));
				}
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
				MMB.Selected = false;
			}
			else if (MMB.IsUnlockables)
			{
				if (!AccountMaster.instance.isSignedIn)
				{
					StartCoroutine(DisplayExtrasNotification("You need to be signed in"));
					return;
				}
				if (UnlockableParent.transform.childCount < 2)
				{
					for (int k = 0; k < OptionsMainMenu.instance.UIs.Length; k++)
					{
						GameObject obj2 = UnityEngine.Object.Instantiate(UnlockablePrefab);
						obj2.transform.SetParent(UnlockableParent.transform);
						obj2.GetComponent<UnlockableScript>().ULID = OptionsMainMenu.instance.UIs[k].ULID;
					}
				}
				deselectButton(MMB);
				enableMenu(13);
				MMB.Selected = false;
			}
			else if (MMB.IsAchievements)
			{
				if (!AccountMaster.instance.isSignedIn)
				{
					StartCoroutine(DisplayExtrasNotification("You need to be signed in"));
					return;
				}
				if (AchievementParent.transform.childCount < 2)
				{
					for (int l = 0; l < 50; l++)
					{
						if (OptionsMainMenu.instance.AMnames[l] != "")
						{
							GameObject obj3 = UnityEngine.Object.Instantiate(AchievementPrefab);
							obj3.transform.SetParent(AchievementParent.transform);
							obj3.GetComponent<AchievementItemScript>().AMID = l;
						}
					}
				}
				deselectButton(MMB);
				enableMenu(14);
				MMB.Selected = false;
			}
			else if (MMB.IsStatistics)
			{
				if (!AccountMaster.instance.isSignedIn)
				{
					StartCoroutine(DisplayExtrasNotification("You need to be signed in"));
					return;
				}
				int num3 = 0;
				if (TankKillItemParent.transform.childCount < 2)
				{
					for (int m = 0; m < GameMaster.instance.TankColorKilled.Count; m++)
					{
						GameObject obj4 = UnityEngine.Object.Instantiate(TankKillItemPrefab);
						obj4.transform.SetParent(TankKillItemParent.transform);
						obj4.GetComponent<TankStatsItem>().myMenu = 1;
						obj4.GetComponent<TankStatsItem>().myStatID = m;
						obj4.GetComponent<TankStatsItem>().NMC = this;
						obj4.GetComponent<TankStatsItem>().originalParent = TankKillItemParent.transform;
						if (GameMaster.instance.TankColorKilled[m] > 0)
						{
							num3++;
						}
					}
					_ = 0;
					for (int n = 0; n < 5; n++)
					{
						GameObject obj5 = UnityEngine.Object.Instantiate(TankKillItemPrefab);
						obj5.transform.SetParent(TankKillItemParent.transform);
						obj5.GetComponent<TankStatsItem>().myMenu = 0;
						obj5.GetComponent<TankStatsItem>().myStatID = n;
						obj5.GetComponent<TankStatsItem>().NMC = this;
						obj5.GetComponent<TankStatsItem>().originalParent = TankKillItemParent.transform;
					}
					for (int num4 = 0; num4 < 7; num4++)
					{
						GameObject obj6 = UnityEngine.Object.Instantiate(TankKillItemPrefab);
						obj6.transform.SetParent(TankKillItemParent.transform);
						obj6.GetComponent<TankStatsItem>().myMenu = 2;
						obj6.GetComponent<TankStatsItem>().myStatID = num4;
						obj6.GetComponent<TankStatsItem>().NMC = this;
						obj6.GetComponent<TankStatsItem>().originalParent = TankKillItemParent.transform;
					}
				}
				deselectButton(MMB);
				enableMenu(15);
				MMB.Selected = false;
			}
			else if (MMB.IsBack)
			{
				deselectButton(MMB);
				enableMenu(0);
			}
			else if (MMB.IsBackPrevMenu)
			{
				deselectButton(MMB);
				enableMenu(PreviousMenu);
			}
			else if (MMB.IsBackCustomMenu)
			{
				deselectButton(MMB);
				enableMenu(MMB.menuNumber);
			}
			else if (MMB.IsExit)
			{
				Debug.Log("buh bye");
				Application.Quit();
			}
			else if (MMB.IsVideo && !OptionsMainMenu.instance.inAndroid)
			{
				deselectButton(MMB);
				enableMenu(4);
			}
			else if (MMB.IsGamePlay)
			{
				deselectButton(MMB);
				enableMenu(5);
			}
			else if (MMB.IsControls)
			{
				ControlMapper component3 = GameObject.Find("ControlMapper").GetComponent<ControlMapper>();
				if ((bool)component3)
				{
					component3.Open();
				}
			}
			else if (MMB.IsAudio)
			{
				deselectButton(MMB);
				enableMenu(6);
			}
			else if (MMB.IsBack2Menu)
			{
				deselectButton(MMB);
				enableMenu(1);
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
			}
			else if (MMB.IsCreateMap)
			{
				deselectButton(MMB);
				enableMenu(16);
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
			}
			else if (MMB.IsToTankeyTown)
			{
				StartCoroutine(LoadYourAsyncScene(6));
			}
			else if (MMB.IsSurvivalMode)
			{
				deselectButton(MMB);
				enableMenu(8);
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
			}
			else if (MMB.StartMatchButton)
			{
				Debug.Log("Starting Match..");
				for (int num5 = 0; num5 < 4; num5++)
				{
					bool flag = false;
					for (int num6 = 0; num6 < ReInput.controllers.GetControllers(ControllerType.Joystick).Length; num6++)
					{
						if (PIM.Dropdowns[num5].captionText.text == ReInput.controllers.GetController(ControllerType.Joystick, num6).name)
						{
							Debug.Log("FOUND ONE!!!: " + ReInput.controllers.GetController(ControllerType.Joystick, num6).name);
							ReInput.players.GetPlayer(num5).controllers.AddController(ReInput.controllers.GetController(ControllerType.Joystick, num6), removeFromOtherPlayers: true);
							flag = true;
							OptionsMainMenu.instance.PlayerJoined[num5] = true;
						}
					}
					if (!flag)
					{
						if (num5 == 0)
						{
							ReInput.players.GetPlayer(num5).controllers.ClearAllControllers();
							ReInput.players.GetPlayer(num5).controllers.AddController(ReInput.controllers.GetController(ControllerType.Keyboard, 0), removeFromOtherPlayers: true);
							ReInput.players.GetPlayer(num5).controllers.AddController(ReInput.controllers.GetController(ControllerType.Mouse, 0), removeFromOtherPlayers: true);
							OptionsMainMenu.instance.PlayerJoined[num5] = true;
						}
						else if (PIM.Dropdowns[num5].captionText.text.Contains("AI"))
						{
							OptionsMainMenu.instance.AIcompanion[num5] = true;
							OptionsMainMenu.instance.PlayerJoined[num5] = false;
						}
						else
						{
							OptionsMainMenu.instance.PlayerJoined[num5] = false;
						}
					}
				}
				Debug.Log("Controllers set..");
				if (Temp_scene == 100)
				{
					OnMapStart();
					return;
				}
				if (Temp_scene == 1)
				{
					Debug.Log("Starting game scene 1");
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
						Debug.Log("Starting game scene 1 0");
						LoadLevel(Temp_scene, Temp_MMB, Temp_startingLevel);
					}
				}
				else if (Temp_scene == 2)
				{
					Debug.Log("Starting game scene 2");
					PIM.CanPlayWithAI = false;
					LoadLevel(Temp_scene, Temp_MMB, Temp_startingLevel);
				}
			}
		}
		StartCoroutine(doing());
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
		if (LIS.Play)
		{
			yield break;
		}
		Debug.Log("loading new level: " + lvlNumber);
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
		Difficulty_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
		Difficulty_list_campaign.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
	}

	public void SetGraphics()
	{
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
		if (menunumber == 4)
		{
			SetGraphicsText();
		}
		if (menunumber == 0 && !FirstTransition)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				RewardsButton.SetActive(value: true);
			}
			else
			{
				RewardsButton.SetActive(value: false);
			}
		}
		FirstTransition = false;
	}

	public string PasswordCheck(string one, string two)
	{
		if (one.Length < 6)
		{
			return "Error: has to be at least 6 long";
		}
		if (one.Contains("/") || one.Contains("$") || one.Contains("{") || one.Contains("}") || one.Contains(",") || one.Contains(".") || one.Contains("'"))
		{
			return "Error: invalid characters";
		}
		if (one != two)
		{
			return "Error: Passwords not the same";
		}
		return "jippie";
	}
}
