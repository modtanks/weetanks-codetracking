using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMainMenu : MonoBehaviour
{
	private static OptionsMainMenu _instance;

	public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

	[Header("Options")]
	public bool StartPlayer2Mode;

	public bool FriendlyFire;

	public int ExtraLives;

	public bool AimAssist;

	public bool MarkedTanks;

	public bool[] AIcompanion;

	public bool[] PlayerJoined;

	public bool SnowMode;

	public bool demoMode;

	public bool vsync;

	public bool showxraybullets;

	public int StartLevel;

	public int currentGraphicSettings = 5;

	public int currentResolutionSettings = 5;

	public int currentFPSSettings = 5;

	public int musicVolumeLvl = 10;

	public int masterVolumeLvl = 10;

	public int sfxVolumeLvl = 10;

	public int currentDifficulty = 1;

	public int UIsetting = 1;

	public int[] AM;

	public int[] AM_marbles;

	public int[] AMids;

	public int[] AMdifficulty;

	public int MapSize = 285;

	public string[] AMnames;

	public Texture[] AMimages;

	public Texture[] AMimagesNot;

	[TextArea]
	public string[] AMdesc;

	public List<int> AMselected = new List<int>();

	public List<UnlockableScript> AMUS = new List<UnlockableScript>();

	public int WaveReached;

	public string MapEditorMapName = "";

	public string CurrentVersion = "v0.7.6a";

	public bool inAndroid;

	public bool isFullscreen = true;

	public Texture2D cursorTexture;

	public Texture2D OriginalCursorTexture;

	public CrosshairPrefab[] Crosshairs;

	public Texture2D[] CustomCursorTextures;

	public int[] CustomCursorTexturesAMIDs;

	public CursorMode cursorMode;

	public Vector2 hotSpot = Vector2.zero;

	public bool autoCenterHotSpot;

	public Vector2 hotSpotCustom = Vector2.zero;

	private Vector2 hotSpotAuto;

	public bool enableCursor = true;

	public CustomSkinData[] FullBodySkins;

	public UnlockableItem[] UIs;

	public int CompletedCustomCampaigns;

	public int[] ResolutionX;

	public int[] ResolutionY;

	public bool BloodMode;

	public bool[] MapEditorTankMessagesReceived;

	public bool IsThirdPerson;

	private bool showCursor = true;

	public static OptionsMainMenu instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		_instance = this;
		if (SavingData.ExistSettingsData())
		{
			SettingsData settingsData = SavingData.LoadSettingsData();
			musicVolumeLvl = settingsData.musicVolLevel;
			sfxVolumeLvl = settingsData.sfxVolLevel;
			masterVolumeLvl = settingsData.masterVolLevel;
			currentGraphicSettings = settingsData.graphicsSettings;
			currentResolutionSettings = settingsData.resolution;
			currentFPSSettings = settingsData.fps;
			FriendlyFire = settingsData.friendlyFire;
			isFullscreen = settingsData.Fullscreen;
			currentDifficulty = settingsData.difficultySetting;
			vsync = settingsData.vsync;
			BloodMode = settingsData.GoreMode;
			UIsetting = settingsData.UIsettings;
			if (settingsData.MapEditorTankMessagesReceived != null)
			{
				MapEditorTankMessagesReceived = settingsData.MapEditorTankMessagesReceived;
			}
			if (settingsData.SnowyMode)
			{
				if (DateTime.Now.Month == 12 || DateTime.Now.Month == 1 || DateTime.Now.Month == 2)
				{
					SnowMode = settingsData.SnowyMode;
				}
				else
				{
					SnowMode = false;
				}
			}
			else
			{
				SnowMode = false;
			}
			CompletedCustomCampaigns = settingsData.CompletedCustomCampaigns;
			MarkedTanks = settingsData.MarkedTanks;
			if (settingsData.keys != null)
			{
				settingsData.keys.ContainsKey("hudKey");
			}
			if (settingsData.xraybullets)
			{
				showxraybullets = settingsData.xraybullets;
			}
			Debug.LogWarning("Settings-Data loaded from save file");
		}
		else
		{
			if (DateTime.Now.Month == 12 || DateTime.Now.Month == 1 || DateTime.Now.Month == 2)
			{
				SnowMode = true;
			}
			else
			{
				SnowMode = false;
			}
			SavingData.SaveSettingsData(this);
			Debug.LogWarning("NEW Settings data has been saved");
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			enableCursor = false;
			Cursor.visible = false;
			inAndroid = true;
		}
		ChangeGraphics(0);
		ChangeResolution(0, isFullscreen);
		ChangeFPS(0);
		CheckCustomHitmarkers();
	}

	private void SetNewKeys()
	{
		keys.Add("mineKey", KeyCode.Space);
		keys.Add("boostKey", KeyCode.LeftShift);
		keys.Add("shootKey", KeyCode.Mouse0);
		keys.Add("upKey", KeyCode.W);
		keys.Add("downKey", KeyCode.S);
		keys.Add("leftKey", KeyCode.A);
		keys.Add("rightKey", KeyCode.D);
		keys.Add("hudKey", KeyCode.H);
	}

	private void OnEnable()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	public void SaveNewData()
	{
		SavingData.SaveSettingsData(this);
	}

	public void CheckCustomHitmarkers()
	{
		for (int i = 0; i < Crosshairs.Length; i++)
		{
			if (AMselected.Contains(Crosshairs[i].AMID))
			{
				CrosshairScript.instance.SelectedCrosshair = Crosshairs[i];
				return;
			}
		}
		CrosshairScript.instance.SelectedCrosshair = CrosshairScript.instance.OriginalCrosshair;
	}

	private void FixedUpdate()
	{
	}

	public void Vsync()
	{
		vsync = ((!vsync) ? true : false);
		if (vsync)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
	}

	public void ChangeDifficulty(int change)
	{
		if (currentDifficulty > 0 && change == -1)
		{
			currentDifficulty--;
			SaveNewData();
		}
		else if (currentDifficulty < 3 && change == 1)
		{
			currentDifficulty++;
			SaveNewData();
		}
	}

	public void ChangeMusicVolume(int change)
	{
		if (musicVolumeLvl > 0 && change == -1)
		{
			musicVolumeLvl--;
			Debug.LogError("Volume downed");
		}
		else if (musicVolumeLvl < 10 && change == 1)
		{
			musicVolumeLvl++;
			Debug.LogError("Volumeupped");
		}
		SaveNewData();
	}

	public void ChangeMasterVolume(int change)
	{
		if (masterVolumeLvl > 0 && change == -1)
		{
			masterVolumeLvl--;
			Debug.LogError("Volume downed");
		}
		else if (masterVolumeLvl < 10 && change == 1)
		{
			masterVolumeLvl++;
			Debug.LogError("Volumeupped");
		}
		SaveNewData();
	}

	public void ChangeSFXVolume(int change)
	{
		if (sfxVolumeLvl > 0 && change == -1)
		{
			sfxVolumeLvl--;
			Debug.LogError("Volume downed");
		}
		else if (sfxVolumeLvl < 10 && change == 1)
		{
			sfxVolumeLvl++;
			Debug.LogError("Volumeupped");
		}
		SaveNewData();
	}

	public void ChangeFullscreen()
	{
		isFullscreen = ((!isFullscreen) ? true : false);
		ChangeResolution(0, isFullscreen);
		SaveNewData();
	}

	public void ChangeGraphics(int change)
	{
		if (currentGraphicSettings > 1 && change == -1)
		{
			currentGraphicSettings--;
		}
		if (currentGraphicSettings < 5 && change == 1)
		{
			currentGraphicSettings++;
		}
		QualitySettings.SetQualityLevel(currentGraphicSettings - 1, applyExpensiveChanges: true);
		SaveNewData();
	}

	public void ChangeResolution(int change, bool fullscreen)
	{
		if (!inAndroid)
		{
			if (currentResolutionSettings > 1 && change == -1)
			{
				currentResolutionSettings--;
			}
			if (currentResolutionSettings < ResolutionX.Length && change == 1)
			{
				currentResolutionSettings++;
			}
			if (currentResolutionSettings < 0)
			{
				currentResolutionSettings = 0;
			}
			else if (currentResolutionSettings >= ResolutionX.Length)
			{
				currentResolutionSettings = ResolutionX.Length - 1;
			}
			Screen.SetResolution(ResolutionX[currentResolutionSettings], ResolutionY[currentResolutionSettings], fullscreen);
		}
		SaveNewData();
	}

	public void ChangeFPS(int change)
	{
		if (!inAndroid)
		{
			QualitySettings.vSyncCount = 0;
			if (currentFPSSettings > 1 && change == -1)
			{
				currentFPSSettings--;
			}
			if (currentFPSSettings < 9 && change == 1)
			{
				currentFPSSettings++;
			}
			switch (currentFPSSettings)
			{
			case 1:
				Application.targetFrameRate = 10;
				break;
			case 2:
				Application.targetFrameRate = 25;
				break;
			case 3:
				Application.targetFrameRate = 30;
				break;
			case 4:
				Application.targetFrameRate = 40;
				break;
			case 5:
				Application.targetFrameRate = 50;
				break;
			case 6:
				Application.targetFrameRate = 59;
				break;
			case 7:
				Application.targetFrameRate = 60;
				break;
			case 8:
				Application.targetFrameRate = 100;
				break;
			case 9:
				Application.targetFrameRate = 120;
				break;
			}
		}
		SaveNewData();
	}

	private void Start()
	{
		autoCenterHotSpot = true;
	}

	private void Update()
	{
		if (autoCenterHotSpot)
		{
			if (Screen.height > 1080)
			{
				hotSpotAuto = new Vector2((float)cursorTexture.width * 0.5f, (float)cursorTexture.height * 0.5f);
			}
			else
			{
				hotSpotAuto = new Vector2((float)cursorTexture.width * 0.5f, (float)cursorTexture.height * 0.5f);
			}
			_ = hotSpotAuto;
		}
		else
		{
			_ = hotSpotCustom;
		}
		if (currentDifficulty == 0 && ExtraLives != 2)
		{
			ExtraLives = 2;
		}
		else if (currentDifficulty != 0 && ExtraLives == 2)
		{
			ExtraLives = 0;
		}
		Cursor.visible = false;
		if (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.C))
		{
			showCursor = !showCursor;
			CrosshairScript.instance.showCursor = showCursor;
		}
		if (AMselected.Contains(7))
		{
			IsThirdPerson = true;
		}
		if ((bool)MapEditorMaster.instance && !MapEditorMaster.instance.inPlayingMode)
		{
			IsThirdPerson = false;
		}
		if ((bool)GameMaster.instance && GameMaster.instance.inMenuMode)
		{
			IsThirdPerson = false;
		}
		if (IsThirdPerson && !GameMaster.instance.GameHasPaused && GameMaster.instance.GameHasStarted)
		{
			showCursor = false;
			CrosshairScript.instance.showCursor = showCursor;
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			if ((bool)GameMaster.instance && (bool)GameMaster.instance.MainCamera)
			{
				GameMaster.instance.MainCamera.enabled = true;
			}
			Cursor.lockState = CursorLockMode.None;
			showCursor = true;
			CrosshairScript.instance.showCursor = showCursor;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
