using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorMaster : MonoBehaviour
{
	private static MapEditorMaster _instance;

	public List<MapPiecesClass> MissionsData = new List<MapPiecesClass>();

	public bool inPlayingMode = false;

	public int SelectedProp = 0;

	public int maxMission = 20;

	public GameObject MenuItemPrefab;

	public GameObject TankeyTownListParent;

	public GameObject[] Props;

	public float[] PropStartHeight;

	public bool RemoveMode = false;

	public int selectedFields = 0;

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

	public bool isTesting = false;

	public bool canPlaceProp = false;

	public GameObject EditingCanvas;

	public int IsPublished = 0;

	public Material[] FloorMaterials;

	[Header("Layer Editing")]
	public int CurrentLayer = 0;

	public int MaxLayer = 5;

	public bool ShowAllLayers = false;

	[Header("Map editor Levels")]
	public List<SingleMapEditorData> Levels = new List<SingleMapEditorData>();

	public GameObject LevelPrefab;

	public GameObject LevelUIListPrefab;

	public List<GameObject> LevelUIList = new List<GameObject>();

	public string campaignName;

	public string signedName;

	public GameObject[] MapBorders;

	public int[] WeatherTypes;

	public int[] MissionFloorTextures;

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

	private int playingAmount = 0;

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
	public int minTankSpeed = 0;

	public int maxTankSpeed = 100;

	private int maxTankSpeedBoosted = 1000;

	public float minFireSpeed = 0.15f;

	public float maxFireSpeed = 100f;

	public int minBulletSpeed = 1;

	public int maxBulletSpeed = 100;

	public int minBounces = 0;

	public int maxBounces = 10;

	private int maxBouncesBoosted = 100;

	public int minBullets = 0;

	public int maxBullets = 100;

	public float minMineSpeed = 0.5f;

	public float maxMineSpeed = 10f;

	public int minTurnHead = 1;

	public int maxTurnHead = 5;

	public int minAccuracy = 0;

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
	public int Difficulty = 0;

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

	public int PlayerBulletType = 0;

	public int PlayerAmountBounces = 0;

	public int PlayerArmourPoints = 0;

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

	public bool OnTeamsMenu = false;

	public OnTeamsMenu OTM;

	public List<Material> CustomMaterial;

	public GameObject SelectedPropUITexture;

	public int SelectedPropUITextureMenu = 0;

	public int MenuCurrent = 0;

	public Vector3 AnimatorStartHeight;

	public int ID = 0;

	public int ShowIDSlider = -1;

	public bool canDoButton = true;

	public RawImage[] BarButtons;

	public Texture BarButtonNotSelected;

	public Texture BarButtonSelected;

	public Button TankeyTownItemsButton;

	public int[] ColorsTeamsPlaced;

	public int LastPlacedColor;

	public int LastPlacedRotation;

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
		Animator camAnimator = Camera.main.GetComponent<Animator>();
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
			foreach (RawImage img in menuImages)
			{
				img.color = TabNotSelectedColor;
			}
			TeleportationFields.SetActive(value: false);
			AudioSource mySrc = Camera.main.GetComponent<AudioSource>();
			if (OptionsMainMenu.instance != null)
			{
				lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
				lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
				mySrc.volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			}
			mySrc.clip = EditingModeBackgroundMusic;
			mySrc.loop = true;
			mySrc.Play();
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
		yield return new WaitForSeconds(60f);
		if (SavingMapEditorData.AutoSaveMap(GameMaster.instance, instance))
		{
			Debug.Log("AUTO SAVE COMPLETED!");
			ShowPositiveMessage("Auto save completed!");
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
		for (int j = 0; j < TeamColorsToggles.Length; j++)
		{
			TeamColorsToggles[j].IsEnabled = TeamColorEnabled[j];
		}
		Menus[8].transform.position += new Vector3(0f, -3000f, 0f);
		TankeyTownItemsButton.gameObject.SetActive(value: false);
		yield return new WaitForSeconds(0.25f);
		MapEditorDifficultyList.SetValueWithoutNotify(OptionsMainMenu.instance.currentDifficulty);
		MapEditorDifficultyList.value = OptionsMainMenu.instance.currentDifficulty;
		UpdateMembus();
		int AmountOfItems = 0;
		if (AccountMaster.instance.Inventory.InventoryItems != null && AccountMaster.instance.Inventory.InventoryItems.Length != 0)
		{
			int[] inventoryItems = AccountMaster.instance.Inventory.InventoryItems;
			foreach (int item in inventoryItems)
			{
				for (int i = 0; i < GlobalAssets.instance.StockDatabase.Count; i++)
				{
					if (item == GlobalAssets.instance.StockDatabase[i].ItemID && GlobalAssets.instance.StockDatabase[i].IsMapEditorObject)
					{
						ItemsPlayerHas.Add(GlobalAssets.instance.StockDatabase[i]);
						AmountOfItems++;
						GameObject newListItem = UnityEngine.Object.Instantiate(MenuItemPrefab, TankeyTownListParent.transform);
						MapEditorUIprop MEUP = newListItem.GetComponent<MapEditorUIprop>();
						if ((bool)MEUP)
						{
							MEUP.PropID = GlobalAssets.instance.StockDatabase[i].MapEditorPropID;
							MEUP.customTex = GlobalAssets.instance.StockDatabase[i].ItemTexture;
						}
					}
				}
			}
		}
		if (AmountOfItems > 0)
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
			int startingLives = (OptionsMainMenu.instance.AMselected.Contains(63) ? 1000 : 50);
			StartLivesSlider.maxValue = startingLives;
			if (StartingLives < 1)
			{
				StartingLives = 1;
			}
			else if (StartingLives > startingLives)
			{
				StartingLives = startingLives;
			}
			PlayerSpeed = Mathf.RoundToInt(PlayerSpeedSlider.value);
			int maxSpeed = (OptionsMainMenu.instance.AMselected.Contains(63) ? 1000 : 100);
			PlayerSpeedSlider.maxValue = maxSpeed;
			if (PlayerSpeed < 0)
			{
				PlayerSpeed = 0;
			}
			else if (PlayerSpeed > maxSpeed)
			{
				PlayerSpeed = maxSpeed;
			}
			PlayerMaxBullets = Mathf.RoundToInt(PlayerMaxBulletsSlider.value);
			int maxBullets = (OptionsMainMenu.instance.AMselected.Contains(63) ? 100 : 10);
			PlayerMaxBulletsSlider.maxValue = maxBullets;
			if (PlayerMaxBullets < 0)
			{
				PlayerMaxBullets = 0;
			}
			else if (PlayerMaxBullets > maxBullets)
			{
				PlayerMaxBullets = maxBullets;
			}
			PlayerAmountBounces = Mathf.RoundToInt(PlayerBouncesSlider.value);
			int maxBounces = (OptionsMainMenu.instance.AMselected.Contains(63) ? 100 : 10);
			PlayerBouncesSlider.maxValue = maxBounces;
			if (PlayerAmountBounces < 0)
			{
				PlayerAmountBounces = 0;
			}
			else if (PlayerAmountBounces > maxBounces)
			{
				PlayerAmountBounces = maxBounces;
			}
			PlayerArmourPoints = Mathf.RoundToInt(PlayerArmourPointsSlider.value);
			int maxPoints = (OptionsMainMenu.instance.AMselected.Contains(63) ? 100 : 20);
			PlayerArmourPointsSlider.maxValue = maxPoints;
			if (PlayerArmourPoints < 0)
			{
				PlayerArmourPoints = 0;
			}
			else if (PlayerArmourPoints > maxPoints)
			{
				PlayerArmourPoints = maxPoints;
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
				for (int k = 0; k < 4; k++)
				{
					CustomTankExampleRockets[k].SetActive(value: false);
				}
				for (int l = 0; l < CustomTankDatas[SelectedCustomTank].CustomMissileCapacity; l++)
				{
					CustomTankExampleRockets[l].SetActive(value: true);
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
			AmountBouncesField.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxBouncesBoosted : this.maxBounces);
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
			else if (CustomTankDatas[SelectedCustomTank].CustomBullets > this.maxBullets)
			{
				CustomTankDatas[SelectedCustomTank].CustomBullets = this.maxBullets;
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
			for (int j = 0; j < CustomTankExampleRenderers.Length; j++)
			{
				if (j == 0)
				{
					Material[] mats2 = CustomTankExampleRenderers[j].materials;
					mats2[1].color = CustomTankMaterialSource.material.GetColor("_Color1");
					mats2[2].color = CustomTankMaterialSource.material.GetColor("_Color1");
					CustomTankExampleRenderers[j].materials = mats2;
				}
				else
				{
					Material[] mats = CustomTankExampleRenderers[j].materials;
					mats[0].color = CustomTankMaterialSource.material.GetColor("_Color1");
					CustomTankExampleRenderers[j].materials = mats;
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
			AudioSource mySrc = Camera.main.GetComponent<AudioSource>();
			mySrc.volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
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
			if (true)
			{
				FCP.startingColor = CustomMaterial[customtank].color;
				FCP.color = CustomMaterial[customtank].color;
			}
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
				Material[] mats2 = CustomTankExampleRenderers[i].materials;
				mats2[1].color = CustomTankMaterialSource.material.GetColor("_Color1");
				mats2[2].color = CustomTankMaterialSource.material.GetColor("_Color1");
				CustomTankExampleRenderers[i].materials = mats2;
			}
			else
			{
				Material[] mats = CustomTankExampleRenderers[i].materials;
				mats[0].color = CustomTankMaterialSource.material.GetColor("_Color1");
				CustomTankExampleRenderers[i].materials = mats;
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
			float redoneData = CustomTankDatas[SelectedCustomTank].CustomFireSpeed;
			OnCursorText.text = redoneData.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 2)
		{
			OnCursorText.text = CustomTankDatas[SelectedCustomTank].CustomBounces.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		else if (ShowIDSlider == 3)
		{
			float redoneData2 = Mathf.Round(1f / (10.5f - CustomTankDatas[SelectedCustomTank].CustomMineSpeed) * 100f) / 100f;
			OnCursorText.text = redoneData2.ToString();
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
		MapEditorData theData = null;
		if ((bool)OptionsMainMenu.instance.ClassicMap)
		{
			SingleMapEditorData SMED = SavingMapEditorData.LoadDataFromTXT(OptionsMainMenu.instance.ClassicMap);
			GameObject newLevels = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			GameMaster.instance.Levels.Add(newLevels);
			Levels.Add(SMED);
			playerOnePlaced.Add(0);
			playerTwoPlaced.Add(0);
			playerThreePlaced.Add(0);
			playerFourPlaced.Add(0);
			enemyTanksPlaced.Add(0);
			StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion: false, 0));
			return;
		}
		theData = SavingMapEditorData.LoadData(OptionsMainMenu.instance.MapEditorMapName);
		if (theData == null && !OptionsMainMenu.instance.ClassicMap)
		{
			Debug.Log("no data found");
			GameObject singleLevel = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			GameMaster.instance.Levels.Add(singleLevel);
			CreateNewLevel(isBrandNew: true);
			return;
		}
		int missions = theData.missionAmount;
		Debug.LogError("MISSION AMOUNT IS:" + missions);
		GameMaster.instance.NightLevels = theData.nightMissions;
		List<string> thenames = theData.missionNames.ToList();
		campaignName = theData.campaignName;
		signedName = theData.signedName;
		StartingLives = theData.StartingLives;
		if (theData.TeamColorsShowing != null)
		{
			TeamColorEnabled = theData.TeamColorsShowing;
		}
		if (theData.VersionCreated == "v0.7.9" || theData.VersionCreated == "v0.7.8")
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
			PlayerSpeed = theData.PTS;
			PlayerMaxBullets = theData.PMB;
			PlayerCanLayMines = theData.PCLM;
			PlayerArmourPoints = theData.PAP;
			PlayerBulletType = theData.PBT;
			PlayerAmountBounces = theData.PAB;
		}
		if (!inPlayingMode)
		{
			PlayerSpeedSlider.value = PlayerSpeed;
			PlayerMaxBulletsSlider.value = PlayerMaxBullets;
			PlayerBouncesSlider.value = PlayerAmountBounces;
			PlayerBulletTypeList.value = PlayerBulletType;
			DifficultyList.value = theData.difficulty;
			PlayerArmourPointsSlider.value = PlayerArmourPoints;
			PlayerCanLayMinesToggle.IsEnabled = PlayerCanLayMines;
		}
		SetCustomTankData(theData);
		GameObject newLevel = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(newLevel);
		for (int j = 0; j < theData.MissionMessages.Count; j++)
		{
			Debug.Log(theData.MissionMessages[j]);
		}
		int i;
		for (i = 0; i < missions; i++)
		{
			SingleMapEditorData newSMED = new SingleMapEditorData(null, null);
			newSMED.MissionDataProps = theData.MissionDataProps.FindAll((MapPiecesClass x) => x.missionNumber == i);
			if (theData.MissionMessages != null && theData.MissionMessages.Count > 0 && theData.MissionMessages[i] != null)
			{
				newSMED.MissionMessage = theData.MissionMessages[i];
				if (i == 0)
				{
					MissionMessageInputField.SetTextWithoutNotify(theData.MissionMessages[i]);
				}
			}
			for (int k = 0; k < OptionsMainMenu.instance.MapSize; k++)
			{
				newSMED.MissionDataProps[k].ID = k;
			}
			GameMaster.instance.MissionNames.Add("Level " + Levels.Count);
			Levels.Add(newSMED);
			CreateNewLevel(isBrandNew: false);
			if (thenames.ElementAtOrDefault(i) != null)
			{
				GameMaster.instance.MissionNames[i] = thenames[i];
			}
			else
			{
				GameMaster.instance.MissionNames[i] = "no name";
			}
		}
		bool oldVersion = false;
		if (theData.VersionCreated == "v0.7.9" || theData.VersionCreated == "v0.7.8" || theData.VersionCreated == "v0.7.10" || theData.VersionCreated == "v0.7.11" || theData.VersionCreated == "v0.7.12" || theData.VersionCreated == "v0.8.0a" || theData.VersionCreated == "v0.8.0b" || theData.VersionCreated == "v0.8.0c")
		{
			oldVersion = true;
		}
		StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion, 0));
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(0) ? true : false);
		NoBordersToggler.IsEnabled = (NoBordersMissions.Contains(0) ? true : false);
		PID = theData.PID;
		IsPublished = theData.isPublished;
		if (theData.WeatherTypes != null && theData.WeatherTypes.Length != 0)
		{
			WeatherTypes = theData.WeatherTypes;
			WeatherList.value = WeatherTypes[0];
		}
		if (theData.MissionFloorTextures != null && theData.MissionFloorTextures.Length != 0)
		{
			MissionFloorTextures = theData.MissionFloorTextures;
			FloorDropdown.value = MissionFloorTextures[0];
		}
		if (theData.NoBordersMissions != null)
		{
			NoBordersMissions = theData.NoBordersMissions;
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
			for (int j = 0; j < theData.CTD.Count; j++)
			{
				Material M2 = new Material(Shader.Find("Standard"));
				M2.color = CustomTankDatas[j].CustomTankColor.Color;
				M2.name = "CustomTank";
				CustomMaterial.Add(M2);
			}
		}
		else
		{
			if (!(theData.VersionCreated == "0.8.6n") && !(theData.VersionCreated == "0.8.6"))
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				CustomTankDatas.Add(new CustomTankData());
				CustomTankDatas[i].CustomTankSpeed = theData.CustomTankSpeed[i];
				CustomTankDatas[i].CustomFireSpeed = theData.CustomFireSpeed[i];
				CustomTankDatas[i].CustomBounces = theData.CustomBounces[i];
				CustomTankDatas[i].CustomMineSpeed = theData.CustomMineSpeed[i];
				CustomTankDatas[i].CustomTurnHead = theData.CustomTurnHead[i];
				CustomTankDatas[i].CustomAccuracy = theData.CustomAccuracy[i];
				CustomTankDatas[i].CustomLayMines = theData.LayMines[i];
				CustomTankDatas[i].CustomTankColor = theData.CTC[i];
				_ = theData.CTC[i].Color;
				if (true)
				{
					Material M = new Material(Shader.Find("Standard"));
					CustomMaterial.Add(M);
					CustomMaterial[i].color = theData.CTC[i].Color;
				}
				CustomTankDatas[i].CustomBulletType = theData.CustomBulletType[i];
				CustomTankDatas[i].CustomArmoured = theData.CustomArmoured[i];
				CustomTankDatas[i].CustomArmourPoints = theData.CustomArmourPoints[i];
				CustomTankDatas[i].CustomInvisibility = theData.CustomInvisibility[i];
				CustomTankDatas[i].CustomCalculateShots = theData.CustomCalculateShots[i];
				if (theData.CustomBullets != null && theData.CustomBullets.Count > 0)
				{
					CustomTankDatas[i].CustomBullets = theData.CustomBullets[i];
				}
				if (theData.CustomMusic != null && theData.CustomMusic.Count > 0)
				{
					CustomTankDatas[i].CustomMusic = theData.CustomMusic[i];
				}
				if (theData.CustomScalePoints != null && theData.CustomScalePoints.Count > 1)
				{
					CustomTankDatas[i].CustomTankScale = theData.CustomScalePoints[i];
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
		GameObject[] allGridPieces = GameObject.FindGameObjectsWithTag("MapeditorField");
		GameObject[] array = allGridPieces;
		foreach (GameObject piece in array)
		{
			MapEditorGridPiece MEGP = piece.GetComponent<MapEditorGridPiece>();
			MapPiecesClass myClass = allPropData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			if (myClass == null)
			{
				continue;
			}
			if (myClass.propID.Length < 3 || oldVersion)
			{
				int propID = Convert.ToInt32(myClass.propID);
				if (propID == -1)
				{
					continue;
				}
				int propRotation = Convert.ToInt32(myClass.propRotation);
				int propTeam = Convert.ToInt32(myClass.TeamColor);
				if (propID > -1)
				{
					if (propID == 4)
					{
						playerOnePlaced[myClass.missionNumber]++;
					}
					else if (propID == 5)
					{
						playerTwoPlaced[myClass.missionNumber]++;
					}
					else if (propID == 28)
					{
						playerThreePlaced[myClass.missionNumber]++;
					}
					else if (propID == 29)
					{
						playerFourPlaced[myClass.missionNumber]++;
					}
					else if (propID > 5 && propID < 40)
					{
						enemyTanksPlaced[myClass.missionNumber]++;
					}
					MEGP.MyTeamNumber = propTeam;
					switch (propID)
					{
					case 41:
						MEGP.SpawnInProps(0, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(41, propRotation, propTeam, 1, 0);
						break;
					case 42:
						MEGP.SpawnInProps(1, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(42, propRotation, propTeam, 1, 0);
						break;
					case 43:
						MEGP.SpawnInProps(1, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(1, propRotation, propTeam, 1, 0);
						break;
					case 44:
						MEGP.SpawnInProps(0, propRotation, propTeam, 0, 0);
						MEGP.SpawnInProps(0, propRotation, propTeam, 1, 0);
						break;
					default:
						MEGP.SpawnInProps(propID, propRotation, propTeam, 0, 0);
						break;
					}
				}
				continue;
			}
			for (int i = 0; i < 5; i++)
			{
				if (myClass.propID[i] > -1)
				{
					if (myClass.propID[i] == 4)
					{
						playerOnePlaced[myClass.missionNumber]++;
					}
					else if (myClass.propID[i] == 5)
					{
						playerTwoPlaced[myClass.missionNumber]++;
					}
					else if (myClass.propID[i] == 28)
					{
						playerThreePlaced[myClass.missionNumber]++;
					}
					else if (myClass.propID[i] == 29)
					{
						playerFourPlaced[myClass.missionNumber]++;
					}
					else if ((myClass.propID[i] > 5 && myClass.propID[i] < 40) || myClass.propID[i] == 1001 || myClass.propID[i] == 1003)
					{
						enemyTanksPlaced[myClass.missionNumber]++;
					}
					Convert.ToInt32(myClass.SpawnDifficulty);
					MEGP.MyTeamNumber = myClass.TeamColor[i];
					MEGP.SpawnDifficulty = myClass.SpawnDifficulty;
					MEGP.mission = missionnumber;
					MEGP.SpawnInProps(myClass.propID[i], myClass.propRotation[i], myClass.TeamColor[i], i, myClass.SpawnDifficulty);
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
		string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		string generated_string = "";
		for (int i = 0; i < lenght; i++)
		{
			generated_string += characters[UnityEngine.Random.Range(0, characters.Length)];
		}
		return generated_string;
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
					GameObject NewUITank = UnityEngine.Object.Instantiate(CustomTankMenuItem, CustomTankUIParent);
					NewUITank.transform.SetSiblingIndex(i);
					SpawnedCustomTankUI.Add(NewUITank);
					MapEditorUIprop MEUP = NewUITank.GetComponent<MapEditorUIprop>();
					if ((bool)MEUP)
					{
						MEUP.SecondImage.color = CustomTankDatas[i].CustomTankColor.Color;
						MEUP.PropID = 100 + i;
						ButtonMouseEvents BME = MEUP.GetComponentInChildren<ButtonMouseEvents>();
						BME.CustomTankID = i;
						MEUP.myBME = BME;
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
			WeatherTypes[GameMaster.instance.CurrentMission] = WeatherList.value;
		}
	}

	public void SaveFloorType()
	{
		if (canChangeValues)
		{
			MissionFloorTextures[GameMaster.instance.CurrentMission] = FloorDropdown.value;
			LoadModTexture LMT = GameMaster.instance.floor.GetComponent<LoadModTexture>();
			if (!LMT || !LMT.IsModded)
			{
				GameMaster.instance.floor.GetComponent<MeshRenderer>().material = FloorMaterials[MissionFloorTextures[GameMaster.instance.CurrentMission]];
			}
		}
	}

	public void ChangeCamera()
	{
		Animator camAnimator = Camera.main.GetComponent<Animator>();
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

	private void ActivateView(int tab)
	{
		SFXManager.instance.PlaySFX(MenuSwitch);
		for (int j = 0; j < Menus.Length; j++)
		{
			Menus[j].SetActive(value: false);
		}
		RawImage[] menuImages = MenuImages;
		foreach (RawImage img in menuImages)
		{
			img.color = TabNotSelectedColor;
		}
		if (TeamColorsToggles.Length != 0)
		{
			for (int i = 0; i < TeamColorsToggles.Length; i++)
			{
				TeamColorsToggles[i].IsEnabled = TeamColorEnabled[i];
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
		Material M = new Material(Shader.Find("Standard"));
		M.name = "CustomTank";
		CustomMaterial.Add(M);
		CustomTankData NewCustomTank = new CustomTankData();
		NewCustomTank.UniqueTankID = RandomStringGenerator(12);
		CustomTankDatas.Add(NewCustomTank);
		SelectedCustomTank = CustomTankDatas.Count - 1;
		ShowMenu(8);
	}

	public void SaveCurrentProps()
	{
		GameObject[] allMapEditorFields = GameObject.FindGameObjectsWithTag("MapeditorField");
		GameObject[] array = allMapEditorFields;
		foreach (GameObject field in array)
		{
			MapEditorGridPiece MEGP = field.GetComponent<MapEditorGridPiece>();
			for (int i = 0; i < 5; i++)
			{
				if (MEGP.myPropID[i] > -1)
				{
					List<MapPiecesClass> TempList2 = Levels[GameMaster.instance.CurrentMission].MissionDataProps;
					MapPiecesClass MPC2 = TempList2.Find((MapPiecesClass x) => x.ID == MEGP.ID);
					if (MPC2 != null)
					{
						MPC2.propID[i] = MEGP.myPropID[i];
						if ((bool)MEGP.myMEP[i])
						{
							MPC2.TeamColor[i] = MEGP.myMEP[i].TeamNumber;
						}
						else
						{
							MPC2.TeamColor[i] = -1;
						}
						MPC2.missionNumber = MEGP.mission;
						MPC2.propRotation[i] = MEGP.rotationDirection[i];
						MPC2.SpawnDifficulty = MEGP.myMEP[i].MyDifficultySpawn;
						Levels[GameMaster.instance.CurrentMission].MissionDataProps = TempList2;
					}
				}
				else
				{
					List<MapPiecesClass> TempList = Levels[GameMaster.instance.CurrentMission].MissionDataProps;
					MapPiecesClass MPC = TempList.Find((MapPiecesClass x) => x.ID == MEGP.ID);
					MPC.propID[i] = -1;
				}
			}
		}
	}

	public void CreateNewLevelButton()
	{
		NightToggler.IsEnabled = false;
		Debug.Log("new map button clicked!");
		SFXManager.instance.PlaySFX(NewMapSound);
		CreateNewLevel(isBrandNew: true);
		SwitchLevel(Levels.Count - 1);
	}

	public void CreateNewLevel(bool isBrandNew)
	{
		Debug.LogWarning("New Level!");
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
				SingleMapEditorData newSMED = new SingleMapEditorData(null, null);
				for (int i = 0; i < OptionsMainMenu.instance.MapSize; i++)
				{
					MapPiecesClass newMPC = new MapPiecesClass();
					newMPC.ID = i;
					for (int j = 0; j < 5; j++)
					{
						newMPC.propID[j] = -1;
					}
					newMPC.missionNumber = Levels.Count;
					newSMED.MissionDataProps.Add(newMPC);
				}
				Levels.Add(newSMED);
				GameMaster.instance.MissionNames.Add("Level " + Levels.Count);
			}
			playerOnePlaced.Add(0);
			playerTwoPlaced.Add(0);
			playerThreePlaced.Add(0);
			playerFourPlaced.Add(0);
			enemyTanksPlaced.Add(0);
			if (!inPlayingMode)
			{
				GameObject newMissionBar = UnityEngine.Object.Instantiate(LevelUIListPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
				newMissionBar.transform.SetParent(ParentObjectList.transform, worldPositionStays: false);
				LevelUIList.Add(newMissionBar);
				newMissionBar.transform.SetSiblingIndex(newMissionBar.transform.parent.transform.childCount - 2);
				newMissionBar.transform.localScale = new Vector3(1f, 1f, 1f);
				MapEditorMissionBarUI MEMBU = newMissionBar.GetComponent<MapEditorMissionBarUI>();
				MEMBU.mission = Levels.Count - 1;
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
		foreach (GameObject uilist in LevelUIList)
		{
			if ((bool)uilist)
			{
				MapEditorMissionBarUI MEMBU = uilist.GetComponent<MapEditorMissionBarUI>();
				if ((bool)MEMBU)
				{
					MEMBU.CheckMaster();
				}
			}
		}
	}

	public void RemoveCurrentObjects()
	{
		GameObject[] allGridPieces = GameObject.FindGameObjectsWithTag("MapeditorField");
		GameObject[] array = allGridPieces;
		foreach (GameObject piece in array)
		{
			MapEditorGridPiece MEGP = piece.GetComponent<MapEditorGridPiece>();
			if ((bool)MEGP)
			{
				MEGP.Reset();
			}
		}
	}

	public void SwitchLevel(int missionNumber)
	{
		Debug.Log("switching level to " + missionNumber);
		SaveCurrentProps();
		OTM.OnCloseMenu();
		RemoveCurrentObjects();
		Debug.Log("Level was" + GameMaster.instance.CurrentMission);
		GameMaster.instance.CurrentMission = missionNumber;
		Debug.Log("Level is now" + GameMaster.instance.CurrentMission);
		StartCoroutine(PlaceAllProps(Levels[missionNumber].MissionDataProps, oldVersion: false, missionNumber));
		UpdateMembus();
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(missionNumber) ? true : false);
		NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(missionNumber)) ? true : false);
		UpdateMapBorder(missionNumber);
		MissionNameInputField.text = GameMaster.instance.MissionNames[missionNumber];
		MissionMessageInputField.text = Levels[missionNumber].MissionMessage;
		WeatherList.value = WeatherTypes[missionNumber];
		FloorDropdown.value = MissionFloorTextures[missionNumber];
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
		int thisWeatherType = WeatherTypes[removeMission];
		WeatherTypes[removeMission] = WeatherTypes[removeMission + 1];
		WeatherTypes[removeMission + 1] = thisWeatherType;
		int floorType = MissionFloorTextures[removeMission];
		MissionFloorTextures[removeMission] = MissionFloorTextures[removeMission + 1];
		MissionFloorTextures[removeMission + 1] = floorType;
		UnityEngine.Object.Destroy(LevelUIList[removeMission]);
		LevelUIList.RemoveAt(removeMission);
		UpdateMembus();
		GameMaster.instance.CurrentMission = 0;
		StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion: false, 0));
		NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(0) ? true : false);
		NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(0)) ? true : false);
		UpdateMapBorder(0);
		MissionNameInputField.text = GameMaster.instance.MissionNames[0];
		WeatherList.value = WeatherTypes[0];
		FloorDropdown.value = MissionFloorTextures[0];
		StartCoroutine(MembusDelay());
	}

	private IEnumerator MembusDelay()
	{
		yield return new WaitForSeconds(0.001f);
		UpdateMembus();
	}

	public void UpdateMapBorder(int missionNumber)
	{
		if (NoBordersMissions.Contains(missionNumber))
		{
			Debug.Log("CONTAINED : " + missionNumber);
			GameObject[] mapBorders = MapBorders;
			foreach (GameObject border2 in mapBorders)
			{
				MapBorders MB2 = border2.GetComponent<MapBorders>();
				if ((bool)MB2)
				{
					MB2.HideMapBorders();
				}
			}
			return;
		}
		Debug.Log("Showing bordars ");
		GameObject[] mapBorders2 = MapBorders;
		foreach (GameObject border in mapBorders2)
		{
			MapBorders MB = border.GetComponent<MapBorders>();
			if ((bool)MB)
			{
				MB.ShowMapBorders();
			}
		}
	}

	public void MoveLevel(bool up, int mission)
	{
		SaveCurrentProps();
		RemoveCurrentObjects();
		Debug.LogError("MOVED LEEVLE");
		if (up)
		{
			if (mission != 0)
			{
				int item = playerOnePlaced[mission - 1];
				playerOnePlaced.RemoveAt(mission - 1);
				playerOnePlaced.Insert(mission, item);
				int item3 = playerTwoPlaced[mission - 1];
				playerTwoPlaced.RemoveAt(mission - 1);
				playerTwoPlaced.Insert(mission, item3);
				int item5 = playerThreePlaced[mission - 1];
				playerThreePlaced.RemoveAt(mission - 1);
				playerThreePlaced.Insert(mission, item5);
				int item7 = playerFourPlaced[mission - 1];
				playerFourPlaced.RemoveAt(mission - 1);
				playerFourPlaced.Insert(mission, item7);
				int enemies = enemyTanksPlaced[mission - 1];
				enemyTanksPlaced.RemoveAt(mission - 1);
				enemyTanksPlaced.Insert(mission, enemies);
				SingleMapEditorData level = Levels[mission - 1];
				Levels.RemoveAt(mission - 1);
				Levels.Insert(mission, level);
				GameObject UIitem = LevelUIList[mission - 1];
				LevelUIList.RemoveAt(mission - 1);
				LevelUIList.Insert(mission, UIitem);
				UIitem.transform.SetSiblingIndex(UIitem.transform.GetSiblingIndex() + 1);
				int thisWeatherType = WeatherTypes[mission];
				WeatherTypes[mission] = WeatherTypes[mission - 1];
				WeatherTypes[mission - 1] = thisWeatherType;
				int floorType = MissionFloorTextures[mission];
				MissionFloorTextures[mission] = MissionFloorTextures[mission - 1];
				MissionFloorTextures[mission - 1] = floorType;
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
				string levelname = GameMaster.instance.MissionNames[mission - 1];
				GameMaster.instance.MissionNames.RemoveAt(mission - 1);
				GameMaster.instance.MissionNames.Insert(mission, levelname);
				for (int j = 0; j < GameMaster.instance.Levels.Count; j++)
				{
					GameMaster.instance.Levels[j].name = "Level " + (j + 1);
				}
				UpdateMapBorder(mission - 1);
				int ToMission2 = mission - 1;
				GameMaster.instance.CurrentMission = ToMission2;
				StartCoroutine(PlaceAllProps(Levels[ToMission2].MissionDataProps, oldVersion: false, 0));
				NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(ToMission2) ? true : false);
				NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(ToMission2)) ? true : false);
				UpdateMapBorder(ToMission2);
				MissionNameInputField.text = GameMaster.instance.MissionNames[ToMission2];
				WeatherList.value = WeatherTypes[ToMission2];
				FloorDropdown.value = MissionFloorTextures[ToMission2];
				StartCoroutine(MembusDelay());
				ShowPositiveMessage("SUCCES: Level " + (mission + 2) + " moved up");
			}
			else
			{
				ShowErrorMessage("ERROR: Can't move this level up");
			}
		}
		else if (mission != Levels.Count - 1)
		{
			int item2 = playerOnePlaced[mission + 1];
			playerOnePlaced.RemoveAt(mission + 1);
			playerOnePlaced.Insert(mission, item2);
			int item4 = playerTwoPlaced[mission + 1];
			playerTwoPlaced.RemoveAt(mission + 1);
			playerTwoPlaced.Insert(mission, item4);
			int item6 = playerThreePlaced[mission + 1];
			playerThreePlaced.RemoveAt(mission + 1);
			playerThreePlaced.Insert(mission, item6);
			int item8 = playerFourPlaced[mission + 1];
			playerFourPlaced.RemoveAt(mission + 1);
			playerFourPlaced.Insert(mission, item8);
			int enemies2 = enemyTanksPlaced[mission + 1];
			enemyTanksPlaced.RemoveAt(mission + 1);
			enemyTanksPlaced.Insert(mission, enemies2);
			SingleMapEditorData level2 = Levels[mission + 1];
			Levels.RemoveAt(mission + 1);
			Levels.Insert(mission, level2);
			GameObject UIitem2 = LevelUIList[mission + 1];
			LevelUIList.RemoveAt(mission + 1);
			LevelUIList.Insert(mission, UIitem2);
			UIitem2.transform.SetSiblingIndex(UIitem2.transform.GetSiblingIndex() - 1);
			string levelname2 = GameMaster.instance.MissionNames[mission + 1];
			GameMaster.instance.MissionNames.RemoveAt(mission + 1);
			GameMaster.instance.MissionNames.Insert(mission, levelname2);
			int thisWeatherType2 = WeatherTypes[mission];
			WeatherTypes[mission] = WeatherTypes[mission + 1];
			WeatherTypes[mission + 1] = thisWeatherType2;
			int floorType2 = MissionFloorTextures[mission];
			MissionFloorTextures[mission] = MissionFloorTextures[mission + 1];
			MissionFloorTextures[mission + 1] = floorType2;
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
			for (int i = 0; i < GameMaster.instance.Levels.Count; i++)
			{
				GameMaster.instance.Levels[i].name = "Level " + (i + 1);
			}
			UpdateMapBorder(mission + 1);
			int ToMission = mission + 1;
			GameMaster.instance.CurrentMission = ToMission;
			StartCoroutine(PlaceAllProps(Levels[ToMission].MissionDataProps, oldVersion: false, 0));
			NightToggler.IsEnabled = (GameMaster.instance.NightLevels.Contains(ToMission) ? true : false);
			NoBordersToggler.IsEnabled = ((!NoBordersMissions.Contains(ToMission)) ? true : false);
			UpdateMapBorder(ToMission);
			MissionNameInputField.text = GameMaster.instance.MissionNames[ToMission];
			WeatherList.value = WeatherTypes[ToMission];
			FloorDropdown.value = MissionFloorTextures[ToMission];
			StartCoroutine(MembusDelay());
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
			MapPiecesClass newMPC = new MapPiecesClass();
			MapPiecesClass otherMPC = MissionsData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			newMPC.offsetX = otherMPC.offsetX;
			newMPC.offsetY = otherMPC.offsetY;
			newMPC.missionNumber = MEGP.mission;
			for (int i = 0; i < 5; i++)
			{
				newMPC.propID[i] = otherMPC.propID[i];
				newMPC.propRotation[i] = otherMPC.propRotation[i];
			}
			MEGP.ID = OptionsMainMenu.instance.MapSize * MEGP.mission + MEGP.ID % OptionsMainMenu.instance.MapSize;
			newMPC.ID = MEGP.ID;
			MissionsData.Add(newMPC);
		}
		else
		{
			MapPiecesClass MPC = MissionsData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			MEGP.ID = OptionsMainMenu.instance.MapSize * MEGP.mission + MEGP.ID % OptionsMainMenu.instance.MapSize;
			MPC.ID = MEGP.ID;
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
		SingleMapEditorData newSMED = new SingleMapEditorData(null, null);
		List<MapPiecesClass> DuplicateList = Levels[MissionToDuplicate].MissionDataProps;
		for (int i = 0; i < OptionsMainMenu.instance.MapSize; i++)
		{
			MapPiecesClass newMPC = new MapPiecesClass();
			newMPC.ID = i;
			for (int j = 0; j < 5; j++)
			{
				newMPC.propID[j] = DuplicateList[i].propID[j];
				newMPC.propRotation[j] = DuplicateList[i].propRotation[j];
				newMPC.TeamColor[j] = DuplicateList[i].TeamColor[j];
				newMPC.SpawnDifficulty = DuplicateList[i].SpawnDifficulty;
				newMPC.CustomColor = DuplicateList[i].CustomColor;
			}
			newMPC.missionNumber = Levels.Count;
			newSMED.MissionDataProps.Add(newMPC);
		}
		Levels.Insert(MissionToDuplicate, newSMED);
		GameMaster.instance.MissionNames.Insert(MissionToDuplicate, GameMaster.instance.MissionNames[MissionToDuplicate]);
		GameObject ListUIItem = UnityEngine.Object.Instantiate(LevelUIListPrefab);
		ListUIItem.transform.SetParent(ParentObjectList.transform, worldPositionStays: false);
		LevelUIList.Insert(MissionToDuplicate, ListUIItem);
		ListUIItem.transform.SetSiblingIndex(ListUIItem.transform.parent.transform.childCount - (Levels.Count - MissionToDuplicate) - 1);
		ListUIItem.transform.localScale = new Vector3(1f, 1f, 1f);
		MapEditorMissionBarUI MEMBU = ListUIItem.GetComponent<MapEditorMissionBarUI>();
		MEMBU.mission = MissionToDuplicate + 1;
		WeatherTypes[MissionToDuplicate + 1] = WeatherTypes[MissionToDuplicate];
		MissionFloorTextures[MissionToDuplicate + 1] = MissionFloorTextures[MissionToDuplicate];
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
		WeatherList.value = WeatherTypes[ToMission];
		FloorDropdown.value = MissionFloorTextures[ToMission];
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
		Animator camAnimator = Camera.main.GetComponent<Animator>();
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
		GetComponent<AudioSource>().PlayOneShot(SuccesSound);
		StartCoroutine(SetBoolFalse("ShowGoodMessage"));
	}

	private IEnumerator SetBoolFalse(string Bool)
	{
		yield return new WaitForSeconds(0.1f);
		ErrorFieldMessage.SetBool(Bool, value: false);
	}
}
