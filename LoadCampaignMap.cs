using UnityEngine;

public class LoadCampaignMap : MonoBehaviour
{
	public GameObject[] Props;

	public float[] PropStartHeight;

	public GameObject LoadMap(SingleMapEditorData SMED, int number)
	{
		string name = "Level" + number;
		GameObject NewLevel = new GameObject(name);
		foreach (MapPiecesClass PropData in SMED.MissionDataProps)
		{
			if (PropData == null)
			{
				continue;
			}
			for (int i = 0; i < 5; i++)
			{
				if (PropData.propID[i] > -1)
				{
					SpawnInProp(PropData.propID[i], PropData.propRotation[i], i, NewLevel, PropData.ID, PropData.SpawnDifficulty, PropData.TeamColor[i]);
				}
			}
		}
		NewLevel.transform.position = new Vector3(-18f, 0f, 15f);
		return NewLevel;
	}

	public void SpawnInProp(int propID, int direction, int LayerHeight, GameObject parent, int ID, int Difficulty, int TeamColor)
	{
		float Yheight = PropStartHeight[propID];
		int Xpos = Mathf.FloorToInt(ID / 15);
		int Zpos = ID - Xpos * 15;
		GameObject Prop = Object.Instantiate(Props[propID], new Vector3(Xpos * 2, Yheight + (float)(LayerHeight * 2), -(Zpos * 2)), Quaternion.Euler(0f, 90f * (float)direction, 0f), parent.transform);
		Quaternion theRot = Quaternion.Euler(0f, 90f * (float)direction, 0f);
		if (propID > 3 && propID < 40)
		{
			Prop.transform.rotation *= theRot;
		}
		if (propID == 44)
		{
			Prop.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
		}
		DifficultyCheck DC = Prop.GetComponent<DifficultyCheck>();
		if ((bool)DC)
		{
			DC.myDifficulty = Difficulty;
		}
		if (Prop.transform.childCount <= 0)
		{
			return;
		}
		EnemyAI EA = Prop.transform.GetChild(0).GetComponent<EnemyAI>();
		if ((bool)EA)
		{
			EA.MyTeam = TeamColor;
			if (TeamColor == 1 && (bool)EA.ETSN)
			{
				EA.ETSN.isHuntingEnemies = true;
				GameObject newFlag = Object.Instantiate(GlobalAssets.instance.TankFlag, EA.ETSN.transform.position, EA.ETSN.transform.rotation);
				newFlag.transform.SetParent(EA.ETSN.transform);
			}
		}
	}
}
