using TMPro;
using UnityEngine;

public class AccountMenuHandler : MonoBehaviour
{
	public TextMeshProUGUI CreateAcc;

	public GameObject CreateAccSlider;

	public TextMeshProUGUI SignInAcc;

	public GameObject SignInAccSlider;

	public GameObject SignOutAccSlider;

	public GameObject SignOutButton;

	public GameObject CampaignsAccSlider;

	public GameObject CampaignsButton;

	public TextMeshProUGUI CurrentlySignedInAs;

	public NewMenuControl NMC;

	public GameObject LobbyButton;

	public GameObject LobbySlider;

	private void Start()
	{
		CheckMenu();
		InvokeRepeating("CheckMenu", 1f, 1f);
	}

	public void CheckMenu()
	{
		if (AccountMaster.instance.Username != null && AccountMaster.instance.UserID != null && AccountMaster.instance.Key != null)
		{
			CreateAccSlider.SetActive(value: false);
			SignInAccSlider.SetActive(value: false);
			CreateAcc.gameObject.SetActive(value: false);
			SignInAcc.gameObject.SetActive(value: false);
			SignOutButton.SetActive(value: true);
			SignOutAccSlider.SetActive(value: true);
			CampaignsAccSlider.SetActive(value: true);
			CampaignsButton.SetActive(value: true);
			LobbyButton.SetActive(value: false);
			LobbySlider.SetActive(value: false);
			CurrentlySignedInAs.gameObject.SetActive(value: true);
			CurrentlySignedInAs.text = "Currently signed in as: " + AccountMaster.instance.Username + " (" + AccountMaster.instance.PDO.marbles + " marbles)";
		}
		else
		{
			CreateAccSlider.SetActive(value: true);
			SignInAccSlider.SetActive(value: true);
			CreateAcc.gameObject.SetActive(value: true);
			SignInAcc.gameObject.SetActive(value: true);
			SignOutButton.SetActive(value: false);
			SignOutAccSlider.SetActive(value: false);
			CampaignsAccSlider.SetActive(value: false);
			CampaignsButton.SetActive(value: false);
			LobbyButton.SetActive(value: false);
			LobbySlider.SetActive(value: false);
			CurrentlySignedInAs.gameObject.SetActive(value: false);
			CurrentlySignedInAs.text = "";
		}
	}
}
