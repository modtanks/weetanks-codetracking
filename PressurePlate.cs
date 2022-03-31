using UnityEngine;

public class PressurePlate : MonoBehaviour
{
	public GameObject TheNextLevel;

	public bool Triggered = false;

	public AudioClip StepOnSound;

	public string missionName;

	public bool isNightMission = false;

	public bool isCustomTrainMission = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !Triggered)
		{
			Triggered = true;
			SFXManager.instance.PlaySFX(StepOnSound, 1f, null);
			GameMaster.instance.Levels[GameMaster.instance.CurrentMission + 1] = TheNextLevel;
			GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission + 1] = missionName;
			GameMaster.instance.SecretMissionCounter = 2;
			if (isNightMission)
			{
				GameMaster.instance.NightLevels.Add(GameMaster.instance.CurrentMission + 1);
			}
			if (isCustomTrainMission)
			{
				GameMaster.instance.TrainLevels.Add(GameMaster.instance.CurrentMission + 1);
			}
			if (GameMaster.instance.CurrentMission + 1 == 39)
			{
				GameMaster.instance.TrainLevels.Remove(GameMaster.instance.CurrentMission + 1);
			}
			if (GameMaster.instance.CurrentMission + 1 == 80)
			{
				GameMaster.instance.NightLevels.Remove(GameMaster.instance.CurrentMission + 1);
			}
		}
	}
}
