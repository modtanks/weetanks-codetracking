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

	public float anglerotationcheck = 0f;

	public Vector3 startingPosition = Vector3.zero;

	private Quaternion originalRotation;

	[Header("New Shoot Mechanism")]
	public bool canShoot = false;

	public float ShootCountdown;

	public bool specialMoveLockedIn = false;

	public int firedBullets = 0;

	public int maxFiredBullets = 5;

	public float FirstShotDelay = 0f;

	public bool IgnoreCork = false;

	public bool canMine = false;

	public bool canMineOnlyNearCork = false;

	public float mineCountdown = 0f;

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

	private bool canReloadRockets = false;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private Vector3 lastFrameVelocity;

	[Header("MainMenu Properties")]
	public bool isHuntingEnemies;

	public bool isTeamBlue;

	public bool isTeamRed;

	public bool specialMove = false;

	public bool LookingAtTarget = false;

	public bool normalLockedIn = false;

	public bool TargetInSight;

	public bool CR_running = false;

	public bool CFP_running = false;

	public int SR_running = 0;

	public bool turning = false;

	public bool turningLeft = false;

	private Quaternion rotation;

	public float angleSize;

	public bool doubleStopper = false;

	[Range(0.5f, 5f)]
	public float myTurnSpeed = 1f;

	public float LookAngleOffset = 0f;

	private float pick;

	private Quaternion angleTarget;

	private float originalLvl30TurnSpeed;

	private bool isCheckingForPlayer = false;

	private Quaternion startRot;

	public float timeTakenDuringLerp = 1f;

	private float _timeStartedLerping;

	private bool changedTargetedPlayer = false;

	private float specialMoveBounceCooldown = 2f;

	public ParticleSystem ElectricCharge;

	public bool LMCrunning = false;

	public Animator TankTopAnimator;

	private LayerMask myLayerMasks;

	[Header("Custom Tank SHIT")]
	public int AmountShotgunShots = 0;

	public int CustomShootSpeed = -1;

	public TemporaryRotation RotateOnFire;

	private bool ScriptDisabled = false;

	private float SecretTimer = 0f;

	private int prevKnownEnemies = 0;

	public bool PlayerInsightRunning = false;

	public bool DeflectingBullet = false;

	public float RocketReloadSpeed = -1f;

	public void SetLayerMasks()
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
		float playerchecker = ((AIscript.difficulty < 2) ? 0.8f : 0.25f);
		if (!PlayerInsightRunning)
		{
			InvokeRepeating("PlayerInsight", 0.3f, playerchecker);
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
			float BounceDetectionSpeed = ((OptionsMainMenu.instance.currentDifficulty == 0) ? 0.025f : 0.01f);
			if (!CFP_running)
			{
				InvokeRepeating("CheckForPlayer", 0.1f, BounceDetectionSpeed);
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
					GameObject newRocket = Object.Instantiate(rocketPrefab, rocketSlotLocations[i]);
					newRocket.GetComponent<RocketScript>().isHuntingEnemies = isHuntingEnemies;
					newRocket.GetComponent<RocketScript>().papaTank = AIscript.gameObject;
					rockets[i] = newRocket;
					newRocket.transform.parent = rocketSlotLocations[i];
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
		List<GameObject> TargetsToRemove = new List<GameObject>();
		foreach (GameObject Target2 in Targets)
		{
			if (Target2 == null || !Target2.activeSelf)
			{
				TargetsToRemove.Add(Target2);
			}
		}
		if (TargetsToRemove.Count <= 0)
		{
			return;
		}
		foreach (GameObject Target in TargetsToRemove)
		{
			Targets.Remove(Target);
		}
	}

	private Vector3 CalculateEnemyPosition(GameObject TargetObject)
	{
		Rigidbody rb = currentTarget.GetComponent<Rigidbody>();
		if (rb.velocity.magnitude > 0.5f)
		{
			float distance = Vector3.Distance(currentTarget.transform.position, base.transform.position);
			EnemyAI EA = currentTarget.GetComponent<EnemyAI>();
			Vector3 lookPos;
			if ((bool)EA)
			{
				lookPos = ((!EA.DrivingBackwards) ? (currentTarget.transform.position + currentTarget.transform.forward * (distance / 6f)) : (currentTarget.transform.position + -currentTarget.transform.forward * (distance / 6f)));
			}
			else
			{
				MoveTankScript MTS = currentTarget.GetComponent<MoveTankScript>();
				lookPos = ((!MTS.DrivingBackwards) ? (currentTarget.transform.position + currentTarget.transform.forward * (distance / 6f)) : (currentTarget.transform.position + -currentTarget.transform.forward * (distance / 6f)));
			}
			Debug.DrawLine(lookPos, base.transform.position, Color.blue);
			return lookPos - base.transform.position;
		}
		return currentTarget.transform.position - base.transform.position;
	}

	private void Update()
	{
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
			if (SecretTimer >= 10f)
			{
				firedBullets = 0;
			}
		}
		else
		{
			SecretTimer = 0f;
		}
		int KnownEnemies = GameMaster.instance.AmountCalledInTanks;
		if (Targets.Count < 1 || KnownEnemies != prevKnownEnemies)
		{
			prevKnownEnemies = KnownEnemies;
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
		if (AIscript.HTscript.health < 1)
		{
			return;
		}
		if ((firePoint.Length < 4 || (firePoint.Length < 12 && !AIscript.isLevel30Boss)) && (currentTarget != null || AIscript.isLevel30Boss))
		{
			if (!specialMove && !normalLockedIn)
			{
				if (!turning)
				{
					Vector3 lookPos = (AIscript.CalculateEnemyPosition ? CalculateEnemyPosition(currentTarget) : (currentTarget.transform.position - base.transform.position));
					lookPos.y = 0f;
					if (!(lookPos != Vector3.zero))
					{
						return;
					}
					rotation = Quaternion.LookRotation(lookPos);
					float Accuracy = AIscript.Accuracy;
					if (currentTarget != null)
					{
						EnemyAI EA = currentTarget.GetComponent<EnemyAI>();
						if ((bool)EA && EA.isInvisible)
						{
							Accuracy += 15f;
						}
					}
					pick = Random.Range(0f - Accuracy, Accuracy) + LookAngleOffset;
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
					float timeSinceStarted2 = Time.time - _timeStartedLerping;
					float percentageComplete2 = timeSinceStarted2 / timeTakenDuringLerp;
					if (!AIscript.isStunned)
					{
						base.transform.rotation = Quaternion.Lerp(startRot, rotation, percentageComplete2);
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
						float timeSinceStarted = Time.time - _timeStartedLerping;
						float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
						if (!AIscript.isStunned)
						{
							base.transform.rotation = Quaternion.Lerp(startRot, rotation, percentageComplete);
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
		float time = 0.5f - myTurnSpeed / 10f;
		yield return new WaitForSeconds(time);
		turning = false;
	}

	private void SearchPlayers()
	{
		Targets.Clear();
		if ((bool)MapEditorMaster.instance)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject Enemy in array)
			{
				EnemyAI EA3 = Enemy.GetComponent<EnemyAI>();
				if ((bool)EA3 && Enemy.gameObject != AIscript.gameObject && (EA3.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(Enemy);
				}
			}
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("Boss");
			foreach (GameObject Enemy2 in array2)
			{
				EnemyAI EA4 = Enemy2.GetComponent<EnemyAI>();
				if ((bool)EA4 && Enemy2.gameObject != AIscript.gameObject && (EA4.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(Enemy2);
				}
			}
			GameObject[] array3 = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject Player in array3)
			{
				MoveTankScript MTS = Player.GetComponent<MoveTankScript>();
				if ((bool)MTS)
				{
					if (MTS.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0)
					{
						Targets.Add(Player);
					}
					continue;
				}
				EnemyAI EA5 = Player.GetComponent<EnemyAI>();
				if ((bool)EA5 && Player.gameObject != AIscript.gameObject && (EA5.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(Player);
				}
			}
			return;
		}
		if (isHuntingEnemies || GameMaster.instance.inMenuMode || AIscript.MyTeam == 1)
		{
			GameObject[] array4 = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject GO in array4)
			{
				EnemyAI EA = GO.GetComponent<EnemyAI>();
				if ((bool)EA && GO.gameObject != AIscript.gameObject && (EA.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
				{
					Targets.Add(GO);
				}
			}
			GameObject[] array5 = GameObject.FindGameObjectsWithTag("Boss");
			foreach (GameObject GO2 in array5)
			{
				if (GO2.gameObject != AIscript.gameObject)
				{
					Targets.Add(GO2);
				}
			}
			return;
		}
		GameObject[] array6 = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject GO3 in array6)
		{
			Targets.Add(GO3);
		}
		GameObject[] array7 = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject GO4 in array7)
		{
			EnemyAI EA2 = GO4.GetComponent<EnemyAI>();
			if ((bool)EA2 && GO4.gameObject != AIscript.gameObject && EA2.MyTeam != AIscript.MyTeam)
			{
				Targets.Add(GO4);
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
			HealthTanks currentTargetHealth = currentTarget.GetComponent<HealthTanks>();
			if ((bool)currentTargetHealth && currentTargetHealth.dying)
			{
				LookingAtTarget = false;
				TargetInSight = false;
				currentTarget = null;
				return;
			}
			EnemyAI TargetEA = currentTarget.GetComponent<EnemyAI>();
			if ((bool)TargetEA)
			{
				if (TargetEA.isInvisible)
				{
					AIscript.ShootSpeed = AIscript.OriginalShootSpeed * 1.5f;
				}
				else
				{
					AIscript.ShootSpeed = AIscript.OriginalShootSpeed;
				}
			}
			RaycastHit rayhit;
			if (Targets[0] != currentTarget)
			{
				float distTarget0 = Vector3.Distance(base.transform.position, Targets[0].transform.position);
				float distCurrentTarget = Vector3.Distance(base.transform.position, currentTarget.transform.position);
				if (distTarget0 < distCurrentTarget - 5f)
				{
					Vector3 testdir = Targets[0].transform.position - base.transform.position;
					Debug.DrawRay(base.transform.position, testdir * 50f, Color.green, 0.5f);
					testdir += new Vector3(0f, 0.5f, 0f);
					if (Physics.Raycast(base.transform.position, testdir, out rayhit, 50f, myLayerMasks) && rayhit.collider.gameObject == Targets[0])
					{
						TargetInSight = true;
						currentTarget = Targets[0];
						LookingAtTarget = false;
						normalLockedIn = false;
						return;
					}
				}
			}
			Vector3 dir = currentTarget.transform.position - base.transform.position;
			dir += new Vector3(0f, 0.5f, 0f);
			Debug.DrawRay(base.transform.position, dir * 50f, Color.green, 0.1f);
			if (Physics.Raycast(base.transform.position, dir, out rayhit, 50f, myLayerMasks))
			{
				if (rayhit.collider.gameObject == currentTarget)
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
			GameObject Target = CheckTargetsIfInSight(myLayerMasks);
			if ((bool)Target)
			{
				LookingAtTarget = false;
				normalLockedIn = false;
				specialMoveLockedIn = false;
				TargetInSight = false;
				StartCoroutine(StartTheShooting(Target));
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
		List<GameObject> TargetsInSight = new List<GameObject>();
		TargetsInSight.Clear();
		foreach (GameObject Target in Targets)
		{
			Vector3 dir = Target.transform.position - base.transform.position;
			dir += new Vector3(0f, 0.5f, 0f);
			Debug.DrawRay(base.transform.position, dir * 50f, Color.yellow, 0.1f);
			if (!Physics.Raycast(base.transform.position, dir, out var rayhit, float.PositiveInfinity, layerMask))
			{
				continue;
			}
			if ((bool)MapEditorMaster.instance)
			{
				if (rayhit.collider.tag == "Enemy" || rayhit.collider.tag == "Boss")
				{
					EnemyAI EA = rayhit.collider.GetComponent<EnemyAI>();
					if ((bool)EA && (EA.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
					{
						TargetsInSight.Add(Target);
					}
				}
				else
				{
					if (!(rayhit.collider.tag == "Player"))
					{
						continue;
					}
					MoveTankScript MTS = rayhit.collider.GetComponent<MoveTankScript>();
					if ((bool)MTS)
					{
						if (MTS.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0)
						{
							TargetsInSight.Add(Target);
						}
						continue;
					}
					EnemyAI EA2 = rayhit.collider.GetComponent<EnemyAI>();
					if ((bool)EA2 && (EA2.MyTeam != AIscript.MyTeam || AIscript.MyTeam == 0))
					{
						TargetsInSight.Add(Target);
					}
				}
			}
			else if (isHuntingEnemies || AIscript.MyTeam == 1)
			{
				if (rayhit.collider.tag == "Enemy" || rayhit.collider.tag == "Boss")
				{
					TargetsInSight.Add(Target);
				}
			}
			else if (!isHuntingEnemies && rayhit.collider.tag == "Player")
			{
				if (IgnoreCork && EnemiesNear(rayhit.transform.position, 4))
				{
					currentTarget = Target;
					StartCoroutine(CooldownTarget(1f));
					return null;
				}
				HealthTanks currentTargetHealth = rayhit.transform.gameObject.GetComponent<HealthTanks>();
				if (!currentTargetHealth || !currentTargetHealth.dying)
				{
					TargetsInSight.Add(Target);
				}
			}
		}
		if (TargetsInSight.Count > 0)
		{
			TargetsInSight = TargetsInSight.OrderBy((GameObject x) => Vector3.Distance(base.transform.position, x.transform.position)).ToList();
			return TargetsInSight[0];
		}
		return null;
	}

	private IEnumerator StartTheShooting(GameObject Target)
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
			Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 2.5f);
			Collider[] array = objectsInRange;
			foreach (Collider col2 in array)
			{
				DestroyableWall DestroyWall2 = col2.GetComponent<DestroyableWall>();
				if (DestroyWall2 != null && col2.gameObject.layer == LayerMask.NameToLayer("CorkWall"))
				{
					PlaceMine(deathmine: false);
					return;
				}
			}
			if (!EnemiesNear(base.transform.position, 6) && currentTarget != null)
			{
				float dist = Vector3.Distance(base.transform.position, currentTarget.transform.position);
				if (dist < 16f)
				{
					PlaceMine(deathmine: false);
				}
			}
		}
		else
		{
			if (!canMine || !canMineOnlyNearCork || EnemiesNear(base.transform.position, 5))
			{
				return;
			}
			Collider[] objectsInRange2 = Physics.OverlapSphere(base.transform.position, 2.5f);
			Collider[] array2 = objectsInRange2;
			foreach (Collider col in array2)
			{
				DestroyableWall DestroyWall = col.GetComponent<DestroyableWall>();
				if (DestroyWall != null && !DestroyWall.IsStone)
				{
					PlaceMine(deathmine: false);
				}
			}
		}
	}

	public void PlaceMine(bool deathmine)
	{
		int random = Random.Range(0, 2);
		if (random == 1 || deathmine)
		{
			GameObject mine = Object.Instantiate(minePrefab, base.transform.position + new Vector3(0f, -0.65f, 0f), Quaternion.identity);
			MineScript minescript = mine.GetComponent<MineScript>();
			minescript.isHuntingEnemies = isHuntingEnemies;
			minescript.placedByEnemies = true;
			if ((bool)GameMaster.instance.CM)
			{
				mine.transform.SetParent(GameMaster.instance.CM.transform);
			}
			else
			{
				mine.transform.parent = null;
			}
			mine.transform.position = new Vector3(mine.transform.position.x, -0.02f, mine.transform.position.z);
			canMine = false;
			minescript.MyPlacer = AIscript.gameObject;
			mineCountdown = Random.Range(1f, 2.5f) + AIscript.LayMinesSpeed;
			if (deathmine)
			{
				minescript.StartCoroutine(minescript.setactive());
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
			foreach (GameObject incomingbullet in AIscript.IncomingBullets)
			{
				if (!incomingbullet)
				{
					continue;
				}
				float dist = Vector3.Distance(base.transform.position, incomingbullet.transform.position);
				if (((dist < 8f && TargetInSight) || (dist < 12f && !TargetInSight)) && !specialMove && !specialMove && canShoot && !specialMoveLockedIn)
				{
					startingPosition = incomingbullet.transform.position;
					startingPosition = incomingbullet.transform.position + -incomingbullet.transform.up;
					startingPosition = new Vector3(startingPosition.x, base.transform.position.y, startingPosition.z);
					TurnTowardsSpecialMove();
					AIscript.IncomingBullets.Remove(incomingbullet);
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
			Vector3 lookPos = startingPosition - base.transform.position;
			rotation = Quaternion.LookRotation(lookPos);
		}
	}

	private bool DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining, bool CheckingForFriendlies)
	{
		int iterations = 0;
		float distToPoint = -1f;
		do
		{
			bool seeingPlayer = false;
			Vector3 startPosition = position;
			Ray ray = new Ray(position, direction);
			LayerMask layerMask = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
			RaycastHit[] allhits = (from h in Physics.RaycastAll(ray, 100f, layerMask)
				orderby h.distance
				select h).ToArray();
			for (int i = 0; i < allhits.Length; i++)
			{
				RaycastHit hit = allhits[i];
				if (CheckingForFriendlies)
				{
					Debug.DrawLine(startPosition, hit.point, Color.yellow, 1f);
				}
				else
				{
					Debug.DrawLine(startPosition, hit.point, Color.red);
				}
				if (hit.collider.gameObject == AIscript.gameObject && iterations == 0)
				{
					continue;
				}
				if ((hit.collider.tag == "Player" || hit.collider.tag == "HitZone") && !isHuntingEnemies && hit.collider.transform.parent.gameObject != AIscript.gameObject && hit.collider.gameObject != AIscript.gameObject)
				{
					MoveTankScript MTS = hit.transform.GetComponent<MoveTankScript>();
					if (MTS == null && hit.collider.tag == "HitZone")
					{
						MTS = hit.transform.parent.GetChild(0).transform.GetComponent<MoveTankScript>();
					}
					if ((bool)MTS)
					{
						if (!(hit.collider.tag == "HitZone"))
						{
							return false;
						}
						if (MTS.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && !CheckingForFriendlies)
						{
							return false;
						}
						if (MTS.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && CheckingForFriendlies)
						{
							return true;
						}
					}
					else
					{
						EnemyAI EA = hit.transform.GetComponent<EnemyAI>();
						if ((bool)EA)
						{
							if (EA.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && !CheckingForFriendlies)
							{
								return false;
							}
							if (EA.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && CheckingForFriendlies)
							{
								return true;
							}
						}
					}
					HealthTanks HT = hit.transform.gameObject.GetComponent<HealthTanks>();
					if (HT != null && HT.dying)
					{
						return false;
					}
					if (distToPoint < 0f && reflectionsRemaining < 1)
					{
						return false;
					}
					seeingPlayer = true;
					if (distToPoint != -1f && AIscript.CanMove)
					{
						Vector3 posTemp = hit.point;
						float secondDistToPoint = Vector3.Distance(base.transform.position, posTemp);
						if (distToPoint < secondDistToPoint)
						{
							return false;
						}
					}
				}
				if ((hit.collider.tag == "Enemy" || hit.collider.tag == "Boss") && hit.collider.gameObject != AIscript.gameObject)
				{
					EnemyAI EA2 = hit.transform.GetComponent<EnemyAI>();
					if (EA2.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && !CheckingForFriendlies && EA2.gameObject != AIscript.gameObject)
					{
						return false;
					}
					if (EA2.MyTeam == AIscript.MyTeam && AIscript.MyTeam != 0 && CheckingForFriendlies && EA2.gameObject != AIscript.gameObject)
					{
						return true;
					}
					if (distToPoint != -1f)
					{
						Vector3 posTemp2 = hit.point;
						float secondDistToPoint2 = Vector3.Distance(position, posTemp2);
						if (distToPoint < secondDistToPoint2 && distToPoint < 9f)
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
				if ((hit.collider.tag == "Enemy" || hit.collider.tag == "Boss") && hit.collider.gameObject == AIscript.gameObject && !AIscript.CanMove && CheckingForFriendlies)
				{
					return true;
				}
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("NoBounceWall") && !CheckingForFriendlies)
				{
					return false;
				}
				if (hit.collider.tag == "Solid" || hit.collider.tag == "MapBorder")
				{
					if (reflectionsRemaining == maxReflectionCount && startingPosition == Vector3.zero)
					{
						startingPosition = hit.point;
					}
					direction = Vector3.Reflect(direction, hit.normal);
					position = hit.point;
					distToPoint = Vector3.Distance(base.transform.position, position);
				}
				distToPoint = Vector3.Distance(base.transform.position, position);
				if (((distToPoint < 6f && AIscript.CanMove) || (distToPoint < 1f && !AIscript.CanMove)) && reflectionsRemaining == maxReflectionCount && !CheckingForFriendlies)
				{
					return false;
				}
				if (hit.collider.tag == "Solid" || hit.collider.tag == "MapBorder")
				{
					break;
				}
			}
			if (seeingPlayer && !CheckingForFriendlies)
			{
				return true;
			}
			iterations++;
			reflectionsRemaining--;
		}
		while (reflectionsRemaining >= 0);
		return false;
	}

	public void ShootAllBarrels()
	{
		int BulletsLeft = maxFiredBullets - firedBullets;
		if (BulletsLeft <= 0)
		{
			return;
		}
		if (AmountShotgunShots > 0)
		{
			if (AmountShotgunShots == 1 && BulletsLeft >= 1)
			{
				firedBullets++;
				FireTank(firePoint[0], bulletPrefab);
			}
			else if (AmountShotgunShots == 2 && BulletsLeft >= 2)
			{
				firedBullets += 2;
				FireTank(firePoint[1], bulletPrefab);
				FireTank(firePoint[2], bulletPrefab);
			}
			else if (AmountShotgunShots == 3 && BulletsLeft >= 3)
			{
				firedBullets += 3;
				FireTank(firePoint[0], bulletPrefab);
				FireTank(firePoint[1], bulletPrefab);
				FireTank(firePoint[2], bulletPrefab);
			}
			else if (AmountShotgunShots == 4 && BulletsLeft >= 4)
			{
				firedBullets += 4;
				FireTank(firePoint[1], bulletPrefab);
				FireTank(firePoint[2], bulletPrefab);
				FireTank(firePoint[3], bulletPrefab);
				FireTank(firePoint[4], bulletPrefab);
			}
			else
			{
				if (AmountShotgunShots != 5 || BulletsLeft < 5)
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
		else if (BulletsLeft >= firePoint.Length)
		{
			firedBullets += firePoint.Length;
			Transform[] array = firePoint;
			foreach (Transform point in array)
			{
				FireTank(point, bulletPrefab);
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
		if (GameMaster.instance.AmountGoodTanks < 1 && !isHuntingEnemies && !GameMaster.instance.CM && !GameMaster.instance.inMenuMode && !MapEditorMaster.instance)
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
				if (CheckForEverything(reflections: (AIscript.amountOfBounces > 0) ? 1 : 0, checkAngle: base.transform.rotation, checkOffset: 0.6f, FriendliesCheck: true))
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
				if (Physics.Raycast(layerMask: (LayerMask)(~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("OneWayBlock")) | (1 << LayerMask.NameToLayer("EnemyBorder")))), origin: base.transform.position, direction: base.transform.TransformDirection(Vector3.forward) * 100f, hitInfo: out var rayhit, maxDistance: float.PositiveInfinity))
				{
					if (rayhit.collider.tag == "Solid" || rayhit.collider.tag == "MapBorder" || rayhit.collider.tag == "Mine")
					{
						float distance = Vector3.Distance(base.transform.position, rayhit.point);
						if (distance > 5f)
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
				for (int j = 0; j < 8; j++)
				{
					FireTank(firePoint[j], SecondBulletPrefab);
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
				for (int i = 0; i < firePoint.Length; i++)
				{
					FireTank(firePoint[i], bulletPrefab);
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
		Vector3 checkPos1 = base.transform.position + (checkAngle * Vector3.left).normalized * checkOffset;
		Vector3 checkPos2 = base.transform.position + (checkAngle * Vector3.right).normalized * checkOffset;
		bool StillHitLeft = DrawReflectionPattern(checkPos1, checkAngle * Vector3.forward, reflections, FriendliesCheck);
		bool StillHitRight = DrawReflectionPattern(checkPos2, checkAngle * Vector3.forward, reflections, FriendliesCheck);
		if (FriendliesCheck)
		{
			if (StillHitLeft || StillHitRight)
			{
				return true;
			}
		}
		else if (!StillHitLeft || !StillHitRight)
		{
			return true;
		}
		return false;
	}

	public void AddRocket(int index)
	{
		GameObject newRocket = Object.Instantiate(rocketPrefab, rocketSlotLocations[index]);
		newRocket.GetComponent<RocketScript>().isHuntingEnemies = isHuntingEnemies;
		newRocket.GetComponent<RocketScript>().papaTank = AIscript.gameObject;
		newRocket.GetComponent<RocketScript>().MyTeam = AIscript.MyTeam;
		if (AIscript.isInvisible)
		{
			foreach (Transform T in newRocket.transform)
			{
				T.gameObject.SetActive(value: false);
				foreach (Transform TR in T)
				{
					TR.gameObject.SetActive(value: false);
				}
			}
		}
		rockets[index] = newRocket;
		newRocket.transform.parent = rocketSlotLocations[index];
	}

	private IEnumerator ShootRockets()
	{
		if (SR_running > 1 || !AIscript.hasRockets)
		{
			yield break;
		}
		float RandomizedWaitingTime2 = Random.Range(2f, 2.5f);
		if (RocketReloadSpeed > 0f)
		{
			RandomizedWaitingTime2 = RocketReloadSpeed;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 1)
		{
			RandomizedWaitingTime2 = Random.Range(1.5f, 2f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 2)
		{
			RandomizedWaitingTime2 = Random.Range(1f, 1.5f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 3)
		{
			RandomizedWaitingTime2 = Random.Range(0.75f, 1f);
		}
		yield return new WaitForSeconds(RandomizedWaitingTime2);
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
					RandomizedWaitingTime2 = RocketReloadSpeed / 2f;
				}
				else
				{
					RandomizedWaitingTime2 = Random.Range(0.4f, 0.9f);
					if (OptionsMainMenu.instance.currentDifficulty == 1)
					{
						RandomizedWaitingTime2 = Random.Range(0.3f, 0.8f);
					}
					else if (OptionsMainMenu.instance.currentDifficulty == 2)
					{
						RandomizedWaitingTime2 = Random.Range(0.2f, 0.7f);
					}
					else if (OptionsMainMenu.instance.currentDifficulty == 3)
					{
						RandomizedWaitingTime2 = Random.Range(0.1f, 0.6f);
					}
				}
				yield return new WaitForSeconds(RandomizedWaitingTime2);
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
						RandomizedWaitingTime2 = RocketReloadSpeed / 2f;
					}
					else
					{
						RandomizedWaitingTime2 = Random.Range(0.4f, 0.9f);
						if (OptionsMainMenu.instance.currentDifficulty == 1)
						{
							RandomizedWaitingTime2 = Random.Range(0.3f, 0.8f);
						}
						else if (OptionsMainMenu.instance.currentDifficulty == 2)
						{
							RandomizedWaitingTime2 = Random.Range(0.2f, 0.7f);
						}
						else if (OptionsMainMenu.instance.currentDifficulty == 3)
						{
							RandomizedWaitingTime2 = Random.Range(0.1f, 0.6f);
						}
					}
					yield return new WaitForSeconds(RandomizedWaitingTime2);
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
		float RandomizedWaitingTime = Random.Range(4f, 5f);
		if (RocketReloadSpeed > 0f)
		{
			RandomizedWaitingTime = RocketReloadSpeed;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 1)
		{
			RandomizedWaitingTime = Random.Range(3.5f, 4f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 2)
		{
			RandomizedWaitingTime = Random.Range(3f, 3.5f);
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 3)
		{
			RandomizedWaitingTime = Random.Range(2.5f, 3f);
		}
		yield return new WaitForSeconds(RandomizedWaitingTime);
		canReloadRockets = true;
	}

	public void FireTank(Transform firepoint, GameObject bulletprefab)
	{
		if (doubleStopper && !AIscript.isLevel30Boss && firePoint.Length < 2)
		{
			Debug.LogError("Doublestopper kicked in");
			return;
		}
		if ((!GameMaster.instance.GameHasStarted || GameMaster.instance.AmountGoodTanks < 1) && !isHuntingEnemies && !GameMaster.instance.CM && !GameMaster.instance.inMenuMode && !MapEditorMaster.instance)
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
		bool isPlayingShootSound = false;
		GameObject bulletGO;
		if (AIscript.isElectric && !AIscript.HTscript.IsCustom)
		{
			if (AIscript.isCharged || AIscript.isLevel70Boss)
			{
				bulletGO = Object.Instantiate(bulletprefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
				if (ExtraShotSounds.Length != 0)
				{
					int picky2 = Random.Range(0, ExtraShotSounds.Length);
					SFXManager.instance.PlaySFX(ExtraShotSounds[picky2]);
					isPlayingShootSound = true;
				}
				else
				{
					SFXManager.instance.PlaySFX(ShootSound);
					isPlayingShootSound = true;
				}
			}
			else
			{
				bulletGO = Object.Instantiate(SecondBulletPrefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
				SFXManager.instance.PlaySFX(secondShotSound);
				isPlayingShootSound = true;
			}
		}
		else
		{
			if (ExtraShotSounds.Length != 0)
			{
				int picky = Random.Range(0, ExtraShotSounds.Length);
				SFXManager.instance.PlaySFX(ExtraShotSounds[picky]);
				isPlayingShootSound = true;
			}
			else if (AIscript.HTscript.EnemyID == -1)
			{
				if (bulletprefab.name == "EnemyExplosiveBullet")
				{
					SFXManager.instance.PlaySFX(secondShotSound);
				}
				isPlayingShootSound = true;
			}
			bulletGO = Object.Instantiate(bulletprefab, firepoint.position, Quaternion.Euler(firepoint.transform.forward));
		}
		if (!isPlayingShootSound)
		{
			SFXManager.instance.PlaySFX(ShootSound);
		}
		PlayerBulletScript bullet = bulletGO.GetComponent<PlayerBulletScript>();
		bullet.MyTeam = AIscript.MyTeam;
		if (AIscript.isShiny)
		{
			bullet.isShiny = true;
		}
		if (AIscript.isSuperShiny)
		{
			bullet.isSuperShiny = true;
		}
		Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
		if ((bool)bulletBody)
		{
			bulletBody.AddForce(firepoint.forward * 6f);
			bullet.StartingVelocity = firepoint.forward * 6f;
			Vector3 startingPosition = firepoint.position;
			Ray ray = new Ray(firepoint.position, firepoint.forward);
			LayerMask LM = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall")) | (1 << LayerMask.NameToLayer("DestroyableWall"));
			Debug.DrawRay(firepoint.position, firepoint.forward * 15f, Color.cyan, 3f);
			if (Physics.Raycast(ray, out var hit, 1000f, LM))
			{
				bullet.WallGonnaBounceInTo = hit.transform.gameObject;
			}
		}
		if (!AIscript.IsCompanion)
		{
			bullet.MaxBounces = AIscript.amountOfBounces;
			bullet.papaTank = AIscript.gameObject;
			bullet.EnemyTankScript = this;
			bullet.isEnemyBullet = true;
		}
		else
		{
			bullet.MaxBounces = AIscript.amountOfBounces;
			bullet.papaTank = AIscript.gameObject;
			bullet.ShotByPlayer = AIscript.CompanionID;
			bullet.EnemyTankScript = this;
		}
		if (CustomShootSpeed > -1)
		{
			bullet.BulletSpeed = CustomShootSpeed;
		}
		if (bulletprefab == SecondBulletPrefab && AIscript.isLevel30Boss)
		{
			bullet.MaxBounces = 1;
		}
		Physics.IgnoreCollision(bullet.GetComponent<Collider>(), AIscript.GetComponent<Collider>());
		bullet.CollidersIgnoring.Add(AIscript.GetComponent<Collider>());
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
		Collider[] objectsInRange = Physics.OverlapSphere(position, range);
		Collider[] array = objectsInRange;
		foreach (Collider col in array)
		{
			if (((col.tag == "Enemy" && !isHuntingEnemies) || (col.tag == "Player" && isHuntingEnemies)) && col.gameObject != AIscript.gameObject)
			{
				return true;
			}
			if (col.tag == "Player" && AIscript.IsCompanion && col.gameObject != AIscript.gameObject)
			{
				return true;
			}
		}
		return false;
	}
}
