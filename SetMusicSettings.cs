using UnityEngine;

public class SetMusicSettings : MonoBehaviour
{
	public float OGvolume;

	public bool disableThis = false;

	private void Start()
	{
		if (GetComponent<AudioSource>() != null)
		{
			OGvolume = GetComponent<AudioSource>().volume;
		}
	}

	private void FixedUpdate()
	{
		if (GetComponent<AudioSource>() != null && OptionsMainMenu.instance != null && !disableThis)
		{
			GetComponent<AudioSource>().volume = OGvolume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
	}
}
