using System.Collections;
using UnityEngine;

public class PlayAnClip : MonoBehaviour
{
	public AudioClip[] Clip1;

	public float Volume;

	public bool AutoDestroy;

	public float DestroyTime;

	public bool PlayClipOnStart;

	private void Start()
	{
		if (PlayClipOnStart)
		{
			OnClip1();
		}
	}

	private void OnClip1()
	{
		int num = Random.Range(0, Clip1.Length);
		SFXManager.instance.PlaySFX(Clip1[num], Volume);
		if (AutoDestroy)
		{
			StartCoroutine(DestroyMe());
		}
	}

	private IEnumerator DestroyMe()
	{
		yield return new WaitForSeconds(DestroyTime);
		Object.Destroy(base.gameObject);
	}
}
