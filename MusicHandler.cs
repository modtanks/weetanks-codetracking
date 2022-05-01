using UnityEngine;

public class MusicHandler : MonoBehaviour
{
	public GameMaster masterScript;

	public AudioSource MusicSource;

	public AudioClip BeginMusicClip;

	public AudioClip FirstMissionStart;

	public AudioClip FirstMidPart;

	public AudioClip SecMidPart;

	public AudioClip VictoryClip;

	public AudioClip DefeatClip;

	public AudioClip SwitchClip;

	public AudioClip Boss30Music;

	public AudioClip Boss30MidPart;

	public AudioClip Boss70Start;

	public AudioClip Boss70MidPart;

	public NewOrchestra Orchestra;

	public bool CanStartMusic = false;

	public bool canStartMainPart = false;

	public bool paused = false;

	private void Start()
	{
		Orchestra.StopPlaying();
	}

	private void Update()
	{
		if (!(GameMaster.instance != null) || !(Time.timeScale > 0.5f) || GameMaster.instance.GameHasPaused || paused || MusicSource.isPlaying || Orchestra.isPlaying)
		{
			return;
		}
		MusicSource.clip = null;
		if (masterScript.CurrentMission == 29 && !MapEditorMaster.instance)
		{
			if (CanStartMusic)
			{
				CanStartMusic = false;
				canStartMainPart = true;
				MusicSource.loop = false;
				MusicSource.clip = Boss30Music;
				MusicSource.Play();
			}
			else if (canStartMainPart)
			{
				GameMaster.instance.restartGame = false;
				CanStartMusic = false;
				MusicSource.loop = true;
				MusicSource.clip = Boss30MidPart;
				masterScript.StartGame();
				MusicSource.Play();
			}
		}
		else if (masterScript.CurrentMission == 69)
		{
			if (CanStartMusic)
			{
				Debug.Log("BEGIN PARTY");
				CanStartMusic = false;
				MusicSource.loop = false;
				MusicSource.clip = Boss70Start;
				MusicSource.Play();
				canStartMainPart = true;
			}
			else if (canStartMainPart)
			{
				GameMaster.instance.restartGame = false;
				CanStartMusic = false;
				MusicSource.loop = true;
				MusicSource.clip = Boss70MidPart;
				masterScript.StartGame();
				MusicSource.Play();
			}
		}
		else if (CanStartMusic)
		{
			Orchestra.StartPlaying();
			GameMaster.instance.restartGame = false;
			if (!masterScript.isZombieMode)
			{
				masterScript.StartGame();
			}
			CanStartMusic = false;
		}
	}

	public void StopMusic()
	{
		Orchestra.StopPlaying();
		CanStartMusic = false;
		canStartMainPart = false;
		MusicSource.Stop();
		MusicSource.loop = false;
		MusicSource.clip = null;
	}

	public void Victory()
	{
		Orchestra.StopPlaying();
		CanStartMusic = false;
		canStartMainPart = false;
		MusicSource.Stop();
		MusicSource.loop = false;
		MusicSource.clip = null;
		if (!GameMaster.instance.isZombieMode)
		{
			MusicSource.PlayOneShot(VictoryClip);
		}
	}

	public void Defeat()
	{
		Orchestra.StopPlaying();
		CanStartMusic = false;
		canStartMainPart = false;
		MusicSource.Stop();
		MusicSource.loop = false;
		MusicSource.clip = null;
	}

	public void StartMusic()
	{
		if ((masterScript.CurrentMission != 29 && masterScript.CurrentMission != 69) || (bool)MapEditorMaster.instance)
		{
			Orchestra.StopPlaying();
			MusicSource.Stop();
			MusicSource.loop = false;
			MusicSource.clip = BeginMusicClip;
			Debug.Log("BEGIN CLIP");
			MusicSource.Play();
			CanStartMusic = true;
		}
	}

	public void StartSurvivalMusic()
	{
		Orchestra.StopPlaying();
		MusicSource.Stop();
		MusicSource.loop = false;
		MusicSource.clip = BeginMusicClip;
		Debug.Log("BEGIN survival CLIP");
		MusicSource.Play();
		CanStartMusic = true;
	}
}
