using UnityEngine;

public class SetVolume : MonoBehaviour
{
	private void Start()
	{
		AudioSource component = GetComponent<AudioSource>();
		if ((bool)component)
		{
			float volume = component.volume;
			component.volume = volume * ((float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl) / 10f;
		}
	}
}
