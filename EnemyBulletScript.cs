using System.Collections;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
	public float BulletSpeed = 5f;

	public Vector3 dir;

	public int BounceAmount;

	public int maxBounces;

	public Quaternion _lookRotation;

	public Vector3 _direction;

	public Quaternion _rot;

	public Vector3 startVelocity;

	public AudioClip[] WallHit;

	public AudioClip[] WallHitElectro;

	public AudioClip[] DeadHit;

	public AudioClip[] DeadHitElectric;

	public GameObject poofParticles;

	public GameObject poofParticlesElectro;

	public GameObject bounceParticles;

	public bool canHurt;

	public bool onlyPlayer;

	public bool isExplosive;

	public bool isElectric;

	public GameObject deathExplosion;

	private Vector3 lastAngularVelocity;

	private Vector3 lastPosition;

	public GameObject papaTank;

	public EnemyTargetingSystemNew papaScript;

	public GameObject PreviousCollidedWith;

	[HideInInspector]
	public AudioSource MySource;

	public Vector3 newVelocity;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private float minVelocity = 10f;

	private Vector3 lastFrameVelocity;

	private Rigidbody rb;

	public bool nearWall;

	private LaserScript myLaser;

	private bool hittingWall;

	public bool isBossBullet;

	[Header("Electric Stuff")]
	public AudioClip ElectricCharge;

	public bool IsElectricCharged;

	public ParticleSystem SmokeBullet;

	public GameObject ElectricState;

	private void Awake()
	{
		if (_direction != Vector3.zero)
		{
			_lookRotation = Quaternion.LookRotation(_direction);
		}
		base.transform.rotation = _lookRotation;
		base.transform.Rotate(-90f, 0f, 0f);
	}

	private void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		rb.velocity = initialVelocity;
		canHurt = true;
	}

	private IEnumerator Hurt()
	{
		canHurt = true;
		return null;
	}

	public void ChargeElectric()
	{
		if (!IsElectricCharged && !isElectric)
		{
			SmokeBullet.Stop();
			ElectricState.SetActive(value: true);
			IsElectricCharged = true;
			BulletSpeed = 14f;
			if (base.name == "EnemyRocket2Bounce" || base.name == "EnemyRocket2Bounce(Clone)")
			{
				BulletSpeed = 18f;
			}
			Play2DClipAtPoint(ElectricCharge);
		}
	}

	private void Start()
	{
		myLaser = GetComponent<LaserScript>();
		MySource = GetComponent<AudioSource>();
		startVelocity = rb.velocity;
		_direction = rb.velocity;
		if (_direction != Vector3.zero)
		{
			_lookRotation = Quaternion.LookRotation(_direction);
		}
		base.transform.rotation = _lookRotation;
		base.transform.Rotate(-90f, 0f, 0f);
		hittingWall = true;
		if (isBossBullet)
		{
			if (OptionsMainMenu.instance.currentDifficulty == 1)
			{
				BulletSpeed *= 1.1f;
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 2)
			{
				BulletSpeed *= 1.2f;
			}
		}
		StartCoroutine("deHit");
		InvokeRepeating("CheckIfMoving", 0.5f, 0.1f);
		StartCoroutine(AutoDestroy());
		if (isExplosive)
		{
			InvokeRepeating("AlertDetectionFields", 0.3f, 0.3f);
		}
	}

	private IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(15f);
		BounceAmount = 9999;
	}

	private void CheckIfMoving()
	{
		if (rb.velocity.magnitude < 0.1f)
		{
			Debug.LogError("Bullet not moving, time to kill maself");
			BounceAmount = 999;
		}
	}

	private void AlertDetectionFields()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 1.5f);
		foreach (Collider collider in array)
		{
			if (!(collider.tag == "EnemyDetectionField"))
			{
				continue;
			}
			EnemyDetection component = collider.gameObject.GetComponent<EnemyDetection>();
			if ((bool)component)
			{
				Collider component2 = GetComponent<Collider>();
				if (!component.Bullets.Contains(component2))
				{
					component.Bullets.Add(component2);
				}
			}
		}
	}

	private void Update()
	{
		lastFrameVelocity = rb.velocity;
		rb.velocity = rb.velocity.normalized * BulletSpeed;
		lastAngularVelocity = rb.angularVelocity;
		lastPosition = base.transform.position;
		base.transform.LookAt(myLaser.lookAtPoint);
		base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
		if (!(GameMaster.instance != null))
		{
			return;
		}
		if (GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted)
		{
			BulletDeath(isEndingGame: true);
		}
		if (MapEditorMaster.instance != null && MapEditorMaster.instance.inPlayingMode && !GameMaster.instance.GameHasStarted)
		{
			BulletDeath(isEndingGame: true);
		}
		if (GameMaster.instance.AmountEnemyTanks < 1 || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive || (bool)GameMaster.instance.CM)
		{
			if (BounceAmount >= maxBounces && (!GameMaster.instance.GameHasStarted || (bool)GameMaster.instance.CM))
			{
				BulletDeath(isEndingGame: true);
			}
			else if (!GameMaster.instance.isZombieMode && !GameMaster.instance.GameHasStarted && !GameMaster.instance.inMenuMode)
			{
				BulletDeath(isEndingGame: true);
			}
			if (papaScript != null && !GameMaster.instance.CM && !papaScript.isHuntingEnemies)
			{
				BulletDeath(isEndingGame: true);
			}
		}
		else if (canHurt && BounceAmount >= maxBounces)
		{
			BulletDeath(isEndingGame: false);
		}
	}

	public void BulletDeath(bool isEndingGame)
	{
		if (papaScript != null)
		{
			papaScript.firedBullets--;
		}
		if (isExplosive)
		{
			if (!isEndingGame)
			{
				AreaDamageEnemies(base.transform.position, 3f, 1f);
				CameraShake component = Camera.main.GetComponent<CameraShake>();
				if ((bool)component)
				{
					component.StartCoroutine(component.Shake(0.1f, 0.1f));
				}
				DeadSound();
			}
			Object.Destroy(Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity).gameObject, 2f);
			Transform transform = base.transform.Find("SmokeBullet");
			if ((bool)transform)
			{
				transform.GetComponent<ParticleSystem>().Stop();
				transform.parent = null;
			}
			if (transform != null)
			{
				Object.Destroy(transform.gameObject, 5f);
			}
			Object.Destroy(base.gameObject);
			return;
		}
		GameObject gameObject = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
		gameObject.GetComponent<ParticleSystem>().Play();
		Object.Destroy(gameObject.gameObject, 3f);
		if (!isEndingGame || (bool)GameMaster.instance.CM)
		{
			DeadSound();
		}
		Transform transform2 = base.transform.Find("SmokeBullet");
		if ((bool)transform2)
		{
			transform2.GetComponent<ParticleSystem>().Stop();
			transform2.parent = null;
		}
		Transform transform3 = base.transform.Find("Trail");
		if ((bool)transform3)
		{
			transform3.GetComponent<ParticleSystem>().Stop();
			transform3.parent = null;
			transform3.localScale = new Vector3(1f, 1f, 1f);
		}
		if (transform2 != null)
		{
			Object.Destroy(transform2.gameObject, 5f);
		}
		Object.Destroy(base.gameObject);
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] array = Physics.OverlapSphere(location, radius);
		foreach (Collider obj in array)
		{
			HealthTanks component = obj.GetComponent<HealthTanks>();
			LookAtMyDirection component2 = obj.GetComponent<LookAtMyDirection>();
			EnemyBulletScript component3 = obj.GetComponent<EnemyBulletScript>();
			MineScript component4 = obj.GetComponent<MineScript>();
			DestroyableWall component5 = obj.GetComponent<DestroyableWall>();
			WeeTurret component6 = obj.GetComponent<WeeTurret>();
			if (component6 != null)
			{
				component6.Health--;
			}
			if (component != null && !component.immuneToExplosion)
			{
				component.DamageMe(1);
			}
			if (component2 != null)
			{
				component2.BounceAmount = 999;
			}
			if (component3 != null && !component3.isElectric)
			{
				component3.BounceAmount = 999;
			}
			if (component4 != null)
			{
				component4.DetinationTime = 0f;
			}
			if (component5 != null)
			{
				component5.StartCoroutine(component5.destroy());
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnCollision(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
	}

	private void OnCollision(Collision collision)
	{
		if (collision.gameObject.tag == "Bullet" && !isExplosive)
		{
			if (CheckBulletShotgun(collision))
			{
				return;
			}
		}
		else
		{
			if (myLaser.WallGonnaBounceInTo == null && collision.gameObject.tag != "Player")
			{
				if (BounceAmount != 0 || !(collision.gameObject == papaTank.gameObject))
				{
					Debug.LogWarning("No target error enter");
					BulletDeath(isEndingGame: true);
				}
				return;
			}
			if (collision.gameObject != myLaser.WallGonnaBounceInTo && !nearWall && collision.gameObject.tag == "Solid")
			{
				IgnoreThatCollision(collision);
				return;
			}
			if (myLaser.WallGonnaBounceInTo == base.transform.gameObject)
			{
				Debug.LogWarning("PRevented weird bounce glitch!");
				IgnoreThatCollision(collision);
				return;
			}
		}
		if (!hittingWall && canHurt)
		{
			WallImpact(collision);
		}
	}

	private IEnumerator deHit()
	{
		yield return new WaitForSeconds(0.001f);
		hittingWall = false;
	}

	private void IgnoreThatCollision(Collision collision)
	{
		rb.velocity = lastFrameVelocity;
		rb.angularVelocity = lastAngularVelocity;
		base.transform.position = lastPosition;
		Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
	}

	private bool CheckBulletShotgun(Collision collision)
	{
		EnemyBulletScript component = collision.gameObject.GetComponent<EnemyBulletScript>();
		LookAtMyDirection component2 = collision.gameObject.GetComponent<LookAtMyDirection>();
		if (component != null)
		{
			if (component.papaTank == papaTank && component.BounceAmount == 0 && BounceAmount == 0)
			{
				IgnoreThatCollision(collision);
				return true;
			}
			if (component.isElectric || isElectric)
			{
				IgnoreThatCollision(collision);
				return true;
			}
		}
		else
		{
			if (component2 != null && isElectric)
			{
				if (papaScript.AIscript.isLevel70Boss)
				{
					component2.ChargeElectric();
				}
				IgnoreThatCollision(collision);
				return true;
			}
			if (isElectric)
			{
				IgnoreThatCollision(collision);
				return true;
			}
		}
		return false;
	}

	public void WallImpact(Collision collision)
	{
		hittingWall = true;
		StartCoroutine("deHit");
		if (collision.gameObject == PreviousCollidedWith)
		{
			return;
		}
		PreviousCollidedWith = null;
		if (collision.gameObject.tag == "MapBorder" && GameMaster.instance.isZombieMode)
		{
			BounceAmount = 99;
		}
		else
		{
			if (!canHurt)
			{
				return;
			}
			if ((collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder") && !onlyPlayer)
			{
				if (collision.gameObject.layer == 16)
				{
					BounceAmount = 99;
				}
				PreviousCollidedWith = collision.gameObject;
				if (nearWall)
				{
					Bounce(new Vector3(0f, 0f, 0f), unexpectedBounce: false);
				}
				else
				{
					Bounce(collision.contacts[0].normal, unexpectedBounce: true);
				}
				BounceAmount++;
				if (BounceAmount < maxBounces)
				{
					HitWallSound();
				}
			}
			else if (collision.gameObject.tag == "Bullet" && !isExplosive)
			{
				if (!CheckBulletShotgun(collision))
				{
					BounceAmount = 999;
				}
			}
			else if (collision.gameObject.tag == "Turret")
			{
				WeeTurret component = collision.gameObject.GetComponent<WeeTurret>();
				if (component != null)
				{
					component.Health--;
					BounceAmount = 999;
				}
			}
			else
			{
				if (((collision.gameObject.layer != LayerMask.NameToLayer("Tank") || onlyPlayer) && collision.gameObject.layer != LayerMask.NameToLayer("PlayerTank")) || (collision.gameObject == papaTank && BounceAmount == 0))
				{
					return;
				}
				if (isElectric)
				{
					MoveTankScript component2 = collision.gameObject.GetComponent<MoveTankScript>();
					if (component2 != null)
					{
						if (GameMaster.instance.CurrentMission == 69)
						{
							HealthTanks component3 = collision.gameObject.GetComponent<HealthTanks>();
							if ((bool)component3)
							{
								component3.health--;
							}
						}
						else
						{
							component2.StunMe(3f);
						}
					}
					EnemyAI component4 = collision.gameObject.GetComponent<EnemyAI>();
					if (component4 != null && !component4.isElectric)
					{
						if (GameMaster.instance.CurrentMission == 69)
						{
							HealthTanks component5 = collision.gameObject.GetComponent<HealthTanks>();
							if ((bool)component5)
							{
								component5.health--;
							}
						}
						else
						{
							component2.StunMe(3f);
						}
					}
				}
				else
				{
					EnemyAI component6 = collision.gameObject.GetComponent<EnemyAI>();
					if (component6 != null && component6.isTransporting)
					{
						return;
					}
					HealthTanks component7 = collision.gameObject.GetComponent<HealthTanks>();
					if (GameMaster.instance.isZombieMode && component7.health > 1)
					{
						SFXManager.instance.PlaySFX(component7.Buzz);
					}
					component7.health--;
				}
				BounceAmount = 999;
			}
		}
	}

	public void Bounce(Vector3 collisionNormal, bool unexpectedBounce)
	{
		GameObject obj = Object.Instantiate(bounceParticles, base.transform.position, Quaternion.identity);
		obj.GetComponent<ParticleSystem>().Play();
		Object.Destroy(obj.gameObject, 3f);
		_ = lastFrameVelocity.magnitude;
		Vector3 velocity;
		if (!unexpectedBounce)
		{
			velocity = newVelocity;
			myLaser.WallGonnaBounceInTo = base.transform.gameObject;
		}
		else
		{
			velocity = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
		}
		rb.velocity = velocity;
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		nearWall = false;
	}

	private void HitWallSound()
	{
		if (IsElectricCharged)
		{
			int maxExclusive = WallHitElectro.Length;
			int num = Random.Range(0, maxExclusive);
			Play2DClipAtPoint(WallHitElectro[num]);
		}
		else
		{
			int maxExclusive2 = WallHit.Length;
			int num2 = Random.Range(0, maxExclusive2);
			Play2DClipAtPoint(WallHit[num2]);
		}
	}

	private void DeadSound()
	{
		if (IsElectricCharged)
		{
			int maxExclusive = DeadHitElectric.Length;
			int num = Random.Range(0, maxExclusive);
			Play2DClipAtPoint(DeadHitElectric[num]);
		}
		else
		{
			int maxExclusive2 = DeadHit.Length;
			int num2 = Random.Range(0, maxExclusive2);
			Play2DClipAtPoint(DeadHit[num2]);
		}
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
