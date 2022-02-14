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
			int num = AccountMaster.instance.PDO.maxMission2 - 1;
			if (num < 0)
			{
				num = 0;
			}
			textHard = "RecoRd Misision: " + num;
			int num2 = AccountMaster.instance.PDO.maxMission1 - 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			textKid = "RecoRd Misision: " + num2;
			int num3 = AccountMaster.instance.PDO.maxMission0 - 1;
			if (num3 < 0)
			{
				num3 = 0;
			}
			textNormal = "RecoRd Misision: " + num3;
			int num4 = AccountMaster.instance.PDO.maxMission3 - 1;
			if (num4 < 0)
			{
				num4 = 0;
			}
			textGrandpa = "RecoRd Misision: " + num4;
		}
		else if (SavingData.ExistData())
		{
			ProgressDataNew progressDataNew = SavingData.LoadData();
			int num5 = progressDataNew.cH - 1;
			if (num5 < 0)
			{
				num5 = 0;
			}
			textHard = "RecoRd Misision: " + num5;
			int num6 = progressDataNew.cK - 1;
			if (num6 < 0)
			{
				num6 = 0;
			}
			textKid = "RecoRd Misision: " + num6;
			int num7 = progressDataNew.cM - 1;
			if (num7 < 0)
			{
				num7 = 0;
			}
			textNormal = "RecoRd Misision: " + num7;
			if (progressDataNew.cG > 0)
			{
				int num8 = progressDataNew.cG - 1;
				if (num8 < 0)
				{
					num8 = 0;
				}
				textGrandpa = "RecoRd Misision: " + num8;
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
