using UnityEngine;

public class Play2DClipOnce : MonoBehaviour
{
	public AudioClip sound;

	public bool overrideGameStarted;

	private void Start()
	{
		if (GameMaster.instance != null && ((GameMaster.instance.AmountGoodTanks > 0 && GameMaster.instance.GameHasStarted) || overrideGameStarted))
		{
			Play2DClipAtPoint(sound);
		}
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 3f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
