using System.Collections;
using TMPro;
using UnityEngine;

public class RecordMissionText : MonoBehaviour
{
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
		yield return new WaitForSeconds(0.2f);
		if (AccountMaster.instance.isSignedIn)
		{
			int num = GameMaster.instance.maxMissionReachedHard - 1;
			if (num < 0)
			{
				num = 0;
			}
			textHard = "RecoRd Misision: " + num;
			int num2 = GameMaster.instance.maxMissionReachedKid - 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			textKid = "RecoRd Misision: " + num2;
			int num3 = GameMaster.instance.maxMissionReached - 1;
			if (num3 < 0)
			{
				num3 = 0;
			}
			textNormal = "RecoRd Misision: " + num3;
		}
		else if (SavingData.ExistData())
		{
			ProgressDataNew progressDataNew = SavingData.LoadData();
			int num4 = progressDataNew.cH - 1;
			if (num4 < 0)
			{
				num4 = 0;
			}
			textHard = "RecoRd Misision: " + num4;
			int num5 = progressDataNew.cK - 1;
			if (num5 < 0)
			{
				num5 = 0;
			}
			textKid = "RecoRd Misision: " + num5;
			int num6 = progressDataNew.cM - 1;
			if (num6 < 0)
			{
				num6 = 0;
			}
			textNormal = "RecoRd Misision: " + num6;
		}
		else
		{
			textHard = "RecoRd Misision: -";
			textNormal = "RecoRd Misision: -";
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
	}
}
