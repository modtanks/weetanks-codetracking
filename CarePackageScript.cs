using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CarePackageScript : MonoBehaviour
{
	public Transform TankSpawnPoint;

	public GameObject TankToSpawn;

	public AudioClip ChuteOpen;

	public AudioClip Impact;

	public MeshRenderer ChuteRenderer;

	public List<Material> mats;

	public int MyTeam = 2;

	public int index = -1;

	private void Start()
	{
		SFXManager.instance.PlaySFX(ChuteOpen, 1f, null);
		if (index == 17 || index == 16 || index == 15)
		{
			Material[] materials = ChuteRenderer.materials;
			int num = Random.Range(0, mats.Count);
			materials[1] = mats[num];
			ChuteRenderer.materials = materials;
			return;
		}
		if (index > 17)
		{
			Material[] materials2 = ChuteRenderer.materials;
			materials2[1] = mats[index + 1];
			ChuteRenderer.materials = materials2;
			return;
		}
		Material[] materials3 = ChuteRenderer.materials;
		if (int.TryParse(Regex.Replace(TankToSpawn.name, "[^0-9]", ""), out var result))
		{
			if (TankToSpawn.name == "Enemy_Tank-12")
			{
				materials3[0] = mats[13];
			}
			materials3[1] = mats[result];
		}
		ChuteRenderer.materials = materials3;
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
		MapEditorProp mapEditorProp = null;
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
			if (enemyAI == null && gameObject.transform.GetChild(0).transform.childCount > 0)
			{
				enemyAI = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyAI>();
			}
		}
		if (gameObject.transform.childCount > 0)
		{
			mapEditorProp = gameObject.transform.GetChild(0).GetComponent<MapEditorProp>();
			if (mapEditorProp == null && gameObject.transform.GetChild(0).transform.childCount > 0)
			{
				mapEditorProp = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<MapEditorProp>();
			}
		}
		if ((bool)mapEditorProp && index > 17)
		{
			Debug.Log("INDEX IS " + index);
			mapEditorProp.CustomAInumber = index - 18;
			mapEditorProp.CustomUniqueTankID = MapEditorMaster.instance.CustomTankDatas[index - 18].UniqueTankID;
			Debug.Log("CUSTOM ID = " + MapEditorMaster.instance.CustomTankDatas[index - 18].UniqueTankID);
			mapEditorProp.SetRendColors();
			mapEditorProp.UpdateTankProperties();
		}
		else
		{
			Debug.Log("NO MEP");
		}
		bool flag = false;
		if ((bool)healthTanks)
		{
			healthTanks.IsAirdropped = true;
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
			if (GameMaster.instance.Enemies != null)
			{
				GameMaster.instance.Enemies.Clear();
			}
			GameMaster.instance.Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
		}
		else if (flag)
		{
			GameMaster.instance.AmountCalledInTanks++;
		}
		SFXManager.instance.PlaySFX(Impact, 1f, null);
	}

	public void RemoveMe()
	{
		Object.Destroy(base.gameObject);
	}
}
