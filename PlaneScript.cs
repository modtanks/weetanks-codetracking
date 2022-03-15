using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
	public bool isBomberPlane;

	public bool isFlying;

	public Transform SpawnBoxPoint;

	public GameObject BoxPrefab;

	public List<GameObject> Tanks = new List<GameObject>();

	public List<GameObject> OriginalTanks = new List<GameObject>();

	public Vector3 TargetLocation;

	public Vector3 PlaneTargetLocation;

	public Vector3 StartLocation;

	public float speed = 10f;

	public float startTime;

	private bool BoxSpawned;

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

	public bool CanDropBombs;

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
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		for (int j = 0; j < array.Length; j++)
		{
			HealthTanks component = array[j].GetComponent<HealthTanks>();
			if ((bool)component && component.EnemyID == 17)
			{
				EnemyAI component2 = component.GetComponent<EnemyAI>();
				if ((bool)component2)
				{
					CommandosActiveColor[component2.MyTeam]++;
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
			int num = 0;
			for (int i = 0; i < CommandosActiveColor.Length; i++)
			{
				if (CommandosActiveColor[i] > 0)
				{
					num++;
				}
			}
			CurrentTeamNumber = TeamNumber;
			if (RecentSpawnedInColor == -1)
			{
				RecentSpawnedInColor = TeamNumber;
			}
			else if (CommandosActives > 1 && num > 1 && TeamNumber == RecentSpawnedInColor && Random.Range(0, 6) != 2)
			{
				PlaneModel.SetActive(value: false);
				return;
			}
			RecentSpawnedInColor = TeamNumber;
		}
		isFlying = true;
		int num2 = ((Random.Range(0, 2) == 0) ? (-55) : 55);
		float y = (isBomberPlane ? 20f : 10f);
		StartLocation = TargetLocation + new Vector3(num2, y, 0f);
		base.transform.position = StartLocation;
		Vector3 vector = TargetLocation - StartLocation;
		Vector3 vector2 = vector + vector.normalized * 60f;
		PlaneTargetLocation = StartLocation + vector2;
		Vector3 forward = PlaneTargetLocation - base.transform.position;
		forward.y = 0f;
		Quaternion rotation = Quaternion.LookRotation(forward);
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
			Vector3 position = SpawnBoxPoint.position + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
			GameObject gameObject = Object.Instantiate(BoxPrefab, position, Quaternion.Euler(SpawnBoxPoint.transform.forward));
			PlayerBulletScript component = gameObject.GetComponent<PlayerBulletScript>();
			component.MyTeam = 0;
			Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
			if ((bool)component2)
			{
				component2.AddForce(SpawnBoxPoint.forward * 3f);
				component.StartingVelocity = SpawnBoxPoint.forward * 3f;
				component.BulletSpeed = 3f;
				gameObject.transform.Rotate(Vector3.right * 90f);
				_ = SpawnBoxPoint.position;
				Ray ray = new Ray(SpawnBoxPoint.position, SpawnBoxPoint.forward);
				LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall")) | (1 << LayerMask.NameToLayer("FLOOR"));
				Debug.DrawRay(SpawnBoxPoint.position, SpawnBoxPoint.forward * 15f, Color.cyan, 3f);
				if (Physics.Raycast(ray, out var hitInfo, 500f, layerMask))
				{
					component.WallGonnaBounceInTo = hitInfo.transform.gameObject;
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
			float num = (Time.time - startTime) * speed / journeyLength;
			base.transform.position = Vector3.Lerp(StartLocation, PlaneTargetLocation, num);
			float y = Mathf.Pow(num * 4f - 2f, 2f) + 6f;
			if (num < 0.5f)
			{
				mySource.volume = num * 2f;
			}
			else
			{
				mySource.volume = 2f - num * 2f;
			}
			base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
			if (!isBomberPlane)
			{
				if (num > 0.3f && num < 0.5f && !myAnimator.GetBool("KlepOpen"))
				{
					myAnimator.SetBool("KlepOpen", value: true);
				}
				else if (num > 0.6f && num < 0.8f && myAnimator.GetBool("KlepOpen"))
				{
					myAnimator.SetBool("KlepOpen", value: false);
				}
			}
			if ((bool)PlaneParticles && !PlaneParticles.isPlaying)
			{
				PlaneParticles.Play();
			}
			if (num > 1f)
			{
				isFlying = false;
				BoxSpawned = false;
				PlaneModel.SetActive(value: false);
				mySource.volume = 0f;
			}
			else if (isBomberPlane)
			{
				CanDropBombs = false;
				LayerMask layerMask = 1 << LayerMask.NameToLayer("TeleportBlock");
				Debug.DrawRay(base.transform.position, -Vector3.up * 25f, Color.red, 3f);
				if (Physics.Raycast(base.transform.position, -Vector3.up, out var _, 25f, layerMask))
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
				LayerMask layerMask2 = 1 << LayerMask.NameToLayer("FLOOR");
				Debug.DrawRay(base.transform.position, -Vector3.up * 25f, Color.red, 3f);
				if (!Physics.Raycast(base.transform.position, -Vector3.up, out var hitInfo2, 25f, layerMask2))
				{
					return;
				}
				float num2 = Vector3.Distance(hitInfo2.point, new Vector3(TargetLocation.x, 0f, TargetLocation.z));
				if (!(num2 < 3f))
				{
					return;
				}
				if (num2 < 3f && GameMaster.instance.GameHasStarted)
				{
					BoxSpawned = true;
					CarePackageScript component = Object.Instantiate(BoxPrefab, new Vector3(TargetLocation.x, 0f, TargetLocation.z), Quaternion.identity).transform.GetChild(0).GetComponent<CarePackageScript>();
					if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.CustomTankDatas.Count > 0)
					{
						for (int i = 0; i < MapEditorMaster.instance.CustomTankDatas.Count; i++)
						{
							if (MapEditorMaster.instance.CustomTankDatas[i].CustomCanBeAirdropped)
							{
								Tanks.Add(MapEditorMaster.instance.Props[19]);
								component.mats.Add(MapEditorMaster.instance.CustomMaterial[i]);
							}
						}
					}
					GameObject gameObject;
					if (TankToSpawnID < 0)
					{
						do
						{
							int num3 = Random.Range(0, Tanks.Count);
							if (SkipTankIndexKid.Contains(num3) && OptionsMainMenu.instance.currentDifficulty == 1)
							{
								gameObject = null;
							}
							else if (SkipTankIndexToddler.Contains(num3) && OptionsMainMenu.instance.currentDifficulty == 0)
							{
								gameObject = null;
							}
							else if (num3 != 15 && num3 != 16 && num3 != 17)
							{
								gameObject = Tanks[num3];
								component.index = num3;
							}
							else if (num3 > 17)
							{
								gameObject = Tanks[num3];
								component.index = 18 - num3;
							}
							else
							{
								gameObject = null;
							}
						}
						while (gameObject == null);
					}
					else
					{
						gameObject = Tanks[TankToSpawnID];
						component.index = TankToSpawnID;
					}
					component.TankToSpawn = gameObject;
					component.MyTeam = CurrentTeamNumber;
					if ((bool)MapEditorMaster.instance && MapEditorMaster.instance.CustomTankDatas.Count > 0)
					{
						for (int j = 0; j < MapEditorMaster.instance.CustomTankDatas.Count; j++)
						{
							if (MapEditorMaster.instance.CustomTankDatas[j].CustomCanBeAirdropped)
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
