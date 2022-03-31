using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GlobalAssets : MonoBehaviour
{
	[Serializable]
	public class ModAudioDatabase
	{
		public AudioClip[] ArmourHits;

		public AudioClip[] MenuHover;

		public AudioClip[] MenuAway;

		public AudioClip[] MenuClick;

		public AudioClip[] MenuBack;

		public AudioClip success;

		public AudioClip failure;

		public AudioClip click;

		public AudioClip swoosh;

		public AudioClip unlock;

		public AudioClip extraLife;

		public AudioClip lostLife;

		public AudioClip trackCarpet;

		public AudioClip trackGrass;

		public AudioClip track;

		public AudioClip MinePlace;

		public AudioClip MineExplosion;

		public List<AudioClip> NormalBulletShootSound = new List<AudioClip>();

		public List<AudioClip> RocketBulletShootSound = new List<AudioClip>();

		public List<AudioClip> ExplosiveBulletShootSound = new List<AudioClip>();

		public List<AudioClip> ElectricBulletShootSound = new List<AudioClip>();

		public List<AudioClip> WallBoingSounds = new List<AudioClip>();
	}

	private static GlobalAssets _instance;

	public ModAudioDatabase AudioDB;

	public TMP_FontAsset AsiaFont;

	public List<TankeyTownStockItem> StockDatabase = new List<TankeyTownStockItem>();

	[Header("Cached mod textures")]
	public Texture WoodenBlock_1;

	public Texture WoodenBlock_2;

	public Texture WoodenBlock_1_half;

	public Texture WoodenBlock_2_half;

	public Texture StoneBlock;

	public Texture CorkBlock;

	[Header("Objects")]
	public GameObject TankFlag;

	public List<AudioClip> clips = new List<AudioClip>();

	public AudioClip AC;

	public static GlobalAssets instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		GetAudioMod("shoot_sound_", 1);
		GetAudioMod("mine_place", 0);
		GetAudioMod("mine_explosion", 0);
		Debug.LogError("SHOOT SOUNDS SET");
	}

	private void GetAudioMod(string filename, int index)
	{
		string name = filename + index;
		if (index == 0)
		{
			name = filename;
		}
		if (!UpdateAudio(name, ".wav", AudioType.WAV))
		{
			if (!UpdateAudio(name, ".ogg", AudioType.OGGVORBIS))
			{
				if (UpdateAudio(name, ".mp3", AudioType.MPEG) && index > 0)
				{
					int newIndex3 = index + 1;
					GetAudioMod(filename, newIndex3);
				}
			}
			else if (index > 0)
			{
				int newIndex2 = index + 1;
				GetAudioMod(filename, newIndex2);
			}
		}
		else if (index > 0)
		{
			int newIndex = index + 1;
			GetAudioMod(filename, newIndex);
		}
	}

	private bool UpdateAudio(string filename, string extension, AudioType type)
	{
		string savePath = "";
		savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath = savePath + "/My Games/Wee Tanks/mods/" + filename + extension;
		if (File.Exists(savePath))
		{
			if (filename == "shoot_sound_1")
			{
				AudioDB.NormalBulletShootSound.Clear();
			}
			StartCoroutine(LoadClip(savePath, type, filename));
			return true;
		}
		return false;
	}

	private IEnumerator LoadClip(string path, AudioType type, string filename)
	{
		UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file:///" + path, type);
		yield return req.SendWebRequest();
		AC = DownloadHandlerAudioClip.GetContent(req);
		AC.name = filename;
		if (filename.Contains("shoot_sound"))
		{
			AudioDB.NormalBulletShootSound.Add(AC);
		}
		else if (filename.Contains("mine_place"))
		{
			AudioDB.MinePlace = AC;
		}
		else if (filename.Contains("mine_explosion"))
		{
			AudioDB.MineExplosion = AC;
		}
	}

	private void Update()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
