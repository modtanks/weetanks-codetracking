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

	public int PlacedByPlayer;

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
			float num = float.PositiveInfinity;
			foreach (Transform target in Targets)
			{
				float num2 = Vector3.Distance(target.transform.position, MyHead.transform.position);
				if (num2 < num && num2 <= 20f)
				{
					num = num2;
					closestTarget = target;
				}
			}
		}
		if (!(closestTarget != null) || turning)
		{
			return;
		}
		Vector3 direction = closestTarget.transform.position - MyHead.transform.position;
		direction.y = 1f;
		Ray ray = new Ray(MyHead.transform.position, direction);
		LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
		RaycastHit[] array3 = (from h in Physics.RaycastAll(ray, 100f, layerMask)
			orderby h.distance
			select h).ToArray();
		for (int j = 0; j < array3.Length; j++)
		{
			RaycastHit raycastHit = array3[j];
			if (raycastHit.collider.tag == "Turret")
			{
				if (raycastHit.collider.GetComponent<WeeTurret>() != this)
				{
					return;
				}
				continue;
			}
			if (raycastHit.collider.tag == "Solid" || raycastHit.collider.tag == "Player" || raycastHit.collider.tag == "Mine")
			{
				TargetInSight = false;
				return;
			}
			_ = raycastHit.collider.tag == "Enemy";
		}
		if (!LastTarget)
		{
			GameMaster.instance.Play2DClipAtPoint(TargetFound, 1f);
		}
		else if (LastTarget != closestTarget)
		{
			GameMaster.instance.Play2DClipAtPoint(TargetFound, 1f);
		}
		LastTarget = closestTarget;
		TargetInSight = true;
		Rigidbody component = closestTarget.GetComponent<Rigidbody>();
		float num3 = Vector3.Distance(closestTarget.transform.position, MyHead.transform.position);
		Vector3 vector = ((!(component.velocity.magnitude > 0.8f)) ? closestTarget.position : (closestTarget.position + closestTarget.transform.forward * (num3 / 3f)));
		direction = vector - MyHead.transform.position;
		startRot = MyHead.transform.rotation;
		GotRandomLookTarget = false;
		CoolingDown = false;
		direction.y = 0f;
		rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90f, 90f, 0f);
		float num4 = Quaternion.Angle(MyHead.transform.rotation, rotation);
		timeTakenDuringLerp = num4 / 180f * 0.5f;
		if (timeTakenDuringLerp < 0.2f)
		{
			timeTakenDuringLerp = 0.2f;
		}
		_timeStartedLerping = Time.time;
		turning = true;
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
		GameMaster.instance.Play2DClipAtPoint(ShootSound, 1f);
		GameObject gameObject = ((upgradeLevel <= 0) ? Object.Instantiate(BulletPrefab, ShootPoint.position, ShootPoint.transform.rotation) : Object.Instantiate(UpgradedBulletPrefab, ShootPoint.position, ShootPoint.transform.rotation));
		gameObject.GetComponent<Rigidbody>().AddForce(ShootPoint.forward * 6f);
		PlayerBulletScript component = gameObject.GetComponent<PlayerBulletScript>();
		if ((bool)component)
		{
			component.TurretBullet = true;
			component.ShotByPlayer = PlacedByPlayer;
		}
	}

	private void DIE()
	{
		GameMaster.instance.Play2DClipAtPoint(ExplosionSound, 1f);
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
		GameMaster.instance.Play2DClipAtPoint(RepairSound, 1f);
	}

	public void Upgrade()
	{
		upgradeLevel++;
		Health = 10 + upgradeLevel * 5;
		maxHealth = Health;
		shootSpeed = 1f;
		UpgradeParts.SetActive(value: true);
	}
}
