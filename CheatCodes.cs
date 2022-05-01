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
					Play2DClipAtPoint(CheckSound);
				}
				else
				{
					Play2DClipAtPoint(ClickSound);
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
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text != "false")
		{
			int num = int.Parse(uwr.downloadHandler.text);
			if (OptionsMainMenu.instance.AM[num] != 1)
			{
				OptionsMainMenu.instance.AM[num] = 1;
				OptionsMainMenu.instance.SaveNewData();
				AccountMaster.instance.PDO.AM[num] = 1;
				AccountMaster.instance.SaveCloudData(3, num, 0, bounceKill: false);
				Play2DClipAtPoint(UnlockedSound);
			}
		}
		else
		{
			Play2DClipAtPoint(ErrorSound);
		}
	}

	public void Play2DClipAtPoint(AudioClip clip)
	{
		GameObject obj = new GameObject("TempAudio");
		AudioSource audioSource = obj.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.ignoreListenerVolume = true;
		audioSource.volume = 1f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(obj, clip.length);
	}
}
