using UnityEngine;

public class SetVolume : MonoBehaviour
{
	private void Start()
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		if ((bool)audioSource)
		{
			float OriginalVolume = audioSource.volume;
			audioSource.volume = OriginalVolume * ((float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl) / 10f;
		}
	}
}
