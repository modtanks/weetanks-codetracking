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

	public bool InitiateDestroyTower = false;

	private bool prevent = false;

	private void Start()
	{
		TowerGlowBlock.material.SetVector("_EmissionColor", TowerColor * BlockLightIntensity);
		InvokeRepeating("CheckForPlayers", 0.25f, 0.25f);
	}

	private void CheckForPlayers()
	{
		Collider[] bigobjectsInRange = Physics.OverlapSphere(base.transform.position, TowerRange);
		PlayersInRange.Clear();
		Collider[] array = bigobjectsInRange;
		foreach (Collider col in array)
		{
			if (col.tag == "Player")
			{
				MoveTankScript MTS = col.gameObject.GetComponent<MoveTankScript>();
				if ((bool)MTS && !PlayersInRange.Contains(MTS))
				{
					PlayersInRange.Add(MTS);
				}
			}
		}
	}

	public void DestroyTower()
	{
		GameObject DestructionObject = Object.Instantiate(TowerDestruction, base.transform.position, Quaternion.identity);
		Object.Destroy(DestructionObject, 4f);
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
			float CurrentIntensity2 = TowerGlowBlock.material.GetVector("_EmissionColor")[3];
			if (CurrentIntensity2 < BlockPlayerLightIntensity)
			{
				float newIntensity2 = CurrentIntensity2 + 0.05f;
				TowerGlowBlock.material.SetVector("_EmissionColor", TowerColor * newIntensity2);
			}
			if (!TowerParticles.isPlaying)
			{
				TowerParticles.Play();
			}
			List<MoveTankScript> PlayersToRemove = new List<MoveTankScript>();
			foreach (MoveTankScript Player2 in PlayersInRange)
			{
				if (Player2 != null)
				{
					float DistToPlayer = Vector3.Distance(base.transform.position, Player2.transform.position);
					if (DistToPlayer <= TowerRange + 0.3f)
					{
						Player2.isBeingTowerBoosted = true;
						continue;
					}
					Player2.isBeingTowerBoosted = false;
					PlayersToRemove.Add(Player2);
				}
			}
			if (PlayersToRemove.Count <= 0)
			{
				return;
			}
			{
				foreach (MoveTankScript Player in PlayersToRemove)
				{
					Player.isBeingTowerBoosted = false;
					PlayersInRange.Remove(Player);
				}
				return;
			}
		}
		float CurrentIntensity = TowerGlowBlock.material.GetVector("_EmissionColor")[3];
		if (CurrentIntensity > BlockLightIntensity)
		{
			float newIntensity = CurrentIntensity - 0.05f;
			TowerGlowBlock.material.SetVector("_EmissionColor", TowerColor * newIntensity);
		}
		TowerParticles.Stop();
		TowerLight.intensity = Mathf.Lerp(TowerLight.intensity, NormalLightIntensity, Time.deltaTime);
	}
}
