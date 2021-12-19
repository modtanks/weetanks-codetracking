using System.Collections;
using UnityEngine;

public class blink : MonoBehaviour
{
	public Material normal;

	public Material blinked;

	public float blinkSpeed;

	private MeshRenderer MR;

	private void Start()
	{
		MR = GetComponent<MeshRenderer>();
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		MR.material = blinked;
		yield return new WaitForSeconds(blinkSpeed);
		MR.material = normal;
		yield return new WaitForSeconds(blinkSpeed * 2f);
		StartCoroutine(Blink());
	}
}
