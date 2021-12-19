using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathfindingAI : MonoBehaviour
{
	public PathfindingAI closestPAI;

	public List<PathGridPieceClass> BlockScripts = new List<PathGridPieceClass>();

	public bool canReset;

	public bool callFinished;

	public EnemyAI myAI;

	public List<int> PathIDS = new List<int>();

	public bool SearchForPlayer;

	public int ScanRange = 24;

	private int amountOfScans;

	private int scannedDone = 1;

	private int amountToScan;

	private void Start()
	{
		float repeatRate = Random.Range(2f, 3f);
		InvokeRepeating("SearchAIblock", 0.5f, repeatRate);
		myAI = GetComponent<EnemyAI>();
	}

	private void Update()
	{
	}

	private void AddPGPC(PathfindingBlock PAB, PathGridPieceClass ClassCaller, int hasAlreadyPGPC, bool diagonal, bool overwrite)
	{
		PathGridPieceClass pathGridPieceClass;
		if (hasAlreadyPGPC == 1)
		{
			pathGridPieceClass = PAB.PGPC;
		}
		else
		{
			pathGridPieceClass = new PathGridPieceClass();
			pathGridPieceClass.ID = BlockScripts.Count;
			pathGridPieceClass.GridID = PAB.GridID;
			pathGridPieceClass.myPAB = PAB;
			pathGridPieceClass.position = PAB.transform.position;
			pathGridPieceClass.AmountCalls = amountOfScans;
			pathGridPieceClass.Score = 2;
			if (PAB.ElectricInMe)
			{
				pathGridPieceClass.Score = ((!myAI.isElectric) ? 20 : 0);
			}
			if (myAI.isWallHugger)
			{
				pathGridPieceClass.Score += (PAB.SouthOnTop ? (-1) : 0);
			}
			if (myAI.isAggressive)
			{
				pathGridPieceClass.Score += (PAB.SolidInMeIsCork ? Random.Range(8, 12) : 0);
			}
			BlockScripts.Add(pathGridPieceClass);
		}
		if (ClassCaller != null && pathGridPieceClass.ID != 0)
		{
			PrevCallers prevCallers = new PrevCallers();
			prevCallers.IDcaller = ClassCaller.ID;
			int num = 0;
			num = ((ClassCaller.PrevCalls.Count <= 0) ? ClassCaller.AmountCalls : ClassCaller.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList()[0].AmountPieces);
			prevCallers.AmountPieces = num + 1;
			int num2 = 0;
			int num3 = (diagonal ? 1 : 0);
			if (ClassCaller.PrevCalls.Count > 0)
			{
				ClassCaller.PrevCalls = ClassCaller.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
				num2 = ClassCaller.PrevCalls[0].OurScore;
			}
			else
			{
				num2 = ClassCaller.Score;
			}
			prevCallers.OurScore = pathGridPieceClass.Score + num2 + num3;
			if (pathGridPieceClass.PrevCalls.Count > 0)
			{
				pathGridPieceClass.PrevCalls = pathGridPieceClass.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
				if (prevCallers.OurScore < pathGridPieceClass.PrevCalls[0].OurScore)
				{
					pathGridPieceClass.PrevCalls.Add(prevCallers);
					pathGridPieceClass.PrevCalls = pathGridPieceClass.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
					CheckAllDirections(pathGridPieceClass, IsOverwrite: true);
				}
				else if (!overwrite && pathGridPieceClass.ID != 0)
				{
					pathGridPieceClass.PrevCalls.Add(prevCallers);
					pathGridPieceClass.PrevCalls = pathGridPieceClass.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
				}
			}
			else if (!overwrite)
			{
				pathGridPieceClass.PrevCalls.Add(prevCallers);
			}
			PAB.MyPreviousCallers.Add(prevCallers);
		}
		SetPABcolor(PAB, pathGridPieceClass.Score);
		PAB.PGPC = pathGridPieceClass;
	}

	public void SearchAIblock()
	{
		if (((myAI.isPathfinding || myAI.hasGottenPath) && !SearchForPlayer) || (SearchForPlayer && myAI.GoingToPlayer) || myAI.CanGoStraightToPlayer || !myAI.CanMove)
		{
			return;
		}
		if (!GameMaster.instance.GameHasStarted)
		{
			BlockScripts.Clear();
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 1f, 0f), 0.2f);
		foreach (Collider collider in array)
		{
			if (!(collider.tag == "TeleportBlock"))
			{
				continue;
			}
			PathfindingBlock component = collider.GetComponent<PathfindingBlock>();
			if (!component)
			{
				continue;
			}
			Debug.Log("start the search!");
			BlockScripts.Clear();
			AddPGPC(component, null, 2, diagonal: false, overwrite: false);
			amountOfScans = 0;
			scannedDone = 1;
			PathIDS.Clear();
			amountToScan = CheckAllDirections(BlockScripts.Find((PathGridPieceClass x) => x.ID == 0), IsOverwrite: false) + scannedDone;
			amountOfScans++;
			while (scannedDone != amountToScan && amountOfScans < ScanRange)
			{
				int num = 0;
				int i;
				for (i = scannedDone; i < amountToScan; i++)
				{
					num += CheckAllDirections(BlockScripts.Find((PathGridPieceClass x) => x.ID == i), IsOverwrite: false);
					scannedDone++;
				}
				amountToScan += num;
				amountOfScans++;
			}
			BlockScripts = BlockScripts.OrderBy((PathGridPieceClass w) => w.Score).ToList();
			PathGridPieceClass pathGridPieceClass = null;
			List<PathGridPieceClass> blockScripts = BlockScripts;
			int num2 = 0;
			bool flag = false;
			do
			{
				if (blockScripts.Count < 1)
				{
					myAI.isPathfinding = false;
					myAI.hasGottenPath = false;
					return;
				}
				if (pathGridPieceClass != null)
				{
					blockScripts.Remove(pathGridPieceClass);
				}
				if (SearchForPlayer)
				{
					if (!myAI.DownedPlayer)
					{
						myAI.isPathfinding = false;
						myAI.hasGottenPath = false;
						SearchForPlayer = false;
						return;
					}
					pathGridPieceClass = blockScripts.Find((PathGridPieceClass x) => x.myPAB.inMe.Contains(myAI.DownedPlayer));
					myAI.GoingToPlayer = true;
					flag = true;
				}
				else if (myAI.isWallHugger)
				{
					Debug.Log("Wall hugger searching" + blockScripts.Count);
					if (blockScripts.Count < 1)
					{
						return;
					}
					pathGridPieceClass = blockScripts.Find((PathGridPieceClass x) => x.myPAB.SolidSouthOfMe && !x.myPAB.inMe.Contains(myAI.gameObject));
					Debug.Log("Wall hugger found:" + pathGridPieceClass);
					flag = true;
				}
				else if (myAI.isAggressive && (bool)myAI.ETSN)
				{
					if (!(myAI.ETSN.currentTarget != null))
					{
						myAI.isPathfinding = false;
						myAI.hasGottenPath = false;
						return;
					}
					pathGridPieceClass = blockScripts.Find((PathGridPieceClass x) => x.myPAB.inMe.Contains(myAI.ETSN.currentTarget));
					flag = true;
				}
				else if (myAI.ETSN == null)
				{
					return;
				}
				if (pathGridPieceClass == null)
				{
					flag = false;
				}
				else if (pathGridPieceClass != null && (float)pathGridPieceClass.Score < 1f)
				{
					flag = false;
				}
				num2++;
				if (num2 > 49)
				{
					myAI.isPathfinding = false;
					myAI.hasGottenPath = false;
					return;
				}
			}
			while (!flag);
			Debug.Log("Found one");
			pathGridPieceClass.myPAB.MR.material.color = Color.blue;
			PathIDS.Add(pathGridPieceClass.ID);
			pathGridPieceClass.PrevCalls = pathGridPieceClass.PrevCalls.OrderBy((PrevCallers w) => w.OurScore).ToList();
			int prevCallerID = pathGridPieceClass.PrevCalls[0].IDcaller;
			Debug.Log(pathGridPieceClass.PrevCalls[0].AmountPieces);
			for (int k = 0; k < pathGridPieceClass.PrevCalls[0].AmountPieces; k++)
			{
				PathGridPieceClass pathGridPieceClass2 = BlockScripts.Find((PathGridPieceClass x) => x.ID == prevCallerID);
				if (pathGridPieceClass2 != null)
				{
					_ = myAI.isWallHugger;
					PathIDS.Add(pathGridPieceClass2.ID);
					pathGridPieceClass2.PrevCalls = pathGridPieceClass2.PrevCalls.OrderBy((PrevCallers w) => w.OurScore).ToList();
					if (pathGridPieceClass2.PrevCalls.Count <= 0)
					{
						break;
					}
					prevCallerID = pathGridPieceClass2.PrevCalls[0].IDcaller;
					if (prevCallerID == 0)
					{
						break;
					}
					pathGridPieceClass2.myPAB.MR.material.color = Color.blue;
				}
			}
			Debug.Log("Lets go!");
			PathfindingBlock myPAB = BlockScripts.Find((PathGridPieceClass x) => x.ID == PathIDS[PathIDS.Count - 1]).myPAB;
			if ((bool)myPAB)
			{
				Color green = Color.green;
				myPAB.MR.material.color = green;
				myAI.hasGottenPath = true;
				myAI.isPathfinding = true;
				myAI.preferredLocation = myPAB.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
				Debug.Log("MOVEEE");
				if (myAI.isWallHugger)
				{
					myAI.TankSpeed = myAI.OriginalTankSpeed;
				}
			}
			break;
		}
	}

	public void NextPoint()
	{
		if (PathIDS.Count < 1)
		{
			return;
		}
		PathfindingBlock myPAB = BlockScripts.Find((PathGridPieceClass x) => x.ID == PathIDS[PathIDS.Count - 1]).myPAB;
		if ((bool)myPAB)
		{
			Color clear = Color.clear;
			clear.a = 0f;
			myPAB.MR.material.color = clear;
		}
		PathIDS.RemoveAt(PathIDS.Count - 1);
		if (PathIDS.Count < 1)
		{
			if (!myAI.isWallHugger)
			{
				myAI.isPathfinding = false;
				myAI.hasGottenPath = false;
			}
			else
			{
				myAI.isPathfinding = false;
				myAI.hasGottenPath = false;
				myAI.TankSpeed = 0f;
			}
			SearchAIblock();
			return;
		}
		PathfindingBlock myPAB2 = BlockScripts.Find((PathGridPieceClass x) => x.ID == PathIDS[PathIDS.Count - 1]).myPAB;
		if ((bool)myPAB2)
		{
			Color green = Color.green;
			myPAB2.MR.material.color = green;
			myAI.hasGottenPath = true;
			myAI.preferredLocation = myPAB2.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
			myAI.SetDestination(myAI.preferredLocation);
		}
	}

	private void SetPABcolor(PathfindingBlock PAB, int score)
	{
		Color red = Color.red;
		red.a = (float)score / 10f;
		if (red.a < 0f)
		{
			red.a = 0f;
		}
		if (red.a > 1f)
		{
			red.a = 1f;
		}
		PAB.MR.material.color = red;
	}

	private int CheckPrevCalls(PathfindingBlock BlockOnSide, PathGridPieceClass thisPGPC)
	{
		if (BlockOnSide == null)
		{
			return 0;
		}
		PathGridPieceClass pathGridPieceClass = BlockScripts.Find((PathGridPieceClass x) => x.myPAB == BlockOnSide);
		int num = 0;
		if (pathGridPieceClass != null)
		{
			return 1;
		}
		return 2;
	}

	private int CheckAllDirections(PathGridPieceClass PGPC, bool IsOverwrite)
	{
		int num = 0;
		PathfindingBlock pathfindingBlock = CheckAdjacent(PGPC, 0);
		int num2 = CheckPrevCalls(pathfindingBlock, PGPC);
		if (pathfindingBlock != null && num2 > 0)
		{
			num2 = (IsOverwrite ? 1 : num2);
			AddPGPC(pathfindingBlock, PGPC, num2, diagonal: false, IsOverwrite);
			PGPC.LeftID = BlockScripts.Count;
			if (num2 == 2)
			{
				num++;
			}
		}
		else
		{
			PGPC.LeftID = -1;
		}
		PathfindingBlock pathfindingBlock2 = CheckAdjacent(PGPC, 2);
		num2 = CheckPrevCalls(pathfindingBlock2, PGPC);
		if (pathfindingBlock2 != null && num2 > 0)
		{
			num2 = (IsOverwrite ? 1 : num2);
			AddPGPC(pathfindingBlock2, PGPC, num2, diagonal: false, IsOverwrite);
			PGPC.UpID = BlockScripts.Count;
			if (num2 == 2)
			{
				num++;
			}
		}
		else
		{
			PGPC.UpID = -1;
		}
		PathfindingBlock pathfindingBlock3 = CheckAdjacent(PGPC, 4);
		num2 = CheckPrevCalls(pathfindingBlock3, PGPC);
		if (pathfindingBlock3 != null && num2 > 0)
		{
			num2 = (IsOverwrite ? 1 : num2);
			AddPGPC(pathfindingBlock3, PGPC, num2, diagonal: false, IsOverwrite);
			PGPC.RightID = BlockScripts.Count;
			if (num2 == 2)
			{
				num++;
			}
		}
		else
		{
			PGPC.RightID = -1;
		}
		PathfindingBlock pathfindingBlock4 = CheckAdjacent(PGPC, 6);
		num2 = CheckPrevCalls(pathfindingBlock4, PGPC);
		if (pathfindingBlock4 != null && num2 > 0)
		{
			num2 = (IsOverwrite ? 1 : num2);
			AddPGPC(pathfindingBlock4, PGPC, num2, diagonal: false, IsOverwrite);
			PGPC.BottomID = BlockScripts.Count;
			if (num2 == 2)
			{
				num++;
			}
		}
		else
		{
			PGPC.BottomID = -1;
		}
		PathfindingBlock pathfindingBlock5 = CheckAdjacent(PGPC, 1);
		num2 = CheckPrevCalls(pathfindingBlock5, PGPC);
		if (pathfindingBlock5 != null && num2 > 0)
		{
			bool flag = false;
			if (CheckForSolid(PGPC, 0))
			{
				flag = true;
			}
			if (CheckForSolid(PGPC, 2))
			{
				flag = true;
			}
			if (!flag)
			{
				num2 = (IsOverwrite ? 1 : num2);
				AddPGPC(pathfindingBlock5, PGPC, num2, diagonal: true, IsOverwrite);
				PGPC.LeftTopID = BlockScripts.Count;
				if (num2 == 2)
				{
					num++;
				}
			}
			else
			{
				PGPC.LeftTopID = -1;
			}
		}
		else
		{
			PGPC.LeftTopID = -1;
		}
		PathfindingBlock pathfindingBlock6 = CheckAdjacent(PGPC, 3);
		num2 = CheckPrevCalls(pathfindingBlock6, PGPC);
		if (pathfindingBlock6 != null && num2 > 0)
		{
			bool flag2 = false;
			if (CheckForSolid(PGPC, 4))
			{
				flag2 = true;
			}
			if (CheckForSolid(PGPC, 2))
			{
				flag2 = true;
			}
			if (!flag2)
			{
				num2 = (IsOverwrite ? 1 : num2);
				AddPGPC(pathfindingBlock6, PGPC, num2, diagonal: true, IsOverwrite);
				PGPC.RightTopID = BlockScripts.Count;
				if (num2 == 2)
				{
					num++;
				}
			}
			else
			{
				PGPC.RightTopID = -1;
			}
		}
		else
		{
			PGPC.RightTopID = -1;
		}
		PathfindingBlock pathfindingBlock7 = CheckAdjacent(PGPC, 5);
		num2 = CheckPrevCalls(pathfindingBlock7, PGPC);
		if (pathfindingBlock7 != null && num2 > 0)
		{
			bool flag3 = false;
			if (CheckForSolid(PGPC, 4))
			{
				flag3 = true;
			}
			if (CheckForSolid(PGPC, 6))
			{
				flag3 = true;
			}
			if (!flag3)
			{
				num2 = (IsOverwrite ? 1 : num2);
				AddPGPC(pathfindingBlock7, PGPC, num2, diagonal: true, IsOverwrite);
				PGPC.RightBottomID = BlockScripts.Count;
				if (num2 == 2)
				{
					num++;
				}
			}
			else
			{
				PGPC.RightBottomID = -1;
			}
		}
		else
		{
			PGPC.RightBottomID = -1;
		}
		PathfindingBlock pathfindingBlock8 = CheckAdjacent(PGPC, 7);
		num2 = CheckPrevCalls(pathfindingBlock8, PGPC);
		if (pathfindingBlock8 != null && num2 > 0)
		{
			bool flag4 = false;
			if (CheckForSolid(PGPC, 0))
			{
				flag4 = true;
			}
			if (CheckForSolid(PGPC, 6))
			{
				flag4 = true;
			}
			if (!flag4)
			{
				num2 = (IsOverwrite ? 1 : num2);
				AddPGPC(pathfindingBlock8, PGPC, num2, diagonal: true, IsOverwrite);
				PGPC.LeftBottomID = BlockScripts.Count;
				if (num2 == 2)
				{
					num++;
				}
			}
			else
			{
				PGPC.LeftBottomID = -1;
			}
		}
		else
		{
			PGPC.LeftBottomID = -1;
		}
		return num;
	}

	private PathfindingBlock CheckAdjacent(PathGridPieceClass caller, int direction)
	{
		PathfindingBlock pathfindingBlock = PathfindingBlocksMaster.instance.FindBlock(caller.GridID, direction);
		if ((bool)pathfindingBlock && !pathfindingBlock.SolidInMe)
		{
			return pathfindingBlock;
		}
		if ((bool)pathfindingBlock && pathfindingBlock.SolidInMe && pathfindingBlock.SolidInMeIsCork && myAI.LayMines && !SearchForPlayer)
		{
			return pathfindingBlock;
		}
		return null;
	}

	private bool CheckForSolid(PathGridPieceClass caller, int direction)
	{
		PathfindingBlock pathfindingBlock = PathfindingBlocksMaster.instance.FindBlock(caller.GridID, direction);
		if ((bool)pathfindingBlock && pathfindingBlock.SolidInMe)
		{
			return true;
		}
		return false;
	}
}
