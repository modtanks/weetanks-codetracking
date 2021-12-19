using System.Linq;
using UnityEngine;

public class PlaneScript : MonoBehaviour
{
	public bool isFlying;

	public Transform SpawnBoxPoint;

	public GameObject BoxPrefab;

	public GameObject[] Tanks;

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

	private void Start()
	{
		myAnimator = GetComponent<Animator>();
		PlaneParticles.Stop();
		InvokeRepeating("SearchCommandos", 2f, 2f);
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
		isFlying = true;
		int num2 = ((Random.Range(0, 2) == 0) ? (-55) : 55);
		float z = Random.Range(-20f, 20f);
		StartLocation = new Vector3(num2, 10f, z);
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
		if (!PlaneParticles.isPlaying)
		{
			PlaneParticles.Play();
		}
		journeyLength = Vector3.Distance(StartLocation, PlaneTargetLocation);
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
				mySource.volume = 1f - num;
			}
			base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
			if (num > 0.3f && num < 0.5f && !myAnimator.GetBool("KlepOpen"))
			{
				myAnimator.SetBool("KlepOpen", value: true);
			}
			else if (num > 0.6f && num < 0.8f && myAnimator.GetBool("KlepOpen"))
			{
				myAnimator.SetBool("KlepOpen", value: false);
			}
			if (!PlaneParticles.isPlaying)
			{
				PlaneParticles.Play();
			}
			if (num > 1f)
			{
				isFlying = false;
				BoxSpawned = false;
				mySource.volume = 0f;
			}
			if (BoxSpawned)
			{
				return;
			}
			LayerMask layerMask = 1 << LayerMask.NameToLayer("FLOOR");
			Debug.DrawRay(base.transform.position, -Vector3.up * 25f, Color.red, 3f);
			if (!Physics.Raycast(base.transform.position, -Vector3.up * 25f, out var hitInfo, (int)layerMask))
			{
				return;
			}
			float num2 = Vector3.Distance(hitInfo.point, new Vector3(TargetLocation.x, 0f, TargetLocation.z));
			if (!(num2 < 3f))
			{
				return;
			}
			if (num2 < 3f && GameMaster.instance.GameHasStarted)
			{
				BoxSpawned = true;
				CarePackageScript component = Object.Instantiate(BoxPrefab, new Vector3(TargetLocation.x, 0f, TargetLocation.z), Quaternion.identity).transform.GetChild(0).GetComponent<CarePackageScript>();
				GameObject gameObject;
				if (TankToSpawnID < 0)
				{
					do
					{
						int num3 = Random.Range(0, Tanks.Length);
						if (SkipTankIndexKid.Contains(num3) && OptionsMainMenu.instance.currentDifficulty == 1)
						{
							gameObject = null;
							continue;
						}
						if (SkipTankIndexToddler.Contains(num3) && OptionsMainMenu.instance.currentDifficulty == 0)
						{
							gameObject = null;
							continue;
						}
						gameObject = Tanks[num3];
						component.index = num3;
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
				PlaneModel.SetActive(value: true);
			}
			else if (!GameMaster.instance.GameHasStarted)
			{
				BoxSpawned = true;
			}
		}
		else
		{
			if (PlaneParticles.isPlaying)
			{
				PlaneParticles.Stop();
			}
			mySource.volume = 0f;
			PlaneModel.SetActive(value: false);
		}
	}
}
