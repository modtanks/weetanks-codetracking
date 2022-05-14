using System.Collections;
using UnityEngine;

public class OnBeatLightFlash : MonoBehaviour
{
	public MeshRenderer[] MRS;

	public float speed = 1f;

	public float offset;

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
		for (int i = 0; i < mRS.Length; i++)
		{
			mRS[i].material.SetColor("_EmissionColor", LightUp);
		}
		float t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime * speed;
			Vector4 vector = Vector4.Lerp(LightUp, OriginalLight, t2);
			mRS = MRS;
			for (int i = 0; i < mRS.Length; i++)
			{
				mRS[i].material.SetColor("_EmissionColor", vector);
			}
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		t2 = 0f;
		while (t2 < 1f)
		{
			t2 += Time.deltaTime * speed;
			Vector4 vector2 = Vector4.Lerp(OriginalLight, LightUp, t2);
			mRS = MRS;
			for (int i = 0; i < mRS.Length; i++)
			{
				mRS[i].material.SetColor("_EmissionColor", vector2);
			}
			yield return null;
		}
		StartCoroutine(OnBeat(FirstTime: false));
	}
}
