using UnityEngine;

public class MapEditorGridGenerator : MonoBehaviour
{
	public GameObject gridPiece;

	public int[] GridX;

	public int[] GridY;

	public bool generated;

	private int selection = 1;

	private void Start()
	{
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			selection = 0;
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			selection = 1;
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			selection = 2;
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			selection = 3;
		}
		if (!generated)
		{
			GenerateFields();
		}
	}

	public void GenerateFields()
	{
		MapEditorMaster.instance.ID = OptionsMainMenu.instance.MapSize * (GameMaster.instance.Levels.Count - 1);
		if (OptionsMainMenu.instance.MapSize == 180)
		{
			selection = 0;
		}
		else if (OptionsMainMenu.instance.MapSize == 285)
		{
			selection = 1;
		}
		else if (OptionsMainMenu.instance.MapSize == 374)
		{
			selection = 2;
		}
		else if (OptionsMainMenu.instance.MapSize == 475)
		{
			selection = 3;
		}
		for (int i = 0; i <= GridX[selection]; i++)
		{
			for (int j = 0; j <= GridY[selection]; j++)
			{
				GameObject obj = Object.Instantiate(gridPiece, new Vector3(base.transform.position.x + (float)(i * 2), base.transform.position.y, base.transform.position.z - (float)(j * 2)), Quaternion.identity);
				obj.transform.parent = base.transform.parent;
				MapEditorGridPiece component = obj.GetComponent<MapEditorGridPiece>();
				component.mission = GameMaster.instance.CurrentMission;
				MapPiecesClass item = new MapPiecesClass
				{
					ID = MapEditorMaster.instance.ID,
					missionNumber = GameMaster.instance.CurrentMission
				};
				component.mission = GameMaster.instance.CurrentMission;
				component.ID = MapEditorMaster.instance.ID;
				MapEditorMaster.instance.ID++;
				MapEditorMaster.instance.MissionsData.Add(item);
			}
		}
		generated = true;
	}
}
