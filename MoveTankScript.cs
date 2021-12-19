using System;
using System.Collections;
using Rewired;
using UnityEngine;

public class MoveTankScript : MonoBehaviour
{
	public bool DrivingBackwards;

	public float TankSpeed = 5000f;

	private float TankNormalSpeed = 75f;

	public float TankUpgradedSpeed = 90f;

	public float TankTowerBoostSpeed = 90f;

	public int SpeedTime = 5;

	public bool speedIsUpgraded;

	public float turnSpeed = 10f;

	public bool isPlayer2;

	public int playerId;

	public Joystick joystick;

	private Vector2 input;

	public float angle;

	public int MyTeam;

	private Quaternion targetRotation;

	public Transform cam;

	public Rigidbody rigi;

	public AudioClip TankTracks;

	public AudioClip TankTracksFast;

	public AudioClip TankTracksCarpet;

	public AudioClip TankTracksSnow;

	public AudioClip Honk;

	public AudioSource source;

	public bool driving;

	public Transform skidMarkLocation;

	public float boosterFluid = 2f;

	public float maxFluid = 1f;

	public bool canBoost;

	public GameObject Rush;

	public GameObject BackwardsRush;

	private AudioSource BoostSource;

	public HealthTanks HTtanks;

	private float originalDurationParticles;

	public bool boosting;

	public bool mobileBoosting;

	public bool isStunned;

	public float stunTimer;

	public bool isBeingTowerBoosted;

	public ParticleSystem TowerBoostEffect;

	public ParticleSystem StunnedParticles;

	public Transform TankHead;

	[Header("Survival Game Options")]
	public UpgradeField UpgField;

	public int[] Upgrades;

	public int[] UpgradedSpeeds;

	[Header("TankTracks")]
	public ParticleSystem skidMarkCreator;

	public ParticleSystem skidMarkCreatorLvl50;

	public GameObject ColoredTankTracks;

	public Player player;

	public Controller playerController;

	public bool IsUsingController;

	public bool[] SolidDetection;

	private float slidingH;

	private float slidingV;

	public float sensi = 8f;

	private bool canHonkIt = true;

	public float offset;

	private void OnEnable()
	{
		HTtanks = GetComponent<HealthTanks>();
		if (GameMaster.instance != null && base.name == "Second_Tank_FBX_body")
		{
			isPlayer2 = true;
		}
		if (GameMaster.instance.CurrentMission != 49)
		{
			GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)80;
		}
		skidMarkCreator.Stop();
	}

	private void Awake()
	{
		player = ReInput.players.GetPlayer(playerId);
		Debug.unityLogger.logEnabled = false;
		TankNormalSpeed = TankSpeed;
		BoostSource = Rush.GetComponent<AudioSource>();
		InvokeRepeating("CheckTypeInput", 0.1f, 0.1f);
		InvokeRepeating("AlertTeleportationFields", 1f, 1f);
		skidMarkCreator.Stop();
		if (GameMaster.instance.isOfficialCampaign)
		{
			base.transform.position = new Vector3(base.transform.position.x, 0.04f, base.transform.position.z);
		}
	}

	private void AlertTeleportationFields()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 3f);
		foreach (Collider collider in array)
		{
			if (collider.tag == "TeleportBlock")
			{
				TeleportationBlock component = collider.GetComponent<TeleportationBlock>();
				if ((bool)component)
				{
					component.ActivateMe(2f);
				}
			}
		}
	}

	private void Start()
	{
		if (MyTeam < 0)
		{
			MyTeam = 1;
		}
		if (OptionsMainMenu.instance.AMselected.Contains(1) && ColoredTankTracks != null)
		{
			skidMarkCreator.gameObject.SetActive(value: false);
			ColoredTankTracks.SetActive(value: true);
			skidMarkCreator = ColoredTankTracks.GetComponent<ParticleSystem>();
		}
		originalDurationParticles = skidMarkCreator.main.duration;
		HTtanks = GetComponent<HealthTanks>();
		cam = Camera.main.transform;
		rigi = GetComponent<Rigidbody>();
		BoostSource = Rush.GetComponent<AudioSource>();
		if (GameMaster.instance != null)
		{
			if (OptionsMainMenu.instance.SnowMode && !GameMaster.instance.inMapEditor)
			{
				TankTracks = GameMaster.instance.TankTracksCarpetSound;
			}
			if (base.name == "Second_Tank_FBX_body")
			{
				isPlayer2 = true;
			}
			GameMaster.instance.PlayerAlive = true;
		}
		skidMarkCreator.Pause();
		skidMarkCreator.Stop();
		GameMaster.instance.PlayerJoined[playerId] = true;
	}

	private void CheckTypeInput()
	{
		if (rigi == null)
		{
			rigi = GetComponent<Rigidbody>();
		}
		float num = 1.1f;
		LayerMask layerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("CorkWall"));
		if (Physics.Raycast(base.transform.position, Vector3.forward * num, out var hitInfo, num, layerMask))
		{
			if (hitInfo.transform.tag == "Solid" || hitInfo.transform.tag == "MapBorder")
			{
				SolidDetection[0] = true;
			}
			else
			{
				SolidDetection[0] = false;
			}
		}
		else
		{
			SolidDetection[0] = false;
		}
		if (Physics.Raycast(base.transform.position, -Vector3.left * num, out hitInfo, num, layerMask))
		{
			if (hitInfo.transform.tag == "Solid" || hitInfo.transform.tag == "MapBorder")
			{
				SolidDetection[1] = true;
			}
			else
			{
				SolidDetection[1] = false;
			}
		}
		else
		{
			SolidDetection[1] = false;
		}
		if (Physics.Raycast(base.transform.position, Vector3.left * num, out hitInfo, num, layerMask))
		{
			if (hitInfo.transform.tag == "Solid" || hitInfo.transform.tag == "MapBorder")
			{
				SolidDetection[3] = true;
			}
			else
			{
				SolidDetection[3] = false;
			}
		}
		else
		{
			SolidDetection[3] = false;
		}
		if (Physics.Raycast(base.transform.position, -Vector3.forward * num, out hitInfo, num, layerMask))
		{
			if (hitInfo.transform.tag == "Solid" || hitInfo.transform.tag == "MapBorder")
			{
				SolidDetection[2] = true;
			}
			else
			{
				SolidDetection[2] = false;
			}
		}
		else
		{
			SolidDetection[2] = false;
		}
	}

	public void StunMe(float sec)
	{
		if (GameMaster.instance.CurrentMission == 69)
		{
			AchievementsTracker.instance.HasBeenStunnedByBoss = true;
		}
		isStunned = true;
		stunTimer = sec;
		StunnedParticles.Play();
	}

	private void FixedUpdate()
	{
		if (GameMaster.instance.GameHasPaused || isStunned)
		{
			return;
		}
		if ((bool)TowerBoostEffect)
		{
			if (isBeingTowerBoosted)
			{
				if (!TowerBoostEffect.isPlaying)
				{
					TowerBoostEffect.Play();
				}
				TankSpeed = TankTowerBoostSpeed;
			}
			else if (TowerBoostEffect.isPlaying)
			{
				TowerBoostEffect.Stop();
				TankSpeed = TankNormalSpeed;
			}
		}
		if (((GameMaster.instance.GameHasStarted && !GameMaster.instance.isZombieMode) || GameMaster.instance.isZombieMode) && ((HTtanks.health > 0 && (base.transform.rotation.eulerAngles.x > 350f || (base.transform.rotation.eulerAngles.x < 10f && base.transform.rotation.eulerAngles.z > 350f) || base.transform.rotation.eulerAngles.z < 10f)) || base.transform.position.y > 0.1f))
		{
			Move();
		}
	}

	private void CheckControllerType()
	{
		playerController = player.controllers.GetLastActiveController();
		if (playerController != null)
		{
			if (player.controllers.joystickCount > 0)
			{
				playerController = player.controllers.Joysticks[0];
				if (playerId == 0)
				{
					GameMaster.instance.isPlayingWithController = true;
				}
			}
			else
			{
				playerController = player.controllers.Keyboard;
				if (playerId == 0)
				{
					GameMaster.instance.isPlayingWithController = false;
				}
			}
			if (playerController.type != ControllerType.Joystick)
			{
				if (playerId != 0)
				{
					Debug.Log("No joystick");
				}
				IsUsingController = false;
			}
			else if (playerController.type == ControllerType.Joystick)
			{
				_ = playerId;
				IsUsingController = true;
			}
			else if (playerId != 0 && GameMaster.instance.GameHasStarted)
			{
				HTtanks.health = -100;
			}
		}
		else if (playerId != 0 && GameMaster.instance.GameHasStarted)
		{
			HTtanks.health = -100;
			GameMaster.instance.PlayerJoined[playerId] = false;
		}
	}

	private void Update()
	{
		CheckControllerType();
		if (GameMaster.instance.GameHasPaused)
		{
			skidMarkCreator.Stop();
			return;
		}
		if (!GameMaster.instance.GameHasStarted && !GameMaster.instance.isZombieMode)
		{
			rigi.isKinematic = true;
		}
		else if (!GameMaster.instance.isZombieMode)
		{
			rigi.isKinematic = false;
		}
		if (base.transform.position.y < -0.3f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y * 1.12f, base.transform.position.z);
			if (base.transform.position.y < -100.1f)
			{
				HTtanks.health = 0;
				HTtanks.amountRevivesLeft = -1;
			}
		}
		if (GameMaster.instance != null && GameMaster.instance.isZombieMode && TankSpeed != (float)UpgradedSpeeds[Upgrades[1]])
		{
			TankSpeed = UpgradedSpeeds[Upgrades[1]];
		}
		if (stunTimer > 0f)
		{
			isStunned = true;
			stunTimer -= Time.deltaTime;
		}
		else if (isStunned)
		{
			isStunned = false;
			StunnedParticles.Stop();
			StunnedParticles.Clear();
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
		if (OptionsMainMenu.instance.currentDifficulty == 3 && !GameMaster.instance.isZombieMode)
		{
			canBoost = false;
			TankSpeed = 80f;
		}
		GetInput();
		if (GameMaster.instance.isZombieMode && GameMaster.instance.CurrentMission == 3)
		{
			if (source.clip != TankTracksCarpet)
			{
				source.clip = TankTracksCarpet;
			}
		}
		else if (source.clip != TankTracks)
		{
			source.clip = TankTracks;
		}
		if (Time.timeScale < 0.5f || (!GameMaster.instance.GameHasStarted && !GameMaster.instance.isZombieMode))
		{
			source.loop = false;
			source.Stop();
			skidMarkCreatorLvl50.Stop();
			driving = false;
			skidMarkCreator.Stop();
			return;
		}
		if ((double)Math.Abs(input.x) < 0.5 && (double)Mathf.Abs(input.y) < 0.5)
		{
			if (source.isPlaying)
			{
				source.clip = null;
				source.Stop();
			}
			skidMarkCreatorLvl50.Stop();
			skidMarkCreator.Stop();
			driving = false;
			if (boosting)
			{
				DeactivateBooster();
			}
		}
		if ((double)Math.Abs(input.x) > 0.5 || (double)Mathf.Abs(input.y) > 0.5)
		{
			if (!GameMaster.instance.GameHasPaused)
			{
				CalculateDirection();
			}
			if (rigi.velocity.magnitude <= 0.7f)
			{
				if (source.isPlaying)
				{
					source.clip = null;
					source.Stop();
				}
				skidMarkCreatorLvl50.Stop();
				skidMarkCreator.Stop();
				if (boosting)
				{
					DeactivateBooster();
				}
			}
			else if (rigi.velocity.magnitude > 0.7f)
			{
				if (GameMaster.instance.CurrentMission == 49)
				{
					if (!skidMarkCreatorLvl50.isPlaying)
					{
						skidMarkCreator.Stop();
						skidMarkCreatorLvl50.Play();
					}
				}
				else if (!skidMarkCreator.isPlaying || !skidMarkCreator.isEmitting)
				{
					skidMarkCreatorLvl50.Stop();
					skidMarkCreator.Play();
				}
				if (!source.isPlaying)
				{
					source.loop = true;
					source.Play();
				}
			}
			if ((bool)AchievementsTracker.instance)
			{
				AchievementsTracker.instance.Moved = true;
			}
			driving = true;
		}
		if (!isStunned && driving && ((GameMaster.instance.GameHasStarted && !GameMaster.instance.isZombieMode) || GameMaster.instance.isZombieMode) && ((HTtanks.health > 0 && (base.transform.rotation.eulerAngles.x > 350f || (base.transform.rotation.eulerAngles.x < 10f && base.transform.rotation.eulerAngles.z > 350f) || base.transform.rotation.eulerAngles.z < 10f)) || base.transform.position.y > 0.1f))
		{
			Rotate();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Lvl50FloorZone" && GameMaster.instance.CurrentMission == 49)
		{
			skidMarkCreatorLvl50.trigger.SetCollider(0, other);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Lvl50FloorZone" && GameMaster.instance.CurrentMission == 49)
		{
			skidMarkCreatorLvl50.trigger.SetCollider(0, other);
		}
	}

	private void OnDisable()
	{
		source.loop = false;
		source.Stop();
		skidMarkCreator.Pause();
		skidMarkCreator.Stop();
		DeactivateBooster();
	}

	public IEnumerator UpgradeSpeed()
	{
		if (TankSpeed != TankUpgradedSpeed)
		{
			Debug.LogWarning("tank upgraded speed!");
			TankSpeed = TankUpgradedSpeed;
			yield return new WaitForSeconds(SpeedTime);
			TankSpeed = TankNormalSpeed;
		}
	}

	private Vector2 SmoothInput(float targetH, float targetV)
	{
		float num = sensi;
		float num2 = 0.0001f;
		slidingH = Mathf.MoveTowards(slidingH, targetH, num * Time.deltaTime);
		slidingV = Mathf.MoveTowards(slidingV, targetV, num * Time.deltaTime);
		return new Vector2((Mathf.Abs(slidingH) < num2) ? 0f : slidingH, (Mathf.Abs(slidingV) < num2) ? 0f : slidingV);
	}

	private void GetInput()
	{
		Vector3 vector = default(Vector3);
		vector.x = player.GetAxis("Move Horizontal");
		vector.y = player.GetAxis("Move Vertically");
		Vector2 vector2 = SmoothInput(vector.x, vector.y);
		input.x = vector2.x;
		input.y = vector2.y;
	}

	public void HonkIt()
	{
		if (canHonkIt && !GameMaster.instance.isZombieMode)
		{
			canHonkIt = false;
			Play2DClipAtPoint(Honk, 0.5f);
			StartCoroutine(ResetHonk());
		}
	}

	private IEnumerator ResetHonk()
	{
		yield return new WaitForSeconds(1f);
		canHonkIt = true;
	}

	private void CalculateDirection()
	{
		if ((!((double)input.x > 0.5) && !((double)input.x < 0.5) && !((double)input.y > 0.5) && !((double)input.y < 0.5)) || !cam)
		{
			return;
		}
		if (!OptionsMainMenu.instance.IsThirdPerson)
		{
			if (SolidDetection[0] && ((double)input.x < -0.5 || (double)input.x > 0.5) && (double)input.y > 0.5)
			{
				input.y = 0f;
			}
			else if (SolidDetection[1] && (double)input.x > 0.5 && ((double)input.y < -0.5 || (double)input.y > 0.5))
			{
				input.x = 0f;
			}
			else if (SolidDetection[2] && ((double)input.x < -0.5 || (double)input.x > 0.5) && (double)input.y < -0.5)
			{
				input.y = 0f;
			}
			else if (SolidDetection[3] && (double)input.x < -0.5 && ((double)input.y < -0.5 || (double)input.y > 0.5))
			{
				input.x = 0f;
			}
		}
		if (DrivingBackwards)
		{
			angle = Mathf.Atan2(0f - input.x, 0f - input.y);
		}
		else
		{
			angle = Mathf.Atan2(input.x, input.y);
		}
		angle = 57.29578f * angle;
		angle += cam.eulerAngles.y;
	}

	private void Rotate()
	{
		if (GameMaster.instance.GameHasPaused || !(Time.timeScale > 0f) || HTtanks.dying)
		{
			return;
		}
		if (OptionsMainMenu.instance.IsThirdPerson)
		{
			base.transform.rotation = TankHead.transform.rotation * Quaternion.Euler(0f, angle, 0f);
			return;
		}
		targetRotation = Quaternion.Euler(0f, angle, 0f);
		float num = Quaternion.Angle(base.transform.rotation, targetRotation);
		if (num > 1f && num < 120f)
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
		}
		else if (num >= 120f)
		{
			DrivingBackwards = ((!DrivingBackwards) ? true : false);
		}
	}

	private void Move()
	{
		if ((bool)GameMaster.instance.PKU && (bool)GameMaster.instance.PKU.BoostAmounts[playerId])
		{
			RectTransform rectTransform = GameMaster.instance.PKU.BoostAmounts[playerId].rectTransform;
			rectTransform.localScale = new Vector3(boosterFluid * 2f / maxFluid, rectTransform.localScale.y, rectTransform.localScale.z);
		}
		float num = TankSpeed;
		if (canBoost)
		{
			if (driving && player.GetButton("Boost"))
			{
				boosterFluid -= Time.deltaTime;
				num *= 1.5f;
				if (GameMaster.instance.CurrentMission == 1)
				{
					GameMaster.instance.mission2HasBoosted = true;
				}
				ActivateBooster();
			}
			else
			{
				DeactivateBooster();
			}
		}
		if (HTtanks.dying || (!((double)input.x > 0.5) && !((double)input.y > 0.5) && !((double)input.x < -0.5) && !((double)input.y < -0.5)))
		{
			return;
		}
		if (!OptionsMainMenu.instance.IsThirdPerson || OptionsMainMenu.instance.IsThirdPerson)
		{
			if (DrivingBackwards)
			{
				rigi.AddRelativeForce(-Vector3.forward * num * Time.deltaTime * 50f);
			}
			else
			{
				rigi.AddRelativeForce(Vector3.forward * num * Time.deltaTime * 50f);
			}
		}
		else if (DrivingBackwards)
		{
			rigi.AddRelativeForce(-TankHead.forward * num * Time.deltaTime * 50f);
		}
		else
		{
			rigi.AddRelativeForce(TankHead.forward * num * Time.deltaTime * 50f);
		}
	}

	private void ActivateBooster()
	{
		if (!BoostSource.isPlaying)
		{
			BoostSource.Play();
		}
		if (!DrivingBackwards)
		{
			ParticleSystem[] componentsInChildren = BackwardsRush.transform.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] componentsInChildren2 = Rush.transform.GetComponentsInChildren<ParticleSystem>();
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
				if (!particleSystem2.isPlaying)
				{
					particleSystem2.Play();
				}
			}
		}
		else
		{
			ParticleSystem[] componentsInChildren3 = BackwardsRush.transform.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] componentsInChildren4 = Rush.transform.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren3;
			foreach (ParticleSystem particleSystem3 in array)
			{
				if (!particleSystem3.isPlaying)
				{
					particleSystem3.Play();
				}
			}
			array = componentsInChildren4;
			foreach (ParticleSystem particleSystem4 in array)
			{
				if (particleSystem4.isPlaying)
				{
					particleSystem4.Stop();
				}
			}
		}
		_ = skidMarkCreator.main;
		if (!boosting)
		{
			if (GameMaster.instance.CurrentMission == 49)
			{
				SetDuration(originalDurationParticles / 1.5f, isMission49: true);
			}
			else
			{
				SetDuration(originalDurationParticles / 1.5f, isMission49: false);
			}
		}
		if (!boosting)
		{
			boosting = true;
		}
	}

	private void SetDuration(float dur, bool isMission49)
	{
		if (!isMission49)
		{
			if (skidMarkCreator.isEmitting)
			{
				skidMarkCreator.Stop();
			}
			if (!skidMarkCreator.isEmitting)
			{
				ParticleSystem.MainModule main = skidMarkCreator.main;
				main.duration = dur;
				skidMarkCreator.Play();
			}
		}
		else
		{
			skidMarkCreatorLvl50.Stop();
			ParticleSystem.MainModule main2 = skidMarkCreatorLvl50.main;
			main2.duration = dur;
			skidMarkCreatorLvl50.Play();
		}
	}

	private void DeactivateBooster()
	{
		if (BoostSource.isPlaying)
		{
			BoostSource.Stop();
		}
		Rush.GetComponent<AudioSource>().Stop();
		ParticleSystem[] componentsInChildren = BackwardsRush.transform.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] componentsInChildren2 = Rush.transform.GetComponentsInChildren<ParticleSystem>();
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
		if (boosting)
		{
			if (GameMaster.instance.CurrentMission == 49)
			{
				SetDuration(originalDurationParticles, isMission49: true);
			}
			else
			{
				SetDuration(originalDurationParticles, isMission49: false);
			}
		}
		if (boosting)
		{
			boosting = false;
		}
	}

	public void Play2DClipAtPoint(AudioClip clip, float Vol)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = Vol;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		UnityEngine.Object.Destroy(obj, clip.length);
	}
}
