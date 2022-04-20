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

	public bool canHurt = false;

	public bool onlyPlayer = false;

	public bool isExplosive = false;

	public bool isElectric = false;

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

	public bool nearWall = false;

	private LaserScript myLaser;

	private bool hittingWall = false;

	public bool isBossBullet = false;

	[Header("Electric Stuff")]
	public AudioClip ElectricCharge;

	public bool IsElectricCharged = false;

	public ParticleSystem SmokeBullet;

	public GameObject ElectricState;

	[HideInInspector]
	public bool IsSideBullet = false;

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
		Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 1.5f);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (!(col.tag == "EnemyDetectionField"))
			{
				continue;
			}
			EnemyDetection ED = col.gameObject.GetComponent<EnemyDetection>();
			if ((bool)ED)
			{
				Collider myCollider = GetComponent<Collider>();
				if (!ED.Bullets.Contains(myCollider))
				{
					ED.Bullets.Add(myCollider);
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
		if (papaScript != null && !IsSideBullet)
		{
			papaScript.firedBullets--;
		}
		if (isExplosive)
		{
			if (!isEndingGame)
			{
				AreaDamageEnemies(base.transform.position, 3f, 1f);
				CameraShake CS = Camera.main.GetComponent<CameraShake>();
				if ((bool)CS)
				{
					CS.StartCoroutine(CS.Shake(0.1f, 0.1f));
				}
				DeadSound();
			}
			GameObject poof = Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity);
			Object.Destroy(poof.gameObject, 2f);
			Transform PE2 = base.transform.Find("SmokeBullet");
			if ((bool)PE2)
			{
				ParticleSystem PEsystem3 = PE2.GetComponent<ParticleSystem>();
				PEsystem3.Stop();
				PE2.parent = null;
			}
			if (PE2 != null)
			{
				Object.Destroy(PE2.gameObject, 5f);
			}
			Object.Destroy(base.gameObject);
			return;
		}
		GameObject poof2 = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
		poof2.GetComponent<ParticleSystem>().Play();
		Object.Destroy(poof2.gameObject, 3f);
		if (!isEndingGame || (bool)GameMaster.instance.CM)
		{
			DeadSound();
		}
		Transform PE = base.transform.Find("SmokeBullet");
		if ((bool)PE)
		{
			ParticleSystem PEsystem2 = PE.GetComponent<ParticleSystem>();
			PEsystem2.Stop();
			PE.parent = null;
		}
		Transform Trail = base.transform.Find("Trail");
		if ((bool)Trail)
		{
			ParticleSystem PEsystem = Trail.GetComponent<ParticleSystem>();
			PEsystem.Stop();
			Trail.parent = null;
			Trail.localScale = new Vector3(1f, 1f, 1f);
		}
		if (PE != null)
		{
			Object.Destroy(PE.gameObject, 5f);
		}
		Object.Destroy(base.gameObject);
	}

	private void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			HealthTanks enemy = col.GetComponent<HealthTanks>();
			LookAtMyDirection friendbullet = col.GetComponent<LookAtMyDirection>();
			EnemyBulletScript enemybullet = col.GetComponent<EnemyBulletScript>();
			MineScript mines = col.GetComponent<MineScript>();
			DestroyableWall DestroyWall = col.GetComponent<DestroyableWall>();
			WeeTurret WT = col.GetComponent<WeeTurret>();
			if (WT != null)
			{
				WT.Health--;
			}
			if (enemy != null && !enemy.immuneToExplosion)
			{
				enemy.DamageMe(1);
			}
			if (friendbullet != null)
			{
				friendbullet.BounceAmount = 999;
			}
			if (enemybullet != null && !enemybullet.isElectric)
			{
				enemybullet.BounceAmount = 999;
			}
			if (mines != null)
			{
				mines.DetinationTime = 0f;
			}
			if (DestroyWall != null)
			{
				DestroyWall.StartCoroutine(DestroyWall.destroy());
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
		EnemyBulletScript EBS = collision.gameObject.GetComponent<EnemyBulletScript>();
		LookAtMyDirection LAMD = collision.gameObject.GetComponent<LookAtMyDirection>();
		if (EBS != null)
		{
			if (EBS.papaTank == papaTank && EBS.BounceAmount == 0 && BounceAmount == 0)
			{
				IgnoreThatCollision(collision);
				return true;
			}
			if (EBS.isElectric || isElectric)
			{
				IgnoreThatCollision(collision);
				return true;
			}
		}
		else
		{
			if (LAMD != null && isElectric)
			{
				if (papaScript.AIscript.isLevel70Boss)
				{
					LAMD.ChargeElectric();
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
				WeeTurret WT = collision.gameObject.GetComponent<WeeTurret>();
				if (WT != null)
				{
					WT.Health--;
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
					MoveTankScript movescript = collision.gameObject.GetComponent<MoveTankScript>();
					if (movescript != null)
					{
						if (GameMaster.instance.CurrentMission == 69)
						{
							HealthTanks HT = collision.gameObject.GetComponent<HealthTanks>();
							if ((bool)HT)
							{
								HT.health--;
							}
						}
						else
						{
							movescript.StunMe(3f);
						}
					}
					EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
					if (enemy != null && !enemy.isElectric)
					{
						if (GameMaster.instance.CurrentMission == 69)
						{
							HealthTanks HT2 = collision.gameObject.GetComponent<HealthTanks>();
							if ((bool)HT2)
							{
								HT2.health--;
							}
						}
						else
						{
							movescript.StunMe(3f);
						}
					}
				}
				else
				{
					EnemyAI AIscript = collision.gameObject.GetComponent<EnemyAI>();
					if (AIscript != null && AIscript.isTransporting)
					{
						return;
					}
					HealthTanks healthscript = collision.gameObject.GetComponent<HealthTanks>();
					if (GameMaster.instance.isZombieMode && healthscript.health > 1)
					{
						SFXManager.instance.PlaySFX(healthscript.Buzz);
					}
					healthscript.health--;
				}
				BounceAmount = 999;
			}
		}
	}

	public void Bounce(Vector3 collisionNormal, bool unexpectedBounce)
	{
		GameObject poof = Object.Instantiate(bounceParticles, base.transform.position, Quaternion.identity);
		poof.GetComponent<ParticleSystem>().Play();
		Object.Destroy(poof.gameObject, 3f);
		float speed = lastFrameVelocity.magnitude;
		Vector3 direction;
		if (!unexpectedBounce)
		{
			direction = newVelocity;
			myLaser.WallGonnaBounceInTo = base.transform.gameObject;
		}
		else
		{
			direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
		}
		rb.velocity = direction;
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		nearWall = false;
	}

	private void HitWallSound()
	{
		if (IsElectricCharged)
		{
			int lengthClips2 = WallHitElectro.Length;
			int randomPick2 = Random.Range(0, lengthClips2);
			Play2DClipAtPoint(WallHitElectro[randomPick2]);
		}
		else
		{
			int lengthClips = WallHit.Length;
			int randomPick = Random.Range(0, lengthClips);
			Play2DClipAtPoint(WallHit[randomPick]);
		}
	}

	private void DeadSound()
	{
		if (IsElectricCharged)
		{
			int lengthClips2 = DeadHitElectric.Length;
			int randomPick2 = Random.Range(0, lengthClips2);
			Play2DClipAtPoint(DeadHitElectric[randomPick2]);
		}
		else
		{
			int lengthClips = DeadHit.Length;
			int randomPick = Random.Range(0, lengthClips);
			Play2DClipAtPoint(DeadHit[randomPick]);
		}
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 2f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
