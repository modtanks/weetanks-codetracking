using UnityEngine;

public class Play2DClipOnce : MonoBehaviour
{
	public AudioClip sound;

	public bool overrideGameStarted = false;

	private void Start()
	{
		if (GameMaster.instance != null && ((GameMaster.instance.AmountGoodTanks > 0 && GameMaster.instance.GameHasStarted) || overrideGameStarted))
		{
			Play2DClipAtPoint(sound);
		}
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 3f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
