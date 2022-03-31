using UnityEngine;

public class NightLevelEnabler : MonoBehaviour
{
	public GameObject LightObject;

	public bool isOn = false;

	private bool isEnemy = false;

	public Light theLight;

	public float originalItensity = 0f;

	public float P1dist;

	public float P2dist;

	private HealthTanks HT;

	private EnemyAI EA;

	private void Awake()
	{
		if (RenderSettings.ambientLight == Color.black)
		{
			if (!isOn)
			{
				LightObject.SetActive(value: true);
				isOn = true;
			}
			else
			{
				isOn = false;
				LightObject.SetActive(value: false);
			}
		}
		else
		{
			isOn = false;
			if ((bool)LightObject)
			{
				LightObject.SetActive(value: false);
			}
		}
		if (base.transform.tag == "Enemy")
		{
			isEnemy = true;
			theLight = LightObject.transform.GetChild(0).GetComponent<Light>();
			originalItensity = theLight.intensity;
		}
	}

	private void Start()
	{
		InvokeRepeating("blinking", 2f, 2f);
		HT = GetComponent<HealthTanks>();
		EA = GetComponent<EnemyAI>();
		SetLightColor();
	}

	private void Update()
	{
		if (theLight != null)
		{
			if (theLight.intensity > 0f)
			{
				theLight.intensity -= Time.deltaTime * 35f;
				return;
			}
			theLight.intensity = 0f;
			LightObject.SetActive(value: false);
		}
	}

	private void SetLightColor()
	{
		if (HT.IsAirdropped && LightObject != null && (bool)MapEditorMaster.instance)
		{
			LightObject.GetComponent<MeshRenderer>().material.color = MapEditorMaster.instance.TeamColors[EA.MyTeam];
			LightObject.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", MapEditorMaster.instance.TeamColors[EA.MyTeam]);
			LightObject.GetComponentInChildren<Light>().color = MapEditorMaster.instance.TeamColors[EA.MyTeam];
		}
	}

	private void blinking()
	{
		bool OverRide = false;
		if ((bool)CloudGeneration.instance && (CloudGeneration.instance.CurrentWeatherType == 1 || CloudGeneration.instance.CurrentWeatherType == 2 || CloudGeneration.instance.CurrentWeatherType == 3))
		{
			OverRide = true;
		}
		if (RenderSettings.ambientLight == Color.black || OverRide)
		{
			if (!isOn)
			{
				isOn = true;
				if (theLight != null && isEnemy)
				{
					if ((bool)MapEditorMaster.instance)
					{
						theLight.intensity = originalItensity;
						LightObject.SetActive(value: true);
						return;
					}
					float minDistToPlayer = 30f;
					float closestPos = 999999f;
					for (int i = 0; i < GameMaster.instance.PlayerJoined.Count; i++)
					{
						float Playerdist = Vector3.Distance(base.transform.position, GameMaster.instance.Players[i].transform.position);
						if (Playerdist < closestPos)
						{
							closestPos = Playerdist;
							if (Playerdist < minDistToPlayer)
							{
								theLight.intensity = originalItensity - Playerdist * originalItensity / minDistToPlayer;
								LightObject.SetActive(value: true);
							}
							else
							{
								theLight.intensity = 0f;
								LightObject.SetActive(value: false);
							}
						}
					}
					if (closestPos >= minDistToPlayer)
					{
						theLight.intensity = 0f;
						LightObject.SetActive(value: false);
					}
				}
				SetLightColor();
			}
			else
			{
				isOn = false;
				LightObject.SetActive(value: false);
			}
		}
		else
		{
			isOn = false;
			LightObject.SetActive(value: false);
		}
	}
}
