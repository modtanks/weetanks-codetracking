using System.Collections;
using TMPro;
using UnityEngine;

public class RecordMissionText : MonoBehaviour
{
	private string textGrandpa;

	private string textHard;

	private string textNormal;

	private string textKid;

	private TextMeshProUGUI mytext;

	private void Start()
	{
		mytext = GetComponent<TextMeshProUGUI>();
		StartCoroutine(GetInfo());
	}

	private IEnumerator GetInfo()
	{
		if (AccountMaster.instance.isSignedIn)
		{
			int completedMissions2 = AccountMaster.instance.PDO.maxMission2 - 1;
			if (completedMissions2 < 0)
			{
				completedMissions2 = 0;
			}
			textHard = "RecoRd Misision: " + completedMissions2;
			int completedKid2 = AccountMaster.instance.PDO.maxMission1 - 1;
			if (completedKid2 < 0)
			{
				completedKid2 = 0;
			}
			textKid = "RecoRd Misision: " + completedKid2;
			int completedMissions4 = AccountMaster.instance.PDO.maxMission0 - 1;
			if (completedMissions4 < 0)
			{
				completedMissions4 = 0;
			}
			textNormal = "RecoRd Misision: " + completedMissions4;
			int completedGrandpa2 = AccountMaster.instance.PDO.maxMission3 - 1;
			if (completedGrandpa2 < 0)
			{
				completedGrandpa2 = 0;
			}
			textGrandpa = "RecoRd Misision: " + completedGrandpa2;
		}
		else if (SavingData.ExistData())
		{
			ProgressDataNew data = SavingData.LoadData();
			int completedMissions = data.cH - 1;
			if (completedMissions < 0)
			{
				completedMissions = 0;
			}
			textHard = "RecoRd Misision: " + completedMissions;
			int completedKid = data.cK - 1;
			if (completedKid < 0)
			{
				completedKid = 0;
			}
			textKid = "RecoRd Misision: " + completedKid;
			int completedMissions3 = data.cM - 1;
			if (completedMissions3 < 0)
			{
				completedMissions3 = 0;
			}
			textNormal = "RecoRd Misision: " + completedMissions3;
			if (data.cG > 0)
			{
				int completedGrandpa = data.cG - 1;
				if (completedGrandpa < 0)
				{
					completedGrandpa = 0;
				}
				textGrandpa = "RecoRd Misision: " + completedGrandpa;
			}
		}
		else
		{
			textHard = "RecoRd Misision: -";
			textNormal = "RecoRd Misision: -";
			textGrandpa = "RecoRd Misision: -";
		}
		yield return new WaitForSeconds(1f);
		StartCoroutine(GetInfo());
	}

	private void Update()
	{
		if (OptionsMainMenu.instance.currentDifficulty == 0 && mytext.text != textNormal)
		{
			mytext.text = textNormal;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 2 && mytext.text != textHard)
		{
			mytext.text = textHard;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 1 && mytext.text != textKid)
		{
			mytext.text = textKid;
		}
		else if (OptionsMainMenu.instance.currentDifficulty == 3 && mytext.text != textGrandpa)
		{
			mytext.text = textGrandpa;
		}
	}
}
