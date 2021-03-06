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

	public bool IsElectricCharged;

	public bool isExplosive;

	public bool isElectric;

	public bool isBossBullet;

	public bool IsAirBullet;

	public bool IsKingTankBullet;

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

	public bool isSuperShiny;

	public bool isSilver;

	[HideInInspector]
	public bool IsSideBullet;

	[Header("Custom Bullets")]
	public GameObject Dart;

	public AudioClip[] DartShootSound;

	private float TimeFlying;

	public Vector3 upcomingDirection;

	public Vector3 upcomingPosition;

	public Vector3 beforePosition;

	public bool DestroyOnImpact;

	public bool LookAtPointSet;

	public float MaxTimeFlying = 30f;

	public bool IsAirPushed;

	private bool isEnding;

	public List<Collider> CollidersIgnoring = new List<Collider>();

	public float TurningTimeLeft;

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
		if (AccountMaster.instance.Inventory != null && AccountMaster.instance.Inventory.InventoryItems.Count > 0 && AccountMaster.instance.Inventory.InventoryItems.Contains(26) && OptionsMainMenu.instance.AMselected.Contains(1026))
		{
			InstantiateOneParticle component = GetComponent<InstantiateOneParticle>();
			if ((bool)component)
			{
				component.Sound = DartShootSound;
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
		_ = upcomingPosition - beforePosition;
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
				BulletSpeed *= 1.2f;
			}
			else if (OptionsMainMenu.instance.currentDifficulty == 2)
			{
				BulletSpeed *= 1.3f;
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
		else if (isSuperShiny)
		{
			ParticleSystem component3 = base.transform.Find("SuperShinyParticlesBullet").GetComponent<ParticleSystem>();
			if ((bool)component3)
			{
				component3.Play();
			}
			MeshRenderer component4 = GetComponent<MeshRenderer>();
			if ((bool)component4)
			{
				component4.material.color = Color.yellow;
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
		else if (OptionsMainMenu.instance.AMselected.Contains(1025))
		{
			int num2 = Random.Range(0, 5);
			Color color2 = Color.red;
			switch (num2)
			{
			case 0:
				color2 = Color.red;
				break;
			case 1:
				color2 = Color.blue;
				break;
			case 2:
				color2 = Color.green;
				break;
			case 3:
				color2 = Color.cyan;
				break;
			case 4:
				color2 = Color.yellow;
				break;
			}
			GetComponent<Renderer>().material = NeonMaterial;
			GetComponent<Renderer>().material.SetColor("_Color", color2);
			GetComponent<Renderer>().material.SetColor("_EmissionColor", color2 * 3f);
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
		DrawReflectionPattern(base.transform.position, StartingVelocity, isDistanceTest: false, OnlyWallTest: false);
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
			DrawReflectionPattern(base.transform.position, velocity, isDistanceTest: false, OnlyWallTest: false);
		}
	}

	private void DrawReflectionPattern(Vector3 position, Vector3 direction, bool isDistanceTest, bool OnlyWallTest)
	{
		if (isEnding)
		{
			return;
		}
		DestroyOnImpact = false;
		Vector3 start = position;
		Ray ray = new Ray(position, direction);
		LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall")) | (1 << LayerMask.NameToLayer("FLOOR"));
		Debug.DrawRay(position, direction * 15f, Color.cyan, 3f);
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
			if (MaxBounces > 0 && !OnlyWallTest)
			{
				DrawReflectionPattern(position, upcomingDirection, isDistanceTest: true, OnlyWallTest);
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
			Vector3 velocity = (upcomingPosition - beforePosition).normalized * BulletSpeed;
			rb.velocity = velocity;
			if (TimesBounced > 0)
			{
				rb.angularVelocity = Vector3.zero;
			}
		}
		else
		{
			_ = StartingVelocity;
			rb.velocity = StartingVelocity;
		}
		lastFrameVelocity = rb.velocity;
		lastPosition = base.transform.position;
		if (rb.velocity != Vector3.zero)
		{
			if (TurningTimeLeft > 0f)
			{
				TurningTimeLeft -= Time.deltaTime;
				Vector3 vector = (upcomingPosition = Vector3.Lerp(InitialDirection, TurnTowardsPoint, 1f - TurningTimeLeft));
				Vector3 vector2 = (upcomingPosition - beforePosition).normalized * BulletSpeed;
				rb.velocity = vector2;
				base.transform.rotation = Quaternion.LookRotation(rb.velocity, base.transform.up);
				base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
				DrawReflectionPattern(base.transform.position, vector2, isDistanceTest: false, OnlyWallTest: true);
			}
			else
			{
				base.transform.rotation = Quaternion.LookRotation(rb.velocity, base.transform.up);
				base.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);
			}
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
				transform.SetParent(null);
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
				else if (EnemyTankScript != null)
				{
					EnemyTankScript.firedBullets--;
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
			return;
		}
		if ((bool)poofParticles)
		{
			GameObject gameObject = ((!IsElectricCharged) ? Object.Instantiate(poofParticles, base.transform.position, Quaternion.identity) : Object.Instantiate(poofParticlesElectro, base.transform.position, Quaternion.identity));
			gameObject.GetComponent<ParticleSystem>().Play();
			Object.Destroy(gameObject.gameObject, 3f);
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
		if (IsElectricCharged && DeadHitElectric.Length != 0)
		{
			int maxExclusive = DeadHitElectric.Length;
			int num = Random.Range(0, maxExclusive);
			SFXManager.instance.PlaySFX(DeadHitElectric[num], 1f, null);
		}
		else if (DeadHit.Length != 0)
		{
			int maxExclusive2 = DeadHit.Length;
			int num2 = Random.Range(0, maxExclusive2);
			SFXManager.instance.PlaySFX(DeadHit[num2], 1f, null);
		}
	}

	private void HitWallSound()
	{
		if (IsElectricCharged)
		{
			int maxExclusive = WallHitElectro.Length;
			int num = Random.Range(0, maxExclusive);
			SFXManager.instance.PlaySFX(WallHitElectro[num], 1f, null);
		}
		else
		{
			int maxExclusive2 = WallHit.Length;
			int num2 = Random.Range(0, maxExclusive2);
			SFXManager.instance.PlaySFX(WallHit[num2], 1f, null);
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
			PlayerBulletScript component = collision.gameObject.GetComponent<PlayerBulletScript>();
			if (component != null)
			{
				if (component.papaTank == papaTank && component.TimesBounced == 0 && TimesBounced == 0 && !IsAirPushed)
				{
					IgnoreThatCollision(collision);
					return;
				}
				if (component.IsAirBullet)
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
		if (TurningTimeLeft <= 0f)
		{
			InitialDirection = rb.velocity;
			TurnTowardsPoint = direction;
			TurningTimeLeft = 1f;
		}
	}

	private bool CheckBullet(Collision collision)
	{
		PlayerBulletScript component = collision.gameObject.GetComponent<PlayerBulletScript>();
		if (component != null)
		{
			if (isBossBullet && EnemyTankScript != null && EnemyTankScript.AIscript != null)
			{
				if (EnemyTankScript.AIscript.HTscript.EnemyID == 170 && BulletSpeed > 5f)
				{
					BulletSpeed -= 3f;
				}
				return true;
			}
			if (component.papaTank == papaTank && TimesBounced == 0 && component.TimesBounced == 0 && !IsAirPushed && !component.IsAirPushed)
			{
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
			if (IsAirBullet && !component.isExplosive)
			{
				component.IsAirPushed = true;
				Vector3 direction = upcomingPosition - base.transform.position;
				component.TurnTowardsDirection(direction);
				component.ResetIgnoredColliders();
				IgnoreThatCollision(collision);
				return true;
			}
			if (component.isExplosive || component.isSilver || component.IsKingTankBullet)
			{
				return false;
			}
			if (isExplosive || isSilver || IsKingTankBullet)
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
		if (collision.transform.parent != null && collision.transform.parent.parent != null)
		{
			SlotMachine component2 = collision.transform.parent.parent.GetComponent<SlotMachine>();
			if ((bool)component2)
			{
				component2.GetShot();
			}
		}
		if (component != null && !IsSackBullet)
		{
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
		if (collision.gameObject != WallGonnaBounceInTo && (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder" || collision.gameObject.tag == "Floor"))
		{
			if (TimeFlying < 0.05f)
			{
				TimesBounced = 9999;
				yield break;
			}
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
		if (collision.gameObject.tag == "Solid" || collision.gameObject.tag == "MapBorder" || collision.gameObject.tag == "Floor")
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
			if (collision != null)
			{
				PreviousCollision = collision.gameObject;
				StartCoroutine(ResetBounce());
				Bounce();
			}
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
			else
			{
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
						if (component2 == TankScript.tankMovingScript && (TimesBounced > 0 || IsAirPushed))
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
					if (component2 == TankScript.tankMovingScript && (TimesBounced > 0 || IsAirPushed))
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
				else if ((bool)EnemyTankScript && component3 == EnemyTankScript.AIscript)
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
			HealthTanks component6 = collision.gameObject.GetComponent<HealthTanks>();
			if (!GameMaster.instance.isZombieMode)
			{
				if (component5.isTransporting)
				{
					ChargeElectric();
					yield break;
				}
				if (component5.isLevel10Boss || component5.isLevel30Boss || component5.isLevel50Boss || component5.isLevel70Boss)
				{
					_ = (bool)component6;
					GameObject gameObject = GameObject.Find("BossHealthBar");
					if ((bool)gameObject)
					{
						BossHealthBar component7 = gameObject.GetComponent<BossHealthBar>();
						if ((bool)component7)
						{
							component7.StartCoroutine(component7.MakeBarWhite());
						}
					}
					AchievementsTracker.instance.HasShotBoss = true;
				}
			}
			if ((bool)component6 && !component6.enabled)
			{
				TimesBounced = 999;
				yield break;
			}
			if ((bool)component6.ShieldFade && component6.ShieldFade.ShieldHealth > 0)
			{
				TimesBounced = 9999;
				component6.ShieldFade.StartFade();
				yield break;
			}
			if (isElectric && !IsElectricCharged && (bool)component5)
			{
				component5.StunMe(3f);
				TimesBounced = 9999;
				yield break;
			}
			SFXManager.instance.PlaySFX(enemyKill, 1f, null);
			component6.IsHitByBullet = true;
			if (GameMaster.instance.GameHasStarted)
			{
				component6.DamageMe(1);
			}
			if (component6.health < 1 && !GameMaster.instance.inMapEditor && !MapEditorMaster.instance && !GameMaster.instance.isZombieMode && (bool)AccountMaster.instance)
			{
				if (TimesBounced == 0)
				{
					AchievementsTracker.instance.KilledWithoutBounce = true;
					AccountMaster.instance.SaveCloudData(0, component6.EnemyID, 0, bounceKill: false, 0f);
				}
				else
				{
					GameMaster.instance.totalKillsBounce++;
					AccountMaster.instance.SaveCloudData(0, component6.EnemyID, 0, bounceKill: true, 0f);
				}
			}
			else if (component6.health > 0)
			{
				MakeWhite component8 = component6.GetComponent<MakeWhite>();
				if ((bool)component8)
				{
					component8.GotHit();
				}
			}
			if (ShotByPlayer > -1)
			{
				SpawnHitTag(component6);
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
			EnemyAI component9 = collision.gameObject.GetComponent<EnemyAI>();
			if ((bool)EnemyTankScript && component9 == EnemyTankScript.AIscript && TimesBounced < 1 && !IsAirPushed)
			{
				IgnoreThatCollision(collision);
				yield break;
			}
			HealthTanks component10 = collision.gameObject.GetComponent<HealthTanks>();
			if ((bool)component10 && component10.health > 0 && (component9.MyTeam != MyTeam || OptionsMainMenu.instance.FriendlyFire))
			{
				MakeWhite component11 = component10.GetComponent<MakeWhite>();
				if ((bool)component11)
				{
					component11.GotHit();
				}
			}
			if (MyTeam == 0)
			{
				HitTank(collision);
			}
			else if (component9.MyTeam == MyTeam && MyTeam != 0 && OptionsMainMenu.instance.FriendlyFire)
			{
				HitTank(collision);
			}
			else if (component9.MyTeam == MyTeam && MyTeam != 0 && !OptionsMainMenu.instance.FriendlyFire && component9.gameObject != papaTank.gameObject)
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
			else
			{
				_ = component9.MyTeam;
				_ = MyTeam;
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
			bool flag = false;
			if (EnemyTankScript != null && EnemyTankScript.AIscript != null && EnemyTankScript.AIscript.HTscript.EnemyID == 170)
			{
				HealthTanks component = collision.gameObject.GetComponent<HealthTanks>();
				if ((bool)component)
				{
					component.DamageMe(1);
					flag = true;
				}
			}
			if (!flag)
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
		else if (IsAirBullet)
		{
			Rigidbody component4 = collision.gameObject.GetComponent<Rigidbody>();
			if ((bool)component4)
			{
				Vector3 vector = component4.transform.position - base.transform.position;
				float num = 20f - TimeFlying * 8f;
				if (num > 1f)
				{
					component4.AddForce(vector * num, ForceMode.Impulse);
				}
				else
				{
					End(isEndingGame: false);
				}
			}
		}
		else
		{
			EnemyAI component5 = collision.gameObject.GetComponent<EnemyAI>();
			if (component5 != null && component5.isTransporting)
			{
				return;
			}
			HealthTanks component6 = collision.gameObject.GetComponent<HealthTanks>();
			if (GameMaster.instance.isZombieMode && component6.health > 0)
			{
				SFXManager.instance.PlaySFX(component6.Buzz);
			}
			component6.DamageMe(1);
		}
		TimesBounced = 999;
	}

	public void ResetIgnoredColliders()
	{
		if (CollidersIgnoring.Count <= 0)
		{
			return;
		}
		foreach (Collider item in CollidersIgnoring)
		{
			if (item != null)
			{
				Physics.IgnoreCollision(GetComponent<Collider>(), item, ignore: false);
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
		else if ((bool)AchievementsTracker.instance && !isEnemyBullet)
		{
			AchievementsTracker.instance.HasMissed = true;
		}
		TimesBounced++;
		ResetIgnoredColliders();
		GameObject obj = Object.Instantiate(bounceParticles, base.transform.position, Quaternion.identity);
		obj.GetComponent<ParticleSystem>().Play();
		Object.Destroy(obj.gameObject, 3f);
		rb.velocity = upcomingDirection;
		lastFrameVelocity = rb.velocity;
		lastAngularVelocity = rb.angularVelocity;
		rb.angularVelocity = Vector3.zero;
		base.transform.position = upcomingPosition;
		DrawReflectionPattern(upcomingPosition, upcomingDirection, isDistanceTest: false, OnlyWallTest: false);
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
			BossPlatform component8 = obj.GetComponent<BossPlatform>();
			BombSackScript component9 = obj.GetComponent<BombSackScript>();
			if ((bool)component7)
			{
				if (!component7.isExploding)
				{
					component7.StartCoroutine(component7.Death());
				}
			}
			else if (component6 != null)
			{
				component6.Health--;
			}
			else if (component != null)
			{
				if (component.immuneToExplosion)
				{
					continue;
				}
				if ((bool)component.ShieldFade)
				{
					if (component.ShieldFade.ShieldHealth > 0)
					{
						component.ShieldFade.ShieldHealth = 0;
					}
					else
					{
						component.DamageMe(1);
					}
				}
				else
				{
					component.DamageMe(1);
				}
			}
			else if (component2 != null)
			{
				component2.BounceAmount = 999;
			}
			else if (component3 != null)
			{
				if (!component3.isElectric)
				{
					component3.BounceAmount = 999;
				}
			}
			else if (component4 != null)
			{
				component4.DetinationTime = 0f;
			}
			else if (component5 != null)
			{
				component5.StartCoroutine(component5.destroy());
			}
			else if ((bool)component8)
			{
				component8.PlatformHealth--;
			}
			else if (component9 != null)
			{
				component9.FlyBack();
			}
		}
		array = Physics.OverlapSphere(location, radius * 2.2f);
		foreach (Collider collider in array)
		{
			Rigidbody component10 = collider.GetComponent<Rigidbody>();
			if (component10 != null && (collider.tag == "Player" || collider.tag == "Enemy"))
			{
				float num = Vector3.Distance(component10.transform.position, base.transform.position);
				float num2 = (radius * 2.2f - num) * 2f;
				Vector3 vector = component10.transform.position - base.transform.position;
				component10.AddForce(vector * num2, ForceMode.Impulse);
			}
		}
	}
}
