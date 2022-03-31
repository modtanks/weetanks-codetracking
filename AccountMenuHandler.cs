using TMPro;
using UnityEngine;

public class AccountMenuHandler : MonoBehaviour
{
	public TextMeshProUGUI CurrentlySignedInAs;

	public NewMenuControl NMC;

	public GameObject LobbyButton;

	public GameObject[] SignedInObjects;

	public GameObject[] SignedOutObjects;

	public MainMenuButtons BackButton;

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
			GameObject[] signedInObjects = SignedInObjects;
			foreach (GameObject obj3 in signedInObjects)
			{
				obj3.SetActive(value: true);
			}
			GameObject[] signedOutObjects = SignedOutObjects;
			foreach (GameObject obj4 in signedOutObjects)
			{
				obj4.SetActive(value: false);
			}
			BackButton.Place = 4;
			CurrentlySignedInAs.gameObject.SetActive(value: true);
			CurrentlySignedInAs.text = LocalizationMaster.instance.GetText("Account_signed_in_status") + "<br>" + AccountMaster.instance.Username + " (" + AccountMaster.instance.PDO.marbles + " marbles)";
		}
		else
		{
			GameObject[] signedInObjects2 = SignedInObjects;
			foreach (GameObject obj in signedInObjects2)
			{
				obj.SetActive(value: false);
			}
			GameObject[] signedOutObjects2 = SignedOutObjects;
			foreach (GameObject obj2 in signedOutObjects2)
			{
				obj2.SetActive(value: true);
			}
			BackButton.Place = 2;
			CurrentlySignedInAs.gameObject.SetActive(value: false);
			CurrentlySignedInAs.text = "";
		}
	}
}
