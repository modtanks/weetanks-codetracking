using System.Collections;
using UnityEngine;

public class EnemyAINavMeshTargeting : MonoBehaviour
{
	public NewAIagent AIscript;

	public GameObject Player;

	public GameObject bulletPrefab;

	public Transform firePoint;

	public AudioClip shotSound;

	private AudioSource source;

	public Vector3 target;

	public Quaternion rot;

	public Vector3 startingPosition;

	[HideInInspector]
	public int maxReflectionCount = 2;

	public float maxStepDistance = 200f;

	[SerializeField]
	private Vector3 initialVelocity;

	[SerializeField]
	private Vector3 lastFrameVelocity;

	public bool specialMove = false;

	private void Start()
	{
		rot = Quaternion.Euler(0f, 0f, 0f);
		Player = GameObject.FindGameObjectWithTag("Player");
		source = GetComponent<AudioSource>();
		StartCoroutine("ShootAtPlayer");
		if (AIscript.BouncyBullets)
		{
			InvokeRepeating("CheckForPlayer", 0.05f, 0.05f);
			maxReflectionCount = AIscript.amountOfBounces;
		}
	}

	private void Update()
	{
		if (!specialMove)
		{
			initialVelocity = firePoint.transform.forward * 100f;
			target = new Vector3(Player.transform.position.x, firePoint.transform.position.y, Player.transform.position.z);
			Vector3 lookPos2 = Player.transform.position - base.transform.position;
			lookPos2.y = 0f;
			Quaternion rotation2 = Quaternion.LookRotation(lookPos2);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation2, Time.deltaTime * 18.5f);
		}
		else
		{
			Vector3 lookPos = startingPosition - base.transform.position;
			lookPos.y = 0f;
			Quaternion rotation = Quaternion.LookRotation(lookPos);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, rotation, Time.deltaTime * 18.5f);
		}
	}

	public void CheckForPlayer()
	{
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var rayhit) && rayhit.transform.tag != "Player" && !specialMove)
		{
			if (!DrawReflectionPattern(base.transform.position, rot * base.transform.forward, maxReflectionCount))
			{
				rot *= Quaternion.Euler(0f, 6f, 0f);
			}
			else
			{
				specialMove = true;
			}
		}
	}

	private bool DrawReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
	{
		do
		{
			Vector3 startPosition = position;
			Ray ray = new Ray(position, direction);
			if (Physics.Raycast(ray, out var hit, maxStepDistance))
			{
				if (reflectionsRemaining == maxReflectionCount)
				{
					startingPosition = hit.point;
				}
				direction = Vector3.Reflect(direction, hit.normal);
				position = hit.point;
				if (hit.collider.tag == "Player")
				{
					return true;
				}
			}
			else
			{
				position += direction * maxStepDistance;
			}
			Debug.DrawLine(startPosition, position, Color.red);
			reflectionsRemaining--;
		}
		while (reflectionsRemaining > 0);
		return false;
	}

	private IEnumerator ShootAtPlayer()
	{
		float ShootInterval = AIscript.ShootSpeed + Random.Range(0f, 1f);
		yield return new WaitForSeconds(ShootInterval);
		RaycastHit rayhit;
		if (specialMove)
		{
			GameObject bulletGO2 = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.transform.rotation);
			EnemyBulletScript bullet2 = bulletGO2.GetComponent<EnemyBulletScript>();
			if (bullet2 != null)
			{
				bullet2.dir = target;
			}
			bulletGO2.transform.Rotate(Vector3.right * 90f);
			Rigidbody bulletBody2 = bulletGO2.GetComponent<Rigidbody>();
			bulletBody2.AddForce(firePoint.forward * 6f);
			specialMove = false;
		}
		else if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out rayhit) && rayhit.transform.tag == "Player")
		{
			source.PlayOneShot(shotSound);
			GameObject bulletGO = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.transform.rotation);
			EnemyBulletScript bullet = bulletGO.GetComponent<EnemyBulletScript>();
			if (bullet != null)
			{
				bullet.dir = target;
			}
			bulletGO.transform.Rotate(Vector3.right * 90f);
			Rigidbody bulletBody = bulletGO.GetComponent<Rigidbody>();
			bulletBody.AddForce(firePoint.forward * 6f);
		}
		StartCoroutine("ShootAtPlayer");
	}
}
