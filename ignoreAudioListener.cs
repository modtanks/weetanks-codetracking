using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ignoreAudioListener : MonoBehaviour
{
	private void Awake()
	{
		AudioSource src = GetComponent<AudioSource>();
		src.ignoreListenerVolume = true;
	}
}
