using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PathGridPieceClass
{
	public int ID;

	public int GridID;

	public Vector3 position;

	public int LeftID;

	public int RightID;

	public int UpID;

	public int BottomID;

	public int LeftTopID;

	public int RightTopID;

	public int LeftBottomID;

	public int RightBottomID;

	public PathfindingBlock myPAB;

	public int Score;

	public int AmountCalls;

	public int IDcaller;

	public List<PrevCallers> PrevCalls = new List<PrevCallers>();
}
