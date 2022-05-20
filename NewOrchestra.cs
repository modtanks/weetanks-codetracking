using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewOrchestra : MonoBehaviour
{
	[Serializable]
	public class TankIds
	{
		public string TankName;

		public int TankID;

		public int[] ids;
	}

	public string currentlyPlaying = "";

	public bool CherryRage;

	public int RagingCherries;

	public AudioSource[] BaseInstruments;

	public AudioSource[] Instruments;

	public AudioSource[] SpecialSongs;

	[Header("Base Tanks")]
	public int[] WhiteTankIds;

	public int[] TeleportingTankIds;

	[Header("Boss Tanks Instrumentals")]
	public int[] Boss10Ids;

	[Header("Additional Tanks")]
	public int[] BrownTankIds;

	public int[] GoofyBrownTankIds;

	public int[] GreyTankIds;

	public int[] TealTankIds;

	public int[] YellowTankIds;

	public int[] RussetTankIds;

	public int[] RedTankIds;

	public int[] PurpleTankIds;

	public int[] GreenTankIds;

	public int[] OrangeTankIds;

	public int[] ExplosiveTankIds;

	public int[] BlackTankIds;

	public int[] RocketTankIds;

	public int[] CherryTankIds;

	public int[] CherryRageTankIds;

	public int[] PeachTankIds;

	public int[] CommandoTankIds;

	public int[] SilverTankIds;

	public List<TankIds> TankInstruments = new List<TankIds>();

	public int lastKnownAmountEnemies;

	public float VolumeOffset = -0.5f;

	public bool isPlaying;

	public bool isNight;

	public int lastKnownMusicVol;

	public int lastKnownMasterVol;

	public bool PausePlaying;

	[HideInInspector]
	private void Start()
	{
		AudioSource[] instruments = Instruments;
		for (int i = 0; i < instruments.Length; i++)
		{
			instruments[i].volume = 0f;
		}
		if (OptionsMainMenu.instance != null)
		{
			lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
			lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
		}
		SetSongsVolumes(0);
		SetInstrumentVolumes(0);
		SetBaseVolumes(0);
	}

	public void StartPlaying()
	{
		AudioSource[] instruments = Instruments;
		foreach (AudioSource obj in instruments)
		{
			obj.ignoreListenerVolume = true;
			isPlaying = true;
			obj.Play();
		}
		instruments = BaseInstruments;
		foreach (AudioSource obj2 in instruments)
		{
			obj2.ignoreListenerVolume = true;
			isPlaying = true;
			obj2.Play();
		}
		instruments = SpecialSongs;
		foreach (AudioSource obj3 in instruments)
		{
			obj3.ignoreListenerVolume = true;
			isPlaying = true;
			obj3.Play();
		}
	}

	public void StopPlaying()
	{
		AudioSource[] instruments = Instruments;
		foreach (AudioSource obj in instruments)
		{
			isPlaying = false;
			obj.Stop();
		}
		instruments = BaseInstruments;
		foreach (AudioSource obj2 in instruments)
		{
			isPlaying = false;
			obj2.Stop();
		}
		instruments = SpecialSongs;
		foreach (AudioSource obj3 in instruments)
		{
			isPlaying = false;
			obj3.Stop();
		}
	}

	public void SetInstrumentVolumes(int vol)
	{
		AudioSource[] instruments = Instruments;
		for (int i = 0; i < instruments.Length; i++)
		{
			instruments[i].volume = vol;
		}
	}

	public void SetBaseVolumes(int vol)
	{
		AudioSource[] baseInstruments = BaseInstruments;
		for (int i = 0; i < baseInstruments.Length; i++)
		{
			baseInstruments[i].volume = vol;
		}
	}

	public void SetSongsVolumes(int vol)
	{
		AudioSource[] specialSongs = SpecialSongs;
		for (int i = 0; i < specialSongs.Length; i++)
		{
			specialSongs[i].volume = vol;
		}
	}

	public void SetBossMusic(int song)
	{
		SetInstrumentVolumes(0);
		SetBaseVolumes(0);
		SetSongsVolumes(0);
		Debug.Log("now lets set the music for the boss");
		SpecialSongs[song].volume = 1f;
	}

	public void SetInstruments(int[] instrs, int baseInstr)
	{
		for (int i = 0; i < instrs.Length; i++)
		{
			if (instrs[i] == 0 && !isNight)
			{
				Instruments[0].volume = 1f;
			}
			else if (instrs[i] == 1 && isNight)
			{
				Instruments[1].volume = 1f;
			}
			else
			{
				Instruments[instrs[i]].volume = 1f;
			}
		}
		if (baseInstr > -1)
		{
			SetBaseVolumes(0);
			BaseInstruments[baseInstr].volume = 1f;
		}
	}

	public void SetSettingsVolumes()
	{
		AudioSource[] instruments = Instruments;
		foreach (AudioSource obj in instruments)
		{
			obj.volume = obj.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
		instruments = BaseInstruments;
		foreach (AudioSource obj2 in instruments)
		{
			obj2.volume = obj2.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
		instruments = SpecialSongs;
		foreach (AudioSource obj3 in instruments)
		{
			obj3.volume = obj3.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
	}

	public void MuteAll()
	{
	}

	private void Update()
	{
		if (RagingCherries <= 0)
		{
			CherryRage = false;
		}
		if ((bool)GameMaster.instance && !GameMaster.instance.GameHasStarted)
		{
			RagingCherries = 0;
		}
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
			else if (CherryRage && currentlyPlaying == "cherry")
			{
				changeMusic(array);
				lastKnownAmountEnemies = array.Length;
			}
			else if (!CherryRage && currentlyPlaying == "cherryrage")
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
		else
		{
			lastKnownAmountEnemies = -1;
		}
	}

	private void changeMusic(GameObject[] tanks)
	{
		bool flag = false;
		bool flag2 = false;
		if (tanks.Length < 1)
		{
			return;
		}
		SetSongsVolumes(0);
		SetInstrumentVolumes(0);
		SetBaseVolumes(0);
		SetInstruments(BrownTankIds, 0);
		if (RenderSettings.ambientLight == Color.black)
		{
			isNight = true;
		}
		else
		{
			isNight = false;
		}
		if ((bool)CloudGeneration.instance)
		{
			if (CloudGeneration.instance.CurrentWeatherType == 2)
			{
				SetBaseVolumes(0);
				BaseInstruments[4].volume = 1f;
			}
			else if (CloudGeneration.instance.CurrentWeatherType == 3)
			{
				SetBaseVolumes(0);
				BaseInstruments[5].volume = 1f;
			}
		}
		foreach (GameObject gameObject in tanks)
		{
			int ID = gameObject.GetComponentInChildren<HealthTanks>().EnemyID;
			bool flag3 = false;
			if (ID == 7)
			{
				BaseInstruments[0].volume = 0f;
				BaseInstruments[1].volume = 1f;
				SetInstruments(WhiteTankIds, 1);
				currentlyPlaying = "white";
			}
			if (ID == 14)
			{
				BaseInstruments[0].volume = 0f;
				BaseInstruments[2].volume = 1f;
				SetInstruments(TeleportingTankIds, 2);
				currentlyPlaying = "electro";
			}
			if (ID == 15 && CherryRage)
			{
				flag3 = true;
			}
			TankIds tankIds = ((!flag3) ? TankInstruments.Find((TankIds x) => x.TankID == ID) : TankInstruments.Find((TankIds x) => x.TankName == "cherryrage"));
			if (tankIds != null)
			{
				SetInstruments(tankIds.ids, -1);
				currentlyPlaying = tankIds.TankName;
			}
			if (ID == 150)
			{
				flag = true;
			}
			if (ID == 170)
			{
				flag2 = true;
			}
		}
		if (!isNight)
		{
			Instruments[0].volume = 1f;
			Instruments[1].volume = 0f;
		}
		else if (isNight)
		{
			Instruments[1].volume = 1f;
			Instruments[0].volume = 0f;
		}
		if (flag)
		{
			SetBossMusic(0);
			currentlyPlaying = "boss 50";
		}
		if (flag2)
		{
			SetBossMusic(1);
			currentlyPlaying = "boss 50";
		}
		if ((bool)CloudGeneration.instance)
		{
			if (CloudGeneration.instance.CurrentWeatherType == 2)
			{
				Instruments[1].volume = 0f;
				Instruments[0].volume = 0f;
			}
			else if (CloudGeneration.instance.CurrentWeatherType == 3)
			{
				SetSongsVolumes(0);
				SetInstrumentVolumes(0);
				Instruments[1].volume = 0f;
				Instruments[0].volume = 0f;
			}
		}
		AudioSource[] instruments = Instruments;
		for (int i = 0; i < instruments.Length; i++)
		{
			instruments[i].volume += VolumeOffset;
		}
		instruments = BaseInstruments;
		for (int i = 0; i < instruments.Length; i++)
		{
			instruments[i].volume += VolumeOffset;
		}
		instruments = SpecialSongs;
		for (int i = 0; i < instruments.Length; i++)
		{
			instruments[i].volume += VolumeOffset;
		}
		SetSettingsVolumes();
	}
}
