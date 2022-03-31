using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CloudGeneration : MonoBehaviour
{
	public int CurrentWeatherType = 1;

	public int NewWeatherType = 1;

	public Color[] AmbientColor;

	public Color[] StateSunColor;

	public Color[] StateSunSecondColor;

	public float[] StateSunIntensity;

	public float[] StateSunSecondIntensity;

	public PostProcessVolume GlobalVolume;

	public PostProcessProfile[] PPPs;

	public ParticleSystem Rain;

	public ParticleSystem RainImpact;

	public ThunderLightning Thunder;

	public Material SkyBoxClear;

	public Material SkyBoxDark;

	public Light Sun;

	public Light SunSecond;

	public Light Moon;

	public GameObject CloudPrefab;

	public List<GameObject> SpawnedClouds = new List<GameObject>();

	public float CloudWidth = 10f;

	public float CloudHeight = 10f;

	public float CloudSize = 5f;

	[Header("standard values")]
	public float SunOriginalIntensity;

	public float SecondSunOriginalIntensity;

	public float MoonOriginalIntensity;

	public Color OriginalAmbientColor;

	private static CloudGeneration _instance;

	[Header("AUDIO")]
	public AudioClip RainLoop;

	public AudioClip FogWind;

	public AudioClip[] Thunders;

	public AudioSource RainSource;

	public AudioSource WindSource;

	private bool LatestGameState = false;

	public static CloudGeneration instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		SunOriginalIntensity = Sun.intensity;
		SecondSunOriginalIntensity = SunSecond.intensity;
		MoonOriginalIntensity = Moon.intensity;
		OriginalAmbientColor = RenderSettings.ambientLight;
		float startPosX = base.transform.position.x - CloudWidth * CloudSize / 2f;
		float startPosZ = base.transform.position.z - CloudHeight * CloudSize / 2f;
		for (int x = 0; (float)x < CloudWidth; x++)
		{
			for (int y = 0; (float)y < CloudHeight; y++)
			{
				GameObject CL = Object.Instantiate(CloudPrefab, new Vector3(startPosX + (float)x * CloudSize, base.transform.position.y, startPosZ + (float)y * CloudSize), Quaternion.identity, base.transform);
				CL.name = "Cloud_" + x + "_" + y;
				SpawnedClouds.Add(CL);
				CL.gameObject.SetActive(value: false);
			}
		}
		RenderSettings.ambientLight = AmbientColor[CurrentWeatherType];
		GlobalVolume.profile = PPPs[CurrentWeatherType];
		Sun.color = StateSunColor[CurrentWeatherType];
		SunSecond.color = StateSunSecondColor[CurrentWeatherType];
		RenderSettings.fog = true;
		RenderSettings.fogColor = Color.clear;
		RenderSettings.fogDensity = 0f;
		Sun.intensity = StateSunIntensity[CurrentWeatherType];
		SunSecond.intensity = StateSunSecondIntensity[CurrentWeatherType];
		RenderSettings.fog = false;
		Thunder.isLightning = false;
		if (Rain.isPlaying)
		{
			Rain.Stop();
			RainImpact.Stop();
		}
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem PS in componentsInChildren)
		{
			PS.Stop();
		}
		foreach (Transform child in base.transform)
		{
			CloudController ChildCloud = child.GetComponent<CloudController>();
			if ((bool)ChildCloud)
			{
				ChildCloud.myPS.Stop();
				ChildCloud.enabled = false;
			}
		}
	}

	public void PlayAudio(int type)
	{
		switch (type)
		{
		case 0:
			RainSource.Stop();
			WindSource.Stop();
			break;
		case 1:
			RainSource.Stop();
			WindSource.clip = FogWind;
			WindSource.loop = true;
			WindSource.Play();
			break;
		case 2:
			RainSource.clip = RainLoop;
			RainSource.loop = true;
			RainSource.Play();
			WindSource.clip = FogWind;
			WindSource.loop = true;
			WindSource.Play();
			break;
		case 3:
			RainSource.clip = RainLoop;
			RainSource.loop = true;
			RainSource.Play();
			WindSource.clip = FogWind;
			WindSource.loop = true;
			WindSource.Play();
			break;
		default:
			RainSource.Stop();
			WindSource.Stop();
			break;
		}
	}

	public IEnumerator PlayThunderSound()
	{
		float WaitTime = Random.Range(0.25f, 2f);
		yield return new WaitForSeconds(WaitTime);
		SFXManager.instance.PlaySFX(Thunders[Random.Range(0, Thunders.Length)], 1f, null);
	}

	public void ChangeAmbientColorTo(Color clr)
	{
		Debug.Log("AMBIENT COLOR SETTING");
		StartCoroutine(MakeDark(clr));
	}

	public void MakeItDark()
	{
		StartCoroutine(MakeDark(Color.black));
		StartCoroutine(ChangeToMoon());
	}

	public void MakeItDay()
	{
		StartCoroutine(MakeDark(OriginalAmbientColor));
		StartCoroutine(ChangeToSun());
	}

	private IEnumerator ChangeToMoon()
	{
		float CurrentSunInt = Sun.intensity;
		float CurrentSunSecInt = SunSecond.intensity;
		float CurrentMoonInt = Moon.intensity;
		float SunIntDesire = 0f;
		float SunSecIntDesire = 0f;
		float MoonDesire = 0.14f;
		float t = 0f;
		while (t < 1f)
		{
			Sun.intensity = Mathf.Lerp(CurrentSunInt, SunIntDesire, t);
			SunSecond.intensity = Mathf.Lerp(CurrentSunSecInt, SunSecIntDesire, t);
			Moon.intensity = Mathf.Lerp(CurrentMoonInt, MoonDesire, t);
			t += Time.deltaTime / 2f;
			yield return null;
		}
	}

	private IEnumerator ChangeToSun()
	{
		float CurrentSunInt = Sun.intensity;
		float CurrentSunSecInt = SunSecond.intensity;
		float CurrentMoonInt = Moon.intensity;
		float SunIntDesire = SunOriginalIntensity;
		float SunSecIntDesire = SecondSunOriginalIntensity;
		float MoonDesire = MoonOriginalIntensity;
		float t = 0f;
		while (t < 1f)
		{
			Sun.intensity = Mathf.Lerp(CurrentSunInt, SunIntDesire, t);
			SunSecond.intensity = Mathf.Lerp(CurrentSunSecInt, SunSecIntDesire, t);
			Moon.intensity = Mathf.Lerp(CurrentMoonInt, MoonDesire, t);
			t += Time.deltaTime / 2f;
			yield return null;
		}
	}

	private IEnumerator MakeDark(Color clr)
	{
		Color CurrentColor = RenderSettings.ambientLight;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			RenderSettings.ambientLight = Color.Lerp(CurrentColor, clr, t);
			yield return null;
		}
		Debug.Log("AMBIENT SET");
		RenderSettings.ambientLight = clr;
	}

	private IEnumerator ChangeFogColor(Color ToCLR, float density)
	{
		Color CurrentColor = RenderSettings.ambientLight;
		float CurrentDensity = RenderSettings.fogDensity;
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime / 2f;
			RenderSettings.fogColor = Color.Lerp(CurrentColor, ToCLR, t);
			RenderSettings.fogDensity = Mathf.Lerp(CurrentDensity, density, t);
			yield return null;
		}
		Debug.Log("AMBIENT SET");
		RenderSettings.fogColor = ToCLR;
	}

	private IEnumerator DisableClouds()
	{
		yield return new WaitForSeconds(7f);
		if (CurrentWeatherType != 0)
		{
			yield break;
		}
		foreach (GameObject CL in SpawnedClouds)
		{
			CL.SetActive(value: false);
		}
	}

	public IEnumerator SetWeatherType(int type, bool force)
	{
		int OldWeatherType = CurrentWeatherType;
		CurrentWeatherType = type;
		Debug.Log("setting weather type to:" + type);
		yield return new WaitForSeconds(0.25f);
		if (!(type != OldWeatherType || force))
		{
			yield break;
		}
		CurrentWeatherType = type;
		GlobalVolume.profile = PPPs[type];
		Debug.Log(type);
		PlayAudio(type);
		switch (type)
		{
		case 0:
		{
			OptionsMainMenu.instance.SnowMode = false;
			StartCoroutine(ChangeFogColor(Color.clear, 0f));
			Thunder.isLightning = false;
			if (Rain.isPlaying)
			{
				Rain.Stop();
				RainImpact.Stop();
			}
			StartCoroutine(DisableClouds());
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem PS in componentsInChildren)
			{
				PS.Stop();
			}
			foreach (GameObject child5 in SpawnedClouds)
			{
				ParticleSystem childPS4 = child5.transform.GetChild(0).GetComponent<ParticleSystem>();
				if ((bool)childPS4)
				{
					childPS4.Stop();
				}
			}
			foreach (Transform child4 in base.transform)
			{
				CloudController ChildCloud = child4.GetComponent<CloudController>();
				if ((bool)ChildCloud)
				{
					ChildCloud.myPS.Stop();
					ChildCloud.enabled = false;
				}
			}
			break;
		}
		case 1:
			OptionsMainMenu.instance.SnowMode = false;
			Thunder.isLightning = false;
			RenderSettings.fog = true;
			StartCoroutine(ChangeFogColor(StateSunColor[type], 0.00025f));
			if (Rain.isPlaying)
			{
				Rain.Stop();
				RainImpact.Stop();
			}
			foreach (GameObject CL3 in SpawnedClouds)
			{
				CL3.SetActive(value: true);
			}
			foreach (GameObject child3 in SpawnedClouds)
			{
				ParticleSystem childPS3 = child3.transform.GetChild(0).GetComponent<ParticleSystem>();
				if (!childPS3)
				{
				}
			}
			DoClouds();
			break;
		case 2:
			OptionsMainMenu.instance.SnowMode = false;
			Thunder.isLightning = false;
			RenderSettings.fog = true;
			StartCoroutine(ChangeFogColor(StateSunColor[type], 0.00025f));
			RainSource.clip = RainLoop;
			RainSource.loop = true;
			RainSource.Play();
			if (!Rain.isPlaying)
			{
				Rain.Play();
				RainImpact.Play();
			}
			foreach (GameObject CL2 in SpawnedClouds)
			{
				CL2.SetActive(value: true);
			}
			foreach (GameObject child2 in SpawnedClouds)
			{
				ParticleSystem childPS2 = child2.transform.GetChild(0).GetComponent<ParticleSystem>();
				if (!childPS2)
				{
				}
			}
			DoClouds();
			break;
		case 3:
			OptionsMainMenu.instance.SnowMode = false;
			Thunder.isLightning = true;
			RenderSettings.fog = true;
			StartCoroutine(ChangeFogColor(StateSunColor[type], 0.00025f));
			if (!Rain.isPlaying)
			{
				Rain.Play();
				RainImpact.Play();
			}
			foreach (GameObject CL in SpawnedClouds)
			{
				CL.SetActive(value: true);
			}
			foreach (GameObject child in SpawnedClouds)
			{
				ParticleSystem childPS = child.transform.GetChild(0).GetComponent<ParticleSystem>();
				if (!childPS)
				{
				}
			}
			DoClouds();
			break;
		case 4:
			OptionsMainMenu.instance.SnowMode = true;
			break;
		}
		if (!GameMaster.instance.NightLevels.Contains(GameMaster.instance.CurrentMission))
		{
		}
	}

	public void DoClouds()
	{
		foreach (Transform child in base.transform)
		{
			CloudController ChildCloud = child.GetComponent<CloudController>();
			if (!ChildCloud)
			{
				continue;
			}
			ChildCloud.enabled = true;
			ChildCloud.myPS.Clear();
			if (!ChildCloud.myPS.isPlaying)
			{
				ChildCloud.myPS.Play();
			}
			if (GameMaster.instance.NightLevels.Count > 0)
			{
				if (GameMaster.instance.NightLevels.Contains(GameMaster.instance.CurrentMission))
				{
					ChildCloud.myPS.GetComponent<ParticleSystemRenderer>().material = ChildCloud.DarkFog;
				}
				else
				{
					ChildCloud.myPS.GetComponent<ParticleSystemRenderer>().material = ChildCloud.LightFog;
				}
			}
			else
			{
				ChildCloud.myPS.GetComponent<ParticleSystemRenderer>().material = ChildCloud.LightFog;
			}
		}
	}
}
