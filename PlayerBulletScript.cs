using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBulletScript : MonoBehaviour
{
	public bool isEnemyBullet;

	public bool isTowerCharged = false;

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

	public Material NeonMaterial;

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

	public AudioSource MySource;

	private Vector3 lastFrameVelocity;

	private Rigidbody rb;

	private Vector3 lastAngularVelocity;

	private Vector3 lastPosition;

	public Vector3 StartingVelocity;

	public bool IsElectricCharged = false;

	public bool isExplosive = false;

	public bool isElectric = false;

	public bool isBossBullet = false;

	public bool IsAirBullet = false;

	public GameObject ElectricState;

	public GameObject papaTank;

	[Header("Debug Info")]
	public List<GameObject> MyPreviousColliders;

	public List<GameObject> MyPreviousCollidersInteractions;

	public GameObject WallGonnaBounceInTo;

	public float CurrentSpeed;

	public GameObject PreviousBouncedWall;

	public bool IsSackBullet = false;

	public bool TurretBullet = false;

	[SerializeField]
	private Vector3 initialVelocity;

	private Vector3 lookAtPoint;

	public bool isShiny = false;

	public bool isSuperShiny = false;

	public bool isSilver = false;

	[HideInInspector]
	public bool IsSideBullet = false;

	[Header("Custom Bullets")]
	public GameObject Dart;

	public AudioClip[] DartShootSound;

	private float TimeFlying = 0f;

	public Vector3 upcomingDirection;

	public Vector3 upcomingPosition;

	public Vector3 beforePosition;

	public bool DestroyOnImpact;

	public bool LookAtPointSet = false;

	public float MaxTimeFlying = 30f;

	public bool IsAirPushed = false;

	private bool isEnding = false;

	public List<Collider> CollidersIgnoring = new List<Collider>();

	public float TurningTimeLeft = 0f;

	private Vector3 TurnTowardsPoint;

	private Vector3 InitialDirection;

	private bool CanBounce = true;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		MySource = GetComponent<AudioSource>();
		if (!(Dart != null))
		{
			return;
		}
		Dart.SetActive(value: false);
		if (AccountMaster.instance.Inventory != null && AccountMaster.instance.Inventory.InventoryItems.Length != 0 && AccountMaster.instance.Inventory.InventoryItems.Contains(26) && OptionsMainMenu.instance.AMselected.Contains(1026))
		{
			InstantiateOneParticle IOP = GetComponent<InstantiateOneParticle>();
			if ((bool)IOP)
			{
				IOP.Sound = DartShootSound;
			}
			SmokeBullet.gameObject.SetActive(value: false);
			SmokeBulletParty.gameObject.SetActive(value: false);
			Dart.SetActive(value: true);
			GetComponent<MeshRenderer>().enabled = false;
		}
	}

	private void Lasers()
	{
		Debug.DrawLine(base.transform.position, upcomingPosition, Color.green, 3f);
		Debug.DrawRay(base.transform.position, rb.velocity, Color.red, 3f);
		Vector3 direction = upcomingPosition - beforePosition;
	}

	private void Start()
	{
		InvokeRepeating("Lasers", 0.5f, 0.3f);
		if (!isEnemyBullet)
		{
			SetColorBullet();
		}
		if (!GameMaster.instance.isZombieMode && !GameMaster.instance.inMenuMode && (bool)EnemyTankScript && (bool)EnemyTankScript.AIscript && (EnemyTankScript.AIscript.isLevel100Boss || EnemyTankScript.AIscript.isLevel70Boss || EnemyTankScript.AIscript.isLevel50Boss || EnemyTankScript.AIscript.isLevel30Boss || EnemyTankScript.AIscript.isLevel10Boss || EnemyTankScript.AIscript.HTscript.IsCustom))
		{
			base.transform.position = new Vector3(base.transform.position.x, 1f, base.transform.position.z);
		}
		if (OptionsMainMenu.instance.showxraybullets)
		{
			base.gameObject.AddComponent<Outline>();
			Outline O = GetComponent<Outline>();
			O.OutlineMode = Outline.Mode.OutlineHidden;
			O.OutlineColor = Color.white;
			O.OutlineWidth = 0.65f;
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
			ParticleSystem PS2 = base.transform.Find("ShinyParticlesBullet").GetComponent<ParticleSystem>();
			if ((bool)PS2)
			{
				PS2.Play();
			}
		}
		else if (isSuperShiny)
		{
			ParticleSystem PS = base.transform.Find("SuperShinyParticlesBullet").GetComponent<ParticleSystem>();
			if ((bool)PS)
			{
				PS.Play();
			}
			MeshRenderer MR = GetComponent<MeshRenderer>();
			if ((bool)MR)
			{
				MR.material.color = Color.yellow;
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
		if (!OptionsMainMenu.instance || IsSackBullet)
		{
			return;
		}
		if (OptionsMainMenu.instance.AMselected.Contains(6))
		{
			int pick2 = Random.Range(0, 5);
			Color newcc2 = Color.red;
			switch (pick2)
			{
			case 0:
				newcc2 = Color.red;
				break;
			case 1:
				newcc2 = Color.blue;
				break;
			case 2:
				newcc2 = Color.green;
				break;
			case 3:
				newcc2 = Color.cyan;
				break;
			case 4:
				newcc2 = Color.yellow;
				break;
			}
			GetComponent<Renderer>().material.color = newcc2;
			SmokeBullet.gameObject.SetActive(value: false);
			SmokeBulletParty.gameObject.SetActive(value: true);
			SmokeBullet = SmokeBulletParty.GetComponent<ParticleSystem>();
		}
		else if (OptionsMainMenu.instance.AMselected.Contains(1025))
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
			GetComponent<Renderer>().material = NeonMaterial;
			GetComponent<Renderer>().material.SetColor("_Color", newcc);
			GetComponent<Renderer>().material.SetColor("_EmissionColor", newcc * 3f);
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
			SFXManager.instance.PlaySFX(ElectricCharge, 1f, null);
		}
	}

	private IEnumerator LateStart()
	{
		DrawReflectionPattern(base.transform.position, StartingVelocity, isDistanceTest: false);
		yield return new WaitForSeconds(1f);
		StartCoroutine(KeepCasting());
		yield return new WaitForSeconds(15f);
		TimesBounced = 9999;
	}

	private IEnumerator CastLaserDelay()
	{
		Debug.Log("VELOCITY IS ZERO!!");
		yield return new WaitForSeconds(0.01f);
		CastLaser(rb.velocity);
	}

	private IEnumerator KeepCasting()
	{
		yield return new WaitForSeconds(0.25f);
		CastLaser(rb.velocity);
		StartCoroutine(KeepCasting());
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
		if (isEnding)
		{
			return;
		}
		DestroyOnImpact = false;
		Vector3 startingPosition = position;
		Ray ray = new Ray(position, direction);
		LayerMask LM = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall")) | (1 << LayerMask.NameToLayer("FLOOR"));
		Debug.DrawRay(position, direction * 15f, Color.cyan, 3f);
		if (Physics.Raycast(ray, out var hit, 1000f, LM))
		{
			if (isDistanceTest)
			{
				float distToNextWall = Vector3.Distance(position, hit.point);
				if (distToNextWall < 0.3f)
				{
					DestroyOnImpact = true;
				}
				Debug.DrawLine(startingPosition, hit.point, Color.blue, 5f);
				return;
			}
			direction = Vector3.Reflect(direction.normalized, hit.normal);
			position = hit.point;
			beforePosition = base.transform.position;
			upcomingPosition = position;
			upcomingDirection = direction;
			WallGonnaBounceInTo = hit.transform.gameObject;
			lookAtPoint = position;
			LookAtPointSet = true;
			Debug.DrawLine(startingPosition, position, Color.blue, 5f);
			if (MaxBounces > 0)
			{
				DrawReflectionPattern(position, upcomingDirection, isDistanceTest: true);
			}
		}
		else if (isDistanceTest)
		{
			DestroyOnImpact = true;
		}
	}

	private void Update()
	{
		if (isEnding)
		{
			return;
		}
		TimeFlying += Time.deltaTime;
		CurrentSpeed = rb.velocity.magnitude;
		if (GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted)
		{
			End(isEndingGame: true);
		}
		if (TimeFlying > MaxTimeFlying)
		{
			End(isEndingGame: false);
		}
		if (MapEditorMaster.instance != null && MapEditorMaster.instance.inPlayingMode && !GameMaster.instance.GameHasStarted)
		{
			End(isEndingGame: true);
		}
		if ((GameMaster.instance.AmountEnemyTanks < 1 || GameMaster.instance.restartGame || !GameMaster.instance.PlayerAlive) && !GameMaster.instance.isZombieMode && !GameMaster.instance.CM && !GameMaster.instance.inTankeyTown && !GameMaster.instance.inMenuMode && !MapEditorMaster.instance)
		{
			if (GameMaster.instance.PlayerAlive && GameMaster.instance.CurrentMission >= 99)
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
		if (isEnding)
		{
			return;
		}
		if (LookAtPointSet)
		{
			Vector3 newvector = (upcomingPosition - beforePosition).normalized * BulletSpeed;
			rb.velocity = newvector;
			if (TimesBounced > 0)
			{
				rb.angularVelocity = Vector3.zero;
			}
		}
		else
		{
			_ = StartingVelocity;
			if (true)
			{
				Debug.LogWarning("no starting velocity");
				rb.velocity = StartingVelocity;
			}
			else if ((bool)WallGonnaBounceInTo)
			{
			}
		}
		lastFrameVelocity = rb.velocity;
		lastPosition = base.transform.position;
		if (rb.velocity != Vector3.zero)
		{
			if (TurningTimeLeft > 0f)
			{
				TurningTimeLeft -= Time.deltaTime;
				Vector3 newDirection = (upcomingPosition = Vector3.Lerp(InitialDirection, TurnTowardsPoint, 1f - TurningTimeLeft));
				Vector3 newvector2 = (upcomingPosition - beforePosition).normalized * BulletSpeed;
				rb.velocity = newvector2;
				base.transform.rotation = Quaternion.LookRotation(rb.velocity, base.transform.up);
				base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
			}
			else
			{
				base.transform.rotation = Quaternion.LookRotation(rb.velocity, base.transform.up);
				base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
			}
		}
		if (WallGonnaBounceInTo == null)
		{
			Debug.LogWarning("casting a new laser");
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
				CameraShake CS = Camera.main.GetComponent<CameraShake>();
				if ((bool)CS)
				{
					CS.StartCoroutine(CS.Shake(0.1f, 0.1f));
				}
				DeadSound();
			}
			GameObject poof = Object.Instantiate(deathExplosion, base.transform.position, Quaternion.identity);
			Object.Destroy(poof.gameObject, 2f);
			Transform PE = base.transform.Find("SmokeBullet");
			if ((bool)PE)
			{
				ParticleSystem PEsystem = PE.GetComponent<ParticleSystem>();
				PEsystem.Stop();
				PE.SetParent(null);
				AudioSource AS2 = PE.gameObject.GetComponent<AudioSource>();
				if ((bool)AS2)
				{
					AS2.volume = 0f;
					AS2.Stop();
				}
			}
			if ((bool)TowerChargeParticles)
			{
				TowerChargeParticles.Stop();
				TowerChargeParticles.transform.parent = null;
			}
			Transform PE2 = base.transform.Find("ShinyParticlesBullet");
			if ((bool)PE2)
			{
				ParticleSystem PEsystem2 = PE2.GetComponent<ParticleSystem>();
				PEsystem2.Stop();
				PE.parent = null;
			}
			if (!isEnemyBullet)
			{
				if (TankScript != null)
				{
					TankScript.firedBullets--;
				}
				else if (EnemyTankScript != null)
				{
					EnemyTankScript.firedBullets--;
				}
			}
			else if ((bool)EnemyTankScript)
			{
				EnemyTankScript.firedBullets--;
			}
			if (PE != null)
			{
				Object.Destroy(PE.gameObject, 5f);
			}
			Object.Destroy(base.gameObject);
		}
		else
		{
			if ((bool)poofParticles)
			{
				GameObject poof2 = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
				poof2.GetComponent<ParticleSystem>().Play();
				Object.Destroy(poof2.gameObject, 3f);
			}
			DeadSound();
			if (!isEnemyBullet)
			{
				if (TankScript != null)
				{
					TankScript.firedBullets--;
				}
				else if (EnemyTankScript != null)
				{
					EnemyTankScript.firedBullets--;
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
			AudioSource AS = SmokeBullet.gameObject.GetComponent<AudioSource>();
			if ((bool)AS)
			{
				AS.volume = 0f;
				AS.Stop();
			}
			Object.Destroy(SmokeBullet.gameObject, 5f);
		}
		Object.Destroy(base.gameObject);
	}

	private void DeadSound()
	{
		if (IsElectricCharged && DeadHitElectric.Length != 0)
		{
			int lengthClips2 = DeadHitElectric.Length;
			int randomPick2 = Random.Range(0, lengthClips2);
			SFXManager.instance.PlaySFX(DeadHitElectric[randomPick2], 1f, null);
		}
		else if (DeadHit.Length != 0)
		{
			int lengthClips = DeadHit.Length;
			int randomPick = Random.Range(0, lengthClips);
			SFXManager.instance.PlaySFX(DeadHit[randomPick], 1f, null);
		}
	}

	private void HitWallSound()
	{
		if (IsElectricCharged)
		{
			int lengthClips2 = WallHitElectro.Length;
			int randomPick2 = Random.Range(0, lengthClips2);
			SFXManager.instance.PlaySFX(WallHitElectro[randomPick2], 1f, null);
		}
		else
		{
			int lengthClips = WallHit.Length;
			int randomPick = Random.Range(0, lengthClips);
			SFXManager.instance.PlaySFX(WallHit[randomPick], 1f, null);
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
		if (collision.gameObject.tag == "Floor" && !isExplosive)
		{
			IgnoreThatCollision(collision);
			return;
		}
		if (collision.gameObject.tag == "Bullet" && !isExplosive && !isSilver)
		{
			PlayerBulletScript PBS = collision.gameObject.GetComponent<PlayerBulletScript>();
			if (PBS != null)
			{
				if (PBS.papaTank == papaTank && PBS.TimesBounced == 0 && TimesBounced == 0 && !IsAirPushed)
				{
					IgnoreThatCollision(collision);
					return;
				}
				if (PBS.IsAirBullet)
				{
					IgnoreThatCollision(collision);
					return;
				}
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
				Debug.LogWarning("No target error stay");
				End(isEndingGame: false);
				return;
			}
		}
		StartCoroutine(WallImpact(collision));
	}

	public void TurnTowardsDirection(Vector3 direction)
	{
		Debug.Log("SETTING TURN TOWARDS!");
		if (TurningTimeLeft <= 0f)
		{
			InitialDirection = rb.velocity;
			TurnTowardsPoint = direction;
			TurningTimeLeft = 1f;
		}
	}

	private bool CheckBullet(Collision collision)
	{
		PlayerBulletScript PBS = collision.gameObject.GetComponent<PlayerBulletScript>();
		if (PBS != null)
		{
			if (GameMaster.instance.CurrentMission == 69 && isBossBullet)
			{
				if (BulletSpeed > 5f)
				{
					BulletSpeed -= 3f;
				}
				return true;
			}
			if (PBS.papaTank == papaTank && TimesBounced == 0 && PBS.TimesBounced == 0 && !IsAirPushed && !PBS.IsAirPushed)
			{
				return true;
			}
			if (PBS.isElectric)
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
			if (IsAirBullet && !PBS.isExplosive)
			{
				PBS.IsAirPushed = true;
				Vector3 direction = upcomingPosition - base.transform.position;
				PBS.TurnTowardsDirection(direction);
				PBS.ResetIgnoredColliders();
				IgnoreThatCollision(collision);
				return true;
			}
			if (PBS.isExplosive || PBS.isSilver)
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
		BombSackScript BSS = collision.gameObject.GetComponent<BombSackScript>();
		if (BSS != null && !IsSackBullet)
		{
			Vector3 direction = new Vector3(BSS.transform.position.x, base.transform.position.y + 1f, BSS.transform.position.z) - base.transform.position;
			float force = (isTowerCharged ? 15f : 11f);
			BSS.HitByBullet(direction, force);
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
		if (collision.gameObject != WallGonnaBounceInTo && (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder" || collision.gameObject.tag == "Floor"))
		{
			if (TimeFlying < 0.05f)
			{
				TimesBounced = 9999;
				yield break;
			}
			float distToMyWall = Vector3.Distance(base.transform.position, upcomingPosition);
			if (distToMyWall < 0.5f)
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
				if (distToMyWall > 1f)
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
		if (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder" || collision.gameObject.tag == "Floor")
		{
			if (collision.gameObject.layer == 16 || collision.gameObject.layer == LayerMask.NameToLayer("NoBounceWall"))
			{
				HitNoBounceWall(collision);
				TimesBounced = 99;
				yield break;
			}
			float distToMyWall2 = Vector3.Distance(base.transform.position, upcomingPosition);
			if (!(distToMyWall2 < 0.5f))
			{
				float amount = ((BulletSpeed < 10f) ? 125f : 175f);
				float waitingTime = distToMyWall2 * (BulletSpeed / amount);
				rb.detectCollisions = false;
				CanBounce = false;
				Collider C = GetComponent<Collider>();
				C.enabled = false;
				yield return new WaitForSeconds(waitingTime);
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
			WeeTurret WT = collision.gameObject.GetComponent<WeeTurret>();
			if (WT != null && GameMaster.instance.GameHasStarted)
			{
				WT.Health--;
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
			MoveTankScript movetank = collision.gameObject.GetComponent<MoveTankScript>();
			EnemyAI aitank = collision.gameObject.GetComponent<EnemyAI>();
			HealthTanks healthscript = collision.gameObject.GetComponent<HealthTanks>();
			if (movetank != null)
			{
				if (movetank.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire)
				{
					if (TankScript != null)
					{
						if (movetank == TankScript.tankMovingScript && (TimesBounced > 0 || IsAirPushed))
						{
							HitTank(collision);
							yield break;
						}
						if (movetank == TankScript.tankMovingScript)
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
					if (movetank == TankScript.tankMovingScript && (TimesBounced > 0 || IsAirPushed))
					{
						HitTank(collision);
						yield break;
					}
					if (movetank == TankScript.tankMovingScript)
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
						SpawnHitTag(healthscript);
					}
				}
				else if (movetank.MyTeam == MyTeam && OptionsMainMenu.instance.FriendlyFire)
				{
					HitTank(collision);
				}
				else if (movetank.MyTeam == MyTeam && !OptionsMainMenu.instance.FriendlyFire)
				{
					TimesBounced = 999;
				}
				else if (movetank.MyTeam != MyTeam)
				{
					HitTank(collision);
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(healthscript);
					}
				}
				else
				{
					HitTank(collision);
				}
			}
			else
			{
				if (!(aitank != null))
				{
					yield break;
				}
				if (aitank.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire && aitank != EnemyTankScript)
				{
					TimesBounced = 999;
				}
				else if ((bool)EnemyTankScript && aitank == EnemyTankScript.AIscript)
				{
					HitTank(collision);
				}
				else if (MyTeam == 0)
				{
					HitTank(collision);
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(healthscript);
					}
				}
				else if (aitank.MyTeam == MyTeam && OptionsMainMenu.instance.FriendlyFire)
				{
					HitTank(collision);
				}
				else if (aitank.MyTeam == MyTeam && !OptionsMainMenu.instance.FriendlyFire)
				{
					TimesBounced = 999;
				}
				else if (aitank.MyTeam != MyTeam)
				{
					if (ShotByPlayer > -1)
					{
						SpawnHitTag(healthscript);
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
			EnemyAI AIscript2 = collision.gameObject.GetComponent<EnemyAI>();
			if ((bool)AIscript2 && AIscript2.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire)
			{
				TimesBounced = 999;
				yield break;
			}
			HealthTanks healthscript2 = collision.gameObject.GetComponent<HealthTanks>();
			if (!GameMaster.instance.isZombieMode)
			{
				if (AIscript2.isTransporting)
				{
					ChargeElectric();
					yield break;
				}
				if (AIscript2.isLevel10Boss || AIscript2.isLevel30Boss || AIscript2.isLevel50Boss || AIscript2.isLevel70Boss)
				{
					if ((bool)healthscript2)
					{
						BossVoiceLines BVL = healthscript2.GetComponent<BossVoiceLines>();
						if ((bool)BVL)
						{
							BVL.PlayHitSound();
						}
					}
					GameObject HealthBar = GameObject.Find("BossHealthBar");
					if ((bool)HealthBar)
					{
						BossHealthBar BHB = HealthBar.GetComponent<BossHealthBar>();
						if ((bool)BHB)
						{
							BHB.StartCoroutine(BHB.MakeBarWhite());
						}
					}
					AchievementsTracker.instance.HasShotBoss = true;
				}
			}
			if ((bool)healthscript2 && !healthscript2.enabled)
			{
				TimesBounced = 999;
				yield break;
			}
			if ((bool)healthscript2.ShieldFade && healthscript2.ShieldFade.ShieldHealth > 0)
			{
				TimesBounced = 9999;
				healthscript2.ShieldFade.StartFade();
				yield break;
			}
			if (isElectric && !IsElectricCharged && (bool)AIscript2)
			{
				AIscript2.StunMe(3f);
				TimesBounced = 9999;
				yield break;
			}
			SFXManager.instance.PlaySFX(enemyKill, 1f, null);
			healthscript2.IsHitByBullet = true;
			if (GameMaster.instance.GameHasStarted)
			{
				healthscript2.DamageMe(1);
			}
			if (healthscript2.health < 1 && !GameMaster.instance.inMapEditor && !GameMaster.instance.isZombieMode && (bool)AccountMaster.instance)
			{
				if (TimesBounced == 0)
				{
					AchievementsTracker.instance.KilledWithoutBounce = true;
					AccountMaster.instance.SaveCloudData(0, healthscript2.EnemyID, 0, bounceKill: false);
				}
				else
				{
					GameMaster.instance.totalKillsBounce++;
					AccountMaster.instance.SaveCloudData(0, healthscript2.EnemyID, 0, bounceKill: true);
				}
			}
			else if (healthscript2.health > 0)
			{
				MakeWhite MW = healthscript2.GetComponent<MakeWhite>();
				if ((bool)MW)
				{
					MW.GotHit();
				}
			}
			if (ShotByPlayer > -1)
			{
				SpawnHitTag(healthscript2);
			}
			TimesBounced = 999;
		}
		else
		{
			if ((!(collision.gameObject.tag == "Enemy") && !(collision.gameObject.tag == "Boss")) || !isEnemyBullet)
			{
				yield break;
			}
			if (TimesBounced == 0 && collision.gameObject == papaTank.gameObject && !IsAirPushed)
			{
				IgnoreThatCollision(collision);
				yield break;
			}
			EnemyAI AIscript = collision.gameObject.GetComponent<EnemyAI>();
			if ((bool)EnemyTankScript && AIscript == EnemyTankScript.AIscript && TimesBounced < 1)
			{
				IgnoreThatCollision(collision);
			}
			else if (MyTeam == 0)
			{
				HitTank(collision);
			}
			else if (AIscript.MyTeam == MyTeam && MyTeam != 0 && OptionsMainMenu.instance.FriendlyFire)
			{
				HitTank(collision);
			}
			else if (AIscript.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire && AIscript.gameObject != papaTank.gameObject)
			{
				if (IsAirBullet)
				{
					HitTank(collision);
				}
				else
				{
					TimesBounced = 999;
				}
			}
			else if (AIscript.MyTeam != MyTeam)
			{
				HitTank(collision);
			}
			else
			{
				HitTank(collision);
			}
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
		GameObject hittag = Object.Instantiate(hittagPrefab, base.transform.position, Quaternion.identity);
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
		Object.Destroy(hittag, 3f);
	}

	private void HitTank(Collision collision)
	{
		if (isElectric)
		{
			if (GameMaster.instance.CurrentMission == 69)
			{
				HealthTanks HT = collision.gameObject.GetComponent<HealthTanks>();
				if ((bool)HT)
				{
					HT.DamageMe(1);
				}
			}
			else
			{
				MoveTankScript movescript = collision.gameObject.GetComponent<MoveTankScript>();
				if (movescript != null)
				{
					movescript.StunMe(3f);
				}
				EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
				if (enemy != null && enemy.HTscript.EnemyID != 15)
				{
					enemy.StunMe(3f);
				}
			}
		}
		else if (IsAirBullet)
		{
			Rigidbody rigi = collision.gameObject.GetComponent<Rigidbody>();
			if ((bool)rigi)
			{
				Vector3 direction = rigi.transform.position - base.transform.position;
				float force = 20f - TimeFlying * 8f;
				if (force > 1f)
				{
					rigi.AddForce(direction * force, ForceMode.Impulse);
				}
				else
				{
					End(isEndingGame: false);
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
			if (GameMaster.instance.isZombieMode && healthscript.health > 0)
			{
				SFXManager.instance.PlaySFX(healthscript.Buzz);
			}
			healthscript.DamageMe(1);
		}
		TimesBounced = 999;
	}

	public void ResetIgnoredColliders()
	{
		if (CollidersIgnoring.Count <= 0)
		{
			return;
		}
		foreach (Collider c in CollidersIgnoring)
		{
			if (c != null)
			{
				Physics.IgnoreCollision(GetComponent<Collider>(), c, ignore: false);
			}
		}
		CollidersIgnoring.Clear();
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
		ResetIgnoredColliders();
		GameObject poof = Object.Instantiate(bounceParticles, base.transform.position, Quaternion.identity);
		poof.GetComponent<ParticleSystem>().Play();
		Object.Destroy(poof.gameObject, 3f);
		rb.velocity = upcomingDirection;
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		rb.angularVelocity = Vector3.zero;
		base.transform.position = upcomingPosition;
		DrawReflectionPattern(upcomingPosition, upcomingDirection, isDistanceTest: false);
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
			ExplosiveBlock EB = col.GetComponent<ExplosiveBlock>();
			BossPlatform BP = col.GetComponent<BossPlatform>();
			BombSackScript BSS = col.GetComponent<BombSackScript>();
			if ((bool)EB)
			{
				if (!EB.isExploding)
				{
					EB.StartCoroutine(EB.Death());
				}
			}
			else if (WT != null)
			{
				WT.Health--;
			}
			else if (enemy != null)
			{
				if (enemy.immuneToExplosion)
				{
					continue;
				}
				if ((bool)enemy.ShieldFade)
				{
					if (enemy.ShieldFade.ShieldHealth > 0)
					{
						enemy.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						enemy.DamageMe(1);
					}
				}
				else
				{
					enemy.DamageMe(1);
				}
			}
			else if (friendbullet != null)
			{
				friendbullet.BounceAmount = 999;
			}
			else if (enemybullet != null)
			{
				if (!enemybullet.isElectric)
				{
					enemybullet.BounceAmount = 999;
				}
			}
			else if (mines != null)
			{
				mines.DetinationTime = 0f;
			}
			else if (DestroyWall != null)
			{
				DestroyWall.StartCoroutine(DestroyWall.destroy());
			}
			else if ((bool)BP)
			{
				BP.PlatformHealth--;
			}
			else if (BSS != null)
			{
				BSS.FlyBack();
			}
		}
		Collider[] bigobjectsInRange = Physics.OverlapSphere(location, radius * 2.2f);
		Collider[] array2 = bigobjectsInRange;
		foreach (Collider col2 in array2)
		{
			Rigidbody rigi = col2.GetComponent<Rigidbody>();
			if (rigi != null && (col2.tag == "Player" || col2.tag == "Enemy"))
			{
				float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
				float force = (radius * 2.2f - distance) * 2f;
				Vector3 direction = rigi.transform.position - base.transform.position;
				rigi.AddForce(direction * force, ForceMode.Impulse);
			}
		}
	}
}
