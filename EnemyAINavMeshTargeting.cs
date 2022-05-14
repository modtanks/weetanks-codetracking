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

	public bool specialMove;

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
			Vector3 forward = Player.transform.position - base.transform.position;
			forward.y = 0f;
			Quaternion b = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * 18.5f);
		}
		else
		{
			Vector3 forward2 = startingPosition - base.transform.position;
			forward2.y = 0f;
			Quaternion b2 = Quaternion.LookRotation(forward2);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b2, Time.deltaTime * 18.5f);
		}
	}

	public void CheckForPlayer()
	{
		if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out var hitInfo) && hitInfo.transform.tag != "Player" && !specialMove)
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
			Vector3 start = position;
			if (Physics.Raycast(new Ray(position, direction), out var hitInfo, maxStepDistance))
			{
				if (reflectionsRemaining == maxReflectionCount)
				{
					startingPosition = hitInfo.point;
				}
				direction = Vector3.Reflect(direction, hitInfo.normal);
				position = hitInfo.point;
				if (hitInfo.collider.tag == "Player")
				{
					return true;
				}
			}
			else
			{
				position += direction * maxStepDistance;
			}
			Debug.DrawLine(start, position, Color.red);
			reflectionsRemaining--;
		}
		while (reflectionsRemaining > 0);
		return false;
	}

	private IEnumerator ShootAtPlayer()
	{
		float seconds = AIscript.ShootSpeed + Random.Range(0f, 1f);
		yield return new WaitForSeconds(seconds);
		RaycastHit hitInfo;
		if (specialMove)
		{
			GameObject obj = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.transform.rotation);
			EnemyBulletScript component = obj.GetComponent<EnemyBulletScript>();
			if (component != null)
			{
				component.dir = target;
			}
			obj.transform.Rotate(Vector3.right * 90f);
			obj.GetComponent<Rigidbody>().AddForce(firePoint.forward * 6f);
			specialMove = false;
		}
		else if (Physics.Raycast(base.transform.position, base.transform.TransformDirection(Vector3.forward) * 100f, out hitInfo) && hitInfo.transform.tag == "Player")
		{
			source.PlayOneShot(shotSound);
			GameObject obj2 = Object.Instantiate(bulletPrefab, firePoint.position, firePoint.transform.rotation);
			EnemyBulletScript component2 = obj2.GetComponent<EnemyBulletScript>();
			if (component2 != null)
			{
				component2.dir = target;
			}
			obj2.transform.Rotate(Vector3.right * 90f);
			obj2.GetComponent<Rigidbody>().AddForce(firePoint.forward * 6f);
		}
		StartCoroutine("ShootAtPlayer");
	}
}
