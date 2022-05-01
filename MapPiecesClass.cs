using System;

[Serializable]
public class MapPiecesClass
{
	public int ID;

	public int offsetX;

	public int offsetY;

	public int[] propID = new int[5];

	public int missionNumber;

	public int[] propRotation = new int[5];

	public int[] TeamColor = new int[5];

	public int SpawnDifficulty = 0;

	public SerializableColor[] CustomColor = new SerializableColor[5];
}
