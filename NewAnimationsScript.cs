using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewAnimationsScript : MonoBehaviour
{
	[Header("Controllers")]
	public CountDownScript countscript;

	public MusicHandler musicScript;

	public TutorialCanvas tutorialCanvas;

	public Animator myAnimator;

	public Animator CamAnimator;

	[Header("Camera")]
	public Camera mainCam;

	public float ZoomedInSize = 15f;

	public float ZoomedOutSize = 15f;

	private float originalSize;

	private float duration = 2f;

	private float startTime;

	private bool zoomOutCamera;

	private bool zoomInCamera;

	[Header("Texts")]
	public TextMeshProUGUI missionNumber;

	public TextMeshProUGUI missionName;

	public TextMeshProUGUI enemyCount;

	public TextMeshProUGUI livesLeft;

	public TextMeshProUGUI BonusTank;

	public TextMeshProUGUI Checkpoint;

	public SetLivesIcons SLI;

	[Header("Audio")]
	public AudioClip Whoosh;

	public AudioClip Click;

	public AudioClip NewLife;

	public AudioClip FinalWinSound;

	public bool playingAnimation;

	public bool restart;

	private bool finished;

	public float extraWait;

	[Header("Zombie")]
	public TextMeshProUGUI EnemiesHeader;

	private GameObject root;

	public bool newTank;

	public bool checkPoint;

	private void Start()
	{
		root = base.transform.GetChild(0).gameObject;
		startTime = 0f;
		myAnimator = GetComponent<Animator>();
		CamAnimator = Camera.main.GetComponent<Animator>();
		if ((bool)CamAnimator)
		{
			if (OptionsMainMenu.instance.MapSize == 180)
			{
				CamAnimator.SetInteger("MapSize", 0);
			}
			else if (OptionsMainMenu.instance.MapSize == 285)
			{
				CamAnimator.SetInteger("MapSize", 1);
			}
			else if (OptionsMainMenu.instance.MapSize == 374)
			{
				CamAnimator.SetInteger("MapSize", 2);
			}
			else if (OptionsMainMenu.instance.MapSize == 475)
			{
				CamAnimator.SetInteger("MapSize", 3);
			}
		}
		originalSize = mainCam.orthographicSize;
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.02f);
		missionNumber.text = "";
		missionName.text = "";
		enemyCount.text = "";
		livesLeft.text = "";
	}

	public void ResetExtraWait()
	{
		StartCoroutine("WaitZero");
	}

	public void HideBoard()
	{
		root.SetActive(value: false);
	}

	public void ShowBoard()
	{
		root.SetActive(value: true);
	}

	private IEnumerator WaitZero()
	{
		yield return new WaitForSeconds(2f);
		extraWait = 0f;
	}

	private void FixedUpdate()
	{
		float num = 1f;
		if (Input.GetKey(KeyCode.LeftShift) && GameMaster.instance.Lives > 0 && !GameMaster.instance.GameHasStarted)
		{
			num = 3f;
			if (CamAnimator != null)
			{
				CamAnimator.speed = 3f;
			}
			if (myAnimator != null)
			{
				myAnimator.speed = 3f;
			}
		}
		else if (!GameMaster.instance.GameHasStarted)
		{
			num = 1f;
			if (CamAnimator != null)
			{
				CamAnimator.speed = 1f;
			}
			if (myAnimator != null && GameMaster.instance.Lives > 0)
			{
				myAnimator.speed = 1f;
			}
		}
		if ((bool)MapEditorMaster.instance || GameMaster.instance.GameHasStarted)
		{
			return;
		}
		float num2 = (Time.time - startTime) / duration * num;
		if (zoomOutCamera)
		{
			mainCam.orthographicSize = Mathf.SmoothStep(ZoomedInSize, ZoomedOutSize, num2);
			if (num2 >= 0.9f && num > 2f)
			{
				mainCam.orthographicSize = ZoomedOutSize;
				zoomOutCamera = false;
			}
		}
		if (zoomInCamera)
		{
			mainCam.orthographicSize = Mathf.SmoothStep(ZoomedOutSize, ZoomedInSize, num2);
			if (num2 >= 0.9f && num > 2f)
			{
				mainCam.orthographicSize = ZoomedInSize;
				zoomInCamera = false;
			}
		}
	}

	public void NextRound()
	{
		playingAnimation = true;
		StartCoroutine(PlayNextRound());
	}

	private IEnumerator PlayNextRound()
	{
		Debug.LogError("NEXT ROUND");
		yield return new WaitForSeconds(0.5f + extraWait);
		extraWait = 0f;
		if (newTank)
		{
			BonusTank.text = LocalizationMaster.instance.GetText("clapperboard_bonus_tank");
		}
		else
		{
			BonusTank.text = "";
		}
		if (checkPoint)
		{
			Checkpoint.text = LocalizationMaster.instance.GetText("clapperboard_checkpoint_reached") + " " + GameMaster.instance.CurrentMission + "!";
		}
		else
		{
			Checkpoint.text = "";
		}
		missionNumber.text = "";
		missionName.text = "";
		enemyCount.text = "";
		if (CamAnimator != null)
		{
			Debug.Log("TRYUE CAM");
			CamAnimator.SetBool("Play", value: true);
		}
		zoomOutCamera = true;
		zoomInCamera = false;
		startTime = Time.time;
		PlayAnimator(active: true);
	}

	private void PlayAnimator(bool active)
	{
		if (OptionsMainMenu.instance.MapSize == 374)
		{
			myAnimator.SetBool("PlayBig", active);
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			myAnimator.SetBool("PlayLarge", active);
		}
		else
		{
			myAnimator.SetBool("Play", active);
		}
	}

	public void FinishGame()
	{
		Debug.LogError("GAME FINISHED");
		missionNumber.text = "Congratulations!";
		missionName.text = "You have finished the game";
		enemyCount.text = "(for now!)";
		livesLeft.text = "";
		if (MapEditorMaster.instance != null && MapEditorMaster.instance.inPlayingMode)
		{
			OptionsMainMenu.instance.CompletedCustomCampaigns++;
			OptionsMainMenu.instance.SaveNewData();
			if ((bool)EnemiesHeader)
			{
				EnemiesHeader.text = "";
			}
			missionNumber.text = "Congratulations";
			missionName.text = "You have finished this campaign";
			enemyCount.text = "";
		}
		if (CamAnimator != null)
		{
			Debug.Log("TRYUE CAM");
			CamAnimator.SetBool("Play", value: true);
		}
		if (GameMaster.instance.CurrentMission == 49)
		{
			ZoomedInSize = 14.6f;
		}
		else
		{
			ZoomedInSize = 13.5f;
		}
		zoomOutCamera = true;
		zoomInCamera = false;
		playingAnimation = true;
		startTime = Time.time;
		Debug.LogError("made it here");
		SFXManager.instance.PlaySFX(FinalWinSound);
		PlayAnimator(active: true);
		finished = true;
	}

	private IEnumerator ExitGame()
	{
		yield return new WaitForSeconds(3f);
		if (GameMaster.instance.CurrentMission >= 99)
		{
			SceneManager.LoadScene(5);
		}
		else
		{
			SceneManager.LoadScene(0);
		}
	}

	public void StartAnimation()
	{
		if (GameMaster.instance.Lives < 2 && restart)
		{
			EnemiesHeader.text = LocalizationMaster.instance.GetText("clapperboard_kills");
			if (!GameMaster.instance.isZombieMode)
			{
				missionNumber.text = (GameMaster.instance.CurrentMission + 1).ToString();
			}
			enemyCount.text = GameMaster.instance.TotalKillsThisSession + "x";
			if (GameMaster.instance.isOfficialCampaign && !GameMaster.instance.IsSecretName)
			{
				missionName.text = LocalizationMaster.instance.GetText("Mission_" + (GameMaster.instance.CurrentMission + 1));
			}
			else
			{
				missionName.text = GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission];
			}
			livesLeft.text = LocalizationMaster.instance.GetText("clapperboard_dead");
			SLI.RemoveIcons();
		}
	}

	public void LoadRound()
	{
		extraWait = 0f;
		if (finished)
		{
			StartCoroutine("PauseAnimationFor", 5);
			StartCoroutine(ExitGame());
			return;
		}
		if (!GameMaster.instance.isZombieMode)
		{
			if (!restart)
			{
				GameMaster.instance.nextLevel();
			}
			else
			{
				GameMaster.instance.Lives--;
				if (GameMaster.instance.Lives > 0)
				{
					GameMaster.instance.ResetLevel();
				}
				restart = false;
			}
			if (newTank)
			{
				GameMaster.instance.Lives++;
				SFXManager.instance.PlaySFX(NewLife);
			}
			if (newTank || checkPoint)
			{
				StartCoroutine("PauseAnimationFor", 3f);
			}
			playingAnimation = false;
			missionNumber.text = (GameMaster.instance.CurrentMission + 1).ToString();
			enemyCount.text = GameMaster.instance.AmountEnemyTanks.ToString();
			StartCoroutine(GameMaster.instance.GetTankTeamData(fast: true));
			StartCoroutine(SetEnemyAmount());
			if ((GameMaster.instance.CurrentMission == 29 || GameMaster.instance.CurrentMission == 69) && !MapEditorMaster.instance)
			{
				musicScript.CanStartMusic = true;
			}
		}
		else
		{
			if (ZombieTankSpawner.instance.Wave < 1)
			{
				GameMaster.instance.LoadSurvivalMap();
				ZombieTankSpawner.instance.StartSpawn();
			}
			if (restart)
			{
				GameMaster.instance.Lives--;
			}
			if (!restart)
			{
				missionNumber.text = "Survive";
				enemyCount.text = "Unlimited";
			}
		}
		newTank = false;
		checkPoint = false;
		Debug.LogError("WHAT HERE ALREADY!");
		SFXManager.instance.PlaySFX(Click);
		if (GameMaster.instance.Lives >= 1)
		{
			if (GameMaster.instance.MissionNames.Count > GameMaster.instance.CurrentMission)
			{
				if (GameMaster.instance.isOfficialCampaign && !GameMaster.instance.IsSecretName)
				{
					missionName.text = LocalizationMaster.instance.GetText("Mission_" + (GameMaster.instance.CurrentMission + 1));
				}
				else
				{
					missionName.text = GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission];
				}
			}
			else
			{
				missionName.text = "";
			}
			livesLeft.text = "";
			SLI.SetIcons();
			PlayAnimator(active: false);
		}
		else
		{
			StartCoroutine("PauseAnimationFor", 5f);
			livesLeft.text = LocalizationMaster.instance.GetText("clapperboard_dead");
			StartCoroutine(GoToMainMenu());
		}
	}

	private IEnumerator SetEnemyAmount()
	{
		yield return new WaitForSeconds(0.05f);
		Debug.Log(GameMaster.instance.AmountEnemyTanks.ToString());
		enemyCount.text = GameMaster.instance.AmountEnemyTanks.ToString();
	}

	public void ReadyGame()
	{
		playingAnimation = true;
		startTime = Time.time;
		if (GameMaster.instance.CurrentMission == 49)
		{
			ZoomedInSize = 14.6f;
		}
		else if (GameMaster.instance.CurrentMission == 99)
		{
			ZoomedInSize = 10f;
		}
		else
		{
			ZoomedInSize = 13.5f;
		}
		if (GameMaster.instance.isZombieMode)
		{
			ZoomedInSize = 10f;
		}
		zoomOutCamera = false;
		zoomInCamera = true;
		if (CamAnimator != null)
		{
			Debug.Log("RESETIING CAM");
			CamAnimator.SetBool("Play", value: false);
		}
		countscript.start = true;
		musicScript.StartMusic();
		if (tutorialCanvas != null)
		{
			tutorialCanvas.CheckTut();
		}
	}

	private IEnumerator PauseAnimationFor(int sec)
	{
		Debug.Log("Stopping animationr");
		myAnimator.speed = 0f;
		yield return new WaitForSeconds(sec);
		myAnimator.speed = 1f;
	}

	private IEnumerator GoToMainMenu()
	{
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene(0);
	}
}
