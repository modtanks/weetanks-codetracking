using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementItemScript : MonoBehaviour
{
	public int AMID = 0;

	public TextMeshProUGUI MyTitle;

	public TextMeshProUGUI MyDescription;

	public GameObject CheckMark;

	public int myDifficulty = 0;

	public Transform myParent;

	public RawImage AchievementPicture;

	public RawImage AchievementBorder;

	public Texture[] Borders;

	public Vector3 scale;

	public bool isInGame = false;

	public string message;

	public GameObject BACKGROUND;

	public Texture[] TankTextures;

	public GameObject CompanionAllowed;

	public int[] CompanionAllowedIDS;

	private int prevLang = -1;

	private void Start()
	{
		if ((bool)CompanionAllowed)
		{
			CompanionAllowed.SetActive(value: false);
		}
		base.transform.localScale = scale;
		base.transform.localRotation = Quaternion.Euler(Vector3.zero);
		myParent = base.transform.parent;
		if (!isInGame)
		{
			base.transform.SetParent(null);
		}
		if (message != "")
		{
			MyTitle.text = "Map Editor Unlock!";
			CheckMark.SetActive(value: true);
			MyDescription.text = message;
			AchievementPicture.texture = TankTextures[AMID];
			AchievementBorder.gameObject.SetActive(value: false);
			BACKGROUND.SetActive(value: false);
		}
		else if ((bool)OptionsMainMenu.instance)
		{
			SetText();
		}
	}

	private void SetText()
	{
		Debug.Log("SETTING ACHIEVENEMT TEXT ID : " + AMID);
		Debug.Log("AM_" + AMID);
		prevLang = LocalizationMaster.instance.CurrentLang;
		MyTitle.text = LocalizationMaster.instance.GetText("AM_" + AMID);
		MyDescription.text = LocalizationMaster.instance.GetText("AM_desc_" + AMID);
		myDifficulty = OptionsMainMenu.instance.AMdifficulty[AMID];
		if (OptionsMainMenu.instance.AM[AMID] == 1)
		{
			CheckMark.SetActive(value: true);
			AchievementBorder.texture = Borders[myDifficulty];
			if (OptionsMainMenu.instance.AMimages[AMID] != null)
			{
				AchievementPicture.texture = OptionsMainMenu.instance.AMimages[AMID];
			}
			else
			{
				AchievementPicture.texture = OptionsMainMenu.instance.AMimages[0];
			}
			return;
		}
		int[] companionAllowedIDS = CompanionAllowedIDS;
		foreach (int ID in companionAllowedIDS)
		{
			if (ID == AMID && (bool)CompanionAllowed)
			{
				CompanionAllowed.SetActive(value: true);
			}
		}
		CheckMark.SetActive(value: false);
		AchievementBorder.texture = Borders[4];
		if (OptionsMainMenu.instance.AMimages[AMID] != null)
		{
			AchievementPicture.texture = OptionsMainMenu.instance.AMimagesNot[AMID];
		}
		else
		{
			AchievementPicture.texture = OptionsMainMenu.instance.AMimagesNot[0];
		}
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForSeconds(0.1f);
	}

	private void DestroyMe()
	{
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!GameMaster.instance.inMenuMode)
		{
			return;
		}
		if (prevLang != LocalizationMaster.instance.CurrentLang)
		{
			SetText();
		}
		if ((bool)AchievementsTracker.instance && !isInGame)
		{
			if (AchievementsTracker.instance.selectedDifficulty != myDifficulty)
			{
				base.transform.SetParent(null);
			}
			else
			{
				base.transform.SetParent(myParent);
			}
		}
	}
}
