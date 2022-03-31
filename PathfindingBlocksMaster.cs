using System.Collections.Generic;
using UnityEngine;

public class PathfindingBlocksMaster : MonoBehaviour
{
	public List<PathfindingBlock> AllBlocks = new List<PathfindingBlock>();

	public int MapSizeX = 0;

	private static PathfindingBlocksMaster _instance;

	public static PathfindingBlocksMaster instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	public void FindAllBlocks()
	{
		int timer = 0;
		foreach (Transform child in base.transform)
		{
			PathfindingBlock PB = child.GetComponent<PathfindingBlock>();
			if ((bool)PB)
			{
				AllBlocks.Add(PB);
				PB.GridID = timer;
			}
			timer++;
		}
	}

	public PathfindingBlock FindBlock(int CallerID, int Direction)
	{
		PathfindingBlock FoundPB = null;
		switch (Direction)
		{
		case 0:
		{
			int otherGridID = CallerID - 1;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != 0)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 1:
		{
			int otherGridID = CallerID - MapSizeX - 2;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != 0)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 2:
		{
			int otherGridID = CallerID - MapSizeX - 1;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 3:
		{
			int otherGridID = CallerID - MapSizeX;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != MapSizeX)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 4:
		{
			int otherGridID = CallerID + 1;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != MapSizeX)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 5:
		{
			int otherGridID = CallerID + MapSizeX + 2;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != MapSizeX)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 6:
		{
			int otherGridID = CallerID + MapSizeX + 1;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		case 7:
		{
			int otherGridID = CallerID + MapSizeX;
			if (otherGridID > 0 && otherGridID < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != 0)
			{
				FoundPB = AllBlocks[otherGridID];
			}
			break;
		}
		}
		return FoundPB;
	}
}
