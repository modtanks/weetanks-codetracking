using UnityEngine;

public class CustomizableProp : MonoBehaviour
{
	private MapEditorProp MyMEP;

	public int MenuID;

	[Header("Lighthouse")]
	public bool IsLighthouse;

	public int LightHouseID;

	public ContinuousRotating LightHouseScript;

	public float RotationSpeedMultiplier = 10f;

	public Light LightHouse_spotlight;

	public Light LightHouse_normallight;

	private void Start()
	{
		MyMEP = GetComponent<MapEditorProp>();
		CheckCustomProp();
	}

	public void CheckCustomProp()
	{
		if (IsLighthouse)
		{
			LightHouseScript.RotateSpeed = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[MyMEP.myMEGP.ID].F1[MyMEP.LayerNumber] * RotationSpeedMultiplier;
			LightHouse_spotlight.color = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[MyMEP.myMEGP.ID].CustomColor[MyMEP.LayerNumber].Color;
			LightHouse_normallight.color = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[MyMEP.myMEGP.ID].CustomColor[MyMEP.LayerNumber].Color;
		}
	}
}
