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

	public bool TargetInSight;

	public AudioClip ShootSound;

	public AudioClip TargetFound;

	public AudioClip ExplosionSound;

	public AudioClip RepairSound;

	public int Health = 10;

	public int maxHealth = 10;

	public int upgradeLevel;

	public GameObject deathExplosion;

	public GameObject BrokenState;

	public bool BrokenActive;

	public MoveTankScript myMTS;

	public float shootSpeed = 1.5f;

	public GameObject UpgradeParts;

	public GameObject[] SecondUpgradeParts;

	public int PlacedByPlayer;

	public float ScannerRange = 20f;

	private float timeTakenDuringLerp;

	private float _timeStartedLerping;

	public bool LockedIn;

	private bool turning;

	private bool GotRandomLookTarget;

	private bool CoolingDown;

	private void Start()
	{
		InvokeRepeating("SearchTargets", 0.1f, 0.1f);
	}

	private void Awake()
	{
		UpgradeParts.SetActive(value: false);
		GameObject[] secondUpgradeParts = SecondUpgradeParts;
		for (int i = 0; i < secondUpgradeParts.Length; i++)
		{
			secondUpgradeParts[i].SetActive(value: false);
		}
	}

	private void SearchTargets()
	{
		Targets.Clear();
		LockedIn = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
		if (Health < 4 && !BrokenActive)
		{
			ParticleSystem[] componentsInChildren = BrokenState.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Play();
			}
			BrokenActive = true;
		}
		else if (Health >= 4 && BrokenActive)
		{
			ParticleSystem[] componentsInChildren = BrokenState.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop();
			}
			BrokenActive = false;
		}
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Targets.Add(gameObject.transform);
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
			foreach (Transform target in Targets)
			{
				if (Vector3.Distance(target.transform.position, MyHead.transform.position) <= ScannerRange)
				{
					Vector3 vector = new Vector3(target.transform.position.x, 0.5f, target.transform.position.z);
					Vector3 position = MyHead.transform.position;
					position.y = 0.5f;
					Vector3 vector2 = vector - position;
					Ray ray = new Ray(position, vector2);
					Debug.DrawRay(position, vector2 * ScannerRange, Color.red, 0.1f);
					LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
					RaycastHit[] array3 = (from h in Physics.RaycastAll(ray, 50f, layerMask)
						orderby h.distance
						select h).ToArray();
					for (int j = 0; j < array3.Length; j++)
					{
						RaycastHit raycastHit = array3[j];
						if (raycastHit.collider.tag == "Turret")
						{
							Debug.Log("Hit turret!!!");
							if (raycastHit.collider.GetComponent<WeeTurret>() != this)
							{
								break;
							}
							continue;
						}
						if (raycastHit.collider.tag == "Solid" || raycastHit.collider.tag == "Player" || raycastHit.collider.tag == "Mine")
						{
							break;
						}
						if (raycastHit.collider.tag == "Enemy")
						{
							closestTarget = target;
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
			Rigidbody component = closestTarget.GetComponent<Rigidbody>();
			float num = Vector3.Distance(closestTarget.transform.position, MyHead.transform.position);
			Vector3 vector3 = ((!(component.velocity.magnitude > 0.8f)) ? closestTarget.position : (closestTarget.position + closestTarget.transform.forward * (num / 3f)));
			Vector3 forward = vector3 - MyHead.transform.position;
			startRot = MyHead.transform.rotation;
			GotRandomLookTarget = false;
			CoolingDown = false;
			forward.y = 0f;
			rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(-90f, 0f, 0f);
			float num2 = Quaternion.Angle(MyHead.transform.rotation, rotation);
			timeTakenDuringLerp = num2 / 180f * 0.5f;
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
		float t = (Time.time - _timeStartedLerping) / timeTakenDuringLerp;
		MyHead.transform.rotation = Quaternion.Lerp(startRot, rotation, t);
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
				float num = Quaternion.Angle(MyHead.transform.rotation, rotation);
				timeTakenDuringLerp = num / 180f * 3f;
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
		GameObject gameObject = ((upgradeLevel <= 0) ? Object.Instantiate(BulletPrefab, ShootPoint.position, ShootPoint.transform.rotation) : Object.Instantiate(UpgradedBulletPrefab, ShootPoint.position, ShootPoint.transform.rotation));
		gameObject.GetComponent<Rigidbody>().AddForce(ShootPoint.forward * 6f);
		PlayerBulletScript component = gameObject.GetComponent<PlayerBulletScript>();
		component.StartingVelocity = ShootPoint.forward * 6f;
		if ((bool)component)
		{
			component.TurretBullet = true;
			component.ShotByPlayer = PlacedByPlayer;
		}
	}

	private void DIE()
	{
		SFXManager.instance.PlaySFX(ExplosionSound, 1f, null);
		GameObject obj = Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity);
		obj.GetComponent<ParticleSystem>().Play();
		if ((bool)myMTS)
		{
			myMTS.Upgrades[5] = 0;
		}
		GameMaster.instance.TurretsPlaced[PlacedByPlayer]--;
		Object.Destroy(obj.gameObject, 3f);
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
			for (int i = 0; i < secondUpgradeParts.Length; i++)
			{
				secondUpgradeParts[i].SetActive(value: true);
			}
		}
		Health = 5 + upgradeLevel * 5;
		maxHealth = Health;
	}
}
