using System.Collections;
using UnityEngine;

public class BombSackScript : MonoBehaviour
{
	public int BombState;

	private Rigidbody rb;

	public Vector3 droppingPoint;

	public AudioClip LandSound;

	public AudioClip BeepSound;

	public AudioClip ExplosionSound;

	public bool isLanded;

	public float TimeTillDetonation = 6f;

	public GameObject BulletToSpawn;

	public GameObject ParticleToSpawn;

	public bool isDying;

	public bool blinkScriptRunning;

	public GameObject ShootMeTutorial;

	public GameObject LandingParticles;

	private Animator BombAnimator;

	public ParticleSystem FlyingParticles;

	public GameObject LandingTarget;

	public MissionHundredController MHC;

	public Vector3 StartPos;

	private bool HasDoneDamage;

	private float previousYPos;

	private bool IsGrounded;

	private int GroundedCounter;

	private GameObject SpawnedTarget;

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
			BossPlatform component = collision.gameObject.GetComponent<BossPlatform>();
			if ((bool)component && !HasDoneDamage)
			{
				component.KTS.Throne.PlatformHealth--;
				HasDoneDamage = true;
			}
			SFXManager.instance.PlaySFX(ExplosionSound, 0.6f, null);
			Object.Instantiate(ParticleToSpawn, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
		else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
		{
			Explode();
			HealthTanks component2 = collision.gameObject.GetComponent<HealthTanks>();
			if ((bool)component2)
			{
				component2.DamageMe(50);
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
			Object.Destroy(Object.Instantiate(LandingParticles, base.transform), 3f);
			SFXManager.instance.PlaySFX(LandSound, 1f, null);
			CameraShake component = Camera.main.GetComponent<CameraShake>();
			if ((bool)component)
			{
				component.StartCoroutine(component.Shake(0.1f, 0.15f));
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
			Material[] materials = BombBase.materials;
			materials[0].SetColor("_EmissionColor", Color.red * 1.9f);
			BombBase.materials = materials;
		}
		else
		{
			IsWhiteNow = true;
			Material[] materials2 = BombBase.materials;
			materials2[0].SetColor("_EmissionColor", Color.white * 1.7f);
			BombBase.materials = materials2;
		}
		StartCoroutine(BlinkWhite());
	}

	private void ShootBullet(Vector3 direction)
	{
		GameObject obj = Object.Instantiate(BulletToSpawn, direction, Quaternion.identity);
		PlayerBulletScript component = obj.GetComponent<PlayerBulletScript>();
		component.isTowerCharged = true;
		component.IsSackBullet = true;
		InstantiateOneParticle component2 = obj.GetComponent<InstantiateOneParticle>();
		if ((bool)component2)
		{
			component2.Sound = null;
		}
		component.MaxBounces = 0;
		obj.transform.Rotate(Vector3.right * 90f);
		Rigidbody component3 = obj.GetComponent<Rigidbody>();
		Vector3 vector = direction - base.transform.position;
		component3.AddForce(vector * 6f);
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
		Vector3 direction = Vector3.zero;
		switch (index)
		{
		case 0:
			direction = base.transform.position + new Vector3(1f, 0f, 0f);
			break;
		case 1:
			direction = base.transform.position + new Vector3(1f, 0f, 1f);
			break;
		case 2:
			direction = base.transform.position + new Vector3(0f, 0f, 1f);
			break;
		case 3:
			direction = base.transform.position + new Vector3(0f, 0f, -1f);
			break;
		case 4:
			direction = base.transform.position + new Vector3(-1f, 0f, -1f);
			break;
		case 5:
			direction = base.transform.position + new Vector3(-1f, 0f, 0f);
			break;
		case 6:
			direction = base.transform.position + new Vector3(-1f, 0f, 1f);
			break;
		case 7:
			direction = base.transform.position + new Vector3(1f, 0f, -1f);
			break;
		}
		ShootBullet(direction);
		SpawnBullets(++index);
	}

	public void HitByBullet(Vector3 direction, float force)
	{
	}
}
