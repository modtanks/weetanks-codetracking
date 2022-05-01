using System.Collections;
using UnityEngine;

public class OnBeatLightFlash : MonoBehaviour
{
	public MeshRenderer[] MRS;

	public float speed = 1f;

	public float offset = 0f;

	[ColorUsage(true, true)]
	public Color LightUp = Color.white;

	[ColorUsage(true, true)]
	public Color OriginalLight = Color.white;

	private void Start()
	{
		StartCoroutine(OnBeat(FirstTime: true));
	}

	private IEnumerator OnBeat(bool FirstTime)
	{
		if (FirstTime)
		{
			yield return new WaitForSeconds(offset);
		}
		MeshRenderer[] mRS = MRS;
		foreach (MeshRenderer MR in mRS)
		{
			MR.material.SetColor("_EmissionColor", LightUp);
		}
		float t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime * speed;
			Vector4 CurrentLight = Vector4.Lerp(LightUp, OriginalLight, t2);
			MeshRenderer[] mRS2 = MRS;
			foreach (MeshRenderer MR2 in mRS2)
			{
				MR2.material.SetColor("_EmissionColor", CurrentLight);
			}
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime * speed;
			Vector4 CurrentLight2 = Vector4.Lerp(OriginalLight, LightUp, t2);
			MeshRenderer[] mRS3 = MRS;
			foreach (MeshRenderer MR3 in mRS3)
			{
				MR3.material.SetColor("_EmissionColor", CurrentLight2);
			}
			yield return null;
		}
		StartCoroutine(OnBeat(FirstTime: false));
	}
}
