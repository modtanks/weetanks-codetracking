using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GetPublicMaps : MonoBehaviour
{
	public GameObject PublicMapPrefab;

	public GameObject PublicMapParent;

	public NewMenuControl NMC;

	public TextMeshProUGUI PageNumber;

	public int CurrentPage = 0;

	public int PageType = 0;

	public TMP_Dropdown SortDropdown;

	public GameObject LoadingScreen;

	public bool isLoading = false;

	private void Start()
	{
		RetrieveMaps();
	}

	private void Update()
	{
		if (isLoading)
		{
			LoadingScreen.SetActive(value: true);
		}
		else
		{
			LoadingScreen.SetActive(value: false);
		}
	}

	public void SetPageType(int type)
	{
		PageType = type;
		if (!isLoading)
		{
			RetrieveMaps();
		}
	}

	public void RetrieveMaps()
	{
		StartCoroutine(GetMaps());
	}

	public void PrevPage()
	{
		if (CurrentPage > 0)
		{
			CurrentPage--;
			PageNumber.text = (CurrentPage + 1).ToString();
			StartCoroutine(GetMaps());
		}
	}

	public void NextPage()
	{
		CurrentPage++;
		PageNumber.text = (CurrentPage + 1).ToString();
		StartCoroutine(GetMaps());
	}

	private IEnumerator GetMaps()
	{
		if (!AccountMaster.instance.isSignedIn)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("key", AccountMaster.instance.Key);
		form.AddField("userid", AccountMaster.instance.UserID);
		form.AddField("pagenumber", CurrentPage);
		form.AddField("sort", SortDropdown.value);
		form.AddField("type", PageType);
		isLoading = true;
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/get_maps.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		isLoading = false;
		if (uwr.isNetworkError)
		{
			isLoading = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			yield break;
		}
		JSONNode N = JSON.Parse(uwr.downloadHandler.text);
		if (PublicMapParent.transform.childCount > 0)
		{
			foreach (Transform child in PublicMapParent.transform)
			{
				UnityEngine.Object.Destroy(child.gameObject);
			}
		}
		for (int i = 0; i < N.Count; i++)
		{
			GameObject MapPrefab = UnityEngine.Object.Instantiate(PublicMapPrefab, base.transform.position, Quaternion.identity, PublicMapParent.transform);
			CampaignItemScript CIS = MapPrefab.GetComponent<CampaignItemScript>();
			string missionAmount = (((int)N[i]["amount_missions"] > 1) ? " missions" : " mission");
			string mapName = ((int.Parse(N[i]["map_size"]) == 180) ? "small" : ((int.Parse(N[i]["map_size"]) == 285) ? "normal" : ((int.Parse(N[i]["map_size"]) == 374) ? "big" : ((int.Parse(N[i]["map_size"]) == 475) ? "large" : "size"))));
			CIS.text_mapname.text = string.Concat(N[i]["mapname"], " (", N[i]["amount_missions"], missionAmount, ", ", mapName, ")");
			CIS.campaignName = string.Concat(N[i]["mapname"], " (", N[i]["amount_missions"], missionAmount, ", ", mapName, ")");
			CIS.text_version.text = ((N[i]["version"] == (object)OptionsMainMenu.instance.CurrentVersion) ? ((string)N[i]["version"]) : string.Concat("<color=red>(!) ", N[i]["version"], "</color>"));
			CIS.campaignVersion = N[i]["version"];
			CIS.text_authorname.text = N[i]["username"];
			CIS.campaignAuthor = N[i]["username"];
			CIS.campaignID = N[i]["ID"];
			string difficulty = ((int.Parse(N[i]["difficulty"]) == 0) ? "Toddler" : ((int.Parse(N[i]["difficulty"]) == 1) ? "Kid" : ((int.Parse(N[i]["difficulty"]) == 2) ? "Adult" : "Grandpa")));
			CIS.text_difficulty.text = difficulty;
			CIS.text_downloaded.text = N[i]["times_downloaded"];
			CIS.times_downloaded = int.Parse(N[i]["times_downloaded"]);
			CIS.text_favorited.text = N[i]["times_favorited"];
			CIS.times_favorited = N[i]["times_favorited"];
			CIS.NMC = NMC;
			CIS.file_url = N[i]["file_url"];
			CIS.map_size = N[i]["map_size"];
			if (N[i]["has_favorited"] == (object)"true")
			{
				CIS.FavoriteHolder.texture = CIS.Tex_Favorite;
			}
			else
			{
				CIS.FavoriteHolder.texture = CIS.Tex_NoFavorite;
			}
			string pattern = "(.*)\\/(.*)\\.";
			string[] names = Regex.Split(CIS.file_url, pattern);
			_ = Application.persistentDataPath + "/";
			string savePath2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
			savePath2 = savePath2 + "/My Games/Wee Tanks/downloads/" + names[2] + ".campaign";
			CIS.coded_file_name = names[2];
			Debug.Log(savePath2);
			if (File.Exists(savePath2))
			{
				CIS.hasDownloaded = true;
				CIS.HideDownloadButton();
			}
		}
	}
}
