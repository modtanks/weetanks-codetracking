using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButtons : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerClickHandler
{
	private RectTransform myRect;

	private RectTransform childRect;

	public RectTransform myMarker;

	private Vector3 oldpos;

	private Vector3 newpos;

	public float size = 1.1f;

	public int inMenu;

	public int Place;

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

	public bool IsSelected;

	private void CheckCompletedMissions()
	{
		if (AccountMaster.instance.isSignedIn)
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
		if (myMarker != null)
		{
			newpos = myMarker.localPosition;
			oldpos = new Vector3(newpos.x - 120f, newpos.y, newpos.z);
			myMarker.localPosition = oldpos;
		}
		thisText = GetComponent<TextMeshProUGUI>();
		originalColor = thisText.color;
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
	}

	public void OnPointerEnter(PointerEventData pointerEventData)
	{
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

	public void OnPointerStay(PointerEventData pointerEventData)
	{
	}

	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (pointerEventData.button == PointerEventData.InputButton.Right && IsContinueCheckpoint)
		{
			if ((bool)NMC && !NMC.selectedRKS)
			{
				NMC.doRightButton(this);
			}
		}
		else if (ChangeAchievementDifficulty)
		{
			if ((bool)AchievementsTracker.instance)
			{
				AchievementsTracker.instance.selectedDifficulty = AchievementDifficulty;
			}
		}
		else if (ChangeControlsMenu)
		{
			if ((bool)NMC)
			{
				NMC.ControlsOpenMenu = ControlsMenu;
			}
		}
		else if (ChangeStatisticScreen)
		{
			if ((bool)NMC)
			{
				NMC.StatisticsOpenMenu = StatisticScreen;
			}
		}
		else if ((bool)NMC && !NMC.selectedRKS)
		{
			NMC.doButton(this);
		}
		else
		{
			PMS.doButton(this);
		}
	}

	public void resetScale()
	{
		if (myRect != null)
		{
			if (myMarker != null)
			{
				myMarker.localScale = oldpos;
			}
			thisText.fontStyle = FontStyles.Normal;
		}
	}

	private IEnumerator PlayMarkerSound()
	{
		yield return new WaitForSecondsRealtime(0.2f);
		if (Selected && distanceNow < 76f)
		{
			if (NMC != null)
			{
				NMC.Play2DClipAtPoint(NMC.MarkerSound);
			}
			else if (PMS != null)
			{
				PMS.Play2DClipAtPoint(PMS.MarkerSound);
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
		if (IsDifficultyUp && OptionsMainMenu.instance.currentDifficulty == 3 && thisText.color != Color.clear)
		{
			thisText.color = Color.clear;
			canBeSelected = false;
		}
		else if (IsDifficultyUp && OptionsMainMenu.instance.currentDifficulty != 3 && thisText.color == Color.clear)
		{
			thisText.color = originalColor;
			canBeSelected = true;
		}
		if (IsDifficultyDown && OptionsMainMenu.instance.currentDifficulty == 0 && thisText.color != Color.clear)
		{
			thisText.color = Color.clear;
			canBeSelected = false;
		}
		else if (IsDifficultyDown && OptionsMainMenu.instance.currentDifficulty != 0 && thisText.color == Color.clear)
		{
			thisText.color = originalColor;
			canBeSelected = true;
		}
		if ((!IsDifficultyDown || OptionsMainMenu.instance.currentDifficulty != 0 || !Selected) && IsDifficultyUp && OptionsMainMenu.instance.currentDifficulty == 3)
		{
			_ = Selected;
		}
		if (LastKnownDifficulty != OptionsMainMenu.instance.currentDifficulty)
		{
			LastKnownDifficulty = OptionsMainMenu.instance.currentDifficulty;
			if (OptionsMainMenu.instance.currentDifficulty == 0)
			{
				if (CompletedMissions <= ContinueLevel)
				{
					if (IsContinue)
					{
						thisText.color = unavailableColor;
						canBeSelected = false;
					}
				}
				else if (IsContinue)
				{
					thisText.color = originalColor;
					canBeSelected = true;
				}
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 1)
			{
				if (CompletedKidMissions <= ContinueLevel)
				{
					if (IsContinue)
					{
						thisText.color = unavailableColor;
						canBeSelected = false;
					}
				}
				else if (IsContinue)
				{
					thisText.color = originalColor;
					canBeSelected = true;
				}
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 3)
			{
				if (CompletedGrandpa <= ContinueLevel)
				{
					if (IsContinue)
					{
						thisText.color = unavailableColor;
						canBeSelected = false;
					}
				}
				else if (IsContinue)
				{
					thisText.color = originalColor;
					canBeSelected = true;
				}
			}
			else if (CompletedHardMissions <= ContinueLevel)
			{
				if (IsContinue)
				{
					thisText.color = unavailableColor;
					canBeSelected = false;
				}
			}
			else if (IsContinue)
			{
				thisText.color = originalColor;
				canBeSelected = true;
			}
		}
		if (!Selected)
		{
			IsSelected = false;
			if (ShowObjectWhenSelected != null)
			{
				ShowObjectWhenSelected.SetActive(value: false);
			}
			if (myMarker != null)
			{
				distanceNow = Vector3.Distance(myMarker.localPosition, oldpos);
				if (distanceNow > 0.01f)
				{
					float t = (Time.time - startTime) * speed / journeyLength;
					myMarker.localPosition = Vector3.Lerp(myMarker.localPosition, oldpos, t);
				}
			}
			thisText.fontStyle = FontStyles.Normal;
		}
		else if (Selected)
		{
			if (ShowObjectWhenSelected != null)
			{
				ShowObjectWhenSelected.SetActive(value: true);
			}
			if (myMarker != null)
			{
				distanceNow = Vector3.Distance(myMarker.localPosition, newpos);
				if (distanceNow > 0.01f)
				{
					float t2 = (Time.time - startTime) * speed / journeyLength;
					myMarker.localPosition = Vector3.Lerp(myMarker.localPosition, newpos, t2);
				}
			}
		}
		if ((IsBack || IsBack2Menu || IsBackCustomMenu) && menuNumber != 12 && NMC != null && NMC.player.GetButtonUp("Menu Back"))
		{
			NMC.doButton(this);
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
