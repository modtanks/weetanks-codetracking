using System.Collections;
using UnityEngine;

public class SuicideTankScript : MonoBehaviour
{
	public bool canSeePlayer = false;

	public Transform BaseRay;

	public NewAIagent aiAgent;

	public Vector3 targetPos;

	public GameObject bulletPrefab;

	public AudioClip shotSound;

	public int ShootInterval;

	public bool canShoot = true;

	public bool canShootAirRockets = false;

	public RocketScript RS;

	public GameObject RocketPrefab;

	public Transform RocketSpawnLocation;

	private void Start()
	{
		if (canShoot)
		{
			StartCoroutine("ShootRandom");
		}
		if (canShootAirRockets)
		{
			StartCoroutine("ShootAirRandom");
		}
	}

	private void Update()
	{
		LayerMask layerMask = ~((1 << LayerMask.NameToLayer("Other")) | (1 << LayerMask.NameToLayer("OneWayBlock")) | (1 << LayerMask.NameToLayer("IgnorePathFinding")));
		if ((bool)aiAgent.TheTarget)
		{
			targetPos = aiAgent.TheTarget.transform.position;
			Vector3 lookPos = targetPos - base.transform.position;
			lookPos.y = 0f;
			Quaternion rotation = Quaternion.LookRotation(lookPos);
			base.transform.rotation = rotation;
		}
		Vector3 direction = (targetPos - BaseRay.transform.position).normalized;
		Debug.DrawRay(BaseRay.transform.position, direction * 20f, Color.blue);
		if (Physics.Raycast(BaseRay.transform.position, direction * 20f, out var rayhit, float.PositiveInfinity, layerMask))
		{
			if (rayhit.collider.tag == "Player" || rayhit.collider.tag == "Turret")
			{
				canSeePlayer = true;
			}
			else
			{
				canSeePlayer = false;
			}
		}
	}

	private IEnumerator ShootAirRandom()
	{
		float random = (float)ShootInterval + Random.Range(0f, ShootInterval);
		yield return new WaitForSeconds(random);
		RS.Launch();
		yield return new WaitForSeconds(ShootInterval);
		GameObject NewRocket = Object.Instantiate(RocketPrefab, RocketSpawnLocation);
		RS = NewRocket.GetComponent<RocketScript>();
		StartCoroutine("ShootAirRandom");
	}

	private IEnumerator ShootRandom()
	{
		float random = (float)ShootInterval + Random.Range(0f, ShootInterval);
		yield return new WaitForSeconds(random);
		if (canSeePlayer)
		{
			Play2DClipAtPoint(shotSound);
			GameObject bulletGO = Object.Instantiate(bulletPrefab, BaseRay.position, BaseRay.transform.rotation);
			PlayerBulletScript bullet = bulletGO.GetComponent<PlayerBulletScript>();
			if ((bool)bullet)
			{
				bullet.papaTank = aiAgent.gameObject;
			}
			bulletGO.transform.Rotate(Vector3.right * 90f);
			Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
			bulletBody.AddForce(BaseRay.forward * 6f);
			bullet.StartingVelocity = BaseRay.forward * 6f;
		}
		StartCoroutine("ShootRandom");
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
