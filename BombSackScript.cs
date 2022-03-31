using System.Collections;
using UnityEngine;

public class BombSackScript : MonoBehaviour
{
	public int BombState = 0;

	private Rigidbody rb;

	public Vector3 droppingPoint;

	public AudioClip LandSound;

	public AudioClip BeepSound;

	public AudioClip ExplosionSound;

	public AudioClip BombBackUp;

	public bool isLanded;

	public float TimeTillDetonation = 6f;

	public GameObject BulletToSpawn;

	public GameObject ParticleToSpawn;

	public bool isDying = false;

	public bool blinkScriptRunning = false;

	public GameObject ShootMeTutorial;

	public GameObject LandingParticles;

	private Animator BombAnimator;

	public ParticleSystem FlyingParticles;

	public GameObject LandingTarget;

	public MissionHundredController MHC;

	public Vector3 StartPos;

	private bool HasDoneDamage = false;

	private float previousYPos;

	private bool IsGrounded = false;

	private int GroundedCounter = 0;

	private GameObject SpawnedTarget;

	private bool LaunchedBack = false;

	public MeshRenderer BombBase;

	private bool IsWhiteNow = true;

	private void Start()
	{
		TimeTillDetonation = TimeTillDetonation + TimeTillDetonation + Random.Range(0f, TimeTillDetonation / 2f);
		rb = GetComponent<Rigidbody>();
		BombAnimator = GetComponent<Animator>();
		StartPos = base.transform.position;
		FlyingParticles.Stop();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("DestroyableWall"))
		{
			TimeTillDetonation = -100f;
			BossPlatform BP = collision.gameObject.GetComponent<BossPlatform>();
			if ((bool)BP && !HasDoneDamage)
			{
				BP.KTS.Throne.PlatformHealth--;
				HasDoneDamage = true;
			}
			SFXManager.instance.PlaySFX(ExplosionSound, 0.6f, null);
			Object.Instantiate(ParticleToSpawn, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
		else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
		{
			Explode();
			HealthTanks HT = collision.gameObject.GetComponent<HealthTanks>();
			if ((bool)HT)
			{
				HT.DamageMe(50);
			}
		}
	}

	public void FlyBack()
	{
		BombState = 2;
	}

	private void Update()
	{
		if (Mathf.Abs(previousYPos - base.transform.position.y) < 0.001f)
		{
			IsGrounded = true;
			GroundedCounter++;
			if (GroundedCounter >= 10 && base.transform.position.y > 1.5f && TimeTillDetonation > 1f)
			{
				TimeTillDetonation = 0.9f;
			}
		}
		else
		{
			IsGrounded = false;
			GroundedCounter = 0;
		}
		if (!BombAnimator.GetBool("MortarIsLive") && IsGrounded && base.transform.position.y < 3f)
		{
			Debug.Log("LANDED");
			BombAnimator.SetBool("MortarIsLive", value: true);
			GameObject LandParts = Object.Instantiate(LandingParticles, base.transform);
			Object.Destroy(LandParts, 3f);
			SFXManager.instance.PlaySFX(LandSound, 1f, null);
			CameraShake CS = Camera.main.GetComponent<CameraShake>();
			if ((bool)CS)
			{
				CS.StartCoroutine(CS.Shake(0.1f, 0.15f));
			}
		}
		previousYPos = base.transform.position.y;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		if (BombState == 0)
		{
			rb.isKinematic = true;
			base.transform.Translate(Vector3.up * Time.deltaTime * 32f, Space.World);
			if (base.transform.position.y > 40f)
			{
				Debug.Log("reached de sky");
				droppingPoint = GameMaster.instance.GetValidLocation(MHC.DropLocations);
				if (droppingPoint == Vector3.zero)
				{
					Object.Destroy(base.gameObject);
					return;
				}
				base.transform.position = droppingPoint + new Vector3(0f, 50f, 0f);
				BombState = 1;
				SpawnedTarget = Object.Instantiate(LandingTarget, new Vector3(droppingPoint.x, 0.1f, droppingPoint.z), Quaternion.identity);
			}
			if (!FlyingParticles.isPlaying)
			{
				FlyingParticles.Play();
			}
		}
		else if (BombState == 1)
		{
			rb.isKinematic = false;
			if (IsGrounded)
			{
				if ((bool)SpawnedTarget && base.transform.position.y < 4f)
				{
					Object.Destroy(SpawnedTarget);
				}
				if (FlyingParticles.isPlaying)
				{
					FlyingParticles.Stop();
				}
				TimeTillDetonation -= Time.deltaTime;
			}
			else if (!FlyingParticles.isPlaying)
			{
				FlyingParticles.Play();
			}
			if (TimeTillDetonation < 3f && !blinkScriptRunning)
			{
				blinkScriptRunning = true;
				StartCoroutine(BlinkWhite());
			}
			if (TimeTillDetonation < 0f && !isDying)
			{
				Explode();
			}
		}
		else if (BombState == 2)
		{
			if (!LaunchedBack)
			{
				SFXManager.instance.PlaySFX(BombBackUp);
				LaunchedBack = true;
			}
			rb.isKinematic = true;
			if (!FlyingParticles.isPlaying)
			{
				FlyingParticles.Play();
			}
			base.transform.Translate(Vector3.up * Time.deltaTime * 50f, Space.World);
			if (base.transform.position.y > 40f)
			{
				Debug.Log("reached de sky");
				base.transform.position = StartPos + new Vector3(0f, 40f, 0f);
				BombState = 3;
			}
		}
		else if (BombState == 3)
		{
			if (!FlyingParticles.isPlaying)
			{
				FlyingParticles.Play();
			}
			rb.isKinematic = false;
			base.transform.Translate(-Vector3.up * Time.deltaTime * 40f, Space.World);
		}
	}

	public void Explode()
	{
		isDying = true;
		SpawnBullets(0);
		SFXManager.instance.PlaySFX(ExplosionSound, 0.6f, null);
		Object.Instantiate(ParticleToSpawn, base.transform.position, Quaternion.identity);
	}

	private IEnumerator BlinkWhite()
	{
		yield return new WaitForSeconds(0.2f);
		if (IsWhiteNow)
		{
			SFXManager.instance.PlaySFX(BeepSound, 1f, null);
			IsWhiteNow = false;
			Material[] ms2 = BombBase.materials;
			ms2[0].SetColor("_EmissionColor", Color.red * 1.9f);
			BombBase.materials = ms2;
		}
		else
		{
			IsWhiteNow = true;
			Material[] ms = BombBase.materials;
			ms[0].SetColor("_EmissionColor", Color.white * 1.7f);
			BombBase.materials = ms;
		}
		StartCoroutine(BlinkWhite());
	}

	private void ShootBullet(Vector3 direction)
	{
		GameObject Bullet = Object.Instantiate(BulletToSpawn, direction, Quaternion.identity);
		PlayerBulletScript bullet = Bullet.GetComponent<PlayerBulletScript>();
		bullet.isTowerCharged = true;
		bullet.IsSackBullet = true;
		InstantiateOneParticle IOP = Bullet.GetComponent<InstantiateOneParticle>();
		if ((bool)IOP)
		{
			IOP.Sound = null;
		}
		bullet.MaxBounces = 0;
		Bullet.transform.Rotate(Vector3.right * 90f);
		Rigidbody bulletBody = Bullet.GetComponent<Rigidbody>();
		Vector3 Bdirection = direction - base.transform.position;
		bulletBody.AddForce(Bdirection * 6f);
		bullet.StartingVelocity = Bdirection * 6f;
	}

	private void SpawnBullets(int index)
	{
		if (index > 7)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (OptionsMainMenu.instance.currentDifficulty == 0 && (index == 1 || index == 4 || index == 6 || index == 7))
		{
			SpawnBullets(++index);
			return;
		}
		GameObject Bullet = null;
		Vector3 offset = Vector3.zero;
		switch (index)
		{
		case 0:
			offset = base.transform.position + new Vector3(1f, 0.5f, 0f);
			break;
		case 1:
			offset = base.transform.position + new Vector3(1f, 0.5f, 1f);
			break;
		case 2:
			offset = base.transform.position + new Vector3(0f, 0.5f, 1f);
			break;
		case 3:
			offset = base.transform.position + new Vector3(0f, 0.5f, -1f);
			break;
		case 4:
			offset = base.transform.position + new Vector3(-1f, 0.5f, -1f);
			break;
		case 5:
			offset = base.transform.position + new Vector3(-1f, 0.5f, 0f);
			break;
		case 6:
			offset = base.transform.position + new Vector3(-1f, 0.5f, 1f);
			break;
		case 7:
			offset = base.transform.position + new Vector3(1f, 0.5f, -1f);
			break;
		}
		ShootBullet(offset);
		SpawnBullets(++index);
	}

	public void HitByBullet(Vector3 direction, float force)
	{
	}
}
