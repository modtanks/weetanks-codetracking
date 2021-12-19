using System.Linq;
using UnityEngine;

public class OrchestraHandler : MonoBehaviour
{
	public AudioSource BaseLoop;

	public AudioSource Trumpets;

	public AudioSource DrumAndBells;

	public AudioSource KickAndTuba;

	public AudioSource BigBong;

	public AudioSource Timpani;

	public AudioSource BaseSynth;

	public AudioSource BaseNight;

	public AudioSource ExtraDrum;

	public bool isGrey;

	public bool isTeal;

	public bool isYellow;

	public bool isRed;

	public bool isGreen;

	public bool isPurple;

	public bool isWhite;

	public bool isArmoured;

	public bool isOrange;

	public bool isRussian;

	public bool isBlack;

	public int lastKnownAmountEnemies;

	public float VolumeOffset = -0.5f;

	public bool isPlaying;

	public int lastKnownMusicVol;

	public int lastKnownMasterVol;

	private void Start()
	{
		BaseLoop.volume = 1f;
		Trumpets.volume = 0f;
		DrumAndBells.volume = 0f;
		KickAndTuba.volume = 0f;
		BigBong.volume = 0f;
		Timpani.volume = 0f;
		BaseSynth.volume = 0f;
		BaseNight.volume = 0f;
		ExtraDrum.volume = 0f;
		if (OptionsMainMenu.instance != null)
		{
			lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
			lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
		}
	}

	public void StartPlaying()
	{
		BaseLoop.ignoreListenerVolume = true;
		Trumpets.ignoreListenerVolume = true;
		DrumAndBells.ignoreListenerVolume = true;
		KickAndTuba.ignoreListenerVolume = true;
		BigBong.ignoreListenerVolume = true;
		Timpani.ignoreListenerVolume = true;
		BaseSynth.ignoreListenerVolume = true;
		BaseNight.ignoreListenerVolume = true;
		ExtraDrum.ignoreListenerVolume = true;
		isPlaying = true;
		BaseLoop.Play();
		Trumpets.Play();
		DrumAndBells.Play();
		KickAndTuba.Play();
		BigBong.Play();
		Timpani.Play();
		BaseSynth.Play();
		BaseNight.Play();
		ExtraDrum.Play();
	}

	public void StopPlaying()
	{
		isPlaying = false;
		BaseLoop.Stop();
		Trumpets.Stop();
		DrumAndBells.Stop();
		KickAndTuba.Stop();
		BigBong.Stop();
		Timpani.Stop();
		BaseSynth.Stop();
		BaseNight.Stop();
		ExtraDrum.Stop();
	}

	private void Update()
	{
		if (isPlaying)
		{
			GameObject[] first = GameObject.FindGameObjectsWithTag("Enemy");
			GameObject[] second = GameObject.FindGameObjectsWithTag("Boss");
			GameObject[] array = first.Concat(second).ToArray();
			if (lastKnownAmountEnemies == 0 && !GameMaster.instance.isZombieMode && !GameMaster.instance.inMapEditor)
			{
				changeMusic(array);
				lastKnownAmountEnemies = array.Length;
			}
			else if (array.Length != lastKnownAmountEnemies)
			{
				changeMusic(array);
				lastKnownAmountEnemies = array.Length;
			}
			if (OptionsMainMenu.instance != null && (lastKnownMusicVol != OptionsMainMenu.instance.musicVolumeLvl || lastKnownMasterVol != OptionsMainMenu.instance.masterVolumeLvl))
			{
				lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
				lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
				changeMusic(array);
				lastKnownAmountEnemies = array.Length;
			}
		}
	}

	private void SetVolumes()
	{
		BaseLoop.volume = BaseLoop.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		Trumpets.volume = Trumpets.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		DrumAndBells.volume = DrumAndBells.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		KickAndTuba.volume = KickAndTuba.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		BigBong.volume = BigBong.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		Timpani.volume = Timpani.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		BaseSynth.volume = BaseSynth.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		BaseNight.volume = BaseNight.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		ExtraDrum.volume = ExtraDrum.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
	}

	private void changeMusic(GameObject[] tanks)
	{
		isGrey = false;
		isTeal = false;
		isYellow = false;
		isPurple = false;
		isRed = false;
		isGreen = false;
		isWhite = false;
		isArmoured = false;
		isOrange = false;
		isRussian = false;
		isBlack = false;
		Debug.LogWarning("changing music...");
		foreach (GameObject gameObject in tanks)
		{
			if (gameObject.transform.parent.name == "Enemy_Tank-2")
			{
				isGrey = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-3")
			{
				isPurple = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-4" || gameObject.transform.parent.name == "Enemy_Tank-AI2(Clone)")
			{
				isTeal = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-5")
			{
				isYellow = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-6" || gameObject.transform.parent.name == "Boss_Tank-10" || gameObject.transform.parent.name == "Enemy_Tank-AI3(Clone)")
			{
				isRed = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-7")
			{
				isGreen = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-8")
			{
				isWhite = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-10")
			{
				isArmoured = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-11")
			{
				isOrange = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-12" || gameObject.transform.parent.name == "Boss_Tank-50-Others")
			{
				isRussian = true;
			}
			if (gameObject.transform.parent.name == "Enemy_Tank-13")
			{
				isBlack = true;
			}
		}
		BaseLoop.volume = 0f;
		Trumpets.volume = 0f;
		DrumAndBells.volume = 0f;
		KickAndTuba.volume = 0f;
		BigBong.volume = 0f;
		Timpani.volume = 0f;
		BaseSynth.volume = 0f;
		BaseNight.volume = 0f;
		ExtraDrum.volume = 0f;
		if (!isWhite)
		{
			BaseLoop.volume = 1f;
			Trumpets.volume = 0f;
			DrumAndBells.volume = 0f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			BaseSynth.volume = 0f;
			BaseNight.volume = 0f;
			ExtraDrum.volume = 0f;
		}
		else
		{
			BaseLoop.volume = 0f;
			Trumpets.volume = 0f;
			DrumAndBells.volume = 0f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			BaseSynth.volume = 1f;
			BaseNight.volume = 0f;
			ExtraDrum.volume = 0f;
		}
		if (RenderSettings.ambientLight == Color.black)
		{
			BaseLoop.volume = 0f;
			Trumpets.volume = 0f;
			DrumAndBells.volume = 0f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			BaseSynth.volume = 0f;
			BaseNight.volume = 1f;
			ExtraDrum.volume = 0f;
		}
		if (isTeal || isYellow)
		{
			Trumpets.volume = 1f;
			DrumAndBells.volume = 0f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			ExtraDrum.volume = 0f;
		}
		if (isRed || isPurple)
		{
			Trumpets.volume = 0f;
			DrumAndBells.volume = 1f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			ExtraDrum.volume = 0f;
		}
		if (isOrange)
		{
			Trumpets.volume = 0f;
			DrumAndBells.volume = 1f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			ExtraDrum.volume = 0f;
		}
		if (isGreen)
		{
			Trumpets.volume = 0f;
			DrumAndBells.volume = 0f;
			KickAndTuba.volume = 1f;
			BigBong.volume = 0f;
			Timpani.volume = 0f;
			ExtraDrum.volume = 0f;
		}
		if (isArmoured)
		{
			Trumpets.volume = 0f;
			DrumAndBells.volume = 1f;
			KickAndTuba.volume = 0f;
			BigBong.volume = 1f;
			Timpani.volume = 1f;
			ExtraDrum.volume = 0f;
		}
		if (isRussian)
		{
			Trumpets.volume = 0f;
			DrumAndBells.volume = 0f;
			KickAndTuba.volume = 1f;
			BigBong.volume = 0f;
			Timpani.volume = 1f;
			ExtraDrum.volume = 0f;
		}
		if (isBlack)
		{
			Trumpets.volume = 0f;
			DrumAndBells.volume = 1f;
			KickAndTuba.volume = 1f;
			BigBong.volume = 0f;
			Timpani.volume = 1f;
			ExtraDrum.volume = 1f;
		}
		BaseLoop.volume += VolumeOffset;
		Trumpets.volume += VolumeOffset;
		DrumAndBells.volume += VolumeOffset;
		KickAndTuba.volume += VolumeOffset;
		BigBong.volume += VolumeOffset;
		Timpani.volume += VolumeOffset;
		BaseSynth.volume += VolumeOffset;
		BaseNight.volume += VolumeOffset;
		ExtraDrum.volume += VolumeOffset;
		SetVolumes();
	}
}
