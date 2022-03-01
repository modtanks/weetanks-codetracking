using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private RectTransform myRect;

	private RectTransform childRect;

	public RectTransform myMarker;

	private Vector3 oldpos;

	private Vector3 newpos;

	public float size = 1.1f;

	public int inMenu;

	public int Place;

	public bool isLast;

	[Header("New Button Stuff")]
	public Color HoverColor;

	public Color UnavailableColor;

	public Color AvailableColor;

	public Image ButtonBorder;

	public Image ChavronBorder;

	public Image ChavronBackground;

	private Animator ButtonAnimator;

	public bool HoverOnEnter;

	[Header("Meta Button Stuff")]
	public TextMeshProUGUI ButtonMetaTitle;

	public TextMeshProUGUI ButtonMetaDescription;

	public TextMeshProUGUI ButtonTitle;

	public Texture ButtonMetaPositive;

	public Texture ButtonMetaNegative;

	public RawImage ButtonMetaImage;

	public Transform ButtonGlow;

	public Vector3 ButtonGlowStartPos;

	public Vector3 ButtonGlowEndPos;

	private Color StartColor;

	private Color StartColorButton;

	public float StartingPosY;

	public Vector3 StartingScale;

	[Header("Old settings")]
	public Animator camAnimator;

	public TextMeshProUGUI thisText;

	public TextMeshProUGUI babyText;

	public NewMenuControl NMC;

	public PauseMenuScript PMS;

	public GameObject ShowObjectWhenSelected;

	public bool StartMatchButton;

	public bool IsSelectPlayerController;

	public int PlayerNumber;

	[Header("Mid Menu")]
	public bool IsClassicCampaign;

	public bool IsSurvivalMode;

	public bool IsMapEditor;

	public bool IsOptions;

	public bool IsStats;

	public bool IsExit;

	public bool IsBeforeExit;

	public bool IsCancleExit;

	[Header("Classic Campaign")]
	public bool IsContinueClassicCampaign;

	public bool IsNewClassicCampaign;

	public bool IsDifficultyDown;

	public bool IsDifficultyUp;

	[Header("Mid Menu Pause")]
	public bool IsContinueGame;

	[Header("Online - Account")]
	public bool IsCreateAccount;

	public bool IsSignIn;

	public bool IsToMenuCreateAccount;

	public bool IsToMenuSignIn;

	public bool IsSignOut;

	public bool IsOpenCampaignsMenu;

	public bool IsGoToLobbyMenu;

	public bool IsJoinLobby;

	public bool IsCreateLobby;

	public bool IsLeaveLobby;

	public bool IsStartLobby;

	public bool IsAcceptTransferAccount;

	public bool IsToTankeyTown;

	public bool IsOnlineButton;

	[Header("Options - Gameplay")]
	public bool IsFriendlyFire;

	public bool IsExtraLivesDown;

	public bool IsExtraLivesUp;

	public bool IsAimAssist;

	public bool IsAICompanionLess;

	public bool IsAICompanionMore;

	public bool IsDisableSnow;

	public bool IsMarkedTanks;

	public bool IsXrayBullets;

	public bool IsGoreMode;

	[Header("Options - Controls")]
	public bool ChangeControlsMenu;

	public int ControlsMenu;

	private Event keyEvent;

	private KeyCode newKey;

	[Header("Options - Video")]
	public bool IsGraphicsDown;

	public bool IsGraphicsUp;

	public bool IsResolutionDown;

	public bool IsResolutionUp;

	public bool IsFPSDown;

	public bool IsFPSUp;

	public bool IsFullscreen;

	public bool IsVsync;

	[Header("Options - Main")]
	public bool IsVideo;

	public bool IsGamePlay;

	public bool IsAudio;

	public bool IsControls;

	[Header("Options - Audio")]
	public bool IsMasterVolumeDown;

	public bool IsMasterVolumeUp;

	public bool IsMusicVolumeDown;

	public bool IsMusicVolumeUp;

	public bool IsSFXVolumeDown;

	public bool IsSFXVolumeUp;

	[Header("Left Menu")]
	public bool IsContinue;

	public bool IsContinueCheckpoint;

	public int ContinueLevel;

	[Header("Extras Menu")]
	public bool IsUnlockables;

	public bool IsAchievements;

	public bool IsStatistics;

	public bool IsCredits;

	[Header("Achievements")]
	public bool ChangeAchievementDifficulty;

	public int AchievementDifficulty;

	[Header("Statistics")]
	public bool ChangeStatisticScreen;

	public int StatisticScreen;

	[Header("Survival Maps")]
	public bool IsSurvivalMap;

	public int SurvivalMapNumber;

	[Header("Map Editor Menu")]
	public bool IsSmallMap;

	public bool IsNormalMap;

	public bool IsBigMap;

	public bool IsLargeMap;

	[Header("Map Editor Menu")]
	public bool IsPlayMap;

	public bool IsPlayMapFile;

	public bool IsLoadMap;

	public bool IsLoadMapFile;

	public bool IsCreateMap;

	public bool IsSaveMap;

	public bool IsSaveMapFile;

	public bool IsCancelReplace;

	public bool IsApproveReplace;

	public bool IsExitTesting;

	public bool IsRefreshButton;

	public bool IsBack;

	public bool IsBack2Menu;

	public bool IsBackCustomMenu;

	public bool IsBackPrevMenu;

	public int menuNumber;

	public bool Selected;

	public bool MouseOn;

	public bool ControllerOn;

	public float speed = 2f;

	public float startTime;

	public bool canBeSelected = true;

	private float journeyLength;

	public float distanceNow;

	private Color originalColor;

	public Color unavailableColor;

	private int LastKnownDifficulty;

	private int CompletedMissions;

	private int CompletedHardMissions;

	private int CompletedKidMissions;

	private int CompletedGrandpa;

	public ScrollRect ParentSR;

	private bool RightClicked;

	public bool IsSelected;

	private float _StartTime;

	public float _Speed = 1f;

	public float _Offset;

	public float _OffsetChevronAnimation;

	public float _OffsetGlowAnimation;

	private float _PrevValue;

	private void Awake()
	{
		StartingPosY = base.transform.localPosition.y;
		StartingScale = base.transform.localScale;
	}

	private void CheckCompletedMissions()
	{
		if (AccountMaster.instance.isSignedIn && AccountMaster.instance.PDO != null)
		{
			CompletedMissions = AccountMaster.instance.PDO.maxMission0;
			CompletedKidMissions = AccountMaster.instance.PDO.maxMission1;
			CompletedHardMissions = AccountMaster.instance.PDO.maxMission2;
			CompletedGrandpa = AccountMaster.instance.PDO.maxMission3;
		}
		else
		{
			CompletedMissions = GameMaster.instance.maxMissionReached;
			CompletedKidMissions = GameMaster.instance.maxMissionReachedKid;
			CompletedHardMissions = GameMaster.instance.maxMissionReachedHard;
			CompletedGrandpa = GameMaster.instance.CurrentData.maxMission3;
		}
		if (CompletedMissions <= ContinueLevel && CompletedKidMissions <= ContinueLevel && CompletedHardMissions <= ContinueLevel)
		{
			if (!IsSurvivalMode)
			{
				_ = SurvivalMapNumber;
			}
			_ = IsContinue;
		}
		else if (IsSurvivalMap && ShowObjectWhenSelected != null)
		{
			ShowObjectWhenSelected.SetActive(value: false);
			ShowObjectWhenSelected = null;
		}
	}

	private void Start()
	{
		StartingScale = base.transform.localScale;
		ButtonAnimator = GetComponent<Animator>();
		if ((bool)ChavronBorder)
		{
			StartColor = ChavronBorder.color;
		}
		if ((bool)ButtonBorder)
		{
			StartColorButton = ButtonBorder.color;
		}
		if ((bool)ButtonGlow)
		{
			ButtonGlow.GetComponent<Image>().color = new Color(HoverColor.r, HoverColor.g, HoverColor.b, 0.2f);
		}
		if ((bool)base.transform.parent && (bool)base.transform.parent.transform.parent && (bool)base.transform.parent.transform.parent.transform.parent)
		{
			ParentSR = base.transform.parent.transform.parent.transform.parent.gameObject.GetComponent<ScrollRect>();
		}
		GameObject gameObject = GameObject.Find("PauseMenuCanvas");
		if ((bool)gameObject)
		{
			PMS = gameObject.GetComponent<PauseMenuScript>();
			speed = 5.5f;
		}
		GameObject gameObject2 = GameObject.Find("Canvas");
		if ((bool)gameObject2 && gameObject == null)
		{
			NMC = gameObject2.GetComponent<NewMenuControl>();
			speed = 100f;
		}
		if (IsContinue || IsSurvivalMap)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				CheckCompletedMissions();
			}
			else if (SavingData.ExistData())
			{
				ProgressDataNew progressDataNew = SavingData.LoadData();
				CompletedMissions = progressDataNew.cM;
				CompletedKidMissions = progressDataNew.cK;
				CompletedHardMissions = progressDataNew.cH;
				CompletedGrandpa = progressDataNew.cG;
				if (CompletedMissions <= ContinueLevel && CompletedHardMissions <= ContinueLevel)
				{
					if (!IsSurvivalMode)
					{
						_ = SurvivalMapNumber;
					}
					if (!IsContinue)
					{
					}
				}
				else if (IsSurvivalMap)
				{
					ShowObjectWhenSelected.SetActive(value: false);
					ShowObjectWhenSelected = null;
				}
			}
			else
			{
				_ = SurvivalMapNumber;
			}
		}
		startTime = Time.time;
		journeyLength = Vector3.Distance(newpos, oldpos);
		StartCoroutine(DelayLoadButton());
	}

	private IEnumerator DelayLoadButton()
	{
		yield return new WaitForSeconds(2f);
		LoadButton();
	}

	public void MakeUnavailable()
	{
		canBeSelected = false;
		ButtonTitle.color = UnavailableColor;
		ButtonMetaImage.texture = ButtonMetaNegative;
		ButtonMetaImage.SetNativeSize();
		ButtonMetaImage.color = UnavailableColor;
		if (IsContinue || IsSurvivalMap)
		{
			ButtonMetaImage.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
		}
	}

	public void MakeAvailable()
	{
		canBeSelected = true;
		ButtonTitle.color = HoverColor;
		ButtonMetaImage.texture = ButtonMetaPositive;
		ButtonMetaImage.SetNativeSize();
		ButtonMetaImage.color = HoverColor;
		if (IsContinue || IsSurvivalMap)
		{
			ButtonMetaImage.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		}
	}

	public void SwitchedMenu()
	{
		PointerLeave();
		if ((bool)ChavronBorder)
		{
			ChavronBorder.color = StartColor;
		}
		if ((bool)ButtonBorder)
		{
			ButtonBorder.color = StartColorButton;
		}
		if (StartingScale == Vector3.zero)
		{
			StartingScale = Vector3.one;
		}
		base.transform.localScale = StartingScale;
		MouseOn = false;
	}

	public void LoadButton()
	{
		RightClicked = false;
		if (!ButtonMetaTitle)
		{
			return;
		}
		if (IsClassicCampaign)
		{
			if (AccountMaster.instance.isSignedIn && AccountMaster.instance.PDO != null)
			{
				ButtonMetaTitle.text = (AccountMaster.instance.PDO.maxMission0 + AccountMaster.instance.PDO.maxMission1 + AccountMaster.instance.PDO.maxMission2 + AccountMaster.instance.PDO.maxMission3) / 4 + "%";
			}
			else
			{
				ButtonMetaTitle.text = "-%";
			}
		}
		else if (IsSurvivalMode)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				int num = 0;
				for (int i = 0; i < GameMaster.instance.highestWaves.Length; i++)
				{
					if (GameMaster.instance.highestWaves[i] > num)
					{
						num = GameMaster.instance.highestWaves[i];
					}
				}
				ButtonMetaTitle.text = num.ToString();
			}
			else
			{
				ButtonMetaTitle.text = "-";
			}
		}
		else if (IsOnlineButton)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				ButtonMetaTitle.text = "";
				ButtonMetaDescription.text = "Signed in";
				ButtonMetaDescription.color = AvailableColor;
				ButtonMetaImage.texture = ButtonMetaPositive;
				ButtonMetaImage.SetNativeSize();
				ButtonMetaImage.transform.localScale = Vector3.one;
			}
			else
			{
				ButtonMetaTitle.text = "";
				ButtonMetaDescription.text = "Not signed in";
				ButtonMetaDescription.color = UnavailableColor;
				ButtonMetaImage.texture = ButtonMetaNegative;
				ButtonMetaImage.SetNativeSize();
				ButtonMetaImage.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			}
		}
		else if (IsMapEditor)
		{
			if ((bool)NMC)
			{
				ButtonMetaTitle.text = NMC.AmountMaps.ToString();
			}
		}
		else if (IsContinueCheckpoint)
		{
			if (!AccountMaster.instance.isSignedIn && SavingData.ExistData())
			{
				ProgressDataNew progressDataNew = SavingData.LoadData();
				CompletedMissions = progressDataNew.cM;
				CompletedKidMissions = progressDataNew.cK;
				CompletedHardMissions = progressDataNew.cH;
				CompletedGrandpa = progressDataNew.cG;
			}
			if (ContinueLevel != 99)
			{
				ButtonTitle.text = "Checkpoint " + ContinueLevel;
			}
			if (CanPlayCheckPoint())
			{
				MakeAvailable();
			}
			else
			{
				MakeUnavailable();
			}
		}
		else if (IsSurvivalMap)
		{
			if (!AccountMaster.instance.isSignedIn && SavingData.ExistData())
			{
				ProgressDataNew progressDataNew2 = SavingData.LoadData();
				CompletedMissions = progressDataNew2.cM;
				CompletedKidMissions = progressDataNew2.cK;
				CompletedHardMissions = progressDataNew2.cH;
				CompletedGrandpa = progressDataNew2.cG;
			}
			if (CanPlaySurvivalMap())
			{
				MakeAvailable();
			}
			else
			{
				MakeUnavailable();
			}
		}
	}

	private bool CanPlayCheckPoint()
	{
		if (OptionsMainMenu.instance.currentDifficulty == 0)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				if (AccountMaster.instance.PDO.maxMission0 >= ContinueLevel)
				{
					return true;
				}
			}
			else if (CompletedMissions >= ContinueLevel)
			{
				return true;
			}
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 1)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				if (AccountMaster.instance.PDO.maxMission1 >= ContinueLevel)
				{
					return true;
				}
			}
			else if (CompletedKidMissions >= ContinueLevel)
			{
				return true;
			}
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 2)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				if (AccountMaster.instance.PDO.maxMission2 >= ContinueLevel)
				{
					return true;
				}
			}
			else if (CompletedHardMissions >= ContinueLevel)
			{
				return true;
			}
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 3)
		{
			if (AccountMaster.instance.isSignedIn)
			{
				if (AccountMaster.instance.PDO.maxMission3 >= ContinueLevel)
				{
					return true;
				}
			}
			else if (CompletedGrandpa >= ContinueLevel)
			{
				return true;
			}
		}
		return false;
	}

	private bool CanPlaySurvivalMap()
	{
		if (AccountMaster.instance.isSignedIn)
		{
			if (AccountMaster.instance.PDO.maxMission0 >= SurvivalMapNumber * 10)
			{
				return true;
			}
			if (AccountMaster.instance.PDO.maxMission1 >= SurvivalMapNumber * 10)
			{
				return true;
			}
			if (AccountMaster.instance.PDO.maxMission2 >= SurvivalMapNumber * 10)
			{
				return true;
			}
			if (AccountMaster.instance.PDO.maxMission3 >= SurvivalMapNumber * 10)
			{
				return true;
			}
		}
		else
		{
			if (CompletedMissions >= SurvivalMapNumber * 10)
			{
				return true;
			}
			if (CompletedKidMissions >= SurvivalMapNumber * 10)
			{
				return true;
			}
			if (CompletedHardMissions >= SurvivalMapNumber * 10)
			{
				return true;
			}
			if (CompletedGrandpa >= SurvivalMapNumber * 10)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator TransitionColor(bool Entered)
	{
		float t2;
		if (Entered)
		{
			t2 = 0f;
			while (t2 < 1f)
			{
				t2 += Time.deltaTime * 8f;
				if ((bool)ChavronBorder)
				{
					ChavronBorder.color = Color.Lerp(StartColor, HoverColor, t2);
				}
				if ((bool)ButtonBorder)
				{
					ButtonBorder.color = Color.Lerp(StartColorButton, HoverColor, t2);
				}
				yield return null;
			}
			yield break;
		}
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime * 8f;
			if ((bool)ChavronBorder)
			{
				ChavronBorder.color = Color.Lerp(HoverColor, StartColor, t2);
			}
			if ((bool)ButtonBorder)
			{
				ButtonBorder.color = Color.Lerp(HoverColor, StartColorButton, t2);
			}
			yield return null;
		}
	}

	public void PointerLeave()
	{
		if ((bool)ButtonGlow)
		{
			ButtonGlow.localPosition = ButtonGlowStartPos;
		}
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MenuAway);
		if ((bool)ButtonAnimator)
		{
			ButtonAnimator.SetBool("Hover", value: false);
		}
		if ((bool)ButtonBorder)
		{
			ButtonBorder.color = StartColorButton;
		}
		MouseOn = false;
		ControllerOn = false;
		_StartTime = 0f;
	}

	public void MouseLeft()
	{
	}

	public void PointerEnter()
	{
		if (!canBeSelected)
		{
			return;
		}
		_StartTime = 0f;
		if ((bool)ButtonAnimator)
		{
			ButtonAnimator.Play("MainMenuBtn_hover", -1, 0f);
			ButtonAnimator.SetBool("Hover", value: true);
		}
		SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MenuHover);
		if ((bool)ChavronBorder)
		{
			ChavronBorder.color = HoverColor;
		}
		MouseOn = true;
		ControllerOn = true;
		CheckCompletedMissions();
		if (!canBeSelected)
		{
			return;
		}
		if (NMC != null)
		{
			if (NMC.Selection != Place)
			{
				NMC.Selection = Place;
				StartCoroutine(PlayMarkerSound());
			}
		}
		else if (PMS.Selection != Place)
		{
			PMS.Selection = Place;
			StartCoroutine(PlayMarkerSound());
		}
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (canBeSelected)
		{
			if (pointerEventData.button == PointerEventData.InputButton.Right)
			{
				NMC.doRightButton(this);
				RightClicked = true;
			}
			else
			{
				PointerClick();
			}
		}
	}

	public void OnScroll()
	{
		if ((bool)ParentSR)
		{
			ParentSR.verticalNormalizedPosition += Input.mouseScrollDelta.y / 16f;
		}
	}

	public void PointerClick()
	{
		if (canBeSelected && !RightClicked)
		{
			if ((bool)NMC)
			{
				NMC.doButton(this);
			}
			else
			{
				PMS.doButton(this);
			}
			SFXManager.instance.PlaySFX(GlobalAssets.instance.AudioDB.MenuClick);
		}
	}

	private IEnumerator PlayMarkerSound()
	{
		yield return new WaitForSecondsRealtime(0.2f);
		if (Selected && distanceNow < 76f)
		{
			if (NMC != null)
			{
				SFXManager.instance.PlaySFX(NMC.MarkerSound, 0.8f);
			}
			else if (PMS != null)
			{
				SFXManager.instance.PlaySFX(PMS.MarkerSound, 0.8f);
			}
		}
	}

	private void SelectUp()
	{
		if (NMC != null)
		{
			if (NMC.Selection == Place && !NMC.selectedRKS)
			{
				NMC.Selection++;
				StartCoroutine(PlayMarkerSound());
			}
		}
		else if (PMS.Selection == Place)
		{
			PMS.Selection++;
			StartCoroutine(PlayMarkerSound());
		}
	}

	private void SelectDown()
	{
		if (NMC != null)
		{
			if (NMC.Selection == Place && !NMC.selectedRKS)
			{
				NMC.Selection--;
				StartCoroutine(PlayMarkerSound());
			}
		}
		else if (PMS.Selection == Place)
		{
			PMS.Selection--;
			StartCoroutine(PlayMarkerSound());
		}
	}

	public void Update()
	{
		if ((IsBack || IsBack2Menu || IsBackCustomMenu) && menuNumber != 12 && NMC != null && NMC.player != null && (NMC.player.GetButtonUp("Escape") || NMC.player.GetButtonUp("Menu Back")))
		{
			NMC.doButton(this);
		}
		if ((bool)NMC)
		{
			if (NMC.player != null && (NMC.player.GetButtonUp("Menu Use") || NMC.player.GetButtonUp("Use")))
			{
				NMC.doButton(this);
			}
			if (NMC != null)
			{
				if (NMC.Selection == Place && ControllerOn && !MouseOn && NMC.IsUsingMouse)
				{
					PointerLeave();
				}
				if (NMC.player != null && NMC.Selection == Place)
				{
					if (!ControllerOn && !NMC.IsUsingMouse)
					{
						PointerEnter();
					}
					if (NMC.player.GetAxis("Move Vertically") > 0f)
					{
						if (NMC.Selection > 0 && NMC.CanMove)
						{
							NMC.StartCoroutine(NMC.MoveSelection(up: true));
						}
					}
					else if (NMC.player.GetAxis("Move Vertically") < 0f && !isLast && NMC.CanMove)
					{
						NMC.StartCoroutine(NMC.MoveSelection(up: false));
					}
				}
				else if (NMC.Selection != Place && !NMC.IsUsingMouse && MouseOn)
				{
					PointerLeave();
				}
			}
		}
		_StartTime += Time.deltaTime;
		if (MouseOn)
		{
			if ((bool)ChavronBorder)
			{
				float x = 0.55f + Mathf.Sin((_StartTime + _OffsetChevronAnimation) * 4f) * 0.25f;
				ChavronBorder.transform.GetComponent<RectTransform>().pivot = new Vector2(x, 0.5f);
				if ((bool)ChavronBackground)
				{
					ChavronBackground.transform.GetComponent<RectTransform>().pivot = new Vector2(x, 0.5f);
				}
			}
			if (HoverOnEnter)
			{
				Vector3 a = new Vector3(base.transform.localPosition.x, StartingPosY, base.transform.localPosition.z);
				Vector3 b = new Vector3(base.transform.localPosition.x, StartingPosY + 6f, base.transform.localPosition.z);
				base.transform.localPosition = Vector3.Lerp(a, b, _StartTime * 4f);
			}
			if ((bool)ButtonGlow)
			{
				float num = Mathf.Abs(Mathf.Sin(_StartTime + 4f));
				if (num > 0f && num > _PrevValue)
				{
					ButtonGlow.localPosition = Vector3.Lerp(ButtonGlowStartPos, ButtonGlowEndPos, num);
				}
				else
				{
					ButtonGlow.localPosition = ButtonGlowStartPos;
				}
				_PrevValue = num;
			}
			if ((bool)ButtonBorder)
			{
				ButtonBorder.color = Color.Lerp(Color.white, HoverColor, Mathf.Abs(Mathf.Sin((_StartTime + _Offset) * _Speed)));
			}
		}
		else if (HoverOnEnter)
		{
			Vector3 b2 = new Vector3(base.transform.localPosition.x, StartingPosY, base.transform.localPosition.z);
			Vector3 a2 = new Vector3(base.transform.localPosition.x, StartingPosY + 6f, base.transform.localPosition.z);
			base.transform.localPosition = Vector3.Lerp(a2, b2, _StartTime * 6f);
		}
	}

	public void Clicked()
	{
		if (IsAimAssist)
		{
			if (!OptionsMainMenu.instance.AimAssist)
			{
				OptionsMainMenu.instance.AimAssist = true;
				GetComponent<TextMeshProUGUI>().text = "(x)";
			}
			else
			{
				OptionsMainMenu.instance.AimAssist = false;
				GetComponent<TextMeshProUGUI>().text = "( )";
			}
		}
	}
}
