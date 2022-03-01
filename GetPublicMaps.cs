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

	public int CurrentPage;

	public int PageType;

	public TMP_Dropdown SortDropdown;

	public GameObject LoadingScreen;

	public bool isLoading;

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
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("pagenumber", CurrentPage);
		wWWForm.AddField("sort", SortDropdown.value);
		wWWForm.AddField("type", PageType);
		isLoading = true;
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/get_maps.php", wWWForm);
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
		JSONNode jSONNode = JSON.Parse(uwr.downloadHandler.text);
		if (PublicMapParent.transform.childCount > 0)
		{
			foreach (Transform item in PublicMapParent.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		for (int i = 0; i < jSONNode.Count; i++)
		{
			CampaignItemScript component = UnityEngine.Object.Instantiate(PublicMapPrefab, base.transform.position, Quaternion.identity, PublicMapParent.transform).GetComponent<CampaignItemScript>();
			string text = (((int)jSONNode[i]["amount_missions"] > 1) ? " missions" : " mission");
			string text2 = ((int.Parse(jSONNode[i]["map_size"]) == 180) ? "small" : ((int.Parse(jSONNode[i]["map_size"]) == 285) ? "normal" : ((int.Parse(jSONNode[i]["map_size"]) == 374) ? "big" : ((int.Parse(jSONNode[i]["map_size"]) == 475) ? "large" : "size"))));
			component.text_mapname.text = string.Concat(jSONNode[i]["mapname"], " (", jSONNode[i]["amount_missions"], text, ", ", text2, ")");
			component.campaignName = string.Concat(jSONNode[i]["mapname"], " (", jSONNode[i]["amount_missions"], text, ", ", text2, ")");
			component.text_version.text = ((jSONNode[i]["version"] == (object)OptionsMainMenu.instance.CurrentVersion) ? ((string)jSONNode[i]["version"]) : string.Concat("<color=red>(!) ", jSONNode[i]["version"], "</color>"));
			component.campaignVersion = jSONNode[i]["version"];
			component.text_authorname.text = jSONNode[i]["username"];
			component.campaignAuthor = jSONNode[i]["username"];
			component.campaignID = jSONNode[i]["ID"];
			string text3 = ((int.Parse(jSONNode[i]["difficulty"]) == 0) ? "Toddler" : ((int.Parse(jSONNode[i]["difficulty"]) == 1) ? "Kid" : ((int.Parse(jSONNode[i]["difficulty"]) == 2) ? "Adult" : "Grandpa")));
			component.text_difficulty.text = text3;
			component.text_downloaded.text = jSONNode[i]["times_downloaded"];
			component.times_downloaded = int.Parse(jSONNode[i]["times_downloaded"]);
			component.text_favorited.text = jSONNode[i]["times_favorited"];
			component.times_favorited = jSONNode[i]["times_favorited"];
			component.NMC = NMC;
			component.file_url = jSONNode[i]["file_url"];
			component.map_size = jSONNode[i]["map_size"];
			if (jSONNode[i]["has_favorited"] == (object)"true")
			{
				component.FavoriteHolder.texture = component.Tex_Favorite;
			}
			else
			{
				component.FavoriteHolder.texture = component.Tex_NoFavorite;
			}
			string pattern = "(.*)\\/(.*)\\.";
			string[] array = Regex.Split(component.file_url, pattern);
			_ = Application.persistentDataPath + "/";
			string text4 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/My Games/Wee Tanks/downloads/" + array[2] + ".campaign";
			component.coded_file_name = array[2];
			Debug.Log(text4);
			if (File.Exists(text4))
			{
				component.hasDownloaded = true;
				component.HideDownloadButton();
			}
		}
	}
}
