using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[Header("AI Behaviour")]
	public bool IsCompanion = false;

	public int CompanionID = 1;

	public int difficulty = 0;

	public int NotSeeBulletChancePercentage = 5;

	public float Accuracy = 10f;

	public bool CanMove = true;

	public bool LayMines = false;

	public float LayMinesSpeed = 0f;

	public float MineSenseRange = 5f;

	public float ShootSpeed = 3f;

	public bool BouncyBullets = false;

	public int amountOfBounces = 2;

	public bool isInvisible = false;

	public GameObject MagicPoof;

	public bool hasRockets = false;

	public bool isAggressive = false;

	public bool CalculateEnemyPosition = false;

	public bool isSmart = false;

	public bool isElectric = false;

	public bool isCharged = false;

	public bool isAggro = false;

	public bool isWallHugger = false;

	public GameObject[] ElectricComponents;

	public ParticleSystem[] ElectricSystems;

	public ParticleSystem AngrySteam;

	public float ElectricMeter = 0f;

	public Vector3 ElectricTargetLocation;

	private Vector3 StartPos;

	public float timeTakenDuringLerp = 1f;

	public float _timeStartedLerping;

	public bool isTransporting = false;

	private bool isInvisibleNow = false;

	public AudioClip StartTeleport;

	public AudioClip EndTeleport;

	public AudioClip ChargedUpSound;

	public List<GameObject> IncomingBullets = new List<GameObject>();

	public int MyTeam = 2;

	[Header("Other Stuff")]
	public float TankSpeed = 5000f;

	public float BoostedTankSpeed = 97.5f;

	public float OriginalTankSpeed;

	public float turnSpeed = 10f;

	public bool IsTurning = false;

	public Transform BaseRaycastPoint;

	public float boosterFluid = 2f;

	public float maxFluid = 1f;

	public bool canBoost;

	public GameObject Rush;

	public GameObject RushBackwards;

	private AudioSource BoostSource;

	private float originalDurationParticles;

	public ParticleSystem skidMarkCreatorLvl50;

	public bool boosting = false;

	private Quaternion targetRotation;

	public AudioClip TankTracks;

	public AudioSource source;

	public Rigidbody rigi;

	public ParticleSystem skidMarkCreator;

	public Transform skidMarkLocation;

	public Vector3 originalLocation;

	public Quaternion originalRotation;

	public float rotyOriginal;

	public bool MineFleeing = false;

	public bool isLevel10Boss = false;

	public bool isLevel30Boss = false;

	public int lvl30Boss2modeLives = 35;

	public int lvl30Boss3modeLives = 20;

	public bool isLevel50Boss = false;

	public bool isLevel70Boss = false;

	public bool isLevel100Boss = false;

	public GameObject groundChecker;

	[HideInInspector]
	public float lvl30BossTurnSpeed = 50f;

	public bool movingToPreferred = false;

	[Header("Raycast Stuff")]
	public GameObject THEGRID;

	public EnemyDetection Ring0Detection;

	public List<EnemyDetection> Ring1Detection = new List<EnemyDetection>();

	public List<EnemyDetection> Ring2Detection = new List<EnemyDetection>();

	public List<EnemyDetection> Ring3Detection = new List<EnemyDetection>();

	public Vector3 TargetPosition;

	public bool Targeted = false;

	public float angle;

	public float desiredangle;

	public bool armoured = false;

	public float OriginalShootSpeed;

	public List<GameObject> MineinCooldown = new List<GameObject>();

	public EnemyTargetingSystemNew ETSN;

	public bool gettingCloser = false;

	public float lastDistanceToPlayer = 10000f;

	public float marge = 5f;

	[Header("electric stuff")]
	public bool isStunned = false;

	public float stunTimer = 0f;

	public ParticleSystem StunnedParticles;

	[HideInInspector]
	public int bulletBounces = -1;

	public HealthTanks HTscript;

	public GameObject armour;

	public bool couldMove;

	public bool DrivingBackwards = false;

	public bool HasPathfinding;

	public bool isPathfinding;

	public float NavMeshTimer;

	public float DownPlayerTimer;

	[Header("SHINY stuff")]
	public bool isShiny = false;

	public bool isSuperShiny = false;

	public ParticleSystem ShinySystem;

	public ParticleSystem SuperShinySystem;

	public MeshRenderer Body;

	public MeshRenderer Barrel;

	private PathfindingAI PAI;

	public GameObject AngerVein;

	public AudioClip AngerTracks;

	public AudioClip spotted;

	public AudioClip cooldown;

	private float TimeBeingAnger = 0f;

	private float coolingDown = 0f;

	public bool ElectricTeleportRunning = false;

	private ElectricPad UnderEP;

	private Vector3 previousPosition;

	private Quaternion rotation;

	private float angleSize;

	public bool isMoving = false;

	public float lastKnownDist = 0f;

	public EnemyDetection chosenED;

	public bool noSafeED = false;

	public int direction = 0;

	public int directionOffset = 0;

	private bool changeCooldown = false;

	public GameObject DownedPlayer = null;

	public bool GoingToPlayer = false;

	public bool CanGoStraightToPlayer = false;

	public Vector3 preferredLocation;

	public bool hasGottenPath = false;

	private void Awake()
	{
		bulletBounces = -1;
		PAI = GetComponent<PathfindingAI>();
		int chance = Random.Range(0, 16384);
		if (chance == 1 && !IsCompanion && MapEditorMaster.instance == null)
		{
			isShiny = true;
			ShinySystem.Play();
			Barrel.materials[0].color = Color.white;
			Body.materials[0].color = Color.white;
			if (isLevel100Boss)
			{
				Body.materials[0].mainTexture = null;
			}
		}
		else
		{
			chance = Random.Range(0, 114688);
			if (chance == 1 && !IsCompanion && MapEditorMaster.instance == null)
			{
				isSuperShiny = true;
				SuperShinySystem.Play();
				Barrel.materials[0].color = Color.yellow;
				Body.materials[0].color = Color.yellow;
				if (isLevel100Boss)
				{
					Body.materials[0].mainTexture = null;
				}
			}
		}
		if (base.transform.position.y < 0.1f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 0.1f, base.transform.position.z);
		}
		OriginalTankSpeed = TankSpeed;
		source = GetComponent<AudioSource>();
		OriginalShootSpeed = ShootSpeed;
		HTscript = GetComponent<HealthTanks>();
		if ((bool)Rush)
		{
			BoostSource = Rush.GetComponent<AudioSource>();
		}
		rotyOriginal = base.transform.eulerAngles.y;
		if (HTscript.health > 1 && base.transform.tag != "Boss")
		{
			armour = base.transform.Find("Armour").gameObject;
			if ((bool)armour)
			{
				armour.SetActive(value: true);
				armoured = true;
			}
		}
		if ((bool)THEGRID)
		{
			for (int i = 0; i < THEGRID.transform.childCount; i++)
			{
				EnemyDetection EDinQuestion = THEGRID.transform.GetChild(i).GetComponent<EnemyDetection>();
				EDinQuestion.papaTank = base.gameObject;
				int ring1amount = 16;
				int ring2amount = 16;
				int ring3amount = 16;
				EDinQuestion.ID = i;
				if (i == 0)
				{
					Ring0Detection = EDinQuestion;
				}
				else if (i > 0 && i <= ring1amount)
				{
					Ring1Detection.Add(EDinQuestion);
					EDinQuestion.isRing1 = true;
				}
				else if (i > ring1amount && i <= ring1amount + ring2amount)
				{
					Ring2Detection.Add(EDinQuestion);
				}
				else if (i > ring1amount + ring2amount && i <= ring1amount + ring2amount + ring3amount)
				{
					Ring3Detection.Add(EDinQuestion);
				}
			}
		}
		if (CanMove)
		{
			if (difficulty < 2)
			{
				EnemyDetection[] all3 = Ring2Detection.Concat(Ring3Detection).ToArray();
				EnemyDetection[] array = all3;
				foreach (EnemyDetection ED4 in array)
				{
					Object.Destroy(ED4.gameObject);
				}
				all3 = Ring3Detection.ToArray();
				EnemyDetection[] array2 = all3;
				foreach (EnemyDetection ED5 in array2)
				{
					Object.Destroy(ED5.gameObject);
				}
			}
			else if (difficulty < 3)
			{
				EnemyDetection[] all4 = Ring3Detection.ToArray();
				EnemyDetection[] array3 = all4;
				foreach (EnemyDetection ED3 in array3)
				{
					Object.Destroy(ED3.gameObject);
				}
			}
			EnemyDetection[] all2 = Ring3Detection.ToArray();
			EnemyDetection[] array4 = all2;
			foreach (EnemyDetection ED2 in array4)
			{
				Object.Destroy(ED2.gameObject);
			}
			Ring3Detection.Clear();
			THEGRID.SetActive(value: false);
		}
		else if ((bool)THEGRID)
		{
			EnemyDetection[] all = Ring2Detection.Concat(Ring1Detection).Concat(Ring3Detection).ToArray();
			EnemyDetection[] array5 = all;
			foreach (EnemyDetection ED in array5)
			{
				Object.Destroy(ED.gameObject);
			}
		}
		skidMarkCreator.Stop();
	}

	public void ActivateElectric()
	{
		if (!ElectricTeleportRunning)
		{
			ElectricTeleportRunning = true;
			ElectricMeter = Random.Range(-2, 0);
			StartCoroutine(ElectricTeleport());
		}
	}

	public void DeactiveElectric()
	{
		ElectricTeleportRunning = false;
	}

	private void Start()
	{
		if (IsCompanion && MyTeam < 0)
		{
			MyTeam = 1;
		}
		if ((isLevel10Boss || isLevel30Boss || isLevel50Boss || isLevel70Boss) && OptionsMainMenu.instance.currentDifficulty > 1)
		{
			if (!isLevel70Boss && !isLevel50Boss)
			{
				TankSpeed = Mathf.Round(TankSpeed * 1.25f * 100f) / 100f;
			}
			OriginalTankSpeed = TankSpeed;
			ShootSpeed /= 1.5f;
			OriginalShootSpeed = ShootSpeed;
		}
		couldMove = CanMove;
		if (ETSN == null)
		{
			Transform Grandparent = base.gameObject.transform.parent;
			ETSN = Grandparent.GetChild(1).GetChild(0).GetComponent<EnemyTargetingSystemNew>();
		}
		if (GameMaster.instance.inMenuMode)
		{
			MyTeam = 0;
		}
		NavMeshTimer = (IsCompanion ? Random.Range(1f, 3f) : Random.Range(2f, 6f));
		DownPlayerTimer = 0f;
		skidMarkCreator.Stop();
		ParticleSystem.MainModule main = skidMarkCreator.main;
		SetArmour();
		if ((bool)Rush)
		{
			originalDurationParticles = skidMarkCreator.main.duration;
			BoostSource = Rush.GetComponent<AudioSource>();
		}
		float durcalc;
		if (TankSpeed < 80f)
		{
			durcalc = 0.85f - TankSpeed / 90f;
			if (durcalc < 0.12f)
			{
				durcalc = 0.12f;
			}
		}
		else
		{
			durcalc = 0.1f;
		}
		main.duration = durcalc;
		angle = rotyOriginal;
		originalLocation = base.transform.position;
		originalRotation = base.transform.rotation;
		rigi = GetComponent<Rigidbody>();
		if (GameMaster.instance.CurrentMission != 49 && !isLevel100Boss)
		{
			rigi.constraints = (RigidbodyConstraints)80;
		}
		desiredangle = angle;
		if (CanMove)
		{
			CheckForSafeED(newdir: false);
		}
		InvokeRepeating("SlowUpdate", 0.25f, 0.25f);
		source.clip = TankTracks;
		if ((bool)GameMaster.instance && OptionsMainMenu.instance.SnowMode && !GameMaster.instance.inMapEditor)
		{
			TankTracks = GameMaster.instance.TankTracksCarpetSound;
		}
		if (isElectric)
		{
			ActivateElectric();
		}
		if (isLevel10Boss || isLevel30Boss || isLevel50Boss || isLevel70Boss)
		{
		}
		if (IsCompanion)
		{
			InvokeRepeating("CheckForDownedPlayer", 0.4f, 0.4f);
		}
		if ((bool)MapEditorMaster.instance && !isShiny && !isSuperShiny)
		{
			MeshRenderer MR = GameObject.Find("Cube.003").GetComponent<MeshRenderer>();
			if ((bool)MR && Body != null && MyTeam > -1 && MapEditorMaster.instance.TeamColorEnabled[MyTeam])
			{
				Body.materials[0].SetColor("_Color", MapEditorMaster.instance.TeamColors[MyTeam]);
			}
		}
		if (isLevel50Boss && OptionsMainMenu.instance.currentDifficulty > 1)
		{
			ShootSpeed = 0.8f;
			Accuracy = 25f;
		}
		if (OptionsMainMenu.instance.currentDifficulty > 2 && !IsCompanion && !HTscript.IsCustom)
		{
			ShootSpeed /= 2f;
			OriginalShootSpeed = ShootSpeed;
			if (!isLevel70Boss && !isLevel50Boss)
			{
				TankSpeed *= 1.25f;
				OriginalTankSpeed = TankSpeed;
			}
			ETSN.maxFiredBullets *= 2;
		}
		SetTankBodyTracks();
	}

	private void SetTankBodyTracks()
	{
		if (!MapEditorMaster.instance || (HTscript.EnemyID != 7 && (!HTscript.IsCustom || !isInvisible)))
		{
			return;
		}
		if (MyTeam != 0)
		{
			if (MapEditorMaster.instance.TeamColorEnabled[MyTeam])
			{
				SkidmarkController SC3 = skidMarkCreator.GetComponent<SkidmarkController>();
				SC3.startingColor = MapEditorMaster.instance.TeamColors[MyTeam];
				SC3.StartCoroutine(SC3.SetTrackColor());
			}
			else
			{
				SkidmarkController SC2 = skidMarkCreator.GetComponent<SkidmarkController>();
				SC2.startingColor = SC2.originalColor;
				SC2.StartCoroutine(SC2.SetTrackColor());
			}
		}
		else
		{
			SkidmarkController SC = skidMarkCreator.GetComponent<SkidmarkController>();
			SC.startingColor = SC.originalColor;
			SC.StartCoroutine(SC.SetTrackColor());
		}
	}

	private void OnEnable()
	{
		if ((bool)THEGRID)
		{
			THEGRID.SetActive(value: true);
		}
		if (source.isPlaying)
		{
			source.Stop();
		}
		hasGottenPath = false;
		isPathfinding = false;
		skidMarkCreator.Clear();
		skidMarkCreator.Stop();
		OriginalShootSpeed = ShootSpeed;
		angleSize = 0f;
		if ((bool)chosenED)
		{
			chosenED.isChosen = false;
			CheckForSafeED(newdir: false);
		}
		if (isInvisible)
		{
			MakeMeInvisible();
		}
		if (MyTeam == 1 && !MapEditorMaster.instance && !IsCompanion)
		{
			GameMaster.instance.AmountEnemyTanks--;
		}
		SetTankBodyTracks();
	}

	private void MakeMeInvisible()
	{
		if (GameMaster.instance.inMapEditor && !MapEditorMaster.instance.isTesting)
		{
			return;
		}
		Renderer[] rs = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = rs;
		foreach (Renderer r in array)
		{
			if (r.tag != "Temp")
			{
				if (r.tag != "AlwaysVisible")
				{
					r.enabled = false;
				}
				else
				{
					r.enabled = true;
				}
			}
		}
		isInvisibleNow = true;
		if (GameMaster.instance.GameHasStarted)
		{
			SpawnMagicPoof();
		}
		if (TankSpeed > 20f && !skidMarkCreator.isPlaying)
		{
			skidMarkCreator.Play();
		}
	}

	private void SlowUpdate()
	{
		if (GameMaster.instance.inMenuMode)
		{
			MyTeam = 0;
		}
		if (!GameMaster.instance.GameHasStarted)
		{
			if (isAggro)
			{
				DisableAnger();
			}
			return;
		}
		NavMeshTimer -= 0.25f;
		DownPlayerTimer -= 0.25f;
		if (isAggro)
		{
			if (TimeBeingAnger > 0f)
			{
				NotSeeBulletChancePercentage = 25;
			}
			else
			{
				NotSeeBulletChancePercentage = 85;
			}
		}
		List<GameObject> ObjToRemove = new List<GameObject>();
		for (int i = IncomingBullets.Count - 1; i > -1; i--)
		{
			if (IncomingBullets[i] == null)
			{
				ObjToRemove.Add(IncomingBullets[i]);
			}
		}
		foreach (GameObject Remove in ObjToRemove)
		{
			IncomingBullets.Remove(Remove);
		}
		if (rigi.velocity.magnitude > 0.5f && HTscript.EnemyID == 7 && (!skidMarkCreator.isPlaying || skidMarkCreator.isStopped))
		{
			skidMarkCreator.Play();
		}
		if (ETSN == null)
		{
			return;
		}
		if (ETSN.TargetInSight)
		{
			if (!isAggro)
			{
				return;
			}
			TankSpeed = OriginalTankSpeed * 1.6f;
			coolingDown = 5f;
			TimeBeingAnger += 0.25f;
			if (TimeBeingAnger >= 40f)
			{
				AngrySteam.Play();
			}
			else
			{
				AngrySteam.Stop();
			}
			if (TimeBeingAnger >= 60f)
			{
				if ((bool)AchievementsTracker.instance)
				{
					AchievementsTracker.instance.completeAchievement(32);
				}
				if ((bool)HTscript)
				{
					HTscript.health = -999;
				}
			}
			if ((bool)AngerVein && !AngerVein.activeSelf)
			{
				GameMaster.instance.musicScript.Orchestra.CherryRage = true;
				GameMaster.instance.musicScript.Orchestra.RagingCherries++;
				source.clip = AngerTracks;
				SFXManager.instance.PlaySFX(spotted);
				AngerVein.SetActive(value: true);
			}
		}
		else if (isAggro)
		{
			AngrySteam.Stop();
			coolingDown -= 0.25f;
			if (AngerVein.activeSelf)
			{
				TimeBeingAnger += 0.25f;
				if (TimeBeingAnger >= 40f)
				{
					AngrySteam.Play();
				}
			}
			if (coolingDown <= 0f)
			{
				DisableAnger();
			}
		}
		else if (!isAggro && !ETSN.specialMove && !canBoost && !HTscript.IsCustom)
		{
			TankSpeed = OriginalTankSpeed;
		}
	}

	private void DisableAnger()
	{
		coolingDown = 0f;
		TankSpeed = OriginalTankSpeed;
		if ((bool)AngerVein && AngerVein.activeSelf)
		{
			if (GameMaster.instance.musicScript.Orchestra.RagingCherries <= 1)
			{
				GameMaster.instance.musicScript.Orchestra.CherryRage = false;
			}
			if (GameMaster.instance.musicScript.Orchestra.RagingCherries > 1)
			{
				GameMaster.instance.musicScript.Orchestra.RagingCherries--;
			}
			source.clip = TankTracks;
			AngerVein.SetActive(value: false);
			TimeBeingAnger = 0f;
			SFXManager.instance.PlaySFX(cooldown);
		}
	}

	private void SpawnMagicPoof()
	{
		Vector3 rot = base.transform.rotation.eulerAngles;
		GameObject poof = Object.Instantiate(rotation: Quaternion.Euler(new Vector3(rot.x + 90f, rot.y, rot.z)), original: MagicPoof, position: base.transform.position + new Vector3(0f, 0.5f, 0f));
		ParticleSystem poofie = poof.GetComponent<ParticleSystem>();
		poofie.Play();
		Object.Destroy(poof, 3f);
	}

	private IEnumerator MakeMeVisible()
	{
		yield return new WaitForSeconds(0.01f);
		Renderer[] rs = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = rs;
		foreach (Renderer r in array)
		{
			if (r.tag != "Temp" && r.tag != "EnemyBorder" && r.tag != "BulletDetection")
			{
				if (r.tag != "AlwaysVisible")
				{
					r.enabled = true;
				}
				else
				{
					r.enabled = false;
				}
			}
		}
		isInvisibleNow = false;
	}

	private void OnDisable()
	{
		if ((bool)THEGRID)
		{
			THEGRID.SetActive(value: false);
		}
		if (isAggro)
		{
			DisableAnger();
		}
		if ((bool)Rush)
		{
			DeactivateBooster();
		}
		if (source.isPlaying)
		{
			if (source.volume > 0.5f)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
			}
			source.loop = false;
			source.volume = 0.25f;
			source.Stop();
		}
		if (isInvisible && base.gameObject.active)
		{
			StartCoroutine(MakeMeVisible());
		}
		if (isElectric && base.gameObject.active)
		{
			isCharged = false;
			isTransporting = false;
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<BoxCollider>().enabled = true;
			StartCoroutine(MakeMeVisible());
			ElectricMeter = Random.Range(-2, 0);
		}
		ShootSpeed = OriginalShootSpeed;
		SetTankBodyTracks();
		Rigidbody myRB = GetComponent<Rigidbody>();
		if ((bool)myRB && GameMaster.instance.CurrentMission != 49)
		{
			myRB.isKinematic = true;
		}
	}

	public void StunMe(float sec)
	{
		isStunned = true;
		stunTimer = sec;
		if ((bool)StunnedParticles)
		{
			StunnedParticles.Play();
		}
	}

	private IEnumerator ElectricTeleport()
	{
		if (!ElectricTeleportRunning)
		{
			yield break;
		}
		new List<int>();
		yield return new WaitForSeconds(4f);
		if (isCharged && (GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode))
		{
			Vector3 Location = GameMaster.instance.GetValidLocation(CheckForDist: true, 8f, base.transform.position, TargetPlayer: false);
			if (Location == Vector3.zero)
			{
				isCharged = false;
				StartCoroutine(ElectricTeleport());
				yield break;
			}
			if (!isInvisibleNow)
			{
				MakeMeInvisible();
				Debug.Log("PAUSING SKID MMARKS");
				skidMarkCreator.Pause();
			}
			SFXManager.instance.PlaySFX(StartTeleport);
			isCharged = false;
			StartPos = base.transform.position;
			_timeStartedLerping = Time.time;
			if (isLevel70Boss)
			{
				timeTakenDuringLerp = 2f;
				CameraShake CS = Camera.main.GetComponent<CameraShake>();
				if ((bool)CS)
				{
					CS.StartCoroutine(CS.Shake(0.15f, 0.15f));
				}
			}
			isTransporting = true;
			Ring0Detection.SolidPieces.Clear();
			EnemyDetection[] EDS = Ring1Detection.Concat(Ring2Detection).Concat(Ring3Detection).ToArray();
			EnemyDetection[] array = EDS;
			foreach (EnemyDetection ED in array)
			{
				ED.SolidPieces.Clear();
			}
			ETSN.normalLockedIn = false;
			ETSN.ShootCountdown = ShootSpeed;
			ElectricTargetLocation = new Vector3(Location.x, base.transform.parent.transform.position.y, Location.z);
		}
		StartCoroutine(ElectricTeleport());
	}

	private void FixedUpdate()
	{
		if ((bool)rigi && GameMaster.instance.CurrentMission != 49)
		{
			if (GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode)
			{
				rigi.isKinematic = false;
			}
			else
			{
				rigi.isKinematic = true;
			}
		}
		else if ((bool)rigi && GameMaster.instance.CurrentMission == 49)
		{
			rigi.isKinematic = false;
		}
		if (boosting && canBoost)
		{
			boosterFluid -= Time.deltaTime;
		}
		if (boosterFluid < maxFluid)
		{
			if (boosterFluid < 0.2f)
			{
				canBoost = false;
				DeactivateBooster();
			}
			if (boosterFluid > maxFluid / 2f)
			{
				canBoost = true;
			}
			boosterFluid += Time.deltaTime / 2f;
		}
		else
		{
			canBoost = true;
		}
		if (HTscript.dying)
		{
			return;
		}
		if (CanMove && !HTscript.dying && (GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode))
		{
			MoveTank();
		}
		if ((!GameMaster.instance.GameHasStarted && GameMaster.instance.inMapEditor) || HTscript.health < 1 || !isLevel70Boss)
		{
			return;
		}
		Vector3 To = new Vector3(base.transform.position.x, base.transform.position.y - 1f, base.transform.position.z);
		Vector3 From = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
		Vector3 toPos = To - From;
		float dist = 4f;
		RaycastHit[] allhits = Physics.RaycastAll(From, toPos, dist);
		for (int i = 0; i < allhits.Length; i++)
		{
			RaycastHit hit = allhits[i];
			if (!(hit.transform.tag == "ElectricPad"))
			{
				continue;
			}
			ElectricPad EP = hit.transform.GetComponent<ElectricPad>();
			if (EP != UnderEP)
			{
				if ((bool)UnderEP)
				{
					UnderEP.Active = false;
				}
				if (!isTransporting)
				{
					EP.Active = true;
				}
				UnderEP = EP;
			}
		}
	}

	private void SetArmour()
	{
		if (armour == null && armoured)
		{
			armour = base.transform.Find("Armour").gameObject;
			if ((bool)armour)
			{
				armour.SetActive(value: true);
				armoured = true;
			}
		}
		if (HTscript.health_armour >= 3 && armoured && !armour.transform.GetChild(2).gameObject.activeSelf)
		{
			armour.transform.GetChild(0).gameObject.SetActive(value: true);
			armour.transform.GetChild(2).gameObject.SetActive(value: true);
			armour.transform.GetChild(1).gameObject.SetActive(value: true);
		}
		else if (HTscript.health_armour == 2 && armoured && (!armour.transform.GetChild(1).gameObject.activeSelf || armour.transform.GetChild(2).gameObject.activeSelf))
		{
			armour.transform.GetChild(2).gameObject.SetActive(value: false);
			armour.transform.GetChild(0).gameObject.SetActive(value: true);
			armour.transform.GetChild(1).gameObject.SetActive(value: true);
		}
		else if (HTscript.health_armour == 1 && armoured && (!armour.transform.GetChild(0).gameObject.activeSelf || armour.transform.GetChild(1).gameObject.activeSelf))
		{
			armour.transform.GetChild(2).gameObject.SetActive(value: false);
			armour.transform.GetChild(1).gameObject.SetActive(value: false);
			armour.transform.GetChild(0).gameObject.SetActive(value: true);
		}
		else if (HTscript.health_armour == 0 && armoured && armour.activeSelf)
		{
			armour.transform.GetChild(2).gameObject.SetActive(value: false);
			armour.transform.GetChild(1).gameObject.SetActive(value: false);
			armour.transform.GetChild(0).gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (GameMaster.instance.restartGame)
		{
			base.transform.position = originalLocation;
			base.transform.rotation = originalRotation;
		}
		if ((bool)THEGRID)
		{
			if (!GameMaster.instance.inMenuMode)
			{
				if (!GameMaster.instance.GameHasStarted && THEGRID.activeSelf)
				{
					THEGRID.SetActive(value: false);
				}
				else if (GameMaster.instance.GameHasStarted && !THEGRID.activeSelf)
				{
					THEGRID.SetActive(value: true);
				}
			}
			else if (GameMaster.instance.inMenuMode && CanMove && !THEGRID.activeSelf)
			{
				THEGRID.SetActive(value: true);
			}
		}
		if (stunTimer > 0f)
		{
			isStunned = true;
			CanMove = false;
			stunTimer -= Time.deltaTime;
		}
		else if (isStunned)
		{
			isStunned = false;
			TankSpeed = OriginalTankSpeed;
			if (TankSpeed > 10f)
			{
				CanMove = couldMove;
			}
			isPathfinding = false;
			hasGottenPath = false;
			HasPathfinding = false;
			if ((bool)StunnedParticles)
			{
				StunnedParticles.Stop();
				StunnedParticles.Clear();
			}
		}
		if (TankSpeed < 20f)
		{
			CanMove = false;
		}
		if (HasPathfinding && isPathfinding && (!GameMaster.instance.GameHasStarted || HTscript.dying || isStunned))
		{
			TankSpeed = 0f;
		}
		if (base.tag == "Enemy")
		{
			SetArmour();
		}
		if (isTransporting)
		{
			float timeSinceStarted = Time.time - _timeStartedLerping;
			float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<BoxCollider>().enabled = false;
			base.transform.position = Vector3.Lerp(StartPos, ElectricTargetLocation, percentageComplete);
			float dist = Vector3.Distance(base.transform.position, ElectricTargetLocation);
			if (!(dist < 1f) && !(percentageComplete >= 1f))
			{
				return;
			}
			SFXManager.instance.PlaySFX(EndTeleport);
			isTransporting = false;
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<BoxCollider>().enabled = true;
			if (!isInvisible)
			{
				StartCoroutine(MakeMeVisible());
			}
			SpawnMagicPoof();
			ETSN.ShootCountdown += 1f;
			skidMarkCreator.Play();
			ElectricMeter = Random.Range(-0.5f, 0f);
			if (isLevel70Boss && HTscript.health < HTscript.maxHealth / 2)
			{
				ElectricMeter = Random.Range(0f, 0.5f);
				CameraShake CS = Camera.main.GetComponent<CameraShake>();
				if ((bool)CS)
				{
					CS.StartCoroutine(CS.Shake(0.15f, 0.15f));
				}
			}
			return;
		}
		if (!GameMaster.instance.GameHasStarted && GameMaster.instance.inMapEditor)
		{
			source.clip = null;
			if (source.isPlaying)
			{
				source.Stop();
			}
			return;
		}
		Rotate();
		if (base.transform.position.y < -0.5f && (isLevel50Boss || IsCompanion || HTscript.EnemyID == 11))
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y * 1.12f, base.transform.position.z);
			if (base.transform.position.y < -100.1f && isLevel50Boss)
			{
				if ((bool)AchievementsTracker.instance)
				{
					AchievementsTracker.instance.completeAchievement(12);
				}
				GetComponent<HealthTanks>().DamageMe(99);
			}
			else if (base.transform.position.y < -100.1f)
			{
				GetComponent<HealthTanks>().DamageMe(99);
			}
		}
		else if (base.transform.position.y < -20.1f || base.transform.position.y > 130.1f)
		{
			GetComponent<HealthTanks>().DamageMe(99);
		}
		if (isElectric && !isTransporting)
		{
			if (ElectricMeter < 1f)
			{
				isCharged = false;
				float chargeSpeed = (isLevel70Boss ? 0.18f : 0.22f);
				ElectricMeter += Time.deltaTime * chargeSpeed;
				GameObject[] electricComponents = ElectricComponents;
				foreach (GameObject Component2 in electricComponents)
				{
					Component2.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
				}
				ParticleSystem[] electricSystems = ElectricSystems;
				foreach (ParticleSystem System in electricSystems)
				{
					System.gameObject.SetActive(value: false);
					if (System.isPlaying)
					{
						System.Stop();
					}
				}
			}
			else if (!isCharged && ElectricMeter >= 1f)
			{
				GameObject[] electricComponents2 = ElectricComponents;
				foreach (GameObject Component in electricComponents2)
				{
					Component.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white);
				}
				ParticleSystem[] electricSystems2 = ElectricSystems;
				foreach (ParticleSystem System2 in electricSystems2)
				{
					System2.gameObject.SetActive(value: true);
					if (!System2.isPlaying)
					{
						System2.Play();
					}
				}
				SFXManager.instance.PlaySFX(ChargedUpSound);
				isCharged = true;
			}
		}
		if (isLevel30Boss && HTscript.health < lvl30Boss2modeLives)
		{
			if (ShootSpeed == OriginalShootSpeed && !HTscript.IsCustom)
			{
				if (OptionsMainMenu.instance.currentDifficulty > 1)
				{
					TankSpeed = 60f;
					ShootSpeed = 1.25f;
				}
				else
				{
					TankSpeed = 50f;
					ShootSpeed = 2f;
				}
			}
		}
		else if (isLevel30Boss && HTscript.health > lvl30Boss2modeLives && !HTscript.IsCustom)
		{
			TankSpeed = OriginalTankSpeed;
			ShootSpeed = OriginalShootSpeed;
		}
		if (GameMaster.instance.GameHasPaused)
		{
			return;
		}
		if (CanMove && (bool)source)
		{
			if (rigi.velocity.magnitude < 0.5f)
			{
				skidMarkCreator.Stop();
				if (source.isPlaying)
				{
					StopTankTracks();
				}
			}
			else if (rigi.velocity.magnitude >= 0.5f && !GameMaster.instance.GameHasPaused)
			{
				if (!skidMarkCreator.isPlaying || !skidMarkCreator.isEmitting)
				{
					skidMarkCreator.Play();
				}
				if (source.enabled && !source.isPlaying && CanMove && (!GameMaster.instance.GameHasPaused || ETSN.isHuntingEnemies))
				{
					PlayTankTracks();
				}
			}
			else if (GameMaster.instance.GameHasPaused)
			{
				StopTankTracks();
			}
			else if (!skidMarkCreator.isPlaying)
			{
				skidMarkCreator.Play();
			}
		}
		else if ((bool)source)
		{
			if (source.isPlaying)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
			}
			skidMarkCreator.Stop();
			source.loop = false;
			source.Stop();
		}
	}

	private void PlayTankTracks()
	{
		source.loop = true;
		source.clip = TankTracks;
		source.Play();
		if (GameMaster.instance.EnemyTankTracksAudio < GameMaster.instance.maxEnemyTankTracks && source.volume < 1f)
		{
			source.volume = 1f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
			GameMaster.instance.EnemyTankTracksAudio++;
		}
		else
		{
			source.volume = 0.05f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
	}

	private void StopTankTracks()
	{
		if ((bool)Rush)
		{
			DeactivateBooster();
		}
		if (source.isPlaying)
		{
			if (source.volume > 0.5f)
			{
				GameMaster.instance.EnemyTankTracksAudio--;
			}
			source.loop = false;
			source.volume = 0.25f;
			source.Stop();
		}
	}

	private void Rotate()
	{
		if ((GameMaster.instance.GameHasStarted || GameMaster.instance.inMenuMode) && CanMove)
		{
			angleSize = Quaternion.Angle(base.transform.rotation, rotation);
			if ((angleSize > 1f && angleSize < 120f) || isLevel100Boss)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, rotation, Time.deltaTime * turnSpeed);
			}
			else if (angleSize >= 120f)
			{
				DrivingBackwards = ((!DrivingBackwards) ? true : false);
			}
		}
	}

	private void CheckForSafeED(bool newdir)
	{
		switch (newdir ? Random.Range(0, 2) : Random.Range(0, 10))
		{
		case 1:
			direction -= Random.Range(1, 2);
			break;
		case 0:
			direction += Random.Range(1, 2);
			break;
		}
		if (CheckRing(Ring1Detection, direction, newdir))
		{
			noSafeED = false;
			return;
		}
		if (!Ring0Detection.isTargeted)
		{
			isMoving = false;
			noSafeED = true;
			StartCoroutine(ResetSafeED());
			return;
		}
		if (difficulty > 1)
		{
			if (CheckRing(Ring2Detection, direction, newdir))
			{
				noSafeED = false;
				return;
			}
			if (!Ring0Detection.isTargeted)
			{
				isMoving = false;
				noSafeED = true;
				if (IsCompanion)
				{
					StartCoroutine(ResetSafeED());
				}
				return;
			}
			if (difficulty > 2 && Ring3Detection.Count > 0 && CheckRing(Ring3Detection, direction, newdir))
			{
				noSafeED = false;
				return;
			}
		}
		if (CheckStuckByMines(toLayMine: false))
		{
			noSafeED = false;
			return;
		}
		isMoving = false;
		noSafeED = true;
		StartCoroutine(ResetSafeED());
	}

	public bool CheckStuckByMines(bool toLayMine)
	{
		if (Ring1Detection.Count > 0)
		{
			int ToGo = Ring1Detection.Count;
			List<EnemyDetection> EDwMines = new List<EnemyDetection>();
			foreach (EnemyDetection ED in Ring1Detection)
			{
				if (ED.MineClose)
				{
					EDwMines.Add(ED);
					ToGo--;
				}
			}
			if (ToGo <= 1 && EDwMines.Count > 0 && !toLayMine)
			{
				EDwMines = EDwMines.OrderByDescending((EnemyDetection x) => x.DistToMine).ToList();
				EDwMines[0].isTargeted = false;
				SetNewTarget(EDwMines[0], isPreferred: false, useBoost: false);
				return true;
			}
			if (ToGo <= 6 && EDwMines.Count > 0 && toLayMine)
			{
				return true;
			}
		}
		if (Ring2Detection.Count > 0 && toLayMine)
		{
			int ToGo2 = Ring2Detection.Count;
			foreach (EnemyDetection ED2 in Ring2Detection)
			{
				if (ED2.MineClose)
				{
					ToGo2--;
				}
			}
			if (ToGo2 <= 8)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckRing(List<EnemyDetection> Ring, int newDirection, bool newdir)
	{
		int counter = 0;
		direction = newDirection;
		if (direction < 0)
		{
			direction = 16 - direction;
		}
		if (direction > 15)
		{
			direction -= 15;
		}
		bool specialCheck = false;
		int leftDirection = (newdir ? (direction - 3) : (direction - 2));
		int rightDirection = (newdir ? (direction + 3) : (direction + 2));
		if (leftDirection < 0)
		{
			leftDirection = 16 + leftDirection;
		}
		if (rightDirection > 15)
		{
			rightDirection -= 15;
		}
		if (Mathf.Abs(rightDirection - leftDirection) > 7)
		{
			specialCheck = true;
		}
		List<EnemyDetection> SaveED = new List<EnemyDetection>();
		List<int> Direction = new List<int>();
		List<EnemyDetection> SaveEDInDir = new List<EnemyDetection>();
		List<int> DirectionInDir = new List<int>();
		List<EnemyDetection> CorrectDir = new List<EnemyDetection>();
		List<int> CorrectDirInt = new List<int>();
		foreach (EnemyDetection ED in Ring)
		{
			if (!ED.isTargeted && counter == direction)
			{
				CorrectDir.Add(ED);
				CorrectDirInt.Add(counter);
			}
			else
			{
				if (ED.isPreferred && !ED.isTargeted)
				{
					SetNewTarget(ED, isPreferred: true, useBoost: true);
					return true;
				}
				if (!ED.isTargeted && ((counter >= leftDirection && counter <= rightDirection && !specialCheck) || counter >= leftDirection || (counter <= rightDirection && specialCheck)))
				{
					SaveEDInDir.Add(ED);
					DirectionInDir.Add(counter);
				}
				else if (!ED.isTargeted)
				{
					SaveED.Add(ED);
					Direction.Add(counter);
				}
				else if (ED.isChosen)
				{
					ED.isChosen = false;
				}
			}
			counter++;
		}
		if (CorrectDir.Count > 0)
		{
			int pick3 = Random.Range(0, CorrectDir.Count - 1);
			direction = CorrectDirInt[pick3];
			SetNewTarget(CorrectDir[pick3], isPreferred: false, useBoost: false);
			return true;
		}
		if (SaveEDInDir.Count > 0)
		{
			int pick2 = Random.Range(0, SaveEDInDir.Count - 1);
			direction = DirectionInDir[pick2];
			SetNewTarget(SaveEDInDir[pick2], isPreferred: false, useBoost: false);
			return true;
		}
		if (SaveED.Count > 0)
		{
			int pick = Random.Range(0, SaveED.Count - 1);
			direction = Direction[pick];
			SetNewTarget(SaveED[pick], isPreferred: false, useBoost: false);
			return true;
		}
		return false;
	}

	private void SetNewTarget(EnemyDetection ED, bool isPreferred, bool useBoost)
	{
		chosenED = ED;
		ED.isChosen = true;
		isMoving = true;
		movingToPreferred = isPreferred;
		TargetPosition = new Vector3(ED.transform.position.x, base.transform.position.y, ED.transform.position.z);
		if (useBoost && (bool)Rush && !boosting && canBoost)
		{
			ActivateBooster();
		}
	}

	private IEnumerator ResetSafeED()
	{
		yield return new WaitForSeconds(0.3f);
		noSafeED = false;
		CheckForSafeED(newdir: false);
	}

	private IEnumerator ResetChange()
	{
		float waittime = Random.Range(0.4f, 0.9f);
		yield return new WaitForSeconds(waittime);
		changeCooldown = false;
	}

	private IEnumerator CheckIfPlayerStraightLine()
	{
		if (!DownedPlayer)
		{
			GoingToPlayer = false;
			CanGoStraightToPlayer = false;
			yield break;
		}
		LayerMask layersToIgnore = ~((1 << LayerMask.NameToLayer("EnemyDetectionLayer")) | (1 << LayerMask.NameToLayer("BulletDetectField")) | (1 << LayerMask.NameToLayer("TeleportBlock")) | (1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("EnemyBorder")) | (1 << LayerMask.NameToLayer("OneWayBlock")));
		float waittime = Random.Range(0.2f, 0.4f);
		yield return new WaitForSeconds(waittime);
		if (!DownedPlayer)
		{
			GoingToPlayer = false;
			CanGoStraightToPlayer = false;
			yield break;
		}
		Vector3 direction = DownedPlayer.transform.position - base.transform.position;
		Debug.DrawRay(base.transform.position, direction * 8f, Color.blue, 1f);
		bool gotHit = false;
		if (Physics.Raycast(base.transform.position, direction, out var rayhit, 8f, layersToIgnore))
		{
			Debug.Log("Companion scanning for player, but is seeing:" + rayhit.transform.name, rayhit.transform.gameObject);
			if (rayhit.transform.gameObject == DownedPlayer)
			{
				Debug.Log("PLAYER STRAGITH LINE!!");
				gotHit = true;
				GoingToPlayer = true;
				CanGoStraightToPlayer = true;
			}
		}
		if (!gotHit && GoingToPlayer)
		{
			StartCoroutine(CheckIfPlayerStraightLine());
		}
	}

	private void CheckForDownedPlayer()
	{
		if (!IsCompanion)
		{
			return;
		}
		bool aPlayerIsDown = false;
		for (int i = 0; i < 4; i++)
		{
			if (GameMaster.instance.PlayerDown[i] && !GoingToPlayer)
			{
				aPlayerIsDown = true;
				foreach (GameObject player in GameMaster.instance.Players)
				{
					MoveTankScript MTS = player.GetComponent<MoveTankScript>();
					if ((bool)MTS)
					{
						if (MTS.playerId == i)
						{
							DownedPlayer = player;
							StartCoroutine(CheckIfPlayerStraightLine());
							PAI.SearchForPlayer = true;
							PAI.SearchAIblock();
							return;
						}
						continue;
					}
					EnemyAI EA = player.GetComponent<EnemyAI>();
					if (EA.CompanionID != i)
					{
						continue;
					}
					DownedPlayer = player;
					StartCoroutine(CheckIfPlayerStraightLine());
					PAI.SearchForPlayer = true;
					PAI.SearchAIblock();
					return;
				}
			}
			else if (GameMaster.instance.PlayerDown[i] && GoingToPlayer)
			{
				aPlayerIsDown = true;
				if (!boosting && canBoost)
				{
					ActivateBooster();
					return;
				}
			}
		}
		if (GoingToPlayer && !aPlayerIsDown)
		{
			DownedPlayer = null;
			DisablePathFinding();
		}
	}

	public void SetDestination(Vector3 dest)
	{
		foreach (EnemyDetection ED4 in Ring1Detection)
		{
			ED4.isChosen = false;
		}
		foreach (EnemyDetection ED3 in Ring2Detection)
		{
			ED3.isChosen = false;
		}
		foreach (EnemyDetection ED2 in Ring3Detection)
		{
			ED2.isChosen = false;
		}
		float closestDistanceSoFar = 10000f;
		EnemyDetection closestED = null;
		foreach (EnemyDetection ED in Ring1Detection)
		{
			float dist = Vector3.Distance(dest, ED.transform.position);
			if (dist < closestDistanceSoFar)
			{
				closestED = ED;
				closestDistanceSoFar = dist;
			}
		}
		if (closestED != null)
		{
			isPathfinding = true;
			closestED.isChosen = true;
			chosenED = closestED;
			isMoving = true;
		}
		else
		{
			isPathfinding = false;
			hasGottenPath = false;
		}
	}

	private void DisablePathFinding()
	{
		hasGottenPath = false;
		isPathfinding = false;
		GoingToPlayer = false;
		PAI.SearchForPlayer = false;
		CanGoStraightToPlayer = false;
	}

	private void MoveTank()
	{
		if (boosting)
		{
			TankSpeed = BoostedTankSpeed;
		}
		int amountTargeted = 0;
		foreach (EnemyDetection ED in Ring1Detection)
		{
			if (ED.isTargeted && ED.Bullets.Count > 0)
			{
				amountTargeted++;
			}
		}
		if (GoingToPlayer && CanGoStraightToPlayer && (bool)DownedPlayer)
		{
			Vector3 ChosenPos = DownedPlayer.transform.position;
			Vector3 targetDir = ((!DrivingBackwards) ? (ChosenPos - base.transform.position) : (base.transform.position - ChosenPos));
			targetDir.y = 0f;
			rotation = Quaternion.LookRotation(targetDir);
			if (!rigi.isKinematic)
			{
				if (DrivingBackwards)
				{
					rigi.AddRelativeForce(-Vector3.forward * TankSpeed * Time.deltaTime * 50f);
				}
				else
				{
					rigi.AddRelativeForce(Vector3.forward * TankSpeed * Time.deltaTime * 50f);
				}
			}
			return;
		}
		if (HasPathfinding && hasGottenPath)
		{
			if (LayMines && !GoingToPlayer)
			{
				Collider[] objectsInRange = Physics.OverlapSphere(base.transform.position, 1.5f);
				Collider[] array = objectsInRange;
				foreach (Collider col in array)
				{
					if (col.gameObject.layer == LayerMask.NameToLayer("CorkWall"))
					{
						DisablePathFinding();
					}
				}
			}
			if (!Ring0Detection.isTargeted && amountTargeted < 2 && !isPathfinding)
			{
				return;
			}
			if (Ring0Detection.isTargeted && Ring0Detection.TargetDanger >= 22)
			{
				DisablePathFinding();
			}
			else if (amountTargeted >= 3)
			{
				DisablePathFinding();
			}
			if (hasGottenPath && isPathfinding && (bool)ETSN)
			{
				if (isAggressive && !isLevel100Boss && ETSN.TargetInSight && !GoingToPlayer)
				{
					DisablePathFinding();
				}
				Vector3 ChosenPos2 = preferredLocation;
				Vector3 targetDir2 = ((!DrivingBackwards) ? (ChosenPos2 - base.transform.position) : (base.transform.position - ChosenPos2));
				targetDir2.y = 0f;
				rotation = Quaternion.LookRotation(targetDir2);
				if (!rigi.isKinematic)
				{
					if (DrivingBackwards)
					{
						rigi.AddRelativeForce(-Vector3.forward * TankSpeed * Time.deltaTime * 50f);
					}
					else
					{
						rigi.AddRelativeForce(Vector3.forward * TankSpeed * Time.deltaTime * 50f);
					}
				}
				float distToPoint = Vector3.Distance(base.transform.position, preferredLocation);
				if (distToPoint < 1.5f)
				{
					PAI.NextPoint();
				}
				else if (distToPoint > 4.5f)
				{
					DisablePathFinding();
				}
				return;
			}
		}
		if (isMoving && !noSafeED)
		{
			float dist = Vector3.Distance(chosenED.transform.position, base.transform.position);
			if (Ring0Detection.isTargeted && Ring0Detection.Bullets.Count > 0 && (bool)Rush)
			{
				float closestBulletDistance = 100f;
				foreach (Collider Bullet in Ring0Detection.Bullets)
				{
					float distance = Vector3.Distance(Bullet.transform.position, base.transform.position);
					if (distance < closestBulletDistance)
					{
						closestBulletDistance = distance;
					}
				}
				if (closestBulletDistance < 8f && (bool)Rush && !boosting && canBoost)
				{
					ActivateBooster();
					StartCoroutine(StopBoostingAfter(1.5f));
				}
			}
			if ((chosenED.isTargeted && difficulty > 0) || chosenED.inWall)
			{
				chosenED.isChosen = false;
				CheckForSafeED(newdir: false);
			}
			if (dist < 0.3f)
			{
				isMoving = false;
				chosenED.isChosen = false;
				CheckForSafeED(newdir: false);
			}
			else
			{
				lastKnownDist = dist;
				if (Random.Range(0, 14) == 0 && !changeCooldown)
				{
					isMoving = false;
					chosenED.isChosen = false;
					changeCooldown = true;
					StartCoroutine(ResetChange());
					CheckForSafeED(newdir: true);
				}
				if (CanMove)
				{
					MoveAndRotateTank();
				}
			}
		}
		if (Ring0Detection.isTargeted && !isMoving && !noSafeED)
		{
			if ((bool)chosenED)
			{
				chosenED.isChosen = false;
			}
			CheckForSafeED(newdir: false);
		}
	}

	private void MoveAndRotateTank()
	{
		Vector3 ChosenPos = new Vector3(chosenED.transform.position.x, chosenED.transform.position.y, chosenED.transform.position.z);
		Vector3 targetDir = ((!DrivingBackwards) ? (ChosenPos - base.transform.position) : (base.transform.position - ChosenPos));
		targetDir.y = 0f;
		rotation = Quaternion.LookRotation(targetDir.normalized);
		if (DrivingBackwards)
		{
			rigi.AddRelativeForce(-Vector3.forward * TankSpeed * Time.deltaTime * 50f);
		}
		else
		{
			rigi.AddRelativeForce(Vector3.forward * TankSpeed * Time.deltaTime * 50f);
		}
	}

	private void ActivateBooster()
	{
		TankSpeed = BoostedTankSpeed;
		if (!BoostSource.isPlaying)
		{
			BoostSource.Play();
		}
		ParticleSystem[] PE = Rush.transform.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] PEback = RushBackwards.transform.GetComponentsInChildren<ParticleSystem>();
		if (DrivingBackwards)
		{
			ParticleSystem[] array = PEback;
			foreach (ParticleSystem ps2 in array)
			{
				if (!ps2.isPlaying)
				{
					ps2.Play();
				}
			}
		}
		else
		{
			ParticleSystem[] array2 = PE;
			foreach (ParticleSystem ps in array2)
			{
				if (!ps.isPlaying)
				{
					ps.Play();
				}
			}
		}
		ParticleSystem.MainModule main = skidMarkCreator.main;
		if (!boosting)
		{
			if (GameMaster.instance.CurrentMission == 49)
			{
				skidMarkCreatorLvl50.Stop();
				main.duration = originalDurationParticles / 1.5f;
				skidMarkCreatorLvl50.Play();
			}
			else
			{
				skidMarkCreator.Stop();
				main.duration = originalDurationParticles / 1.5f;
				skidMarkCreator.Play();
			}
		}
		if (!boosting)
		{
			boosting = true;
		}
	}

	private IEnumerator StopBoostingAfter(float sec)
	{
		yield return new WaitForSeconds(sec);
		DeactivateBooster();
	}

	private void DeactivateBooster()
	{
		TankSpeed = OriginalTankSpeed;
		if (BoostSource.isPlaying)
		{
			BoostSource.Stop();
		}
		Rush.GetComponent<AudioSource>().Stop();
		ParticleSystem[] PE = Rush.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] PEback = RushBackwards.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = PE;
		foreach (ParticleSystem ps in array)
		{
			if (ps.isPlaying)
			{
				ps.Stop();
			}
		}
		ParticleSystem[] array2 = PEback;
		foreach (ParticleSystem ps2 in array2)
		{
			if (ps2.isPlaying)
			{
				ps2.Stop();
			}
		}
		skidMarkCreatorLvl50.Stop();
		ParticleSystem.MainModule main = skidMarkCreator.main;
		if (boosting)
		{
			if (GameMaster.instance.CurrentMission == 49)
			{
				skidMarkCreatorLvl50.Stop();
				main.duration = originalDurationParticles;
				skidMarkCreatorLvl50.Play();
			}
			else
			{
				skidMarkCreator.Stop();
				main.duration = originalDurationParticles;
				skidMarkCreator.Play();
			}
		}
		if (boosting)
		{
			boosting = false;
		}
	}
}
