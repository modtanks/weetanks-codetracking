using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NewAIagent : MonoBehaviour
{
	[Header("AI Behaviour")]
	public float Accuracy = 10f;

	public bool CanMove = true;

	public float Lives = 1f;

	public bool LayMines;

	public float LayMinesSpeed;

	public float BulletSenseRange = 15f;

	public float ShootSpeed = 3f;

	public bool BouncyBullets;

	public int amountOfBounces = 2;

	[Header("Other Stuff")]
	public float TankSpeed = 5000f;

	public float turnSpeed = 10f;

	public bool IsTurning;

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

	public bool Suicide;

	public float dist;

	[Header("Raycast Stuff")]
	public bool Targeted;

	private Transform IncomingBullet;

	private float incomingAngle;

	public bool bulletInPath;

	public bool antiBulletSpam;

	private NavMeshAgent agent;

	public GameObject myLight;

	public float SuicideTimer;

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
		GameObject[] first = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] second = GameObject.FindGameObjectsWithTag("Turret");
		GameObject[] array = first.Concat(second).ToArray();
		Transform transform = null;
		float num = 9999f;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
			if (num2 < num)
			{
				transform = gameObject.transform;
				num = num2;
			}
		}
		if (transform != null)
		{
			agent.SetDestination(transform.position);
			TheTarget = transform;
		}
		else
		{
			myHealth.health = 0;
			agent.isStopped = true;
		}
		dist = Vector3.Distance(base.transform.position, transform.transform.position);
		if (dist < myExplosionDistance && !Suicide)
		{
			Quaternion rotation = base.transform.rotation;
			agent.isStopped = true;
			agent.speed = 0f;
			Suicide = true;
			SuicideTimer = myExplosionTimer;
			base.transform.rotation = rotation;
			storedRot = base.transform.rotation;
		}
	}

	private void BlinkingLight()
	{
		if (Suicide)
		{
			Renderer component = myLight.GetComponent<Renderer>();
			if (component.sharedMaterial == OnLight)
			{
				Play2DClipAtPoint(SuicideBleep);
				component.material = OffLight;
				Debug.LogWarning("Off");
			}
			else if (component.sharedMaterial == OffLight)
			{
				Play2DClipAtPoint(SuicideBleep);
				component.material = OnLight;
				Debug.LogWarning("On");
			}
			else
			{
				Play2DClipAtPoint(SuicideBleep);
				component.material = OffLight;
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
		Collider[] array = Physics.OverlapSphere(location, radius);
		foreach (Collider obj in array)
		{
			HealthTanks component = obj.GetComponent<HealthTanks>();
			LookAtMyDirection component2 = obj.GetComponent<LookAtMyDirection>();
			EnemyBulletScript component3 = obj.GetComponent<EnemyBulletScript>();
			MineScript component4 = obj.GetComponent<MineScript>();
			WeeTurret component5 = obj.GetComponent<WeeTurret>();
			DestroyableWall component6 = obj.GetComponent<DestroyableWall>();
			if (component != null && component.transform.tag == "Player")
			{
				component.health--;
			}
			if (component2 != null)
			{
				component2.BounceAmount = 999;
			}
			if (component3 != null)
			{
				component3.BounceAmount = 999;
			}
			if (component4 != null)
			{
				component4.DetinationTime = 0f;
			}
			if (component6 != null)
			{
				component6.StartCoroutine(component6.destroy());
			}
			if (component5 != null)
			{
				component5.Health--;
			}
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
