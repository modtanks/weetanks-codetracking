using Rewired;
using UnityEngine;

public class ContinuousRotating : MonoBehaviour
{
	public float RotateSpeed = 5f;

	public bool isLightHouse = false;

	private float originalIntensity;

	public bool xAxis = false;

	public bool zAxis = false;

	public bool IsLoadingScreen = false;

	public Player player;

	public GameObject[] ObjToSpawn;

	public GameObject SpawnedObject;

	private void Start()
	{
		player = ReInput.players.GetPlayer(0);
		Light L = GetComponent<Light>();
		if ((bool)L)
		{
			originalIntensity = L.intensity;
		}
		if (!IsLoadingScreen)
		{
			return;
		}
		SpawnedObject = Object.Instantiate(ObjToSpawn[Random.Range(0, ObjToSpawn.Length)], base.transform.position, Quaternion.Euler(0f, 180f, 0f), base.transform);
		MonoBehaviour[] enemyscripts = SpawnedObject.GetComponentsInChildren<MonoBehaviour>();
		MonoBehaviour[] array = enemyscripts;
		foreach (MonoBehaviour script in array)
		{
			script.enabled = false;
			if (script.transform.name == "Shield")
			{
				script.gameObject.SetActive(value: false);
			}
		}
		ParticleSystem[] PS = SpawnedObject.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array2 = PS;
		foreach (ParticleSystem S in array2)
		{
			S.Stop();
			S.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (isLightHouse)
		{
			if (RenderSettings.ambientLight != Color.black)
			{
				Light L2 = GetComponent<Light>();
				if ((bool)L2)
				{
					L2.intensity = 0f;
				}
			}
			else
			{
				Light L = GetComponent<Light>();
				if ((bool)L)
				{
					L.intensity = originalIntensity;
				}
			}
		}
		float addition = 0f;
		if (IsLoadingScreen)
		{
			SpawnedObject.transform.position = base.transform.position;
			base.transform.Rotate(player.GetAxis("Move Vertically") * Time.deltaTime * RotateSpeed, Time.deltaTime * 6f + player.GetAxis("Move Horizontal") * Time.deltaTime * RotateSpeed, 0f);
		}
		else if (xAxis)
		{
			base.transform.Rotate(Time.deltaTime * RotateSpeed, 0f, 0f);
		}
		else if (zAxis)
		{
			base.transform.Rotate(0f, 0f, Time.deltaTime * RotateSpeed);
		}
		else
		{
			base.transform.Rotate(0f, Time.deltaTime * RotateSpeed, 0f);
		}
	}
}
