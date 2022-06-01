using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CampaignItemScript : MonoBehaviour
{
	public bool isMyCampaign;

	public bool isMainMenuCampaign;

	public int campaignID;

	public int isPublished;

	public int times_favorited;

	public int times_downloaded;

	public int map_size;

	public int campaign_difficulty;

	public bool hasDownloaded;

	public int amount_missions;

	public string campaignVersion;

	public string campaignName;

	public string campaignAuthor;

	public string file_url;

	public string coded_file_name;

	public Toggle btn_online;

	public bool isUploading;

	public bool isDownloading;

	public GameObject[] HideWhenMessage;

	public GameObject ConfirmDelete;

	public TextMeshProUGUI text_mapname;

	public TextMeshProUGUI text_authorname;

	public TextMeshProUGUI text_favorited;

	public TextMeshProUGUI text_downloaded;

	public TextMeshProUGUI text_difficulty;

	public TextMeshProUGUI text_version;

	public TextMeshProUGUI text_amountmissions;

	public TextMeshProUGUI text_subtitle;

	public GameObject[] NormalButtons;

	public GameObject DownloadButton;

	public Texture Tex_NoFavorite;

	public Texture Tex_Favorite;

	public RawImage FavoriteHolder;

	public NewMenuControl NMC;

	public void Upload()
	{
		if (!isUploading)
		{
			isUploading = true;
			StartCoroutine(UploadCampaign());
		}
	}

	public void Download()
	{
		if (!isDownloading)
		{
			isDownloading = true;
			StartCoroutine(DownloadCampaign());
		}
	}

	public void Favorite()
	{
		if (!isDownloading && !isUploading)
		{
			StartCoroutine(FavoriteCampaign());
		}
	}

	public void RemoveThisCampaign()
	{
		if (!isDownloading)
		{
			StartCoroutine(RemovePublicCampaign());
		}
	}

	public void PlayPublicMap()
	{
		OptionsMainMenu.instance.MapEditorMapName = "downloads/" + coded_file_name;
		OptionsMainMenu.instance.MapSize = map_size;
		NMC.StartCoroutine(NMC.LoadYourAsyncScene(4));
	}

	public void PlayMap()
	{
		OptionsMainMenu.instance.MapEditorMapName = campaignName;
		OptionsMainMenu.instance.MapSize = map_size;
		NMC.StartCoroutine(NMC.LoadYourAsyncScene(4));
	}

	public void DeleteCampaign()
	{
		GameObject[] hideWhenMessage = HideWhenMessage;
		for (int i = 0; i < hideWhenMessage.Length; i++)
		{
			hideWhenMessage[i].SetActive(value: false);
		}
		ConfirmDelete.SetActive(value: true);
	}

	public void ShowDownloadButton()
	{
		GameObject[] normalButtons = NormalButtons;
		for (int i = 0; i < normalButtons.Length; i++)
		{
			normalButtons[i].SetActive(value: false);
		}
		DownloadButton.SetActive(value: true);
	}

	public void HideDownloadButton()
	{
		GameObject[] normalButtons = NormalButtons;
		for (int i = 0; i < normalButtons.Length; i++)
		{
			normalButtons[i].SetActive(value: true);
		}
		DownloadButton.SetActive(value: false);
	}

	public void DeleteCampaignCancle()
	{
		GameObject[] hideWhenMessage = HideWhenMessage;
		for (int i = 0; i < hideWhenMessage.Length; i++)
		{
			hideWhenMessage[i].SetActive(value: true);
		}
		ConfirmDelete.SetActive(value: false);
	}

	private IEnumerator DeleteCampaignQuery()
	{
		NMC.GPM.isLoading = true;
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		isUploading = false;
		if (keyRequest.result != UnityWebRequest.Result.Success)
		{
			NMC.GPM.isLoading = false;
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			NMC.GPM.isLoading = false;
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignID", campaignID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/remove_map.php", wWWForm);
		yield return uwr.SendWebRequest();
		NMC.GPM.isLoading = false;
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text == "FAILED")
		{
			btn_online.isOn = false;
		}
		else if (uwr.downloadHandler.text == "SUCCES")
		{
			_ = Application.persistentDataPath + "/";
			File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/My Games/Wee Tanks/" + campaignName + ".campaign");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void DeleteCampaignConfirm()
	{
		StartCoroutine(DeleteCampaignQuery());
	}

	private void Start()
	{
		if (isPublished > 0 && campaignName.Length > 2 && isMyCampaign && !isMainMenuCampaign)
		{
			btn_online.isOn = true;
			StartCoroutine(GetMapData());
		}
		else if (isPublished == 0 && isMyCampaign && !isMainMenuCampaign)
		{
			text_favorited.text = times_favorited.ToString();
			text_downloaded.text = times_downloaded.ToString();
			text_authorname.text = AccountMaster.instance.Username;
			text_mapname.text = campaignName;
		}
		if (!isMyCampaign && !hasDownloaded)
		{
			ShowDownloadButton();
		}
	}

	public IEnumerator GetMapData()
	{
		NMC.GPM.isLoading = true;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignID", campaignID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/get_map_data.php", wWWForm);
		yield return uwr.SendWebRequest();
		NMC.GPM.isLoading = false;
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}
		string[] array = uwr.downloadHandler.text.Split(char.Parse(","));
		times_downloaded = int.Parse(array[0]);
		times_favorited = int.Parse(array[1]);
		text_favorited.text = times_favorited.ToString();
		text_downloaded.text = times_downloaded.ToString();
		text_authorname.text = array[2];
		if (int.Parse(array[6]) != 180 && int.Parse(array[6]) != 285 && int.Parse(array[6]) != 374)
		{
			int.Parse(array[6]);
			_ = 475;
		}
		text_mapname.text = array[3];
	}

	public IEnumerator UploadCampaign()
	{
		NMC.GPM.isLoading = true;
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		isUploading = false;
		if (keyRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			NMC.GPM.isLoading = false;
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			NMC.GPM.isLoading = false;
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		_ = Application.persistentDataPath + "/";
		string savePath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath2 = savePath2 + "/My Games/Wee Tanks/" + campaignName + ".campaign";
		Debug.Log(campaignName);
		Debug.Log(savePath2);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignName", campaignName);
		wWWForm.AddField("campaignVersion", campaignVersion);
		wWWForm.AddField("campaignID", campaignID);
		wWWForm.AddField("campaignSize", map_size);
		wWWForm.AddField("campaignMissions", amount_missions);
		wWWForm.AddField("campaignDifficulty", campaign_difficulty);
		if (isPublished > 0)
		{
			wWWForm.AddField("publish", 0);
		}
		else
		{
			wWWForm.AddField("publish", 1);
			wWWForm.AddBinaryData("file", File.ReadAllBytes(savePath2), campaignName + ".campaign", "text/plain");
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/add_map.php", wWWForm);
		yield return uwr.SendWebRequest();
		NMC.GPM.isLoading = false;
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			btn_online.isOn = !btn_online.isOn;
			yield break;
		}
		if (uwr.downloadHandler.text.Contains("CANCLED"))
		{
			MapEditorData mapEditorData = SavingMapEditorData.LoadData(campaignName);
			if (mapEditorData != null)
			{
				mapEditorData.isPublished = 0;
				isPublished = 0;
				SavingMapEditorData.ReSaveMap(mapEditorData, savePath2);
				btn_online.isOn = false;
			}
			yield break;
		}
		MapEditorData mapEditorData2 = SavingMapEditorData.LoadData(campaignName);
		if (mapEditorData2 != null)
		{
			Debug.Log(mapEditorData2.VersionCreated);
			Debug.Log(int.Parse(uwr.downloadHandler.text));
			mapEditorData2.PID = int.Parse(uwr.downloadHandler.text);
			campaignID = mapEditorData2.PID;
			mapEditorData2.isPublished = 1;
			isPublished = 1;
			SavingMapEditorData.ReSaveMap(mapEditorData2, savePath2);
			btn_online.isOn = true;
		}
	}

	public IEnumerator DownloadCampaign()
	{
		NMC.GPM.isLoading = true;
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		isUploading = false;
		if (keyRequest.result != UnityWebRequest.Result.Success)
		{
			isDownloading = false;
			NMC.GPM.isLoading = false;
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			isDownloading = false;
			NMC.GPM.isLoading = false;
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		_ = Application.persistentDataPath + "/";
		string savePath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath2 += "/My Games/Wee Tanks/downloads";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignID", campaignID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/download_map.php", wWWForm);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			isDownloading = false;
			NMC.GPM.isLoading = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		isDownloading = false;
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			NMC.GPM.isLoading = false;
		}
		else if (uwr.downloadHandler.text.Contains("SUCCES"))
		{
			isDownloading = false;
			if (file_url != null)
			{
				string url = "https://weetanks.com/" + file_url;
				UnityWebRequest uwr3 = new UnityWebRequest(url, "GET");
				string pattern = "(.*)\\/(.*)\\.";
				string[] array = Regex.Split(file_url, pattern);
				string path = $"{savePath2}/{array[2]}.campaign";
				uwr3.downloadHandler = new DownloadHandlerFile(path);
				yield return uwr3.SendWebRequest();
				NMC.GPM.isLoading = false;
				if (uwr3.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError(uwr3.error);
				}
				else
				{
					HideDownloadButton();
				}
			}
		}
		else if (uwr.downloadHandler.text.Contains("REDOWNLOAD"))
		{
			isDownloading = false;
			if (file_url != null)
			{
				string url2 = "https://weetanks.com/" + file_url;
				UnityWebRequest uwr3 = new UnityWebRequest(url2, "GET");
				string pattern2 = "(.*)\\/(.*)\\.";
				string[] array2 = Regex.Split(file_url, pattern2);
				string path2 = $"{savePath2}/{array2[2]}.campaign";
				uwr3.downloadHandler = new DownloadHandlerFile(path2);
				yield return uwr3.SendWebRequest();
				NMC.GPM.isLoading = false;
				if (uwr3.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError(uwr3.error);
				}
				else
				{
					HideDownloadButton();
				}
			}
		}
		else if (uwr.downloadHandler.text.Contains("REMOVED"))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public IEnumerator FavoriteCampaign()
	{
		NMC.GPM.isLoading = true;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignID", campaignID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/favorite_map.php", wWWForm);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			isDownloading = false;
			NMC.GPM.isLoading = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		isDownloading = false;
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			NMC.GPM.isLoading = false;
		}
		else if (uwr.downloadHandler.text.Contains("SUCCES"))
		{
			isDownloading = false;
			NMC.GPM.isLoading = false;
			NMC.GPM.RetrieveMaps();
		}
		else
		{
			NMC.GPM.isLoading = false;
		}
	}

	public IEnumerator RemovePublicCampaign()
	{
		NMC.GPM.isLoading = true;
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.result != UnityWebRequest.Result.Success)
		{
			isDownloading = false;
			NMC.GPM.isLoading = false;
			Debug.Log("Error While Sending: " + keyRequest.error);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			Debug.Log("WAIT");
			NMC.GPM.isLoading = false;
			yield break;
		}
		string text = keyRequest.downloadHandler.text;
		_ = Application.persistentDataPath + "/";
		string savePath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		savePath2 += "/My Games/Wee Tanks/downloads";
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("authKey", text);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("campaignID", campaignID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/remove_map_download.php", wWWForm);
		yield return uwr.SendWebRequest();
		if (uwr.result != UnityWebRequest.Result.Success)
		{
			NMC.GPM.isLoading = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			NMC.GPM.isLoading = false;
		}
		else if (uwr.downloadHandler.text.Contains("SUCCES"))
		{
			if (file_url != null)
			{
				string pattern = "(.*)\\/(.*)\\.";
				string[] array = Regex.Split(file_url, pattern);
				Debug.Log(savePath2 + "/" + array[2] + ".campaign");
				string path = savePath2 + "/" + array[2] + ".campaign";
				hasDownloaded = false;
				ShowDownloadButton();
				NMC.GPM.isLoading = false;
				File.Delete(path);
			}
		}
		else
		{
			if (!uwr.downloadHandler.text.Contains("REDOWNLOAD"))
			{
				yield break;
			}
			isDownloading = false;
			if (file_url != null)
			{
				string url = "https://weetanks.com/" + file_url;
				UnityWebRequest uwr2 = new UnityWebRequest(url, "GET");
				string pattern2 = "(.*)\\/(.*)\\.";
				string[] array2 = Regex.Split(file_url, pattern2);
				string path2 = $"{savePath2}/{array2[2]}.campaign";
				uwr2.downloadHandler = new DownloadHandlerFile(path2);
				yield return uwr2.SendWebRequest();
				NMC.GPM.isLoading = false;
				if (uwr2.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError(uwr2.error);
				}
				else
				{
					HideDownloadButton();
				}
			}
		}
	}
}
