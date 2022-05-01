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
			Material[] i = ChuteRenderer.materials;
			int number = Random.Range(0, mats.Count);
			i[1] = mats[number];
			ChuteRenderer.materials = i;
			return;
		}
		if (index > 17)
		{
			Material[] k = ChuteRenderer.materials;
			k[1] = mats[index + 1];
			ChuteRenderer.materials = k;
			return;
		}
		Material[] j = ChuteRenderer.materials;
		if (int.TryParse(Regex.Replace(TankToSpawn.name, "[^0-9]", ""), out var number2))
		{
			if (TankToSpawn.name == "Enemy_Tank-12")
			{
				j[0] = mats[13];
			}
			j[1] = mats[number2];
		}
		ChuteRenderer.materials = j;
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
		GameObject SpawnedTank = Object.Instantiate(TankToSpawn, TankSpawnPoint.position, Quaternion.identity);
		HealthTanks SpawnedHT = null;
		EnemyAI EA = null;
		MapEditorProp MEP = null;
		DestroyableWall DW = SpawnedTank.GetComponent<DestroyableWall>();
		if ((bool)DW)
		{
			DW.IsSpawnedIn = true;
		}
		if (SpawnedTank.transform.childCount > 0)
		{
			SpawnedHT = SpawnedTank.transform.GetChild(0).GetComponent<HealthTanks>();
		}
		if (SpawnedTank.transform.childCount > 0)
		{
			EA = SpawnedTank.transform.GetChild(0).GetComponent<EnemyAI>();
			if (EA == null && SpawnedTank.transform.GetChild(0).transform.childCount > 0)
			{
				EA = SpawnedTank.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyAI>();
			}
		}
		if (SpawnedTank.transform.childCount > 0)
		{
			MEP = SpawnedTank.transform.GetChild(0).GetComponent<MapEditorProp>();
			if (MEP == null && SpawnedTank.transform.GetChild(0).transform.childCount > 0)
			{
				MEP = SpawnedTank.transform.GetChild(0).transform.GetChild(0).GetComponent<MapEditorProp>();
			}
		}
		if ((bool)MEP && index > 17)
		{
			Debug.Log("INDEX IS " + index);
			MEP.CustomAInumber = index - 18;
			MEP.CustomUniqueTankID = MapEditorMaster.instance.CustomTankDatas[index - 18].UniqueTankID;
			Debug.Log("CUSTOM ID = " + MapEditorMaster.instance.CustomTankDatas[index - 18].UniqueTankID);
			MEP.SetRendColors();
			MEP.UpdateTankProperties();
		}
		else
		{
			Debug.Log("NO MEP");
		}
		bool IsTank = false;
		if ((bool)SpawnedHT)
		{
			SpawnedHT.IsAirdropped = true;
		}
		if ((bool)EA)
		{
			IsTank = true;
			EA.MyTeam = MyTeam;
		}
		if (!IsTank && index == 16)
		{
			SpawnedTank.transform.position += new Vector3(0f, 0f, 0f);
			DestroyableBox DB = SpawnedTank.GetComponent<DestroyableBox>();
			if ((bool)DB)
			{
				DB.AmountArmourPlates = 1;
			}
		}
		else if (index == 15)
		{
			SpawnedTank.transform.position += new Vector3(0f, 1f, 0f);
		}
		else if (index == 17)
		{
			ExplosiveBlock EB = SpawnedTank.GetComponent<ExplosiveBlock>();
			if ((bool)EB)
			{
				EB.StartFusing();
			}
		}
		if (GameMaster.instance.CurrentMission == 99 && IsTank)
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
		else if (IsTank)
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
