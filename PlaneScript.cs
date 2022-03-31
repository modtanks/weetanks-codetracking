using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
	public bool isBomberPlane = false;

	public bool isFlying = false;

	public Transform SpawnBoxPoint;

	public GameObject BoxPrefab;

	public List<GameObject> Tanks = new List<GameObject>();

	public List<GameObject> OriginalTanks = new List<GameObject>();

	public Vector3 TargetLocation;

	public Vector3 PlaneTargetLocation;

	public Vector3 StartLocation;

	public float speed = 10f;

	public float startTime;

	private bool BoxSpawned = false;

	private float journeyLength;

	public AudioSource mySource;

	private Animator myAnimator;

	private int CurrentTeamNumber = 2;

	public int[] SkipTankIndexToddler;

	public int[] SkipTankIndexKid;

	public int[] CommandosActiveColor;

	public int CommandosActives;

	public int RecentSpawnedInColor = -1;

	public GameObject PlaneModel;

	public ParticleSystem PlaneParticles;

	private int TankToSpawnID = -1;

	public bool CanDropBombs = false;

	private void Start()
	{
		OriginalTanks = Tanks;
		myAnimator = GetComponent<Animator>();
		if ((bool)PlaneParticles)
		{
			PlaneParticles.Stop();
		}
		if (!isBomberPlane)
		{
			InvokeRepeating("SearchCommandos", 2f, 2f);
		}
		if (GameMaster.instance.CurrentMission < 99)
		{
			PlaneModel.SetActive(value: false);
		}
	}

	private void SearchCommandos()
	{
		for (int i = 0; i < CommandosActiveColor.Length; i++)
		{
			CommandosActiveColor[i] = 0;
		}
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] array = Enemies;
		foreach (GameObject Enemy in array)
		{
			HealthTanks HT = Enemy.GetComponent<HealthTanks>();
			if ((bool)HT && HT.EnemyID == 17)
			{
				EnemyAI EA = HT.GetComponent<EnemyAI>();
				if ((bool)EA)
				{
					CommandosActiveColor[EA.MyTeam]++;
				}
			}
		}
		CommandosActives = CommandosActiveColor[0] + CommandosActiveColor[1] + CommandosActiveColor[2] + CommandosActiveColor[3] + CommandosActiveColor[4];
	}

	public void StartFlyToTB(int TeamNumber, int TankID)
	{
		PlaneModel.SetActive(value: true);
		if (!isBomberPlane)
		{
			TankToSpawnID = -1;
			if (TankID > -1)
			{
				TankToSpawnID = TankID;
			}
			int DifferentTeams = 0;
			for (int i = 0; i < CommandosActiveColor.Length; i++)
			{
				if (CommandosActiveColor[i] > 0)
				{
					DifferentTeams++;
				}
			}
			CurrentTeamNumber = TeamNumber;
			if (RecentSpawnedInColor == -1)
			{
				RecentSpawnedInColor = TeamNumber;
			}
			else if (CommandosActives > 1 && DifferentTeams > 1 && TeamNumber == RecentSpawnedInColor)
			{
				int chanceToSpawn = Random.Range(0, 6);
				if (chanceToSpawn != 2)
				{
					PlaneModel.SetActive(value: false);
					return;
				}
			}
			RecentSpawnedInColor = TeamNumber;
		}
		isFlying = true;
		int X = ((Random.Range(0, 2) == 0) ? (-55) : 55);
		float Yheight = (isBomberPlane ? 20f : 10f);
		StartLocation = TargetLocation + new Vector3(X, Yheight, 0f);
		base.transform.position = StartLocation;
		Vector3 direction = TargetLocation - StartLocation;
		Vector3 finalDirection = direction + direction.normalized * 60f;
		PlaneTargetLocation = StartLocation + finalDirection;
		Vector3 lookPos = PlaneTargetLocation - base.transform.position;
		lookPos.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(lookPos);
		base.transform.rotation = rotation;
		base.transform.Rotate(0f, 180f, 0f);
		startTime = Time.time;
		if ((bool)PlaneParticles && !PlaneParticles.isPlaying)
		{
			PlaneParticles.Play();
		}
		journeyLength = Vector3.Distance(StartLocation, PlaneTargetLocation);
		if (isBomberPlane)
		{
			StartCoroutine(DropBombs());
		}
	}

	private IEnumerator DropBombs()
	{
		if (CanDropBombs)
		{
			GameObject bulletGO = Object.Instantiate(position: SpawnBoxPoint.position + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f)), original: BoxPrefab, rotation: Quaternion.Euler(SpawnBoxPoint.transform.forward));
			PlayerBulletScript bullet = bulletGO.GetComponent<PlayerBulletScript>();
			bullet.MyTeam = 0;
			Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
			if ((bool)bulletBody)
			{
				bulletBody.AddForce(SpawnBoxPoint.forward * 3f);
				bullet.StartingVelocity = SpawnBoxPoint.forward * 3f;
				bullet.BulletSpeed = 3f;
				bulletGO.transform.Rotate(Vector3.right * 90f);
				_ = SpawnBoxPoint.position;
				Ray ray = new Ray(SpawnBoxPoint.position, SpawnBoxPoint.forward);
				LayerMask LM = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall")) | (1 << LayerMask.NameToLayer("FLOOR"));
				Debug.DrawRay(SpawnBoxPoint.position, SpawnBoxPoint.forward * 15f, Color.cyan, 3f);
				if (Physics.Raycast(ray, out var hit, 500f, LM))
				{
					bullet.WallGonnaBounceInTo = hit.transform.gameObject;
				}
			}
		}
		yield return new WaitForSeconds(0.1f);
		if (isFlying)
		{
			StartCoroutine(DropBombs());
		}
	}

	private void Update()
	{
		if (isFlying)
		{
			float distCovered = (Time.time - startTime) * speed;
			float fractionOfJourney = distCovered / journeyLength;
			base.transform.position = Vector3.Lerp(StartLocation, PlaneTargetLocation, fractionOfJourney);
			float CurrentPos = fractionOfJourney * 4f - 2f;
			float Yvalue = Mathf.Pow(CurrentPos, 2f) + 6f;
			if (fractionOfJourney < 0.5f)
			{
				mySource.volume = fractionOfJourney * 2f;
			}
			else
			{
				mySource.volume = 2f - fractionOfJourney * 2f;
			}
			base.transform.position = new Vector3(base.transform.position.x, Yvalue, base.transform.position.z);
			if (!isBomberPlane)
			{
				if (fractionOfJourney > 0.3f && fractionOfJourney < 0.5f && !myAnimator.GetBool("KlepOpen"))
				{
					myAnimator.SetBool("KlepOpen", value: true);
				}
				else if (fractionOfJourney > 0.6f && fractionOfJourney < 0.8f && myAnimator.GetBool("KlepOpen"))
				{
					myAnimator.SetBool("KlepOpen", value: false);
				}
			}
			if ((bool)PlaneParticles && !PlaneParticles.isPlaying)
			{
				PlaneParticles.Play();
			}
			if (fractionOfJourney > 1f)
			{
				isFlying = false;
				BoxSpawned = false;
				PlaneModel.SetActive(value: false);
				mySource.volume = 0f;
			}
			else if (isBomberPlane)
			{
				CanDropBombs = false;
				LayerMask LM2 = 1 << LayerMask.NameToLayer("TeleportBlock");
				Debug.DrawRay(base.transform.position, -Vector3.up * 25f, Color.red, 3f);
				if (Physics.Raycast(base.transform.position, -Vector3.up, out var _, 25f, LM2))
				{
					CanDropBombs = true;
				}
				else
				{
					CanDropBombs = false;
				}
			}
			else
			{
				if (BoxSpawned || isBomberPlane)
				{
					return;
				}
				LayerMask LM = 1 << LayerMask.NameToLayer("FLOOR");
				Debug.DrawRay(base.transform.position, -Vector3.up * 25f, Color.red, 3f);
				if (!Physics.Raycast(base.transform.position, -Vector3.up, out var RH, 25f, LM))
				{
					return;
				}
				float DistToPoint = Vector3.Distance(RH.point, new Vector3(TargetLocation.x, 0f, TargetLocation.z));
				if (!(DistToPoint < 3f))
				{
					return;
				}
				if (DistToPoint < 3f && GameMaster.instance.GameHasStarted)
				{
					BoxSpawned = true;
					GameObject Box = Object.Instantiate(BoxPrefab, new Vector3(TargetLocation.x, 0f, TargetLocation.z), Quaternion.identity);
					CarePackageScript CPS = Box.transform.GetChild(0).GetComponent<CarePackageScript>();
					if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.CustomTankDatas.Count > 0)
					{
						for (int j = 0; j < MapEditorMaster.instance.CustomTankDatas.Count; j++)
						{
							if (MapEditorMaster.instance.CustomTankDatas[j].CustomCanBeAirdropped)
							{
								Tanks.Add(MapEditorMaster.instance.Props[19]);
								CPS.mats.Add(MapEditorMaster.instance.CustomMaterial[j]);
							}
						}
					}
					GameObject TankSpawn;
					if (TankToSpawnID < 0)
					{
						do
						{
							int spawnInt = Random.Range(0, Tanks.Count);
							if (SkipTankIndexKid.Contains(spawnInt) && OptionsMainMenu.instance.currentDifficulty == 1)
							{
								TankSpawn = null;
							}
							else if (SkipTankIndexToddler.Contains(spawnInt) && OptionsMainMenu.instance.currentDifficulty == 0)
							{
								TankSpawn = null;
							}
							else if (spawnInt != 15 && spawnInt != 16 && spawnInt != 17)
							{
								TankSpawn = Tanks[spawnInt];
								CPS.index = spawnInt;
							}
							else if (spawnInt > 17)
							{
								TankSpawn = Tanks[spawnInt];
								CPS.index = 18 - spawnInt;
							}
							else
							{
								TankSpawn = null;
							}
						}
						while (TankSpawn == null);
					}
					else
					{
						TankSpawn = Tanks[TankToSpawnID];
						CPS.index = TankToSpawnID;
					}
					CPS.TankToSpawn = TankSpawn;
					CPS.MyTeam = CurrentTeamNumber;
					if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.CustomTankDatas.Count > 0)
					{
						for (int i = 0; i < MapEditorMaster.instance.CustomTankDatas.Count; i++)
						{
							if (MapEditorMaster.instance.CustomTankDatas[i].CustomCanBeAirdropped)
							{
								Tanks.Remove(MapEditorMaster.instance.Props[19]);
							}
						}
					}
					PlaneModel.SetActive(value: true);
				}
				else if (!GameMaster.instance.GameHasStarted)
				{
					BoxSpawned = true;
				}
			}
		}
		else
		{
			if ((bool)PlaneParticles && PlaneParticles.isPlaying)
			{
				PlaneParticles.Stop();
			}
			mySource.volume = 0f;
			PlaneModel.SetActive(value: false);
		}
	}
}
