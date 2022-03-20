using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	[Header("AI Behaviour")]
	public bool IsCompanion;

	public int CompanionID = 1;

	public int difficulty;

	public int NotSeeBulletChancePercentage = 5;

	public float Accuracy = 10f;

	public bool CanMove = true;

	public bool LayMines;

	public float LayMinesSpeed;

	public float MineSenseRange = 5f;

	public float ShootSpeed = 3f;

	public bool BouncyBullets;

	public int amountOfBounces = 2;

	public bool isInvisible;

	public GameObject MagicPoof;

	public bool hasRockets;

	public bool isAggressive;

	public bool CalculateEnemyPosition;

	public bool isSmart;

	public bool isElectric;

	public bool isCharged;

	public bool isAggro;

	public bool isWallHugger;

	public GameObject[] ElectricComponents;

	public ParticleSystem[] ElectricSystems;

	public ParticleSystem AngrySteam;

	public float ElectricMeter;

	public Vector3 ElectricTargetLocation;

	private Vector3 StartPos;

	public float timeTakenDuringLerp = 1f;

	public float _timeStartedLerping;

	public bool isTransporting;

	private bool isInvisibleNow;

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

	public bool IsTurning;

	public Transform BaseRaycastPoint;

	public float boosterFluid = 2f;

	public float maxFluid = 1f;

	public bool canBoost;

	public GameObject Rush;

	public GameObject RushBackwards;

	private AudioSource BoostSource;

	private float originalDurationParticles;

	public ParticleSystem skidMarkCreatorLvl50;

	public bool boosting;

	private Quaternion targetRotation;

	public AudioClip TankTracks;

	public AudioSource source;

	public Rigidbody rigi;

	public ParticleSystem skidMarkCreator;

	public Transform skidMarkLocation;

	public Vector3 originalLocation;

	public Quaternion originalRotation;

	public float rotyOriginal;

	public bool MineFleeing;

	public bool isLevel10Boss;

	public bool isLevel30Boss;

	public int lvl30Boss2modeLives = 35;

	public int lvl30Boss3modeLives = 20;

	public bool isLevel50Boss;

	public bool isLevel70Boss;

	public bool isLevel100Boss;

	public GameObject groundChecker;

	[HideInInspector]
	public float lvl30BossTurnSpeed = 50f;

	public bool movingToPreferred;

	[Header("Raycast Stuff")]
	public GameObject THEGRID;

	public EnemyDetection Ring0Detection;

	public List<EnemyDetection> Ring1Detection = new List<EnemyDetection>();

	public List<EnemyDetection> Ring2Detection = new List<EnemyDetection>();

	public List<EnemyDetection> Ring3Detection = new List<EnemyDetection>();

	public Vector3 TargetPosition;

	public bool Targeted;

	public float angle;

	public float desiredangle;

	public bool armoured;

	public float OriginalShootSpeed;

	public List<GameObject> MineinCooldown = new List<GameObject>();

	public EnemyTargetingSystemNew ETSN;

	public bool gettingCloser;

	public float lastDistanceToPlayer = 10000f;

	public float marge = 5f;

	[Header("electric stuff")]
	public bool isStunned;

	public float stunTimer;

	public ParticleSystem StunnedParticles;

	[HideInInspector]
	public int bulletBounces = -1;

	public HealthTanks HTscript;

	public GameObject armour;

	public bool couldMove;

	public bool DrivingBackwards;

	public bool HasPathfinding;

	public bool isPathfinding;

	public float NavMeshTimer;

	public float DownPlayerTimer;

	[Header("SHINY stuff")]
	public bool isShiny;

	public bool isSuperShiny;

	public ParticleSystem ShinySystem;

	public ParticleSystem SuperShinySystem;

	public MeshRenderer Body;

	public MeshRenderer Barrel;

	private PathfindingAI PAI;

	public GameObject AngerVein;

	public AudioClip AngerTracks;

	public AudioClip spotted;

	public AudioClip cooldown;

	private float TimeBeingAnger;

	private float coolingDown;

	public bool ElectricTeleportRunning;

	private ElectricPad UnderEP;

	private Vector3 previousPosition;

	private Quaternion rotation;

	private float angleSize;

	public bool isMoving;

	public float lastKnownDist;

	public EnemyDetection chosenED;

	public bool noSafeED;

	public int direction;

	public int directionOffset;

	private bool changeCooldown;

	public GameObject DownedPlayer;

	public bool GoingToPlayer;

	public bool CanGoStraightToPlayer;

	public Vector3 preferredLocation;

	public bool hasGottenPath;

	private void Awake()
	{
		bulletBounces = -1;
		PAI = GetComponent<PathfindingAI>();
		if (Random.Range(0, 16384) == 1 && !IsCompanion && MapEditorMaster.instance == null)
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
		else if (Random.Range(0, 114688) == 1 && !IsCompanion && MapEditorMaster.instance == null)
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
				EnemyDetection component = THEGRID.transform.GetChild(i).GetComponent<EnemyDetection>();
				component.papaTank = base.gameObject;
				int num = 16;
				int num2 = 16;
				int num3 = 16;
				component.ID = i;
				if (i == 0)
				{
					Ring0Detection = component;
				}
				else if (i > 0 && i <= num)
				{
					Ring1Detection.Add(component);
					component.isRing1 = true;
				}
				else if (i > num && i <= num + num2)
				{
					Ring2Detection.Add(component);
				}
				else if (i > num + num2 && i <= num + num2 + num3)
				{
					Ring3Detection.Add(component);
				}
			}
		}
		if (CanMove)
		{
			EnemyDetection[] array;
			if (difficulty < 2)
			{
				array = Ring2Detection.Concat(Ring3Detection).ToArray();
				for (int j = 0; j < array.Length; j++)
				{
					Object.Destroy(array[j].gameObject);
				}
				array = Ring3Detection.ToArray();
				for (int j = 0; j < array.Length; j++)
				{
					Object.Destroy(array[j].gameObject);
				}
			}
			else if (difficulty < 3)
			{
				array = Ring3Detection.ToArray();
				for (int j = 0; j < array.Length; j++)
				{
					Object.Destroy(array[j].gameObject);
				}
			}
			array = Ring3Detection.ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				Object.Destroy(array[j].gameObject);
			}
			Ring3Detection.Clear();
			THEGRID.SetActive(value: false);
		}
		else if ((bool)THEGRID)
		{
			EnemyDetection[] array = Ring2Detection.Concat(Ring1Detection).Concat(Ring3Detection).ToArray();
			for (int j = 0; j < array.Length; j++)
			{
				Object.Destroy(array[j].gameObject);
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
			Transform parent = base.gameObject.transform.parent;
			ETSN = parent.GetChild(1).GetChild(0).GetComponent<EnemyTargetingSystemNew>();
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
		float num;
		if (TankSpeed < 80f)
		{
			num = 0.85f - TankSpeed / 90f;
			if (num < 0.12f)
			{
				num = 0.12f;
			}
		}
		else
		{
			num = 0.1f;
		}
		main.duration = num;
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
		if (!isLevel10Boss && !isLevel30Boss && !isLevel50Boss)
		{
			_ = isLevel70Boss;
		}
		if (IsCompanion)
		{
			InvokeRepeating("CheckForDownedPlayer", 0.4f, 0.4f);
		}
		if ((bool)MapEditorMaster.instance && !isShiny && !isSuperShiny && (bool)GameObject.Find("Cube.003").GetComponent<MeshRenderer>() && Body != null && MyTeam > -1 && MapEditorMaster.instance.TeamColorEnabled[MyTeam])
		{
			Body.materials[0].SetColor("_Color", MapEditorMaster.instance.TeamColors[MyTeam]);
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
				SkidmarkController component = skidMarkCreator.GetComponent<SkidmarkController>();
				component.startingColor = MapEditorMaster.instance.TeamColors[MyTeam];
				component.StartCoroutine(component.SetTrackColor());
			}
			else
			{
				SkidmarkController component2 = skidMarkCreator.GetComponent<SkidmarkController>();
				component2.startingColor = component2.originalColor;
				component2.StartCoroutine(component2.SetTrackColor());
			}
		}
		else
		{
			SkidmarkController component3 = skidMarkCreator.GetComponent<SkidmarkController>();
			component3.startingColor = component3.originalColor;
			component3.StartCoroutine(component3.SetTrackColor());
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
		Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.tag != "Temp")
			{
				if (renderer.tag != "AlwaysVisible")
				{
					renderer.enabled = false;
				}
				else
				{
					renderer.enabled = true;
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
		List<GameObject> list = new List<GameObject>();
		for (int num = IncomingBullets.Count - 1; num > -1; num--)
		{
			if (IncomingBullets[num] == null)
			{
				list.Add(IncomingBullets[num]);
			}
		}
		foreach (GameObject item in list)
		{
			IncomingBullets.Remove(item);
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
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		GameObject obj2 = Object.Instantiate(rotation: Quaternion.Euler(new Vector3(eulerAngles.x + 90f, eulerAngles.y, eulerAngles.z)), original: MagicPoof, position: base.transform.position + new Vector3(0f, 0.5f, 0f));
		obj2.GetComponent<ParticleSystem>().Play();
		Object.Destroy(obj2, 3f);
	}

	private IEnumerator MakeMeVisible()
	{
		yield return new WaitForSeconds(0.01f);
		Renderer[] componentsInChildren = base.transform.parent.gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.tag != "Temp" && renderer.tag != "EnemyBorder" && renderer.tag != "BulletDetection")
			{
				if (renderer.tag != "AlwaysVisible")
				{
					renderer.enabled = true;
				}
				else
				{
					renderer.enabled = false;
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
		Rigidbody component = GetComponent<Rigidbody>();
		if ((bool)component && GameMaster.instance.CurrentMission != 49)
		{
			component.isKinematic = true;
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
			Vector3 validLocation = GameMaster.instance.GetValidLocation(CheckForDist: true, 8f, base.transform.position, TargetPlayer: false);
			if (validLocation == Vector3.zero)
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
				CameraShake component = Camera.main.GetComponent<CameraShake>();
				if ((bool)component)
				{
					component.StartCoroutine(component.Shake(0.15f, 0.15f));
				}
			}
			isTransporting = true;
			Ring0Detection.SolidPieces.Clear();
			EnemyDetection[] array = Ring1Detection.Concat(Ring2Detection).Concat(Ring3Detection).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SolidPieces.Clear();
			}
			ETSN.normalLockedIn = false;
			ETSN.ShootCountdown = ShootSpeed;
			ElectricTargetLocation = new Vector3(validLocation.x, base.transform.parent.transform.position.y, validLocation.z);
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
		Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y - 1f, base.transform.position.z);
		Vector3 vector2 = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
		Vector3 vector3 = vector - vector2;
		float maxDistance = 4f;
		RaycastHit[] array = Physics.RaycastAll(vector2, vector3, maxDistance);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (!(raycastHit.transform.tag == "ElectricPad"))
			{
				continue;
			}
			ElectricPad component = raycastHit.transform.GetComponent<ElectricPad>();
			if (component != UnderEP)
			{
				if ((bool)UnderEP)
				{
					UnderEP.Active = false;
				}
				if (!isTransporting)
				{
					component.Active = true;
				}
				UnderEP = component;
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
			float num = (Time.time - _timeStartedLerping) / timeTakenDuringLerp;
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<BoxCollider>().enabled = false;
			base.transform.position = Vector3.Lerp(StartPos, ElectricTargetLocation, num);
			if (!(Vector3.Distance(base.transform.position, ElectricTargetLocation) < 1f) && !(num >= 1f))
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
				CameraShake component = Camera.main.GetComponent<CameraShake>();
				if ((bool)component)
				{
					component.StartCoroutine(component.Shake(0.15f, 0.15f));
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
				float num2 = (isLevel70Boss ? 0.18f : 0.22f);
				ElectricMeter += Time.deltaTime * num2;
				GameObject[] electricComponents = ElectricComponents;
				for (int i = 0; i < electricComponents.Length; i++)
				{
					electricComponents[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
				}
				ParticleSystem[] electricSystems = ElectricSystems;
				foreach (ParticleSystem particleSystem in electricSystems)
				{
					particleSystem.gameObject.SetActive(value: false);
					if (particleSystem.isPlaying)
					{
						particleSystem.Stop();
					}
				}
			}
			else if (!isCharged && ElectricMeter >= 1f)
			{
				GameObject[] electricComponents = ElectricComponents;
				for (int i = 0; i < electricComponents.Length; i++)
				{
					electricComponents[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white);
				}
				ParticleSystem[] electricSystems = ElectricSystems;
				foreach (ParticleSystem particleSystem2 in electricSystems)
				{
					particleSystem2.gameObject.SetActive(value: true);
					if (!particleSystem2.isPlaying)
					{
						particleSystem2.Play();
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
			int num = Ring1Detection.Count;
			List<EnemyDetection> list = new List<EnemyDetection>();
			foreach (EnemyDetection item in Ring1Detection)
			{
				if (item.MineClose)
				{
					list.Add(item);
					num--;
				}
			}
			if (num <= 1 && list.Count > 0 && !toLayMine)
			{
				list = list.OrderByDescending((EnemyDetection x) => x.DistToMine).ToList();
				list[0].isTargeted = false;
				SetNewTarget(list[0], isPreferred: false, useBoost: false);
				return true;
			}
			if (num <= 6 && list.Count > 0 && toLayMine)
			{
				return true;
			}
		}
		if (Ring2Detection.Count > 0 && toLayMine)
		{
			int num2 = Ring2Detection.Count;
			foreach (EnemyDetection item2 in Ring2Detection)
			{
				if (item2.MineClose)
				{
					num2--;
				}
			}
			if (num2 <= 8)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckRing(List<EnemyDetection> Ring, int newDirection, bool newdir)
	{
		int num = 0;
		direction = newDirection;
		if (direction < 0)
		{
			direction = 16 - direction;
		}
		if (direction > 15)
		{
			direction -= 15;
		}
		bool flag = false;
		int num2 = (newdir ? (direction - 3) : (direction - 2));
		int num3 = (newdir ? (direction + 3) : (direction + 2));
		if (num2 < 0)
		{
			num2 = 16 + num2;
		}
		if (num3 > 15)
		{
			num3 -= 15;
		}
		if (Mathf.Abs(num3 - num2) > 7)
		{
			flag = true;
		}
		List<EnemyDetection> list = new List<EnemyDetection>();
		List<int> list2 = new List<int>();
		List<EnemyDetection> list3 = new List<EnemyDetection>();
		List<int> list4 = new List<int>();
		List<EnemyDetection> list5 = new List<EnemyDetection>();
		List<int> list6 = new List<int>();
		foreach (EnemyDetection item in Ring)
		{
			if (!item.isTargeted && num == direction)
			{
				list5.Add(item);
				list6.Add(num);
			}
			else
			{
				if (item.isPreferred && !item.isTargeted)
				{
					SetNewTarget(item, isPreferred: true, useBoost: true);
					return true;
				}
				if (!item.isTargeted && ((num >= num2 && num <= num3 && !flag) || num >= num2 || (num <= num3 && flag)))
				{
					list3.Add(item);
					list4.Add(num);
				}
				else if (!item.isTargeted)
				{
					list.Add(item);
					list2.Add(num);
				}
				else if (item.isChosen)
				{
					item.isChosen = false;
				}
			}
			num++;
		}
		if (list5.Count > 0)
		{
			int index = Random.Range(0, list5.Count - 1);
			direction = list6[index];
			SetNewTarget(list5[index], isPreferred: false, useBoost: false);
			return true;
		}
		if (list3.Count > 0)
		{
			int index2 = Random.Range(0, list3.Count - 1);
			direction = list4[index2];
			SetNewTarget(list3[index2], isPreferred: false, useBoost: false);
			return true;
		}
		if (list.Count > 0)
		{
			int index3 = Random.Range(0, list.Count - 1);
			direction = list2[index3];
			SetNewTarget(list[index3], isPreferred: false, useBoost: false);
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
		float seconds = Random.Range(0.4f, 0.9f);
		yield return new WaitForSeconds(seconds);
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
		float seconds = Random.Range(0.2f, 0.4f);
		yield return new WaitForSeconds(seconds);
		if (!DownedPlayer)
		{
			GoingToPlayer = false;
			CanGoStraightToPlayer = false;
			yield break;
		}
		Vector3 vector = DownedPlayer.transform.position - base.transform.position;
		Debug.DrawRay(base.transform.position, vector * 8f, Color.blue, 1f);
		bool flag = false;
		if (Physics.Raycast(base.transform.position, vector, out var hitInfo, 8f, layersToIgnore))
		{
			Debug.Log("Companion scanning for player, but is seeing:" + hitInfo.transform.name, hitInfo.transform.gameObject);
			if (hitInfo.transform.gameObject == DownedPlayer)
			{
				Debug.Log("PLAYER STRAGITH LINE!!");
				flag = true;
				GoingToPlayer = true;
				CanGoStraightToPlayer = true;
			}
		}
		if (!flag && GoingToPlayer)
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
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			if (GameMaster.instance.PlayerDown[i] && !GoingToPlayer)
			{
				flag = true;
				foreach (GameObject player in GameMaster.instance.Players)
				{
					MoveTankScript component = player.GetComponent<MoveTankScript>();
					if ((bool)component)
					{
						if (component.playerId == i)
						{
							DownedPlayer = player;
							StartCoroutine(CheckIfPlayerStraightLine());
							PAI.SearchForPlayer = true;
							PAI.SearchAIblock();
							return;
						}
					}
					else if (player.GetComponent<EnemyAI>().CompanionID == i)
					{
						DownedPlayer = player;
						StartCoroutine(CheckIfPlayerStraightLine());
						PAI.SearchForPlayer = true;
						PAI.SearchAIblock();
						return;
					}
				}
			}
			else if (GameMaster.instance.PlayerDown[i] && GoingToPlayer)
			{
				flag = true;
				if (!boosting && canBoost)
				{
					ActivateBooster();
					return;
				}
			}
		}
		if (GoingToPlayer && !flag)
		{
			DownedPlayer = null;
			DisablePathFinding();
		}
	}

	public void SetDestination(Vector3 dest)
	{
		foreach (EnemyDetection item in Ring1Detection)
		{
			item.isChosen = false;
		}
		foreach (EnemyDetection item2 in Ring2Detection)
		{
			item2.isChosen = false;
		}
		foreach (EnemyDetection item3 in Ring3Detection)
		{
			item3.isChosen = false;
		}
		float num = 10000f;
		EnemyDetection enemyDetection = null;
		foreach (EnemyDetection item4 in Ring1Detection)
		{
			float num2 = Vector3.Distance(dest, item4.transform.position);
			if (num2 < num)
			{
				enemyDetection = item4;
				num = num2;
			}
		}
		if (enemyDetection != null)
		{
			isPathfinding = true;
			enemyDetection.isChosen = true;
			chosenED = enemyDetection;
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
		int num = 0;
		foreach (EnemyDetection item in Ring1Detection)
		{
			if (item.isTargeted && item.Bullets.Count > 0)
			{
				num++;
			}
		}
		if (GoingToPlayer && CanGoStraightToPlayer && (bool)DownedPlayer)
		{
			Vector3 position = DownedPlayer.transform.position;
			Vector3 forward = ((!DrivingBackwards) ? (position - base.transform.position) : (base.transform.position - position));
			forward.y = 0f;
			rotation = Quaternion.LookRotation(forward);
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
				Collider[] array = Physics.OverlapSphere(base.transform.position, 1.5f);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].gameObject.layer == LayerMask.NameToLayer("CorkWall"))
					{
						DisablePathFinding();
					}
				}
			}
			if (!Ring0Detection.isTargeted && num < 2 && !isPathfinding)
			{
				return;
			}
			if (Ring0Detection.isTargeted && Ring0Detection.TargetDanger >= 22)
			{
				DisablePathFinding();
			}
			else if (num >= 3)
			{
				DisablePathFinding();
			}
			if (hasGottenPath && isPathfinding && (bool)ETSN)
			{
				if (isAggressive && !isLevel100Boss && ETSN.TargetInSight && !GoingToPlayer)
				{
					DisablePathFinding();
				}
				Vector3 vector = preferredLocation;
				Vector3 forward2 = ((!DrivingBackwards) ? (vector - base.transform.position) : (base.transform.position - vector));
				forward2.y = 0f;
				rotation = Quaternion.LookRotation(forward2);
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
				float num2 = Vector3.Distance(base.transform.position, preferredLocation);
				if (num2 < 1.5f)
				{
					PAI.NextPoint();
				}
				else if (num2 > 4.5f)
				{
					DisablePathFinding();
				}
				return;
			}
		}
		if (isMoving && !noSafeED)
		{
			float num3 = Vector3.Distance(chosenED.transform.position, base.transform.position);
			if (Ring0Detection.isTargeted && Ring0Detection.Bullets.Count > 0 && (bool)Rush)
			{
				float num4 = 100f;
				foreach (Collider bullet in Ring0Detection.Bullets)
				{
					float num5 = Vector3.Distance(bullet.transform.position, base.transform.position);
					if (num5 < num4)
					{
						num4 = num5;
					}
				}
				if (num4 < 8f && (bool)Rush && !boosting && canBoost)
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
			if (num3 < 0.3f)
			{
				isMoving = false;
				chosenED.isChosen = false;
				CheckForSafeED(newdir: false);
			}
			else
			{
				lastKnownDist = num3;
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
		Vector3 vector = new Vector3(chosenED.transform.position.x, chosenED.transform.position.y, chosenED.transform.position.z);
		Vector3 vector2 = ((!DrivingBackwards) ? (vector - base.transform.position) : (base.transform.position - vector));
		vector2.y = 0f;
		rotation = Quaternion.LookRotation(vector2.normalized);
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
		ParticleSystem[] componentsInChildren = Rush.transform.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] componentsInChildren2 = RushBackwards.transform.GetComponentsInChildren<ParticleSystem>();
		if (DrivingBackwards)
		{
			ParticleSystem[] array = componentsInChildren2;
			foreach (ParticleSystem particleSystem in array)
			{
				if (!particleSystem.isPlaying)
				{
					particleSystem.Play();
				}
			}
		}
		else
		{
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem2 in array)
			{
				if (!particleSystem2.isPlaying)
				{
					particleSystem2.Play();
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
		ParticleSystem[] componentsInChildren = Rush.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] componentsInChildren2 = RushBackwards.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			if (particleSystem.isPlaying)
			{
				particleSystem.Stop();
			}
		}
		array = componentsInChildren2;
		foreach (ParticleSystem particleSystem2 in array)
		{
			if (particleSystem2.isPlaying)
			{
				particleSystem2.Stop();
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
