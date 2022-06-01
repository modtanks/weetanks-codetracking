using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
	private static SFXManager _instance;

	public static SFXManager instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	public void PlaySFX(AudioClip sound)
	{
		if (!(sound == null))
		{
			GameObject obj = new GameObject("SFXTemporary");
			obj.transform.tag = "Temp";
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			audioSource.clip = sound;
			audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
			audioSource.spatialBlend = 0f;
			audioSource.Play();
			Object.Destroy(obj, sound.length);
		}
	}

	public void PlaySFX(AudioClip[] sound)
	{
		if (sound != null && sound.Length >= 1)
		{
			GameObject gameObject = new GameObject("SFXTemporary");
			gameObject.transform.tag = "Temp";
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			AudioClip audioClip = sound[Random.Range(0, sound.Length)];
			if ((bool)audioClip)
			{
				audioSource.clip = audioClip;
				audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
				audioSource.spatialBlend = 0f;
				audioSource.Play();
				Object.Destroy(gameObject, audioClip.length);
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}
	}

	public void PlaySFX(List<AudioClip> sound)
	{
		if (sound != null && sound.Count >= 1)
		{
			GameObject gameObject = new GameObject("SFXTemporary");
			gameObject.transform.tag = "Temp";
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			AudioClip audioClip = sound[Random.Range(0, sound.Count)];
			if ((bool)audioClip)
			{
				audioSource.clip = audioClip;
				audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
				audioSource.spatialBlend = 0f;
				audioSource.Play();
				Object.Destroy(gameObject, audioClip.length);
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}
	}

	public void PlaySFX(AudioClip sound, float volume)
	{
		if (!(sound == null))
		{
			GameObject obj = new GameObject("SFXTemporary");
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			audioSource.clip = sound;
			audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
			audioSource.spatialBlend = 0f;
			audioSource.Play();
			Object.Destroy(obj, sound.length);
		}
	}

	public void PlaySFX(AudioClip sound, float volume, AudioMixerGroup Mixer)
	{
		if (!(sound == null))
		{
			GameObject obj = new GameObject("SFXTemporary");
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			if (Mixer != null)
			{
				audioSource.outputAudioMixerGroup = Mixer;
			}
			audioSource.clip = sound;
			audioSource.volume = volume * ((float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl) / 10f;
			audioSource.spatialBlend = 0f;
			audioSource.Play();
			Object.Destroy(obj, sound.length);
		}
	}
}
