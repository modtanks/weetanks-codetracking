using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rewired;
using Rewired.UI.ControlMapper;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
	public Canvas myCanvas;

	public TextMeshProUGUI TanksLeft_Text;

	public GameObject TanksLeftField;

	public int currentMenu;

	public List<int> menuAmountOptions = new List<int>();

	public GameObject[] Menus;

	public GameObject[] HideOnSurvivalMode;

	public GameObject[] HideOnMapEditor;

	public GameObject[] HideOnClassicCampaign;

	public GameObject[] HideOnNormal;

	public int Selection;

	public TextMeshProUGUI[] MenuTexts;

	public AudioClip MenuClick;

	public AudioClip MenuSelect;

	public AudioClip MarkerSound;

	public AudioClip MenuSwitch;

	public AudioClip MenuChange;

	public AudioClip ErrorSound;

	public AudioClip SuccesSound;

	public LoadingIconScript LIS;

	[Header("Map Editor Stuff")]
	public GameObject NormalButtonsSavingMap;

	public GameObject ReplaceButtonsSavingMap;

	public GameObject NormalQuitButton;

	public GameObject StopTestingButton;

	public GameObject NormalQuitButtonMarker;

	public GameObject StopTestingButtonMarker;

	public GameObject ApproveQuitObject;

	public TMP_InputField campaignNameInput;

	public TextMeshProUGUI errorCampaignInputText;

	public ButtonMouseEvents SignMapToggle;

	public TMP_InputField SignMapInput;

	public GameObject SigningTheMap;

	public CountDownScript CDS;

	public bool doSomething = true;

	private Vector2 input;

	private bool LoadingScene;

	public TextMeshProUGUI CheckpointText;

	public TextMeshProUGUI Graphicstext;

	public TextMeshProUGUI Resolutiontext;

	public TextMeshProUGUI Fullscreentext;

	public TextMeshProUGUI MasterVolumetext;

	public TextMeshProUGUI MusicVolumetext;

	public TextMeshProUGUI SFXVolumetext;

	public TextMeshProUGUI VideoOptionstext;

	private MainMenuButtons currentScript;

	public Player player;

	[Header("Settings Inputs")]
	public TMP_Dropdown Graphics_list;

	public TMP_Dropdown Resolution_list;

	public ButtonMouseEvents Fullscreen_toggle;

	public TMP_Dropdown FPS_list;

	public TMP_Dropdown MasterVolume_list;

	public TMP_Dropdown MusicVolume_list;

	public TMP_Dropdown SFXVolume_list;

	public ButtonMouseEvents FriendlyFire_toggle;

	public ButtonMouseEvents vsync_toggle;

	private bool pausedMusicScript;

	public float musicLvlBefore;

	private void Start()
	{
		player = ReInput.players.GetPlayer(0);
		if ((bool)MapEditorMaster.instance && errorCampaignInputText != null && MapEditorMaster.instance.IsPublished != 1)
		{
			errorCampaignInputText.color = Color.clear;
		}
		myCanvas = GetComponent<Canvas>();
		GameObject[] menus = Menus;
		foreach (GameObject gameObject in menus)
		{
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
		if ((bool)ZombieTankSpawner.instance)
		{
			menus = HideOnSurvivalMode;
			for (int i = 0; i < menus.Length; i++)
			{
				menus[i].SetActive(value: false);
			}
		}
		else if (GameMaster.instance.isOfficialCampaign)
		{
			menus = HideOnClassicCampaign;
			for (int i = 0; i < menus.Length; i++)
			{
				menus[i].SetActive(value: false);
			}
		}
		else if ((bool)MapEditorMaster.instance)
		{
			if (MapEditorMaster.instance.inPlayingMode)
			{
				menus = HideOnClassicCampaign;
				for (int i = 0; i < menus.Length; i++)
				{
					menus[i].SetActive(value: false);
				}
			}
			else
			{
				menus = HideOnMapEditor;
				for (int i = 0; i < menus.Length; i++)
				{
					menus[i].SetActive(value: false);
				}
			}
		}
		else
		{
			menus = HideOnNormal;
			for (int i = 0; i < menus.Length; i++)
			{
				menus[i].SetActive(value: false);
			}
		}
		if (GameMaster.instance.inMapEditor && MapEditorMaster.instance.signedName != "")
		{
			SigningTheMap.SetActive(value: false);
		}
		Fullscreen_toggle.IsEnabled = OptionsMainMenu.instance.isFullscreen;
		vsync_toggle.IsEnabled = OptionsMainMenu.instance.vsync;
		if (OptionsMainMenu.instance.inAndroid)
		{
			VideoOptionstext.color = Color.gray;
		}
		SetGraphicsText();
		SetResolutionText();
		if (GameMaster.instance.isZombieMode)
		{
			TanksLeftField.SetActive(value: false);
		}
		ResumeGame();
		if ((bool)CheckpointText)
		{
			CheckpointText.text = LocalizationMaster.instance.GetText("MM_Checkpoint") + " " + OptionsMainMenu.instance.StartLevel;
		}
		SetAudioText();
		SetGraphicsText();
		SetResolutionText();
		SetFPSText();
		enableMenu(0);
	}

	private void Update()
	{
		if ((bool)CheckpointText && GameMaster.instance.CurrentMission > OptionsMainMenu.instance.StartLevel + 9)
		{
			OptionsMainMenu.instance.StartLevel += 10;
			CheckpointText.text = LocalizationMaster.instance.GetText("MM_Checkpoint") + " " + OptionsMainMenu.instance.StartLevel;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < ReInput.players.playerCount; i++)
		{
			Player player = ReInput.players.GetPlayer(i);
			if ((player.isPlaying && GameMaster.instance.PlayerJoined[i]) || (bool)MapEditorMaster.instance)
			{
				input.x = player.GetAxis("Move Horizontal");
				input.y = player.GetAxis("Move Vertically");
				flag = player.GetButtonUp("Menu Use");
				flag2 = player.GetButtonDown("Escape");
				if (input.y < 0f || input.y > 0f || flag || flag2)
				{
					break;
				}
			}
		}
		if (GameMaster.instance.GameHasPaused)
		{
			if ((bool)GameMaster.instance && (bool)TanksLeft_Text && (bool)LocalizationMaster.instance)
			{
				TanksLeft_Text.text = LocalizationMaster.instance.GetText("HUD_tanks_left") + " " + GameMaster.instance.Lives;
			}
			MainMenuButtons[] array = UnityEngine.Object.FindObjectsOfType(typeof(MainMenuButtons)) as MainMenuButtons[];
			foreach (MainMenuButtons mainMenuButtons in array)
			{
				if (mainMenuButtons.Place == Selection && mainMenuButtons.inMenu == currentMenu)
				{
					if (!mainMenuButtons.Selected)
					{
						currentScript = mainMenuButtons;
						mainMenuButtons.Selected = true;
						mainMenuButtons.startTime = Time.unscaledDeltaTime;
					}
				}
				else if (mainMenuButtons.Selected)
				{
					mainMenuButtons.Selected = false;
					mainMenuButtons.startTime = Time.unscaledDeltaTime;
				}
			}
		}
		if (doSomething && GameMaster.instance.GameHasPaused)
		{
			if (input.y < -0.5f)
			{
				if (Selection < menuAmountOptions[currentMenu])
				{
					Selection++;
					Debug.LogError("GOING DOWN");
					StartCoroutine("doing");
				}
			}
			else if (input.y > 0.5f && Selection > 0)
			{
				Selection--;
				Debug.LogError("GOING UP");
				StartCoroutine("doing");
			}
			if (flag && currentMenu != 5)
			{
				doButton(currentScript);
			}
		}
		if ((bool)MapEditorMaster.instance && !MapEditorMaster.instance.inPlayingMode)
		{
			if (MapEditorMaster.instance.isTesting)
			{
				NormalQuitButton.SetActive(value: false);
				StopTestingButton.SetActive(value: true);
			}
			else if (!myCanvas.enabled)
			{
				NormalQuitButton.SetActive(value: true);
				StopTestingButton.SetActive(value: false);
			}
		}
		if ((bool)SteamTest.instance && SteamTest.instance.SteamOverlayActive && !myCanvas.enabled)
		{
			PauseGame();
		}
		if (flag2)
		{
			_ = GameMaster.instance.inMapEditor;
			if (myCanvas.enabled && !LoadingScene)
			{
				ResumeGame();
			}
			else if ((bool)CDS)
			{
				if (!CDS.start)
				{
					Debug.LogWarning("Menu opening!");
				}
				PauseGame();
			}
			else
			{
				PauseGame();
			}
		}
		else
		{
			_ = (bool)CDS;
		}
	}

	public IEnumerator UploadCampaign(MapEditorData MED)
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			StartCoroutine(ShowCampaignInputError("File saved! (but upload failed)", Color.green));
			SFXManager.instance.PlaySFX(SuccesSound);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			StartCoroutine(ShowCampaignInputError("File saved! (but upload failed)", Color.green));
			SFXManager.instance.PlaySFX(SuccesSound);
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		_ = Application.persistentDataPath + "/";
		string text2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		text2 = text2 + "/My Games/Wee Tanks/" + MED.campaignName + ".campaign";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignName", MED.campaignName);
		wWWForm.AddField("campaignVersion", MED.VersionCreated);
		wWWForm.AddField("campaignID", MED.PID);
		wWWForm.AddField("campaignSize", MED.MapSize);
		wWWForm.AddField("campaignMissions", MED.missionAmount);
		wWWForm.AddField("campaignDifficulty", MED.difficulty);
		wWWForm.AddBinaryData("file", File.ReadAllBytes(text2), MED.campaignName + ".campaign", "text/plain");
		Debug.Log("SENDING NOW");
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_map.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			StartCoroutine(ShowCampaignInputError("File saved! (but upload failed)", Color.green));
			SFXManager.instance.PlaySFX(SuccesSound);
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			StartCoroutine(ShowCampaignInputError("File saved! (but upload failed)", Color.green));
			SFXManager.instance.PlaySFX(SuccesSound);
		}
		else
		{
			StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
			SFXManager.instance.PlaySFX(SuccesSound);
		}
	}

	private void deselectButton(MainMenuButtons MMB)
	{
		MMB.Selected = false;
		MMB.startTime = Time.time;
	}

	public void doButton(MainMenuButtons MMB)
	{
		if (MMB.IsOptions)
		{
			deselectButton(MMB);
			currentMenu = 1;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		else if (MMB.IsBack)
		{
			deselectButton(MMB);
			currentMenu = 1;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		else if (MMB.IsContinueGame)
		{
			deselectButton(MMB);
			ResumeGame();
			StartCoroutine("doing");
		}
		else if (MMB.IsBackCustomMenu)
		{
			deselectButton(MMB);
			enableMenu(MMB.menuNumber);
			StartCoroutine("doing");
		}
		else if (MMB.IsControls)
		{
			ControlMapper component = GameObject.Find("ControlMapper").GetComponent<ControlMapper>();
			if ((bool)component)
			{
				component.Open();
			}
		}
		else if (MMB.IsBeforeExit)
		{
			if (MapEditorMaster.instance.Levels.Count < 2)
			{
				if (musicLvlBefore > 0f)
				{
					AudioListener.volume = musicLvlBefore;
				}
				Time.timeScale = 1f;
				OptionsMainMenu.instance.StartLevel = 0;
				OptionsMainMenu.instance.MapSize = 285;
				StartCoroutine(LoadYourAsyncScene(0));
				return;
			}
			deselectButton(MMB);
			enableMenu(4);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsCancleExit)
		{
			deselectButton(MMB);
			enableMenu(0);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsExit)
		{
			if (musicLvlBefore > 0f)
			{
				AudioListener.volume = musicLvlBefore;
			}
			Time.timeScale = 0.1f;
			OptionsMainMenu.instance.StartLevel = 0;
			OptionsMainMenu.instance.MapSize = 285;
			StartCoroutine(LoadYourAsyncScene(0));
		}
		if (MMB.IsExitTesting)
		{
			if (musicLvlBefore > 0f)
			{
				AudioListener.volume = musicLvlBefore;
			}
			Debug.LogError("EXIT TESTING!!!");
			ResumeGame();
			GameMaster.instance.ResetMapEditorMap(skip: true);
		}
		if (MMB.IsContinue)
		{
			if (OptionsMainMenu.instance.StartLevel <= 8)
			{
				AchievementsTracker.instance.ResetVariables();
			}
			StartCoroutine(LoadYourAsyncScene(SceneManager.GetActiveScene().buildIndex));
		}
		if (MMB.IsSaveMap)
		{
			if (errorCampaignInputText != null)
			{
				errorCampaignInputText.color = Color.clear;
			}
			if (SignMapInput != null && (bool)SignMapToggle)
			{
				errorCampaignInputText.color = Color.clear;
			}
			if (errorCampaignInputText != null && MapEditorMaster.instance.IsPublished == 1)
			{
				errorCampaignInputText.text = "this will also update the online map (only if internet connection)";
				errorCampaignInputText.color = Color.red;
			}
			if (OptionsMainMenu.instance.MapEditorMapName != null)
			{
				campaignNameInput.text = OptionsMainMenu.instance.MapEditorMapName;
			}
			deselectButton(MMB);
			enableMenu(5);
			Selection = 0;
			NormalButtonsSavingMap.SetActive(value: true);
			ReplaceButtonsSavingMap.SetActive(value: false);
			StartCoroutine("doing");
		}
		if (MMB.IsSaveMapFile)
		{
			Debug.Log("SAVING MAP FILO");
			bool flag = false;
			deselectButton(MMB);
			StartCoroutine("doing");
			if (campaignNameInput.text == "")
			{
				SFXManager.instance.PlaySFX(ErrorSound);
				StartCoroutine(ShowCampaignInputError(LocalizationMaster.instance.GetText("CE_Name_Error"), Color.red));
				return;
			}
			if (campaignNameInput.text.Contains("?"))
			{
				SFXManager.instance.PlaySFX(ErrorSound);
				StartCoroutine(ShowCampaignInputError("Cant save with ?", Color.red));
				return;
			}
			if (SignMapInput.text == "" && SignMapToggle.IsEnabled)
			{
				SFXManager.instance.PlaySFX(ErrorSound);
				StartCoroutine(ShowCampaignInputError(LocalizationMaster.instance.GetText("CE_Name_Error"), Color.red));
				return;
			}
			Debug.Log(campaignNameInput.text);
			if (campaignNameInput.text.Contains("_CampaignMission"))
			{
				Debug.Log("CAMPAIGN MAP!");
				flag = true;
			}
			MapEditorMaster.instance.campaignName = campaignNameInput.text;
			if (SignMapInput.text != "" && SignMapToggle.IsEnabled)
			{
				MapEditorMaster.instance.signedName = SignMapInput.text;
			}
			else
			{
				MapEditorMaster.instance.signedName = "";
			}
			foreach (GameObject level in GameMaster.instance.Levels)
			{
				level.SetActive(value: true);
			}
			Debug.Log("saving with map size" + OptionsMainMenu.instance.MapSize);
			Debug.Log(campaignNameInput.text);
			MapEditorMaster.instance.SaveCurrentProps();
			bool flag2 = false;
			if ((bool)OptionsMainMenu.instance.ClassicMap)
			{
				flag2 = SavingMapEditorData.SaveClassicCampaignMap(GameMaster.instance, MapEditorMaster.instance, campaignNameInput.text, OptionsMainMenu.instance.ClassicMap);
				return;
			}
			if (!((!flag) ? SavingMapEditorData.SaveMap(GameMaster.instance, MapEditorMaster.instance, campaignNameInput.text, overwrite: false) : SavingMapEditorData.SaveCampaignMap(GameMaster.instance, MapEditorMaster.instance, campaignNameInput.text, overwrite: false)))
			{
				errorCampaignInputText.text = "File alraedy exisists REPLACE?";
				errorCampaignInputText.color = Color.red;
				NormalButtonsSavingMap.SetActive(value: false);
				ReplaceButtonsSavingMap.SetActive(value: true);
			}
			else
			{
				if (SignMapInput.text != "" && SignMapToggle.IsEnabled)
				{
					SigningTheMap.SetActive(value: false);
				}
				Debug.Log("HERE WE GO MATE");
				MapEditorData mapEditorData = SavingMapEditorData.LoadData(campaignNameInput.text);
				if (mapEditorData != null)
				{
					Debug.Log("HERE WE GO MATE!");
					if (mapEditorData.PID > 0 && MapEditorMaster.instance.IsPublished == 1)
					{
						StartCoroutine(ShowCampaignInputError("Uploading...", Color.red));
						StartCoroutine(UploadCampaign(mapEditorData));
					}
					else
					{
						StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
						SFXManager.instance.PlaySFX(SuccesSound, 0.8f);
					}
				}
				else
				{
					StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
					SFXManager.instance.PlaySFX(SuccesSound, 0.8f);
				}
			}
			foreach (GameObject level2 in GameMaster.instance.Levels)
			{
				if (GameMaster.instance.Levels[GameMaster.instance.CurrentMission] != level2)
				{
					level2.SetActive(value: false);
				}
			}
		}
		if (MMB.IsCancelReplace)
		{
			CancelReplace();
			StartCoroutine("doing");
		}
		if (MMB.IsApproveReplace)
		{
			ReplaceFileMap();
			StartCoroutine("doing");
		}
		if (MMB.IsVideo && !OptionsMainMenu.instance.inAndroid)
		{
			deselectButton(MMB);
			currentMenu = 2;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsAudio)
		{
			deselectButton(MMB);
			currentMenu = 3;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsBack)
		{
			deselectButton(MMB);
			currentMenu = 0;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsBack2Menu)
		{
			deselectButton(MMB);
			currentMenu = 1;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsGraphicsDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeGraphics(-1);
			SetGraphicsText();
		}
		if (MMB.IsGraphicsUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeGraphics(1);
			SetGraphicsText();
		}
		if (MMB.IsResolutionDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeResolution(-1, OptionsMainMenu.instance.isFullscreen);
			SetResolutionText();
		}
		if (MMB.IsResolutionUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeResolution(1, OptionsMainMenu.instance.isFullscreen);
			SetResolutionText();
		}
		if (MMB.IsFullscreen)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeFullscreen();
			if (!OptionsMainMenu.instance.isFullscreen)
			{
				Fullscreentext.text = "( )";
			}
			else
			{
				Fullscreentext.text = "(x)";
			}
			SetResolutionText();
		}
		if (MMB.IsMasterVolumeDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeMasterVolume(-1);
			MasterVolumetext.text = OptionsMainMenu.instance.masterVolumeLvl.ToString();
		}
		if (MMB.IsMasterVolumeUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeMasterVolume(1);
			MasterVolumetext.text = OptionsMainMenu.instance.masterVolumeLvl.ToString();
		}
		if (MMB.IsMusicVolumeDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeMusicVolume(-1);
			MusicVolumetext.text = OptionsMainMenu.instance.musicVolumeLvl.ToString();
		}
		if (MMB.IsMusicVolumeUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeMusicVolume(1);
			MusicVolumetext.text = OptionsMainMenu.instance.musicVolumeLvl.ToString();
		}
		if (MMB.IsSFXVolumeDown)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeSFXVolume(-1);
			SFXVolumetext.text = OptionsMainMenu.instance.sfxVolumeLvl.ToString();
		}
		if (MMB.IsSFXVolumeUp)
		{
			PlayMenuChangeSound();
			OptionsMainMenu.instance.ChangeSFXVolume(1);
			SFXVolumetext.text = OptionsMainMenu.instance.sfxVolumeLvl.ToString();
		}
	}

	public void ReplaceFileMap()
	{
		if (campaignNameInput.text == "")
		{
			SFXManager.instance.PlaySFX(ErrorSound, 0.8f);
			StartCoroutine(ShowCampaignInputError("Plaese entre name", Color.red));
			NormalButtonsSavingMap.SetActive(value: true);
			ReplaceButtonsSavingMap.SetActive(value: false);
		}
		else
		{
			if (!SavingMapEditorData.SaveMap(GameMaster.instance, MapEditorMaster.instance, campaignNameInput.text, overwrite: true))
			{
				return;
			}
			NormalButtonsSavingMap.SetActive(value: true);
			ReplaceButtonsSavingMap.SetActive(value: false);
			Debug.Log("HERE WE GO MATE");
			MapEditorData mapEditorData = SavingMapEditorData.LoadData(campaignNameInput.text);
			if (mapEditorData != null)
			{
				Debug.Log("HERE WE GO MATE!");
				if (mapEditorData.PID > 0 && MapEditorMaster.instance.IsPublished == 1)
				{
					StartCoroutine(ShowCampaignInputError("Uploading...", Color.red));
					StartCoroutine(UploadCampaign(mapEditorData));
				}
				else
				{
					StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
					SFXManager.instance.PlaySFX(SuccesSound, 0.8f);
				}
			}
			else
			{
				StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
				SFXManager.instance.PlaySFX(SuccesSound, 0.8f);
			}
		}
	}

	public void CancelReplace()
	{
		StartCoroutine(ShowCampaignInputError("", Color.clear));
		NormalButtonsSavingMap.SetActive(value: true);
		ReplaceButtonsSavingMap.SetActive(value: false);
	}

	private void PlayMenuChangeSound()
	{
		SFXManager.instance.PlaySFX(MenuChange, 0.8f);
	}

	public void enableMenu(int menunumber)
	{
		StartCoroutine(MenuTransition(menunumber));
	}

	private IEnumerator MenuTransition(int menunumber)
	{
		Selection = 0;
		SFXManager.instance.PlaySFX(MenuSwitch);
		float t2 = 0f;
		CanvasGroup CG2 = Menus[currentMenu].GetComponent<CanvasGroup>();
		MainMenuButtons[] componentsInChildren = Menus[currentMenu].GetComponentsInChildren<MainMenuButtons>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SwitchedMenu();
		}
		if ((bool)CG2)
		{
			while (t2 < 1f)
			{
				t2 += Time.unscaledDeltaTime * 5f;
				CG2.alpha = 1f - t2;
				yield return null;
			}
		}
		GameObject[] menus = Menus;
		foreach (GameObject gameObject in menus)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
		currentMenu = menunumber;
		Menus[menunumber].SetActive(value: true);
		componentsInChildren = Menus[currentMenu].GetComponentsInChildren<MainMenuButtons>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].LoadButton();
		}
		t2 = 0f;
		CG2 = Menus[currentMenu].GetComponent<CanvasGroup>();
		if ((bool)CG2)
		{
			while (t2 < 1f)
			{
				t2 = (CG2.alpha = t2 + Time.unscaledDeltaTime * 5f);
				yield return null;
			}
			CG2.alpha = 1f;
		}
		SetGraphicsText();
	}

	public IEnumerator LoadYourAsyncScene(int lvlNumber)
	{
		if (!LIS.Play)
		{
			GameObject[] menus = Menus;
			for (int i = 0; i < menus.Length; i++)
			{
				menus[i].SetActive(value: false);
			}
			if ((bool)LIS)
			{
				LIS.gameObject.SetActive(value: true);
				LIS.Play = true;
			}
			LoadingScene = true;
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(lvlNumber);
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}

	public IEnumerator doing()
	{
		doSomething = false;
		SFXManager.instance.PlaySFX(MarkerSound, 0.8f);
		yield return new WaitForSecondsRealtime(0.2f);
		doSomething = true;
	}

	private void ResumeGame()
	{
		if (MapEditorMaster.instance != null)
		{
			MapEditorMaster.instance.canPlaceProp = false;
			MapEditorMaster.instance.StartCoroutine(MapEditorMaster.instance.PlacePropTimer(0.2f));
		}
		if (musicLvlBefore > 0f)
		{
			AudioListener.volume = musicLvlBefore;
			musicLvlBefore = 0f;
		}
		if ((bool)GameMaster.instance.musicScript && pausedMusicScript)
		{
			Debug.Log("RESUMED");
			GameMaster.instance.musicScript.MusicSource.Play();
			GameMaster.instance.musicScript.paused = false;
			pausedMusicScript = false;
		}
		ControlMapper component = GameObject.Find("ControlMapper").GetComponent<ControlMapper>();
		if ((bool)component)
		{
			component.Close(save: true);
		}
		myCanvas.enabled = false;
		Canvas[] componentsInChildren = GetComponentsInChildren<Canvas>();
		foreach (Canvas canvas in componentsInChildren)
		{
			if (canvas.name == "Dropdown List")
			{
				canvas.enabled = false;
			}
		}
		GameMaster.instance.GameHasPaused = false;
		Time.timeScale = 1f;
	}

	public void PauseGame()
	{
		currentMenu = 0;
		Selection = 0;
		musicLvlBefore = AudioListener.volume;
		AudioListener.volume = 0.3f;
		enableMenu(0);
		myCanvas.enabled = true;
		pausedMusicScript = false;
		if (GameMaster.instance.musicScript != null && GameMaster.instance.musicScript.MusicSource.clip != null)
		{
			Debug.Log("PAUSED");
			GameMaster.instance.musicScript.MusicSource.Pause();
			GameMaster.instance.musicScript.paused = true;
			pausedMusicScript = true;
		}
		if (GameMaster.instance.isZombieMode)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<NewAIagent>().StopTracks();
			}
		}
		GameMaster.instance.GameHasPaused = true;
		Time.timeScale = 0f;
	}

	public void SetGraphicsText()
	{
		Graphics_list.SetValueWithoutNotify(OptionsMainMenu.instance.currentGraphicSettings);
	}

	public void SetGraphics()
	{
		Debug.LogError(Graphics_list.value);
		OptionsMainMenu.instance.ChangeGraphics(Graphics_list.value);
	}

	public void SetVSYNC()
	{
		OptionsMainMenu.instance.Vsync();
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

	private IEnumerator ShowCampaignInputError(string text, Color clr)
	{
		errorCampaignInputText.color = clr;
		errorCampaignInputText.text = text;
		yield return new WaitForSecondsRealtime(2f);
		errorCampaignInputText.color = Color.clear;
	}
}
