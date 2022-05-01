using System.Linq;
using UnityEngine;

public class NewOrchestra : MonoBehaviour
{
	public string currentlyPlaying = "";

	public bool CherryRage = false;

	public int RagingCherries = 0;

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

	public int lastKnownAmountEnemies = 0;

	public float VolumeOffset = -0.5f;

	public bool isPlaying = false;

	public bool isNight = false;

	public int lastKnownMusicVol;

	public int lastKnownMasterVol;

	public bool PausePlaying = false;

	[HideInInspector]
	public bool addClav = false;

	private void Start()
	{
		AudioSource[] instruments = Instruments;
		foreach (AudioSource instr in instruments)
		{
			instr.volume = 0f;
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
		foreach (AudioSource instr in instruments)
		{
			instr.ignoreListenerVolume = true;
			isPlaying = true;
			instr.Play();
		}
		AudioSource[] baseInstruments = BaseInstruments;
		foreach (AudioSource instr2 in baseInstruments)
		{
			instr2.ignoreListenerVolume = true;
			isPlaying = true;
			instr2.Play();
		}
		AudioSource[] specialSongs = SpecialSongs;
		foreach (AudioSource song in specialSongs)
		{
			song.ignoreListenerVolume = true;
			isPlaying = true;
			song.Play();
		}
	}

	public void StopPlaying()
	{
		AudioSource[] instruments = Instruments;
		foreach (AudioSource instr in instruments)
		{
			isPlaying = false;
			instr.Stop();
		}
		AudioSource[] baseInstruments = BaseInstruments;
		foreach (AudioSource instr2 in baseInstruments)
		{
			isPlaying = false;
			instr2.Stop();
		}
		AudioSource[] specialSongs = SpecialSongs;
		foreach (AudioSource song in specialSongs)
		{
			isPlaying = false;
			song.Stop();
		}
	}

	public void SetInstrumentVolumes(int vol)
	{
		AudioSource[] instruments = Instruments;
		foreach (AudioSource instr in instruments)
		{
			instr.volume = vol;
		}
	}

	public void SetBaseVolumes(int vol)
	{
		AudioSource[] baseInstruments = BaseInstruments;
		foreach (AudioSource instr in baseInstruments)
		{
			instr.volume = vol;
		}
	}

	public void SetSongsVolumes(int vol)
	{
		AudioSource[] specialSongs = SpecialSongs;
		foreach (AudioSource song in specialSongs)
		{
			song.volume = vol;
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
		foreach (AudioSource instr in instruments)
		{
			instr.volume = instr.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
		AudioSource[] baseInstruments = BaseInstruments;
		foreach (AudioSource instr2 in baseInstruments)
		{
			instr2.volume = instr2.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		}
		AudioSource[] specialSongs = SpecialSongs;
		foreach (AudioSource song in specialSongs)
		{
			song.volume = song.volume / 10f * (float)OptionsMainMenu.instance.musicVolumeLvl * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
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
			GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
			GameObject[] Bosses = GameObject.FindGameObjectsWithTag("Boss");
			GameObject[] all = Enemies.Concat(Bosses).ToArray();
			if (lastKnownAmountEnemies == 0 && !GameMaster.instance.isZombieMode && !GameMaster.instance.inMapEditor)
			{
				changeMusic(all);
				lastKnownAmountEnemies = all.Length;
			}
			else if (all.Length != lastKnownAmountEnemies)
			{
				changeMusic(all);
				lastKnownAmountEnemies = all.Length;
			}
			else if (CherryRage && currentlyPlaying == "cherry")
			{
				changeMusic(all);
				lastKnownAmountEnemies = all.Length;
			}
			else if (!CherryRage && currentlyPlaying == "cherryrage")
			{
				changeMusic(all);
				lastKnownAmountEnemies = all.Length;
			}
			if (OptionsMainMenu.instance != null && (lastKnownMusicVol != OptionsMainMenu.instance.musicVolumeLvl || lastKnownMasterVol != OptionsMainMenu.instance.masterVolumeLvl))
			{
				lastKnownMusicVol = OptionsMainMenu.instance.musicVolumeLvl;
				lastKnownMasterVol = OptionsMainMenu.instance.masterVolumeLvl;
				changeMusic(all);
				lastKnownAmountEnemies = all.Length;
			}
		}
		else
		{
			lastKnownAmountEnemies = -1;
		}
	}

	private void changeMusic(GameObject[] tanks)
	{
		bool isGoofyBrown = false;
		bool isGrey = false;
		bool isTeal = false;
		bool isYellow = false;
		bool isPurple = false;
		bool isRusset = false;
		bool isRed = false;
		bool isGreen = false;
		bool isWhite = false;
		bool isArmoured = false;
		bool isOrange = false;
		bool isRussian = false;
		bool isBlack = false;
		bool isCherry = false;
		bool isTeleporting = false;
		bool isPeach = false;
		bool isCommando = false;
		bool isSilver = false;
		bool isBoss10 = false;
		bool isBoss11 = false;
		bool isBoss12 = false;
		if (tanks.Length < 1)
		{
			return;
		}
		foreach (GameObject EnemyTank in tanks)
		{
			int ID = EnemyTank.GetComponentInChildren<HealthTanks>().EnemyID;
			if (ID == 1 || ID == -11)
			{
				isGrey = true;
			}
			if (ID == 2)
			{
				isPurple = true;
			}
			if (ID == 3 || ID == -12)
			{
				isTeal = true;
			}
			if (ID == 4)
			{
				isYellow = true;
			}
			if (ID == 5)
			{
				isRed = true;
			}
			if (ID == 110 || ID == -110)
			{
				isBoss10 = true;
			}
			if (ID == 6)
			{
				isGreen = true;
			}
			if (ID == 7)
			{
				isWhite = true;
			}
			if (ID == 9)
			{
				isArmoured = true;
			}
			if (ID == 10)
			{
				isOrange = true;
			}
			if (ID == 11)
			{
				isRussian = true;
			}
			if (ID == 12)
			{
				isBlack = true;
			}
			if (ID == 13)
			{
				isRusset = true;
			}
			if (ID == 14)
			{
				isTeleporting = true;
			}
			if (ID == 15)
			{
				isCherry = true;
			}
			if (ID == 16)
			{
				isPeach = true;
			}
			if (ID == 17)
			{
				isCommando = true;
			}
			if (ID == 18)
			{
				isSilver = true;
			}
			if (ID == 51)
			{
				isGoofyBrown = true;
			}
			if (ID == 150)
			{
				isBoss11 = true;
			}
			if (ID == 170)
			{
				isBoss12 = true;
			}
		}
		SetSongsVolumes(0);
		SetInstrumentVolumes(0);
		SetBaseVolumes(0);
		if (isGoofyBrown)
		{
			SetInstruments(GoofyBrownTankIds, -1);
		}
		else
		{
			SetInstruments(BrownTankIds, 0);
		}
		if (isWhite)
		{
			SetInstruments(WhiteTankIds, 1);
			currentlyPlaying = "white";
		}
		if (isTeleporting)
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
		if (isGrey)
		{
			SetInstruments(GreyTankIds, -1);
			currentlyPlaying = "grey";
		}
		if (isYellow)
		{
			SetInstruments(YellowTankIds, -1);
			currentlyPlaying = "yellow";
		}
		if (isTeal)
		{
			SetInstruments(TealTankIds, -1);
			currentlyPlaying = "teal";
		}
		if (isRusset)
		{
			addClav = true;
			SetInstruments(RussetTankIds, -1);
			currentlyPlaying = "russet";
		}
		else
		{
			addClav = false;
		}
		if (isRed)
		{
			SetInstruments(RedTankIds, -1);
			currentlyPlaying = "red";
		}
		if (isBoss10)
		{
			SetInstruments(Boss10Ids, -1);
			currentlyPlaying = "boss10";
		}
		if (isPurple)
		{
			SetInstruments(PurpleTankIds, -1);
			currentlyPlaying = "purple";
		}
		if (isGreen)
		{
			SetInstruments(GreenTankIds, -1);
			currentlyPlaying = "green";
		}
		if (isOrange)
		{
			SetInstruments(OrangeTankIds, -1);
			currentlyPlaying = "orange";
		}
		if (isArmoured)
		{
			SetInstruments(ExplosiveTankIds, -1);
			currentlyPlaying = "armoured";
		}
		if (isRussian)
		{
			SetInstruments(RocketTankIds, -1);
			currentlyPlaying = "rocket defender";
		}
		if (isBlack)
		{
			SetInstruments(BlackTankIds, -1);
			currentlyPlaying = "black";
		}
		if (isCherry && !CherryRage)
		{
			SetInstruments(CherryTankIds, -1);
			currentlyPlaying = "cherry";
		}
		else if (isCherry && CherryRage)
		{
			SetInstruments(CherryRageTankIds, -1);
			currentlyPlaying = "cherryrage";
		}
		if (isPeach)
		{
			SetInstruments(PeachTankIds, -1);
			currentlyPlaying = "peach";
		}
		if (isCommando)
		{
			SetInstruments(CommandoTankIds, -1);
			currentlyPlaying = "commando";
		}
		if (isSilver)
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
		if (isBoss11)
		{
			SetBossMusic(0);
			currentlyPlaying = "boss 50";
		}
		if (isBoss12)
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
		for (int j = 0; j < instruments.Length; j++)
		{
			instruments[j].volume += VolumeOffset;
		}
		AudioSource[] baseInstruments = BaseInstruments;
		for (int k = 0; k < baseInstruments.Length; k++)
		{
			baseInstruments[k].volume += VolumeOffset;
		}
		AudioSource[] specialSongs = SpecialSongs;
		for (int l = 0; l < specialSongs.Length; l++)
		{
			specialSongs[l].volume += VolumeOffset;
		}
		SetSettingsVolumes();
	}
}
