using UnityEngine;

public class BossPlatform : MonoBehaviour
{
	public int PlatformHealth;

	public int PlatformMaxHealth;

	public GameObject[] PlatformObjects;

	public Material[] MaterialWoodState;

	public int PlatformLayer;

	public KingTankScript KTS;

	public int lastCheckedHealth;

	private void Start()
	{
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
		PlatformHealth = PlatformMaxHealth;
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
			if (KTS.CurrentLayer == 2 && KTS.MHC.BossPhase < 3 && PlatformLayer == 2)
			{
				KTS.FallLayerDown();
			}
			else if (KTS.CurrentLayer == 1 && KTS.MHC.BossPhase >= 3 && PlatformLayer == 1)
			{
				KTS.FallLayerDown();
			}
		}
		else
		{
			GetComponent<Collider>().enabled = true;
			lastCheckedHealth = PlatformHealth;
			GameObject[] platformObjects = PlatformObjects;
			for (int i = 0; i < platformObjects.Length; i++)
			{
				platformObjects[i].SetActive(value: true);
			}
			platformObjects = PlatformObjects;
			for (int i = 0; i < platformObjects.Length; i++)
			{
				platformObjects[i].GetComponent<MeshRenderer>().material = MaterialWoodState[PlatformHealth];
			}
		}
	}
}
