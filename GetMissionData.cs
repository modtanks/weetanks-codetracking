using TMPro;
using UnityEngine;

public class GetMissionData : MonoBehaviour
{
	public TextMeshProUGUI Mission_Text;

	public TextMeshProUGUI Mission_Name_Text;

	public TextMeshProUGUI Mission_ShadowText;

	public TextMeshProUGUI TanksLeft_Text;

	public TextMeshProUGUI TanksLeft_ShadowText;

	public TextMeshProUGUI P1kills_Text;

	public TextMeshProUGUI P1kills_ShadowText;

	public TextMeshProUGUI P2kills_Text;

	public TextMeshProUGUI P2kills_ShadowText;

	private void Start()
	{
		Mission_Text.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (GameMaster.instance.CurrentMission + 1);
	}

	private void Update()
	{
		Mission_Name_Text.text = "'" + GameMaster.instance.MissionNames[GameMaster.instance.CurrentMission] + "'";
		if ((bool)MapEditorMaster.instance)
		{
			Mission_Text.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + (GameMaster.instance.CurrentMission + 1);
			TanksLeft_Text.text = "x" + GameMaster.instance.AmountEnemyTanks;
		}
		else if (GameMaster.instance != null)
		{
			int amount = GameMaster.instance.CurrentMission + 1;
			if (amount > 100)
			{
				amount = 100;
			}
			if (GameMaster.instance.PlayerAlive && GameMaster.instance.AmountGoodTanks > 0 && GameMaster.instance.AmountEnemyTanks > 0)
			{
				Mission_Text.text = LocalizationMaster.instance.GetText("HUD_mission") + " " + amount;
			}
			TanksLeft_Text.text = "x" + GameMaster.instance.AmountEnemyTanks;
		}
	}
}
