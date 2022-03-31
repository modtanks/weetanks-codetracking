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

	public bool canMine = false;

	public float mineCountdown = 0f;

	public GameObject minePrefab;

	public Transform[] rocketSlotLocations;

	public int[] rocketSlots;

	public GameObject rocketPrefab;

	public GameObject[] rockets;

	[HideInInspector]
	public int maxReflectionCount = 2;

	public float maxStepDistance = 200f;

	private int lvl30BossSpecial = 6;

	private bool canReloadRockets = false;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private float minVelocity = 10f;

	private Vector3 lastFrameVelocity;

	public bool specialMove = false;

	public bool PlayerInSight;

	public bool CR_running = false;

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
				Vector3 lookPos2 = Player[currentTarget].transform.position - base.transform.position;
				lookPos2.y = 0f;
				Quaternion rotation2 = Quaternion.LookRotation(lookPos2);
				base.transform.rotation = rotation2;
			}
			else
			{
				Vector3 lookPos = startingPosition - base.transform.position;
				lookPos.y = 0f;
				Quaternion rotation = Quaternion.LookRotation(lookPos);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, Time.deltaTime * 18.5f);
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
		if (!Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var rayhit, float.PositiveInfinity, layerMask))
		{
			return;
		}
		if (rayhit.collider.tag != "Player" && rayhit.transform.tag != "Player")
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
			Vector3 startPosition = position;
			Ray ray = new Ray(position, direction);
			LayerMask layerMask = ~((1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
			if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, layerMask))
			{
				if (reflectionsRemaining == maxReflectionCount)
				{
					startingPosition = hit.point;
				}
				direction = Vector3.Reflect(direction, hit.normal);
				position = hit.point;
				if (hit.collider.tag == "Player" && hit.collider.tag != "Enemy")
				{
					return true;
				}
				if (hit.collider.tag == "Enemy" || hit.collider.gameObject.layer == LayerMask.NameToLayer("NoBounceWall"))
				{
					return false;
				}
			}
			else
			{
				position += direction * maxStepDistance;
			}
			Debug.DrawLine(startPosition, position, Color.red);
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
			float ShootInterval = ((!specialMove) ? (AIscript.ShootSpeed + Random.Range(0f, 1f)) : AIscript.ShootSpeed);
			yield return new WaitForSeconds(ShootInterval);
			if (specialMove)
			{
				Transform[] array = firePoint;
				foreach (Transform point in array)
				{
					FireTank(point, bulletPrefab);
				}
				specialMove = false;
			}
			if (PlayerInSight)
			{
				if (!Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var _, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Tank")))
				{
					Transform[] array2 = firePoint;
					foreach (Transform point2 in array2)
					{
						FireTank(point2, bulletPrefab);
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
			float ShootInterval = AIscript.ShootSpeed;
			yield return new WaitForSeconds(ShootInterval);
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
			float ShootInterval = AIscript.ShootSpeed;
			yield return new WaitForSeconds(ShootInterval);
			for (int j = 0; j < firePoint.Length; j++)
			{
				FireTank(firePoint[j], bulletPrefab);
			}
			if (AIscript.HTscript.health < AIscript.lvl30Boss3modeLives)
			{
				lvl30BossSpecial--;
			}
		}
		if (AIscript.LayMines && canMine && !AIscript.MineFleeing && !EnemiesNearMe(4))
		{
			int random = Random.Range(0, 2);
			if (random == 1)
			{
				GameObject mine = Object.Instantiate(minePrefab, base.transform.position + new Vector3(0f, -0.65f, 0f), Quaternion.identity);
				mine.transform.parent = base.transform.parent.parent;
				canMine = false;
				mineCountdown = 2f + AIscript.LayMinesSpeed;
			}
		}
		if (AIscript.hasRockets)
		{
			for (int i = 0; i < rocketSlots.Length; i++)
			{
				if (rocketSlots[i] == 0 && canReloadRockets)
				{
					rocketSlots[i] = 1;
					GameObject newRocket = Object.Instantiate(rocketPrefab, rocketSlotLocations[i]);
					rockets[i] = newRocket;
					newRocket.transform.parent = rocketSlotLocations[i];
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
		GameObject bulletGO = Object.Instantiate(bulletprefab, firepoint.position, firepoint.transform.rotation);
		EnemyBulletScript bullet = bulletGO.GetComponent<EnemyBulletScript>();
		if (bullet != null)
		{
			bullet.dir = target;
		}
		bulletGO.transform.Rotate(Vector3.right * 90f);
		Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
		bulletBody.AddForce(firepoint.forward * 6f);
	}

	public bool EnemiesNearMe(int range)
	{
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, range);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (col.tag == "Enemy" && (col.name == "Enemy_Tank-7" || col.name == "Enemy_Tank-1"))
			{
				return true;
			}
		}
		return false;
	}
}
