using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BugReportingMenu : MonoBehaviour
{
	public bool IsShowingMenu;

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
		GameObject gameObject = GameObject.Find("BUG_REPORTING");
		if ((bool)gameObject && gameObject != base.gameObject)
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
			GameMaster.instance.Play2DClipAtPoint(UseMenuButtonSound, 1f);
		}
	}

	public void PlayClickSound()
	{
		if ((bool)GameMaster.instance)
		{
			GameMaster.instance.Play2DClipAtPoint(TypeSound, 0.6f);
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
					GameMaster.instance.Play2DClipAtPoint(OpenMenuSound, 1f);
				}
				if ((bool)GameMaster.instance)
				{
					if ((bool)GameMaster.instance.PauseMenu)
					{
						PauseMenuScript component = GameMaster.instance.PauseMenu.GetComponent<PauseMenuScript>();
						if ((bool)component)
						{
							component.PauseGame();
						}
					}
					else
					{
						GameObject gameObject = GameObject.Find("PauseMenuCanvas");
						if ((bool)gameObject)
						{
							PauseMenuScript component2 = gameObject.GetComponent<PauseMenuScript>();
							if ((bool)component2)
							{
								component2.PauseGame();
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
			GameMaster.instance.Play2DClipAtPoint(CloseMenuSound, 1f);
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
				GameMaster.instance.Play2DClipAtPoint(ErrorSentSound, 1f);
			}
		}
		else if (DescriptionField.text.Length < 10)
		{
			ErrorField.text = "<font-weight=900>Error:</font-weight> please fill in a valid description";
			if ((bool)GameMaster.instance)
			{
				GameMaster.instance.Play2DClipAtPoint(ErrorSentSound, 1f);
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
		WWWForm wWWForm = new WWWForm();
		if ((bool)GameMaster.instance)
		{
			if (GameMaster.instance.inMenuMode)
			{
				wWWForm.AddField("Scene", "Main Menu");
			}
			else if (GameMaster.instance.isZombieMode)
			{
				wWWForm.AddField("Scene", "Survival Mode");
			}
			else if (GameMaster.instance.inMapEditor)
			{
				wWWForm.AddField("Scene", "Map Editor");
			}
			else if (GameMaster.instance.inTankeyTown)
			{
				wWWForm.AddField("Scene", "Tankey Town");
			}
			else if (GameMaster.instance.isOfficialCampaign)
			{
				wWWForm.AddField("Scene", "Classic Campaign");
			}
			else if ((bool)MapEditorMaster.instance)
			{
				wWWForm.AddField("Scene", "Playing Custom Campaign");
			}
		}
		for (int i = 0; i < HappinessButtons.Length; i++)
		{
			if (HappinessButtons[i].IsSelected)
			{
				wWWForm.AddField("happiness", i);
				break;
			}
		}
		wWWForm.AddField("bug", DescriptionField.text);
		wWWForm.AddField("reproduce", ReproductionField.text);
		if (AccountMaster.instance.isSignedIn)
		{
			string username = AccountMaster.instance.Username;
			wWWForm.AddField("account_name", username);
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://hooks.zapier.com/hooks/catch/11555672/b1fglp9/", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			StatusField.text = "<COLOR=#CF7171>Failed!</color>";
			if ((bool)GameMaster.instance)
			{
				GameMaster.instance.Play2DClipAtPoint(ErrorSentSound, 1f);
			}
			yield return new WaitForSeconds(2f);
			if (IsShowingMenu)
			{
				CloseMenu();
			}
		}
		else if (uwr.downloadHandler.text.Contains("success") || uwr.downloadHandler.text.Contains("Success"))
		{
			StatusField.text = "<COLOR=#71CF9D>Success!</color>";
			if ((bool)GameMaster.instance)
			{
				GameMaster.instance.Play2DClipAtPoint(SuccesSentSound, 1f);
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
				GameMaster.instance.Play2DClipAtPoint(ErrorSentSound, 1f);
			}
			yield return new WaitForSeconds(2f);
			if (IsShowingMenu)
			{
				CloseMenu();
			}
		}
	}
}
