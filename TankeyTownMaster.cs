using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TankeyTownMaster : MonoBehaviour
{
	private static TankeyTownMaster _instance;

	public ShopScript[] Shops;

	public List<TankeyTownShopData> TTS_Shops = new List<TankeyTownShopData>();

	public TextMeshProUGUI MarblesText;

	public Animator WalletAnimator;

	public static TankeyTownMaster instance => _instance;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	private void Start()
	{
		StartCoroutine(GetLatestTankeyTownStock(IsUpdate: false));
		if ((bool)AccountMaster.instance && AccountMaster.instance.isSignedIn)
		{
			MarblesText.text = AccountMaster.instance.PDO.marbles.ToString();
			AccountMaster.instance.StartCoroutine(AccountMaster.instance.GetCloudInventory());
		}
		StartCoroutine(UpdateStockRepeater());
	}

	private IEnumerator UpdateStockRepeater()
	{
		yield return new WaitForSeconds(4f);
		StartCoroutine(GetLatestTankeyTownStock(IsUpdate: true));
		StartCoroutine(UpdateStockRepeater());
	}

	public void AssignDataToShops(bool IsUpdate)
	{
		for (int i = 0; i < Shops.Length; i++)
		{
			if (Shops[i].MyStands.Length != 0 && i != 3)
			{
				for (int j = 0; j < Shops[i].MyStands.Length; j++)
				{
					Shops[i].MyStands[j].MyStandItem = TTS_Shops[i].ShopData[j];
					Shops[i].MyStands[j].SetItemOnDisplay(IsUpdate);
				}
			}
		}
	}

	public IEnumerator GetLatestTankeyTownStock(bool IsUpdate)
	{
		WWWForm form = new WWWForm();
		form.AddField("userid", AccountMaster.instance.UserID);
		form.AddField("key", AccountMaster.instance.Key);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/get_tankey_town_stock.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			SceneManager.LoadScene(0);
			yield break;
		}
		string[] splitArray = uwr.downloadHandler.text.Split(char.Parse("/"));
		TankeyTownShopData TTS1 = JsonUtility.FromJson<TankeyTownShopData>("{\"ShopData\":" + splitArray[0] + "}");
		TankeyTownShopData TTS2 = JsonUtility.FromJson<TankeyTownShopData>("{\"ShopData\":" + splitArray[1] + "}");
		TankeyTownShopData TTS3 = JsonUtility.FromJson<TankeyTownShopData>("{\"ShopData\":" + splitArray[2] + "}");
		TTS_Shops.Clear();
		TTS_Shops.Add(TTS1);
		TTS_Shops.Add(TTS2);
		TTS_Shops.Add(TTS3);
		AssignDataToShops(IsUpdate);
	}
}
