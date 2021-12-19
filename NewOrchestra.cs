using System.Linq;
using UnityEngine;

public class NewOrchestra : MonoBehaviour
{
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

	public int lastKnownAmountEnemies;

	public float VolumeOffset = -0.5f;

	public bool isPlaying;

	public bool isNight;

	public int lastKnownMusicVol;

	public int lastKnownMasterVol;

	public bool PausePlaying;

	[HideInInspector]
	public bool addClav;

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
		if (addClav)
		{
			Instruments[15].volume = 1f;
		}
		else
		{
			Instruments[15].volume = 0f;
		}
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
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		bool flag9 = false;
		bool flag10 = false;
		bool flag11 = false;
		bool flag12 = false;
		bool flag13 = false;
		bool flag14 = false;
		bool flag15 = false;
		bool flag16 = false;
		bool flag17 = false;
		bool flag18 = false;
		bool flag19 = false;
		bool flag20 = false;
		bool flag21 = false;
		if (tanks.Length < 1)
		{
			return;
		}
		for (int i = 0; i < tanks.Length; i++)
		{
			int enemyID = tanks[i].GetComponentInChildren<HealthTanks>().EnemyID;
			if (enemyID == 1 || enemyID == -11)
			{
				flag2 = true;
			}
			if (enemyID == 2)
			{
				flag5 = true;
			}
			if (enemyID == 3 || enemyID == -12)
			{
				flag3 = true;
			}
			if (enemyID == 4)
			{
				flag4 = true;
			}
			if (enemyID == 5)
			{
				flag7 = true;
			}
			if (enemyID == 110 || enemyID == -110)
			{
				flag19 = true;
			}
			if (enemyID == 6)
			{
				flag8 = true;
			}
			if (enemyID == 7)
			{
				flag9 = true;
			}
			if (enemyID == 9)
			{
				flag10 = true;
			}
			if (enemyID == 10)
			{
				flag11 = true;
			}
			if (enemyID == 11)
			{
				flag12 = true;
			}
			if (enemyID == 12)
			{
				flag13 = true;
			}
			if (enemyID == 13)
			{
				flag6 = true;
			}
			if (enemyID == 14)
			{
				flag15 = true;
			}
			if (enemyID == 15)
			{
				flag14 = true;
			}
			if (enemyID == 16)
			{
				flag16 = true;
			}
			if (enemyID == 17)
			{
				flag17 = true;
			}
			if (enemyID == 18)
			{
				flag18 = true;
			}
			if (enemyID == 51)
			{
				flag = true;
			}
			if (enemyID == 150)
			{
				flag20 = true;
			}
			if (enemyID == 170)
			{
				flag21 = true;
			}
		}
		SetSongsVolumes(0);
		SetInstrumentVolumes(0);
		SetBaseVolumes(0);
		if (flag)
		{
			SetInstruments(GoofyBrownTankIds, -1);
		}
		else
		{
			SetInstruments(BrownTankIds, 0);
		}
		if (flag9)
		{
			SetInstruments(WhiteTankIds, 1);
			currentlyPlaying = "white";
		}
		if (flag15)
		{
			SetInstruments(TeleportingTankIds, 2);
			currentlyPlaying = "electro";
		}
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
		if (flag2)
		{
			SetInstruments(GreyTankIds, -1);
			currentlyPlaying = "grey";
		}
		if (flag4)
		{
			SetInstruments(YellowTankIds, -1);
			currentlyPlaying = "yellow";
		}
		if (flag3)
		{
			SetInstruments(TealTankIds, -1);
			currentlyPlaying = "teal";
		}
		if (flag6)
		{
			addClav = true;
			SetInstruments(RussetTankIds, -1);
			currentlyPlaying = "russet";
		}
		else
		{
			addClav = false;
		}
		if (flag7)
		{
			SetInstruments(RedTankIds, -1);
			currentlyPlaying = "red";
		}
		if (flag19)
		{
			SetInstruments(Boss10Ids, -1);
			currentlyPlaying = "boss10";
		}
		if (flag5)
		{
			SetInstruments(PurpleTankIds, -1);
			currentlyPlaying = "purple";
		}
		if (flag8)
		{
			SetInstruments(GreenTankIds, -1);
			currentlyPlaying = "green";
		}
		if (flag11)
		{
			SetInstruments(OrangeTankIds, -1);
			currentlyPlaying = "orange";
		}
		if (flag10)
		{
			SetInstruments(ExplosiveTankIds, -1);
			currentlyPlaying = "armoured";
		}
		if (flag12)
		{
			SetInstruments(RocketTankIds, -1);
			currentlyPlaying = "rocket defender";
		}
		if (flag13)
		{
			SetInstruments(BlackTankIds, -1);
			currentlyPlaying = "black";
		}
		if (flag14 && !CherryRage)
		{
			SetInstruments(CherryTankIds, -1);
			currentlyPlaying = "cherry";
		}
		else if (flag14 && CherryRage)
		{
			SetInstruments(CherryRageTankIds, -1);
			currentlyPlaying = "cherryrage";
		}
		if (flag16)
		{
			SetInstruments(PeachTankIds, -1);
			currentlyPlaying = "peach";
		}
		if (flag17)
		{
			SetInstruments(CommandoTankIds, -1);
			currentlyPlaying = "commando";
		}
		if (flag18)
		{
			SetInstruments(SilverTankIds, -1);
			currentlyPlaying = "silver";
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
		if (flag20)
		{
			SetBossMusic(0);
			currentlyPlaying = "boss 50";
		}
		if (flag21)
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
