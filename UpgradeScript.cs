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
			MoveTankScript moveScript = collision.transform.GetComponent<MoveTankScript>();
			if (moveScript != null)
			{
				Debug.LogWarning("speed boost got picked up by: " + collision.name);
				moveScript.StartCoroutine("UpgradeSpeed");
				Play2DClipAtPoint(PickupSound);
				Object.Destroy(base.gameObject);
			}
		}
		if (!(collision.transform.tag == "Player") || !isShield)
		{
			return;
		}
		HealthTanks healthScript = collision.transform.GetComponent<HealthTanks>();
		if (healthScript != null)
		{
			if (healthScript.health < 2)
			{
				healthScript.health++;
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
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 0.5f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
