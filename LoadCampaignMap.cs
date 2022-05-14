using UnityEngine;

public class LoadCampaignMap : MonoBehaviour
{
	public GameObject[] Props;

	public float[] PropStartHeight;

	public GameObject LoadMap(SingleMapEditorData SMED, int number)
	{
		GameObject gameObject = new GameObject("Level" + number);
		foreach (MapPiecesClass missionDataProp in SMED.MissionDataProps)
		{
			if (missionDataProp == null)
			{
				continue;
			}
			for (int i = 0; i < 5; i++)
			{
				if (missionDataProp.propID[i] > -1)
				{
					SpawnInProp(missionDataProp.propID[i], missionDataProp.propRotation[i], i, gameObject, missionDataProp.ID, missionDataProp.SpawnDifficulty, missionDataProp.TeamColor[i]);
				}
			}
		}
		gameObject.transform.position = new Vector3(-18f, 0f, 15f);
		return gameObject;
	}

	public void SpawnInProp(int propID, int direction, int LayerHeight, GameObject parent, int ID, int Difficulty, int TeamColor)
	{
		float num = PropStartHeight[propID];
		int num2 = Mathf.FloorToInt(ID / 15);
		int num3 = ID - num2 * 15;
		GameObject gameObject = Object.Instantiate(Props[propID], new Vector3(num2 * 2, num + (float)(LayerHeight * 2), -(num3 * 2)), Quaternion.Euler(0f, 90f * (float)direction, 0f), parent.transform);
		Quaternion quaternion = Quaternion.Euler(0f, 90f * (float)direction, 0f);
		if (propID > 3 && propID < 40)
		{
			gameObject.transform.rotation *= quaternion;
		}
		if (propID == 44)
		{
			gameObject.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
		}
		DifficultyCheck component = gameObject.GetComponent<DifficultyCheck>();
		if ((bool)component)
		{
			component.myDifficulty = Difficulty;
		}
		if (gameObject.transform.childCount <= 0)
		{
			return;
		}
		EnemyAI component2 = gameObject.transform.GetChild(0).GetComponent<EnemyAI>();
		if ((bool)component2)
		{
			component2.MyTeam = TeamColor;
			if (TeamColor == 1 && (bool)component2.ETSN)
			{
				component2.ETSN.isHuntingEnemies = true;
				Object.Instantiate(GlobalAssets.instance.TankFlag, component2.ETSN.transform.position, component2.ETSN.transform.rotation).transform.SetParent(component2.ETSN.transform);
			}
		}
	}
}
