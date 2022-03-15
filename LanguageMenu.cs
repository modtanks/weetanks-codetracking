using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageMenu : MonoBehaviour
{
	private Animator myAnimator;

	public RawImage[] SelectableCircles;

	public TextMeshProUGUI[] SelectableTexts;

	public Texture SelectedCircle;

	public Texture NotSelectedCircle;

	public Color NormalTextColor;

	public Color SelectedTextColor;

	public Sprite[] LangSprites;

	public string[] LangNames;

	public Image CurrentCountrySprite;

	public TextMeshProUGUI CurrentLangText;

	private void Start()
	{
		myAnimator = GetComponent<Animator>();
		SetLang(LocalizationMaster.instance.CurrentLang);
	}

	public void ToggleMenu()
	{
		if (!myAnimator.GetBool("OpenMenu"))
		{
			myAnimator.SetBool("OpenMenu", !myAnimator.GetBool("OpenMenu"));
		}
	}

	public void SetLang(int langID)
	{
		SelectableCircles[LocalizationMaster.instance.CurrentLang].texture = NotSelectedCircle;
		SelectableTexts[LocalizationMaster.instance.CurrentLang].color = NormalTextColor;
		LocalizationMaster.instance.CurrentLang = langID;
		myAnimator.SetBool("OpenMenu", value: false);
		CurrentCountrySprite.sprite = LangSprites[langID];
		CurrentLangText.text = LangNames[langID];
		SelectableCircles[langID].texture = SelectedCircle;
		SelectableTexts[langID].color = SelectedTextColor;
		OptionsMainMenu.instance.SaveNewData();
	}
}
