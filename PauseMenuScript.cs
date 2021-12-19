using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
	public Canvas myCanvas;

	public TextMeshProUGUI TanksLeft_Text;

	public GameObject TanksLeftField;

	public int currentMenu;

	public List<int> menuAmountOptions = new List<int>();

	public GameObject[] Menus;

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

	public Toggle SignMapToggle;

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

	private bool pausedMusicScript;

	public int musicLvlBefore;

	private void Start()
	{
		player = ReInput.players.GetPlayer(0);
		if (errorCampaignInputText != null && MapEditorMaster.instance.IsPublished != 1)
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
		if (!OptionsMainMenu.instance.isFullscreen)
		{
			Fullscreentext.text = "( )";
		}
		else
		{
			Fullscreentext.text = "(x)";
		}
		if (GameMaster.instance.inMapEditor && MapEditorMaster.instance.signedName != "")
		{
			SigningTheMap.SetActive(value: false);
		}
		MusicVolumetext.text = OptionsMainMenu.instance.musicVolumeLvl.ToString();
		MasterVolumetext.text = OptionsMainMenu.instance.masterVolumeLvl.ToString();
		SFXVolumetext.text = OptionsMainMenu.instance.sfxVolumeLvl.ToString();
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
			CheckpointText.text = "Checkpoint " + OptionsMainMenu.instance.StartLevel;
		}
	}

	private void Update()
	{
		if ((bool)CheckpointText && GameMaster.instance.CurrentMission > OptionsMainMenu.instance.StartLevel + 9)
		{
			OptionsMainMenu.instance.StartLevel += 10;
			CheckpointText.text = "Checkpoint " + OptionsMainMenu.instance.StartLevel;
		}
		if (GameMaster.instance.GameHasPaused)
		{
			input.x = player.GetAxis("Move Horizontal");
			input.y = player.GetAxis("Move Vertically");
			if ((bool)GameMaster.instance && (bool)TanksLeft_Text)
			{
				TanksLeft_Text.text = "Tanks left: " + GameMaster.instance.Lives;
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
			if (player.GetButtonUp("Menu Use"))
			{
				doButton(currentScript);
			}
		}
		if ((bool)MapEditorMaster.instance && !MapEditorMaster.instance.inPlayingMode)
		{
			if (MapEditorMaster.instance.isTesting)
			{
				NormalQuitButton.SetActive(value: false);
				NormalQuitButtonMarker.SetActive(value: false);
				StopTestingButton.SetActive(value: true);
				StopTestingButtonMarker.SetActive(value: true);
			}
			else if (!myCanvas.enabled)
			{
				NormalQuitButton.SetActive(value: true);
				NormalQuitButtonMarker.SetActive(value: true);
				StopTestingButton.SetActive(value: false);
				StopTestingButtonMarker.SetActive(value: false);
			}
		}
		if (SteamTest.instance.SteamOverlayActive && !myCanvas.enabled)
		{
			PauseGame();
		}
		if (player.GetButtonUp("Escape"))
		{
			if (GameMaster.instance.inMapEditor)
			{
				ApproveQuitObject.SetActive(value: false);
				NormalQuitButton.SetActive(value: true);
				NormalQuitButtonMarker.SetActive(value: true);
			}
			if (myCanvas.enabled && !LoadingScene)
			{
				Debug.LogWarning("Menu closing esc!");
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
			Play2DClipAtPoint(SuccesSound);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			StartCoroutine(ShowCampaignInputError("File saved! (but upload failed)", Color.green));
			Play2DClipAtPoint(SuccesSound);
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
			Play2DClipAtPoint(SuccesSound);
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			StartCoroutine(ShowCampaignInputError("File saved! (but upload failed)", Color.green));
			Play2DClipAtPoint(SuccesSound);
		}
		else
		{
			StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
			Play2DClipAtPoint(SuccesSound);
		}
	}

	private void deselectButton(MainMenuButtons MMB)
	{
		MMB.Selected = false;
		MMB.startTime = Time.unscaledDeltaTime;
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
		if (MMB.IsBack)
		{
			deselectButton(MMB);
			currentMenu = 1;
			enableMenu(currentMenu);
			Selection = 0;
			StartCoroutine("doing");
		}
		if (MMB.IsContinueGame)
		{
			deselectButton(MMB);
			ResumeGame();
			StartCoroutine("doing");
		}
		if (MMB.IsBeforeExit)
		{
			if (GameMaster.instance.Levels.Count < 2)
			{
				if (musicLvlBefore > 0 && OptionsMainMenu.instance.musicVolumeLvl == 3)
				{
					OptionsMainMenu.instance.musicVolumeLvl = musicLvlBefore;
					musicLvlBefore = 0;
				}
				Time.timeScale = 1f;
				OptionsMainMenu.instance.StartLevel = 0;
				OptionsMainMenu.instance.MapSize = 285;
				StartCoroutine(LoadYourAsyncScene(0));
				return;
			}
			ApproveQuitObject.SetActive(value: true);
			NormalQuitButton.SetActive(value: false);
			NormalQuitButtonMarker.SetActive(value: false);
		}
		if (MMB.IsCancleExit)
		{
			ApproveQuitObject.SetActive(value: false);
			NormalQuitButton.SetActive(value: true);
			NormalQuitButtonMarker.SetActive(value: true);
		}
		if (MMB.IsExit)
		{
			if (musicLvlBefore > 0 && OptionsMainMenu.instance.musicVolumeLvl == 3)
			{
				OptionsMainMenu.instance.musicVolumeLvl = musicLvlBefore;
				musicLvlBefore = 0;
			}
			Time.timeScale = 1f;
			OptionsMainMenu.instance.StartLevel = 0;
			OptionsMainMenu.instance.MapSize = 285;
			StartCoroutine(LoadYourAsyncScene(0));
		}
		if (MMB.IsExitTesting)
		{
			if (musicLvlBefore > 0 && OptionsMainMenu.instance.musicVolumeLvl == 3)
			{
				OptionsMainMenu.instance.musicVolumeLvl = musicLvlBefore;
				musicLvlBefore = 0;
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
			StartCoroutine(LoadYourAsyncScene(1));
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
			currentMenu = 4;
			enableMenu(currentMenu);
			Selection = 0;
			NormalButtonsSavingMap.SetActive(value: true);
			ReplaceButtonsSavingMap.SetActive(value: false);
			StartCoroutine("doing");
		}
		if (MMB.IsSaveMapFile)
		{
			bool flag = false;
			deselectButton(MMB);
			if (campaignNameInput.text == "")
			{
				Play2DClipAtPoint(ErrorSound);
				StartCoroutine(ShowCampaignInputError("Plaese entre name", Color.red));
				return;
			}
			if (campaignNameInput.text.Contains("?"))
			{
				Play2DClipAtPoint(ErrorSound);
				StartCoroutine(ShowCampaignInputError("Cant save with ?", Color.red));
				return;
			}
			if (SignMapInput.text == "" && SignMapToggle.isOn)
			{
				Play2DClipAtPoint(ErrorSound);
				StartCoroutine(ShowCampaignInputError("Plaese entre name", Color.red));
				return;
			}
			Debug.Log(campaignNameInput.text);
			if (campaignNameInput.text.Contains("_CampaignMission"))
			{
				Debug.Log("CAMPAIGN MAP!");
				flag = true;
			}
			MapEditorMaster.instance.campaignName = campaignNameInput.text;
			if (SignMapInput.text != "" && SignMapToggle.isOn)
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
			MapEditorMaster.instance.SaveCurrentProps();
			bool flag2 = false;
			if (!((!flag) ? SavingMapEditorData.SaveMap(GameMaster.instance, MapEditorMaster.instance, campaignNameInput.text, overwrite: false) : SavingMapEditorData.SaveCampaignMap(GameMaster.instance, MapEditorMaster.instance, campaignNameInput.text, overwrite: false)))
			{
				errorCampaignInputText.text = "File alraedy exisists REPLACE?";
				errorCampaignInputText.color = Color.red;
				NormalButtonsSavingMap.SetActive(value: false);
				ReplaceButtonsSavingMap.SetActive(value: true);
			}
			else
			{
				if (SignMapInput.text != "" && SignMapToggle.isOn)
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
						Play2DClipAtPoint(SuccesSound);
					}
				}
				else
				{
					StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
					Play2DClipAtPoint(SuccesSound);
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
			Play2DClipAtPoint(ErrorSound);
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
					Play2DClipAtPoint(SuccesSound);
				}
			}
			else
			{
				StartCoroutine(ShowCampaignInputError("File saved!", Color.green));
				Play2DClipAtPoint(SuccesSound);
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
		Play2DClipAtPoint(MenuChange);
	}

	public void enableMenu(int menunumber)
	{
		GameObject[] menus = Menus;
		for (int i = 0; i < menus.Length; i++)
		{
			menus[i].SetActive(value: false);
		}
		Menus[menunumber].SetActive(value: true);
		Play2DClipAtPoint(MenuSwitch);
	}

	public IEnumerator LoadYourAsyncScene(int lvlNumber)
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

	public IEnumerator doing()
	{
		doSomething = false;
		Play2DClipAtPoint(MarkerSound);
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
		if (musicLvlBefore > 0 && OptionsMainMenu.instance.musicVolumeLvl == 3)
		{
			OptionsMainMenu.instance.musicVolumeLvl = musicLvlBefore;
			musicLvlBefore = 0;
		}
		if ((bool)GameMaster.instance.musicScript && pausedMusicScript)
		{
			Debug.Log("RESUMED");
			GameMaster.instance.musicScript.MusicSource.Play();
			GameMaster.instance.musicScript.paused = false;
			pausedMusicScript = false;
		}
		myCanvas.enabled = false;
		GameMaster.instance.GameHasPaused = false;
		Time.timeScale = 1f;
	}

	public void PauseGame()
	{
		currentMenu = 0;
		Selection = 0;
		if (OptionsMainMenu.instance.musicVolumeLvl > 3)
		{
			musicLvlBefore = OptionsMainMenu.instance.musicVolumeLvl;
			OptionsMainMenu.instance.musicVolumeLvl = 3;
		}
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
		switch (OptionsMainMenu.instance.currentGraphicSettings)
		{
		case 1:
			Graphicstext.text = "Garbage";
			break;
		case 2:
			Graphicstext.text = "Low";
			break;
		case 3:
			Graphicstext.text = "Medium";
			break;
		case 4:
			Graphicstext.text = "High";
			break;
		case 5:
			Graphicstext.text = "Ultra";
			break;
		}
	}

	public void SetResolutionText()
	{
		Resolutiontext.text = OptionsMainMenu.instance.ResolutionX[OptionsMainMenu.instance.currentResolutionSettings] + "x" + OptionsMainMenu.instance.ResolutionY[OptionsMainMenu.instance.currentResolutionSettings];
	}

	private IEnumerator ShowCampaignInputError(string text, Color clr)
	{
		errorCampaignInputText.color = clr;
		errorCampaignInputText.text = text;
		yield return new WaitForSecondsRealtime(2f);
		errorCampaignInputText.color = Color.clear;
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.ignoreListenerVolume = true;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		UnityEngine.Object.Destroy(obj, clip.length);
	}
}
