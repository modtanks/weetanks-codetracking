using Rewired;
using UnityEngine;

public class ContinuousRotating : MonoBehaviour
{
	public float RotateSpeed = 5f;

	public bool isLightHouse;

	private float originalIntensity;

	public bool xAxis;

	public bool zAxis;

	public bool IsLoadingScreen;

	public Player player;

	public GameObject[] ObjToSpawn;

	public GameObject SpawnedObject;

	private void Start()
	{
		player = ReInput.players.GetPlayer(0);
		Light component = GetComponent<Light>();
		if ((bool)component)
		{
			originalIntensity = component.intensity;
		}
		if (!IsLoadingScreen)
		{
			return;
		}
		SpawnedObject = Object.Instantiate(ObjToSpawn[Random.Range(0, ObjToSpawn.Length)], base.transform.position, Quaternion.Euler(0f, 180f, 0f), base.transform);
		MonoBehaviour[] componentsInChildren = SpawnedObject.GetComponentsInChildren<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in componentsInChildren)
		{
			monoBehaviour.enabled = false;
			if (monoBehaviour.transform.name == "Shield")
			{
				monoBehaviour.gameObject.SetActive(value: false);
			}
		}
		ParticleSystem[] componentsInChildren2 = SpawnedObject.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem obj in componentsInChildren2)
		{
			obj.Stop();
			obj.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (isLightHouse)
		{
			if (RenderSettings.ambientLight != Color.black)
			{
				Light component = GetComponent<Light>();
				if ((bool)component)
				{
					component.intensity = 0f;
				}
			}
			else
			{
				Light component2 = GetComponent<Light>();
				if ((bool)component2)
				{
					component2.intensity = originalIntensity;
				}
			}
		}
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
