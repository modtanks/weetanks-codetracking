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
			for (int i = 0; i < signedInObjects.Length; i++)
			{
				signedInObjects[i].SetActive(value: true);
			}
			signedInObjects = SignedOutObjects;
			for (int i = 0; i < signedInObjects.Length; i++)
			{
				signedInObjects[i].SetActive(value: false);
			}
			BackButton.Place = 3;
			CurrentlySignedInAs.gameObject.SetActive(value: true);
			CurrentlySignedInAs.text = "Currently signed in as: " + AccountMaster.instance.Username + " (" + AccountMaster.instance.PDO.marbles + " marbles)";
		}
		else
		{
			GameObject[] signedInObjects = SignedInObjects;
			for (int i = 0; i < signedInObjects.Length; i++)
			{
				signedInObjects[i].SetActive(value: false);
			}
			signedInObjects = SignedOutObjects;
			for (int i = 0; i < signedInObjects.Length; i++)
			{
				signedInObjects[i].SetActive(value: true);
			}
			BackButton.Place = 2;
			CurrentlySignedInAs.gameObject.SetActive(value: false);
			CurrentlySignedInAs.text = "";
		}
	}
}