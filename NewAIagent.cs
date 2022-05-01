using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NewAIagent : MonoBehaviour
{
	[Header("AI Behaviour")]
	public float Accuracy = 10f;

	public bool CanMove = true;

	public float Lives = 1f;

	public bool LayMines = false;

	public float LayMinesSpeed = 0f;

	public float BulletSenseRange = 15f;

	public float ShootSpeed = 3f;

	public bool BouncyBullets = false;

	public int amountOfBounces = 2;

	[Header("Other Stuff")]
	public float TankSpeed = 5000f;

	public float turnSpeed = 10f;

	public bool IsTurning = false;

	private Vector2 input;

	public float angle;

	public float desiredangle;

	public float MoveTime;

	private Quaternion targetRotation;

	private Transform cam;

	public AudioClip TankTracks;

	public AudioClip TankTracksCarpet;

	public AudioClip SuicideBleep;

	public AudioSource source;

	private Rigidbody rigi;

	public ParticleSystem skidMark;

	public Transform skidMarkLocation;

	public bool Suicide = false;

	public float dist;

	[Header("Raycast Stuff")]
	public bool Targeted = false;

	private Transform IncomingBullet;

	private float incomingAngle;

	public bool bulletInPath = false;

	public bool antiBulletSpam = false;

	private NavMeshAgent agent;

	public GameObject myLight;

	public float SuicideTimer = 0f;

	public Material OnLight;

	public Material OffLight;

	public GameObject deathExplosion;

	public AudioClip Deathsound;

	public AudioClip Placesound;

	public int choose = -1;

	private HealthTanks myHealth;

	public Rigidbody rb;

	private Quaternion storedRot;

	public Vector3 lastPosition;

	public float speed;

	public float myExplosionDistance = 3f;

	public float myExplosionTimer = 1f;

	public Transform TheTarget;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
	}

	private void Start()
	{
		TankSpeed += Random.Range(0f, 1f);
		myExplosionDistance = 2f + Random.Range(0f, 3f);
		myExplosionTimer = myExplosionDistance / 2f - Random.Range(0f, 0.5f);
		rigi = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
		desiredangle = angle;
		InvokeRepeating("BlinkingLight", 0.3f, 0.3f);
		myHealth = GetComponent<HealthTanks>();
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (!GameMaster.instance.GameHasPaused)
		{
			speed = Mathf.Lerp(speed, (base.transform.position - lastPosition).magnitude / Time.deltaTime, 0.75f);
			lastPosition = base.transform.position;
			PlayerSense();
			RealSkidMarkCreating();
		}
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			agent.speed = 0f;
			return;
		}
		if (SuicideTimer > 0f)
		{
			if (SuicideTimer < 0.33f)
			{
				Boom();
				SuicideTimer = 2f;
			}
			SuicideTimer -= Time.deltaTime;
		}
		else if (!Suicide && !GameMaster.instance.GameHasPaused)
		{
			agent.speed = TankSpeed;
		}
		if (agent.velocity.sqrMagnitude > Mathf.Epsilon && !Suicide)
		{
			base.transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
		}
		else if (Suicide)
		{
			base.transform.rotation = storedRot;
		}
	}

	private void RealSkidMarkCreating()
	{
		if ((double)speed >= 0.5)
		{
			if (!source.isPlaying && GameMaster.instance.EnemyTankTracksAudio < GameMaster.instance.maxEnemyTankTracks && !GameMaster.instance.GameHasPaused)
			{
				PlayTracks();
			}
			else if (GameMaster.instance.GameHasPaused)
			{
				StopTracks();
			}
		}
		else if (source.isPlaying)
		{
			StopTracks();
		}
	}

	public void StopTracks()
	{
		GameMaster.instance.EnemyTankTracksAudio--;
		source.volume = 0.5f;
		source.clip = null;
		source.loop = false;
		source.Stop();
		skidMark.Pause();
	}

	private void PlayTracks()
	{
		GameMaster.instance.EnemyTankTracksAudio++;
		source.volume = 0.5f;
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
		source.loop = true;
		source.Play();
		skidMark.Play();
	}

	private void PlayerSense()
	{
		GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
		GameObject[] all = player.Concat(turrets).ToArray();
		Transform closest = null;
		float lastdist = 9999f;
		GameObject[] array = all;
		foreach (GameObject target in array)
		{
			float dist = Vector3.Distance(base.transform.position, target.transform.position);
			if (dist < lastdist)
			{
				closest = target.transform;
				lastdist = dist;
			}
		}
		if (closest != null)
		{
			agent.SetDestination(closest.position);
			TheTarget = closest;
			this.dist = Vector3.Distance(base.transform.position, closest.transform.position);
			if (this.dist < myExplosionDistance && !Suicide)
			{
				Quaternion nowRot = base.transform.rotation;
				agent.isStopped = true;
				agent.speed = 0f;
				Suicide = true;
				SuicideTimer = myExplosionTimer;
				base.transform.rotation = nowRot;
				storedRot = base.transform.rotation;
			}
		}
		else
		{
			myHealth.health = 0;
			agent.isStopped = true;
		}
	}

	private void BlinkingLight()
	{
		if (Suicide)
		{
			Renderer m_Material = myLight.GetComponent<Renderer>();
			if (m_Material.sharedMaterial == OnLight)
			{
				Play2DClipAtPoint(SuicideBleep);
				m_Material.material = OffLight;
				Debug.LogWarning("Off");
			}
			else if (m_Material.sharedMaterial == OffLight)
			{
				Play2DClipAtPoint(SuicideBleep);
				m_Material.material = OnLight;
				Debug.LogWarning("On");
			}
			else
			{
				Play2DClipAtPoint(SuicideBleep);
				m_Material.material = OffLight;
				Debug.LogWarning("Off 2");
			}
		}
	}

	public void Boom()
	{
		if (myHealth.EnemyID == -110)
		{
			AreaDamageEnemies(base.transform.position, 6f, 1f);
		}
		else
		{
			AreaDamageEnemies(base.transform.position, 3.8f, 1f);
		}
		Play2DClipAtPoint(Deathsound);
		myHealth.health = -9;
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
			WeeTurret WT = col.GetComponent<WeeTurret>();
			DestroyableWall DestroyWall = col.GetComponent<DestroyableWall>();
			if (enemy != null && enemy.transform.tag == "Player")
			{
				enemy.DamageMe(1);
				if (myHealth.EnemyID <= -100)
				{
					enemy.DamageMe(1);
				}
			}
			if (friendbullet != null)
			{
				friendbullet.BounceAmount = 999;
			}
			if (enemybullet != null)
			{
				enemybullet.BounceAmount = 999;
			}
			if (mines != null)
			{
				mines.DetinationTime = 0f;
			}
			if (DestroyWall != null)
			{
				DestroyWall.StartCoroutine(DestroyWall.destroy());
			}
			if (WT != null)
			{
				WT.Health--;
			}
		}
		Collider[] bigobjectsInRange = Physics.OverlapSphere(location, radius * 2f);
		Collider[] array2 = bigobjectsInRange;
		foreach (Collider col2 in array2)
		{
			Rigidbody rigi = col2.GetComponent<Rigidbody>();
			if (rigi != null && (col2.tag == "Player" || col2.tag == "Enemy"))
			{
				float distance = Vector3.Distance(rigi.transform.position, base.transform.position);
				float force = (radius * 2f - distance) * 1.5f;
				Vector3 direction = rigi.transform.position - base.transform.position;
				rigi.AddForce(direction * force, ForceMode.Impulse);
			}
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
