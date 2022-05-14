using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ignoreAudioListener : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<AudioSource>().ignoreListenerVolume = true;
	}
}
