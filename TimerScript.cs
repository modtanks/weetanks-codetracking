using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
	private TextMeshProUGUI MyTimerText;

	private void Start()
	{
		MyTimerText = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if ((bool)GameMaster.instance)
		{
			int minutes = 0;
			int seconds = 0;
			int hours = 0;
			int timer = Mathf.RoundToInt(GameMaster.instance.counter);
			hours = ((timer >= 3600) ? Mathf.FloorToInt(timer / 3600) : 0);
			if (hours < 1)
			{
				minutes = ((timer >= 60) ? Mathf.FloorToInt(timer / 60) : 0);
			}
			else
			{
				int noHoursTimer = timer - hours * 3600;
				minutes = ((noHoursTimer >= 60) ? Mathf.FloorToInt(noHoursTimer / 60) : 0);
			}
			seconds = ((minutes < 1) ? timer : ((hours >= 1) ? (timer - minutes * 60 - hours * 3600) : (timer - minutes * 60)));
			string hoursText = ((hours < 10) ? ("0" + hours) : hours.ToString());
			string minutesText = ((minutes < 10) ? ("0" + minutes) : minutes.ToString());
			string secondsText = ((seconds < 10) ? ("0" + seconds) : seconds.ToString());
			MyTimerText.text = hoursText + ":" + minutesText + ":" + secondsText;
		}
	}
}
