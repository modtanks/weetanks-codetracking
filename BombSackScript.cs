using System.Collections;
using UnityEngine;

public class BombSackScript : MonoBehaviour
{
	public int BombState;

	private Rigidbody rb;

	public Vector3 droppingPoint;

	public AudioClip LandSound;

	public AudioClip ExplosionSound;

	public bool isLanded;

	public float TimeTillDetonation = 6f;

	public GameObject BulletToSpawn;

	public GameObject ParticleToSpawn;

	public bool isDying;

	public bool blinkScriptRunning;

	public GameObject ShootMeTutorial;

	private bool HasDoneDamage;

	private float previousYPos;

	private bool isGrounded;

	private int GroundedCounter;

	private void Start()
	{
		TimeTillDetonation = TimeTillDetonation + TimeTillDetonation + Random.Range(0f, TimeTillDetonation / 2f);
		rb = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("DestroyableWall"))
		{
			TimeTillDetonation = -100f;
			BossPlatform component = collision.gameObject.GetComponent<BossPlatform>();
			if ((bool)component && !HasDoneDamage)
			{
				if (component.PlatformLayer == 2 && component.KTS.MHC.BossPhase < 3)
				{
					component.KTS.Platform_2.PlatformHealth--;
				}
				else if (component.PlatformLayer == 2 && component.KTS.MHC.BossPhase >= 3)
				{
					component.KTS.Platform_1.PlatformHealth--;
				}
				else if (component.PlatformLayer == 1 && component.KTS.MHC.BossPhase < 3)
				{
					component.KTS.Platform_2.PlatformHealth--;
				}
				else if (component.PlatformLayer == 1 && component.KTS.MHC.BossPhase >= 3)
				{
					component.KTS.Platform_1.PlatformHealth--;
				}
				HasDoneDamage = true;
			}
			GameMaster.instance.Play2DClipAtPoint(ExplosionSound, 0.6f);
			Object.Instantiate(ParticleToSpawn, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
		else if (collision.gameObject.tag == "Player")
		{
			Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z) - collision.gameObject.transform.position;
			rb.AddForce(vector * 6f, ForceMode.Impulse);
			ShootMeTutorial.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (Mathf.Abs(previousYPos - base.transform.position.y) < 0.001f)
		{
			isGrounded = true;
			GroundedCounter++;
			if (GroundedCounter >= 10 && base.transform.position.y > 1.5f && TimeTillDetonation > 1f)
			{
				TimeTillDetonation = 0.9f;
			}
		}
		else
		{
			isGrounded = false;
			GroundedCounter = 0;
		}
		previousYPos = base.transform.position.y;
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		if (BombState == 0)
		{
			rb.isKinematic = true;
			base.transform.Translate(Vector3.up * Time.deltaTime * 32f, Space.World);
			if (base.transform.position.y > 70f)
			{
				Debug.Log("reached de sky");
				droppingPoint = GameMaster.instance.GetValidLocation(CheckForDist: false, 99999f, Vector3.zero);
				base.transform.position = droppingPoint + new Vector3(0f, 60f, 0f);
				BombState = 1;
			}
		}
		else if (BombState == 1)
		{
			rb.isKinematic = false;
			if (isGrounded)
			{
				TimeTillDetonation -= Time.deltaTime;
			}
			if (TimeTillDetonation < 3f && !blinkScriptRunning)
			{
				blinkScriptRunning = true;
				StartCoroutine(BlinkWhite());
			}
			if (TimeTillDetonation < 0f && !isDying)
			{
				isDying = true;
				SpawnBullets(0);
				GameMaster.instance.Play2DClipAtPoint(ExplosionSound, 0.6f);
				Object.Instantiate(ParticleToSpawn, base.transform.position, Quaternion.identity);
			}
		}
	}

	private IEnumerator BlinkWhite()
	{
		yield return new WaitForSeconds(0.5f);
		MakeWhite component = GetComponent<MakeWhite>();
		if ((bool)component)
		{
			component.GotHit();
		}
		StartCoroutine(BlinkWhite());
	}

	private void ShootBullet(Vector3 direction)
	{
		GameObject obj = Object.Instantiate(BulletToSpawn, direction, Quaternion.identity);
		PlayerBulletScript component = obj.GetComponent<PlayerBulletScript>();
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
		rb.AddForce(direction * force, ForceMode.Impulse);
		ShootMeTutorial.SetActive(value: false);
	}
}
