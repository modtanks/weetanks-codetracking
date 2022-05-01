using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankStatsItem : MonoBehaviour
{
	public int myStatID;

	public Texture[] TankImages;

	public string[] TankNames;

	public string[] CampaignNames;

	public string[] SurvivalNames;

	public TextMeshProUGUI Title;

	public TextMeshProUGUI Amount;

	public RawImage TankImage;

	public NewMenuControl NMC;

	public Transform originalParent;

	public int myMenu = 0;

	public Vector3 scale;

	public bool hidden = false;

	private void Start()
	{
		base.transform.localScale = scale;
		base.transform.localRotation = Quaternion.Euler(Vector3.zero);
		if (originalParent == null)
		{
			originalParent = base.transform.parent;
		}
		base.transform.SetParent(null);
		InvokeRepeating("SetData", 0.1f, 3f);
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.1f);
		base.transform.localScale = scale;
		if (myMenu == 0 || myMenu == 2)
		{
			Title.transform.localPosition += new Vector3(-25f, 0f, 0f);
		}
	}

	private void SetData()
	{
		if (!AccountMaster.instance.isSignedIn)
		{
			return;
		}
		if (myMenu == 1)
		{
			base.transform.localScale = scale;
			if (AccountMaster.instance.PDO.killed[myStatID] < 1)
			{
				base.transform.SetParent(null);
				hidden = true;
				return;
			}
			hidden = false;
			base.transform.SetParent(originalParent);
			TankImage.texture = TankImages[myStatID];
			Title.text = LocalizationMaster.instance.GetText(TankNames[myStatID]);
			Amount.text = AccountMaster.instance.PDO.killed[myStatID] + "x";
		}
		else if (myMenu == 0)
		{
			base.transform.localScale = scale;
			TankImage.gameObject.SetActive(value: false);
			Title.text = LocalizationMaster.instance.GetText(CampaignNames[myStatID]);
			string amount = "";
			switch (myStatID)
			{
			case 0:
				amount = AccountMaster.instance.PDO.maxMission0.ToString();
				break;
			case 1:
				amount = AccountMaster.instance.PDO.totalKills + "x";
				break;
			case 2:
				amount = AccountMaster.instance.PDO.totalKillsBounce + "x";
				break;
			case 3:
				amount = AccountMaster.instance.PDO.totalWins + "x";
				break;
			case 4:
				amount = AccountMaster.instance.PDO.totalDefeats + "x";
				break;
			}
			Amount.text = amount.ToString();
		}
		else if (myMenu == 2)
		{
			base.transform.localScale = scale;
			TankImage.gameObject.SetActive(value: false);
			Title.text = LocalizationMaster.instance.GetText(SurvivalNames[myStatID]);
			string amount2 = "";
			switch (myStatID)
			{
			case 0:
				amount2 = AccountMaster.instance.PDO.survivalTanksKilled + "x";
				break;
			case 1:
				amount2 = AccountMaster.instance.PDO.hW[0].ToString();
				break;
			case 2:
				amount2 = AccountMaster.instance.PDO.hW[1].ToString();
				break;
			case 3:
				amount2 = AccountMaster.instance.PDO.hW[2].ToString();
				break;
			case 4:
				amount2 = AccountMaster.instance.PDO.hW[3].ToString();
				break;
			case 5:
				amount2 = AccountMaster.instance.PDO.hW[4].ToString();
				break;
			case 6:
				amount2 = AccountMaster.instance.PDO.hW[5].ToString();
				break;
			}
			Amount.text = amount2.ToString();
		}
	}

	private void Update()
	{
		if ((bool)NMC)
		{
			if (NMC.StatisticsOpenMenu == myMenu && !hidden)
			{
				base.transform.SetParent(originalParent);
				base.transform.localScale = scale;
			}
			else if (base.transform.parent != null)
			{
				base.transform.SetParent(null);
				base.transform.localScale = scale;
			}
		}
	}
}
