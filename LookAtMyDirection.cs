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
	public bool ShotByPlayer2;

	public FiringTank TankScript;

	public EnemyTargetingSystemNew TankScriptAI;

	public AudioSource MySource;

	public bool canHurt;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private float minVelocity = 10f;

	private Vector3 lastFrameVelocity;

	private Rigidbody rb;

	private Vector3 lastAngularVelocity;

	private Vector3 lastPosition;

	public bool IsElectricCharged;

	public GameObject ElectricState;

	public bool nearWall;

	private LaserScript myLaser;

	public Vector3 newVelocity;

	private GameObject PreviousCollidedWith;

	private bool hittingWall;

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
			int num = Random.Range(0, 5);
			Color color = Color.red;
			switch (num)
			{
			case 0:
				color = Color.red;
				break;
			case 1:
				color = Color.blue;
				break;
			case 2:
				color = Color.green;
				break;
			case 3:
				color = Color.cyan;
				break;
			case 4:
				color = Color.yellow;
				break;
			}
			GetComponent<Renderer>().material.color = color;
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
		GameObject gameObject = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
		gameObject.GetComponent<ParticleSystem>().Play();
		Object.Destroy(gameObject.gameObject, 3f);
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
		EnemyBulletScript component = collision.gameObject.GetComponent<EnemyBulletScript>();
		if (component != null && component.isElectric)
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
			WeeTurret component = collision.gameObject.GetComponent<WeeTurret>();
			if (component != null && GameMaster.instance.GameHasStarted)
			{
				component.Health--;
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
					bool flag = false;
					MoveTankScript component2 = collision.gameObject.GetComponent<MoveTankScript>();
					EnemyAI component3 = collision.gameObject.GetComponent<EnemyAI>();
					if ((bool)component2)
					{
						if (component2.isPlayer2)
						{
							flag = true;
						}
					}
					else if ((bool)component3 && component3.IsCompanion)
					{
						flag = true;
					}
					if ((flag && ShotByPlayer2) || (!ShotByPlayer2 && !flag))
					{
						if (BounceAmount > 0)
						{
							HealthTanks component4 = collision.gameObject.GetComponent<HealthTanks>();
							if (component4.health > 1 && GameMaster.instance.GameHasStarted)
							{
								Play2DClipAtPoint(BuzzHit);
							}
							if (GameMaster.instance.GameHasStarted)
							{
								component4.health--;
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
				bool flag2 = false;
				MoveTankScript component5 = collision.gameObject.GetComponent<MoveTankScript>();
				EnemyAI component6 = collision.gameObject.GetComponent<EnemyAI>();
				if ((bool)component5)
				{
					if (component5.isPlayer2)
					{
						flag2 = true;
					}
				}
				else if ((bool)component6 && component6.IsCompanion)
				{
					flag2 = true;
				}
				if (component5 != null || component6 != null)
				{
					if ((flag2 && ShotByPlayer2) || (!ShotByPlayer2 && !flag2))
					{
						if (BounceAmount > 0)
						{
							Debug.LogError("Killed thy self");
							HealthTanks component7 = collision.gameObject.GetComponent<HealthTanks>();
							if (component7.health > 1 && GameMaster.instance.GameHasStarted)
							{
								Play2DClipAtPoint(BuzzHit);
							}
							if (GameMaster.instance.GameHasStarted)
							{
								component7.health--;
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
						HealthTanks component8 = collision.gameObject.GetComponent<HealthTanks>();
						if (component8.health > 1 && GameMaster.instance.GameHasStarted)
						{
							Play2DClipAtPoint(BuzzHit);
						}
						if (GameMaster.instance.GameHasStarted)
						{
							component8.health--;
						}
						BounceAmount = 999;
					}
					return;
				}
				EnemyAI component9 = collision.gameObject.GetComponent<EnemyAI>();
				if (!(component9 != null))
				{
					return;
				}
				if (component9.IsCompanion && ShotByPlayer2)
				{
					if (BounceAmount > 0)
					{
						HealthTanks component10 = collision.gameObject.GetComponent<HealthTanks>();
						if (component10.health > 1 && GameMaster.instance.GameHasStarted)
						{
							Play2DClipAtPoint(BuzzHit);
						}
						if (GameMaster.instance.GameHasStarted)
						{
							component10.health--;
						}
						BounceAmount = 999;
					}
				}
				else
				{
					HealthTanks component11 = collision.gameObject.GetComponent<HealthTanks>();
					if (component11.health > 1 && GameMaster.instance.GameHasStarted)
					{
						Play2DClipAtPoint(BuzzHit);
					}
					if (GameMaster.instance.GameHasStarted)
					{
						component11.health--;
					}
					BounceAmount = 999;
				}
				return;
			}
			EnemyAI component12 = collision.gameObject.GetComponent<EnemyAI>();
			if (!GameMaster.instance.isZombieMode)
			{
				if (component12.isTransporting)
				{
					return;
				}
				if (component12.isLevel10Boss || component12.isLevel30Boss || component12.isLevel50Boss || component12.isLevel70Boss)
				{
					AchievementsTracker.instance.HasShotBoss = true;
				}
			}
			Play2DClipAtPoint(enemyKill);
			HealthTanks component13 = collision.gameObject.GetComponent<HealthTanks>();
			if (GameMaster.instance.GameHasStarted)
			{
				component13.health--;
			}
			if (component13.health < 1 && !GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode)
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
		GameObject obj = Object.Instantiate(hittagPRefab, base.transform.position, Quaternion.identity);
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
		Object.Destroy(obj, 3f);
	}

	public void Bounce(Vector3 collisionNormal, bool unexpectedBounce)
	{
		GameObject obj = Object.Instantiate(bounceParticles, base.transform.position, Quaternion.identity);
		obj.GetComponent<ParticleSystem>().Play();
		Object.Destroy(obj.gameObject, 3f);
		float magnitude = lastFrameVelocity.magnitude;
		Vector3 vector;
		if (!unexpectedBounce)
		{
			vector = newVelocity;
		}
		else
		{
			vector = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);
			Debug.LogError("Unexpected bounce oh no!!!");
		}
		canHurt = false;
		StartCoroutine("Hurt");
		rb.velocity = vector * Mathf.Max(magnitude, minVelocity);
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
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
