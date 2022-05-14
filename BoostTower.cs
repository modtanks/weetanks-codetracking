using System.Collections.Generic;
using UnityEngine;

public class BoostTower : MonoBehaviour
{
	public Light TowerLight;

	public ParticleSystem TowerParticles;

	public ParticleSystem PlayerParticles;

	public float TowerRange = 6f;

	public AudioClip TowerBoostingSound;

	public float NormalLightIntensity;

	public float PlayerLightIntensity;

	public Color TowerColor;

	public MeshRenderer TowerGlowBlock;

	public float BlockLightIntensity;

	public float BlockPlayerLightIntensity;

	public List<MoveTankScript> PlayersInRange = new List<MoveTankScript>();

	public GameObject TowerDestruction;

	public bool InitiateDestroyTower;

	private bool prevent;

	private void Start()
	{
		TowerGlowBlock.material.SetVector("_EmissionColor", TowerColor * BlockLightIntensity);
		InvokeRepeating("CheckForPlayers", 0.25f, 0.25f);
	}

	private void CheckForPlayers()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, TowerRange);
		PlayersInRange.Clear();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider.tag == "Player")
			{
				MoveTankScript component = collider.gameObject.GetComponent<MoveTankScript>();
				if ((bool)component && !PlayersInRange.Contains(component))
				{
					PlayersInRange.Add(component);
				}
			}
		}
	}

	public void DestroyTower()
	{
		Object.Destroy(Object.Instantiate(TowerDestruction, base.transform.position, Quaternion.identity), 4f);
		base.transform.parent.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (InitiateDestroyTower && !prevent)
		{
			prevent = true;
			DestroyTower();
		}
		if (PlayersInRange.Count > 0)
		{
			TowerLight.intensity = Mathf.Lerp(TowerLight.intensity, PlayerLightIntensity, Time.deltaTime);
			float num = TowerGlowBlock.material.GetVector("_EmissionColor")[3];
			if (num < BlockPlayerLightIntensity)
			{
				float num2 = num + 0.05f;
				TowerGlowBlock.material.SetVector("_EmissionColor", TowerColor * num2);
			}
			if (!TowerParticles.isPlaying)
			{
				TowerParticles.Play();
			}
			List<MoveTankScript> list = new List<MoveTankScript>();
			foreach (MoveTankScript item in PlayersInRange)
			{
				if (item != null)
				{
					if (Vector3.Distance(base.transform.position, item.transform.position) <= TowerRange + 0.3f)
					{
						item.isBeingTowerBoosted = true;
						continue;
					}
					item.isBeingTowerBoosted = false;
					list.Add(item);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			{
				foreach (MoveTankScript item2 in list)
				{
					item2.isBeingTowerBoosted = false;
					PlayersInRange.Remove(item2);
				}
				return;
			}
		}
		float num3 = TowerGlowBlock.material.GetVector("_EmissionColor")[3];
		if (num3 > BlockLightIntensity)
		{
			float num4 = num3 - 0.05f;
			TowerGlowBlock.material.SetVector("_EmissionColor", TowerColor * num4);
		}
		TowerParticles.Stop();
		TowerLight.intensity = Mathf.Lerp(TowerLight.intensity, NormalLightIntensity, Time.deltaTime);
	}
}
