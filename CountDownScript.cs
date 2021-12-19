using TMPro;
using UnityEngine;

public class CountDownScript : MonoBehaviour
{
	public TextMeshProUGUI Text1;

	public TextMeshProUGUI Text2;

	public float count;

	public bool start;

	public MusicHandler musicScript;

	private void Start()
	{
	}

	private void Update()
	{
		if (start && (double)count < 2.49 && count >= 0f)
		{
			if (!musicScript.MusicSource.isPlaying)
			{
				if (GameMaster.instance.CurrentMission != 29 && !GameMaster.instance.GameHasPaused)
				{
					musicScript.MusicSource.PlayOneShot(musicScript.BeginMusicClip);
				}
				if (!GameMaster.instance.GameHasPaused)
				{
					musicScript.CanStartMusic = true;
				}
			}
			Text1.text = Mathf.RoundToInt(3f - count) + "..";
			Text2.text = Mathf.RoundToInt(3f - count) + "..";
			count += Time.deltaTime;
		}
		else if ((double)count < 3.49 && start && count >= 0f)
		{
			Text1.text = "Start!";
			Text2.text = "Start!";
			count += Time.deltaTime;
		}
		else if (count <= 0f && start)
		{
			count += Time.deltaTime;
			Text1.text = "";
			Text2.text = "";
		}
		else if (start)
		{
			if (GameMaster.instance != null && !GameMaster.instance.GameHasStarted && GameMaster.instance.isZombieMode)
			{
				GameMaster.instance.NewRound();
			}
			count = -0.49f;
			Text1.text = "";
			Text2.text = "";
			start = false;
		}
		else if (!start)
		{
			count = -0.49f;
			Text1.text = "";
			Text2.text = "";
		}
	}
}
