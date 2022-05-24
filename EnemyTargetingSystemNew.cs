using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyTargetingSystemNew : MonoBehaviour
{
	public EnemyAI AIscript;

	public List<GameObject> Targets = new List<GameObject>();

	public GameObject currentTarget;

	public GameObject bulletPrefab;

	public GameObject SecondBulletPrefab;

	public Transform[] firePoint;

	public List<AudioClip> ShootSound = new List<AudioClip>();

	public AudioClip[] ExtraShotSounds;

	public AudioClip secondShotSound;

	private AudioSource source;

	public Vector3 target;

	public Quaternion rot;

	public float anglerotationcheck;

	public Vector3 startingPosition = Vector3.zero;

	private Quaternion originalRotation;

	[Header("New Shoot Mechanism")]
	public bool CanSpawnBullet = true;

	public bool canShoot;

	public float ShootCountdown;

	public bool specialMoveLockedIn;

	public int firedBullets;

	public int maxFiredBullets = 5;

	public float FirstShotDelay;

	public bool CanRotateHead = true;

	public bool IgnoreCork;

	public bool canMine;

	public bool canMineOnlyNearCork;

	public float mineCountdown;

	public GameObject minePrefab;

	public Transform[] rocketSlotLocations;

	public List<int> rocketSlots;

	public GameObject rocketPrefab;

	public List<GameObject> rockets;

	public float RocketAccuracy;

	[HideInInspector]
	public int maxReflectionCount = 2;

	public float maxStepDistance = 200f;

	private int lvl30BossSpecial = 6;

	private bool canReloadRockets;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private Vector3 lastFrameVelocity;

	[Header("MainMenu Properties")]
	public bool CanGetNewTarget = true;

	public bool IsJustLookingAround;

	public bool isHuntingEnemies;

	public bool isTeamBlue;

	public bool isTeamRed;

	public bool specialMove;

	public bool LookingAtTarget;

	public bool normalLockedIn;

	public bool TargetInSight;

	public bool CR_running;

	public bool CFP_running;

	public int SR_running;

	public bool turning;

	public bool turningLeft;

	private Quaternion rotation;

	public float angleSize;

	public bool doubleStopper;

	[Range(0.5f, 5f)]
	public float myTurnSpeed = 1f;

	public float LookAngleOffset;

	private float pick;

	private Quaternion angleTarget;

	private float originalLvl30TurnSpeed;

	private bool isCheckingForPlayer;

	private Quaternion startRot;

	public float timeTakenDuringLerp = 1f;

	private float _timeStartedLerping;

	private bool changedTargetedPlayer;

	private float specialMoveBounceCooldown = 2f;

	public ParticleSystem ElectricCharge;

	public bool LMCrunning;

	public Animator TankTopAnimator;

	private LayerMask myLayerMasks;

	[Header("Custom Tank SHIT")]
	public int AmountShotgunShots;

	public int CustomShootSpeed = -1;

	public TemporaryRotation RotateOnFire;

	private bool ScriptDisabled;

	private float SecretTimer;

	private int prevKnownEnemies;

	public bool PlayerInsightRunning;

	public bool DeflectingBullet;

	public float RocketReloadSpeed = -1f;

	public void SetLayerMasks()
	{
		if ((bool)bulletPrefab)
		{
			if (IgnoreCork || bulletPrefab.name.Contains("plosive"))
			{
				myLayerMasks = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("EnemyBorder")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
			}
			else
			{
				myLayerMasks = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("EnemyBorder")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
			}
		}
		else
		{
			myLayerMasks = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("EnemyBorder")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
		}
	}

	private void Start()
	{
		rot = Quaternion.Euler(0f, 0f, 0f);
		if (AIscript.IsCompanion)
		{
			mineCountdown = 1.5f;
		}
		else
		{
			mineCountdown = AIscript.LayMinesSpeed + Random.Range(-0.4f, AIscript.LayMinesSpeed / 2f);
		}
		source = GetComponent<AudioSource>();
		SetLayerMasks();
		if (AIscript.HTscript.IsAirdropped)
		{
			StartCoroutine(LatePlayerCheck());
		}
		originalLvl30TurnSpeed = AIscript.lvl30BossTurnSpeed;
		originalRotation = base.transform.rotation;
		if (OptionsMainMenu.instance.currentDifficulty == 3)
		{
			AIscript.isSmart = true;
		}
	}

	private IEnumerator LatePlayerCheck()
	{
		yield return new WaitForSeconds(0.2f);
		SearchPlayers();
	}

	private void StartInvokes()
	{
		float repeatRate = ((AIscript.difficulty < 2) ? 0.9f : 0.35f) + Random.Range(-0.1f, 0.1f);
		if (!PlayerInsightRunning)
		{
			InvokeRepeating("PlayerInsight", 0.3f, repeatRate);
		}
		if (AIscript.isLevel30Boss)
		{
			InvokeRepeating("Changelvl30BossTurnSpeed", 0.5f, 1f);
		}
		if (AIscript.LayMines && !LMCrunning)
		{
			InvokeRepeating("LayMineCheck", 0.4f, 0.4f);
		}
		if ((AIscript.BouncyBullets || AIscript.isSmart) && !isCheckingForPlayer)
		{
			float repeatRate2 = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 0.025f : 0.01f);
			if (!CFP_running)
			{
				InvokeRepeating("CheckForPlayer", 0.1f, repeatRate2);
			}
			isCheckingForPlayer = true;
			maxReflectionCount = AIscript.amountOfBounces;
		}
	}

	private void OnEnable()
	{
		ScriptDisabled = false;
		if (AIscript.IsCompanion)
		{
			mineCountdown = 1.5f;
		}
		else
		{
			mineCountdown = AIscript.LayMinesSpeed + Random.Range(-0.4f, AIscript.LayMinesSpeed);
		}
		CR_running = false;
		SR_running = 0;
		specialMove = false;
		SetLayerMasks();
		if (SR_running == 0 && AIscript.hasRockets)
		{
			for (int i = 0; i < rocketSlots.Count; i++)
			{
				if (rocketSlots[i] == 0)
				{
					rocketSlots[i] = 1;
					GameObject gameObject = Object.Instantiate(rocketPrefab, rocketSlotLocations[i]);
					gameObject.GetComponent<RocketScript>().isHuntingEnemies = isHuntingEnemies;
					gameObject.GetComponent<RocketScript>().papaTank = AIscript.gameObject;
					rockets[i] = gameObject;
					gameObject.transform.parent = rocketSlotLocations[i];
				}
			}
			SR_running++;
			StartCoroutine(ShootRockets());
		}
		StartInvokes();
		firedBullets = 0;
		SearchPlayers();
		ShootCountdown = 0f;
	}

	private void OnDisable()
	{
		DisableThis();
	}

	private void DisableThis()
	{
		ScriptDisabled = true;
		if (SR_running > 0)
		{
			SR_running--;
			StopCoroutine(ShootRockets());
		}
		CFP_running = false;
		isCheckingForPlayer = false;
		PlayerInsightRunning = false;
		CR_running = false;
		CancelInvoke();
		ShootCountdown = 0f;
	}

	private void CheckTargetsList()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject target in Targets)
		{
			if (target == null || !target.activeSelf)
			{
				list.Add(target);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (GameObject item in list)
		{
			Targets.Remove(item);
		}
	}

	private Vector3 CalculateEnemyPosition(GameObject TargetObject)
	{
		Rigidbody component = currentTarget.GetComponent<Rigidbody>();
		Vector3 vector = Vector3.forward;
		if (component.velocity.magnitude > 0.5f)
		{
			float num = Vector3.Distance(currentTarget.transform.position, base.transform.position);
			EnemyAI component2 = currentTarget.GetComponent<EnemyAI>();
			if ((bool)component2)
			{
				vector = ((!component2.DrivingBackwards) ? (currentTarget.transform.position + currentTarget.transform.forward * (num / 6f)) : (currentTarget.transform.position + -currentTarget.transform.forward * (num / 6f)));
			}
			else
			{
				MoveTankScript component3 = currentTarget.GetComponent<MoveTankScript>();
				if ((bool)component3)
				{
					vector = ((!component3.DrivingBackwards) ? (currentTarget.transform.position + currentTarget.transform.forward * (num / 6f)) : (currentTarget.transform.position + -currentTarget.transform.forward * (num / 6f)));
				}
			}
			Debug.DrawLine(vector, base.transform.position, Color.blue);
			return vector - base.transform.position;
		}
		return currentTarget.transform.position - base.transform.position;
	}

	private void Update()
	{
		if (IsJustLookingAround)
		{
			if (specialMove || normalLockedIn)
			{
				return;
			}
			if (!turning)
			{
				rotation = Quaternion.LookRotation(base.transform.forward);
				pick = Random.Range(-50f, 50f) + LookAngleOffset;
				rotation *= Quaternion.Euler(0f, pick, 0f);
				angleSize = Quaternion.Angle(base.transform.rotation, rotation);
				if (!(angleSize > 1f))
				{
					return;
				}
				turning = true;
				normalLockedIn = false;
				timeTakenDuringLerp = angleSize / 180f * (5.5f - myTurnSpeed);
				if (timeTakenDuringLerp < 0.5f)
				{
					if (IsJustLookingAround)
					{
						timeTakenDuringLerp = Random.Range(0.5f, 1.5f);
					}
					else if (AIscript.isAggro || AIscript.isAggressive || AIscript.ShootSpeed < 0.2f)
					{
						timeTakenDuringLerp = 0.2f;
					}
					else
					{
						timeTakenDuringLerp = 0.5f;
					}
				}
				startRot = base.transform.rotation;
				_timeStartedLerping = Time.time;
			}
			else
			{
				float t = (Time.time - _timeStartedLerping) / timeTakenDuringLerp;
				if (!AIscript.isStunned)
				{
					base.transform.rotation = Quaternion.Lerp(startRot, rotation, t);
				}
				if (Quaternion.Angle(base.transform.rotation, rotation) <= 0.6f)
				{
					LookingAtTarget = true;
					normalLockedIn = true;
					StartCoroutine(AutoUnlock());
					base.transform.rotation = rotation;
					StartCoroutine(SetTurnFalse());
				}
			}
			return;
		}
		if (GameMaster.instance.restartGame)
		{
			base.transform.rotation = originalRotation;
			ShootCountdown = AIscript.ShootSpeed + Random.Range(0f, AIscript.ShootSpeed / 2f);
			mineCountdown = AIscript.LayMinesSpeed + Random.Range(0f, AIscript.LayMinesSpeed);
		}
		if (GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted)
		{
			if (!ScriptDisabled)
			{
				DisableThis();
			}
			return;
		}
		if (GameMaster.instance.isOfficialCampaign && AIscript.MyTeam == 1)
		{
			isHuntingEnemies = true;
		}
		if (firedBullets >= maxFiredBullets)
		{
			SecretTimer += Time.deltaTime;
			if (SecretTimer >= 12f)
			{
				firedBullets = 0;
			}
		}
		else
		{
			SecretTimer = 0f;
		}
		int amountCalledInTanks = GameMaster.instance.AmountCalledInTanks;
		if ((Targets.Count < 1 || amountCalledInTanks != prevKnownEnemies) && CanGetNewTarget)
		{
			prevKnownEnemies = amountCalledInTanks;
			SearchPlayers();
			return;
		}
		if (firedBullets < 0)
		{
			firedBullets = 0;
		}
		if (ShootCountdown <= 0f)
		{
			canShoot = true;
		}
		else
		{
			canShoot = false;
		}
		if (AIscript.isElectric && AIscript.isLevel70Boss)
		{
			if (ShootCountdown <= -2f)
			{
				ShootCountdown = 3.5f;
			}
			if (ShootCountdown < 1.1f && ElectricCharge.isStopped)
			{
				ElectricCharge.Clear();
				ElectricCharge.Play();
				if (!AIscript.isTransporting && OptionsMainMenu.instance.currentDifficulty < 2)
				{
					AIscript.TankSpeed = AIscript.OriginalTankSpeed / 2f;
				}
			}
			else if (ShootCountdown >= 1.1f && ElectricCharge.isPlaying)
			{
				ElectricCharge.Stop();
				ElectricCharge.Clear();
				AIscript.TankSpeed = AIscript.OriginalTankSpeed;
				AIscript.CanMove = true;
			}
		}
		if (mineCountdown <= 0f)
		{
			canMine = true;
		}
		else
		{
			canMine = false;
		}
		mineCountdown -= Time.deltaTime;
		ShootCountdown -= Time.deltaTime;
		specialMoveBounceCooldown -= Time.deltaTime;
		if ((AIscript.HTscript != null && AIscript.HTscript.health < 1) || !CanRotateHead)
		{
			return;
		}
		if ((firePoint.Length < 4 || (firePoint.Length < 12 && !AIscript.isLevel30Boss)) && (currentTarget != null || AIscript.isLevel30Boss))
		{
			if (!specialMove && !normalLockedIn)
			{
				if (!turning)
				{
					Vector3 vector = (AIscript.CalculateEnemyPosition ? CalculateEnemyPosition(currentTarget) : (currentTarget.transform.position - base.transform.position));
					vector.y = 0f;
					if (!(vector != Vector3.zero))
					{
						return;
					}
					rotation = Quaternion.LookRotation(vector);
					float num = AIscript.Accuracy;
					if (currentTarget != null)
					{
						EnemyAI component = currentTarget.GetComponent<EnemyAI>();
						if ((bool)component && component.isInvisible)
						{
							num += 15f;
						}
					}
					pick = Random.Range(0f - num, num) + LookAngleOffset;
					rotation *= Quaternion.Euler(0f, pick, 0f);
					angleSize = Quaternion.Angle(base.transform.rotation, rotation);
					if (angleSize > 1f)
					{
						turning = true;
						normalLockedIn = false;
						timeTakenDuringLerp = angleSize / 180f * (5.5f - myTurnSpeed);
						if (timeTakenDuringLerp < 0.5f)
						{
							if (AIscript.isAggro || AIscript.isAggressive || AIscript.ShootSpeed < 0.2f)
							{
								timeTakenDuringLerp = 0.2f;
							}
							else
							{
								timeTakenDuringLerp = 0.5f;
							}
						}
						startRot = base.transform.rotation;
						_timeStartedLerping = Time.time;
					}
					else
					{
						LookingAtTarget = true;
						normalLockedIn = true;
						StartCoroutine(AutoUnlock());
					}
				}
				else
				{
					float t2 = (Time.time - _timeStartedLerping) / timeTakenDuringLerp;
					if (!AIscript.isStunned)
					{
						base.transform.rotation = Quaternion.Lerp(startRot, rotation, t2);
					}
					if (Quaternion.Angle(base.transform.rotation, rotation) <= 0.6f)
					{
						LookingAtTarget = true;
						normalLockedIn = true;
						StartCoroutine(AutoUnlock());
						base.transform.rotation = rotation;
						StartCoroutine("SetTurnFalse");
					}
				}
			}
			else
			{
				if (!specialMove)
				{
					return;
				}
				angleSize = Quaternion.Angle(base.transform.rotation, rotation);
				if (!turning && !specialMoveLockedIn)
				{
					Debug.LogError("angle size of: " + angleSize);
					turning = true;
					_timeStartedLerping = Time.time;
					specialMoveLockedIn = false;
				}
				else
				{
					if (specialMoveLockedIn)
					{
						return;
					}
					if (TargetInSight && AIscript.CanMove)
					{
						turning = false;
						specialMoveLockedIn = false;
						specialMove = false;
						if (AIscript.TankSpeed != AIscript.OriginalTankSpeed && !AIscript.isAggro)
						{
							AIscript.TankSpeed = AIscript.OriginalTankSpeed;
						}
					}
					else if (angleSize <= 0.05f)
					{
						specialMoveLockedIn = true;
						startingPosition = Vector3.zero;
						base.transform.rotation = rotation;
						if (AIscript.CanMove && AIscript.TankSpeed != AIscript.OriginalTankSpeed && !AIscript.isAggro)
						{
							AIscript.TankSpeed = AIscript.OriginalTankSpeed;
						}
						turning = false;
					}
					else
					{
						float t3 = (Time.time - _timeStartedLerping) / timeTakenDuringLerp;
						if (!AIscript.isStunned)
						{
							base.transform.rotation = Quaternion.Lerp(startRot, rotation, t3);
						}
					}
				}
			}
		}
		else if (!AIscript.isStunned)
		{
			base.transform.rotation *= Quaternion.Euler(0f, Time.deltaTime * AIscript.lvl30BossTurnSpeed, 0f);
		}
	}

	private IEnumerator AutoUnlock()
	{
		yield return new WaitForSeconds(0.2f);
		normalLockedIn = false;
	}

	private void Changelvl30BossTurnSpeed()
	{
		AIscript.lvl30BossTurnSpeed = originalLvl30TurnSpeed + (float)Random.Range(-15, 15);
	}

	public IEnumerator SetTurnFalse()
	{
		float seconds = 0.5f - myTurnSpeed / 10f;
		yield return new WaitForSeconds(seconds);
		turning = false;
	}

	private void SearchPlayers()
	{
		if (!CanGetNewTarget)
		{
			return;
		}
		Targets.Clear();
		GameObject[] array;
		if ((bool)MapEditorMaster.instance)
		{
			array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject in array)
			{
				EnemyAI component = gameObject.GetComponent<EnemyAI>();
				if ((bool)component && gameObject.gameObject != AIscript.gameObject && (component.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(gameObject);
				}
			}
			array = GameObject.FindGameObjectsWithTag("Boss");
			foreach (GameObject gameObject2 in array)
			{
				EnemyAI component2 = gameObject2.GetComponent<EnemyAI>();
				if ((bool)component2 && gameObject2.gameObject != AIscript.gameObject && (component2.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(gameObject2);
				}
			}
			array = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject gameObject3 in array)
			{
				MoveTankScript component3 = gameObject3.GetComponent<MoveTankScript>();
				if ((bool)component3)
				{
					if (component3.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0)
					{
						Targets.Add(gameObject3);
					}
					continue;
				}
				EnemyAI component4 = gameObject3.GetComponent<EnemyAI>();
				if ((bool)component4 && gameObject3.gameObject != AIscript.gameObject && (component4.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(gameObject3);
				}
			}
			return;
		}
		if (isHuntingEnemies || GameMaster.instance.inMenuMode || AIscript.MyTeam == 1)
		{
			array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject gameObject4 in array)
			{
				EnemyAI component5 = gameObject4.GetComponent<EnemyAI>();
				if ((bool)component5 && gameObject4.gameObject != AIscript.gameObject && (component5.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(gameObject4);
				}
			}
			array = GameObject.FindGameObjectsWithTag("Boss");
			foreach (GameObject gameObject5 in array)
			{
				if (gameObject5.gameObject != AIscript.gameObject)
				{
					Targets.Add(gameObject5);
				}
			}
			return;
		}
		array = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject item in array)
		{
			Targets.Add(item);
		}
		array = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject gameObject6 in array)
		{
			EnemyAI component6 = gameObject6.GetComponent<EnemyAI>();
			if ((bool)component6 && gameObject6.gameObject != AIscript.gameObject && component6.MyTeam != AIscript.MyTeam)
			{
				Targets.Add(gameObject6);
			}
		}
	}

	public void PlayerInsight()
	{
		PlayerInsightRunning = true;
		if ((GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted) || !AIscript.enabled)
		{
			return;
		}
		if ((AIscript.hasRockets || AIscript.isLevel30Boss) && !CR_running && GameMaster.instance.GameHasStarted)
		{
			StartCoroutine("ShootAtPlayer", true);
		}
		if (changedTargetedPlayer)
		{
			return;
		}
		CheckTargetsList();
		Targets = Targets.OrderBy((GameObject x) => Vector3.Distance(base.transform.position, x.transform.position)).ToList();
		if ((bool)currentTarget && TargetInSight)
		{
			HealthTanks component = currentTarget.GetComponent<HealthTanks>();
			if ((bool)component && component.dying)
			{
				LookingAtTarget = false;
				TargetInSight = false;
				currentTarget = null;
				return;
			}
			EnemyAI component2 = currentTarget.GetComponent<EnemyAI>();
			if ((bool)component2)
			{
				if (component2.isInvisible)
				{
					AIscript.ShootSpeed = AIscript.OriginalShootSpeed * 1.5f;
				}
				else
				{
					AIscript.ShootSpeed = AIscript.OriginalShootSpeed;
				}
			}
			RaycastHit hitInfo;
			if (Targets.Count > 0 && Targets[0] != currentTarget)
			{
				float num = Vector3.Distance(base.transform.position, Targets[0].transform.position);
				float num2 = Vector3.Distance(base.transform.position, currentTarget.transform.position);
				if (num < num2 - 5f)
				{
					Vector3 vector = Targets[0].transform.position - base.transform.position;
					Debug.DrawRay(base.transform.position, vector * 50f, Color.green, 0.5f);
					vector += new Vector3(0f, 0.5f, 0f);
					if (Physics.Raycast(base.transform.position, vector, out hitInfo, 50f, myLayerMasks) && hitInfo.collider.gameObject == Targets[0])
					{
						TargetInSight = true;
						currentTarget = Targets[0];
						LookingAtTarget = false;
						normalLockedIn = false;
						return;
					}
				}
			}
			Vector3 vector2 = currentTarget.transform.position - base.transform.position;
			vector2 += new Vector3(0f, 0.5f, 0f);
			Debug.DrawRay(base.transform.position, vector2 * 50f, Color.green, 0.1f);
			if (Physics.Raycast(base.transform.position, vector2, out hitInfo, 50f, myLayerMasks))
			{
				if (hitInfo.collider.gameObject == currentTarget)
				{
					TargetInSight = true;
				}
				else
				{
					TargetInSight = false;
				}
			}
		}
		else if (Targets.Count > 0)
		{
			LookingAtTarget = false;
			if (!currentTarget)
			{
				TargetInSight = false;
			}
			GameObject gameObject = CheckTargetsIfInSight(myLayerMasks);
			if ((bool)gameObject)
			{
				LookingAtTarget = false;
				normalLockedIn = false;
				specialMoveLockedIn = false;
				TargetInSight = false;
				StartCoroutine(StartTheShooting(gameObject));
			}
			if (!TargetInSight && currentTarget == null)
			{
				currentTarget = Targets[0];
				StartCoroutine(CooldownTarget(1f));
			}
		}
	}

	private GameObject CheckTargetsIfInSight(LayerMask layerMask)
	{
		List<GameObject> list = new List<GameObject>();
		list.Clear();
		foreach (GameObject target in Targets)
		{
			Vector3 vector = target.transform.position - base.transform.position;
			vector += new Vector3(0f, 0.5f, 0f);
			Debug.DrawRay(base.transform.position, vector * 50f, Color.yellow, 0.1f);
			if (!Physics.Raycast(base.transform.position, vector, out var hitInfo, float.PositiveInfinity, layerMask))
			{
				continue;
			}
			if ((bool)MapEditorMaster.instance)
			{
				if (hitInfo.collider.tag == "Enemy" || hitInfo.collider.tag == "Boss")
				{
					EnemyAI component = hitInfo.collider.GetComponent<EnemyAI>();
					if ((bool)component && (component.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
					{
						list.Add(target);
					}
				}
				else
				{
					if (!(hitInfo.collider.tag == "Player"))
					{
						continue;
					}
					MoveTankScript component2 = hitInfo.collider.GetComponent<MoveTankScript>();
					if ((bool)component2)
					{
						if (component2.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0)
						{
							list.Add(target);
						}
						continue;
					}
					EnemyAI component3 = hitInfo.collider.GetComponent<EnemyAI>();
					if ((bool)component3 && (component3.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
					{
						list.Add(target);
					}
				}
				continue;
			}
			EnemyAI component4 = hitInfo.collider.GetComponent<EnemyAI>();
			bool flag = false;
			if ((bool)component4 && component4.MyTeam != AIscript.MyTeam)
			{
				flag = true;
			}
			if (isHuntingEnemies || AIscript.MyTeam == 1 || flag)
			{
				if (hitInfo.collider.tag == "Enemy" || hitInfo.collider.tag == "Boss")
				{
					list.Add(target);
				}
			}
			else if (!isHuntingEnemies && hitInfo.collider.tag == "Player")
			{
				if (IgnoreCork && EnemiesNear(hitInfo.transform.position, 4))
				{
					currentTarget = target;
					StartCoroutine(CooldownTarget(1f));
					return null;
				}
				HealthTanks component5 = hitInfo.transform.gameObject.GetComponent<HealthTanks>();
				if (!component5 || !component5.dying)
				{
					list.Add(target);
				}
			}
		}
		if (list.Count > 0)
		{
			list = list.OrderBy((GameObject x) => Vector3.Distance(base.transform.position, x.transform.position)).ToList();
			return list[0];
		}
		return null;
	}

	public IEnumerator StartTheShooting(GameObject Target)
	{
		yield return new WaitForSeconds(FirstShotDelay);
		TargetInSight = true;
		currentTarget = Target;
		StartCoroutine(CooldownTarget(1f));
		if (!CR_running)
		{
			StartCoroutine("ShootAtPlayer", true);
		}
	}

	private IEnumerator CooldownTarget(float sec)
	{
		changedTargetedPlayer = true;
		yield return new WaitForSeconds(sec);
		changedTargetedPlayer = false;
	}

	public void LayMineCheck()
	{
		LMCrunning = true;
		if ((GameMaster.instance.inMapEditor && !GameMaster.instance.GameHasStarted) || !AIscript.enabled || !GameMaster.instance.GameHasStarted || !AIscript.enabled || !canMine || !AIscript.LayMines || ((!GameMaster.instance.GameHasStarted || GameMaster.instance.GameHasPaused) && !isHuntingEnemies))
		{
			return;
		}
		if (AIscript.CheckStuckByMines(toLayMine: true))
		{
			Debug.Log("Mine too close to lay new one!");
		}
		else if (canMine && !canMineOnlyNearCork)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 2.5f);
			foreach (Collider collider in array)
			{
				if (collider.GetComponent<DestroyableWall>() != null && collider.gameObject.layer == LayerMask.NameToLayer("CorkWall"))
				{
					PlaceMine(deathmine: false);
					return;
				}
			}
			if (!EnemiesNear(base.transform.position, 6) && currentTarget != null && Vector3.Distance(base.transform.position, currentTarget.transform.position) < 16f)
			{
				PlaceMine(deathmine: false);
			}
		}
		else
		{
			if (!canMine || !canMineOnlyNearCork || EnemiesNear(base.transform.position, 5))
			{
				return;
			}
			Collider[] array = Physics.OverlapSphere(base.transform.position, 2.5f);
			for (int i = 0; i < array.Length; i++)
			{
				DestroyableWall component = array[i].GetComponent<DestroyableWall>();
				if (component != null && !component.IsStone)
				{
					PlaceMine(deathmine: false);
				}
			}
		}
	}

	public void PlaceMine(bool deathmine)
	{
		if (Random.Range(0, 2) == 1 || deathmine)
		{
			GameObject gameObject = Object.Instantiate(minePrefab, base.transform.position + new Vector3(0f, -0.65f, 0f), Quaternion.identity);
			MineScript component = gameObject.GetComponent<MineScript>();
			component.isHuntingEnemies = isHuntingEnemies;
			component.placedByEnemies = true;
			if ((bool)GameMaster.instance.CM)
			{
				gameObject.transform.SetParent(GameMaster.instance.CM.transform);
			}
			else
			{
				gameObject.transform.parent = null;
			}
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, -0.02f, gameObject.transform.position.z);
			canMine = false;
			component.MyPlacer = AIscript.gameObject;
			mineCountdown = Random.Range(1f, 2.5f) + AIscript.LayMinesSpeed;
			if (deathmine)
			{
				component.StartCoroutine(component.setactive());
			}
		}
	}

	public void CheckForPlayer()
	{
		if (!GameMaster.instance.GameHasStarted || !AIscript.enabled)
		{
			return;
		}
		if (!CFP_running)
		{
			CFP_running = true;
		}
		isCheckingForPlayer = true;
		if (AIscript.IncomingBullets.Count > 0 && AIscript.isSmart && !specialMove)
		{
			foreach (GameObject incomingBullet in AIscript.IncomingBullets)
			{
				if (!incomingBullet)
				{
					continue;
				}
				float num = Vector3.Distance(base.transform.position, incomingBullet.transform.position);
				if (((num < 8f && TargetInSight) || (num < 12f && !TargetInSight)) && !specialMove && !specialMove && canShoot && !specialMoveLockedIn)
				{
					startingPosition = incomingBullet.transform.position;
					startingPosition = incomingBullet.transform.position + -incomingBullet.transform.up;
					startingPosition = new Vector3(startingPosition.x, base.transform.position.y, startingPosition.z);
					TurnTowardsSpecialMove();
					AIscript.IncomingBullets.Remove(incomingBullet);
					DeflectingBullet = true;
					if (!CR_running)
					{
						StartCoroutine(ShootAtPlayer());
					}
					return;
				}
			}
		}
		if ((TargetInSight && AIscript.HTscript.EnemyID != 6) || !AIscript.BouncyBullets || !(specialMoveBounceCooldown < 0.5f) || specialMove || !canShoot || specialMoveLockedIn)
		{
			return;
		}
		startingPosition = Vector3.zero;
		rot *= Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		if (!DrawReflectionPattern(base.transform.position, rot * Vector3.forward, maxReflectionCount, CheckingForFriendlies: false))
		{
			rot *= Quaternion.Euler(0f, 2.8f, 0f);
		}
		else
		{
			if ((!isHuntingEnemies && !GameMaster.instance.GameHasStarted) || specialMove)
			{
				return;
			}
			if (CheckForEverything(rot, 0.2f, FriendliesCheck: false, AIscript.amountOfBounces))
			{
				rot *= Quaternion.Euler(0f, 3f, 0f);
				return;
			}
			if (CheckForEverything(rot, 0.9f, FriendliesCheck: true, AIscript.amountOfBounces))
			{
				rot *= Quaternion.Euler(0f, 3f, 0f);
				return;
			}
			TurnTowardsSpecialMove();
			if (!CR_running)
			{
				StartCoroutine("ShootAtPlayer", true);
			}
		}
	}

	private void TurnTowardsSpecialMove()
	{
		startRot = base.transform.rotation;
		_timeStartedLerping = Time.time;
		angleSize = Quaternion.Angle(base.transform.rotation, rot);
		if (AIscript.CanMove && !AIscript.isAggro)
		{
			AIscript.TankSpeed /= 2f;
			specialMoveBounceCooldown += 3f;
		}
		timeTakenDuringLerp = angleSize / 180f * (5.1f - myTurnSpeed);
		specialMove = true;
		turning = true;
		specialMoveLockedIn = false;
		if (startingPosition != Vector3.zero)
		{
			Vector3 forward = startingPosition - base.transform.position;
			rotation = Quaternion.LookRotation(forward);
		}
	}

	private bool DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining, bool CheckingForFriendlies)
	{
		int num = 0;
		float num2 = -1f;
		do
		{
			bool flag = false;
			Vector3 start = position;
			Ray ray = new Ray(position, direction);
			LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
			RaycastHit[] array = (from h in Physics.RaycastAll(ray, 100f, layerMask)
				orderby h.distance
				select h).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (CheckingForFriendlies)
				{
					Debug.DrawLine(start, raycastHit.point, Color.yellow, 1f);
				}
				else
				{
					Debug.DrawLine(start, raycastHit.point, Color.red);
				}
				if (raycastHit.collider.gameObject == AIscript.gameObject && num == 0)
				{
					continue;
				}
				if ((raycastHit.collider.tag == "Player" || raycastHit.collider.tag == "HitZone") && !isHuntingEnemies && raycastHit.collider.transform.parent.gameObject != AIscript.gameObject && raycastHit.collider.gameObject != AIscript.gameObject)
				{
					MoveTankScript component = raycastHit.transform.GetComponent<MoveTankScript>();
					if (component == null && raycastHit.collider.tag == "HitZone")
					{
						component = raycastHit.transform.parent.GetChild(0).transform.GetComponent<MoveTankScript>();
					}
					if ((bool)component)
					{
						if (!(raycastHit.collider.tag == "HitZone"))
						{
							return false;
						}
						if (component.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && !CheckingForFriendlies)
						{
							return false;
						}
						if (component.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && CheckingForFriendlies)
						{
							return true;
						}
					}
					else
					{
						EnemyAI component2 = raycastHit.transform.GetComponent<EnemyAI>();
						if ((bool)component2)
						{
							if (component2.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && !CheckingForFriendlies)
							{
								return false;
							}
							if (component2.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && CheckingForFriendlies)
							{
								return true;
							}
						}
					}
					HealthTanks component3 = raycastHit.transform.gameObject.GetComponent<HealthTanks>();
					if (component3 != null && component3.dying)
					{
						return false;
					}
					if (num2 < 0f && reflectionsRemaining < 1)
					{
						return false;
					}
					flag = true;
					if (num2 != -1f && AIscript.CanMove)
					{
						Vector3 point = raycastHit.point;
						float num3 = Vector3.Distance(base.transform.position, point);
						if (num2 < num3)
						{
							return false;
						}
					}
				}
				if ((raycastHit.collider.tag == "Enemy" || raycastHit.collider.tag == "Boss") && raycastHit.collider.gameObject != AIscript.gameObject)
				{
					EnemyAI component4 = raycastHit.transform.GetComponent<EnemyAI>();
					if (component4.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && !CheckingForFriendlies && component4.gameObject != AIscript.gameObject)
					{
						return false;
					}
					if (component4.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && CheckingForFriendlies && component4.gameObject != AIscript.gameObject)
					{
						return true;
					}
					if (num2 != -1f)
					{
						Vector3 point2 = raycastHit.point;
						float num4 = Vector3.Distance(position, point2);
						if (num2 < num4 && num2 < 9f)
						{
							return false;
						}
					}
					if (!CheckingForFriendlies)
					{
						return true;
					}
					return false;
				}
				if ((raycastHit.collider.tag == "Enemy" || raycastHit.collider.tag == "Boss") && raycastHit.collider.gameObject == AIscript.gameObject && !AIscript.CanMove && CheckingForFriendlies)
				{
					return true;
				}
				if (raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") && !CheckingForFriendlies)
				{
					return false;
				}
				if (raycastHit.collider.tag == "Solid" || raycastHit.collider.tag == "MapBorder")
				{
					if (reflectionsRemaining == maxReflectionCount && startingPosition == Vector3.zero)
					{
						startingPosition = raycastHit.point;
					}
					direction = Vector3.Reflect(direction, raycastHit.normal);
					position = raycastHit.point;
					num2 = Vector3.Distance(base.transform.position, position);
				}
				num2 = Vector3.Distance(base.transform.position, position);
				if (((num2 < 6f && AIscript.CanMove) || (num2 < 1f && !AIscript.CanMove)) && reflectionsRemaining == maxReflectionCount && !CheckingForFriendlies)
				{
					return false;
				}
				if (raycastHit.collider.tag == "Solid" || raycastHit.collider.tag == "MapBorder")
				{
					break;
				}
			}
			if (flag && !CheckingForFriendlies)
			{
				return true;
			}
			num++;
			reflectionsRemaining--;
		}
		while (reflectionsRemaining >= 0);
		return false;
	}

	public void ShootAllBarrels()
	{
		int num = maxFiredBullets - firedBullets;
		if (num <= 0)
		{
			return;
		}
		if (AmountShotgunShots > 0)
		{
			if (AmountShotgunShots == 1 && num >= 1)
			{
				firedBullets++;
				FireTank(firePoint[0], bulletPrefab);
			}
			else if (AmountShotgunShots == 2 && num >= 2)
			{
				firedBullets += 2;
				FireTank(firePoint[1], bulletPrefab);
				FireTank(firePoint[2], bulletPrefab);
			}
			else if (AmountShotgunShots == 3 && num >= 3)
			{
				firedBullets += 3;
				FireTank(firePoint[0], bulletPrefab);
				FireTank(firePoint[1], bulletPrefab);
				FireTank(firePoint[2], bulletPrefab);
			}
			else if (AmountShotgunShots == 4 && num >= 4)
			{
				firedBullets += 4;
				FireTank(firePoint[1], bulletPrefab);
				FireTank(firePoint[2], bulletPrefab);
				FireTank(firePoint[3], bulletPrefab);
				FireTank(firePoint[4], bulletPrefab);
			}
			else
			{
				if (AmountShotgunShots != 5 || num < 5)
				{
					return;
				}
				firedBullets += 5;
				for (int i = 0; i < 5; i++)
				{
					if (i == 0)
					{
						FireTank(firePoint[i], bulletPrefab);
					}
					else
					{
						FireTank(firePoint[i], bulletPrefab);
					}
				}
			}
		}
		else if (num >= firePoint.Length)
		{
			firedBullets += firePoint.Length;
			Transform[] array = firePoint;
			foreach (Transform firepoint in array)
			{
				FireTank(firepoint, bulletPrefab);
			}
		}
	}

	private IEnumerator ShootAtPlayer()
	{
		if (!CR_running)
		{
			CR_running = true;
			if (AIscript.amountOfBounces != 2 && !specialMove && !AIscript.isAggro)
			{
				yield return new WaitForSeconds(Random.Range(0.05f, AIscript.ShootSpeed / 2f));
			}
		}
		CR_running = true;
		if (GameMaster.instance.AmountGoodTanks < 1 && !isHuntingEnemies && !GameMaster.instance.CM && !GameMaster.instance.inMenuMode && !MapEditorMaster.instance && !TankeyTownMaster.instance)
		{
			yield break;
		}
		if (AIscript.isTransporting || AIscript.HTscript.health < 1)
		{
			if (AIscript.isAggro)
			{
				yield return new WaitForSeconds(0.01f);
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
			}
			StartCoroutine("ShootAtPlayer", false);
			yield break;
		}
		if (firePoint.Length < 2 || (firePoint.Length < 12 && !AIscript.isLevel30Boss))
		{
			if (specialMove && canShoot && specialMoveLockedIn)
			{
				if (CheckForEverything(base.transform.rotation, 0.6f, FriendliesCheck: true, AIscript.amountOfBounces) && !isHuntingEnemies && !DeflectingBullet)
				{
					if (AIscript.isAggro)
					{
						yield return new WaitForSeconds(0.01f);
					}
					else
					{
						yield return new WaitForSeconds(0.1f);
					}
					StartCoroutine("ShootAtPlayer");
					specialMoveLockedIn = false;
					if (AIscript.CanMove && !AIscript.isAggro && AIscript.TankSpeed != AIscript.OriginalTankSpeed)
					{
						AIscript.TankSpeed = AIscript.OriginalTankSpeed;
					}
					specialMove = false;
					DeflectingBullet = false;
					specialMoveBounceCooldown += 1f;
					yield break;
				}
				startingPosition = Vector3.zero;
				if (firedBullets >= maxFiredBullets)
				{
					yield return new WaitForSeconds(0.05f);
					StartCoroutine(ShootAtPlayer());
					yield break;
				}
				ShootAllBarrels();
				DeflectingBullet = false;
				specialMove = false;
				normalLockedIn = false;
				specialMoveLockedIn = false;
			}
			else if (TargetInSight && !specialMove && canShoot && LookingAtTarget)
			{
				int reflections = ((AIscript.amountOfBounces > 0) ? 1 : 0);
				if (CheckForEverything(base.transform.rotation, 0.6f, FriendliesCheck: true, reflections))
				{
					if (AIscript.isAggro)
					{
						yield return new WaitForSeconds(0.01f);
					}
					else
					{
						yield return new WaitForSeconds(0.1f);
					}
					StartCoroutine("ShootAtPlayer");
					normalLockedIn = false;
					yield break;
				}
				normalLockedIn = false;
				Debug.DrawRay(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, Color.green, 2f);
				LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("OneWayBlock")) | (1 << LayerMask.NameToLayer("EnemyBorder")));
				if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var hitInfo, float.PositiveInfinity, layerMask))
				{
					if (hitInfo.collider.tag == "Solid" || hitInfo.collider.tag == "MapBorder" || hitInfo.collider.tag == "Mine")
					{
						if (Vector3.Distance(base.transform.position, hitInfo.point) > 5f)
						{
							if (firedBullets >= maxFiredBullets)
							{
								yield return new WaitForSeconds(0.05f);
								StartCoroutine(ShootAtPlayer());
								yield break;
							}
							ShootAllBarrels();
						}
					}
					else
					{
						if (firedBullets >= maxFiredBullets)
						{
							yield return new WaitForSeconds(0.05f);
							StartCoroutine(ShootAtPlayer());
							yield break;
						}
						ShootAllBarrels();
					}
				}
			}
		}
		else if (canShoot)
		{
			if (AIscript.HTscript.health < AIscript.lvl30Boss3modeLives && lvl30BossSpecial < 8)
			{
				for (int i = 0; i < 8; i++)
				{
					FireTank(firePoint[i], SecondBulletPrefab);
				}
				if (lvl30BossSpecial > 1)
				{
					lvl30BossSpecial--;
				}
				else
				{
					lvl30BossSpecial = 12;
				}
			}
			else
			{
				for (int j = 0; j < firePoint.Length; j++)
				{
					FireTank(firePoint[j], bulletPrefab);
				}
				if (AIscript.HTscript.health < AIscript.lvl30Boss3modeLives)
				{
					lvl30BossSpecial--;
				}
			}
		}
		yield return new WaitForSeconds(0.02f);
		StartCoroutine(ShootAtPlayer());
	}

	private bool CheckForEverything(Quaternion checkAngle, float checkOffset, bool FriendliesCheck, int reflections)
	{
		Vector3 position = base.transform.position + (checkAngle * Vector3.left).normalized * checkOffset;
		Vector3 position2 = base.transform.position + (checkAngle * Vector3.right).normalized * checkOffset;
		bool flag = DrawReflectionPattern(position, checkAngle * Vector3.forward, reflections, FriendliesCheck);
		bool flag2 = DrawReflectionPattern(position2, checkAngle * Vector3.forward, reflections, FriendliesCheck);
		if (FriendliesCheck)
		{
			if (flag || flag2)
			{
				return true;
			}
		}
		else if (!flag || !flag2)
		{
			return true;
		}
		return false;
	}

	public void AddRocket(int index)
	{
		GameObject gameObject = Object.Instantiate(rocketPrefab, rocketSlotLocations[index]);
		gameObject.GetComponent<RocketScript>().isHuntingEnemies = isHuntingEnemies;
		gameObject.GetComponent<RocketScript>().papaTank = AIscript.gameObject;
		gameObject.GetComponent<RocketScript>().MyTeam = AIscript.MyTeam;
		if (AIscript.isInvisible)
		{
			foreach (Transform item in gameObject.transform)
			{
				item.gameObject.SetActive(value: false);
				foreach (Transform item2 in item)
				{
					item2.gameObject.SetActive(value: false);
				}
			}
		}
		rockets[index] = gameObject;
		gameObject.transform.parent = rocketSlotLocations[index];
	}

	private IEnumerator ShootRockets()
	{
		if (SR_running > 1 || !AIscript.hasRockets)
		{
			yield break;
		}
		float seconds = Random.Range(2f, 2.5f);
		if (RocketReloadSpeed > 0f)
		{
			seconds = RocketReloadSpeed;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 1)
		{
			seconds = Random.Range(1.5f, 2f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 2)
		{
			seconds = Random.Range(1f, 1.5f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 3)
		{
			seconds = Random.Range(0.75f, 1f);
		}
		yield return new WaitForSeconds(seconds);
		if (!GameMaster.instance.GameHasStarted || GameMaster.instance.GameHasPaused)
		{
			yield break;
		}
		for (int i = 0; i < rocketSlots.Count; i++)
		{
			if (rocketSlots[i] == 0 && canReloadRockets)
			{
				rocketSlots[i] = 1;
				AddRocket(i);
				if (i + 1 >= rocketSlots.Count)
				{
					continue;
				}
				if (RocketReloadSpeed > 0f)
				{
					seconds = RocketReloadSpeed / 2f;
				}
				else
				{
					seconds = Random.Range(0.4f, 0.9f);
					if (OptionsMainMenu.instance.currentDifficulty == 1)
					{
						seconds = Random.Range(0.3f, 0.8f);
					}
					else if (OptionsMainMenu.instance.currentDifficulty == 2)
					{
						seconds = Random.Range(0.2f, 0.7f);
					}
					else if (OptionsMainMenu.instance.currentDifficulty == 3)
					{
						seconds = Random.Range(0.1f, 0.6f);
					}
				}
				yield return new WaitForSeconds(seconds);
			}
			else
			{
				if (rocketSlots[i] != 1)
				{
					continue;
				}
				rockets[i].GetComponent<RocketScript>().isHuntingEnemies = isHuntingEnemies;
				rockets[i].GetComponent<RocketScript>().papaTank = AIscript.gameObject;
				rockets[i].GetComponent<RocketScript>().MyTeam = AIscript.MyTeam;
				rockets[i].GetComponent<RocketScript>().Launch();
				rocketSlots[i] = 0;
				rockets[i] = null;
				if (i + 1 < rocketSlots.Count)
				{
					if (RocketReloadSpeed > 0f)
					{
						seconds = RocketReloadSpeed / 2f;
					}
					else
					{
						seconds = Random.Range(0.4f, 0.9f);
						if (OptionsMainMenu.instance.currentDifficulty == 1)
						{
							seconds = Random.Range(0.3f, 0.8f);
						}
						else if (OptionsMainMenu.instance.currentDifficulty == 2)
						{
							seconds = Random.Range(0.2f, 0.7f);
						}
						else if (OptionsMainMenu.instance.currentDifficulty == 3)
						{
							seconds = Random.Range(0.1f, 0.6f);
						}
					}
					yield return new WaitForSeconds(seconds);
				}
				else
				{
					canReloadRockets = false;
					StartCoroutine("reloadRocketsTimer");
				}
			}
		}
		StartCoroutine("ShootRockets");
	}

	private IEnumerator reloadRocketsTimer()
	{
		float seconds = Random.Range(4f, 5f);
		if (RocketReloadSpeed > 0f)
		{
			seconds = RocketReloadSpeed;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 1)
		{
			seconds = Random.Range(3.5f, 4f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 2)
		{
			seconds = Random.Range(3f, 3.5f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 3)
		{
			seconds = Random.Range(2.5f, 3f);
		}
		yield return new WaitForSeconds(seconds);
		canReloadRockets = true;
	}

	public void FireTank(Transform firepoint, GameObject bulletprefab)
	{
		if (!CanSpawnBullet)
		{
			return;
		}
		if (doubleStopper && !AIscript.isLevel30Boss && firePoint.Length < 2)
		{
			Debug.LogError("Doublestopper kicked in");
			return;
		}
		if ((!GameMaster.instance.GameHasStarted || GameMaster.instance.AmountGoodTanks < 1) && !isHuntingEnemies && !GameMaster.instance.CM && !GameMaster.instance.inMenuMode && !MapEditorMaster.instance && !TankeyTownMaster.instance)
		{
			Debug.LogError("End game stopping Bullet Firing");
			return;
		}
		canShoot = false;
		if (!DeflectingBullet)
		{
			ShootCountdown = AIscript.ShootSpeed + Random.Range(0f, AIscript.ShootSpeed / 2f);
		}
		else
		{
			ShootCountdown = AIscript.ShootSpeed - Random.Range(0f, AIscript.ShootSpeed / 2f);
		}
		if (AIscript.HTscript.IsCustom)
		{
			ShootCountdown = AIscript.ShootSpeed;
		}
		if ((bool)RotateOnFire)
		{
			RotateOnFire.RotateFor(0.5f);
		}
		DeflectingBullet = false;
		if (!firepoint)
		{
			firepoint = firePoint[0];
		}
		if (!bulletprefab)
		{
			bulletprefab = bulletPrefab;
		}
		if ((bool)TankTopAnimator)
		{
			TankTopAnimator.Play("ShootingTank", -1, 0f);
			if ((bool)AIscript)
			{
				if (AIscript.CanMove)
				{
					AIscript.rigi.AddForce(-base.transform.forward * 4f, ForceMode.Impulse);
				}
				else
				{
					AIscript.rigi.AddForce(-base.transform.forward * 2f, ForceMode.Impulse);
				}
			}
		}
		bool flag = false;
		GameObject gameObject;
		if (AIscript.isElectric && !AIscript.HTscript.IsCustom)
		{
			if (AIscript.isCharged || AIscript.isLevel70Boss)
			{
				gameObject = Object.Instantiate(bulletprefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
				if (ExtraShotSounds.Length != 0)
				{
					int num = Random.Range(0, ExtraShotSounds.Length);
					SFXManager.instance.PlaySFX(ExtraShotSounds[num]);
					flag = true;
				}
				else
				{
					SFXManager.instance.PlaySFX(ShootSound);
					flag = true;
				}
			}
			else
			{
				gameObject = Object.Instantiate(SecondBulletPrefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
				SFXManager.instance.PlaySFX(secondShotSound);
				flag = true;
			}
		}
		else
		{
			if (ExtraShotSounds.Length != 0)
			{
				int num2 = Random.Range(0, ExtraShotSounds.Length);
				SFXManager.instance.PlaySFX(ExtraShotSounds[num2]);
				flag = true;
			}
			else if (AIscript.HTscript.EnemyID == -1)
			{
				if (bulletprefab.name == "EnemyExplosiveBullet")
				{
					SFXManager.instance.PlaySFX(secondShotSound);
				}
				flag = true;
			}
			gameObject = Object.Instantiate(bulletprefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
		}
		if (!flag)
		{
			SFXManager.instance.PlaySFX(ShootSound);
		}
		PlayerBulletScript component = gameObject.GetComponent<PlayerBulletScript>();
		component.MyTeam = AIscript.MyTeam;
		if (AIscript.isShiny)
		{
			component.isShiny = true;
		}
		if (AIscript.isSuperShiny)
		{
			component.isSuperShiny = true;
		}
		Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
		if ((bool)component2)
		{
			component2.AddForce(firepoint.forward * 6f);
			component.StartingVelocity = firepoint.forward * 6f;
			_ = firepoint.position;
			Ray ray = new Ray(firepoint.position, firepoint.forward);
			LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall"));
			Debug.DrawRay(firepoint.position, firepoint.forward * 15f, Color.cyan, 3f);
			if (Physics.Raycast(ray, out var hitInfo, 1000f, layerMask))
			{
				component.WallGonnaBounceInTo = hitInfo.transform.gameObject;
			}
		}
		if (!AIscript.IsCompanion)
		{
			component.MaxBounces = AIscript.amountOfBounces;
			component.papaTank = AIscript.gameObject;
			component.EnemyTankScript = this;
			component.isEnemyBullet = true;
		}
		else
		{
			component.MaxBounces = AIscript.amountOfBounces;
			component.papaTank = AIscript.gameObject;
			component.ShotByPlayer = AIscript.CompanionID;
			component.EnemyTankScript = this;
		}
		if (CustomShootSpeed > -1)
		{
			component.BulletSpeed = CustomShootSpeed;
		}
		if (bulletprefab == SecondBulletPrefab && AIscript.isLevel30Boss)
		{
			component.MaxBounces = 1;
		}
		Physics.IgnoreCollision(component.GetComponent<Collider>(), AIscript.GetComponent<Collider>());
		component.CollidersIgnoring.Add(AIscript.GetComponent<Collider>());
		doubleStopper = true;
		StartCoroutine("preventDouble");
	}

	private IEnumerator preventDouble()
	{
		yield return new WaitForSeconds(0.1f);
		doubleStopper = false;
		yield return new WaitForSeconds(0.05f);
	}

	public bool EnemiesNear(Vector3 position, int range)
	{
		Collider[] array = Physics.OverlapSphere(position, range);
		foreach (Collider collider in array)
		{
			if (((collider.tag == "Enemy" && !isHuntingEnemies) || (collider.tag == "Player" && isHuntingEnemies)) && collider.gameObject != AIscript.gameObject)
			{
				return true;
			}
			if (collider.tag == "Player" && AIscript.IsCompanion && collider.gameObject != AIscript.gameObject)
			{
				return true;
			}
		}
		return false;
	}
}
