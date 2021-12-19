using System.Text.RegularExpressions;
using UnityEngine;

public class CarePackageScript : MonoBehaviour
{
	public Transform TankSpawnPoint;

	public GameObject TankToSpawn;

	public AudioClip ChuteOpen;

	public AudioClip Impact;

	public MeshRenderer ChuteRenderer;

	public Material[] mats;

	public int MyTeam = 2;

	public int index = -1;

	private void Start()
	{
		GameMaster.instance.Play2DClipAtPoint(ChuteOpen, 1f);
		if (index == 17 || index == 16 || index == 15)
		{
			Material[] materials = ChuteRenderer.materials;
			int num = Random.Range(0, mats.Length);
			materials[1] = mats[num];
			ChuteRenderer.materials = materials;
			return;
		}
		Material[] materials2 = ChuteRenderer.materials;
		if (int.TryParse(Regex.Replace(TankToSpawn.name, "[^0-9]", ""), out var result))
		{
			if (TankToSpawn.name == "Enemy_Tank-12")
			{
				materials2[0] = mats[13];
			}
			materials2[1] = mats[result];
		}
		ChuteRenderer.materials = materials2;
	}

	private void Update()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			RemoveMe();
		}
	}

	public void SpawnTank()
	{
		if (!GameMaster.instance.GameHasStarted)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate(TankToSpawn, TankSpawnPoint.position, Quaternion.identity);
		HealthTanks healthTanks = null;
		EnemyAI enemyAI = null;
		DestroyableWall component = gameObject.GetComponent<DestroyableWall>();
		if ((bool)component)
		{
			component.IsSpawnedIn = true;
		}
		if (gameObject.transform.childCount > 0)
		{
			healthTanks = gameObject.transform.GetChild(0).GetComponent<HealthTanks>();
		}
		if (gameObject.transform.childCount > 0)
		{
			enemyAI = gameObject.transform.GetChild(0).GetComponent<EnemyAI>();
		}
		bool flag = false;
		if ((bool)healthTanks)
		{
			healthTanks.isSpawnedIn = true;
		}
		if ((bool)enemyAI)
		{
			flag = true;
			enemyAI.MyTeam = MyTeam;
		}
		if (!flag && index == 16)
		{
			gameObject.transform.position += new Vector3(0f, 0f, 0f);
			DestroyableBox component2 = gameObject.GetComponent<DestroyableBox>();
			if ((bool)component2)
			{
				component2.AmountArmourPlates = 1;
			}
		}
		else if (index == 15)
		{
			gameObject.transform.position += new Vector3(0f, 1f, 0f);
		}
		else if (index == 17)
		{
			ExplosiveBlock component3 = gameObject.GetComponent<ExplosiveBlock>();
			if ((bool)component3)
			{
				component3.StartFusing();
			}
		}
		if (GameMaster.instance.CurrentMission == 99 && flag)
		{
			GameMaster.instance.AmountEnemyTanks++;
			GameMaster.instance.AmountTeamTanks[MyTeam]++;
			GameMaster.instance.StartCoroutine(GameMaster.instance.GetTankTeamData(fast: true));
			GameMaster.instance.Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		}
		else if (flag)
		{
			GameMaster.instance.AmountCalledInTanks++;
		}
		GameMaster.instance.Play2DClipAtPoint(Impact, 1f);
	}

	public void RemoveMe()
	{
		Object.Destroy(base.gameObject);
	}
}
