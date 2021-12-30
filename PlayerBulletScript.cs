using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
	public bool isEnemyBullet;

	public bool isTowerCharged;

	public float BulletSpeed = 5f;

	public int TimesBounced;

	public int MaxBounces;

	public Vector3[] MyCollisionPoints;

	public AudioClip[] WallHit;

	public AudioClip[] WallHitElectro;

	public AudioClip[] DeadHit;

	public AudioClip[] DeadHitElectric;

	public AudioClip ElectricCharge;

	public ParticleSystem SmokeBullet;

	public ParticleSystem SmokeBulletParty;

	public ParticleSystem TowerChargeParticles;

	public GameObject poofParticles;

	public GameObject poofParticlesElectro;

	public GameObject bounceParticles;

	public GameObject deathExplosion;

	public GameObject HitTextP1;

	public GameObject HitTextP2;

	public GameObject HitTextP3;

	public GameObject HitTextP4;

	public AudioClip enemyKill;

	public AudioClip BuzzHit;

	public GameObject PreviousCollision;

	public int MyTeam;

	public int ShotByPlayer = -1;

	public EnemyTargetingSystemNew EnemyTankScript;

	public FiringTank TankScript;

	public EnemyTargetingSystemNew TankScriptAI;

	public AudioSource MySource;

	private Vector3 lastFrameVelocity;

	private Rigidbody rb;

	private Vector3 lastAngularVelocity;

	private Vector3 lastPosition;

	public bool IsElectricCharged;

	public bool isExplosive;

	public bool isElectric;

	public bool isBossBullet;

	public GameObject ElectricState;

	public GameObject papaTank;

	[Header("Debug Info")]
	public List<GameObject> MyPreviousColliders;

	public List<GameObject> MyPreviousCollidersInteractions;

	public GameObject WallGonnaBounceInTo;

	public float CurrentSpeed;

	public GameObject PreviousBouncedWall;

	public bool IsSackBullet;

	public bool TurretBullet;

	[SerializeField]
	private Vector3 initialVelocity;

	private Vector3 lookAtPoint;

	public bool isShiny;

	public bool isSilver;

	public Vector3 upcomingDirection;

	public Vector3 upcomingPosition;

	public Vector3 beforePosition;

	public bool DestroyOnImpact;

	public bool LookAtPointSet;

	private bool isEnding;

	public List<Collider> CollidersIgnoring = new List<Collider>();

	private bool CanBounce = true;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		MySource = GetComponent<AudioSource>();
		base.transform.Rotate(-90f, 180f, 0f);
		if (!GameMaster.instance.isZombieMode && !GameMaster.instance.inMenuMode)
		{
			base.transform.position = new Vector3(base.transform.position.x, 1f, base.transform.position.z);
		}
	}

	private void Lasers()
	{
		Debug.DrawLine(base.transform.position, upcomingPosition, Color.green, 3f);
		Debug.DrawRay(base.transform.position, rb.velocity, Color.red, 3f);
		_ = upcomingPosition - beforePosition;
	}

	private void Start()
	{
		InvokeRepeating("Lasers", 0.5f, 0.3f);
		if (!isEnemyBullet)
		{
			SetColorBullet();
		}
		if (OptionsMainMenu.instance.showxraybullets)
		{
			base.gameObject.AddComponent<Outline>();
			Outline component = GetComponent<Outline>();
			component.OutlineMode = Outline.Mode.OutlineHidden;
			component.OutlineColor = Color.white;
			component.OutlineWidth = 0.65f;
		}
		StartCoroutine(LateStart());
		if (isBossBullet)
		{
			if (OptionsMainMenu.instance.currentDifficulty == 1)
			{
				BulletSpeed *= 1.1f;
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 2)
			{
				BulletSpeed *= 1.25f;
			}
		}
		if (isShiny)
		{
			ParticleSystem component2 = base.transform.Find("ShinyParticlesBullet").GetComponent<ParticleSystem>();
			if ((bool)component2)
			{
				component2.Play();
			}
		}
		if (isTowerCharged)
		{
			TowerChargeParticles.Play();
			GetComponent<MeshRenderer>().material.color = Color.yellow;
			BulletSpeed *= 1.5f;
			SmokeBullet.Stop();
		}
	}

	private void SetColorBullet()
	{
		if ((bool)OptionsMainMenu.instance && !IsSackBullet && OptionsMainMenu.instance.AMselected.Contains(6))
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
		if (!IsElectricCharged && !isElectric)
		{
			SmokeBullet.Stop();
			if ((bool)SmokeBulletParty)
			{
				SmokeBulletParty.Stop();
			}
			ElectricState.SetActive(value: true);
			IsElectricCharged = true;
			if (BulletSpeed < 11f)
			{
				BulletSpeed = Mathf.Round(BulletSpeed * 2f);
			}
			else
			{
				BulletSpeed = Mathf.Round(BulletSpeed * 1.5f);
			}
			GameMaster.instance.Play2DClipAtPoint(ElectricCharge, 1f);
		}
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.01f);
		initialVelocity = rb.velocity;
		CastLaser(initialVelocity);
		yield return new WaitForSeconds(15f);
		TimesBounced = 9999;
	}

	private IEnumerator CastLaserDelay()
	{
		yield return new WaitForSeconds(0.01f);
		CastLaser(rb.velocity);
	}

	private void CastLaser(Vector3 velocity)
	{
		if (velocity == Vector3.zero)
		{
			StartCoroutine(CastLaserDelay());
		}
		else
		{
			DrawReflectionPattern(base.transform.position, velocity, isDistanceTest: false);
		}
	}

	private void DrawReflectionPattern(Vector3 position, Vector3 direction, bool isDistanceTest)
	{
		DestroyOnImpact = false;
		Vector3 start = position;
		Ray ray = new Ray(position, direction);
		LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall"));
		if (Physics.Raycast(ray, out var hitInfo, 1000f, layerMask))
		{
			if (isDistanceTest)
			{
				if (Vector3.Distance(position, hitInfo.point) < 0.3f)
				{
					DestroyOnImpact = true;
				}
				Debug.DrawLine(start, hitInfo.point, Color.blue, 5f);
				return;
			}
			direction = Vector3.Reflect(direction.normalized, hitInfo.normal);
			position = hitInfo.point;
			beforePosition = base.transform.position;
			upcomingPosition = position;
			upcomingDirection = direction;
			WallGonnaBounceInTo = hitInfo.transform.gameObject;
			lookAtPoint = position;
			LookAtPointSet = true;
			Debug.DrawLine(start, position, Color.blue, 5f);
			DrawReflectionPattern(position, upcomingDirection, isDistanceTest: true);
		}
		else if (isDistanceTest)
		{
			DestroyOnImpact = true;
		}
		else
		{
			Debug.DrawRay(position, direction * 15f, Color.red, 5f);
			Physics.Raycast(ray, out hitInfo, 1000f);
			TimesBounced = 999;
		}
	}

	private void Update()
	{
		CurrentSpeed = rb.velocity.magnitude;
		if (GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted)
		{
			End(isEndingGame: true);
		}
		if (MapEditorMaster.instance != null && MapEditorMaster.instance.inPlayingMode && !GameMaster.instance.GameHasStarted)
		{
			End(isEndingGame: true);
		}
		if ((GameMaster.instance.AmountEnemyTanks < 1 || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive) && !GameMaster.instance.isZombieMode && !GameMaster.instance.CM && !GameMaster.instance.inTankeyTown && !GameMaster.instance.inMenuMode && !MapEditorMaster.instance)
		{
			if (GameMaster.instance.PlayerAlive && GameMaster.instance.CurrentMission == 99)
			{
				if (TimesBounced > MaxBounces)
				{
					End(isEndingGame: false);
				}
			}
			else
			{
				End(isEndingGame: true);
			}
		}
		else if (!GameMaster.instance.PlayerAlive && (GameMaster.instance.isZombieMode || (bool)GameMaster.instance.CM))
		{
			End(isEndingGame: true);
		}
		else if (TimesBounced > MaxBounces)
		{
			End(isEndingGame: false);
		}
	}

	private void FixedUpdate()
	{
		if (LookAtPointSet)
		{
			Vector3 velocity = (upcomingPosition - beforePosition).normalized * BulletSpeed;
			rb.velocity = velocity;
			if (TimesBounced > 0)
			{
				rb.angularVelocity = Vector3.zero;
			}
		}
		lastFrameVelocity = rb.velocity;
		lastPosition = base.transform.position;
		if (rb.velocity != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(rb.velocity, base.transform.up);
			base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
		}
		if (WallGonnaBounceInTo == null)
		{
			CastLaser(rb.velocity);
		}
	}

	private void End(bool isEndingGame)
	{
		if (isEnding)
		{
			return;
		}
		isEnding = true;
		if (isExplosive)
		{
			if (!isEndingGame && (GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode))
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
				AudioSource component2 = transform.gameObject.GetComponent<AudioSource>();
				if ((bool)component2)
				{
					component2.volume = 0f;
					component2.Stop();
				}
			}
			if ((bool)TowerChargeParticles)
			{
				TowerChargeParticles.Stop();
				TowerChargeParticles.transform.parent = null;
			}
			Transform transform2 = base.transform.Find("ShinyParticlesBullet");
			if ((bool)transform2)
			{
				transform2.GetComponent<ParticleSystem>().Stop();
				transform.parent = null;
			}
			if (!isEnemyBullet)
			{
				if (TankScript != null)
				{
					TankScript.firedBullets--;
				}
				else if (TankScriptAI != null)
				{
					TankScriptAI.firedBullets--;
				}
			}
			else if ((bool)EnemyTankScript)
			{
				EnemyTankScript.firedBullets--;
			}
			if (transform != null)
			{
				Object.Destroy(transform.gameObject, 5f);
			}
			Object.Destroy(base.gameObject);
		}
		else
		{
			GameObject gameObject = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
			gameObject.GetComponent<ParticleSystem>().Play();
			Object.Destroy(gameObject.gameObject, 3f);
			DeadSound();
			if (!isEnemyBullet)
			{
				if (TankScript != null)
				{
					TankScript.firedBullets--;
				}
				else if (TankScriptAI != null)
				{
					TankScriptAI.firedBullets--;
				}
			}
			else if ((bool)EnemyTankScript)
			{
				EnemyTankScript.firedBullets--;
			}
		}
		if ((bool)SmokeBullet)
		{
			SmokeBullet.Stop();
			SmokeBullet.transform.SetParent(null);
			AudioSource component3 = SmokeBullet.gameObject.GetComponent<AudioSource>();
			if ((bool)component3)
			{
				component3.volume = 0f;
				component3.Stop();
			}
			Object.Destroy(SmokeBullet.gameObject, 5f);
		}
		Object.Destroy(base.gameObject);
	}

	private void DeadSound()
	{
		if (IsElectricCharged)
		{
			int maxExclusive = DeadHitElectric.Length;
			int num = Random.Range(0, maxExclusive);
			GameMaster.instance.Play2DClipAtPoint(DeadHitElectric[num], 1f);
		}
		else
		{
			int maxExclusive2 = DeadHit.Length;
			int num2 = Random.Range(0, maxExclusive2);
			GameMaster.instance.Play2DClipAtPoint(DeadHit[num2], 1f);
		}
	}

	private void HitWallSound()
	{
		if (IsElectricCharged)
		{
			int maxExclusive = WallHitElectro.Length;
			int num = Random.Range(0, maxExclusive);
			GameMaster.instance.Play2DClipAtPoint(WallHitElectro[num], 1f);
		}
		else
		{
			int maxExclusive2 = WallHit.Length;
			int num2 = Random.Range(0, maxExclusive2);
			GameMaster.instance.Play2DClipAtPoint(WallHit[num2], 1f);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (DestroyOnImpact)
		{
			TimesBounced = 9999;
		}
		OnCollision(collision);
	}

	private void IgnoreThatCollision(Collision collision)
	{
		rb.velocity = lastFrameVelocity;
		base.transform.position = lastPosition;
		Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
		CollidersIgnoring.Add(collision.collider);
	}

	private void OnCollision(Collision collision)
	{
		MyPreviousColliders.Add(collision.gameObject);
		if (collision.gameObject.tag == "Bullet" && !isExplosive && !isSilver)
		{
			PlayerBulletScript component = collision.gameObject.GetComponent<PlayerBulletScript>();
			if (component != null && component.papaTank == papaTank && component.TimesBounced == 0 && TimesBounced == 0)
			{
				IgnoreThatCollision(collision);
				return;
			}
		}
		if (WallGonnaBounceInTo == null)
		{
			if ((TimesBounced == 0 && collision.gameObject.tag == "Player" && !isEnemyBullet) || (TimesBounced == 0 && collision.gameObject.tag == "Enemy" && isEnemyBullet))
			{
				return;
			}
			if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Boss")
			{
				End(isEndingGame: false);
				return;
			}
		}
		StartCoroutine(WallImpact(collision));
	}

	private bool CheckBullet(Collision collision)
	{
		PlayerBulletScript component = collision.gameObject.GetComponent<PlayerBulletScript>();
		if (component != null)
		{
			if (GameMaster.instance.CurrentMission == 69 && isBossBullet)
			{
				if (BulletSpeed > 5f)
				{
					BulletSpeed -= 3f;
				}
				Debug.Log("SLOWED DOWN" + BulletSpeed);
				return true;
			}
			if (component.isElectric)
			{
				IgnoreThatCollision(collision);
				if (GameMaster.instance.CurrentMission == 69)
				{
					ChargeElectric();
				}
				return true;
			}
			if (isElectric)
			{
				IgnoreThatCollision(collision);
				return true;
			}
			if (component.isExplosive || component.isSilver)
			{
				return false;
			}
			if (isExplosive || isSilver)
			{
				IgnoreThatCollision(collision);
				return true;
			}
		}
		return false;
	}

	private IEnumerator ResetBounce()
	{
		CanBounce = false;
		yield return new WaitForSeconds(0.1f);
		CanBounce = true;
	}

	public void HitNoBounceWall(Collision collision)
	{
		BombSackScript component = collision.gameObject.GetComponent<BombSackScript>();
		if (component != null && !IsSackBullet)
		{
			Debug.Log("HIT SACK");
			Vector3 direction = new Vector3(component.transform.position.x, base.transform.position.y + 1f, component.transform.position.z) - base.transform.position;
			float force = (isTowerCharged ? 15f : 11f);
			component.HitByBullet(direction, force);
		}
		End(isEndingGame: false);
	}

	public IEnumerator WallImpact(Collision collision)
	{
		if (collision.gameObject == PreviousCollision)
		{
			IgnoreThatCollision(collision);
			yield break;
		}
		if (collision.gameObject.name == "Shield" && (bool)EnemyTankScript && collision.gameObject.transform.parent == EnemyTankScript.AIscript.gameObject)
		{
			IgnoreThatCollision(collision);
			yield break;
		}
		MyPreviousCollidersInteractions.Add(collision.gameObject);
		if (collision.gameObject != WallGonnaBounceInTo && (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder"))
		{
			float num = Vector3.Distance(base.transform.position, upcomingPosition);
			if (num < 0.5f)
			{
				if (collision.gameObject.layer == LayerMask.NameToLayer("NoBounceWall"))
				{
					HitNoBounceWall(collision);
					yield break;
				}
				PreviousCollision = collision.gameObject;
				StartCoroutine(ResetBounce());
				Bounce();
			}
			else if (collision.gameObject.layer == LayerMask.NameToLayer("NoBounceWall"))
			{
				HitNoBounceWall(collision);
			}
			else
			{
				if (!CanBounce)
				{
					yield break;
				}
				if (num > 1f)
				{
					IgnoreThatCollision(collision);
					yield break;
				}
				if (TimesBounced == MaxBounces)
				{
					End(isEndingGame: false);
				}
				PreviousCollision = collision.gameObject;
				upcomingPosition = base.transform.position;
				StartCoroutine(ResetBounce());
				Bounce();
			}
			yield break;
		}
		PreviousCollision = null;
		if (collision.collider.gameObject.tag == "MapBorder" && (GameMaster.instance.isZombieMode || GameMaster.instance.CurrentMission == 49))
		{
			TimesBounced = 999;
		}
		if (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder")
		{
			if (collision.gameObject.layer == 16 || collision.gameObject.layer == LayerMask.NameToLayer("NoBounceWall"))
			{
				HitNoBounceWall(collision);
				TimesBounced = 99;
				yield break;
			}
			float num2 = Vector3.Distance(base.transform.position, upcomingPosition);
			if (!(num2 < 0.5f))
			{
				float num3 = ((BulletSpeed < 10f) ? 125f : 175f);
				float seconds = num2 * (BulletSpeed / num3);
				rb.detectCollisions = false;
				CanBounce = false;
				Collider C = GetComponent<Collider>();
				C.enabled = false;
				yield return new WaitForSeconds(seconds);
				rb.detectCollisions = true;
				C.enabled = true;
			}
			PreviousCollision = collision.gameObject;
			StartCoroutine(ResetBounce());
			Bounce();
		}
		else if (collision.gameObject.tag == "Bullet")
		{
			if (!CheckBullet(collision))
			{
				TimesBounced = 999;
			}
		}
		else if (collision.gameObject.tag == "Turret")
		{
			if (TurretBullet)
			{
				TimesBounced = 999;
				yield break;
			}
			WeeTurret component = collision.gameObject.GetComponent<WeeTurret>();
			if (component != null && GameMaster.instance.GameHasStarted)
			{
				component.Health--;
				TimesBounced = 999;
			}
		}
		else if (collision.gameObject.tag == "Player")
		{
			if (TurretBullet)
			{
				TimesBounced = 999;
				yield break;
			}
			MoveTankScript component2 = collision.gameObject.GetComponent<MoveTankScript>();
			EnemyAI component3 = collision.gameObject.GetComponent<EnemyAI>();
			HealthTanks component4 = collision.gameObject.GetComponent<HealthTanks>();
			if (component2 != null)
			{
				if (component2.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire)
				{
					if (TankScript != null)
					{
						if (component2 == TankScript.tankMovingScript && TimesBounced > 0)
						{
							HitTank(collision);
							yield break;
						}
						if (component2 == TankScript.tankMovingScript)
						{
							IgnoreThatCollision(collision);
							yield break;
						}
					}
					TimesBounced = 999;
					yield break;
				}
				if ((bool)TankScript)
				{
					if (component2 == TankScript.tankMovingScript && TimesBounced > 0)
					{
						HitTank(collision);
						yield break;
					}
					if (component2 == TankScript.tankMovingScript)
					{
						IgnoreThatCollision(collision);
						yield break;
					}
				}
				if (MyTeam == 0)
				{
					HitTank(collision);
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(component4);
					}
				}
				else if (component2.MyTeam == MyTeam && OptionsMainMenu.instance.FriendlyFire)
				{
					HitTank(collision);
				}
				else if (component2.MyTeam == MyTeam && !OptionsMainMenu.instance.FriendlyFire)
				{
					TimesBounced = 999;
				}
				else if (component2.MyTeam != MyTeam)
				{
					HitTank(collision);
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(component4);
					}
				}
				else
				{
					HitTank(collision);
				}
			}
			else
			{
				if (!(component3 != null))
				{
					yield break;
				}
				if (component3.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire && component3 != EnemyTankScript)
				{
					TimesBounced = 999;
				}
				else if ((bool)TankScriptAI && component3 == TankScriptAI.AIscript)
				{
					HitTank(collision);
				}
				else if (MyTeam == 0)
				{
					HitTank(collision);
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(component4);
					}
				}
				else if (component3.MyTeam == MyTeam && OptionsMainMenu.instance.FriendlyFire)
				{
					HitTank(collision);
				}
				else if (component3.MyTeam == MyTeam && !OptionsMainMenu.instance.FriendlyFire)
				{
					TimesBounced = 999;
				}
				else if (component3.MyTeam != MyTeam)
				{
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(component4);
					}
					HitTank(collision);
				}
				else
				{
					HitTank(collision);
				}
			}
		}
		else if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss") && !isEnemyBullet)
		{
			EnemyAI component5 = collision.gameObject.GetComponent<EnemyAI>();
			if ((bool)component5 && component5.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire)
			{
				TimesBounced = 999;
				yield break;
			}
			if (!GameMaster.instance.isZombieMode)
			{
				if (component5.isTransporting)
				{
					ChargeElectric();
					yield break;
				}
				if (component5.isLevel10Boss || component5.isLevel30Boss || component5.isLevel50Boss || component5.isLevel70Boss)
				{
					GameObject gameObject = GameObject.Find("BossHealthBar");
					if ((bool)gameObject)
					{
						BossHealthBar component6 = gameObject.GetComponent<BossHealthBar>();
						if ((bool)component6)
						{
							component6.StartCoroutine(component6.MakeBarWhite());
						}
					}
					AchievementsTracker.instance.HasShotBoss = true;
				}
			}
			HealthTanks component7 = collision.gameObject.GetComponent<HealthTanks>();
			if ((bool)component7 && !component7.enabled)
			{
				TimesBounced = 999;
				yield break;
			}
			if ((bool)component7.ShieldFade && component7.ShieldFade.ShieldHealth > 0)
			{
				TimesBounced = 9999;
				component7.ShieldFade.StartFade();
				yield break;
			}
			if (isElectric && !IsElectricCharged && (bool)component5)
			{
				component5.StunMe(3f);
				TimesBounced = 9999;
				yield break;
			}
			GameMaster.instance.Play2DClipAtPoint(enemyKill, 1f);
			if (GameMaster.instance.GameHasStarted)
			{
				component7.health--;
			}
			if (component7.health < 1 && !GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode && (bool)AccountMaster.instance)
			{
				if (TimesBounced == 0)
				{
					AchievementsTracker.instance.KilledWithoutBounce = true;
					AccountMaster.instance.SaveCloudData(0, component7.EnemyID, 0, bounceKill: false);
				}
				else
				{
					GameMaster.instance.totalKillsBounce++;
					AccountMaster.instance.SaveCloudData(0, component7.EnemyID, 0, bounceKill: true);
				}
			}
			else if (component7.health > 0)
			{
				MakeWhite component8 = component7.GetComponent<MakeWhite>();
				if ((bool)component8)
				{
					component8.GotHit();
				}
				if (component7.EnemyID == 9 || component7.EnemyID == 17 || component7.EnemyID == -14 || component7.EnemyID == -110 || component7.Armour != null)
				{
					int maxExclusive = GlobalAssets.instance.ArmourHits.Length;
					int num4 = Random.Range(0, maxExclusive);
					GameMaster.instance.Play2DClipAtPoint(GlobalAssets.instance.ArmourHits[num4], 1f);
				}
			}
			if (ShotByPlayer > -1)
			{
				SpawnHitTag(component7);
			}
			TimesBounced = 999;
		}
		else
		{
			if ((!(collision.gameObject.tag == "Enemy") && !(collision.gameObject.tag == "Boss")) || !isEnemyBullet)
			{
				yield break;
			}
			if (TimesBounced == 0 && collision.gameObject == papaTank.gameObject)
			{
				IgnoreThatCollision(collision);
				yield break;
			}
			EnemyAI component9 = collision.gameObject.GetComponent<EnemyAI>();
			if ((bool)EnemyTankScript && component9 == EnemyTankScript.AIscript && TimesBounced < 1)
			{
				IgnoreThatCollision(collision);
				yield break;
			}
			if (MyTeam == 0)
			{
				HitTank(collision);
				yield break;
			}
			if (component9.MyTeam == MyTeam && MyTeam != 0 && OptionsMainMenu.instance.FriendlyFire)
			{
				HitTank(collision);
				yield break;
			}
			if (component9.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire && component9.gameObject != papaTank.gameObject)
			{
				TimesBounced = 999;
				yield break;
			}
			_ = component9.MyTeam;
			_ = MyTeam;
			HitTank(collision);
		}
	}

	private void SpawnHitTag(HealthTanks healthscript)
	{
		if (ShotByPlayer == 0)
		{
			HitTag(0, HitTextP1, healthscript);
		}
		else if (ShotByPlayer == 1)
		{
			HitTag(1, HitTextP2, healthscript);
		}
		else if (ShotByPlayer == 2)
		{
			HitTag(2, HitTextP3, healthscript);
		}
		else if (ShotByPlayer == 3)
		{
			HitTag(3, HitTextP4, healthscript);
		}
	}

	private void HitTag(int playerID, GameObject hittagPrefab, HealthTanks healthscript)
	{
		GameObject obj = Object.Instantiate(hittagPrefab, base.transform.position, Quaternion.identity);
		if (GameMaster.instance != null && healthscript.health < 1)
		{
			GameMaster.instance.Playerkills[playerID]++;
			GameMaster.instance.TotalKillsThisSession++;
			if (!GameMaster.instance.inMapEditor)
			{
				if ((bool)AchievementsTracker.instance && TimesBounced == 0)
				{
					AchievementsTracker.instance.KilledWithoutBounce = true;
				}
				if ((bool)GameMaster.instance.PKU)
				{
					GameMaster.instance.PKU.StartCoroutine("StartPlayerKillsAnimation", playerID + 1);
				}
			}
		}
		Object.Destroy(obj, 3f);
	}

	private void HitTank(Collision collision)
	{
		if (isElectric)
		{
			if (GameMaster.instance.CurrentMission == 69)
			{
				HealthTanks component = collision.gameObject.GetComponent<HealthTanks>();
				if ((bool)component)
				{
					component.health--;
				}
			}
			else
			{
				MoveTankScript component2 = collision.gameObject.GetComponent<MoveTankScript>();
				if (component2 != null)
				{
					component2.StunMe(3f);
				}
				EnemyAI component3 = collision.gameObject.GetComponent<EnemyAI>();
				if (component3 != null && component3.HTscript.EnemyID != 15)
				{
					component3.StunMe(3f);
				}
			}
		}
		else
		{
			EnemyAI component4 = collision.gameObject.GetComponent<EnemyAI>();
			if (component4 != null && component4.isTransporting)
			{
				return;
			}
			HealthTanks component5 = collision.gameObject.GetComponent<HealthTanks>();
			if (component5.health > 1 && (component5.EnemyID == 9 || component5.EnemyID == 17 || component5.EnemyID == -14 || component5.EnemyID == -110 || component5.isMainTank || component5.Armour != null))
			{
				int maxExclusive = GlobalAssets.instance.ArmourHits.Length;
				int num = Random.Range(0, maxExclusive);
				GameMaster.instance.Play2DClipAtPoint(GlobalAssets.instance.ArmourHits[num], 1f);
			}
			if (GameMaster.instance.isZombieMode && component5.health > 1)
			{
				component5.Play2DClipAtPoint(component5.Buzz);
			}
			else if (component5.health > 1 && (component5.EnemyID == 9 || component5.EnemyID == 17 || component5.EnemyID == -12 || component5.EnemyID == -110))
			{
				int maxExclusive2 = GlobalAssets.instance.ArmourHits.Length;
				int num2 = Random.Range(0, maxExclusive2);
				GameMaster.instance.Play2DClipAtPoint(GlobalAssets.instance.ArmourHits[num2], 1f);
			}
			component5.health--;
		}
		TimesBounced = 999;
	}

	public void Bounce()
	{
		if (TimesBounced < MaxBounces)
		{
			HitWallSound();
		}
		else if ((bool)AchievementsTracker.instance)
		{
			AchievementsTracker.instance.HasMissed = true;
		}
		TimesBounced++;
		if (CollidersIgnoring.Count > 0)
		{
			foreach (Collider item in CollidersIgnoring)
			{
				if (item != null)
				{
					Physics.IgnoreCollision(GetComponent<Collider>(), item, ignore: false);
				}
			}
			CollidersIgnoring.Clear();
		}
		GameObject obj = Object.Instantiate(bounceParticles, base.transform.position, Quaternion.identity);
		obj.GetComponent<ParticleSystem>().Play();
		Object.Destroy(obj.gameObject, 3f);
		rb.velocity = upcomingDirection;
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		rb.angularVelocity = Vector3.zero;
		base.transform.position = upcomingPosition;
		DrawReflectionPattern(upcomingPosition, upcomingDirection, isDistanceTest: false);
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
			ExplosiveBlock component7 = obj.GetComponent<ExplosiveBlock>();
			if ((bool)component7 && !component7.isExploding)
			{
				component7.StartCoroutine(component7.Death());
			}
			if (component6 != null)
			{
				component6.Health--;
			}
			if (component != null && !component.immuneToExplosion)
			{
				if ((bool)component.ShieldFade)
				{
					if (component.ShieldFade.ShieldHealth > 0)
					{
						component.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						component.health--;
					}
				}
				else
				{
					component.health--;
				}
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
}
