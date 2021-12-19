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
		int num = Random.Range(0, Clip1.Length);
		Play2DClipAtPoint(Clip1[num], Volume);
	}

	public void Play2DClipAtPoint(AudioClip clip, float vol)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = vol;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
