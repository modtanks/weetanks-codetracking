using System.Collections;
using UnityEngine;

public class BossPlatform : MonoBehaviour
{
	public int PlatformHealth;

	public int PlatformMaxHealth;

	public GameObject[] PlatformObjects;

	public Material[] MaterialWoodState;

	public KingTankScript KTS;

	public int lastCheckedHealth;

	public Transform[] MissileSpawnPoints;

	public GameObject Missile;

	private Animator ThroneAnimator;

	private void Start()
	{
		ThroneAnimator = GetComponent<Animator>();
		switch (OptionsMainMenu.instance.currentDifficulty)
		{
		case 0:
			PlatformMaxHealth = 2;
			break;
		case 1:
			PlatformMaxHealth = 3;
			break;
		case 2:
			PlatformMaxHealth = 4;
			break;
		case 3:
			PlatformMaxHealth = 5;
			break;
		}
		PlatformMaxHealth = 10;
		PlatformHealth = PlatformMaxHealth;
		StartCoroutine(MissileSpawning());
	}

	private IEnumerator MissileSpawning()
	{
		if (KTS.IsInBattle && !KTS.IsInFinalBattle)
		{
			ThroneAnimator.SetBool("OpenFlaps", value: true);
			yield return new WaitForSeconds(1f);
			int MissilesToSpawn = 1 + OptionsMainMenu.instance.currentDifficulty;
			for (int i = 0; i < MissilesToSpawn; i++)
			{
				SpawnMissiles(i);
				yield return new WaitForSeconds(0.25f);
			}
			yield return new WaitForSeconds(3f);
			ThroneAnimator.SetBool("OpenFlaps", value: false);
			float seconds = Random.Range(4f, 14f);
			yield return new WaitForSeconds(seconds);
		}
		yield return new WaitForSeconds(1f);
		StartCoroutine(MissileSpawning());
	}

	public void SpawnMissiles(int index)
	{
		GameObject obj = Object.Instantiate(Missile, MissileSpawnPoints[index % 2].transform.position, Quaternion.identity, null);
		obj.GetComponent<RocketScript>().MyTeam = 2;
		obj.GetComponent<RocketScript>().isHuntingEnemies = true;
		obj.GetComponent<RocketScript>().Launch();
		obj.GetComponent<RocketScript>().landingOffset = 1f;
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void Update()
	{
		if (lastCheckedHealth == PlatformHealth)
		{
			return;
		}
		if (PlatformHealth < 1)
		{
			GameObject[] platformObjects = PlatformObjects;
			for (int i = 0; i < platformObjects.Length; i++)
			{
				platformObjects[i].SetActive(value: false);
			}
			lastCheckedHealth = PlatformHealth;
			GetComponent<Collider>().enabled = false;
			KTS.FallLayerDown();
			return;
		}
		GetComponent<Collider>().enabled = true;
		lastCheckedHealth = PlatformHealth;
		if (PlatformHealth >= 10)
		{
			GameObject[] platformObjects = PlatformObjects;
			for (int i = 0; i < platformObjects.Length; i++)
			{
				platformObjects[i].SetActive(value: true);
			}
			PlatformObjects[1].SetActive(value: false);
			PlatformObjects[2].SetActive(value: false);
		}
		else if (PlatformHealth >= 7)
		{
			PlatformObjects[0].SetActive(value: true);
			PlatformObjects[1].SetActive(value: false);
			PlatformObjects[2].SetActive(value: false);
		}
		else if (PlatformHealth >= 3)
		{
			PlatformObjects[0].SetActive(value: false);
			PlatformObjects[1].SetActive(value: true);
			PlatformObjects[2].SetActive(value: false);
		}
		else
		{
			PlatformObjects[0].SetActive(value: false);
			PlatformObjects[1].SetActive(value: false);
			PlatformObjects[2].SetActive(value: true);
		}
	}
}
