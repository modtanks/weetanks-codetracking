using UnityEngine;

public class DarkMissionLightEnabler : MonoBehaviour
{
	public GameObject Lights;

	public GameObject LightsP2;

	public GameObject LightsP3;

	public GameObject LightsP4;

	public GameObject[] Lamps;

	public bool isSpotlights = false;

	private void Awake()
	{
		SetLights();
	}

	private void OnDisable()
	{
		SetLights();
	}

	private void Start()
	{
		InvokeRepeating("SetLights", 0.5f, 0.5f);
	}

	private void SetLights()
	{
		if (RenderSettings.ambientLight == Color.black && Lights != null)
		{
			Lights.SetActive(value: true);
			if ((bool)LightsP2)
			{
				LightsP2.SetActive(value: true);
			}
			if (Lamps.Length == 0)
			{
				return;
			}
			if (isSpotlights)
			{
				if (GameMaster.instance.Players[1] != null)
				{
					Lamps[1].SetActive(value: true);
				}
				else if (GameMaster.instance.Players[1] == null)
				{
					Lamps[1].SetActive(value: false);
				}
				if (GameMaster.instance.Players[2] != null)
				{
					Lamps[2].SetActive(value: true);
				}
				else if (GameMaster.instance.Players[2] == null)
				{
					Lamps[2].SetActive(value: false);
				}
				if (GameMaster.instance.Players[3] != null)
				{
					Lamps[3].SetActive(value: true);
				}
				else if (GameMaster.instance.Players[3] == null)
				{
					Lamps[3].SetActive(value: false);
				}
				if (GameMaster.instance.Players[0] != null)
				{
					Lamps[0].SetActive(value: true);
				}
				else if (GameMaster.instance.Players[0] == null)
				{
					Lamps[0].SetActive(value: false);
				}
			}
			else
			{
				GameObject[] lamps = Lamps;
				foreach (GameObject Lamp2 in lamps)
				{
					Lamp2.SetActive(value: true);
				}
			}
			return;
		}
		if ((bool)Lights)
		{
			Lights.SetActive(value: false);
		}
		if ((bool)LightsP2)
		{
			LightsP2.SetActive(value: false);
		}
		if (Lamps.Length != 0)
		{
			GameObject[] lamps2 = Lamps;
			foreach (GameObject Lamp in lamps2)
			{
				Lamp.SetActive(value: false);
			}
		}
	}
}
