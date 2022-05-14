using System.Collections;
using UnityEngine;

public class EnemyTargetingSystem : MonoBehaviour
{
	public EnemyAI AIscript;

	public GameObject[] Player;

	public int amountplayers;

	public int currentTarget;

	public GameObject bulletPrefab;

	public GameObject SecondBulletPrefab;

	public Transform[] firePoint;

	public AudioClip shotSound;

	private AudioSource source;

	public Vector3 target;

	public Quaternion rot;

	public Vector3 startingPosition;

	public bool canMine;

	public float mineCountdown;

	public GameObject minePrefab;

	public Transform[] rocketSlotLocations;

	public int[] rocketSlots;

	public GameObject rocketPrefab;

	public GameObject[] rockets;

	[HideInInspector]
	public int maxReflectionCount = 2;

	public float maxStepDistance = 200f;

	private int lvl30BossSpecial = 6;

	private bool canReloadRockets;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private float minVelocity = 10f;

	private Vector3 lastFrameVelocity;

	public bool specialMove;

	public bool PlayerInSight;

	public bool CR_running;

	private void Start()
	{
		rot = Quaternion.Euler(0f, 0f, 0f);
		mineCountdown = 2f + AIscript.LayMinesSpeed;
		source = GetComponent<AudioSource>();
		InvokeRepeating("PlayerInsight", 0.3f, 0.3f);
		if (AIscript.BouncyBullets)
		{
			InvokeRepeating("CheckForPlayer", 0.03f, 0.03f);
			maxReflectionCount = AIscript.amountOfBounces;
		}
	}

	private void OnEnable()
	{
		CR_running = false;
	}

	private void Update()
	{
		Player = GameObject.FindGameObjectsWithTag("Player");
		if (mineCountdown <= 0f)
		{
			canMine = true;
		}
		mineCountdown -= Time.deltaTime;
		if (firePoint.Length < 2 || (firePoint.Length < 12 && AIscript.transform.tag == "Enemy"))
		{
			if (!specialMove && amountplayers > 0)
			{
				if (amountplayers < 2)
				{
					currentTarget = 0;
				}
				initialVelocity = firePoint[0].transform.forward * 100f;
				target = new Vector3(Player[currentTarget].transform.position.x, firePoint[0].transform.position.y, Player[currentTarget].transform.position.z);
				Vector3 forward = Player[currentTarget].transform.position - base.transform.position;
				forward.y = 0f;
				Quaternion rotation = Quaternion.LookRotation(forward);
				base.transform.rotation = rotation;
			}
			else
			{
				Vector3 forward2 = startingPosition - base.transform.position;
				forward2.y = 0f;
				Quaternion b = Quaternion.LookRotation(forward2);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 18.5f);
			}
		}
		else
		{
			base.transform.rotation *= Quaternion.Euler(0f, Time.deltaTime * AIscript.lvl30BossTurnSpeed, 0f);
		}
	}

	public void PlayerInsight()
	{
		Debug.Log("Player insighting check" + base.name);
		if (AIscript.IsTurning)
		{
			return;
		}
		LayerMask layerMask = ~((1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")) | (1 << LayerMask.NameToLayer("Tank")));
		if (!Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var hitInfo, float.PositiveInfinity, layerMask))
		{
			return;
		}
		if (hitInfo.collider.tag != "Player" && hitInfo.transform.tag != "Player")
		{
			PlayerInSight = false;
			return;
		}
		PlayerInSight = true;
		if (!CR_running)
		{
			StartCoroutine("ShootAtPlayer");
		}
	}

	public void CheckForPlayer()
	{
		if (!PlayerInSight && !specialMove)
		{
			if (!DrawReflectionPattern(base.transform.position, rot * base.transform.forward, maxReflectionCount))
			{
				rot *= Quaternion.Euler(0f, 3f, 0f);
			}
			else
			{
				specialMove = true;
			}
		}
	}

	private bool DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
	{
		do
		{
			Vector3 start = position;
			Ray ray = new Ray(position, direction);
			LayerMask layerMask = ~((1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
			if (Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask))
			{
				if (reflectionsRemaining == maxReflectionCount)
				{
					startingPosition = hitInfo.point;
				}
				direction = Vector3.Reflect(direction, hitInfo.normal);
				position = hitInfo.point;
				if (hitInfo.collider.tag == "Player" && hitInfo.collider.tag != "Enemy")
				{
					return true;
				}
				if (hitInfo.collider.tag == "Enemy" || hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("NoBounceWall"))
				{
					return false;
				}
			}
			else
			{
				position += direction * maxStepDistance;
			}
			Debug.DrawLine(start, position, Color.red);
			reflectionsRemaining--;
		}
		while (reflectionsRemaining > 0);
		return false;
	}

	private IEnumerator ShootAtPlayer()
	{
		CR_running = true;
		if (GameMaster.instance.AmountGoodTanks < 1)
		{
			yield break;
		}
		_ = AIscript.ShootSpeed;
		if (firePoint.Length < 2 || (firePoint.Length < 12 && AIscript.transform.tag == "Enemy"))
		{
			float seconds = ((!specialMove) ? (AIscript.ShootSpeed + Random.Range(0f, 1f)) : AIscript.ShootSpeed);
			yield return new WaitForSeconds(seconds);
			if (specialMove)
			{
				Transform[] array = firePoint;
				foreach (Transform firepoint in array)
				{
					FireTank(firepoint, bulletPrefab);
				}
				specialMove = false;
			}
			if (PlayerInSight)
			{
				if (!Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var _, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Tank")))
				{
					Transform[] array = firePoint;
					foreach (Transform firepoint2 in array)
					{
						FireTank(firepoint2, bulletPrefab);
					}
				}
			}
			else if (amountplayers > 1)
			{
				if (currentTarget == 0)
				{
					currentTarget = 1;
				}
				else
				{
					currentTarget = 0;
				}
			}
			else
			{
				currentTarget = 0;
			}
		}
		else if (AIscript.HTscript.health < AIscript.lvl30Boss3modeLives && lvl30BossSpecial < 6)
		{
			float seconds = AIscript.ShootSpeed;
			yield return new WaitForSeconds(seconds);
			for (int k = 0; k < 8; k++)
			{
				FireTank(firePoint[k], SecondBulletPrefab);
			}
			if (lvl30BossSpecial > 1)
			{
				lvl30BossSpecial--;
			}
			else
			{
				lvl30BossSpecial = 12;
			}
		}
		else
		{
			float seconds = AIscript.ShootSpeed;
			yield return new WaitForSeconds(seconds);
			for (int l = 0; l < firePoint.Length; l++)
			{
				FireTank(firePoint[l], bulletPrefab);
			}
			if (AIscript.HTscript.health < AIscript.lvl30Boss3modeLives)
			{
				lvl30BossSpecial--;
			}
		}
		if (AIscript.LayMines && canMine && !AIscript.MineFleeing && !EnemiesNearMe(4) && Random.Range(0, 2) == 1)
		{
			Object.Instantiate(minePrefab, base.transform.position + new Vector3(0f, -0.65f, 0f), Quaternion.identity).transform.parent = base.transform.parent.parent;
			canMine = false;
			mineCountdown = 2f + AIscript.LayMinesSpeed;
		}
		if (AIscript.hasRockets)
		{
			for (int i = 0; i < rocketSlots.Length; i++)
			{
				if (rocketSlots[i] == 0 && canReloadRockets)
				{
					rocketSlots[i] = 1;
					GameObject gameObject = Object.Instantiate(rocketPrefab, rocketSlotLocations[i]);
					rockets[i] = gameObject;
					gameObject.transform.parent = rocketSlotLocations[i];
					if (i + 1 < rocketSlots.Length)
					{
						yield return new WaitForSeconds(0.5f);
					}
				}
				else if (rocketSlots[i] == 1)
				{
					rockets[i].GetComponent<RocketScript>().Launch();
					rocketSlots[i] = 0;
					rockets[i] = null;
					if (i + 1 < rocketSlots.Length)
					{
						yield return new WaitForSeconds(0.5f);
						continue;
					}
					canReloadRockets = false;
					StartCoroutine("reloadRocketsTimer");
				}
			}
		}
		CR_running = false;
		StartCoroutine("ShootAtPlayer");
	}

	private IEnumerator reloadRocketsTimer()
	{
		yield return new WaitForSeconds(3f);
		canReloadRockets = true;
	}

	public void FireTank(Transform firepoint, GameObject bulletprefab)
	{
		if (!firepoint)
		{
			firepoint = firePoint[0];
		}
		if (!bulletprefab)
		{
			bulletprefab = bulletPrefab;
		}
		source.PlayOneShot(shotSound);
		GameObject obj = Object.Instantiate(bulletprefab, firepoint.position, firepoint.transform.rotation);
		EnemyBulletScript component = obj.GetComponent<EnemyBulletScript>();
		if (component != null)
		{
			component.dir = target;
		}
		obj.transform.Rotate(Vector3.right * 90f);
		obj.GetComponent<Rigidbody>().AddForce(firepoint.forward * 6f);
	}

	public bool EnemiesNearMe(int range)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, range);
		foreach (Collider collider in array)
		{
			if (collider.tag == "Enemy" && (collider.name == "Enemy_Tank-7" || collider.name == "Enemy_Tank-1"))
			{
				return true;
			}
		}
		return false;
	}
}
