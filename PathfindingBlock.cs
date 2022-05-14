using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingBlock : MonoBehaviour
{
	[Serializable]
	public class TankInfo
	{
		public GameObject TankObject;

		public int EnemyTeam = -1;

		public int EnemyID = -1;
	}

	public List<GameObject> inMe = new List<GameObject>();

	public MeshRenderer MR;

	public bool SolidInMe;

	public bool SolidInMeIsCork;

	public bool ElectricInMe;

	public int myLatestScore = -1;

	public int inMeWhenCalled = -5;

	public int callerID = -1;

	public int AmountCalls = -1;

	public int myID;

	public int GridID;

	public bool SolidSouthOfMe;

	public bool SouthOnTop;

	public PathGridPieceClass PGPC;

	public List<PrevCallers> MyPreviousCallers = new List<PrevCallers>();

	public List<TankInfo> TanksInMe = new List<TankInfo>();

	private void Start()
	{
		MR = GetComponent<MeshRenderer>();
		InvokeRepeating("CheckPlace", UnityEngine.Random.Range(1f, 2f), UnityEngine.Random.Range(1f, 2f));
	}

	private void CheckPlace()
	{
		if (!base.enabled || !base.gameObject.activeSelf || !base.transform.parent.gameObject.activeSelf)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject item in inMe)
		{
			if (item == null)
			{
				list.Add(item);
			}
			else if (item.transform.tag == "Solid" || item.transform.tag == "ElectricPad")
			{
				SolidInMe = true;
				if (item.gameObject.layer == LayerMask.NameToLayer("CorkWall"))
				{
					SolidInMeIsCork = true;
				}
				else
				{
					SolidInMeIsCork = false;
				}
			}
		}
		foreach (GameObject obj in list)
		{
			TankInfo tankInfo = TanksInMe.Find((TankInfo x) => x.TankObject == obj);
			if (tankInfo != null)
			{
				TanksInMe.Remove(tankInfo);
			}
			inMe.Remove(obj);
		}
		if (inMe.Count < 1)
		{
			SolidInMe = false;
		}
		_ = GameMaster.instance.GameHasStarted;
		if (!GameMaster.instance.AssassinTankAlive)
		{
			return;
		}
		LayerMask layerMask = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall"));
		Debug.DrawRay(base.transform.position, new Vector3(0f, 0f, -1f) * 1.5f, Color.red, 0.1f);
		if (Physics.Raycast(base.transform.position, new Vector3(0f, 0f, -1f), out var hitInfo, 1.5f, layerMask))
		{
			if (hitInfo.transform.tag == "Solid" || hitInfo.transform.tag == "MapBorder")
			{
				SolidSouthOfMe = true;
				if (Physics.Raycast(hitInfo.transform.position, new Vector3(0f, 1f, 0f), out hitInfo, 1.5f, layerMask))
				{
					SouthOnTop = true;
				}
			}
			else
			{
				SolidSouthOfMe = false;
				SouthOnTop = false;
			}
		}
		else
		{
			SolidSouthOfMe = false;
			SouthOnTop = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		OnTrigger(other);
	}

	private void OnTrigger(Collider other)
	{
		if ((other.tag == "Solid" || other.tag == "Other" || other.tag == "Bullet" || other.tag == "Enemy" || other.tag == "Player" || other.tag == "Mine") && !inMe.Contains(other.gameObject))
		{
			inMe.Add(other.gameObject);
			if (other.tag == "Enemy" || other.tag == "Player")
			{
				TankInfo tankInfo = new TankInfo();
				tankInfo.TankObject = other.gameObject;
				tankInfo.EnemyID = other.GetComponent<HealthTanks>().EnemyID;
				MoveTankScript component = other.gameObject.GetComponent<MoveTankScript>();
				if ((bool)component)
				{
					tankInfo.EnemyTeam = component.MyTeam;
				}
				else
				{
					tankInfo.EnemyTeam = other.gameObject.GetComponent<EnemyAI>().MyTeam;
				}
				TanksInMe.Add(tankInfo);
			}
		}
		if ((other.tag == "Solid" || other.tag == "ElectricPad") && !inMe.Contains(other.gameObject))
		{
			SolidInMe = true;
			if (other.gameObject.layer == LayerMask.NameToLayer("CorkWall"))
			{
				SolidInMeIsCork = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.tag == "Solid" || other.tag == "ElectricPad") && inMe.Contains(other.gameObject))
		{
			SolidInMeIsCork = false;
			SolidInMe = false;
		}
		if ((other.tag == "Solid" || other.tag == "Other" || other.tag == "Bullet" || other.tag == "Enemy" || other.tag == "Player" || other.tag == "Mine") && inMe.Contains(other.gameObject))
		{
			TankInfo tankInfo = TanksInMe.Find((TankInfo x) => x.TankObject == other.gameObject);
			if (tankInfo != null)
			{
				TanksInMe.Remove(tankInfo);
			}
			inMe.Remove(other.gameObject);
		}
	}
}
