using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionHundredController : MonoBehaviour
{
	public bool DevMode;

	public int BossPhase;

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

	public Camera CameraObject;

	public PlaneMaster PM;

	public AudioClip WallsMovingDown;

	public AudioClip Phase1Music;

	public AudioClip Phase3Music;

	private AudioSource mySource;

	public KingTankScript KTS;

	public Animator FinalBossDoorAnimator;

	private bool MovedCameraToDefeat;

	private void Start()
	{
		PM = GameMaster.instance.GetComponent<PlaneMaster>();
		CameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		mySource = GetComponent<AudioSource>();
		foreach (GameObject item in DestroyableCratesToRespawn)
		{
			CratesPositions.Add(item.transform.position);
		}
	}

	private void Update()
	{
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
			BossPhase = 5;
			KTS.IsInFinalBattle = true;
			KTS.Platform_1.PlatformHealth = 0;
			KTS.Platform_2.PlatformHealth = 0;
			KTS.enabled = true;
			KTS.ActivateBoss();
		}
		if (GameMaster.instance.AmountEnemyTanks <= 1 && !GameMaster.instance.HasGotten100Checkpoint)
		{
			GameObject[] blocksToMoveDown = BlocksToMoveDown;
			foreach (GameObject value in blocksToMoveDown)
			{
				StartCoroutine("MoveBlockDown", value);
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
		else if (BossPhase == 1 && PM.SpawnInOrder.Count < 1 && !PM.PS.isFlying)
		{
			if (!KTS.IsInMortarMode && GameMaster.instance.AmountEnemyTanks <= 1 && GameMaster.instance.Enemies == null)
			{
				Debug.Log("PHASE 2");
				BossPhase = 2;
				KTS.GoInMortarMode(BossPhase);
			}
		}
		else if (BossPhase == 3 && PM.SpawnInOrder.Count < 1 && !PM.PS.isFlying && !KTS.IsInMortarMode && GameMaster.instance.AmountEnemyTanks <= 1 && GameMaster.instance.Enemies == null)
		{
			Debug.Log("PHASE 4");
			BossPhase = 4;
			KTS.GoInMortarMode(BossPhase);
		}
	}

	public IEnumerator StartPhase1a()
	{
		KTS.IsInBattle = true;
		Debug.Log("starting phase 1a");
		Animator component = Plane1.GetComponent<Animator>();
		component.Play("MovePlaneForward", -1, 0f);
		if ((bool)component)
		{
			component.SetBool("FlyAway", value: true);
		}
		GameMaster.instance.musicScript.Orchestra.StopPlaying();
		if (!mySource.isPlaying)
		{
			mySource.clip = Phase1Music;
			mySource.loop = true;
			mySource.Play();
		}
		yield return new WaitForSeconds(2f);
		List<int> list = new List<int>();
		PM.PS.speed = 50f;
		list.Add(0);
		list.Add(0);
		list.Add(1);
		list.Add(1);
		list.Add(15);
		if (Random.Range(0, 4) == 0)
		{
			list.Add(15);
			if (Random.Range(0, 4) == 0)
			{
				list.Add(15);
			}
		}
		if (Random.Range(0, 8) == 0)
		{
			list.Add(16);
		}
		if (Random.Range(0, 4) == 0)
		{
			list.Add(17);
			if (Random.Range(0, 4) == 0)
			{
				list.Add(17);
			}
		}
		if (OptionsMainMenu.instance.currentDifficulty > 0)
		{
			list.Add(1);
			list.Add(1);
			list.Add(3);
			list.Add(3);
			list.Add(4);
			list.Add(4);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 1)
		{
			list.Add(4);
			list.Add(4);
			list.Add(5);
			list.Add(5);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 2)
		{
			list.Add(6);
			list.Add(6);
		}
		for (int i = 0; i < list.Count; i++)
		{
			int value = list[i];
			int index = Random.Range(i, list.Count);
			list[i] = list[index];
			list[index] = value;
		}
		foreach (int item in list)
		{
			PM.SpawnInOrder.Add(item);
		}
		PM.StartCoroutine(PM.DoOrder());
		BossPhase = 1;
	}

	public IEnumerator StartPhase2a()
	{
		KTS.IsInBattle = true;
		Debug.Log("starting phase 2a");
		Animator component = Plane2.GetComponent<Animator>();
		component.Play("MovePlaneForward", -1, 0f);
		if ((bool)component)
		{
			component.SetBool("FlyAway", value: true);
		}
		GameMaster.instance.musicScript.Orchestra.StopPlaying();
		if (!mySource.isPlaying)
		{
			mySource.clip = Phase1Music;
			mySource.loop = true;
			mySource.Play();
		}
		yield return new WaitForSeconds(2f);
		List<int> list = new List<int>();
		PM.PS.speed = 60f;
		list.Add(8);
		list.Add(8);
		if (Random.Range(0, 3) == 0)
		{
			list.Add(15);
			if (Random.Range(0, 4) == 0)
			{
				list.Add(15);
			}
		}
		if (Random.Range(0, 8) == 0)
		{
			list.Add(16);
		}
		if (Random.Range(0, 3) == 0)
		{
			list.Add(17);
			if (Random.Range(0, 3) == 0)
			{
				list.Add(17);
			}
		}
		if (OptionsMainMenu.instance.currentDifficulty > 0)
		{
			list.Add(1);
			list.Add(6);
			list.Add(11);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 1)
		{
			list.Add(0);
			list.Add(12);
			list.Add(8);
		}
		if (OptionsMainMenu.instance.currentDifficulty > 2)
		{
			list.Add(2);
			list.Add(11);
			list.Add(7);
		}
		for (int i = 0; i < list.Count; i++)
		{
			int value = list[i];
			int index = Random.Range(i, list.Count);
			list[i] = list[index];
			list[index] = value;
		}
		foreach (int item in list)
		{
			PM.SpawnInOrder.Add(item);
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
			foreach (GameObject shotMortar in KTS.ShotMortars)
			{
				Object.Destroy(shotMortar);
			}
		}
		KTS.DeactivateBoss();
		KTS.IsInFinalBattle = false;
		KTS.CurrentLayer = 2;
		GameObject[] boostTowers = BoostTowers;
		for (int i = 0; i < boostTowers.Length; i++)
		{
			boostTowers[i].SetActive(value: true);
		}
		KTS.Platform_1.PlatformHealth = KTS.Platform_1.PlatformMaxHealth;
		KTS.Platform_2.PlatformHealth = KTS.Platform_1.PlatformMaxHealth;
		KTS.transform.position = KTS.StartingPosition;
		KTS.transform.rotation = KTS.StartingRotation;
		for (int j = 0; j < DestroyableCratesToRespawn.Count; j++)
		{
			if (DestroyableCratesToRespawn[j] == null)
			{
				GameObject value = Object.Instantiate(CrateToSpawn, CratesPositions[j], Quaternion.identity);
				DestroyableCratesToRespawn[j] = value;
			}
		}
	}

	public IEnumerator StartPhase1b()
	{
		yield return new WaitForSeconds(2f);
		if (PM.SpawnInOrder.Count >= 1)
		{
			StartCoroutine(StartPhase1a());
		}
	}

	public void FinalBossTrigger()
	{
		GameObject[] blocksToMoveUp = BlocksToMoveUp;
		foreach (GameObject value in blocksToMoveUp)
		{
			StartCoroutine("MoveBlockUp", value);
		}
		StartCoroutine(MoveCameraTo());
		GameMaster.instance.Play2DClipAtPoint(WallsMovingDown, 1f);
		CameraObject.transform.parent.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
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
		if (BossPhase < 4)
		{
			StartCoroutine(StartPhase1a());
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
		CameraFollowPlayer component = CameraObject.transform.parent.gameObject.GetComponent<CameraFollowPlayer>();
		CameraObject.orthographicSize = WinArenaCameraFOV;
		component.enabled = true;
		FinalBossDoorAnimator.SetBool("BossKilled", value: true);
		GameMaster.instance.GameHasStarted = true;
		yield break;
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
		for (int i = 0; i < objectsToDisable.Length; i++)
		{
			Object.Destroy(objectsToDisable[i]);
		}
	}
}
