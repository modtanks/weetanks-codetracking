using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathfindingAI : MonoBehaviour
{
	public PathfindingAI closestPAI;

	public List<PathGridPieceClass> BlockScripts = new List<PathGridPieceClass>();

	public bool canReset = false;

	public bool callFinished = false;

	public EnemyAI myAI;

	public List<int> PathIDS = new List<int>();

	public bool SearchForPlayer = false;

	public int ScanRange = 24;

	private int amountOfScans = 0;

	private int scannedDone = 1;

	private int amountToScan = 0;

	private void Start()
	{
		float RandomStartTimer = Random.Range(2f, 3f);
		InvokeRepeating("SearchAIblock", 0.5f, RandomStartTimer);
		myAI = GetComponent<EnemyAI>();
	}

	private void Update()
	{
	}

	private void AddPGPC(PathfindingBlock PAB, PathGridPieceClass ClassCaller, int hasAlreadyPGPC, bool diagonal, bool overwrite)
	{
		PathGridPieceClass PGPC;
		if (hasAlreadyPGPC == 1)
		{
			PGPC = PAB.PGPC;
		}
		else
		{
			PGPC = new PathGridPieceClass();
			PGPC.ID = BlockScripts.Count;
			PGPC.GridID = PAB.GridID;
			PGPC.myPAB = PAB;
			PGPC.position = PAB.transform.position;
			PGPC.AmountCalls = amountOfScans;
			PGPC.Score = 2;
			if (PAB.ElectricInMe)
			{
				PGPC.Score = ((!myAI.isElectric) ? 20 : 0);
			}
			if (myAI.isWallHugger)
			{
				PGPC.Score += (PAB.SouthOnTop ? (-1) : 0);
			}
			if (myAI.isAggressive)
			{
				PGPC.Score += (PAB.SolidInMeIsCork ? Random.Range(8, 12) : 0);
			}
			BlockScripts.Add(PGPC);
		}
		if (ClassCaller != null && PGPC.ID != 0)
		{
			PrevCallers newCallClass = new PrevCallers();
			newCallClass.IDcaller = ClassCaller.ID;
			int ClassCallerCalls = 0;
			ClassCallerCalls = ((ClassCaller.PrevCalls.Count <= 0) ? ClassCaller.AmountCalls : ClassCaller.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList()[0].AmountPieces);
			newCallClass.AmountPieces = ClassCallerCalls + 1;
			int lowestScore = 0;
			int AdditionalScore = (diagonal ? 1 : 0);
			if (ClassCaller.PrevCalls.Count > 0)
			{
				ClassCaller.PrevCalls = ClassCaller.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
				lowestScore = ClassCaller.PrevCalls[0].OurScore;
			}
			else
			{
				lowestScore = ClassCaller.Score;
			}
			newCallClass.OurScore = PGPC.Score + lowestScore + AdditionalScore;
			if (PGPC.PrevCalls.Count > 0)
			{
				PGPC.PrevCalls = PGPC.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
				if (newCallClass.OurScore < PGPC.PrevCalls[0].OurScore)
				{
					PGPC.PrevCalls.Add(newCallClass);
					PGPC.PrevCalls = PGPC.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
					CheckAllDirections(PGPC, IsOverwrite: true);
				}
				else if (!overwrite && PGPC.ID != 0)
				{
					PGPC.PrevCalls.Add(newCallClass);
					PGPC.PrevCalls = PGPC.PrevCalls.OrderBy((PrevCallers x) => x.OurScore).ToList();
				}
			}
			else if (!overwrite)
			{
				PGPC.PrevCalls.Add(newCallClass);
			}
			PAB.MyPreviousCallers.Add(newCallClass);
		}
		SetPABcolor(PAB, PGPC.Score);
		PAB.PGPC = PGPC;
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
		Collider[] Hits = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 1f, 0f), 0.2f);
		Collider[] array = Hits;
		foreach (Collider coll in array)
		{
			if (!(coll.tag == "TeleportBlock"))
			{
				continue;
			}
			PathfindingBlock PAB = coll.GetComponent<PathfindingBlock>();
			if (!PAB)
			{
				continue;
			}
			BlockScripts.Clear();
			AddPGPC(PAB, null, 2, diagonal: false, overwrite: false);
			amountOfScans = 0;
			scannedDone = 1;
			PathIDS.Clear();
			amountToScan = CheckAllDirections(BlockScripts.Find((PathGridPieceClass x) => x.ID == 0), IsOverwrite: false) + scannedDone;
			amountOfScans++;
			while (scannedDone != amountToScan && amountOfScans < ScanRange)
			{
				int toadd = 0;
				int i;
				for (i = scannedDone; i < amountToScan; i++)
				{
					toadd += CheckAllDirections(BlockScripts.Find((PathGridPieceClass x) => x.ID == i), IsOverwrite: false);
					scannedDone++;
				}
				amountToScan += toadd;
				amountOfScans++;
			}
			BlockScripts = BlockScripts.OrderBy((PathGridPieceClass w) => w.Score).ToList();
			PathGridPieceClass thePGPC = null;
			List<PathGridPieceClass> TempList = BlockScripts;
			int tries = 0;
			bool canContinue = false;
			do
			{
				if (TempList.Count < 1)
				{
					myAI.isPathfinding = false;
					myAI.hasGottenPath = false;
					return;
				}
				if (thePGPC != null)
				{
					TempList.Remove(thePGPC);
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
					thePGPC = TempList.Find((PathGridPieceClass x) => x.myPAB.inMe.Contains(myAI.DownedPlayer));
					myAI.GoingToPlayer = true;
					canContinue = true;
				}
				else if (myAI.isWallHugger)
				{
					if (TempList.Count < 1)
					{
						return;
					}
					thePGPC = TempList.Find((PathGridPieceClass x) => x.myPAB.SolidSouthOfMe && !x.myPAB.inMe.Contains(myAI.gameObject));
					canContinue = true;
				}
				else if (myAI.StaysCloseToAllies)
				{
					if (TempList.Count < 1)
					{
						return;
					}
					thePGPC = TempList.Find((PathGridPieceClass x) => x.myPAB.TanksInMe.Find((PathfindingBlock.TankInfo y) => y.EnemyTeam == myAI.MyTeam && y.TankObject != base.gameObject) != null);
					canContinue = true;
				}
				else if (myAI.isAggressive && (bool)myAI.ETSN)
				{
					if (!(myAI.ETSN.currentTarget != null))
					{
						myAI.isPathfinding = false;
						myAI.hasGottenPath = false;
						return;
					}
					thePGPC = TempList.Find((PathGridPieceClass x) => x.myPAB.inMe.Contains(myAI.ETSN.currentTarget));
					canContinue = true;
				}
				else if (myAI.ETSN == null)
				{
					return;
				}
				if (thePGPC == null)
				{
					canContinue = false;
					myAI.isPathfinding = false;
					myAI.hasGottenPath = false;
					return;
				}
				if (thePGPC != null && (float)thePGPC.Score < 1f)
				{
					canContinue = false;
				}
			}
			while (!canContinue);
			thePGPC.myPAB.MR.material.color = Color.blue;
			PathIDS.Add(thePGPC.ID);
			thePGPC.PrevCalls = thePGPC.PrevCalls.OrderBy((PrevCallers w) => w.OurScore).ToList();
			if (thePGPC.PrevCalls.Count < 1)
			{
				myAI.isPathfinding = false;
				myAI.hasGottenPath = false;
				break;
			}
			int prevCallerID = thePGPC.PrevCalls[0].IDcaller;
			for (int j = 0; j < thePGPC.PrevCalls[0].AmountPieces; j++)
			{
				PathGridPieceClass PrevClass = BlockScripts.Find((PathGridPieceClass x) => x.ID == prevCallerID);
				if (PrevClass != null)
				{
					if (myAI.isWallHugger)
					{
					}
					PathIDS.Add(PrevClass.ID);
					PrevClass.PrevCalls = PrevClass.PrevCalls.OrderBy((PrevCallers w) => w.OurScore).ToList();
					if (PrevClass.PrevCalls.Count <= 0)
					{
						break;
					}
					prevCallerID = PrevClass.PrevCalls[0].IDcaller;
					if (prevCallerID == 0)
					{
						break;
					}
					PrevClass.myPAB.MR.material.color = Color.blue;
				}
			}
			PathfindingBlock closestBlock = BlockScripts.Find((PathGridPieceClass x) => x.ID == PathIDS[PathIDS.Count - 1]).myPAB;
			if ((bool)closestBlock)
			{
				Color clr = Color.green;
				closestBlock.MR.material.color = clr;
				myAI.hasGottenPath = true;
				myAI.isPathfinding = true;
				myAI.preferredLocation = closestBlock.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
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
		PathfindingBlock prevBlock = BlockScripts.Find((PathGridPieceClass x) => x.ID == PathIDS[PathIDS.Count - 1]).myPAB;
		if ((bool)prevBlock)
		{
			Color clr2 = Color.clear;
			clr2.a = 0f;
			prevBlock.MR.material.color = clr2;
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
		PathfindingBlock closestBlock = BlockScripts.Find((PathGridPieceClass x) => x.ID == PathIDS[PathIDS.Count - 1]).myPAB;
		if ((bool)closestBlock)
		{
			Color clr = Color.green;
			closestBlock.MR.material.color = clr;
			myAI.hasGottenPath = true;
			myAI.preferredLocation = closestBlock.transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
			myAI.SetDestination(myAI.preferredLocation);
		}
	}

	private void SetPABcolor(PathfindingBlock PAB, int score)
	{
	}

	private int CheckPrevCalls(PathfindingBlock BlockOnSide, PathGridPieceClass thisPGPC)
	{
		if (BlockOnSide == null)
		{
			return 0;
		}
		PathGridPieceClass Test = BlockScripts.Find((PathGridPieceClass x) => x.myPAB == BlockOnSide);
		int canCheck = 0;
		return (Test != null) ? 1 : 2;
	}

	private int CheckAllDirections(PathGridPieceClass PGPC, bool IsOverwrite)
	{
		int AmountToAddToTotal = 0;
		PathfindingBlock Left = CheckAdjacent(PGPC, 0);
		int canCheck = CheckPrevCalls(Left, PGPC);
		if (Left != null && canCheck > 0)
		{
			canCheck = (IsOverwrite ? 1 : canCheck);
			AddPGPC(Left, PGPC, canCheck, diagonal: false, IsOverwrite);
			PGPC.LeftID = BlockScripts.Count;
			if (canCheck == 2)
			{
				AmountToAddToTotal++;
			}
		}
		else
		{
			PGPC.LeftID = -1;
		}
		PathfindingBlock Up = CheckAdjacent(PGPC, 2);
		canCheck = CheckPrevCalls(Up, PGPC);
		if (Up != null && canCheck > 0)
		{
			canCheck = (IsOverwrite ? 1 : canCheck);
			AddPGPC(Up, PGPC, canCheck, diagonal: false, IsOverwrite);
			PGPC.UpID = BlockScripts.Count;
			if (canCheck == 2)
			{
				AmountToAddToTotal++;
			}
		}
		else
		{
			PGPC.UpID = -1;
		}
		PathfindingBlock Right = CheckAdjacent(PGPC, 4);
		canCheck = CheckPrevCalls(Right, PGPC);
		if (Right != null && canCheck > 0)
		{
			canCheck = (IsOverwrite ? 1 : canCheck);
			AddPGPC(Right, PGPC, canCheck, diagonal: false, IsOverwrite);
			PGPC.RightID = BlockScripts.Count;
			if (canCheck == 2)
			{
				AmountToAddToTotal++;
			}
		}
		else
		{
			PGPC.RightID = -1;
		}
		PathfindingBlock Down = CheckAdjacent(PGPC, 6);
		canCheck = CheckPrevCalls(Down, PGPC);
		if (Down != null && canCheck > 0)
		{
			canCheck = (IsOverwrite ? 1 : canCheck);
			AddPGPC(Down, PGPC, canCheck, diagonal: false, IsOverwrite);
			PGPC.BottomID = BlockScripts.Count;
			if (canCheck == 2)
			{
				AmountToAddToTotal++;
			}
		}
		else
		{
			PGPC.BottomID = -1;
		}
		PathfindingBlock LeftTop = CheckAdjacent(PGPC, 1);
		canCheck = CheckPrevCalls(LeftTop, PGPC);
		if (LeftTop != null && canCheck > 0)
		{
			bool skip4 = false;
			if (CheckForSolid(PGPC, 0))
			{
				skip4 = true;
			}
			if (CheckForSolid(PGPC, 2))
			{
				skip4 = true;
			}
			if (!skip4)
			{
				canCheck = (IsOverwrite ? 1 : canCheck);
				AddPGPC(LeftTop, PGPC, canCheck, diagonal: true, IsOverwrite);
				PGPC.LeftTopID = BlockScripts.Count;
				if (canCheck == 2)
				{
					AmountToAddToTotal++;
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
		PathfindingBlock RightTop = CheckAdjacent(PGPC, 3);
		canCheck = CheckPrevCalls(RightTop, PGPC);
		if (RightTop != null && canCheck > 0)
		{
			bool skip3 = false;
			if (CheckForSolid(PGPC, 4))
			{
				skip3 = true;
			}
			if (CheckForSolid(PGPC, 2))
			{
				skip3 = true;
			}
			if (!skip3)
			{
				canCheck = (IsOverwrite ? 1 : canCheck);
				AddPGPC(RightTop, PGPC, canCheck, diagonal: true, IsOverwrite);
				PGPC.RightTopID = BlockScripts.Count;
				if (canCheck == 2)
				{
					AmountToAddToTotal++;
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
		PathfindingBlock RightBottom = CheckAdjacent(PGPC, 5);
		canCheck = CheckPrevCalls(RightBottom, PGPC);
		if (RightBottom != null && canCheck > 0)
		{
			bool skip2 = false;
			if (CheckForSolid(PGPC, 4))
			{
				skip2 = true;
			}
			if (CheckForSolid(PGPC, 6))
			{
				skip2 = true;
			}
			if (!skip2)
			{
				canCheck = (IsOverwrite ? 1 : canCheck);
				AddPGPC(RightBottom, PGPC, canCheck, diagonal: true, IsOverwrite);
				PGPC.RightBottomID = BlockScripts.Count;
				if (canCheck == 2)
				{
					AmountToAddToTotal++;
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
		PathfindingBlock LeftBottom = CheckAdjacent(PGPC, 7);
		canCheck = CheckPrevCalls(LeftBottom, PGPC);
		if (LeftBottom != null && canCheck > 0)
		{
			bool skip = false;
			if (CheckForSolid(PGPC, 0))
			{
				skip = true;
			}
			if (CheckForSolid(PGPC, 6))
			{
				skip = true;
			}
			if (!skip)
			{
				canCheck = (IsOverwrite ? 1 : canCheck);
				AddPGPC(LeftBottom, PGPC, canCheck, diagonal: true, IsOverwrite);
				PGPC.LeftBottomID = BlockScripts.Count;
				if (canCheck == 2)
				{
					AmountToAddToTotal++;
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
		return AmountToAddToTotal;
	}

	private PathfindingBlock CheckAdjacent(PathGridPieceClass caller, int direction)
	{
		PathfindingBlock other = PathfindingBlocksMaster.instance.FindBlock(caller.GridID, direction);
		if ((bool)other && !other.SolidInMe)
		{
			return other;
		}
		if ((bool)other && other.SolidInMe && other.SolidInMeIsCork && myAI.LayMines && !SearchForPlayer)
		{
			return other;
		}
		return null;
	}

	private bool CheckForSolid(PathGridPieceClass caller, int direction)
	{
		PathfindingBlock other = PathfindingBlocksMaster.instance.FindBlock(caller.GridID, direction);
		if ((bool)other && other.SolidInMe)
		{
			return true;
		}
		return false;
	}
}
