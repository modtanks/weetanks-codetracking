using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CheatCodes : MonoBehaviour
{
	public AudioClip SuccesSound;

	public AudioClip ErrorSound;

	public AudioClip UnlockedSound;

	public AudioClip ClickSound;

	public AudioClip CheckSound;

	public string code;

	public string tempEnter;

	private void Start()
	{
	}

	private void Update()
	{
		string inputString;
		if (Input.anyKeyDown)
		{
			inputString = Input.inputString;
			foreach (char c in inputString)
			{
				if (c == '\n' || c == '\r')
				{
					SFXManager.instance.PlaySFX(CheckSound);
				}
				else
				{
					SFXManager.instance.PlaySFX(ClickSound);
				}
			}
		}
		if (!Input.anyKeyDown)
		{
			return;
		}
		inputString = Input.inputString;
		for (int i = 0; i < inputString.Length; i++)
		{
			char c2 = inputString[i];
			if (c2 == '\n' || c2 == '\r')
			{
				if (tempEnter.Length > 0)
				{
					StartCoroutine(PostRequest("https://www.weetanks.com/check_code.php", tempEnter));
				}
				tempEnter = "";
			}
			else
			{
				tempEnter += c2;
			}
		}
	}

	private IEnumerator PostRequest(string url, string cheatcode)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("cheatCode", cheatcode);
		wWWForm.AddField("fromGame", "true");
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text != "false")
			{
				int num = int.Parse(uwr.downloadHandler.text);
				if (!AccountMaster.instance.PDO.CC.Contains(num))
				{
					OptionsMainMenu.instance.AM_unlocked.Add(num);
					OptionsMainMenu.instance.SaveNewData();
					AccountMaster.instance.SaveCloudData(9, num, 0, bounceKill: false, 0f);
					SFXManager.instance.PlaySFX(UnlockedSound);
				}
			}
			else
			{
				SFXManager.instance.PlaySFX(ErrorSound);
			}
		}
		uwr.Dispose();
	}
}
