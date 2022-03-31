using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
	public float FlickerSpeed = 0.75f;

	public float FlickerRange = 0.25f;

	public MeshRenderer LightMaterialRenderer;

	public Light theLight;

	private float LightOriginalIntensity;

	public float EmissionIntesity;

	private Color StartEmission;

	private void Start()
	{
		LightOriginalIntensity = theLight.intensity;
		StartEmission = LightMaterialRenderer.material.GetColor("_EmissionColor");
		StartCoroutine(ChangeLight());
	}

	private IEnumerator ChangeLight()
	{
		float t = 0f;
		float FlickerChange = Random.Range(0f - FlickerRange, FlickerRange);
		float currentIntensity = theLight.intensity;
		float targetIntensity = LightOriginalIntensity + FlickerChange * 5f;
		float CurrentIntensity = LightMaterialRenderer.material.GetVector("_EmissionColor")[3];
		float NewEmissionIntensity = EmissionIntesity + FlickerChange;
		float Speed = FlickerSpeed + Random.Range(0f - FlickerSpeed / 2f, FlickerSpeed / 2f);
		while (t < 1f)
		{
			t += Time.deltaTime * Speed;
			theLight.intensity = Mathf.Lerp(currentIntensity, targetIntensity, t);
			float LerpedIntensity = Mathf.Lerp(CurrentIntensity, NewEmissionIntensity, t);
			LightMaterialRenderer.material.SetVector("_EmissionColor", StartEmission * LerpedIntensity);
			yield return null;
		}
		StartCoroutine(ChangeLight());
	}
}
