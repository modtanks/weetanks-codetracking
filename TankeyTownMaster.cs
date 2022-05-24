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

	public PauseMenuScript PMS;

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
		if (GameMaster.instance.Players.Count < 1)
		{
			PMS.gameObject.SetActive(value: true);
			PMS.StartCoroutine(PMS.LoadYourAsyncScene(0));
		}
		else
		{
			Debug.Log("UPDATING STOCK...");
			StartCoroutine(GetLatestTankeyTownStock(IsUpdate: true));
			StartCoroutine(UpdateStockRepeater());
		}
	}

	public void AssignDataToShops(bool IsUpdate)
	{
		for (int i = 0; i < Shops.Length; i++)
		{
			if (Shops[i].MyStands.Length != 0 && i != 3 && i != 2 && i != 1 && TTS_Shops.Count >= i)
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
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/get_tankey_town_stock.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (!uwr.isNetworkError)
		{
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				TutorialMaster.instance.ShowTutorial("Getting stock failed... going back to main menu!");
				uwr.Dispose();
				yield return new WaitForSeconds(2f);
				SceneManager.LoadScene(0);
			}
			else
			{
				string[] array = uwr.downloadHandler.text.Split(char.Parse("/"));
				TTS_Shops.Clear();
				if (array.Length != 0)
				{
					TankeyTownShopData item = JsonUtility.FromJson<TankeyTownShopData>("{\"ShopData\":" + array[0] + "}");
					TTS_Shops.Add(item);
				}
				if (array.Length > 1)
				{
					TankeyTownShopData item2 = JsonUtility.FromJson<TankeyTownShopData>("{\"ShopData\":" + array[1] + "}");
					TTS_Shops.Add(item2);
				}
				if (array.Length > 2)
				{
					TankeyTownShopData item3 = JsonUtility.FromJson<TankeyTownShopData>("{\"ShopData\":" + array[2] + "}");
					TTS_Shops.Add(item3);
				}
				AssignDataToShops(IsUpdate);
			}
		}
		uwr.Dispose();
	}
}
