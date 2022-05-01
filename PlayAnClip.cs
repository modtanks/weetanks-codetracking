using UnityEngine;

public class PlayAnClip : MonoBehaviour
{
	public AudioClip[] Clip1;

	public float Volume;

	private void Start()
	{
	}

	private void OnClip1()
	{
		int choose = Random.Range(0, Clip1.Length);
		SFXManager.instance.PlaySFX(Clip1[choose], Volume);
	}
}
