using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorMaster : MonoBehaviour
{
	[Serializable]
	public class MissionProperties
	{
		public int MissionNumber;

		public int WeatherType;

		public int MissionFloorTexture;

		public string CurrentFloorName;
	}

	private static MapEditorMaster _instance;

	public List<MapPiecesClass> MissionsData = new List<MapPiecesClass>();

	public bool inPlayingMode;

	public int SelectedProp;

	public int maxMission = 20;

	public GameObject MenuItemPrefab;

	public GameObject TankeyTownListParent;

	public GameObject[] Props;

	public float[] PropStartHeight;

	public bool RemoveMode;

	public int selectedFields;

	public Transform startSelectionField;

	public Animator ErrorFieldMessage;

	public GameObject TeleportationFields;

	public List<int> playerOnePlaced = new List<int>();

	public List<int> playerTwoPlaced = new List<int>();

	public List<int> playerThreePlaced = new List<int>();

	public List<int> playerFourPlaced = new List<int>();

	public List<int> enemyTanksPlaced = new List<int>();

	public List<int> NoBordersMissions = new List<int>();

	public int maxEnemyTanks = 10;

	public CountDownScript CDS;

	public bool isTesting;

	public bool canPlaceProp;

	public GameObject EditingCanvas;

	public int IsPublished;

	public Material[] FloorMaterials;

	[Header("Layer Editing")]
	public int CurrentLayer;

	public int MaxLayer = 5;

	public bool ShowAllLayers;

	[Header("Map editor Levels")]
	public List<SingleMapEditorData> Levels = new List<SingleMapEditorData>();

	public GameObject LevelPrefab;

	public GameObject LevelUIListPrefab;

	public List<GameObject> LevelUIList = new List<GameObject>();

	public string campaignName;

	public string signedName;

	public GameObject[] MapBorders;

	public List<MissionProperties> Properties = new List<MissionProperties>();

	public ParticleSystem SelectedParticles;

	[Header("Map Editor Audio")]
	public AudioClip PlaceHeavy;

	public AudioClip PlaceLight;

	public AudioClip RemoveObject;

	public AudioClip RotateObject;

	public AudioClip ErrorSound;

	public AudioClip SuccesSound;

	public AudioClip EditingModeBackgroundMusic;

	public AudioClip MenuSelect;

	public AudioClip MenuSwitch;

	public AudioClip NewMapSound;

	public AudioClip ScribbleSound;

	public AudioClip ClickSound;

	public AudioClip ChangeSound;

	private int playingAmount;

	public int lastKnownMusicVol;

	public int lastKnownMasterVol;

	public int PID;

	[Header("Map editor UI")]
	public TextMeshProUGUI ErrorField;

	public TMP_InputField MissionNameInputField;

	public TMP_InputField MissionMessageInputField;

	public GameObject[] Menus;

	public RawImage[] MenuImages;

	public GameObject ParentObjectList;

	public Color TabNotSelectedColor;

	public Color TabSelectedColor;

	[Header("Custom Tank")]
	public int minTankSpeed;

	public int maxTankSpeed = 100;

	private int maxTankSpeedBoosted = 1000;

	public float minFireSpeed = 0.15f;

	public float maxFireSpeed = 100f;

	public int minBulletSpeed = 1;

	public int maxBulletSpeed = 100;

	public int minBounces;

	public int maxBounces = 10;

	private int maxBouncesBoosted = 100;

	public int minBullets;

	public int maxBullets = 100;

	public float minMineSpeed = 0.5f;

	public float maxMineSpeed = 10f;

	public int minTurnHead = 1;

	public int maxTurnHead = 5;

	public int minAccuracy;

	public int maxAccuracy = 100;

	public int minArmourPoints = 1;

	public int maxArmourPoints = 3;

	private int maxArmourPointsBoosted = 1000;

	public float minScalePoints = 0.5f;

	public float maxScalePoints = 1.5f;

	private float maxScalePointsBoosted = 10f;

	public MeshRenderer[] CustomTankExampleRenderers;

	public GameObject CustomTankExampleRocketHolder;

	public GameObject[] CustomTankExampleRockets;

	public Image CustomTankMaterialSource;

	public List<CustomTankData> CustomTankDatas = new List<CustomTankData>();

	public MapEditorUIprop CustomTankScript;

	public GameObject[] BulletPrefabs;

	public GameObject[] PlayerBulletPrefabs;

	public AudioClip[] PlayerBulletSound;

	public int StartingLives = 3;

	public Color[] TeamColors;

	public bool[] TeamColorEnabled;

	[Header("Custom Tank input fields")]
	public CustomTankEditor CTE;

	public Slider TankSpeedField;

	public Slider FireSpeedField;

	public Slider AmountBouncesField;

	public Slider AmountBulletsField;

	public Slider MineLaySpeedField;

	public Slider TurnHeadSlider;

	public Slider AccuracySlider;

	public Slider ArmouredSlider;

	public Slider ScaleSlider;

	public Slider BulletsPerShotSlider;

	public Slider BulletSpeedSlider;

	public Slider TankHealthSlider;

	public GameObject MenuAddCustomTankButton;

	[Header("Campaign Settings")]
	public int Difficulty;

	public Slider StartLivesSlider;

	public Slider PlayerSpeedSlider;

	public Slider PlayerMaxBulletsSlider;

	public Slider PlayerBouncesSlider;

	public Slider PlayerArmourPointsSlider;

	public TMP_Dropdown PlayerBulletTypeList;

	public TMP_Dropdown DifficultyList;

	public TMP_Dropdown WeatherList;

	public TMP_Dropdown FloorDropdown;

	public ButtonMouseEvents PlayerCanLayMinesToggle;

	public ButtonMouseEvents[] TeamColorsToggles;

	public ButtonMouseEvents NightToggler;

	public ButtonMouseEvents NoBordersToggler;

	public int PlayerSpeed = 65;

	public int PlayerMaxBullets = 5;

	public int PlayerBulletType;

	public int PlayerAmountBounces;

	public int PlayerArmourPoints;

	public bool PlayerCanLayMines = true;

	public int SelectedCustomTank;

	public FlexibleColorPicker FCP;

	public TMP_Dropdown BulletTypeList;

	public TMP_Dropdown MusicList;

	public TMP_InputField CustomTankInputName;

	public ButtonMouseEvents CanLayMinesToggle;

	public ButtonMouseEvents InvisibilityToggle;

	public ButtonMouseEvents CalculateShotsToggle;

	public ButtonMouseEvents ArmouredToggle;

	public ButtonMouseEvents CanBeAirdroppedToggle;

	public ButtonMouseEvents ShowHealthbarToggle;

	public ButtonMouseEvents CanTeleport;

	public ButtonMouseEvents CanAirMissiles;

	public Slider AirMissileReloadSpeed;

	public Slider AirMissileCapacity;

	public TextMeshProUGUI OnCursorText;

	public GameObject TeamsCursorMenu;

	public bool OnTeamsMenu;

	public OnTeamsMenu OTM;

	public List<Material> CustomMaterial;

	public GameObject SelectedPropUITexture;

	public int SelectedPropUITextureMenu;

	public int MenuCurrent;

	public Vector3 AnimatorStartHeight;

	public int ID;

	public int ShowIDSlider = -1;

	public bool canDoButton = true;

	public RawImage[] BarButtons;

	public Texture BarButtonNotSelected;

	public Texture BarButtonSelected;

	public Button TankeyTownItemsButton;

	public int[] ColorsTeamsPlaced;

	public int LastPlacedColor;

	public int LastPlacedRotation;

	private Animator camAnimator;

	public List<TankeyTownStockItem> ItemsPlayerHas = new List<TankeyTownStockItem>();

	private bool canClick = true;

	private bool canChangeValues = true;

	public GameObject CustomTankMenuItem;

	public Transform CustomTankUIParent;

	public List<GameObject> SpawnedCustomTankUI = new List<GameObject>();

	public TMP_Dropdown MapEditorDifficultyList;

	public static MapEditorMaster instance => _instance;

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
	}

	private void SetMapBorder(int sel)
	{
		for (int i = 0; i < MapBorders.Length; i++)
		{
			if (i == sel)
			{
				MapBorders[i].SetActive(value: true);
			}
			else
			{
				MapBorders[i].SetActive(value: false);
			}
		}
	}

	private void Start()
	{
		if ((bool)MapEditorDifficultyList)
		{
			MapEditorDifficultyList.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
			MapEditorDifficultyList.value = OptionsMainMenu.instance.currentDifficulty;
		}
		if ((bool)AccountMaster.instance && AccountMaster.instance.isSignedIn)
		{
			AccountMaster.instance.StartCoroutine(AccountMaster.instance.GetCloudInventory());
		}
		camAnimator = Camera.main.GetComponent<Animator>();
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			SetMapBorder(0);
			camAnimator.SetInteger("MapSize", 0);
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			SetMapBorder(1);
			camAnimator.SetInteger("MapSize", 1);
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			SetMapBorder(2);
			camAnimator.SetInteger("MapSize", 2);
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			SetMapBorder(3);
			camAnimator.SetInteger("MapSize", 3);
		}
		if (GameMaster.instance.inMapEditor)
		{
			CDS.gameObject.SetActive(value: false);
		}
		if (!inPlayingMode)
		{
			LoadData();
			LastPlacedColor = 2;
			RawImage[] menuImages = MenuImages;
			for (int i = 0; i < menuImages.Length; i++)
			{
				menuImages[i].color = TabNotSelectedColor;
			}
			TeleportationFields.SetActive(value: false);
			AudioSource component = Camera.main.GetComponent<AudioSource>();
			if (OptionsMainMenu.instance != null)
			{
				lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
				lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
				component.volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			}
			component.clip = EditingModeBackgroundMusic;
			component.loop = true;
			component.Play();
			ShowMenu(0);
		}
		if (!inPlayingMode)
		{
			StartCoroutine(LateStart());
		}
		StartCoroutine(AutoSaveProject());
	}

	private IEnumerator AutoSaveProject()
	{
		yield return new WaitForSeconds(120f);
		SaveCurrentProps();
		bool num = SavingMapEditorData.AutoSaveMap(GameMaster.instance, instance);
		if (campaignName != null && campaignName.Length > 2)
		{
			SavingMapEditorData.SaveMap(GameMaster.instance, instance, campaignName, overwrite: true);
		}
		if (num)
		{
			Debug.Log("AUTO SAVE COMPLETED!");
			ShowPositiveMessage("Auto save + backup completed!");
		}
		else
		{
			Debug.Log("AUTO SAVE FAILED");
		}
		StartCoroutine(AutoSaveProject());
	}

	private IEnumerator LateStart()
	{
		StartLivesSlider.value = StartingLives;
		for (int i = 0; i < TeamColorsToggles.Length; i++)
		{
			TeamColorsToggles[i].IsEnabled = TeamColorEnabled[i];
		}
		Menus[8].transform.position += new Vector3(0f, -3000f, 0f);
		TankeyTownItemsButton.gameObject.SetActive(value: false);
		yield return new WaitForSeconds(0.25f);
		MapEditorDifficultyList.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
		MapEditorDifficultyList.value = OptionsMainMenu.instance.currentDifficulty;
		UpdateMembus();
		int num = 0;
		if (AccountMaster.instance.Inventory.InventoryItems != null && AccountMaster.instance.Inventory.InventoryItems.Count > 0)
		{
			foreach (int inventoryItem in AccountMaster.instance.Inventory.InventoryItems)
			{
				for (int j = 0; j < GlobalAssets.instance.StockDatabase.Count; j++)
				{
					if (inventoryItem == GlobalAssets.instance.StockDatabase[j].ItemID && GlobalAssets.instance.StockDatabase[j].IsMapEditorObject)
					{
						ItemsPlayerHas.Add(GlobalAssets.instance.StockDatabase[j]);
						num++;
						MapEditorUIprop component = UnityEngine.Object.Instantiate(MenuItemPrefab, TankeyTownListParent.transform).GetComponent<MapEditorUIprop>();
						if ((bool)component)
						{
							component.PropID = GlobalAssets.instance.StockDatabase[j].MapEditorPropID;
							component.customTex = GlobalAssets.instance.StockDatabase[j].ItemTexture;
						}
					}
				}
			}
		}
		if (num > 0)
		{
			TankeyTownItemsButton.gameObject.SetActive(value: true);
		}
	}

	public IEnumerator coolDown()
	{
		canDoButton = false;
		yield return new WaitForSeconds(0.5f);
		canDoButton = true;
	}

	public void SetMissionMessage(TMP_InputField InputField)
	{
		if (Levels.Count >= GameMaster.instance.CurrentMission)
		{
			Levels[GameMaster.instance.CurrentMission].MissionMessage = InputField.text;
		}
	}

	public void LoadCustomTankDataUI(int customNumber)
	{
		TankSpeedField.value = CustomTankDatas[customNumber].CustomTankSpeed;
		FireSpeedField.value = CustomTankDatas[customNumber].CustomFireSpeed;
		AmountBouncesField.value = CustomTankDatas[customNumber].CustomBounces;
		AmountBulletsField.value = CustomTankDatas[customNumber].CustomBullets;
		MineLaySpeedField.value = CustomTankDatas[customNumber].CustomMineSpeed;
		TurnHeadSlider.value = CustomTankDatas[customNumber].CustomTurnHead;
		AccuracySlider.value = CustomTankDatas[customNumber].CustomAccuracy;
		ScaleSlider.value = CustomTankDatas[customNumber].CustomTankScale;
		if (FCP != null)
		{
			FCP.color = CustomMaterial[customNumber].color;
			FCP.startingColor = CustomTankDatas[customNumber].CustomTankColor.Color;
		}
		CanLayMinesToggle.IsEnabled = CustomTankDatas[customNumber].CustomLayMines;
		InvisibilityToggle.IsEnabled = CustomTankDatas[customNumber].CustomInvisibility;
		CalculateShotsToggle.IsEnabled = CustomTankDatas[customNumber].CustomCalculateShots;
		CanTeleport.IsEnabled = CustomTankDatas[customNumber].CustomCanTeleport;
		ShowHealthbarToggle.IsEnabled = CustomTankDatas[customNumber].CustomShowHealthbar;
		BulletsPerShotSlider.value = CustomTankDatas[customNumber].CustomBulletsPerShot;
		BulletSpeedSlider.value = CustomTankDatas[customNumber].CustomBulletSpeed;
		CanBeAirdroppedToggle.IsEnabled = CustomTankDatas[customNumber].CustomCanBeAirdropped;
		TankHealthSlider.value = CustomTankDatas[customNumber].CustomTankHealth;
		ArmouredSlider.value = CustomTankDatas[customNumber].CustomArmourPoints;
		ArmouredToggle.IsEnabled = CustomTankDatas[customNumber].CustomArmoured;
		BulletTypeList.value = CustomTankDatas[customNumber].CustomBulletType;
		MusicList.value = CustomTankDatas[customNumber].CustomMusic;
		CustomTankInputName.text = CustomTankDatas[customNumber].CustomTankName;
		BulletSpeedSlider.value = CustomTankDatas[customNumber].CustomBulletSpeed;
		AirMissileCapacity.value = CustomTankDatas[customNumber].CustomMissileCapacity;
		AirMissileReloadSpeed.value = CustomTankDatas[customNumber].CustomMissileReloadSpeed;
		CanAirMissiles.IsEnabled = CustomTankDatas[customNumber].CanShootAirMissiles;
		if (!CanAirMissiles.IsEnabled)
		{
			CTE.CustomTankImageRockets.SetActive(value: false);
		}
		else
		{
			CTE.CustomTankImageRockets.SetActive(value: true);
		}
	}

	private IEnumerator ResetClickSound()
	{
		yield return new WaitForSeconds(0.01f);
		canClick = true;
	}

	public void UpdateInventoryItemsUI()
	{
	}

	public void PlayChangeSound()
	{
		if (canClick)
		{
			SFXManager.instance.PlaySFX(ChangeSound, 0.5f, null);
			StartCoroutine(ResetClickSound());
			canClick = false;
		}
	}

	private void Update()
	{
		if (inPlayingMode)
		{
			return;
		}
		if ((bool)MenuAddCustomTankButton)
		{
			if (CustomTankDatas.Count >= 40)
			{
				MenuAddCustomTankButton.SetActive(value: false);
			}
			else
			{
				MenuAddCustomTankButton.SetActive(value: true);
			}
		}
		if (!isTesting)
		{
			if (Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				if (CurrentLayer < MaxLayer)
				{
					CurrentLayer++;
				}
			}
			else if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				if (CurrentLayer > 0)
				{
					CurrentLayer--;
				}
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				CurrentLayer = 0;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				CurrentLayer = 1;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				CurrentLayer = 2;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				CurrentLayer = 3;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				CurrentLayer = 4;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				ShowAllLayers = !ShowAllLayers;
			}
		}
		if (MenuCurrent == 6)
		{
			StartingLives = Mathf.RoundToInt(StartLivesSlider.value);
			int num = (OptionsMainMenu.instance.AMselected.Contains(63) ? 1000 : 50);
			StartLivesSlider.maxValue = num;
			if (StartingLives < 1)
			{
				StartingLives = 1;
			}
			else if (StartingLives > num)
			{
				StartingLives = num;
			}
			PlayerSpeed = Mathf.RoundToInt(PlayerSpeedSlider.value);
			int num2 = (OptionsMainMenu.instance.AMselected.Contains(63) ? 1000 : 100);
			PlayerSpeedSlider.maxValue = num2;
			if (PlayerSpeed < 0)
			{
				PlayerSpeed = 0;
			}
			else if (PlayerSpeed > num2)
			{
				PlayerSpeed = num2;
			}
			PlayerMaxBullets = Mathf.RoundToInt(PlayerMaxBulletsSlider.value);
			int num3 = (OptionsMainMenu.instance.AMselected.Contains(63) ? 100 : 10);
			PlayerMaxBulletsSlider.maxValue = num3;
			if (PlayerMaxBullets < 0)
			{
				PlayerMaxBullets = 0;
			}
			else if (PlayerMaxBullets > num3)
			{
				PlayerMaxBullets = num3;
			}
			PlayerAmountBounces = Mathf.RoundToInt(PlayerBouncesSlider.value);
			int num4 = (OptionsMainMenu.instance.AMselected.Contains(63) ? 100 : 10);
			PlayerBouncesSlider.maxValue = num4;
			if (PlayerAmountBounces < 0)
			{
				PlayerAmountBounces = 0;
			}
			else if (PlayerAmountBounces > num4)
			{
				PlayerAmountBounces = num4;
			}
			PlayerArmourPoints = Mathf.RoundToInt(PlayerArmourPointsSlider.value);
			int num5 = (OptionsMainMenu.instance.AMselected.Contains(63) ? 100 : 20);
			PlayerArmourPointsSlider.maxValue = num5;
			if (PlayerArmourPoints < 0)
			{
				PlayerArmourPoints = 0;
			}
			else if (PlayerArmourPoints > num5)
			{
				PlayerArmourPoints = num5;
			}
			PlayerBulletType = PlayerBulletTypeList.value;
			Difficulty = DifficultyList.value;
			PlayerCanLayMines = PlayerCanLayMinesToggle.IsEnabled;
			for (int i = 0; i < TeamColorEnabled.Length; i++)
			{
				TeamColorEnabled[i] = TeamColorsToggles[i].IsEnabled;
			}
		}
		else if (MenuCurrent == 8)
		{
			if (CanLayMinesToggle.IsEnabled && !MineLaySpeedField.transform.parent.gameObject.activeSelf)
			{
				MineLaySpeedField.transform.parent.gameObject.SetActive(value: true);
				CustomTankDatas[SelectedCustomTank].CustomLayMines = true;
			}
			else if (!CanLayMinesToggle.IsEnabled && MineLaySpeedField.transform.parent.gameObject.activeSelf)
			{
				MineLaySpeedField.transform.parent.gameObject.SetActive(value: false);
				CustomTankDatas[SelectedCustomTank].CustomLayMines = false;
			}
			if (ArmouredToggle.IsEnabled && !ArmouredSlider.transform.parent.gameObject.activeSelf)
			{
				ArmouredSlider.transform.parent.gameObject.SetActive(value: true);
				CustomTankDatas[SelectedCustomTank].CustomArmoured = true;
			}
			else if (!ArmouredToggle.IsEnabled && ArmouredSlider.transform.parent.gameObject.activeSelf)
			{
				ArmouredSlider.transform.parent.gameObject.SetActive(value: false);
				CustomTankDatas[SelectedCustomTank].CustomArmoured = false;
			}
			CustomTankDatas[SelectedCustomTank].CanShootAirMissiles = CanAirMissiles.IsEnabled;
			CustomTankDatas[SelectedCustomTank].CustomMissileReloadSpeed = AirMissileReloadSpeed.value;
			CustomTankDatas[SelectedCustomTank].CustomMissileCapacity = Mathf.RoundToInt(AirMissileCapacity.value);
			if (!CanAirMissiles.IsEnabled)
			{
				CustomTankExampleRocketHolder.SetActive(value: false);
			}
			else
			{
				CustomTankExampleRocketHolder.SetActive(value: true);
				for (int j = 0; j < 4; j++)
				{
					CustomTankExampleRockets[j].SetActive(value: false);
				}
				for (int k = 0; k < CustomTankDatas[SelectedCustomTank].CustomMissileCapacity; k++)
				{
					CustomTankExampleRockets[k].SetActive(value: true);
				}
			}
			CustomTankDatas[SelectedCustomTank].CustomTankName = CustomTankInputName.text;
			CustomTankDatas[SelectedCustomTank].CustomMusic = MusicList.value;
			CustomTankDatas[SelectedCustomTank].CustomInvisibility = InvisibilityToggle.IsEnabled;
			CustomTankDatas[SelectedCustomTank].CustomCalculateShots = CalculateShotsToggle.IsEnabled;
			CustomTankDatas[SelectedCustomTank].CustomCanTeleport = CanTeleport.IsEnabled;
			CustomTankDatas[SelectedCustomTank].CustomShowHealthbar = ShowHealthbarToggle.IsEnabled;
			CustomTankDatas[SelectedCustomTank].CustomBulletsPerShot = Mathf.RoundToInt(BulletsPerShotSlider.value);
			CustomTankDatas[SelectedCustomTank].CustomBulletSpeed = Mathf.RoundToInt(BulletSpeedSlider.value);
			CustomTankDatas[SelectedCustomTank].CustomCanBeAirdropped = CanBeAirdroppedToggle.IsEnabled;
			CustomTankDatas[SelectedCustomTank].CustomTankHealth = Mathf.RoundToInt(TankHealthSlider.value);
			CustomTankDatas[SelectedCustomTank].CustomArmourPoints = Mathf.RoundToInt(ArmouredSlider.value);
			ArmouredSlider.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxArmourPointsBoosted : maxArmourPoints);
			if (CustomTankDatas[SelectedCustomTank].CustomArmourPoints < minArmourPoints)
			{
				CustomTankDatas[SelectedCustomTank].CustomArmourPoints = minArmourPoints;
			}
			else if ((float)CustomTankDatas[SelectedCustomTank].CustomArmourPoints > ArmouredSlider.maxValue)
			{
				CustomTankDatas[SelectedCustomTank].CustomArmourPoints = Mathf.RoundToInt(ArmouredSlider.maxValue);
			}
			CustomTankDatas[SelectedCustomTank].CustomTankScale = ScaleSlider.value;
			ScaleSlider.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxScalePointsBoosted : maxScalePoints);
			if (CustomTankDatas[SelectedCustomTank].CustomTankScale < minScalePoints)
			{
				CustomTankDatas[SelectedCustomTank].CustomTankScale = minScalePoints;
			}
			else if (CustomTankDatas[SelectedCustomTank].CustomTankScale > ScaleSlider.maxValue)
			{
				CustomTankDatas[SelectedCustomTank].CustomTankScale = ScaleSlider.maxValue;
			}
			CustomTankDatas[SelectedCustomTank].CustomBulletType = BulletTypeList.value;
			CustomTankDatas[SelectedCustomTank].CustomMusic = MusicList.value;
			CustomTankDatas[SelectedCustomTank].CustomTankSpeed = Mathf.RoundToInt(TankSpeedField.value);
			TankSpeedField.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxTankSpeedBoosted : maxTankSpeed);
			if (CustomTankDatas[SelectedCustomTank].CustomTankSpeed < minTankSpeed)
			{
				CustomTankDatas[SelectedCustomTank].CustomTankSpeed = minTankSpeed;
			}
			else if ((float)CustomTankDatas[SelectedCustomTank].CustomTankSpeed > TankSpeedField.maxValue)
			{
				CustomTankDatas[SelectedCustomTank].CustomTankSpeed = Mathf.RoundToInt(TankSpeedField.maxValue);
			}
			CustomTankDatas[SelectedCustomTank].CustomFireSpeed = Mathf.Floor(FireSpeedField.value * 100f) / 100f;
			if (CustomTankDatas[SelectedCustomTank].CustomFireSpeed < minFireSpeed)
			{
				CustomTankDatas[SelectedCustomTank].CustomFireSpeed = minFireSpeed;
			}
			else if (CustomTankDatas[SelectedCustomTank].CustomFireSpeed > maxFireSpeed)
			{
				CustomTankDatas[SelectedCustomTank].CustomFireSpeed = maxFireSpeed;
			}
			CustomTankDatas[SelectedCustomTank].CustomBounces = Mathf.RoundToInt(AmountBouncesField.value);
			AmountBouncesField.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxBouncesBoosted : maxBounces);
			if (CustomTankDatas[SelectedCustomTank].CustomBounces < minBounces)
			{
				CustomTankDatas[SelectedCustomTank].CustomBounces = minBounces;
			}
			else if ((float)CustomTankDatas[SelectedCustomTank].CustomBounces > AmountBouncesField.maxValue)
			{
				CustomTankDatas[SelectedCustomTank].CustomBounces = Mathf.RoundToInt(AmountBouncesField.maxValue);
			}
			CustomTankDatas[SelectedCustomTank].CustomBullets = Mathf.RoundToInt(AmountBulletsField.value);
			if (CustomTankDatas[SelectedCustomTank].CustomBullets < minBullets)
			{
				CustomTankDatas[SelectedCustomTank].CustomBullets = minBullets;
			}
			else if (CustomTankDatas[SelectedCustomTank].CustomBullets > maxBullets)
			{
				CustomTankDatas[SelectedCustomTank].CustomBullets = maxBullets;
			}
			CustomTankDatas[SelectedCustomTank].CustomMineSpeed = Mathf.Floor(MineLaySpeedField.value * 100f) / 100f;
			if (CustomTankDatas[SelectedCustomTank].CustomMineSpeed < minMineSpeed)
			{
				CustomTankDatas[SelectedCustomTank].CustomMineSpeed = minMineSpeed;
			}
			else if (CustomTankDatas[SelectedCustomTank].CustomMineSpeed > maxMineSpeed)
			{
				CustomTankDatas[SelectedCustomTank].CustomMineSpeed = maxMineSpeed;
			}
			CustomTankDatas[SelectedCustomTank].CustomTurnHead = Mathf.RoundToInt(TurnHeadSlider.value);
			if (CustomTankDatas[SelectedCustomTank].CustomTurnHead < minTurnHead)
			{
				CustomTankDatas[SelectedCustomTank].CustomTurnHead = minTurnHead;
			}
			else if (CustomTankDatas[SelectedCustomTank].CustomTurnHead > maxTurnHead)
			{
				CustomTankDatas[SelectedCustomTank].CustomTurnHead = maxTurnHead;
			}
			CustomTankDatas[SelectedCustomTank].CustomAccuracy = Mathf.RoundToInt(AccuracySlider.value);
			if (CustomTankDatas[SelectedCustomTank].CustomAccuracy < minAccuracy)
			{
				CustomTankDatas[SelectedCustomTank].CustomAccuracy = minAccuracy;
			}
			else if (CustomTankDatas[SelectedCustomTank].CustomAccuracy > maxAccuracy)
			{
				CustomTankDatas[SelectedCustomTank].CustomAccuracy = maxAccuracy;
			}
			for (int l = 0; l < CustomTankExampleRenderers.Length; l++)
			{
				if (l == 0)
				{
					Material[] materials = CustomTankExampleRenderers[l].materials;
					materials[1].color = CustomTankMaterialSource.material.GetColor("_Color1");
					materials[2].color = CustomTankMaterialSource.material.GetColor("_Color1");
					CustomTankExampleRenderers[l].materials = materials;
				}
				else
				{
					Material[] materials2 = CustomTankExampleRenderers[l].materials;
					materials2[0].color = CustomTankMaterialSource.material.GetColor("_Color1");
					CustomTankExampleRenderers[l].materials = materials2;
				}
			}
			CustomTankDatas[SelectedCustomTank].CustomTankColor.Color = CustomTankMaterialSource.material.GetColor("_Color1");
			if (CustomMaterial[SelectedCustomTank].GetColor("_Color") != CustomTankDatas[SelectedCustomTank].CustomTankColor.Color)
			{
				CustomMaterial[SelectedCustomTank].SetColor("_Color", CustomTankDatas[SelectedCustomTank].CustomTankColor.Color);
			}
		}
		if (SelectedPropUITextureMenu != MenuCurrent)
		{
			SelectedPropUITexture.SetActive(value: false);
		}
		else
		{
			SelectedPropUITexture.SetActive(value: true);
		}
		if (OptionsMainMenu.instance != null && !isTesting && (lastKnownMusicVol != OptionsMainMenu.instance.musicVolumeLvl || lastKnownMasterVol != OptionsMainMenu.instance.masterVolumeLvl))
		{
			lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
			lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
			Camera.main.GetComponent<AudioSource>().volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
	}

	private IEnumerator ChangeBack()
	{
		yield return new WaitForSeconds(0.1f);
		canChangeValues = true;
	}

	public void ShowCustomTank(int customtank)
	{
		canChangeValues = false;
		StartCoroutine(ChangeBack());
		CustomTankScript.PropID = 19 + customtank;
		TankSpeedField.value = CustomTankDatas[customtank].CustomTankSpeed;
		FireSpeedField.value = CustomTankDatas[customtank].CustomFireSpeed;
		AmountBouncesField.value = CustomTankDatas[customtank].CustomBounces;
		AmountBulletsField.value = CustomTankDatas[customtank].CustomBullets;
		MineLaySpeedField.value = CustomTankDatas[customtank].CustomMineSpeed;
		TurnHeadSlider.value = CustomTankDatas[customtank].CustomTurnHead;
		AccuracySlider.value = CustomTankDatas[customtank].CustomAccuracy;
		ArmouredSlider.value = CustomTankDatas[customtank].CustomArmourPoints;
		ScaleSlider.value = CustomTankDatas[customtank].CustomTankScale;
		if (FCP != null)
		{
			_ = CustomMaterial[customtank].color;
			FCP.startingColor = CustomMaterial[customtank].color;
			FCP.color = CustomMaterial[customtank].color;
		}
		SelectedCustomTank = customtank;
		CanLayMinesToggle.IsEnabled = CustomTankDatas[customtank].CustomLayMines;
		BulletTypeList.value = CustomTankDatas[customtank].CustomBulletType;
		MusicList.value = CustomTankDatas[customtank].CustomMusic;
		InvisibilityToggle.IsEnabled = CustomTankDatas[customtank].CustomInvisibility;
		CalculateShotsToggle.IsEnabled = CustomTankDatas[customtank].CustomCalculateShots;
		CanTeleport.IsEnabled = CustomTankDatas[customtank].CustomCanTeleport;
		ShowHealthbarToggle.IsEnabled = CustomTankDatas[customtank].CustomShowHealthbar;
		BulletsPerShotSlider.value = CustomTankDatas[customtank].CustomBulletsPerShot;
		BulletSpeedSlider.value = CustomTankDatas[customtank].CustomBulletSpeed;
		CanBeAirdroppedToggle.IsEnabled = CustomTankDatas[customtank].CustomCanBeAirdropped;
		TankHealthSlider.value = CustomTankDatas[customtank].CustomTankHealth;
		ArmouredToggle.IsEnabled = CustomTankDatas[customtank].CustomArmoured;
		ActivateView(2);
		MenuCurrent = 2;
		CustomTankMaterialSource.material.SetColor("_Color1", CustomMaterial[customtank].color);
		for (int i = 0; i < CustomTankExampleRenderers.Length; i++)
		{
			if (i == 0)
			{
				Material[] materials = CustomTankExampleRenderers[i].materials;
				materials[1].color = CustomTankMaterialSource.material.GetColor("_Color1");
				materials[2].color = CustomTankMaterialSource.material.GetColor("_Color1");
				CustomTankExampleRenderers[i].materials = materials;
			}
			else
			{
				Material[] materials2 = CustomTankExampleRenderers[i].materials;
				materials2[0].color = CustomTankMaterialSource.material.GetColor("_Color1");
				CustomTankExampleRenderers[i].materials = materials2;
			}
		}
		CustomTankDatas[SelectedCustomTank].CustomTankColor.Color = CustomTankMaterialSource.material.GetColor("_Color1");
	}

	public void ChangeSlider(int ID)
	{
		ShowIDSlider = ID;
	}

	private void OnGUI()
	{
		if (!Input.GetMouseButton(0) && ShowIDSlider > -1)
		{
			OnCursorText.transform.gameObject.SetActive(value: false);
			ShowIDSlider = -1;
		}
		else if (ShowIDSlider == 0)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomTankSpeed.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 1)
		{
			float customFireSpeed = CustomTankDatas[SelectedCustomTank].CustomFireSpeed;
			OnCursorText.text = customFireSpeed.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 2)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomBounces.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 3)
		{
			float num = Mathf.Round(1f / (10.5f - CustomTankDatas[SelectedCustomTank].CustomMineSpeed) * 100f) / 100f;
			OnCursorText.text = num.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 4)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomTurnHead.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 5)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomAccuracy.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 6)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomArmourPoints.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 7)
		{
			OnCursorText.text = (Mathf.Round(CustomTankDatas[SelectedCustomTank].CustomTankScale * 100f) / 100f).ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 8)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomBullets.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 9)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomBulletsPerShot.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 10)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomTankHealth.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 11)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomBulletSpeed.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 12)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomMissileCapacity.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 13)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomMissileReloadSpeed.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 50)
		{
			OnCursorText.text = StartingLives.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 51)
		{
			OnCursorText.text = PlayerSpeed.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 52)
		{
			OnCursorText.text = PlayerMaxBullets.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 53)
		{
			OnCursorText.text = PlayerArmourPoints.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 54)
		{
			OnCursorText.text = PlayerAmountBounces.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
	}

	public void OnChangeCustomBulletType()
	{
		switch (BulletTypeList.value)
		{
		case 0:
			BulletSpeedSlider.value = 8f;
			break;
		case 1:
			BulletSpeedSlider.value = 14f;
			break;
		case 2:
			BulletSpeedSlider.value = 5f;
			break;
		case 3:
			BulletSpeedSlider.value = 8f;
			break;
		}
	}

	public void PlayAudio(AudioClip AC)
	{
		if (playingAmount < 2)
		{
			playingAmount++;
			SFXManager.instance.PlaySFX(AC);
			StartCoroutine(resetPlayingAmount());
		}
	}

	private IEnumerator resetPlayingAmount()
	{
		yield return new WaitForSeconds(0.1f);
		playingAmount = 0;
	}

	public void LoadData()
	{
		MapEditorData mapEditorData = null;
		if ((bool)OptionsMainMenu.instance.ClassicMap)
		{
			SingleMapEditorData item = SavingMapEditorData.LoadDataFromTXT(OptionsMainMenu.instance.ClassicMap);
			GameObject item2 = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			GameMaster.instance.Levels.Add(item2);
			Levels.Add(item);
			playerOnePlaced.Add(0);
			playerTwoPlaced.Add(0);
			playerThreePlaced.Add(0);
			playerFourPlaced.Add(0);
			enemyTanksPlaced.Add(0);
			StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion: false, 0));
			return;
		}
		mapEditorData = SavingMapEditorData.LoadData(OptionsMainMenu.instance.MapEditorMapName);
		if (mapEditorData == null && !OptionsMainMenu.instance.ClassicMap)
		{
			GameObject item3 = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			GameMaster.instance.Levels.Add(item3);
			CreateNewLevel(isBrandNew: true);
			return;
		}
		int missionAmount = mapEditorData.missionAmount;
		GameMaster.instance.NightLevels = mapEditorData.nightMissions;
		List<string> list = mapEditorData.missionNames.ToList();
		campaignName = mapEditorData.campaignName;
		signedName = mapEditorData.signedName;
		StartingLives = mapEditorData.StartingLives;
		if (mapEditorData.TeamColorsShowing != null)
		{
			TeamColorEnabled = mapEditorData.TeamColorsShowing;
		}
		if (mapEditorData.VersionCreated == "v0.7.9" || mapEditorData.VersionCreated == "v0.7.8")
		{
			PlayerSpeed = 65;
			PlayerMaxBullets = 5;
			PlayerCanLayMines = true;
			PlayerArmourPoints = 0;
			PlayerBulletType = 0;
			PlayerAmountBounces = 1;
		}
		else
		{
			PlayerSpeed = mapEditorData.PTS;
			PlayerMaxBullets = mapEditorData.PMB;
			PlayerCanLayMines = mapEditorData.PCLM;
			PlayerArmourPoints = mapEditorData.PAP;
			PlayerBulletType = mapEditorData.PBT;
			PlayerAmountBounces = mapEditorData.PAB;
		}
		if (!inPlayingMode)
		{
			PlayerSpeedSlider.value = PlayerSpeed;
			PlayerMaxBulletsSlider.value = PlayerMaxBullets;
			PlayerBouncesSlider.value = PlayerAmountBounces;
			PlayerBulletTypeList.value = PlayerBulletType;
			DifficultyList.value = mapEditorData.difficulty;
			PlayerArmourPointsSlider.value = PlayerArmourPoints;
			PlayerCanLayMinesToggle.IsEnabled = PlayerCanLayMines;
		}
		SetCustomTankData(mapEditorData);
		GameObject item4 = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(item4);
		int j;
		for (j = 0; j < missionAmount; j++)
		{
			SingleMapEditorData singleMapEditorData = new SingleMapEditorData(null, null);
			singleMapEditorData.MissionDataProps = mapEditorData.MissionDataProps.FindAll((MapPiecesClass x) => x.missionNumber == j);
			if (mapEditorData.MissionMessages != null && mapEditorData.MissionMessages.Count > 0 && mapEditorData.MissionMessages[j] != null)
			{
				singleMapEditorData.MissionMessage = mapEditorData.MissionMessages[j];
				if (j == 0)
				{
					MissionMessageInputField.SetTextWithoutNotify(mapEditorData.MissionMessages[j]);
				}
			}
			for (int k = 0; k < OptionsMainMenu.instance.MapSize; k++)
			{
				singleMapEditorData.MissionDataProps[k].ID = k;
			}
			GameMaster.instance.MissionNames.Add("Level " + Levels.Count);
			Levels.Add(singleMapEditorData);
			CreateNewLevel(isBrandNew: false);
			if (list.ElementAtOrDefault(j) != null)
			{
				GameMaster.instance.MissionNames[j] = list[j];
			}
			else
			{
				GameMaster.instance.MissionNames[j] = "no name";
			}
		}
		bool oldVersion = false;
		if (mapEditorData.VersionCreated == "v0.7.9" || mapEditorData.VersionCreated == "v0.7.8" || mapEditorData.VersionCreated == "v0.7.10" || mapEditorData.VersionCreated == "v0.7.11" || mapEditorData.VersionCreated == "v0.7.12" || mapEditorData.VersionCreated == "v0.8.0a" || mapEditorData.VersionCreated == "v0.8.0b" || mapEditorData.VersionCreated == "v0.8.0c")
		{
			oldVersion = true;
		}
		StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion, 0));
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(0) ? true : false);
		NoBordersToggler.IsEnabled = (NoBordersMissions.Contains(0) ? true : false);
		PID = mapEditorData.PID;
		IsPublished = mapEditorData.isPublished;
		if (mapEditorData.WeatherTypes != null)
		{
			if (mapEditorData.WeatherTypes.Length != 0)
			{
				if (mapEditorData.WeatherTypes != null && mapEditorData.WeatherTypes.Length != 0)
				{
					for (int l = 0; l < mapEditorData.WeatherTypes.Length; l++)
					{
						int num = mapEditorData.WeatherTypes[l];
						if (num > 0)
						{
							MissionProperties missionProperties = new MissionProperties();
							missionProperties.MissionFloorTexture = 0;
							missionProperties.WeatherType = num;
							missionProperties.MissionNumber = l;
							Properties.Add(missionProperties);
						}
					}
					WeatherList.value = mapEditorData.WeatherTypes[0];
				}
				if (mapEditorData.MissionFloorTextures != null && mapEditorData.MissionFloorTextures.Length != 0)
				{
					int i;
					for (i = 0; i < mapEditorData.MissionFloorTextures.Length; i++)
					{
						int num2 = mapEditorData.MissionFloorTextures[i];
						if (num2 > 0)
						{
							MissionProperties missionProperties2 = Properties.Find((MissionProperties x) => x.MissionNumber == i);
							if (missionProperties2 != null)
							{
								missionProperties2.MissionFloorTexture = num2;
								continue;
							}
							MissionProperties missionProperties3 = new MissionProperties();
							missionProperties3.MissionFloorTexture = num2;
							missionProperties3.WeatherType = 0;
							missionProperties3.MissionNumber = i;
							Properties.Add(missionProperties3);
						}
					}
					FloorDropdown.value = mapEditorData.MissionFloorTextures[0];
				}
			}
			Debug.Log(mapEditorData.WeatherTypes[0]);
		}
		else
		{
			Debug.Log("NEW WAY FOUND!");
			if (mapEditorData.Properties != null && mapEditorData.Properties.Count > 0)
			{
				for (int m = 0; m < mapEditorData.Properties.Count; m++)
				{
					MissionProperties missionProperties4 = new MissionProperties();
					missionProperties4.CurrentFloorName = mapEditorData.Properties[m].CurrentFloorName;
					missionProperties4.MissionFloorTexture = mapEditorData.Properties[m].MissionFloorTexture;
					missionProperties4.WeatherType = mapEditorData.Properties[m].WeatherType;
					missionProperties4.MissionNumber = m;
					Properties.Add(missionProperties4);
				}
				FloorDropdown.value = mapEditorData.Properties[0].MissionFloorTexture;
			}
		}
		if (mapEditorData.NoBordersMissions != null)
		{
			NoBordersMissions = mapEditorData.NoBordersMissions;
			if (NoBordersMissions.Count < 1)
			{
				NoBordersToggler.IsEnabled = true;
			}
			else
			{
				NoBordersToggler.IsEnabled = (NoBordersMissions.Contains(0) ? true : false);
			}
		}
	}

	public void SetCustomTankData(MapEditorData theData)
	{
		if (theData.CTD != null && theData.CTD.Count > 0)
		{
			CustomTankDatas = theData.CTD;
			for (int i = 0; i < theData.CTD.Count; i++)
			{
				Material material = new Material(Shader.Find("Standard"));
				material.color = CustomTankDatas[i].CustomTankColor.Color;
				material.name = "CustomTank";
				CustomMaterial.Add(material);
			}
		}
		else
		{
			if (!(theData.VersionCreated == "0.8.6n") && !(theData.VersionCreated == "0.8.6"))
			{
				return;
			}
			for (int j = 0; j < 3; j++)
			{
				CustomTankDatas.Add(new CustomTankData());
				CustomTankDatas[j].CustomTankSpeed = theData.CustomTankSpeed[j];
				CustomTankDatas[j].CustomFireSpeed = theData.CustomFireSpeed[j];
				CustomTankDatas[j].CustomBounces = theData.CustomBounces[j];
				CustomTankDatas[j].CustomMineSpeed = theData.CustomMineSpeed[j];
				CustomTankDatas[j].CustomTurnHead = theData.CustomTurnHead[j];
				CustomTankDatas[j].CustomAccuracy = theData.CustomAccuracy[j];
				CustomTankDatas[j].CustomLayMines = theData.LayMines[j];
				CustomTankDatas[j].CustomTankColor = theData.CTC[j];
				_ = theData.CTC[j].Color;
				Material item = new Material(Shader.Find("Standard"));
				CustomMaterial.Add(item);
				CustomMaterial[j].color = theData.CTC[j].Color;
				CustomTankDatas[j].CustomBulletType = theData.CustomBulletType[j];
				CustomTankDatas[j].CustomArmoured = theData.CustomArmoured[j];
				CustomTankDatas[j].CustomArmourPoints = theData.CustomArmourPoints[j];
				CustomTankDatas[j].CustomInvisibility = theData.CustomInvisibility[j];
				CustomTankDatas[j].CustomCalculateShots = theData.CustomCalculateShots[j];
				if (theData.CustomBullets != null && theData.CustomBullets.Count > 0)
				{
					CustomTankDatas[j].CustomBullets = theData.CustomBullets[j];
				}
				if (theData.CustomMusic != null && theData.CustomMusic.Count > 0)
				{
					CustomTankDatas[j].CustomMusic = theData.CustomMusic[j];
				}
				if (theData.CustomScalePoints != null && theData.CustomScalePoints.Count > 1)
				{
					CustomTankDatas[j].CustomTankScale = theData.CustomScalePoints[j];
				}
			}
		}
	}

	public IEnumerator PlacePropTimer(float waittime)
	{
		yield return new WaitForSeconds(waittime);
		canPlaceProp = true;
	}

	public IEnumerator PlaceAllProps(List<MapPiecesClass> allPropData, bool oldVersion, int missionnumber)
	{
		int currMission = 0;
		yield return new WaitForSeconds(0.01f);
		GameMaster.instance.Levels[0].SetActive(value: true);
		GameObject[] array = GameObject.FindGameObjectsWithTag("MapeditorField");
		if (enemyTanksPlaced.Count >= missionnumber)
		{
			enemyTanksPlaced[missionnumber] = 0;
		}
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			MapEditorGridPiece MEGP = gameObject.GetComponent<MapEditorGridPiece>();
			MEGP.GetComponent<BoxCollider>().enabled = true;
			MapPiecesClass mapPiecesClass = allPropData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			if (mapPiecesClass == null || mapPiecesClass.propID == null)
			{
				continue;
			}
			if (mapPiecesClass.propID.Length < 3 || oldVersion)
			{
				int num = Convert.ToInt32(mapPiecesClass.propID);
				if (num == -1)
				{
					continue;
				}
				int direction = Convert.ToInt32(mapPiecesClass.propRotation);
				int num2 = Convert.ToInt32(mapPiecesClass.TeamColor);
				if (num <= -1)
				{
					continue;
				}
				MEGP.GetComponent<BoxCollider>().enabled = false;
				switch (num)
				{
				case 4:
					playerOnePlaced[mapPiecesClass.missionNumber]++;
					break;
				case 5:
					playerTwoPlaced[mapPiecesClass.missionNumber]++;
					break;
				case 28:
					playerThreePlaced[mapPiecesClass.missionNumber]++;
					break;
				case 29:
					playerFourPlaced[mapPiecesClass.missionNumber]++;
					break;
				default:
					if (IsATank(num, OnlyEnemies: true))
					{
						enemyTanksPlaced[mapPiecesClass.missionNumber]++;
					}
					break;
				}
				MEGP.MyTeamNumber = num2;
				switch (num)
				{
				case 41:
					MEGP.SpawnInProps(0, direction, num2, 0, 0);
					MEGP.SpawnInProps(41, direction, num2, 1, 0);
					break;
				case 42:
					MEGP.SpawnInProps(1, direction, num2, 0, 0);
					MEGP.SpawnInProps(42, direction, num2, 1, 0);
					break;
				case 43:
					MEGP.SpawnInProps(1, direction, num2, 0, 0);
					MEGP.SpawnInProps(1, direction, num2, 1, 0);
					break;
				case 44:
					MEGP.SpawnInProps(0, direction, num2, 0, 0);
					MEGP.SpawnInProps(0, direction, num2, 1, 0);
					break;
				default:
					MEGP.SpawnInProps(num, direction, num2, 0, 0);
					break;
				}
				continue;
			}
			for (int j = 0; j < 5; j++)
			{
				if (mapPiecesClass.propID[j] > -1)
				{
					if (mapPiecesClass.propID[j] == 4)
					{
						playerOnePlaced[mapPiecesClass.missionNumber]++;
					}
					else if (mapPiecesClass.propID[j] == 5)
					{
						playerTwoPlaced[mapPiecesClass.missionNumber]++;
					}
					else if (mapPiecesClass.propID[j] == 28)
					{
						playerThreePlaced[mapPiecesClass.missionNumber]++;
					}
					else if (mapPiecesClass.propID[j] == 29)
					{
						playerFourPlaced[mapPiecesClass.missionNumber]++;
					}
					else if ((mapPiecesClass.propID[j] > 5 && mapPiecesClass.propID[j] < 40) || mapPiecesClass.propID[j] == 1001 || mapPiecesClass.propID[j] == 1003)
					{
						enemyTanksPlaced[mapPiecesClass.missionNumber]++;
					}
					Convert.ToInt32(mapPiecesClass.SpawnDifficulty);
					int team = -1;
					int direction2 = 0;
					if (mapPiecesClass.TeamColor != null)
					{
						MEGP.MyTeamNumber = mapPiecesClass.TeamColor[j];
						team = mapPiecesClass.TeamColor[j];
					}
					if (mapPiecesClass.propRotation != null)
					{
						direction2 = mapPiecesClass.propRotation[j];
					}
					MEGP.SpawnDifficulty = mapPiecesClass.SpawnDifficulty;
					MEGP.mission = missionnumber;
					MEGP.SpawnInProps(mapPiecesClass.propID[j], direction2, team, j, mapPiecesClass.SpawnDifficulty);
				}
			}
		}
		foreach (GameObject level in GameMaster.instance.Levels)
		{
			if (GameMaster.instance.Levels[currMission] != level)
			{
				level.SetActive(value: false);
			}
		}
	}

	public void MakeButtonSelected(RawImage btn)
	{
		for (int i = 0; i < BarButtons.Length; i++)
		{
			BarButtons[i].texture = BarButtonNotSelected;
		}
		btn.texture = BarButtonSelected;
	}

	public string RandomStringGenerator(int lenght)
	{
		string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		string text2 = "";
		for (int i = 0; i < lenght; i++)
		{
			text2 += text[UnityEngine.Random.Range(0, text.Length)];
		}
		return text2;
	}

	public void ShowMenu(int menu)
	{
		if (menu == 8)
		{
			LoadCustomTankDataUI(SelectedCustomTank);
		}
		ActivateView(menu);
		MenuCurrent = menu;
		if (menu == 4)
		{
			foreach (GameObject item in SpawnedCustomTankUI)
			{
				UnityEngine.Object.Destroy(item);
			}
			for (int i = 0; i < CustomTankDatas.Count; i++)
			{
				if (CustomTankDatas[i] != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(CustomTankMenuItem, CustomTankUIParent);
					gameObject.transform.SetSiblingIndex(i);
					SpawnedCustomTankUI.Add(gameObject);
					MapEditorUIprop component = gameObject.GetComponent<MapEditorUIprop>();
					if ((bool)component)
					{
						component.SecondImage.color = CustomTankDatas[i].CustomTankColor.Color;
						component.PropID = 100 + i;
						ButtonMouseEvents componentInChildren = component.GetComponentInChildren<ButtonMouseEvents>();
						componentInChildren.CustomTankID = i;
						component.myBME = componentInChildren;
					}
				}
			}
		}
		if (menu == 8)
		{
			Menus[8].GetComponent<Animator>().SetBool("ShowMenu", value: true);
		}
		else
		{
			Menus[8].GetComponent<Animator>().SetBool("ShowMenu", value: false);
		}
	}

	public void SaveWeatherType()
	{
		if (canChangeValues)
		{
			MissionProperties missionProperties = Properties.Find((MissionProperties x) => x.MissionNumber == GameMaster.instance.CurrentMission);
			if (missionProperties != null)
			{
				missionProperties.WeatherType = WeatherList.value;
				return;
			}
			MissionProperties missionProperties2 = new MissionProperties();
			missionProperties2.MissionFloorTexture = 0;
			missionProperties2.WeatherType = WeatherList.value;
			missionProperties2.MissionNumber = GameMaster.instance.CurrentMission;
			Properties.Add(missionProperties2);
		}
	}

	public void SaveFloorType()
	{
		if (!canChangeValues)
		{
			return;
		}
		MissionProperties missionProperties = Properties.Find((MissionProperties x) => x.MissionNumber == GameMaster.instance.CurrentMission);
		if (missionProperties != null)
		{
			Debug.Log("NMAME : " + FloorDropdown.options[FloorDropdown.value].text);
			missionProperties.MissionFloorTexture = GlobalAssets.instance.TheFloors.Find((GlobalAssets.Floors x) => x.FloorName == FloorDropdown.options[FloorDropdown.value].text).FloorID;
			missionProperties.CurrentFloorName = FloorDropdown.options[FloorDropdown.value].text;
		}
		else
		{
			MissionProperties missionProperties2 = new MissionProperties();
			missionProperties2.CurrentFloorName = FloorDropdown.options[FloorDropdown.value].text;
			missionProperties2.MissionFloorTexture = GlobalAssets.instance.TheFloors.Find((GlobalAssets.Floors x) => x.FloorName == FloorDropdown.options[FloorDropdown.value].text).FloorID;
			missionProperties2.WeatherType = 0;
			missionProperties2.MissionNumber = GameMaster.instance.CurrentMission;
			Properties.Add(missionProperties2);
		}
		SetFloorTexture();
	}

	private void SetFloorTexture()
	{
		LoadModTexture component = GameMaster.instance.floor.GetComponent<LoadModTexture>();
		if ((!component || !component.IsModded) && GlobalAssets.instance.TheFloors.Find((GlobalAssets.Floors x) => x.FloorName == FloorDropdown.options[FloorDropdown.value].text) != null)
		{
			GameMaster.instance.floor.GetComponent<MeshRenderer>().material = GlobalAssets.instance.TheFloors.Find((GlobalAssets.Floors x) => x.FloorName == FloorDropdown.options[FloorDropdown.value].text).FloorTexture;
		}
	}

	public void ChangeCamera()
	{
		camAnimator.enabled = true;
		if (camAnimator.GetBool("CameraDownEditor"))
		{
			camAnimator.SetBool("CameraDown", value: false);
			camAnimator.SetBool("CameraUp", value: false);
			camAnimator.SetBool("CameraDownEditor", value: false);
			camAnimator.SetBool("CameraUpEditor", value: true);
		}
		else
		{
			camAnimator.SetBool("CameraDown", value: false);
			camAnimator.SetBool("CameraUp", value: false);
			camAnimator.SetBool("CameraDownEditor", value: true);
			camAnimator.SetBool("CameraUpEditor", value: false);
		}
	}

	public bool IsABlock(int ID)
	{
		if (ID == 0 || ID == 1 || ID == 2 || ID == 2002 || ID == 1029)
		{
			return true;
		}
		return false;
	}

	public bool IsATank(int ID, bool OnlyEnemies)
	{
		if (OnlyEnemies)
		{
			if ((ID > 5 && ID < 40) || ID == 46 || (ID >= 100 && ID < 120) || ID == 1000 || ID == 1010 || ID == 1011 || ID == 1012 || ID == 1013 || ID == 2003 || ID == 2004 || ID == 2005)
			{
				return true;
			}
		}
		else if ((ID > 3 && ID < 40) || ID == 28 || ID == 29 || ID == 46 || (ID >= 100 && ID < 120) || ID == 1000 || ID == 1010 || ID == 1011 || ID == 1012 || ID == 1013 || ID == 2003 || ID == 2004 || ID == 2005)
		{
			return true;
		}
		return false;
	}

	public IEnumerator ResetTestLevel(bool skip)
	{
		GameMaster.instance.DestroyTemps();
		if (!skip)
		{
			yield return new WaitForSeconds(1f);
		}
		Debug.Log("Reset test level");
		CloudGeneration.instance.MakeItDay();
		CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(0, force: true));
		for (int i = 0; i < GameMaster.instance.PlayerDown.Count; i++)
		{
			GameMaster.instance.PlayerDown[i] = false;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag("Mine");
		for (int j = 0; j < array.Length; j++)
		{
			UnityEngine.Object.Destroy(array[j]);
		}
		camAnimator.enabled = true;
		Camera.main.GetComponent<AudioSource>().volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		isTesting = false;
		CloudGeneration.instance.StartCoroutine(CloudGeneration.instance.SetWeatherType(0, force: true));
		camAnimator.SetBool("CameraUp", value: true);
		camAnimator.SetBool("CameraDownEditor", value: false);
		camAnimator.SetBool("CameraDown", value: false);
		GetComponent<AudioSource>().Play();
		EditingCanvas.SetActive(value: true);
		GameMaster.instance.GameHasStarted = false;
		GameMaster.instance.AmountPlayersThatNeedRevive = 0;
		ErrorField.transform.localPosition = AnimatorStartHeight;
		for (int k = 0; k < ColorsTeamsPlaced.Length; k++)
		{
			ColorsTeamsPlaced[k] = 0;
		}
		enemyTanksPlaced[GameMaster.instance.CurrentMission] = 0;
		playerOnePlaced[GameMaster.instance.CurrentMission] = 0;
		playerTwoPlaced[GameMaster.instance.CurrentMission] = 0;
		canPlaceProp = false;
		StartCoroutine(PlacePropTimer(2f));
		array = GameObject.FindGameObjectsWithTag("MapeditorField");
		for (int j = 0; j < array.Length; j++)
		{
			MapEditorGridPiece component = array[j].GetComponent<MapEditorGridPiece>();
			for (int l = 0; l < 5; l++)
			{
				if (component.propOnMe[l])
				{
					int team = -1;
					if (component.MyTeamNumber > -1)
					{
						team = component.MyTeamNumber;
					}
					UnityEngine.Object.Destroy(component.myProp[l]);
					component.propOnMe[l] = false;
					component.SpawnInProps(component.myPropID[l], component.rotationDirection[l], team, l, component.SpawnDifficulty);
					if (IsATank(component.myPropID[l], OnlyEnemies: true))
					{
						enemyTanksPlaced[GameMaster.instance.CurrentMission]++;
					}
				}
			}
		}
		GameMaster.instance.DestroyTemps();
		StartCoroutine(GameMaster.instance.TempsDestroy());
		GameMaster.instance.EnemyTankTracksAudio = 0;
		GameMaster.instance.musicScript.StopMusic();
		CDS.gameObject.SetActive(value: false);
	}

	private IEnumerator DisableCam()
	{
		yield return new WaitForSeconds(3f);
		camAnimator.enabled = false;
	}

	private void ActivateView(int tab)
	{
		SFXManager.instance.PlaySFX(MenuSwitch);
		for (int i = 0; i < Menus.Length; i++)
		{
			Menus[i].SetActive(value: false);
		}
		RawImage[] menuImages = MenuImages;
		for (int j = 0; j < menuImages.Length; j++)
		{
			menuImages[j].color = TabNotSelectedColor;
		}
		if (TeamColorsToggles.Length != 0)
		{
			for (int k = 0; k < TeamColorsToggles.Length; k++)
			{
				TeamColorsToggles[k].IsEnabled = TeamColorEnabled[k];
			}
		}
		Menus[tab].SetActive(value: true);
		if (tab <= MenuImages.Length)
		{
			MenuImages[tab].color = TabSelectedColor;
		}
		StartLivesSlider.value = StartingLives;
	}

	public void NewCustomTank()
	{
		Material material = new Material(Shader.Find("Standard"));
		material.name = "CustomTank";
		CustomMaterial.Add(material);
		CustomTankData customTankData = new CustomTankData();
		customTankData.UniqueTankID = RandomStringGenerator(12);
		CustomTankDatas.Add(customTankData);
		SelectedCustomTank = CustomTankDatas.Count - 1;
		ShowMenu(8);
	}

	public void SaveCurrentProps()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("MapeditorField");
		foreach (GameObject gameObject in array)
		{
			MapEditorGridPiece MEGP = gameObject.GetComponent<MapEditorGridPiece>();
			for (int j = 0; j < 5; j++)
			{
				List<MapPiecesClass> missionDataProps = Levels[GameMaster.instance.CurrentMission].MissionDataProps;
				MapPiecesClass mapPiecesClass = missionDataProps.Find((MapPiecesClass x) => x.ID == MEGP.ID);
				bool flag = false;
				if (mapPiecesClass != null && mapPiecesClass.propID != null && mapPiecesClass.propID.Length >= j + 1 && mapPiecesClass.propID[j] > -1)
				{
					flag = true;
				}
				if (!(MEGP.myPropID[j] > -1 || flag) || mapPiecesClass == null)
				{
					continue;
				}
				if (mapPiecesClass.propID == null)
				{
					mapPiecesClass.propID = new int[5];
					for (int k = 0; k < 5; k++)
					{
						mapPiecesClass.propID[k] = -1;
					}
				}
				bool flag2 = true;
				if (MEGP.myPropID[j] == -1)
				{
					for (int l = 0; l < MEGP.myPropID.Length; l++)
					{
						if (MEGP.myPropID[l] > -1)
						{
							flag2 = false;
							break;
						}
					}
				}
				else
				{
					flag2 = false;
				}
				if (flag2)
				{
					mapPiecesClass.propID = null;
				}
				else
				{
					mapPiecesClass.propID[j] = MEGP.myPropID[j];
				}
				if ((bool)MEGP.myMEP[j] && MEGP.myMEP[j].TeamNumber > -1)
				{
					if (mapPiecesClass.TeamColor == null)
					{
						mapPiecesClass.TeamColor = new int[5];
					}
					mapPiecesClass.TeamColor[j] = MEGP.myMEP[j].TeamNumber;
				}
				mapPiecesClass.missionNumber = MEGP.mission;
				if (MEGP.rotationDirection[j] > 0)
				{
					if (mapPiecesClass.propRotation == null)
					{
						mapPiecesClass.propRotation = new int[5];
					}
					mapPiecesClass.propRotation[j] = MEGP.rotationDirection[j];
				}
				if (MEGP.myMEP != null && MEGP.myMEP[j] != null && MEGP.myMEP[j].MyDifficultySpawn > 0)
				{
					mapPiecesClass.SpawnDifficulty = MEGP.myMEP[j].MyDifficultySpawn;
				}
				Levels[GameMaster.instance.CurrentMission].MissionDataProps = missionDataProps;
			}
		}
	}

	public void CreateNewLevelButton()
	{
		NightToggler.IsEnabled = false;
		SFXManager.instance.PlaySFX(NewMapSound);
		CreateNewLevel(isBrandNew: true);
		SwitchLevel(Levels.Count - 1);
	}

	public void CreateNewLevel(bool isBrandNew)
	{
		if (LevelUIList.Count >= maxMission)
		{
			ShowErrorMessage("ERROR: Max missions reached!");
		}
		else
		{
			if (GameMaster.instance.inMapEditor && GameMaster.instance.GameHasPaused)
			{
				return;
			}
			if (isBrandNew)
			{
				SingleMapEditorData singleMapEditorData = new SingleMapEditorData(null, null);
				for (int i = 0; i < OptionsMainMenu.instance.MapSize; i++)
				{
					MapPiecesClass mapPiecesClass = new MapPiecesClass();
					mapPiecesClass.ID = i;
					mapPiecesClass.missionNumber = Levels.Count;
					singleMapEditorData.MissionDataProps.Add(mapPiecesClass);
				}
				Levels.Add(singleMapEditorData);
				GameMaster.instance.MissionNames.Add("Level " + Levels.Count);
			}
			playerOnePlaced.Add(0);
			playerTwoPlaced.Add(0);
			playerThreePlaced.Add(0);
			playerFourPlaced.Add(0);
			enemyTanksPlaced.Add(0);
			if (!inPlayingMode)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(LevelUIListPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
				gameObject.transform.SetParent(ParentObjectList.transform, worldPositionStays: false);
				LevelUIList.Add(gameObject);
				gameObject.transform.SetSiblingIndex(gameObject.transform.parent.transform.childCount - 2);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				gameObject.GetComponent<MapEditorMissionBarUI>().mission = Levels.Count - 1;
				UpdateMembus();
				NoBordersToggler.IsEnabled = true;
				MissionNameInputField.text = GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission];
			}
		}
	}

	private void UpdateMembus()
	{
		if (LevelUIList.Count <= 0)
		{
			return;
		}
		foreach (GameObject levelUI in LevelUIList)
		{
			if ((bool)levelUI)
			{
				MapEditorMissionBarUI component = levelUI.GetComponent<MapEditorMissionBarUI>();
				if ((bool)component)
				{
					component.CheckMaster();
				}
			}
		}
	}

	public void RemoveCurrentObjects()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("MapeditorField");
		for (int i = 0; i < array.Length; i++)
		{
			MapEditorGridPiece component = array[i].GetComponent<MapEditorGridPiece>();
			if ((bool)component)
			{
				component.Reset();
			}
		}
	}

	public void SwitchLevel(int missionNumber)
	{
		SaveCurrentProps();
		OTM.OnCloseMenu();
		RemoveCurrentObjects();
		GameMaster.instance.CurrentMission = missionNumber;
		StartCoroutine(PlaceAllProps(Levels[missionNumber].MissionDataProps, oldVersion: false, missionNumber));
		UpdateMembus();
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(missionNumber) ? true : false);
		NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(missionNumber)) ? true : false);
		UpdateMapBorder(missionNumber);
		MissionNameInputField.text = GameMaster.instance.MissionNames[missionNumber];
		MissionMessageInputField.text = Levels[missionNumber].MissionMessage;
		MissionProperties missionProperties = Properties.Find((MissionProperties x) => x.MissionNumber == missionNumber);
		if (missionProperties != null)
		{
			WeatherList.SetValueWithoutNotify(missionProperties.WeatherType);
			FloorDropdown.SetValueWithoutNotify(missionProperties.MissionFloorTexture);
			SetFloorTexture();
		}
		else
		{
			WeatherList.SetValueWithoutNotify(0);
			FloorDropdown.SetValueWithoutNotify(0);
			SetFloorTexture();
		}
	}

	public void RemoveLevel(int removeMission)
	{
		if (Levels.Count < 2)
		{
			ShowErrorMessage("ERROR: You need to have atleast 1 mission");
			return;
		}
		SaveCurrentProps();
		RemoveCurrentObjects();
		playerOnePlaced.RemoveAt(removeMission);
		playerTwoPlaced.RemoveAt(removeMission);
		playerThreePlaced.RemoveAt(removeMission);
		playerFourPlaced.RemoveAt(removeMission);
		enemyTanksPlaced.RemoveAt(removeMission);
		SFXManager.instance.PlaySFX(NewMapSound);
		Levels.RemoveAt(removeMission);
		ShowPositiveMessage("SUCCES: Level " + (removeMission + 1) + " removed!");
		GameMaster.instance.MissionNames.RemoveAt(removeMission);
		MissionProperties missionProperties = Properties.Find((MissionProperties x) => x.MissionNumber == removeMission + 1);
		if (missionProperties != null)
		{
			Properties.RemoveAll((MissionProperties x) => x.MissionNumber == removeMission);
			missionProperties.MissionNumber--;
		}
		else
		{
			Properties.RemoveAll((MissionProperties x) => x.MissionNumber == removeMission);
		}
		for (int i = 0; i < Properties.Count; i++)
		{
			if (Properties[i].MissionNumber > removeMission + 1)
			{
				Properties[i].MissionNumber--;
			}
		}
		UnityEngine.Object.Destroy(LevelUIList[removeMission]);
		LevelUIList.RemoveAt(removeMission);
		UpdateMembus();
		GameMaster.instance.CurrentMission = 0;
		StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion: false, 0));
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(0) ? true : false);
		NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(0)) ? true : false);
		UpdateMapBorder(0);
		MissionNameInputField.text = GameMaster.instance.MissionNames[0];
		MissionProperties missionProperties2 = Properties.Find((MissionProperties x) => x.MissionNumber == 0);
		if (missionProperties2 != null)
		{
			WeatherList.value = missionProperties2.WeatherType;
			FloorDropdown.value = missionProperties2.MissionFloorTexture;
		}
		else if (Properties.Count > 0)
		{
			WeatherList.value = 0;
			FloorDropdown.value = 0;
		}
		RefreshMissionNumbers();
		StartCoroutine(MembusDelay());
	}

	private void RefreshMissionNumbers()
	{
		for (int i = 0; i < Levels.Count; i++)
		{
			List<MapPiecesClass> missionDataProps = Levels[i].MissionDataProps;
			for (int j = 0; j < missionDataProps.Count; j++)
			{
				missionDataProps[j].missionNumber = i;
			}
			Levels[i].MissionDataProps = missionDataProps;
		}
	}

	private IEnumerator MembusDelay()
	{
		yield return new WaitForSeconds(0.001f);
		UpdateMembus();
	}

	public void UpdateMapBorder(int missionNumber)
	{
		GameObject[] mapBorders;
		if (NoBordersMissions.Contains(missionNumber))
		{
			mapBorders = MapBorders;
			for (int i = 0; i < mapBorders.Length; i++)
			{
				MapBorders component = mapBorders[i].GetComponent<MapBorders>();
				if ((bool)component)
				{
					component.HideMapBorders();
				}
			}
			return;
		}
		mapBorders = MapBorders;
		for (int i = 0; i < mapBorders.Length; i++)
		{
			MapBorders component2 = mapBorders[i].GetComponent<MapBorders>();
			if ((bool)component2)
			{
				component2.ShowMapBorders();
			}
		}
	}

	public void MoveLevel(bool up, int mission)
	{
		SaveCurrentProps();
		RemoveCurrentObjects();
		if (up)
		{
			if (mission != 0)
			{
				int item = playerOnePlaced[mission - 1];
				playerOnePlaced.RemoveAt(mission - 1);
				playerOnePlaced.Insert(mission, item);
				int item2 = playerTwoPlaced[mission - 1];
				playerTwoPlaced.RemoveAt(mission - 1);
				playerTwoPlaced.Insert(mission, item2);
				int item3 = playerThreePlaced[mission - 1];
				playerThreePlaced.RemoveAt(mission - 1);
				playerThreePlaced.Insert(mission, item3);
				int item4 = playerFourPlaced[mission - 1];
				playerFourPlaced.RemoveAt(mission - 1);
				playerFourPlaced.Insert(mission, item4);
				int item5 = enemyTanksPlaced[mission - 1];
				enemyTanksPlaced.RemoveAt(mission - 1);
				enemyTanksPlaced.Insert(mission, item5);
				SingleMapEditorData item6 = Levels[mission - 1];
				Levels.RemoveAt(mission - 1);
				Levels.Insert(mission, item6);
				GameObject gameObject = LevelUIList[mission - 1];
				LevelUIList.RemoveAt(mission - 1);
				LevelUIList.Insert(mission, gameObject);
				gameObject.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
				MissionProperties missionProperties = Properties.Find((MissionProperties x) => x.MissionNumber == mission);
				if (missionProperties != null)
				{
					MissionProperties missionProperties2 = Properties.Find((MissionProperties x) => x.MissionNumber == mission - 1);
					if (missionProperties2 != null)
					{
						missionProperties2.MissionNumber++;
					}
					Debug.Log("FOUND, before: " + missionProperties.MissionNumber + " floor is = " + missionProperties.MissionFloorTexture);
					missionProperties.MissionNumber--;
					Debug.Log("FOUND, after: " + missionProperties.MissionNumber + " floor is = " + missionProperties.MissionFloorTexture);
				}
				else
				{
					MissionProperties missionProperties3 = Properties.Find((MissionProperties x) => x.MissionNumber == mission - 1);
					if (missionProperties3 != null)
					{
						missionProperties3.MissionNumber++;
					}
				}
				if (GameMaster.instance.NightLevels.Contains(mission) && !GameMaster.instance.NightLevels.Contains(mission - 1))
				{
					GameMaster.instance.NightLevels.Remove(mission);
					GameMaster.instance.NightLevels.Add(mission - 1);
				}
				else if (!GameMaster.instance.NightLevels.Contains(mission) && GameMaster.instance.NightLevels.Contains(mission - 1))
				{
					GameMaster.instance.NightLevels.Add(mission);
					GameMaster.instance.NightLevels.Remove(mission - 1);
				}
				if (NoBordersMissions.Contains(mission) && !NoBordersMissions.Contains(mission - 1))
				{
					NoBordersMissions.Remove(mission);
					NoBordersMissions.Add(mission - 1);
				}
				else if (!NoBordersMissions.Contains(mission) && NoBordersMissions.Contains(mission - 1))
				{
					NoBordersMissions.Add(mission);
					NoBordersMissions.Remove(mission - 1);
				}
				string item7 = GameMaster.instance.MissionNames[mission - 1];
				GameMaster.instance.MissionNames.RemoveAt(mission - 1);
				GameMaster.instance.MissionNames.Insert(mission, item7);
				for (int i = 0; i < GameMaster.instance.Levels.Count; i++)
				{
					GameMaster.instance.Levels[i].name = "Level " + (i + 1);
				}
				UpdateMapBorder(mission - 1);
				int num = mission - 1;
				GameMaster.instance.CurrentMission = num;
				StartCoroutine(PlaceAllProps(Levels[num].MissionDataProps, oldVersion: false, 0));
				NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(num) ? true : false);
				NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(num)) ? true : false);
				UpdateMapBorder(num);
				MissionNameInputField.text = GameMaster.instance.MissionNames[num];
				MissionProperties missionProperties4 = Properties.Find((MissionProperties x) => x.MissionNumber == mission);
				if (missionProperties4 != null)
				{
					WeatherList.value = missionProperties4.WeatherType;
					FloorDropdown.value = missionProperties4.MissionFloorTexture;
				}
				else if (Properties.Count > 0)
				{
					WeatherList.value = Properties[0].WeatherType;
					FloorDropdown.value = Properties[0].MissionFloorTexture;
				}
				else
				{
					WeatherList.value = 0;
					FloorDropdown.value = 0;
				}
				RefreshMissionNumbers();
				StartCoroutine(MembusDelay());
				Debug.Log("Moved up!," + mission);
				ShowPositiveMessage("SUCCES: Level " + (mission + 1) + " moved up");
			}
			else
			{
				ShowErrorMessage("ERROR: Can't move this level up");
			}
		}
		else if (mission != Levels.Count - 1)
		{
			int item8 = playerOnePlaced[mission + 1];
			playerOnePlaced.RemoveAt(mission + 1);
			playerOnePlaced.Insert(mission, item8);
			int item9 = playerTwoPlaced[mission + 1];
			playerTwoPlaced.RemoveAt(mission + 1);
			playerTwoPlaced.Insert(mission, item9);
			int item10 = playerThreePlaced[mission + 1];
			playerThreePlaced.RemoveAt(mission + 1);
			playerThreePlaced.Insert(mission, item10);
			int item11 = playerFourPlaced[mission + 1];
			playerFourPlaced.RemoveAt(mission + 1);
			playerFourPlaced.Insert(mission, item11);
			int item12 = enemyTanksPlaced[mission + 1];
			enemyTanksPlaced.RemoveAt(mission + 1);
			enemyTanksPlaced.Insert(mission, item12);
			SingleMapEditorData item13 = Levels[mission + 1];
			Levels.RemoveAt(mission + 1);
			Levels.Insert(mission, item13);
			GameObject gameObject2 = LevelUIList[mission + 1];
			LevelUIList.RemoveAt(mission + 1);
			LevelUIList.Insert(mission, gameObject2);
			gameObject2.transform.SetSiblingIndex(gameObject2.transform.GetSiblingIndex() - 1);
			string item14 = GameMaster.instance.MissionNames[mission + 1];
			GameMaster.instance.MissionNames.RemoveAt(mission + 1);
			GameMaster.instance.MissionNames.Insert(mission, item14);
			MissionProperties missionProperties5 = Properties.Find((MissionProperties x) => x.MissionNumber == mission);
			if (missionProperties5 != null)
			{
				MissionProperties missionProperties6 = Properties.Find((MissionProperties x) => x.MissionNumber == mission + 1);
				if (missionProperties6 != null)
				{
					missionProperties6.MissionNumber--;
				}
				missionProperties5.MissionNumber++;
			}
			else
			{
				MissionProperties missionProperties7 = Properties.Find((MissionProperties x) => x.MissionNumber == mission + 1);
				if (missionProperties7 != null)
				{
					missionProperties7.MissionNumber--;
				}
			}
			if (GameMaster.instance.NightLevels.Contains(mission) && !GameMaster.instance.NightLevels.Contains(mission + 1))
			{
				GameMaster.instance.NightLevels.Remove(mission);
				GameMaster.instance.NightLevels.Add(mission + 1);
			}
			else if (!GameMaster.instance.NightLevels.Contains(mission) && GameMaster.instance.NightLevels.Contains(mission + 1))
			{
				GameMaster.instance.NightLevels.Add(mission);
				GameMaster.instance.NightLevels.Remove(mission + 1);
			}
			if (NoBordersMissions.Contains(mission) && !NoBordersMissions.Contains(mission + 1))
			{
				NoBordersMissions.Remove(mission);
				NoBordersMissions.Add(mission + 1);
			}
			else if (!NoBordersMissions.Contains(mission) && NoBordersMissions.Contains(mission + 1))
			{
				NoBordersMissions.Add(mission);
				NoBordersMissions.Remove(mission + 1);
			}
			for (int j = 0; j < GameMaster.instance.Levels.Count; j++)
			{
				GameMaster.instance.Levels[j].name = "Level " + (j + 1);
			}
			UpdateMapBorder(mission + 1);
			int ToMission = mission + 1;
			GameMaster.instance.CurrentMission = ToMission;
			StartCoroutine(PlaceAllProps(Levels[ToMission].MissionDataProps, oldVersion: false, 0));
			NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(ToMission) ? true : false);
			NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(ToMission)) ? true : false);
			UpdateMapBorder(ToMission);
			MissionNameInputField.text = GameMaster.instance.MissionNames[ToMission];
			MissionProperties missionProperties8 = Properties.Find((MissionProperties x) => x.MissionNumber == ToMission);
			if (missionProperties8 != null)
			{
				WeatherList.SetValueWithoutNotify(missionProperties8.WeatherType);
				FloorDropdown.SetValueWithoutNotify(missionProperties8.MissionFloorTexture);
			}
			else if (Properties.Count > 0)
			{
				WeatherList.SetValueWithoutNotify(Properties[0].WeatherType);
				FloorDropdown.SetValueWithoutNotify(Properties[0].MissionFloorTexture);
			}
			else
			{
				WeatherList.SetValueWithoutNotify(0);
				FloorDropdown.SetValueWithoutNotify(0);
			}
			SetFloorTexture();
			StartCoroutine(MembusDelay());
			RefreshMissionNumbers();
			ShowPositiveMessage("SUCCES: Level " + mission + " moved down");
		}
		else
		{
			ShowErrorMessage("ERROR: Can't move this level down");
		}
		StartCoroutine(MembusDelay());
	}

	private void UpdateMEGPID(MapEditorGridPiece MEGP, bool isDuplicate)
	{
		if (MEGP.mission == MEGP.lastKnownMission)
		{
			return;
		}
		MEGP.lastKnownMission = MEGP.mission;
		if (isDuplicate)
		{
			MapPiecesClass mapPiecesClass = new MapPiecesClass();
			MapPiecesClass mapPiecesClass2 = MissionsData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			mapPiecesClass.missionNumber = MEGP.mission;
			for (int i = 0; i < 5; i++)
			{
				mapPiecesClass.propID[i] = mapPiecesClass2.propID[i];
				mapPiecesClass.propRotation[i] = mapPiecesClass2.propRotation[i];
			}
			MEGP.ID = OptionsMainMenu.instance.MapSize * MEGP.mission + MEGP.ID % OptionsMainMenu.instance.MapSize;
			mapPiecesClass.ID = MEGP.ID;
			MissionsData.Add(mapPiecesClass);
		}
		else
		{
			MapPiecesClass mapPiecesClass3 = MissionsData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			MEGP.ID = OptionsMainMenu.instance.MapSize * MEGP.mission + MEGP.ID % OptionsMainMenu.instance.MapSize;
			mapPiecesClass3.ID = MEGP.ID;
		}
	}

	public void DuplicateLevel(int MissionToDuplicate)
	{
		if (Levels.Count >= maxMission)
		{
			ShowErrorMessage("ERROR: Max missions reached");
			return;
		}
		playerOnePlaced.Insert(MissionToDuplicate, playerOnePlaced[MissionToDuplicate]);
		playerTwoPlaced.Insert(MissionToDuplicate, playerTwoPlaced[MissionToDuplicate]);
		playerThreePlaced.Insert(MissionToDuplicate, playerThreePlaced[MissionToDuplicate]);
		playerFourPlaced.Insert(MissionToDuplicate, playerFourPlaced[MissionToDuplicate]);
		enemyTanksPlaced.Insert(MissionToDuplicate, enemyTanksPlaced[MissionToDuplicate]);
		SFXManager.instance.PlaySFX(NewMapSound);
		SaveCurrentProps();
		RemoveCurrentObjects();
		Debug.Log("duplicating mission " + MissionToDuplicate);
		SingleMapEditorData singleMapEditorData = new SingleMapEditorData(null, null);
		List<MapPiecesClass> missionDataProps = Levels[MissionToDuplicate].MissionDataProps;
		for (int i = 0; i < OptionsMainMenu.instance.MapSize; i++)
		{
			MapPiecesClass mapPiecesClass = new MapPiecesClass();
			mapPiecesClass.ID = i;
			for (int j = 0; j < 5; j++)
			{
				if (missionDataProps[i].propID != null && mapPiecesClass.propID == null)
				{
					mapPiecesClass.propID = new int[5];
				}
				if (missionDataProps[i].propRotation != null && mapPiecesClass.propRotation == null)
				{
					mapPiecesClass.propRotation = new int[5];
				}
				if (missionDataProps[i].TeamColor != null && mapPiecesClass.TeamColor == null)
				{
					mapPiecesClass.TeamColor = new int[5];
				}
				if (missionDataProps[i].propID != null)
				{
					mapPiecesClass.propID[j] = missionDataProps[i].propID[j];
				}
				if (missionDataProps[i].propRotation != null)
				{
					mapPiecesClass.propRotation[j] = missionDataProps[i].propRotation[j];
				}
				if (missionDataProps[i].TeamColor != null)
				{
					mapPiecesClass.TeamColor[j] = missionDataProps[i].TeamColor[j];
				}
				mapPiecesClass.SpawnDifficulty = missionDataProps[i].SpawnDifficulty;
				mapPiecesClass.CustomColor = missionDataProps[i].CustomColor;
			}
			mapPiecesClass.missionNumber = Levels.Count;
			singleMapEditorData.MissionDataProps.Add(mapPiecesClass);
		}
		Levels.Insert(MissionToDuplicate + 1, singleMapEditorData);
		GameMaster.instance.MissionNames.Insert(MissionToDuplicate, GameMaster.instance.MissionNames[MissionToDuplicate]);
		GameObject gameObject = UnityEngine.Object.Instantiate(LevelUIListPrefab);
		gameObject.transform.SetParent(ParentObjectList.transform, worldPositionStays: false);
		LevelUIList.Insert(MissionToDuplicate, gameObject);
		gameObject.transform.SetSiblingIndex(gameObject.transform.parent.transform.childCount - (Levels.Count - MissionToDuplicate) - 1);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<MapEditorMissionBarUI>().mission = MissionToDuplicate + 1;
		MissionProperties missionProperties = Properties.Find((MissionProperties x) => x.MissionNumber == MissionToDuplicate);
		if (missionProperties != null)
		{
			MissionProperties missionProperties2 = new MissionProperties();
			missionProperties2.MissionNumber = missionProperties.MissionNumber + 1;
			missionProperties2.WeatherType = missionProperties.WeatherType;
			missionProperties2.MissionFloorTexture = missionProperties.MissionFloorTexture;
			Properties.Add(missionProperties2);
		}
		for (int k = 0; k < Properties.Count; k++)
		{
			if (Properties[k].MissionNumber > MissionToDuplicate + 1)
			{
				Properties[k].MissionNumber++;
			}
		}
		if (GameMaster.instance.NightLevels.Contains(MissionToDuplicate))
		{
			GameMaster.instance.NightLevels.Add(MissionToDuplicate + 1);
		}
		if (NoBordersMissions.Contains(MissionToDuplicate))
		{
			NoBordersMissions.Add(MissionToDuplicate + 1);
		}
		ShowPositiveMessage("SUCCES: Level " + (MissionToDuplicate + 1) + " duplicated");
		UpdateMapBorder(MissionToDuplicate + 1);
		int ToMission = MissionToDuplicate + 1;
		GameMaster.instance.CurrentMission = ToMission;
		StartCoroutine(PlaceAllProps(Levels[ToMission].MissionDataProps, oldVersion: false, 0));
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(ToMission) ? true : false);
		NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(ToMission)) ? true : false);
		UpdateMapBorder(ToMission);
		MissionNameInputField.text = GameMaster.instance.MissionNames[ToMission];
		MissionProperties missionProperties3 = Properties.Find((MissionProperties x) => x.MissionNumber == ToMission);
		if (missionProperties3 != null)
		{
			WeatherList.value = missionProperties3.WeatherType;
			FloorDropdown.value = missionProperties3.MissionFloorTexture;
		}
		else if (Properties.Count > 0)
		{
			WeatherList.value = Properties[0].WeatherType;
			FloorDropdown.value = Properties[0].MissionFloorTexture;
		}
		else
		{
			WeatherList.value = 0;
			FloorDropdown.value = 0;
		}
		RefreshMissionNumbers();
		StartCoroutine(MembusDelay());
	}

	public void SaveLevelSettings(TMP_InputField theTextFile)
	{
		SFXManager.instance.PlaySFX(ClickSound);
		if (theTextFile.text == "")
		{
			GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission] = "No name";
		}
		else
		{
			GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission] = theTextFile.text;
		}
		UpdateMembus();
	}

	public void SaveLevelSettings2(ButtonMouseEvents BME)
	{
		if (BME.IsEnabled)
		{
			if (!GameMaster.instance.NightLevels.Contains(GameMaster.instance.CurrentMission))
			{
				GameMaster.instance.NightLevels.Add(GameMaster.instance.CurrentMission);
			}
		}
		else if (GameMaster.instance.NightLevels.Contains(GameMaster.instance.CurrentMission))
		{
			GameMaster.instance.NightLevels.Remove(GameMaster.instance.CurrentMission);
		}
	}

	public void SaveLevelSettings3(ButtonMouseEvents BME)
	{
		if (BME.IsEnabled)
		{
			if (NoBordersMissions.Contains(GameMaster.instance.CurrentMission))
			{
				NoBordersMissions.Remove(GameMaster.instance.CurrentMission);
			}
		}
		else if (!NoBordersMissions.Contains(GameMaster.instance.CurrentMission))
		{
			NoBordersMissions.Add(GameMaster.instance.CurrentMission);
		}
		UpdateMapBorder(GameMaster.instance.CurrentMission);
	}

	public void StartTest()
	{
		OptionsMainMenu.instance.currentDifficulty = MapEditorDifficultyList.value;
		SaveCurrentProps();
		GameMaster.instance.DisableGame();
		EditingCanvas.SetActive(value: false);
		GameMaster.instance.AmountEnemyTanks = 0;
		Debug.LogError("Starting game..");
		GameMaster.instance.isResettingTestLevel = false;
		GameMaster.instance.OnlyCompanionLeft = false;
		GetComponent<AudioSource>().Stop();
		if (camAnimator.GetBool("CameraDownEditor"))
		{
			camAnimator.SetBool("CameraDownEditor", value: true);
		}
		else
		{
			camAnimator.SetBool("CameraDownEditor", value: false);
			camAnimator.SetBool("CameraUpEditor", value: false);
		}
		camAnimator.SetBool("CameraUpEditor", value: false);
		camAnimator.SetBool("CameraDown", value: true);
		camAnimator.SetBool("CameraUp", value: false);
		ErrorFieldMessage.transform.localPosition = AnimatorStartHeight;
		ErrorFieldMessage.SetBool("ToTest", value: true);
		isTesting = true;
		CDS.gameObject.SetActive(value: true);
		CDS.start = true;
		TeleportationFields.SetActive(value: true);
		GameMaster.instance.TestLevel();
	}

	public void ShowErrorMessage(string Message)
	{
		ErrorFieldMessage.SetBool("ToTest", value: false);
		ErrorFieldMessage.Play("ErrorMessage", -1, 0f);
		ErrorField.text = Message;
		if (!ErrorFieldMessage.GetBool("ShowMessage"))
		{
			SFXManager.instance.PlaySFX(ErrorSound);
		}
		ErrorFieldMessage.SetBool("ShowMessage", value: true);
		ErrorFieldMessage.SetBool("ShowGoodMessage", value: false);
		StartCoroutine(SetBoolFalse("ShowMessage"));
	}

	public void ShowPositiveMessage(string Message)
	{
		ErrorFieldMessage.SetBool("ToTest", value: false);
		ErrorFieldMessage.Play("GoodMessage", -1, 0f);
		ErrorField.text = Message;
		ErrorFieldMessage.SetBool("ShowGoodMessage", value: true);
		ErrorFieldMessage.SetBool("ShowMessage", value: false);
		SFXManager.instance.PlaySFX(SuccesSound);
		StartCoroutine(SetBoolFalse("ShowGoodMessage"));
	}

	private IEnumerator SetBoolFalse(string Bool)
	{
		yield return new WaitForSeconds(0.1f);
		ErrorFieldMessage.SetBool(Bool, value: false);
	}
}
