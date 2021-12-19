using UnityEngine;

public class LoadCampaignMap : MonoBehaviour
{
	public GameObject[] Props;

	public float[] PropStartHeight;

	public GameObject LoadMap(SingleMapEditorData SMED, int number)
	{
		GameObject gameObject = new GameObject("Level" + number);
		Debug.Log("new level");
		foreach (MapPiecesClass missionDataProp in SMED.MissionDataProps)
		{
			if (missionDataProp == null)
			{
				continue;
			}
			Debug.Log("with data!");
			for (int i = 0; i < 5; i++)
			{
				if (missionDataProp.propID[i] > -1)
				{
					Debug.Log("with prop data");
					SpawnInProp(missionDataProp.propID[i], missionDataProp.propRotation[i], i, gameObject, missionDataProp.ID, missionDataProp.SpawnDifficulty);
				}
			}
		}
		gameObject.transform.position = new Vector3(-18f, 0f, 15f);
		return gameObject;
	}

	public void SpawnInProp(int propID, int direction, int LayerHeight, GameObject parent, int ID, int Difficulty)
	{
		float num = PropStartHeight[propID];
		int num2 = Mathf.FloorToInt(ID / 15);
		int num3 = ID - num2 * 15;
		GameObject gameObject = Object.Instantiate(Props[propID], new Vector3(num2 * 2, num + (float)(LayerHeight * 2), -(num3 * 2)), Quaternion.Euler(0f, 90f * (float)direction, 0f), parent.transform);
		Debug.Log("SPAWNED!!!");
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
	}
}
