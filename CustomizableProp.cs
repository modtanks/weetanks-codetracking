using System.Collections;
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

	[Header("Piston")]
	public bool IsPiston;

	private Animator PistonAnimator;

	private float WaitTime;

	private bool IsStartingPiston;

	private void Start()
	{
		MyMEP = GetComponent<MapEditorProp>();
		PistonAnimator = GetComponent<Animator>();
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
		else if (IsPiston && (bool)PistonAnimator)
		{
			if (MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[MyMEP.myMEGP.ID].F1 == null)
			{
				WaitTime = 0f;
			}
			else
			{
				WaitTime = MapEditorMaster.instance.Levels[GameMaster.instance.CurrentMission].MissionDataProps[MyMEP.myMEGP.ID].F1[MyMEP.LayerNumber];
			}
			if (!IsStartingPiston)
			{
				IsStartingPiston = true;
				StartCoroutine(StartPistonAnimation());
			}
		}
	}

	private IEnumerator StartPistonAnimation()
	{
		yield return new WaitForSeconds(WaitTime);
		PistonAnimator.SetBool("CanPlay", value: true);
	}
}
