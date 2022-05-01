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
		if (Input.anyKeyDown)
		{
			string inputString = Input.inputString;
			foreach (char c2 in inputString)
			{
				if (c2 == '\n' || c2 == '\r')
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
		string inputString2 = Input.inputString;
		for (int j = 0; j < inputString2.Length; j++)
		{
			char c = inputString2[j];
			if (c == '\n' || c == '\r')
			{
				if (tempEnter.Length > 0)
				{
					StartCoroutine(PostRequest("https://www.weetanks.com/check_code.php", tempEnter));
				}
				tempEnter = "";
			}
			else
			{
				tempEnter += c;
			}
		}
	}

	private IEnumerator PostRequest(string url, string cheatcode)
	{
		WWWForm form = new WWWForm();
		form.AddField("cheatCode", cheatcode);
		form.AddField("fromGame", "true");
		UnityWebRequest uwr = UnityWebRequest.Post(url, form);
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
			int ID = int.Parse(uwr.downloadHandler.text);
			if (OptionsMainMenu.instance.AM[ID] != 1)
			{
				OptionsMainMenu.instance.AM[ID] = 1;
				OptionsMainMenu.instance.SaveNewData();
				AccountMaster.instance.PDO.AM[ID] = 1;
				AccountMaster.instance.SaveCloudData(3, ID, 0, bounceKill: false);
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
		GameObject tempAudioSource = new GameObject("TempAudio");
		AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.ignoreListenerVolume = true;
		audioSource.volume = 1f * (float)OptionsMainMenu.instance.masterVolumeLvl / 10f;
		audioSource.spatialBlend = 0f;
		audioSource.Play();
		Object.Destroy(tempAudioSource, clip.length);
	}
}
