using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeeTurret : MonoBehaviour
{
	public List<Transform> Targets = new List<Transform>();

	public Transform Head;

	public Transform closestTarget;

	public Transform MyHead;

	private Transform LastTarget;

	private Quaternion startRot;

	private Quaternion rotation;

	public GameObject BulletPrefab;

	public GameObject UpgradedBulletPrefab;

	public float shootCooldown = 1f;

	public Transform ShootPoint;

	public bool TargetInSight = false;

	public AudioClip ShootSound;

	public AudioClip TargetFound;

	public AudioClip ExplosionSound;

	public AudioClip RepairSound;

	public int Health = 10;

	public int maxHealth = 10;

	public int upgradeLevel = 0;

	public GameObject deathExplosion;

	public GameObject BrokenState;

	public bool BrokenActive = false;

	public MoveTankScript myMTS;

	public float shootSpeed = 1.5f;

	public GameObject UpgradeParts;

	public GameObject[] SecondUpgradeParts;

	public int PlacedByPlayer = 0;

	public float ScannerRange = 20f;

	private float timeTakenDuringLerp;

	private float _timeStartedLerping;

	public bool LockedIn = false;

	private bool turning = false;

	private bool GotRandomLookTarget = false;

	private bool CoolingDown = false;

	private void Start()
	{
		InvokeRepeating("SearchTargets", 0.1f, 0.1f);
	}

	private void Awake()
	{
		UpgradeParts.SetActive(value: false);
		GameObject[] secondUpgradeParts = SecondUpgradeParts;
		foreach (GameObject part in secondUpgradeParts)
		{
			part.SetActive(value: false);
		}
	}

	private void SearchTargets()
	{
		Targets.Clear();
		LockedIn = false;
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		if (Health < 4 && !BrokenActive)
		{
			ParticleSystem[] componentsInChildren = BrokenState.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem PS in componentsInChildren)
			{
				PS.Play();
			}
			BrokenActive = true;
		}
		else if (Health >= 4 && BrokenActive)
		{
			ParticleSystem[] componentsInChildren2 = BrokenState.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem PS2 in componentsInChildren2)
			{
				PS2.Stop();
			}
			BrokenActive = false;
		}
		GameObject[] array = enemies;
		foreach (GameObject enemy in array)
		{
			Targets.Add(enemy.transform);
		}
		if ((bool)Head)
		{
			if (Targets.Count <= 0)
			{
				TargetInSight = false;
				LockedIn = false;
				turning = false;
				return;
			}
			closestTarget = null;
			foreach (Transform potentialTarget in Targets)
			{
				float dist = Vector3.Distance(potentialTarget.transform.position, MyHead.transform.position);
				if (dist <= ScannerRange)
				{
					Vector3 TargetPos = new Vector3(potentialTarget.transform.position.x, 0.5f, potentialTarget.transform.position.z);
					Vector3 HeadPos = MyHead.transform.position;
					HeadPos.y = 0.5f;
					Vector3 targetDirection2 = TargetPos - HeadPos;
					Ray ray = new Ray(HeadPos, targetDirection2);
					Debug.DrawRay(HeadPos, targetDirection2 * ScannerRange, Color.red, 0.1f);
					LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
					RaycastHit[] allhits = (from h in Physics.RaycastAll(ray, 50f, layerMask)
						orderby h.distance
						select h).ToArray();
					for (int i = 0; i < allhits.Length; i++)
					{
						RaycastHit hit = allhits[i];
						if (hit.collider.tag == "Turret")
						{
							Debug.Log("Hit turret!!!");
							WeeTurret WT = hit.collider.GetComponent<WeeTurret>();
							if (WT != this)
							{
								break;
							}
							continue;
						}
						if (hit.collider.tag == "Solid" || hit.collider.tag == "Player" || hit.collider.tag == "Mine")
						{
							break;
						}
						if (hit.collider.tag == "Enemy")
						{
							closestTarget = potentialTarget;
							break;
						}
					}
				}
				if (closestTarget != null)
				{
					break;
				}
			}
		}
		if (closestTarget != null && !turning)
		{
			if (!LastTarget)
			{
				SFXManager.instance.PlaySFX(TargetFound, 1f, null);
			}
			else if (LastTarget != closestTarget)
			{
				SFXManager.instance.PlaySFX(TargetFound, 1f, null);
			}
			LastTarget = closestTarget;
			TargetInSight = true;
			Rigidbody targetRB = closestTarget.GetComponent<Rigidbody>();
			float distance = Vector3.Distance(closestTarget.transform.position, MyHead.transform.position);
			Vector3 infrontTarget = ((!(targetRB.velocity.magnitude > 0.8f)) ? closestTarget.position : (closestTarget.position + closestTarget.transform.forward * (distance / 3f)));
			Vector3 targetDirection = infrontTarget - MyHead.transform.position;
			startRot = MyHead.transform.rotation;
			GotRandomLookTarget = false;
			CoolingDown = false;
			targetDirection.y = 0f;
			rotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(-90f, 0f, 0f);
			float angleSize = Quaternion.Angle(MyHead.transform.rotation, rotation);
			timeTakenDuringLerp = angleSize / 180f * 0.5f;
			if (timeTakenDuringLerp < 0.2f)
			{
				timeTakenDuringLerp = 0.2f;
			}
			_timeStartedLerping = Time.time;
			turning = true;
		}
	}

	private void RotateTo(bool ToShoot)
	{
		if (CoolingDown)
		{
			return;
		}
		float timeSinceStarted = Time.time - _timeStartedLerping;
		float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
		MyHead.transform.rotation = Quaternion.Lerp(startRot, rotation, percentageComplete);
		if (Quaternion.Angle(MyHead.transform.rotation, rotation) <= 0.6f)
		{
			if (ToShoot)
			{
				LockedIn = true;
				turning = false;
			}
			else
			{
				CoolingDown = true;
				StartCoroutine(CooldownRot());
			}
			MyHead.transform.rotation = rotation;
		}
	}

	private IEnumerator CooldownRot()
	{
		yield return new WaitForSeconds(Random.Range(1, 3));
		GotRandomLookTarget = false;
		CoolingDown = false;
	}

	private void Update()
	{
		shootCooldown -= Time.deltaTime;
		if (Health < 1)
		{
			DIE();
		}
		if (!LockedIn && (bool)closestTarget && TargetInSight)
		{
			CoolingDown = false;
			RotateTo(ToShoot: true);
		}
		else if (!TargetInSight)
		{
			if (!GotRandomLookTarget)
			{
				startRot = MyHead.transform.rotation;
				rotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0f);
				float angleSize = Quaternion.Angle(MyHead.transform.rotation, rotation);
				timeTakenDuringLerp = angleSize / 180f * 3f;
				if (timeTakenDuringLerp < 0.2f)
				{
					timeTakenDuringLerp = 0.2f;
				}
				_timeStartedLerping = Time.time;
				GotRandomLookTarget = true;
			}
			else
			{
				RotateTo(ToShoot: false);
			}
		}
		if (LockedIn && shootCooldown < 0f)
		{
			Shoot();
			TargetInSight = false;
			LockedIn = false;
			shootCooldown = shootSpeed + Random.Range(0f, 0.5f);
		}
	}

	private void Shoot()
	{
		SFXManager.instance.PlaySFX(ShootSound, 1f, null);
		GameObject bulletGO = ((upgradeLevel <= 0) ? Object.Instantiate(BulletPrefab, ShootPoint.position, ShootPoint.transform.rotation) : Object.Instantiate(UpgradedBulletPrefab, ShootPoint.position, ShootPoint.transform.rotation));
		Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
		bulletBody.AddForce(ShootPoint.forward * 6f);
		PlayerBulletScript PBS = bulletGO.GetComponent<PlayerBulletScript>();
		PBS.StartingVelocity = ShootPoint.forward * 6f;
		if ((bool)PBS)
		{
			PBS.TurretBullet = true;
			PBS.ShotByPlayer = PlacedByPlayer;
		}
	}

	private void DIE()
	{
		SFXManager.instance.PlaySFX(ExplosionSound, 1f, null);
		GameObject poof = Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity);
		poof.GetComponent<ParticleSystem>().Play();
		if ((bool)myMTS)
		{
			myMTS.Upgrades[5] = 0;
		}
		GameMaster.instance.TurretsPlaced[PlacedByPlayer]--;
		Object.Destroy(poof.gameObject, 3f);
		Object.Destroy(base.gameObject);
	}

	public void Repair()
	{
		Health = maxHealth;
		SFXManager.instance.PlaySFX(RepairSound, 1f, null);
	}

	public void Upgrade()
	{
		upgradeLevel++;
		if (upgradeLevel == 1)
		{
			shootSpeed = 1f;
			ScannerRange = 25f;
			UpgradeParts.SetActive(value: true);
		}
		else if (upgradeLevel == 2)
		{
			shootSpeed = 0.5f;
			ScannerRange = 30f;
			GameObject[] secondUpgradeParts = SecondUpgradeParts;
			foreach (GameObject part in secondUpgradeParts)
			{
				part.SetActive(value: true);
			}
		}
		Health = 5 + upgradeLevel * 5;
		maxHealth = Health;
	}
}
