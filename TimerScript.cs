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
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = Mathf.RoundToInt(GameMaster.instance.counter);
			num3 = ((num4 >= 3600) ? Mathf.FloorToInt(num4 / 3600) : 0);
			if (num3 < 1)
			{
				num = ((num4 >= 60) ? Mathf.FloorToInt(num4 / 60) : 0);
			}
			else
			{
				int num5 = num4 - num3 * 3600;
				num = ((num5 >= 60) ? Mathf.FloorToInt(num5 / 60) : 0);
			}
			num2 = ((num < 1) ? num4 : ((num3 >= 1) ? (num4 - num * 60 - num3 * 3600) : (num4 - num * 60)));
			string text = ((num3 < 10) ? ("0" + num3) : num3.ToString());
			string text2 = ((num < 10) ? ("0" + num) : num.ToString());
			string text3 = ((num2 < 10) ? ("0" + num2) : num2.ToString());
			MyTimerText.text = text + ":" + text2 + ":" + text3;
		}
	}
}
