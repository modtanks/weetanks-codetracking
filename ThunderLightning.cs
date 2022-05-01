using System.Collections;
using UnityEngine;

public class ThunderLightning : MonoBehaviour
{
	private Light myLight;

	public float intensity;

	public bool isLightning = true;

	public CloudGeneration CloudGen;

	private void Start()
	{
		myLight = GetComponent<Light>();
		StartCoroutine(StrikeLightning());
	}

	private IEnumerator StrikeLightning()
	{
		float TimeBetweenStrikes = Random.Range(0.01f, 0.03f);
		float LightningIntensity = intensity + Random.Range(intensity / 2f, intensity / 2f);
		float WaitingTime = Random.Range(3f, 18f);
		yield return new WaitForSeconds(WaitingTime);
		int TimesLightningStrikes = Random.Range(1, 8);
		if (isLightning)
		{
			for (int i = 0; i < TimesLightningStrikes; i++)
			{
				myLight.intensity = LightningIntensity / 1f + (float)Mathf.Abs(Mathf.CeilToInt(TimesLightningStrikes / 2) - i);
				Debug.Log(myLight.intensity);
				yield return new WaitForSeconds(TimeBetweenStrikes);
				myLight.intensity = 0f;
				yield return new WaitForSeconds(TimeBetweenStrikes);
			}
			CameraShake CS = Camera.main.GetComponent<CameraShake>();
			if ((bool)CS)
			{
				CS.StartCoroutine(CS.Shake(0.12f, 0.17f));
			}
			CloudGen.StartCoroutine(CloudGen.PlayThunderSound());
		}
		yield return new WaitForSeconds(TimeBetweenStrikes);
		StartCoroutine(StrikeLightning());
	}
}
