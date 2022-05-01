using System.Collections;
using UnityEngine;

public class LookAtMyDirection : MonoBehaviour
{
	public float BulletSpeed = 5f;

	public int BounceAmount;

	public int maxBounces;

	public AudioClip[] WallHit;

	public AudioClip[] WallHitElectro;

	public AudioClip[] DeadHit;

	public AudioClip[] DeadHitElectric;

	public AudioClip ElectricCharge;

	public ParticleSystem SmokeBullet;

	public ParticleSystem SmokeBulletParty;

	public GameObject poofParticles;

	public GameObject poofParticlesElectro;

	public GameObject bounceParticles;

	public GameObject HitTextP1;

	public GameObject HitTextP2;

	public AudioClip enemyKill;

	public AudioClip BuzzHit;

	[HideInInspector]
	public bool ShotByPlayer2 = false;

	public FiringTank TankScript;

	public EnemyTargetingSystemNew TankScriptAI;

	public AudioSource MySource;

	public bool canHurt = false;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private float minVelocity = 10f;

	private Vector3 lastFrameVelocity;

	private Rigidbody rb;

	private Vector3 lastAngularVelocity;

	private Vector3 lastPosition;

	public bool IsElectricCharged = false;

	public GameObject ElectricState;

	public bool nearWall = false;

	private LaserScript myLaser;

	public Vector3 newVelocity;

	private GameObject PreviousCollidedWith;

	private bool hittingWall = false;

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

	private void Start()
	{
		maxBounces = 1;
		myLaser = GetComponent<LaserScript>();
		MySource = GetComponent<AudioSource>();
		base.transform.Rotate(-90f, 0f, 0f);
		InvokeRepeating("CheckIfMoving", 0.5f, 0.1f);
		StartCoroutine(AutoDestroy());
		if ((bool)OptionsMainMenu.instance && OptionsMainMenu.instance.AMselected.Contains(6))
		{
			int pick = Random.Range(0, 5);
			Color newcc = Color.red;
			switch (pick)
			{
			case 0:
				newcc = Color.red;
				break;
			case 1:
				newcc = Color.blue;
				break;
			case 2:
				newcc = Color.green;
				break;
			case 3:
				newcc = Color.cyan;
				break;
			case 4:
				newcc = Color.yellow;
				break;
			}
			GetComponent<Renderer>().material.color = newcc;
			SmokeBullet.gameObject.SetActive(value: false);
			SmokeBulletParty.gameObject.SetActive(value: true);
			SmokeBullet = SmokeBulletParty.GetComponent<ParticleSystem>();
		}
	}

	public void ChargeElectric()
	{
		if (!IsElectricCharged)
		{
			SmokeBullet.Stop();
			SmokeBulletParty.Stop();
			ElectricState.SetActive(value: true);
			IsElectricCharged = true;
			BulletSpeed = 14f;
			Play2DClipAtPoint(ElectricCharge);
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

	private void Update()
	{
		rb.velocity = rb.velocity.normalized * BulletSpeed;
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		lastPosition = base.transform.position;
		base.transform.LookAt(myLaser.lookAtPoint);
		base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
		if (GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted)
		{
			End();
		}
		if (GameMaster.instance != null)
		{
			if ((BounceAmount > maxBounces || GameMaster.instance.AmountEnemyTanks < 1 || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive) && !GameMaster.instance.isZombieMode && !GameMaster.instance.CM)
			{
				End();
			}
			else if ((BounceAmount > maxBounces || !GameMaster.instance.PlayerAlive) && (GameMaster.instance.isZombieMode || (bool)GameMaster.instance.CM))
			{
				End();
			}
		}
	}

	private void End()
	{
		GameObject poof = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
		poof.GetComponent<ParticleSystem>().Play();
		Object.Destroy(poof.gameObject, 3f);
		DeadSound();
		if (TankScript != null)
		{
			TankScript.firedBullets--;
		}
		else if (TankScriptAI != null)
		{
			TankScriptAI.firedBullets--;
		}
		SmokeBullet.Stop();
		SmokeBullet.transform.parent = null;
		Object.Destroy(SmokeBullet.gameObject, 5f);
		Object.Destroy(base.gameObject);
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
		if (myLaser.WallGonnaBounceInTo == null && collision.gameObject.tag != "Enemy")
		{
			if (BounceAmount != 0 || !(collision.gameObject.tag == "Player"))
			{
				Debug.LogWarning("No target error stay");
				End();
			}
		}
		else if (collision.gameObject != myLaser.WallGonnaBounceInTo && !nearWall && (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder"))
		{
			Debug.LogError("Im bouncing into: " + collision.gameObject?.ToString() + " while I should have bounced into:" + myLaser.WallGonnaBounceInTo);
			rb.velocity = lastFrameVelocity;
			rb.angularVelocity = lastAngularVelocity;
			base.transform.position = lastPosition;
			Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
		}
		else if (!hittingWall)
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
		Debug.LogWarning("Ignoring collision with " + collision.transform.name);
		rb.velocity = lastFrameVelocity;
		rb.angularVelocity = lastAngularVelocity;
		base.transform.position = lastPosition;
		Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
	}

	private bool CheckBullet(Collision collision)
	{
		EnemyBulletScript EBS = collision.gameObject.GetComponent<EnemyBulletScript>();
		if (EBS != null && EBS.isElectric)
		{
			Debug.LogWarning("Its/Me an electric bullet!");
			IgnoreThatCollision(collision);
			return true;
		}
		return false;
	}

	public void WallImpact(Collision collision)
	{
		hittingWall = true;
		StartCoroutine("deHit");
		if (collision.collider.gameObject.tag == "MapBorder" && (GameMaster.instance.isZombieMode || GameMaster.instance.CurrentMission == 49))
		{
			BounceAmount = 999;
		}
		if (collision.gameObject == PreviousCollidedWith)
		{
			return;
		}
		PreviousCollidedWith = null;
		if (!canHurt)
		{
			return;
		}
		if (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder")
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
			if (BounceAmount < maxBounces)
			{
				HitWallSound();
			}
			else if ((bool)AchievementsTracker.instance)
			{
				AchievementsTracker.instance.HasMissed = true;
			}
			BounceAmount++;
		}
		else if (collision.gameObject.tag == "Bullet")
		{
			if (!CheckBullet(collision))
			{
				BounceAmount = 999;
			}
		}
		else if (collision.gameObject.tag == "Turret")
		{
			WeeTurret WT = collision.gameObject.GetComponent<WeeTurret>();
			if (WT != null && GameMaster.instance.GameHasStarted)
			{
				WT.Health--;
				BounceAmount = 999;
			}
		}
		else
		{
			if (collision.gameObject.layer != LayerMask.NameToLayer("Tank") && !(collision.gameObject.tag == "Enemy") && collision.gameObject.layer != LayerMask.NameToLayer("PlayerTank"))
			{
				return;
			}
			if (collision.gameObject.tag == "Player")
			{
				if (!(GameMaster.instance != null))
				{
					return;
				}
				if (!GameMaster.instance.FriendlyFire)
				{
					bool colliderIsPlayer2 = false;
					MoveTankScript movetank = collision.gameObject.GetComponent<MoveTankScript>();
					EnemyAI aitank = collision.gameObject.GetComponent<EnemyAI>();
					if ((bool)movetank)
					{
						if (movetank.isPlayer2)
						{
							colliderIsPlayer2 = true;
						}
					}
					else if ((bool)aitank && aitank.IsCompanion)
					{
						colliderIsPlayer2 = true;
					}
					if ((colliderIsPlayer2 && ShotByPlayer2) || (!ShotByPlayer2 && !colliderIsPlayer2))
					{
						if (BounceAmount > 0)
						{
							HealthTanks healthscript2 = collision.gameObject.GetComponent<HealthTanks>();
							if (healthscript2.health > 1 && GameMaster.instance.GameHasStarted)
							{
								Play2DClipAtPoint(BuzzHit);
							}
							if (GameMaster.instance.GameHasStarted)
							{
								healthscript2.health--;
							}
							BounceAmount = 999;
						}
					}
					else
					{
						Debug.Log("Tank");
						BounceAmount = 999;
					}
					return;
				}
				bool colliderIsPlayer3 = false;
				MoveTankScript movetank2 = collision.gameObject.GetComponent<MoveTankScript>();
				EnemyAI aitank2 = collision.gameObject.GetComponent<EnemyAI>();
				if ((bool)movetank2)
				{
					if (movetank2.isPlayer2)
					{
						colliderIsPlayer3 = true;
					}
				}
				else if ((bool)aitank2 && aitank2.IsCompanion)
				{
					colliderIsPlayer3 = true;
				}
				if (movetank2 != null || aitank2 != null)
				{
					if ((colliderIsPlayer3 && ShotByPlayer2) || (!ShotByPlayer2 && !colliderIsPlayer3))
					{
						if (BounceAmount > 0)
						{
							Debug.LogError("Killed thy self");
							HealthTanks healthscript4 = collision.gameObject.GetComponent<HealthTanks>();
							if (healthscript4.health > 1 && GameMaster.instance.GameHasStarted)
							{
								Play2DClipAtPoint(BuzzHit);
							}
							if (GameMaster.instance.GameHasStarted)
							{
								healthscript4.health--;
							}
							BounceAmount = 999;
						}
						else
						{
							Debug.LogError("I JUST PREVENTED A GLITCH SHOT KILL SELF");
						}
					}
					else
					{
						HealthTanks healthscript3 = collision.gameObject.GetComponent<HealthTanks>();
						if (healthscript3.health > 1 && GameMaster.instance.GameHasStarted)
						{
							Play2DClipAtPoint(BuzzHit);
						}
						if (GameMaster.instance.GameHasStarted)
						{
							healthscript3.health--;
						}
						BounceAmount = 999;
					}
					return;
				}
				EnemyAI AIscript2 = collision.gameObject.GetComponent<EnemyAI>();
				if (!(AIscript2 != null))
				{
					return;
				}
				if (AIscript2.IsCompanion && ShotByPlayer2)
				{
					if (BounceAmount > 0)
					{
						HealthTanks healthscript5 = collision.gameObject.GetComponent<HealthTanks>();
						if (healthscript5.health > 1 && GameMaster.instance.GameHasStarted)
						{
							Play2DClipAtPoint(BuzzHit);
						}
						if (GameMaster.instance.GameHasStarted)
						{
							healthscript5.health--;
						}
						BounceAmount = 999;
					}
				}
				else
				{
					HealthTanks healthscript6 = collision.gameObject.GetComponent<HealthTanks>();
					if (healthscript6.health > 1 && GameMaster.instance.GameHasStarted)
					{
						Play2DClipAtPoint(BuzzHit);
					}
					if (GameMaster.instance.GameHasStarted)
					{
						healthscript6.health--;
					}
					BounceAmount = 999;
				}
				return;
			}
			EnemyAI AIscript = collision.gameObject.GetComponent<EnemyAI>();
			if (!GameMaster.instance.isZombieMode)
			{
				if (AIscript.isTransporting)
				{
					return;
				}
				if (AIscript.isLevel10Boss || AIscript.isLevel30Boss || AIscript.isLevel50Boss || AIscript.isLevel70Boss)
				{
					AchievementsTracker.instance.HasShotBoss = true;
				}
			}
			Play2DClipAtPoint(enemyKill);
			HealthTanks healthscript = collision.gameObject.GetComponent<HealthTanks>();
			if (GameMaster.instance.GameHasStarted)
			{
				healthscript.health--;
			}
			if (healthscript.health < 1 && !GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode)
			{
				if (BounceAmount == 0)
				{
					AchievementsTracker.instance.KilledWithoutBounce = true;
				}
				else
				{
					GameMaster.instance.totalKillsBounce++;
				}
			}
			BounceAmount = 999;
		}
	}

	private void hittag(int playerID, GameObject hittagPRefab, HealthTanks healthscript)
	{
		GameObject hittag = Object.Instantiate(hittagPRefab, base.transform.position, Quaternion.identity);
		if (GameMaster.instance != null && healthscript.health < 1)
		{
			GameMaster.instance.Playerkills[playerID]++;
			GameMaster.instance.TotalKillsThisSession++;
			if (!GameMaster.instance.inMapEditor)
			{
				if ((bool)AchievementsTracker.instance && BounceAmount == 0)
				{
					AchievementsTracker.instance.KilledWithoutBounce = true;
				}
				GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", 1);
			}
		}
		Object.Destroy(hittag, 3f);
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
		}
		else
		{
			direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
			Debug.LogError("Unexpected bounce oh no!!!");
		}
		canHurt = false;
		StartCoroutine("Hurt");
		rb.velocity = direction * Mathf.Max(speed, minVelocity);
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
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
