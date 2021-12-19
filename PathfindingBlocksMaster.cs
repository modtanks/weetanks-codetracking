using System.Collections.Generic;
using UnityEngine;

public class PathfindingBlocksMaster : MonoBehaviour
{
	public List<PathfindingBlock> AllBlocks = new List<PathfindingBlock>();

	public int MapSizeX;

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
		int num = 0;
		foreach (Transform item in base.transform)
		{
			PathfindingBlock component = item.GetComponent<PathfindingBlock>();
			if ((bool)component)
			{
				AllBlocks.Add(component);
				component.GridID = num;
			}
			num++;
		}
	}

	public PathfindingBlock FindBlock(int CallerID, int Direction)
	{
		PathfindingBlock result = null;
		switch (Direction)
		{
		case 0:
		{
			int num = CallerID - 1;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != 0)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 1:
		{
			int num = CallerID - MapSizeX - 2;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != 0)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 2:
		{
			int num = CallerID - MapSizeX - 1;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 3:
		{
			int num = CallerID - MapSizeX;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != MapSizeX)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 4:
		{
			int num = CallerID + 1;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != MapSizeX)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 5:
		{
			int num = CallerID + MapSizeX + 2;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != MapSizeX)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 6:
		{
			int num = CallerID + MapSizeX + 1;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize)
			{
				result = AllBlocks[num];
			}
			break;
		}
		case 7:
		{
			int num = CallerID + MapSizeX;
			if (num > 0 && num < OptionsMainMenu.instance.MapSize && CallerID % (MapSizeX + 1) != 0)
			{
				result = AllBlocks[num];
			}
			break;
		}
		}
		return result;
	}
}
