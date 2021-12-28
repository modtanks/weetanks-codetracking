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

	public bool inPlayingMode;

	public int SelectedProp;

	public int maxMission = 20;

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

	public Toggle NightToggler;

	public Toggle NoBordersToggler;

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

	public int[] WeatherTypes;

	public int[] MissionFloorTextures;

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

	public GameObject ScrollViewBlocks;

	public GameObject ScrollViewExtras;

	public GameObject ScrollViewTanks;

	public GameObject SettingsView;

	public GameObject CustomTankView;

	public GameObject CampaignSettingsView;

	public GameObject ParentObjectList;

	public Color TabNotSelectedColor;

	public Color TabSelectedColor;

	public RawImage TanksImage;

	public RawImage BlocksImage;

	public RawImage ExtrasImage;

	public RawImage SettingsImage;

	public RawImage CustomTankImage;

	public RawImage CampaignSettingsImage;

	[Header("Custom Tank")]
	public int minTankSpeed;

	public int maxTankSpeed = 100;

	private int maxTankSpeedBoosted = 1000;

	public float minFireSpeed = 0.15f;

	public float maxFireSpeed = 100f;

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

	public RawImage CustomTankOverlay;

	public Image CustomTankMaterialSource;

	public List<Color> CustomTankColor = new List<Color>();

	public List<int> CustomTankSpeed = new List<int>();

	public List<float> CustomFireSpeed = new List<float>();

	public List<int> CustomBounces = new List<int>();

	public List<int> CustomBullets = new List<int>();

	public List<float> CustomMineSpeed = new List<float>();

	public List<int> CustomTurnHead = new List<int>();

	public List<int> CustomAccuracy = new List<int>();

	public List<bool> CustomLayMines = new List<bool>();

	public List<int> CustomBulletType = new List<int>();

	public List<int> CustomMusic = new List<int>();

	public List<bool> CustomInvisibility = new List<bool>();

	public List<bool> CustomCalculateShots = new List<bool>();

	public List<bool> CustomArmoured = new List<bool>();

	public List<int> CustomArmourPoints = new List<int>();

	public List<float> CustomTankScale = new List<float>();

	public MapEditorUIprop CustomTankScript;

	public GameObject[] BulletPrefabs;

	public GameObject[] PlayerBulletPrefabs;

	public int StartingLives = 3;

	public Color[] TeamColors;

	public bool[] TeamColorEnabled;

	[Header("Custom Tank input fields")]
	public Slider TankSpeedField;

	public Slider FireSpeedField;

	public Slider AmountBouncesField;

	public Slider AmountBulletsField;

	public Slider MineLaySpeedField;

	public Slider TurnHeadSlider;

	public Slider AccuracySlider;

	public Slider ArmouredSlider;

	public Slider ScaleSlider;

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

	public Toggle PlayerCanLayMinesToggle;

	public Toggle[] TeamColorsToggles;

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

	public Toggle CanLayMinesToggle;

	public Toggle InvisibilityToggle;

	public Toggle CalculateShotsToggle;

	public Toggle ArmouredToggle;

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

	public int[] ColorsTeamsPlaced;

	public int LastPlacedColor;

	public int LastPlacedRotation;

	private bool canClick = true;

	private bool canChangeValues = true;

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
		Animator component = Camera.main.GetComponent<Animator>();
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			SetMapBorder(0);
			component.SetInteger("MapSize", 0);
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			SetMapBorder(1);
			component.SetInteger("MapSize", 1);
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			SetMapBorder(2);
			component.SetInteger("MapSize", 2);
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			SetMapBorder(3);
			component.SetInteger("MapSize", 3);
		}
		if (GameMaster.instance.inMapEditor)
		{
			CDS.gameObject.SetActive(value: false);
		}
		if (!inPlayingMode)
		{
			SetCustomTankValues();
			LoadData();
			LastPlacedColor = 2;
			TanksImage.color = TabNotSelectedColor;
			SettingsImage.color = TabNotSelectedColor;
			BlocksImage.color = TabSelectedColor;
			CustomTankImage.color = TabNotSelectedColor;
			TeleportationFields.SetActive(value: false);
			AudioSource component2 = Camera.main.GetComponent<AudioSource>();
			if (OptionsMainMenu.instance != null)
			{
				lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
				lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
				component2.volume = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			}
			AnimatorStartHeight = ErrorFieldMessage.transform.localPosition;
			component2.clip = EditingModeBackgroundMusic;
			component2.loop = true;
			component2.Play();
			ShowMenu(0);
		}
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.25f);
		UpdateMembus();
	}

	public IEnumerator coolDown()
	{
		canDoButton = false;
		yield return new WaitForSeconds(0.5f);
		canDoButton = true;
	}

	public void SetCustomTankValues()
	{
		for (int i = 0; i < 3; i++)
		{
			CustomTankSpeed[i] = 0;
			CustomFireSpeed[i] = 0f;
			CustomBounces[i] = 0;
			CustomBullets[i] = 0;
			CustomMineSpeed[i] = 0f;
			CustomTurnHead[i] = 0;
			CustomAccuracy[i] = 0;
			CustomLayMines[i] = false;
			CustomBulletType[i] = 0;
			CustomMusic[i] = 0;
			CustomInvisibility[i] = false;
			CustomCalculateShots[i] = false;
			CustomArmoured[i] = false;
			CustomMaterial[i].color = Color.white;
			CustomTankColor[i] = Color.white;
			LoadCustomTankDataUI(0);
		}
	}

	public void LoadCustomTankDataUI(int customNumber)
	{
		TankSpeedField.value = CustomTankSpeed[customNumber];
		FireSpeedField.value = CustomFireSpeed[customNumber];
		AmountBouncesField.value = CustomBounces[customNumber];
		AmountBulletsField.value = CustomBullets[customNumber];
		MineLaySpeedField.value = CustomMineSpeed[customNumber];
		TurnHeadSlider.value = CustomTurnHead[customNumber];
		AccuracySlider.value = CustomAccuracy[customNumber];
		if (FCP != null)
		{
			FCP.startingColor = CustomTankColor[customNumber];
		}
		CanLayMinesToggle.isOn = CustomLayMines[customNumber];
		InvisibilityToggle.isOn = CustomInvisibility[customNumber];
		CalculateShotsToggle.isOn = CustomCalculateShots[customNumber];
		ArmouredToggle.isOn = CustomArmoured[customNumber];
		BulletTypeList.value = CustomBulletType[customNumber];
		MusicList.value = CustomMusic[customNumber];
		TankSpeedField.minValue = minTankSpeed;
		FireSpeedField.minValue = minFireSpeed;
		AmountBouncesField.minValue = minBounces;
		AmountBulletsField.minValue = minBullets;
		AmountBulletsField.maxValue = maxBullets;
		MineLaySpeedField.minValue = minMineSpeed;
		TurnHeadSlider.minValue = minTurnHead;
		AccuracySlider.minValue = minAccuracy;
		TankSpeedField.maxValue = maxTankSpeed;
		FireSpeedField.maxValue = maxFireSpeed;
		AmountBouncesField.maxValue = maxBounces;
		MineLaySpeedField.maxValue = maxMineSpeed;
		TurnHeadSlider.maxValue = maxTurnHead;
		AccuracySlider.maxValue = maxAccuracy;
		StartLivesSlider.value = StartingLives;
		for (int i = 0; i < TeamColorsToggles.Length; i++)
		{
			TeamColorsToggles[i].isOn = TeamColorEnabled[i];
		}
	}

	private IEnumerator ResetClickSound()
	{
		yield return new WaitForSeconds(0.01f);
		canClick = true;
	}

	public void PlayChangeSound()
	{
		if (canClick)
		{
			GameMaster.instance.Play2DClipAtPoint(ChangeSound, 0.5f);
			StartCoroutine(ResetClickSound());
			canClick = false;
		}
	}

	private void Update()
	{
		if (inPlayingMode)
		{
			for (int i = 0; i < 3; i++)
			{
				if (CustomMaterial[i].GetColor("_Color") != CustomTankColor[i])
				{
					CustomMaterial[i].SetColor("_Color", CustomTankColor[i]);
				}
			}
			return;
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
		if (MenuCurrent == 4)
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
			PlayerCanLayMines = PlayerCanLayMinesToggle.isOn;
			for (int j = 0; j < TeamColorEnabled.Length; j++)
			{
				TeamColorEnabled[j] = TeamColorsToggles[j].isOn;
			}
		}
		else if (MenuCurrent == 2)
		{
			if (CanLayMinesToggle.isOn && !MineLaySpeedField.transform.parent.gameObject.activeSelf)
			{
				Debug.Log("MINES Toggle changed ON");
				MineLaySpeedField.transform.parent.gameObject.SetActive(value: true);
				CustomLayMines[SelectedCustomTank] = true;
			}
			else if (!CanLayMinesToggle.isOn && MineLaySpeedField.transform.parent.gameObject.activeSelf)
			{
				Debug.Log("MINES Toggle changed OFF");
				MineLaySpeedField.transform.parent.gameObject.SetActive(value: false);
				CustomLayMines[SelectedCustomTank] = false;
			}
			if (ArmouredToggle.isOn && !ArmouredSlider.transform.parent.gameObject.activeSelf)
			{
				Debug.Log("Armour Toggle changed ON");
				ArmouredSlider.transform.parent.gameObject.SetActive(value: true);
				CustomArmoured[SelectedCustomTank] = true;
			}
			else if (!ArmouredToggle.isOn && ArmouredSlider.transform.parent.gameObject.activeSelf)
			{
				Debug.Log("Armour Toggle changed OFF");
				ArmouredSlider.transform.parent.gameObject.SetActive(value: false);
				CustomArmoured[SelectedCustomTank] = false;
			}
			CustomInvisibility[SelectedCustomTank] = InvisibilityToggle.isOn;
			CustomCalculateShots[SelectedCustomTank] = CalculateShotsToggle.isOn;
			CustomArmourPoints[SelectedCustomTank] = Mathf.RoundToInt(ArmouredSlider.value);
			ArmouredSlider.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxArmourPointsBoosted : maxArmourPoints);
			if (CustomArmourPoints[SelectedCustomTank] < minArmourPoints)
			{
				CustomArmourPoints[SelectedCustomTank] = minArmourPoints;
			}
			else if ((float)CustomArmourPoints[SelectedCustomTank] > ArmouredSlider.maxValue)
			{
				CustomArmourPoints[SelectedCustomTank] = Mathf.RoundToInt(ArmouredSlider.maxValue);
			}
			CustomTankScale[SelectedCustomTank] = ScaleSlider.value;
			ScaleSlider.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxScalePointsBoosted : maxScalePoints);
			if (CustomTankScale[SelectedCustomTank] < minScalePoints)
			{
				CustomTankScale[SelectedCustomTank] = minScalePoints;
			}
			else if (CustomTankScale[SelectedCustomTank] > ScaleSlider.maxValue)
			{
				CustomTankScale[SelectedCustomTank] = ScaleSlider.maxValue;
			}
			CustomBulletType[SelectedCustomTank] = BulletTypeList.value;
			CustomMusic[SelectedCustomTank] = MusicList.value;
			CustomTankSpeed[SelectedCustomTank] = Mathf.RoundToInt(TankSpeedField.value);
			TankSpeedField.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxTankSpeedBoosted : maxTankSpeed);
			if (CustomTankSpeed[SelectedCustomTank] < minTankSpeed)
			{
				CustomTankSpeed[SelectedCustomTank] = minTankSpeed;
			}
			else if ((float)CustomTankSpeed[SelectedCustomTank] > TankSpeedField.maxValue)
			{
				CustomTankSpeed[SelectedCustomTank] = Mathf.RoundToInt(TankSpeedField.maxValue);
			}
			CustomFireSpeed[SelectedCustomTank] = Mathf.Floor(FireSpeedField.value * 100f) / 100f;
			if (CustomFireSpeed[SelectedCustomTank] < minFireSpeed)
			{
				CustomFireSpeed[SelectedCustomTank] = minFireSpeed;
			}
			else if (CustomFireSpeed[SelectedCustomTank] > maxFireSpeed)
			{
				CustomFireSpeed[SelectedCustomTank] = maxFireSpeed;
			}
			CustomBounces[SelectedCustomTank] = Mathf.RoundToInt(AmountBouncesField.value);
			AmountBouncesField.maxValue = (OptionsMainMenu.instance.AMselected.Contains(63) ? maxBouncesBoosted : maxBounces);
			if (CustomBounces[SelectedCustomTank] < minBounces)
			{
				CustomBounces[SelectedCustomTank] = minBounces;
			}
			else if ((float)CustomBounces[SelectedCustomTank] > AmountBouncesField.maxValue)
			{
				CustomBounces[SelectedCustomTank] = Mathf.RoundToInt(AmountBouncesField.maxValue);
			}
			CustomBullets[SelectedCustomTank] = Mathf.RoundToInt(AmountBulletsField.value);
			if (CustomBullets[SelectedCustomTank] < minBullets)
			{
				CustomBullets[SelectedCustomTank] = minBullets;
			}
			else if (CustomBullets[SelectedCustomTank] > maxBullets)
			{
				CustomBullets[SelectedCustomTank] = maxBullets;
			}
			CustomMineSpeed[SelectedCustomTank] = Mathf.Floor(MineLaySpeedField.value * 100f) / 100f;
			if (CustomMineSpeed[SelectedCustomTank] < minMineSpeed)
			{
				CustomMineSpeed[SelectedCustomTank] = minMineSpeed;
			}
			else if (CustomMineSpeed[SelectedCustomTank] > maxMineSpeed)
			{
				CustomMineSpeed[SelectedCustomTank] = maxMineSpeed;
			}
			CustomTurnHead[SelectedCustomTank] = Mathf.RoundToInt(TurnHeadSlider.value);
			if (CustomTurnHead[SelectedCustomTank] < minTurnHead)
			{
				CustomTurnHead[SelectedCustomTank] = minTurnHead;
			}
			else if (CustomTurnHead[SelectedCustomTank] > maxTurnHead)
			{
				CustomTurnHead[SelectedCustomTank] = maxTurnHead;
			}
			CustomAccuracy[SelectedCustomTank] = Mathf.RoundToInt(AccuracySlider.value);
			if (CustomAccuracy[SelectedCustomTank] < minAccuracy)
			{
				CustomAccuracy[SelectedCustomTank] = minAccuracy;
			}
			else if (CustomAccuracy[SelectedCustomTank] > maxAccuracy)
			{
				CustomAccuracy[SelectedCustomTank] = maxAccuracy;
			}
			CustomTankOverlay.color = CustomTankMaterialSource.material.GetColor("_Color1");
			CustomTankColor[SelectedCustomTank] = CustomTankOverlay.color;
			if (CustomMaterial[SelectedCustomTank].GetColor("_Color") != CustomTankColor[SelectedCustomTank])
			{
				CustomMaterial[SelectedCustomTank].SetColor("_Color", CustomTankColor[SelectedCustomTank]);
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
		if (customtank == 1 && GameMaster.instance.maxMissionReached < 31 && !OptionsMainMenu.instance.AMselected.Contains(60))
		{
			ShowErrorMessage("ERROR: Need to have reached checkpoint 30");
			return;
		}
		if (customtank == 2 && GameMaster.instance.maxMissionReached < 51 && !OptionsMainMenu.instance.AMselected.Contains(60))
		{
			ShowErrorMessage("ERROR: Need to have reached checkpoint 50");
			return;
		}
		Debug.Log("Armour toggle set to:" + CustomArmoured[customtank]);
		canChangeValues = false;
		StartCoroutine(ChangeBack());
		CustomTankScript.PropID = 19 + customtank;
		TankSpeedField.value = CustomTankSpeed[customtank];
		FireSpeedField.value = CustomFireSpeed[customtank];
		AmountBouncesField.value = CustomBounces[customtank];
		AmountBulletsField.value = CustomBullets[customtank];
		MineLaySpeedField.value = CustomMineSpeed[customtank];
		TurnHeadSlider.value = CustomTurnHead[customtank];
		AccuracySlider.value = CustomAccuracy[customtank];
		ArmouredSlider.value = CustomArmourPoints[customtank];
		ScaleSlider.value = CustomTankScale[customtank];
		Debug.Log("Setting custom tank = " + customtank);
		if (FCP != null)
		{
			_ = CustomMaterial[customtank].color;
			FCP.startingColor = CustomMaterial[customtank].color;
			FCP.color = CustomMaterial[customtank].color;
		}
		SelectedCustomTank = customtank;
		Debug.Log("MINES toggle set to:" + CustomLayMines[customtank] + "custom tank number=" + customtank);
		CanLayMinesToggle.isOn = CustomLayMines[customtank];
		BulletTypeList.value = CustomBulletType[customtank];
		MusicList.value = CustomMusic[customtank];
		InvisibilityToggle.isOn = CustomInvisibility[customtank];
		CalculateShotsToggle.isOn = CustomCalculateShots[customtank];
		Debug.Log("Armour toggle set to:" + CustomArmoured[customtank] + "custom tank number=" + customtank);
		ArmouredToggle.isOn = CustomArmoured[customtank];
		ActivateView(2);
		MenuCurrent = 2;
		CustomTankMaterialSource.material.SetColor("_Color1", CustomMaterial[customtank].color);
		CustomTankOverlay.color = CustomTankMaterialSource.material.GetColor("_Color1");
		CustomTankColor[SelectedCustomTank] = CustomTankOverlay.color;
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
			return;
		}
		if (ShowIDSlider == 0)
		{
			OnCursorText.text = CustomTankSpeed[SelectedCustomTank].ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 1)
		{
			float num = CustomFireSpeed[SelectedCustomTank];
			OnCursorText.text = num.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 2)
		{
			OnCursorText.text = CustomBounces[SelectedCustomTank].ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 3)
		{
			float num2 = Mathf.Round(1f / (10.5f - CustomMineSpeed[SelectedCustomTank]) * 100f) / 100f;
			OnCursorText.text = num2.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 4)
		{
			OnCursorText.text = CustomTurnHead[SelectedCustomTank].ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 5)
		{
			OnCursorText.text = CustomAccuracy[SelectedCustomTank].ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 6)
		{
			OnCursorText.text = CustomArmourPoints[SelectedCustomTank].ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 7)
		{
			OnCursorText.text = (Mathf.Round(CustomTankScale[SelectedCustomTank] * 100f) / 100f).ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 8)
		{
			OnCursorText.text = CustomBullets[SelectedCustomTank].ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 50)
		{
			OnCursorText.text = StartingLives.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 51)
		{
			OnCursorText.text = PlayerSpeed.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 52)
		{
			OnCursorText.text = PlayerMaxBullets.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 53)
		{
			OnCursorText.text = PlayerArmourPoints.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
		if (ShowIDSlider == 54)
		{
			OnCursorText.text = PlayerAmountBounces.ToString();
			OnCursorText.transform.gameObject.SetActive(value: true);
		}
	}

	public void PlayAudio(AudioClip AC)
	{
		if (playingAmount < 2)
		{
			playingAmount++;
			if (AC != null)
			{
				GetComponent<AudioSource>().PlayOneShot(AC);
			}
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
		MapEditorData mapEditorData = SavingMapEditorData.LoadData(OptionsMainMenu.instance.MapEditorMapName);
		if (mapEditorData == null)
		{
			Debug.Log("no data found");
			GameObject item = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
			GameMaster.instance.Levels.Add(item);
			CreateNewLevel(isBrandNew: true);
			return;
		}
		int missionAmount = mapEditorData.missionAmount;
		Debug.LogError("MISSION AMOUNT IS:" + missionAmount);
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
			PlayerCanLayMinesToggle.isOn = PlayerCanLayMines;
		}
		SetCustomTankData(mapEditorData);
		GameObject item2 = UnityEngine.Object.Instantiate(LevelPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
		GameMaster.instance.Levels.Add(item2);
		int i;
		for (i = 0; i < missionAmount; i++)
		{
			SingleMapEditorData singleMapEditorData = new SingleMapEditorData(null, null);
			singleMapEditorData.MissionDataProps = mapEditorData.MissionDataProps.FindAll((MapPiecesClass x) => x.missionNumber == i);
			for (int j = 0; j < OptionsMainMenu.instance.MapSize; j++)
			{
				singleMapEditorData.MissionDataProps[j].ID = j;
			}
			GameMaster.instance.MissionNames.Add("Level " + Levels.Count);
			Levels.Add(singleMapEditorData);
			CreateNewLevel(isBrandNew: false);
			if (list.ElementAtOrDefault(i) != null)
			{
				GameMaster.instance.MissionNames[i] = list[i];
			}
			else
			{
				GameMaster.instance.MissionNames[i] = "no name";
			}
		}
		bool oldVersion = false;
		if (mapEditorData.VersionCreated == "v0.7.9" || mapEditorData.VersionCreated == "v0.7.8" || mapEditorData.VersionCreated == "v0.7.10" || mapEditorData.VersionCreated == "v0.7.11" || mapEditorData.VersionCreated == "v0.7.12" || mapEditorData.VersionCreated == "v0.8.0a" || mapEditorData.VersionCreated == "v0.8.0b" || mapEditorData.VersionCreated == "v0.8.0c")
		{
			oldVersion = true;
		}
		StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion, 0));
		NightToggler.isOn = (GameMaster.instance.NightLevels.Contains(0) ? true : false);
		NoBordersToggler.isOn = (NoBordersMissions.Contains(0) ? true : false);
		PID = mapEditorData.PID;
		IsPublished = mapEditorData.isPublished;
		if (mapEditorData.WeatherTypes != null && mapEditorData.WeatherTypes.Length != 0)
		{
			WeatherTypes = mapEditorData.WeatherTypes;
			WeatherList.value = WeatherTypes[0];
		}
		if (mapEditorData.MissionFloorTextures != null && mapEditorData.MissionFloorTextures.Length != 0)
		{
			MissionFloorTextures = mapEditorData.MissionFloorTextures;
			FloorDropdown.value = MissionFloorTextures[0];
		}
		if (mapEditorData.NoBordersMissions != null)
		{
			NoBordersMissions = mapEditorData.NoBordersMissions;
			if (NoBordersMissions.Count < 1)
			{
				NoBordersToggler.isOn = true;
			}
			else
			{
				NoBordersToggler.isOn = (NoBordersMissions.Contains(0) ? true : false);
			}
		}
	}

	public void SetCustomTankData(MapEditorData theData)
	{
		for (int i = 0; i < 3; i++)
		{
			Debug.LogError(theData.CustomTankSpeed[i]);
			CustomTankSpeed[i] = theData.CustomTankSpeed[i];
			CustomFireSpeed[i] = theData.CustomFireSpeed[i];
			CustomBounces[i] = theData.CustomBounces[i];
			CustomMineSpeed[i] = theData.CustomMineSpeed[i];
			CustomTurnHead[i] = theData.CustomTurnHead[i];
			CustomAccuracy[i] = theData.CustomAccuracy[i];
			CustomLayMines[i] = theData.LayMines[i];
			CustomTankColor[i] = theData.CTC[i].Color;
			CustomMaterial[i].color = theData.CTC[i].Color;
			CustomBulletType[i] = theData.CustomBulletType[i];
			CustomArmoured[i] = theData.CustomArmoured[i];
			CustomArmourPoints[i] = theData.CustomArmourPoints[i];
			CustomInvisibility[i] = theData.CustomInvisibility[i];
			CustomCalculateShots[i] = theData.CustomCalculateShots[i];
			if (theData.CustomBullets != null && theData.CustomBullets.Count > 0)
			{
				CustomBullets[i] = theData.CustomBullets[i];
			}
			if (theData.CustomMusic != null && theData.CustomMusic.Count > 0)
			{
				CustomMusic[i] = theData.CustomMusic[i];
			}
			if (theData.CustomScalePoints != null && theData.CustomScalePoints.Count > 1)
			{
				CustomTankScale[i] = theData.CustomScalePoints[i];
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
		foreach (GameObject gameObject in array)
		{
			MapEditorGridPiece MEGP = gameObject.GetComponent<MapEditorGridPiece>();
			MapPiecesClass mapPiecesClass = allPropData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			if (mapPiecesClass == null)
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
				if (num > -1)
				{
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
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
					case 22:
					case 23:
					case 24:
					case 25:
					case 26:
					case 27:
					case 30:
					case 31:
					case 32:
					case 33:
					case 34:
					case 35:
					case 36:
					case 37:
					case 38:
					case 39:
						enemyTanksPlaced[mapPiecesClass.missionNumber]++;
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
					else if (mapPiecesClass.propID[j] > 5 && mapPiecesClass.propID[j] < 40)
					{
						enemyTanksPlaced[mapPiecesClass.missionNumber]++;
					}
					Convert.ToInt32(mapPiecesClass.SpawnDifficulty);
					MEGP.MyTeamNumber = mapPiecesClass.TeamColor[j];
					MEGP.SpawnDifficulty = mapPiecesClass.SpawnDifficulty;
					MEGP.mission = missionnumber;
					MEGP.SpawnInProps(mapPiecesClass.propID[j], mapPiecesClass.propRotation[j], mapPiecesClass.TeamColor[j], j, mapPiecesClass.SpawnDifficulty);
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

	public void ShowMenu(int menu)
	{
		ActivateView(menu);
		MenuCurrent = menu;
	}

	public void SaveCustomTankSettings()
	{
		if (canChangeValues)
		{
			CustomLayMines[SelectedCustomTank] = CanLayMinesToggle.isOn;
			CustomArmoured[SelectedCustomTank] = ArmouredToggle.isOn;
			CustomBulletType[SelectedCustomTank] = BulletTypeList.value;
			CustomMusic[SelectedCustomTank] = MusicList.value;
			CustomInvisibility[SelectedCustomTank] = InvisibilityToggle.isOn;
			CustomCalculateShots[SelectedCustomTank] = CalculateShotsToggle.isOn;
			CustomArmoured[SelectedCustomTank] = ArmouredToggle.isOn;
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
			GameMaster.instance.floor.GetComponent<MeshRenderer>().material = FloorMaterials[MissionFloorTextures[GameMaster.instance.CurrentMission]];
		}
	}

	public void ChangeCamera()
	{
		Animator component = Camera.main.GetComponent<Animator>();
		if (component.GetBool("CameraDownEditor"))
		{
			component.SetBool("CameraDown", value: false);
			component.SetBool("CameraUp", value: false);
			component.SetBool("CameraDownEditor", value: false);
			component.SetBool("CameraUpEditor", value: true);
		}
		else
		{
			component.SetBool("CameraDown", value: false);
			component.SetBool("CameraUp", value: false);
			component.SetBool("CameraDownEditor", value: true);
			component.SetBool("CameraUpEditor", value: false);
		}
	}

	private void ActivateView(int tab)
	{
		Play2DClipAtPoint(MenuSwitch);
		CustomTankView.SetActive(value: false);
		ScrollViewExtras.SetActive(value: false);
		SettingsView.SetActive(value: false);
		ScrollViewTanks.SetActive(value: false);
		ScrollViewBlocks.SetActive(value: false);
		CampaignSettingsView.SetActive(value: false);
		TanksImage.color = TabNotSelectedColor;
		BlocksImage.color = TabNotSelectedColor;
		ExtrasImage.color = TabNotSelectedColor;
		SettingsImage.color = TabNotSelectedColor;
		CustomTankImage.color = TabNotSelectedColor;
		CampaignSettingsImage.color = TabNotSelectedColor;
		if (TeamColorsToggles.Length != 0)
		{
			for (int i = 0; i < TeamColorsToggles.Length; i++)
			{
				TeamColorsToggles[i].isOn = TeamColorEnabled[i];
			}
		}
		switch (tab)
		{
		case 0:
			ScrollViewBlocks.SetActive(value: true);
			BlocksImage.color = TabSelectedColor;
			break;
		case 1:
			ScrollViewTanks.SetActive(value: true);
			TanksImage.color = TabSelectedColor;
			break;
		case 2:
			CustomTankView.SetActive(value: true);
			CustomTankImage.color = TabSelectedColor;
			break;
		case 3:
			SettingsView.SetActive(value: true);
			SettingsImage.color = TabSelectedColor;
			break;
		case 4:
			CampaignSettingsView.SetActive(value: true);
			CampaignSettingsImage.color = TabSelectedColor;
			StartLivesSlider.value = StartingLives;
			break;
		case 5:
			ScrollViewExtras.SetActive(value: true);
			ExtrasImage.color = TabSelectedColor;
			break;
		}
	}

	public void SaveCurrentProps()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("MapeditorField");
		foreach (GameObject gameObject in array)
		{
			MapEditorGridPiece MEGP = gameObject.GetComponent<MapEditorGridPiece>();
			for (int j = 0; j < 5; j++)
			{
				if (MEGP.myPropID[j] > -1)
				{
					List<MapPiecesClass> missionDataProps = Levels[GameMaster.instance.CurrentMission].MissionDataProps;
					MapPiecesClass mapPiecesClass = missionDataProps.Find((MapPiecesClass x) => x.ID == MEGP.ID);
					if (mapPiecesClass != null)
					{
						mapPiecesClass.propID[j] = MEGP.myPropID[j];
						if ((bool)MEGP.myMEP[j])
						{
							mapPiecesClass.TeamColor[j] = MEGP.myMEP[j].TeamNumber;
						}
						else
						{
							mapPiecesClass.TeamColor[j] = -1;
						}
						mapPiecesClass.missionNumber = MEGP.mission;
						mapPiecesClass.propRotation[j] = MEGP.rotationDirection[j];
						mapPiecesClass.SpawnDifficulty = MEGP.myMEP[j].MyDifficultySpawn;
						Levels[GameMaster.instance.CurrentMission].MissionDataProps = missionDataProps;
					}
				}
				else
				{
					Levels[GameMaster.instance.CurrentMission].MissionDataProps.Find((MapPiecesClass x) => x.ID == MEGP.ID).propID[j] = -1;
				}
			}
		}
	}

	public void CreateNewLevelButton()
	{
		NightToggler.isOn = false;
		Debug.Log("new map button clicked!");
		Play2DClipAtPoint(NewMapSound);
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
				SingleMapEditorData singleMapEditorData = new SingleMapEditorData(null, null);
				for (int i = 0; i < OptionsMainMenu.instance.MapSize; i++)
				{
					MapPiecesClass mapPiecesClass = new MapPiecesClass();
					mapPiecesClass.ID = i;
					for (int j = 0; j < 5; j++)
					{
						mapPiecesClass.propID[j] = -1;
					}
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
				NoBordersToggler.isOn = true;
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
		Debug.Log("switching level to " + missionNumber);
		SaveCurrentProps();
		RemoveCurrentObjects();
		Debug.Log("Level was" + GameMaster.instance.CurrentMission);
		GameMaster.instance.CurrentMission = missionNumber;
		Debug.Log("Level is now" + GameMaster.instance.CurrentMission);
		StartCoroutine(PlaceAllProps(Levels[missionNumber].MissionDataProps, oldVersion: false, missionNumber));
		UpdateMembus();
		NightToggler.isOn = (GameMaster.instance.NightLevels.Contains(missionNumber) ? true : false);
		NoBordersToggler.isOn = ((!NoBordersMissions.Contains(missionNumber)) ? true : false);
		UpdateMapBorder(missionNumber);
		MissionNameInputField.text = GameMaster.instance.MissionNames[missionNumber];
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
		Play2DClipAtPoint(NewMapSound);
		Levels.RemoveAt(removeMission);
		ShowPositiveMessage("SUCCES: Level " + (removeMission + 1) + " removed!");
		GameMaster.instance.MissionNames.RemoveAt(removeMission);
		int num = WeatherTypes[removeMission];
		WeatherTypes[removeMission] = WeatherTypes[removeMission + 1];
		WeatherTypes[removeMission + 1] = num;
		int num2 = MissionFloorTextures[removeMission];
		MissionFloorTextures[removeMission] = MissionFloorTextures[removeMission + 1];
		MissionFloorTextures[removeMission + 1] = num2;
		UnityEngine.Object.Destroy(LevelUIList[removeMission]);
		LevelUIList.RemoveAt(removeMission);
		UpdateMembus();
		GameMaster.instance.CurrentMission = 0;
		StartCoroutine(PlaceAllProps(Levels[0].MissionDataProps, oldVersion: false, 0));
		NightToggler.isOn = (GameMaster.instance.NightLevels.Contains(0) ? true : false);
		NoBordersToggler.isOn = ((!NoBordersMissions.Contains(0)) ? true : false);
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
		Debug.LogError("MOVED LEEVLE");
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
				int num = WeatherTypes[mission];
				WeatherTypes[mission] = WeatherTypes[mission - 1];
				WeatherTypes[mission - 1] = num;
				int num2 = MissionFloorTextures[mission];
				MissionFloorTextures[mission] = MissionFloorTextures[mission - 1];
				MissionFloorTextures[mission - 1] = num2;
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
				int num3 = mission - 1;
				GameMaster.instance.CurrentMission = num3;
				StartCoroutine(PlaceAllProps(Levels[num3].MissionDataProps, oldVersion: false, 0));
				NightToggler.isOn = (GameMaster.instance.NightLevels.Contains(num3) ? true : false);
				NoBordersToggler.isOn = ((!NoBordersMissions.Contains(num3)) ? true : false);
				UpdateMapBorder(num3);
				MissionNameInputField.text = GameMaster.instance.MissionNames[num3];
				WeatherList.value = WeatherTypes[num3];
				FloorDropdown.value = MissionFloorTextures[num3];
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
			int num4 = WeatherTypes[mission];
			WeatherTypes[mission] = WeatherTypes[mission + 1];
			WeatherTypes[mission + 1] = num4;
			int num5 = MissionFloorTextures[mission];
			MissionFloorTextures[mission] = MissionFloorTextures[mission + 1];
			MissionFloorTextures[mission + 1] = num5;
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
			int num6 = mission + 1;
			GameMaster.instance.CurrentMission = num6;
			StartCoroutine(PlaceAllProps(Levels[num6].MissionDataProps, oldVersion: false, 0));
			NightToggler.isOn = (GameMaster.instance.NightLevels.Contains(num6) ? true : false);
			NoBordersToggler.isOn = ((!NoBordersMissions.Contains(num6)) ? true : false);
			UpdateMapBorder(num6);
			MissionNameInputField.text = GameMaster.instance.MissionNames[num6];
			WeatherList.value = WeatherTypes[num6];
			FloorDropdown.value = MissionFloorTextures[num6];
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
			MapPiecesClass mapPiecesClass = new MapPiecesClass();
			MapPiecesClass mapPiecesClass2 = MissionsData.Find((MapPiecesClass x) => x.ID == MEGP.ID);
			mapPiecesClass.offsetX = mapPiecesClass2.offsetX;
			mapPiecesClass.offsetY = mapPiecesClass2.offsetY;
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
		if (GameMaster.instance.Levels.Count >= 20)
		{
			ShowErrorMessage("ERROR: Max missions reached");
			return;
		}
		playerOnePlaced.Insert(MissionToDuplicate, playerOnePlaced[MissionToDuplicate]);
		playerTwoPlaced.Insert(MissionToDuplicate, playerTwoPlaced[MissionToDuplicate]);
		playerThreePlaced.Insert(MissionToDuplicate, playerThreePlaced[MissionToDuplicate]);
		playerFourPlaced.Insert(MissionToDuplicate, playerFourPlaced[MissionToDuplicate]);
		enemyTanksPlaced.Insert(MissionToDuplicate, enemyTanksPlaced[MissionToDuplicate]);
		Play2DClipAtPoint(NewMapSound);
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
				mapPiecesClass.propID[j] = missionDataProps[i].propID[j];
				mapPiecesClass.propRotation[j] = missionDataProps[i].propRotation[j];
				mapPiecesClass.TeamColor[j] = missionDataProps[i].TeamColor[j];
				mapPiecesClass.SpawnDifficulty = missionDataProps[i].SpawnDifficulty;
			}
			mapPiecesClass.missionNumber = Levels.Count;
			singleMapEditorData.MissionDataProps.Add(mapPiecesClass);
		}
		Levels.Insert(MissionToDuplicate, singleMapEditorData);
		GameMaster.instance.MissionNames.Insert(MissionToDuplicate, GameMaster.instance.MissionNames[MissionToDuplicate]);
		GameObject gameObject = UnityEngine.Object.Instantiate(LevelUIListPrefab);
		gameObject.transform.SetParent(ParentObjectList.transform, worldPositionStays: false);
		LevelUIList.Insert(MissionToDuplicate, gameObject);
		gameObject.transform.SetSiblingIndex(gameObject.transform.parent.transform.childCount - (Levels.Count - MissionToDuplicate) - 1);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.GetComponent<MapEditorMissionBarUI>().mission = MissionToDuplicate + 1;
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
		int num = MissionToDuplicate + 1;
		GameMaster.instance.CurrentMission = num;
		StartCoroutine(PlaceAllProps(Levels[num].MissionDataProps, oldVersion: false, 0));
		NightToggler.isOn = (GameMaster.instance.NightLevels.Contains(num) ? true : false);
		NoBordersToggler.isOn = ((!NoBordersMissions.Contains(num)) ? true : false);
		UpdateMapBorder(num);
		MissionNameInputField.text = GameMaster.instance.MissionNames[num];
		WeatherList.value = WeatherTypes[num];
		FloorDropdown.value = MissionFloorTextures[num];
		StartCoroutine(MembusDelay());
	}

	public void SaveLevelSettings(TMP_InputField theTextFile)
	{
		Play2DClipAtPoint(ClickSound);
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

	public void SaveLevelSettings2(Toggle toggl)
	{
		if (toggl.isOn)
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

	public void SaveLevelSettings3(Toggle toggl)
	{
		if (toggl.isOn)
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
		SaveCurrentProps();
		GameMaster.instance.DisableGame();
		EditingCanvas.SetActive(value: false);
		GameMaster.instance.AmountEnemyTanks = 0;
		Debug.LogError("Starting game..");
		GameMaster.instance.isResettingTestLevel = false;
		GameMaster.instance.OnlyCompanionLeft = false;
		GetComponent<AudioSource>().Stop();
		Animator component = Camera.main.GetComponent<Animator>();
		if (component.GetBool("CameraDownEditor"))
		{
			component.SetBool("CameraDownEditor", value: true);
		}
		else
		{
			component.SetBool("CameraDownEditor", value: false);
			component.SetBool("CameraUpEditor", value: false);
		}
		component.SetBool("CameraUpEditor", value: false);
		component.SetBool("CameraDown", value: true);
		component.SetBool("CameraUp", value: false);
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
		ErrorFieldMessage.transform.localPosition = AnimatorStartHeight;
		ErrorFieldMessage.Play("ErrorMessage", -1, 0f);
		ErrorField.color = Color.red;
		ErrorField.text = Message;
		ErrorFieldMessage.SetBool("ShowMessage", value: true);
		ErrorFieldMessage.SetBool("ShowGoodMessage", value: false);
		GetComponent<AudioSource>().PlayOneShot(ErrorSound);
		StartCoroutine(SetBoolFalse("ShowMessage"));
	}

	public void ShowPositiveMessage(string Message)
	{
		ErrorFieldMessage.SetBool("ToTest", value: false);
		ErrorFieldMessage.transform.localPosition = AnimatorStartHeight;
		ErrorFieldMessage.Play("GoodMessage", -1, 0f);
		ErrorField.color = Color.green;
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

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		UnityEngine.Object.Destroy(obj, clip.length);
	}
}
