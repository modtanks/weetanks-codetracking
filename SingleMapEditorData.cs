using System;
using System.Collections.Generic;

[Serializable]
public class SingleMapEditorData
{
	public List<MapPiecesClass> MissionDataProps = new List<MapPiecesClass>();

	public string MissionMessage;

	public SingleMapEditorData(GameMaster GM, MapEditorMaster MEM)
	{
		if (GM == null && MEM == null)
		{
			MissionDataProps = new List<MapPiecesClass>();
		}
		else
		{
			MissionDataProps = MEM.Levels[0].MissionDataProps;
		}
	}
}
