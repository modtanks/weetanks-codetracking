using UnityEngine;

public class StormLightning : MonoBehaviour
{
	[Header("TIME FOR LIGHT INCREASE")]
	public float time_lightening;

	[Header("TIME FOR LIGHT DECREASE")]
	public float time_darkening;

	[Header("LIGHT INTENSITY")]
	public float intensity;

	[Header("TIME UNTIL NEXT FLASH")]
	public float wait_time;

	[Header("RANDOMIZE FLASHES")]
	public bool flashIsRand;

	[Header("LIGHTNINGBOLT SPRITES")]
	public GameObject[] lightningBolts;

	private Light myLight;

	private float timeA_stamp;

	private float timeB_stamp;

	private float timeC_stamp;

	private float rand_wait;

	private bool timeA_stamped;

	private bool timeB_stamped;

	private bool timeC_stamped;

	private int rand_sfx;

	private int rand_bolt;

	private int prevBolt;

	private void Start()
	{
		myLight = GetComponent<Light>();
		intensity = 0.3f;
		timeA_stamp = 0f;
		timeB_stamp = 0f;
		timeC_stamp = 0f;
		timeA_stamped = false;
		timeB_stamped = false;
		timeC_stamped = false;
		rand_wait = 0f;
		rand_sfx = 0;
		prevBolt = 0;
		rand_bolt = 0;
	}

	private void Update()
	{
		if (!timeC_stamped)
		{
			if (!timeA_stamped)
			{
				rand_sfx = Random.Range(5, 8);
				timeA_stamp = Time.time;
				timeA_stamped = true;
			}
			else if (!timeB_stamped)
			{
				if (Time.time < timeA_stamp + time_lightening)
				{
					myLight.intensity += intensity * Time.timeScale;
				}
				else
				{
					timeB_stamp = Time.time;
					timeB_stamped = true;
				}
			}
			if (!timeB_stamped)
			{
				return;
			}
			if (Time.time < timeB_stamp + time_darkening)
			{
				myLight.intensity -= intensity * Time.timeScale;
			}
			else if (!timeC_stamped)
			{
				timeC_stamp = Time.time;
				timeC_stamped = true;
				if (flashIsRand)
				{
					rand_wait = Random.Range(1, 7);
				}
			}
		}
		else if (!flashIsRand)
		{
			if (Time.time > timeC_stamp + wait_time)
			{
				timeA_stamped = false;
				timeB_stamped = false;
				timeC_stamped = false;
			}
		}
		else if (Time.time > timeC_stamp + rand_wait)
		{
			timeA_stamped = false;
			timeB_stamped = false;
			timeC_stamped = false;
		}
	}
}
