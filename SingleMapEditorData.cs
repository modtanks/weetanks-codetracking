using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SingleMapEditorData
{
	public List<MapPiecesClass> MissionDataProps = new List<MapPiecesClass>();

	public string MissionMessage;

	public SingleMapEditorData(GameMaster GM, MapEditorMaster MEM)
	{
		if (GM == null && MEM == null)
		{
			Debug.Log("NEW SMED!");
			MissionDataProps = new List<MapPiecesClass>();
		}
		else
		{
			Debug.Log("SMED SET!");
			MissionDataProps = MEM.Levels[0].MissionDataProps;
		}
	}
}
