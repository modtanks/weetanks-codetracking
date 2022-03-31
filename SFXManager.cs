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
		GameObject gameObject = new GameObject("SFXTemporary");
		gameObject.transform.tag = "Temp";
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = sound;
		audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(gameObject, sound.length);
	}

	public void PlaySFX(AudioClip[] sound)
	{
		_ = sound.Length;
		if (0 == 0 && sound.Length >= 1)
		{
			GameObject gameObject = new GameObject("SFXTemporary");
			gameObject.transform.tag = "Temp";
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = sound[Random.Range(0, sound.Length)];
			audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
			audioSource.spatialBlend = 0f;
			audioSource.Play();
			Object.Destroy(gameObject, audioSource.clip.length);
		}
	}

	public void PlaySFX(List<AudioClip> sound)
	{
		_ = sound.Count;
		if (0 == 0 && sound.Count >= 1)
		{
			GameObject gameObject = new GameObject("SFXTemporary");
			gameObject.transform.tag = "Temp";
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = sound[Random.Range(0, sound.Count)];
			audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
			audioSource.spatialBlend = 0f;
			audioSource.Play();
			Object.Destroy(gameObject, audioSource.clip.length);
		}
	}

	public void PlaySFX(AudioClip sound, float volume)
	{
		GameObject gameObject = new GameObject("SFXTemporary");
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = sound;
		audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(gameObject, sound.length);
	}

	public void PlaySFX(AudioClip sound, float volume, AudioMixerGroup Mixer)
	{
		GameObject gameObject = new GameObject("SFXTemporary");
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		if (Mixer != null)
		{
			audioSource.outputAudioMixerGroup = Mixer;
		}
		audioSource.clip = sound;
		audioSource.volume = (float)OptionsMainMenu.instance.masterVolumeLvl / 10f * (float)OptionsMainMenu.instance.sfxVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(gameObject, sound.length);
	}
}
