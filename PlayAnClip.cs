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
		Play2DClipAtPoint(Clip1[choose], Volume);
	}

	public void Play2DClipAtPoint(AudioClip clip, float vol)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = vol;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
