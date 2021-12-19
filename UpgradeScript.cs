using System.Collections;
using UnityEngine;

public class UpgradeScript : MonoBehaviour
{
	public AudioClip PickupSound;

	public bool isSpeed;

	public bool isShield;

	public float Time;

	private void Start()
	{
		Time = Random.Range(10f, 20f);
		StartCoroutine(DestroyMe());
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.transform.tag == "Player" && isSpeed)
		{
			MoveTankScript component = collision.transform.GetComponent<MoveTankScript>();
			if (component != null)
			{
				Debug.LogWarning("speed boost got picked up by: " + collision.name);
				component.StartCoroutine("UpgradeSpeed");
				Play2DClipAtPoint(PickupSound);
				Object.Destroy(base.gameObject);
			}
		}
		if (!(collision.transform.tag == "Player") || !isShield)
		{
			return;
		}
		HealthTanks component2 = collision.transform.GetComponent<HealthTanks>();
		if (component2 != null)
		{
			if (component2.health < 2)
			{
				component2.health++;
			}
			Play2DClipAtPoint(PickupSound);
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator DestroyMe()
	{
		yield return new WaitForSeconds(Time);
		Object.Destroy(base.gameObject);
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 0.5f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
