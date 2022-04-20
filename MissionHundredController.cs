using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionHundredController : MonoBehaviour
{
	public bool DevMode = false;

	public int BossPhase = 0;

	public GameObject[] BlocksToMoveDown;

	public GameObject[] BlocksToMoveUp;

	public GameObject CrateToSpawn;

	public List<GameObject> DestroyableCratesToRespawn = new List<GameObject>();

	public List<Vector3> CratesPositions = new List<Vector3>();

	public Transform Player1Checkpoint;

	public Transform Player2Checkpoint;

	public Transform Player3Checkpoint;

	public Transform Player4Checkpoint;

	public GameObject[] ObjectsToDisable;

	public GameObject[] BoostTowers;

	public GameObject FinalArena;

	public PlaneScript BomberPlane;

	[Header("Camera To Arena")]
	public Vector3 ArenaCameraPosition;

	public Vector3 ArenaCameraObjectPosition;

	public float ArenaCameraFOV;

	[Header("Camera To Win")]
	public Vector3 WinArenaCameraPosition;

	public Vector3 WinArenaCameraObjectPosition;

	public float WinArenaCameraFOV;

	public GameObject Plane1;

	public GameObject Plane2;

	public GameObject Plane3;

	public GameObject[] BlocksToDisableWhenThroneBroken;

	public Camera CameraObject;

	public PlaneMaster PM;

	public AudioClip WallsMovingDown;

	public AudioClip Phase0Music;

	public AudioClip Phase1Music;

	public AudioClip Phase3Music;

	public AudioClip AirRaid;

	public MeshRenderer[] LightBlocks;

	public Light[] Lights;

	private AudioSource mySource;

	public KingTankScript KTS;

	public Animator FinalBossDoorAnimator;

	public Transform[] DropLocations;

	public float TimeInBattle = 0f;

	private static MissionHundredController _instance;

	private NewOrchestra orchestra;

	private bool MovedCameraToDefeat = false;

	private bool MessageHasBeenShown = false;

	public static MissionHundredController instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		StartCoroutine(InitiateAirRaid());
		PM = GameMaster.instance.GetComponent<PlaneMaster>();
		CameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		orchestra = GameObject.FindGameObjectWithTag("Orchestra").GetComponent<NewOrchestra>();
		mySource = GetComponent<AudioSource>();
		foreach (GameObject crate in DestroyableCratesToRespawn)
		{
			CratesPositions.Add(crate.transform.position);
		}
		for (int i = 0; i < 4; i++)
		{
			if (!GameMaster.instance.PlayerJoined[i] || GameMaster.instance.PlayerModeWithAI[i] != 0)
			{
				Object.Destroy(DestroyableCratesToRespawn[i]);
			}
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.HasGotten100Checkpoint)
		{
			if (orchestra.isPlaying)
			{
				orchestra.StopPlaying();
			}
			if (mySource.clip != Phase0Music || !mySource.isPlaying)
			{
				mySource.clip = Phase0Music;
				mySource.loop = true;
				mySource.volume = mySource.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
				mySource.Play();
			}
		}
		if (KTS.IsInBattle || KTS.IsInFinalBattle)
		{
			TimeInBattle += Time.deltaTime;
		}
		else
		{
			TimeInBattle = 0f;
		}
		float calc = 0.1f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		mySource.volume = calc;
		if (KTS == null || KTS.BossDefeated)
		{
			if (!MovedCameraToDefeat)
			{
				StartCoroutine(MoveCameraToBossDefeat());
			}
			MovedCameraToDefeat = true;
			mySource.volume = 0f;
			return;
		}
		if (!GameMaster.instance.GameHasStarted && !DevMode)
		{
			BossPhase = 0;
			return;
		}
		if (!GameMaster.instance.GameHasStarted && DevMode)
		{
			BossPhase = 2;
			KTS.IsInFinalBattle = true;
			KTS.Throne.PlatformHealth = 0;
			KTS.enabled = true;
			KTS.ActivateBoss();
		}
		if (GameMaster.instance.AmountEnemyTanks <= 1 && !GameMaster.instance.HasGotten100Checkpoint)
		{
			GameObject[] blocksToMoveDown = BlocksToMoveDown;
			foreach (GameObject Block in blocksToMoveDown)
			{
				StartCoroutine("MoveBlockDown", Block);
			}
			Debug.Log("set points");
			BossPhase = 0;
			GameMaster.instance.HasGotten100Checkpoint = true;
			GameMaster.instance.playerLocation[0] = Player1Checkpoint.position;
			GameMaster.instance.playerLocation[1] = Player2Checkpoint.position;
			GameMaster.instance.playerLocation[2] = Player3Checkpoint.position;
			GameMaster.instance.playerLocation[3] = Player4Checkpoint.position;
			FinalArena.SetActive(value: true);
		}
	}

	public void PlayMusic(int music)
	{
		switch (music)
		{
		case -1:
			mySource.Stop();
			break;
		case 0:
			mySource.clip = Phase1Music;
			mySource.loop = true;
			mySource.volume = mySource.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			mySource.Play();
			break;
		default:
			mySource.clip = Phase3Music;
			mySource.loop = true;
			mySource.volume = mySource.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			mySource.Play();
			break;
		}
	}

	private IEnumerator AirRaidLight(Light L)
	{
		float t2 = 0f;
		Color OG = L.color;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime;
			L.color = Color.Lerp(OG, Color.red, t2);
			yield return null;
		}
		yield return new WaitForSeconds(4f);
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime;
			L.color = Color.Lerp(Color.red, OG, t2);
			yield return null;
		}
	}

	private IEnumerator AirRaidBlock(MeshRenderer R)
	{
		float t2 = 0f;
		Color OG = R.material.GetColor("_EmissionColor");
		while (t2 < 1f)
		{
			t2 += Time.deltaTime;
			Color color = Color.Lerp(OG, Color.red, t2);
			R.material.SetColor("_EmissionColor", color);
			yield return null;
		}
		yield return new WaitForSeconds(4f);
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime;
			Color color2 = Color.Lerp(Color.red, OG, t2);
			R.material.SetColor("_EmissionColor", color2);
			yield return null;
		}
	}

	private IEnumerator InitiateAirRaid()
	{
		float RandomWaitingTime = Random.Range(10f, 30f);
		yield return new WaitForSeconds(RandomWaitingTime);
		if ((KTS.IsInBattle || KTS.IsInFinalBattle) && TimeInBattle > 10f && KTS.HT.health > 0 && OptionsMainMenu.instance.currentDifficulty > 0)
		{
			Vector3 Location = GameMaster.instance.GetValidLocation(CheckForDist: false, 0f, Vector3.zero, TargetPlayer: true);
			if (Location != Vector3.zero)
			{
				BomberPlane.TargetLocation = new Vector3(Location.x, 10f, Location.z);
			}
			BomberPlane.StartFlyToTB(-1, -1);
			SFXManager.instance.PlaySFX(AirRaid);
			Light[] lights = Lights;
			foreach (Light L in lights)
			{
				StartCoroutine(AirRaidLight(L));
			}
			MeshRenderer[] lightBlocks = LightBlocks;
			foreach (MeshRenderer R in lightBlocks)
			{
				StartCoroutine(AirRaidBlock(R));
			}
		}
		yield return new WaitForSeconds(8f);
		StartCoroutine(InitiateAirRaid());
	}

	public IEnumerator DoPhasing1()
	{
		KTS.IsInBattle = true;
		Debug.Log("starting phase 1a");
		Animator Plane1Animator = Plane1.GetComponent<Animator>();
		if (!Plane1Animator.GetBool("FlyAway"))
		{
			Plane1Animator.Play("MovePlaneForward", -1, 0f);
			if ((bool)Plane1Animator)
			{
				Plane1Animator.SetBool("FlyAway", value: true);
			}
		}
		GameMaster.instance.musicScript.Orchestra.StopPlaying();
		if (!mySource.isPlaying || mySource.clip == Phase0Music)
		{
			PlayMusic(0);
		}
		yield return new WaitForSeconds(3f);
		if (!KTS.IsInBattle || KTS.IsInFinalBattle)
		{
			yield break;
		}
		int AmountOfSpawns = Random.Range(2, 5);
		List<int> DropsToAdd = new List<int>();
		for (int i = 0; i < AmountOfSpawns; i++)
		{
			if (KTS.Throne.PlatformHealth <= 3)
			{
				int[] TanksDrops3 = null;
				if (OptionsMainMenu.instance.currentDifficulty == 0)
				{
					TanksDrops3 = new int[5] { 3, 4, 5, 6, 8 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 1)
				{
					TanksDrops3 = new int[4] { 4, 5, 6, 8 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 2)
				{
					TanksDrops3 = new int[5] { 6, 7, 8, 9, 11 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 3)
				{
					TanksDrops3 = new int[7] { 2, 6, 7, 8, 9, 11, 14 };
				}
				int picked3 = TanksDrops3[Random.Range(0, TanksDrops3.Length)];
				DropsToAdd.Add(picked3);
			}
			else if (KTS.Throne.PlatformHealth <= 6)
			{
				int[] TanksDrops2 = null;
				if (OptionsMainMenu.instance.currentDifficulty == 0)
				{
					TanksDrops2 = new int[4] { 0, 1, 3, 4 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 1)
				{
					TanksDrops2 = new int[4] { 1, 3, 4, 5 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 2)
				{
					TanksDrops2 = new int[5] { 3, 4, 5, 6, 7 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 3)
				{
					TanksDrops2 = new int[8] { 4, 5, 6, 7, 8, 9, 11, 14 };
				}
				int picked2 = TanksDrops2[Random.Range(0, TanksDrops2.Length)];
				DropsToAdd.Add(picked2);
			}
			else
			{
				int[] TanksDrops = null;
				if (OptionsMainMenu.instance.currentDifficulty == 0)
				{
					TanksDrops = new int[2] { 0, 1 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 1)
				{
					TanksDrops = new int[4] { 0, 1, 3, 4 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 2)
				{
					TanksDrops = new int[5] { 0, 1, 3, 4, 5 };
				}
				else if (OptionsMainMenu.instance.currentDifficulty == 3)
				{
					TanksDrops = new int[6] { 0, 1, 3, 4, 5, 6 };
				}
				int picked = TanksDrops[Random.Range(0, TanksDrops.Length)];
				DropsToAdd.Add(picked);
			}
		}
		for (int j = 0; j < DropsToAdd.Count; j++)
		{
			int temp = DropsToAdd[j];
			int randomIndex = Random.Range(j, DropsToAdd.Count);
			DropsToAdd[j] = DropsToAdd[randomIndex];
			DropsToAdd[randomIndex] = temp;
		}
		foreach (int Drop in DropsToAdd)
		{
			PM.SpawnInOrder.Add(Drop);
		}
		PM.StartCoroutine(PM.DoOrder());
		if (!KTS.IsInBattle)
		{
			yield break;
		}
		if (GameMaster.instance.Enemies != null && (GameMaster.instance.Enemies.Count > 1 || GameMaster.instance.AmountCalledInTanks > 0))
		{
			yield return new WaitForSeconds(7f);
		}
		yield return new WaitForSeconds(13f);
		if (!KTS.IsInFinalBattle && KTS.IsInBattle)
		{
			AmountOfSpawns = Random.Range(1, 4);
			StartCoroutine(KTS.ShootABomb(first: true, AmountOfSpawns));
			yield return new WaitForSeconds(4f);
			if (!MessageHasBeenShown)
			{
				MessageHasBeenShown = true;
				TutorialMaster.instance.ShowTutorial("Use mines on the bombs!");
			}
			yield return new WaitForSeconds(7f);
			if (KTS.IsInBattle && !KTS.IsInFinalBattle)
			{
				StartCoroutine(DoPhasing1());
			}
		}
	}

	public IEnumerator StartPhase2a()
	{
		KTS.IsInBattle = true;
		Debug.Log("starting phase 2a");
		Animator Plane2Animator = Plane2.GetComponent<Animator>();
		Plane2Animator.Play("MovePlaneForward", -1, 0f);
		if ((bool)Plane2Animator)
		{
			Plane2Animator.SetBool("FlyAway", value: true);
		}
		GameMaster.instance.musicScript.Orchestra.StopPlaying();
		if (!mySource.isPlaying)
		{
			mySource.clip = Phase1Music;
			mySource.volume = mySource.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			mySource.loop = true;
			mySource.Play();
		}
		yield return new WaitForSeconds(2f);
		List<int> DropsToAdd = new List<int>();
		PM.PS.speed = 60f;
		DropsToAdd.Add(8);
		DropsToAdd.Add(8);
		if (Random.Range(0, 5) == 0)
		{
			DropsToAdd.Add(16);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 0)
		{
			DropsToAdd.Add(1);
			DropsToAdd.Add(6);
			DropsToAdd.Add(11);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 1)
		{
			DropsToAdd.Add(0);
			DropsToAdd.Add(12);
			DropsToAdd.Add(8);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 2)
		{
			DropsToAdd.Add(2);
			DropsToAdd.Add(11);
			DropsToAdd.Add(7);
		}
		for (int i = 0; i < DropsToAdd.Count; i++)
		{
			int temp = DropsToAdd[i];
			int randomIndex = Random.Range(i, DropsToAdd.Count);
			DropsToAdd[i] = DropsToAdd[randomIndex];
			DropsToAdd[randomIndex] = temp;
		}
		foreach (int Drop in DropsToAdd)
		{
			PM.SpawnInOrder.Add(Drop);
		}
		PM.StartCoroutine(PM.DoOrder());
		BossPhase = 3;
	}

	public void PlayerDied()
	{
		if (KTS.BossDefeated)
		{
			SceneManager.LoadScene(5);
			return;
		}
		BossPhase = 0;
		KTS.IsInBattle = false;
		mySource.Stop();
		mySource.loop = false;
		if (KTS.ShotMortars.Count > 0)
		{
			KTS.ShotMortars = KTS.ShotMortars.Where((GameObject item) => item != null).ToList();
			foreach (GameObject mortar in KTS.ShotMortars)
			{
				Object.Destroy(mortar);
			}
		}
		KTS.DeactivateBoss();
		KTS.IsInFinalBattle = false;
		GameObject[] boostTowers = BoostTowers;
		foreach (GameObject Tower in boostTowers)
		{
			Tower.SetActive(value: true);
		}
		KTS.Throne.PlatformHealth = KTS.Throne.PlatformMaxHealth;
		KTS.transform.position = KTS.StartingPosition;
		KTS.transform.rotation = KTS.StartingRotation;
		for (int i = 0; i < DestroyableCratesToRespawn.Count; i++)
		{
			if (DestroyableCratesToRespawn[i] == null && GameMaster.instance.PlayerJoined[i] && GameMaster.instance.PlayerModeWithAI[i] == 0)
			{
				GameObject newCrate = Object.Instantiate(CrateToSpawn, CratesPositions[i], Quaternion.identity);
				DestroyableCratesToRespawn[i] = newCrate;
			}
		}
	}

	public void FinalBossTrigger()
	{
		GameObject[] blocksToMoveUp = BlocksToMoveUp;
		foreach (GameObject Block in blocksToMoveUp)
		{
			StartCoroutine("MoveBlockUp", Block);
		}
		StartCoroutine(MoveCameraTo());
		SFXManager.instance.PlaySFX(WallsMovingDown, 1f, null);
		CameraFollowPlayer CFP = CameraObject.transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
		CFP.enabled = false;
	}

	public IEnumerator MoveCameraTo()
	{
		WinArenaCameraPosition = CameraObject.transform.parent.transform.position;
		WinArenaCameraObjectPosition = CameraObject.transform.localPosition;
		WinArenaCameraFOV = CameraObject.orthographicSize;
		float currentFOV = CameraObject.orthographicSize;
		Vector3 currentPos = CameraObject.transform.parent.transform.position;
		Vector3 currentSelfPos = CameraObject.transform.localPosition;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			CameraObject.transform.parent.transform.position = Vector3.Lerp(currentPos, ArenaCameraPosition, t);
			CameraObject.transform.localPosition = Vector3.Lerp(currentSelfPos, ArenaCameraObjectPosition, t);
			CameraObject.orthographicSize = Mathf.Lerp(currentFOV, ArenaCameraFOV, t);
			yield return null;
		}
		if (BossPhase < 2)
		{
			StartCoroutine(DoPhasing1());
		}
		else
		{
			KTS.ActivateBoss();
		}
		CameraObject.transform.parent.transform.position = ArenaCameraPosition;
		CameraObject.orthographicSize = ArenaCameraFOV;
	}

	public IEnumerator MoveCameraToBossDefeat()
	{
		CameraFollowPlayer CFP = CameraObject.transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
		CameraObject.orthographicSize = WinArenaCameraFOV;
		CFP.enabled = true;
		FinalBossDoorAnimator.SetBool("BossKilled", value: true);
		GameMaster.instance.GameHasStarted = true;
		yield return new WaitForSeconds(2f);
		GameMaster.instance.CurrentMission = 100;
	}

	public IEnumerator MoveBlockDown(GameObject Block)
	{
		Vector3 currentPos = Block.transform.position;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			Block.transform.position = Vector3.Lerp(currentPos, new Vector3(Block.transform.position.x, -0.97f, Block.transform.position.z), t);
			yield return null;
		}
		Block.GetComponent<BoxCollider>().enabled = false;
	}

	public IEnumerator MoveBlockUp(GameObject Block)
	{
		Vector3 currentPos = new Vector3(Block.transform.position.x, -0.97f, Block.transform.position.z);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			Block.transform.position = Vector3.Lerp(currentPos, new Vector3(Block.transform.position.x, 1f, Block.transform.position.z), t);
			yield return null;
		}
		Block.GetComponent<BoxCollider>().enabled = true;
		GameObject[] objectsToDisable = ObjectsToDisable;
		foreach (GameObject Blocks in objectsToDisable)
		{
			Object.Destroy(Blocks);
		}
	}
}
