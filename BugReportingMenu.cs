using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BugReportingMenu : MonoBehaviour
{
	public bool IsShowingMenu = false;

	public AudioClip TypeSound;

	public AudioClip UseMenuButtonSound;

	public AudioClip OpenMenuSound;

	public AudioClip CloseMenuSound;

	public AudioClip SuccesSentSound;

	public AudioClip ErrorSentSound;

	public TextMeshProUGUI ErrorField;

	public TextMeshProUGUI StatusField;

	public TMP_InputField DescriptionField;

	public TMP_InputField ReproductionField;

	public GameObject InputMenu;

	public GameObject StatusMenu;

	private Animator myAnimator;

	public ChangeHappinessButton[] HappinessButtons;

	public Canvas myCanvas;

	private void Start()
	{
		myCanvas = GetComponent<Canvas>();
		myCanvas.enabled = false;
		myAnimator = GetComponent<Animator>();
		GameObject ReportingMenu = GameObject.Find("BUG_REPORTING");
		if ((bool)ReportingMenu && ReportingMenu != base.gameObject)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		ResetForm();
	}

	public void SelectedButton(int buttonID)
	{
		for (int i = 0; i < HappinessButtons.Length; i++)
		{
			if (i != buttonID)
			{
				HappinessButtons[i].DeselectButton();
			}
		}
	}

	private void ResetForm()
	{
		ErrorField.text = "";
		StatusField.text = "";
		DescriptionField.text = "";
		ReproductionField.text = "";
	}

	public void DisableCanvas()
	{
		InputMenu.SetActive(value: false);
		StatusMenu.SetActive(value: false);
		for (int i = 0; i < HappinessButtons.Length; i++)
		{
			HappinessButtons[i].IsSelected = false;
			HappinessButtons[i].myImage.texture = HappinessButtons[i].NotSelected;
		}
		ResetForm();
		myCanvas.enabled = false;
	}

	public void EnableCanvas()
	{
		InputMenu.SetActive(value: true);
		StatusMenu.SetActive(value: false);
		myCanvas.enabled = true;
	}

	public void PlayButtonSound()
	{
		if ((bool)GameMaster.instance)
		{
			SFXManager.instance.PlaySFX(UseMenuButtonSound, 1f, null);
		}
	}

	public void PlayClickSound()
	{
		if ((bool)GameMaster.instance)
		{
			SFXManager.instance.PlaySFX(TypeSound, 0.6f, null);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F8))
		{
			if (IsShowingMenu)
			{
				CloseMenu();
			}
			else
			{
				myCanvas.enabled = true;
				IsShowingMenu = true;
				myAnimator.SetBool("ShowMenu", value: true);
				if ((bool)GameMaster.instance)
				{
					SFXManager.instance.PlaySFX(OpenMenuSound, 1f, null);
				}
				if ((bool)GameMaster.instance)
				{
					if ((bool)GameMaster.instance.PauseMenu)
					{
						PauseMenuScript PMS2 = GameMaster.instance.PauseMenu.GetComponent<PauseMenuScript>();
						if ((bool)PMS2)
						{
							PMS2.PauseGame();
						}
					}
					else
					{
						GameObject PauseMenuCanvas = GameObject.Find("PauseMenuCanvas");
						if ((bool)PauseMenuCanvas)
						{
							PauseMenuScript PMS = PauseMenuCanvas.GetComponent<PauseMenuScript>();
							if ((bool)PMS)
							{
								PMS.PauseGame();
							}
						}
					}
				}
			}
		}
		if (IsShowingMenu)
		{
			for (int i = 0; i < HappinessButtons.Length; i++)
			{
			}
		}
	}

	public void CloseMenu()
	{
		IsShowingMenu = false;
		myAnimator.SetBool("ShowMenu", value: false);
		if ((bool)GameMaster.instance)
		{
			SFXManager.instance.PlaySFX(CloseMenuSound, 1f, null);
		}
	}

	public void ReportBug()
	{
		ErrorField.text = "";
		if (DescriptionField.text.Contains("@") || DescriptionField.text.Contains("{") || DescriptionField.text.Contains("}") || DescriptionField.text.Contains("#") || ReproductionField.text.Contains("@") || ReproductionField.text.Contains("{") || ReproductionField.text.Contains("}") || ReproductionField.text.Contains("#"))
		{
			ErrorField.text = "<font-weight=900>Error:</font-weight> invalid character detected";
			if ((bool)GameMaster.instance)
			{
				SFXManager.instance.PlaySFX(ErrorSentSound, 1f, null);
			}
		}
		else if (DescriptionField.text.Length < 10)
		{
			ErrorField.text = "<font-weight=900>Error:</font-weight> please fill in a valid description";
			if ((bool)GameMaster.instance)
			{
				SFXManager.instance.PlaySFX(ErrorSentSound, 1f, null);
			}
		}
		else
		{
			InputMenu.SetActive(value: false);
			StatusMenu.SetActive(value: true);
			StatusField.text = "Sending...";
			StartCoroutine(SendBugReport());
		}
	}

	public IEnumerator SendBugReport()
	{
		WWWForm form = new WWWForm();
		if ((bool)GameMaster.instance)
		{
			if (GameMaster.instance.inMenuMode)
			{
				form.AddField("entry.1328679479", "Main Menu");
			}
			else if (GameMaster.instance.isZombieMode)
			{
				form.AddField("entry.1328679479", "Survival Mode");
			}
			else if (GameMaster.instance.inMapEditor)
			{
				form.AddField("entry.1328679479", "Map Editor");
			}
			else if (GameMaster.instance.inTankeyTown)
			{
				form.AddField("entry.1328679479", "Tankey Town");
			}
			else if (GameMaster.instance.isOfficialCampaign)
			{
				form.AddField("entry.1328679479", "Classic Campaign");
			}
			else if ((bool)MapEditorMaster.instance)
			{
				form.AddField("entry.1328679479", "Playing Custom Campaign");
			}
		}
		for (int i = 0; i < HappinessButtons.Length; i++)
		{
			if (HappinessButtons[i].IsSelected)
			{
				form.AddField("entry.2003059816", i switch
				{
					1 => "\ud83d\ude10", 
					0 => "\ud83d\ude04", 
					_ => "\ud83d\ude1f", 
				});
				break;
			}
		}
		form.AddField("entry.1180556855", OptionsMainMenu.instance.CurrentVersion);
		form.AddField("entry.1714000823", DescriptionField.text);
		form.AddField("entry.1622436738", ReproductionField.text);
		if (AccountMaster.instance.isSignedIn)
		{
			string thename = AccountMaster.instance.Username;
			form.AddField("entry.1662013560", thename);
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://docs.google.com/forms/u/0/d/e/1FAIpQLScpCY6PZeRcGnK_Z-1eingkVRReszR-tOFU3axgIF9yF0YtFQ/formResponse", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			StatusField.text = "<COLOR=#CF7171>Failed!</color>";
			if ((bool)GameMaster.instance)
			{
				SFXManager.instance.PlaySFX(ErrorSentSound, 1f, null);
			}
			yield return new WaitForSeconds(2f);
			if (IsShowingMenu)
			{
				CloseMenu();
			}
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("Wee Tanks Bug/Suggestion Report") || uwr.downloadHandler.text.Contains("Wee Tanks Bug/Suggestion Report"))
		{
			StatusField.text = "<COLOR=#71CF9D>Success!</color>";
			if ((bool)GameMaster.instance)
			{
				SFXManager.instance.PlaySFX(SuccesSentSound, 1f, null);
			}
			yield return new WaitForSeconds(2f);
			if (IsShowingMenu)
			{
				CloseMenu();
			}
		}
		else
		{
			StatusField.text = "<COLOR=#CF7171>Failed!</color>";
			if ((bool)GameMaster.instance)
			{
				SFXManager.instance.PlaySFX(ErrorSentSound, 1f, null);
			}
			yield return new WaitForSeconds(2f);
			if (IsShowingMenu)
			{
				CloseMenu();
			}
		}
	}
}
