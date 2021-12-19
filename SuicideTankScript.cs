using System.Collections;
using UnityEngine;

public class SuicideTankScript : MonoBehaviour
{
	public bool canSeePlayer;

	public Transform BaseRay;

	public NewAIagent aiAgent;

	public Vector3 targetPos;

	public GameObject bulletPrefab;

	public AudioClip shotSound;

	public int ShootInterval;

	public bool canShoot = true;

	private void Start()
	{
		if (canShoot)
		{
			StartCoroutine("ShootRandom");
		}
	}

	private void Update()
	{
		LayerMask layerMask = ~((1 << LayerMask.NameToLayer("OneWayBlock")) | (1 << LayerMask.NameToLayer("IgnorePathFinding")));
		if ((bool)aiAgent.TheTarget)
		{
			targetPos = aiAgent.TheTarget.transform.position;
			Vector3 forward = targetPos - base.transform.position;
			forward.y = 0f;
			Quaternion rotation = Quaternion.LookRotation(forward);
			base.transform.rotation = rotation;
		}
		Vector3 normalized = (targetPos - BaseRay.transform.position).normalized;
		Debug.DrawRay(BaseRay.transform.position, normalized * 20f, Color.blue);
		if (Physics.Raycast(BaseRay.transform.position, normalized * 20f, out var hitInfo, float.PositiveInfinity, layerMask))
		{
			if (hitInfo.collider.tag == "Player" || hitInfo.collider.tag == "Turret")
			{
				canSeePlayer = true;
			}
			else
			{
				canSeePlayer = false;
			}
		}
	}

	private IEnumerator ShootRandom()
	{
		float seconds = (float)ShootInterval + Random.Range(0f, ShootInterval);
		yield return new WaitForSeconds(seconds);
		if (canSeePlayer)
		{
			Play2DClipAtPoint(shotSound);
			GameObject obj = Object.Instantiate(bulletPrefab, BaseRay.position, BaseRay.transform.rotation);
			PlayerBulletScript component = obj.GetComponent<PlayerBulletScript>();
			if ((bool)component)
			{
				component.papaTank = aiAgent.gameObject;
			}
			obj.transform.Rotate(Vector3.right * 90f);
			obj.GetComponent<Rigidbody>().AddForce(BaseRay.forward * 6f);
		}
		StartCoroutine("ShootRandom");
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
