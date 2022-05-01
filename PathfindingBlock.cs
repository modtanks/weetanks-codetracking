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

	public bool SolidInMeIsCork = false;

	public bool ElectricInMe = false;

	public int myLatestScore = -1;

	public int inMeWhenCalled = -5;

	public int callerID = -1;

	public int AmountCalls = -1;

	public int myID = 0;

	public int GridID = 0;

	public bool SolidSouthOfMe = false;

	public bool SouthOnTop = false;

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
		List<GameObject> ToRemove = new List<GameObject>();
		foreach (GameObject inObj in inMe)
		{
			if (inObj == null)
			{
				ToRemove.Add(inObj);
			}
			else if (inObj.transform.tag == "Solid" || inObj.transform.tag == "ElectricPad")
			{
				SolidInMe = true;
				if (inObj.gameObject.layer == LayerMask.NameToLayer("CorkWall"))
				{
					SolidInMeIsCork = true;
				}
				else
				{
					SolidInMeIsCork = false;
				}
			}
		}
		foreach (GameObject obj in ToRemove)
		{
			TankInfo Tank = TanksInMe.Find((TankInfo x) => x.TankObject == obj);
			if (Tank != null)
			{
				TanksInMe.Remove(Tank);
			}
			inMe.Remove(obj);
		}
		if (inMe.Count < 1)
		{
			SolidInMe = false;
		}
		if (!GameMaster.instance.GameHasStarted)
		{
		}
		if (!GameMaster.instance.AssassinTankAlive)
		{
			return;
		}
		LayerMask LM = (1 << LayerMask.NameToLayer("CorkWall")) | (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("NoBounceWall"));
		Debug.DrawRay(base.transform.position, new Vector3(0f, 0f, -1f) * 1.5f, Color.red, 0.1f);
		if (Physics.Raycast(base.transform.position, new Vector3(0f, 0f, -1f), out var hit, 1.5f, LM))
		{
			if (hit.transform.tag == "Solid" || hit.transform.tag == "MapBorder")
			{
				SolidSouthOfMe = true;
				if (Physics.Raycast(hit.transform.position, new Vector3(0f, 1f, 0f), out hit, 1.5f, LM))
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
				TankInfo NewTank = new TankInfo();
				NewTank.TankObject = other.gameObject;
				NewTank.EnemyID = other.GetComponent<HealthTanks>().EnemyID;
				MoveTankScript MTS = other.gameObject.GetComponent<MoveTankScript>();
				if ((bool)MTS)
				{
					NewTank.EnemyTeam = MTS.MyTeam;
				}
				else
				{
					NewTank.EnemyTeam = other.gameObject.GetComponent<EnemyAI>().MyTeam;
				}
				TanksInMe.Add(NewTank);
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
			TankInfo Tank = TanksInMe.Find((TankInfo x) => x.TankObject == other.gameObject);
			if (Tank != null)
			{
				TanksInMe.Remove(Tank);
			}
			inMe.Remove(other.gameObject);
		}
	}
}
