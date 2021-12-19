using UnityEngine;

public class SetMainMenuVolume : MonoBehaviour
{
	public float SFXsoundsMainMenu = 0.3f;

	private void Update()
	{
		if (GameMaster.instance.GameHasPaused)
		{
			AudioListener.volume = SFXsoundsMainMenu * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f / 5f;
		}
		else
		{
			AudioListener.volume = SFXsoundsMainMenu * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
	}
}
