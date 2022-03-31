using System.Collections;
using System.Linq;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopStand : MonoBehaviour
{
	public TankeyTownStock MyStandItem;

	[Header("Texts")]
	public TextMeshProUGUI ItemTitle;

	public TextMeshProUGUI ItemDescription;

	public TextMeshProUGUI ItemRare;

	public TextMeshProUGUI ItemStock;

	public TextMeshProUGUI ItemPrice;

	public TextMeshProUGUI BuyNote;

	[Header("Images and Icons")]
	public RawImage IconRare;

	public RawImage IconStock;

	public RawImage IconPrice;

	public RawImage BodyBorder;

	public RawImage BodyCaret;

	public RawImage CheckMark;

	[Header("Audio Clips")]
	public AudioClip PlayerNearSound;

	public AudioClip PlayerAwaySound;

	public AudioClip ErrorSound;

	public AudioClip BoughtSound;

	[Header("Settings")]
	public float BaseWidth = 83f;

	public float BaseHeight = 83f;

	public float PercentChangeX = 1f;

	public float PercentChangeY = 1f;

	public Color[] RareColor;

	public RectTransform[] Boxes;

	public HorizontalLayoutGroup HLGIcons;

	public bool PlayerNearMe = false;

	public Animator MyAnimator;

	public Animator MyAnimatorObject;

	public GameObject NoItemPrefab;

	public GameObject MySpawnedItem;

	public Transform PlaceToSpawnObject;

	public bool PlayerBoughtMeAlready = false;

	private void Start()
	{
		MyAnimator.transform.position += new Vector3(0f, 0f, -3f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			PlayerNearMe = true;
			MyAnimator.SetBool("PlayerNear", value: true);
			MyAnimatorObject.SetBool("PlayerNear", value: true);
			SFXManager.instance.PlaySFX(PlayerNearSound, 0.8f, null);
			TankeyTownMaster.instance.WalletAnimator.SetBool("ShowWallet", value: true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			PlayerNearMe = false;
			MyAnimator.SetBool("PlayerNear", value: false);
			MyAnimatorObject.SetBool("PlayerNear", value: false);
			SFXManager.instance.PlaySFX(PlayerAwaySound, 0.8f, null);
			TankeyTownMaster.instance.WalletAnimator.SetBool("ShowWallet", value: false);
		}
	}

	private void Update()
	{
		if (!PlayerNearMe)
		{
			return;
		}
		if (ReInput.players.GetPlayer(0).GetButtonDown("Use") && !PlayerBoughtMeAlready)
		{
			if (AccountMaster.instance.IsDoingTransaction || !AccountMaster.instance.isSignedIn)
			{
				SFXManager.instance.PlaySFX(ErrorSound, 0.8f, null);
			}
			else if (AccountMaster.instance.PDO.marbles >= MyStandItem.ItemPrice)
			{
				if (AccountMaster.instance.Inventory.InventoryItems == null)
				{
					AccountMaster.instance.IsDoingTransaction = true;
					AccountMaster.instance.ConnectedStand = this;
					AccountMaster.instance.StartCoroutine(AccountMaster.instance.BuyTankeyTownItem(MyStandItem));
				}
				else if (AccountMaster.instance.Inventory.InventoryItems.Length == 0)
				{
					AccountMaster.instance.IsDoingTransaction = true;
					AccountMaster.instance.ConnectedStand = this;
					AccountMaster.instance.StartCoroutine(AccountMaster.instance.BuyTankeyTownItem(MyStandItem));
				}
				else if (!AccountMaster.instance.Inventory.InventoryItems.Contains(MyStandItem.ItemID))
				{
					AccountMaster.instance.IsDoingTransaction = true;
					AccountMaster.instance.ConnectedStand = this;
					AccountMaster.instance.StartCoroutine(AccountMaster.instance.BuyTankeyTownItem(MyStandItem));
				}
				else
				{
					SFXManager.instance.PlaySFX(ErrorSound, 0.8f, null);
				}
			}
			else
			{
				SFXManager.instance.PlaySFX(ErrorSound, 0.8f, null);
			}
		}
		else
		{
			SFXManager.instance.PlaySFX(ErrorSound, 0.8f, null);
		}
	}

	public void TransactionSucces()
	{
		SFXManager.instance.PlaySFX(BoughtSound, 0.8f, null);
		StartCoroutine(ResetStock());
	}

	private IEnumerator ResetStock()
	{
		TankeyTownMaster.instance.StartCoroutine(TankeyTownMaster.instance.GetLatestTankeyTownStock(IsUpdate: true));
		yield return new WaitForSeconds(1.5f);
		AccountMaster.instance.IsDoingTransaction = false;
	}

	public void TransactionFailed()
	{
		StartCoroutine(ResetStock());
		SFXManager.instance.PlaySFX(ErrorSound, 0.8f, null);
	}

	private IEnumerator SetSizes()
	{
		yield return new WaitForSeconds(1f);
		Vector2 SizeOfTitle = ItemTitle.GetRenderedValues(onlyVisibleCharacters: true);
		RectTransform rt = ItemTitle.GetComponent(typeof(RectTransform)) as RectTransform;
		RectTransform rtDesc = ItemDescription.GetComponent(typeof(RectTransform)) as RectTransform;
		PercentChangeX = SizeOfTitle.x / BaseWidth;
		if (PercentChangeX <= 1f)
		{
			PercentChangeX = 1f;
			SizeOfTitle.x = BaseWidth;
		}
		rt.sizeDelta = new Vector2(SizeOfTitle.x, rt.sizeDelta.y);
		rtDesc.sizeDelta = new Vector2(SizeOfTitle.x, rtDesc.sizeDelta.y);
		yield return new WaitForSeconds(0.1f);
		Vector2 SizeOfDescription = ItemDescription.GetRenderedValues(onlyVisibleCharacters: true);
		PercentChangeY = SizeOfDescription.y / BaseHeight;
		PercentChangeY -= 0.1f;
		Vector2 vector = SizeOfDescription;
		Debug.Log("size of description: " + vector.ToString());
		if (PercentChangeY <= 1f)
		{
			PercentChangeY = 1f;
			SizeOfDescription.y = BaseHeight;
		}
		else
		{
			HLGIcons.padding.top = 3;
		}
		Debug.Log("setting size, with new width of: " + SizeOfTitle.x);
		for (int i = 0; i < Boxes.Length; i++)
		{
			RectTransform rt2 = Boxes[i].GetComponent(typeof(RectTransform)) as RectTransform;
			rt2.sizeDelta = new Vector2(rt2.sizeDelta.x * PercentChangeX, rt2.sizeDelta.y * PercentChangeY);
		}
		rtDesc.sizeDelta = new Vector2(SizeOfTitle.x, SizeOfDescription.y);
	}

	public void SetItemOnDisplay(bool IsUpdate)
	{
		ItemTitle.text = MyStandItem.ItemName;
		ItemDescription.text = MyStandItem.ItemDescription;
		string rarename = ((MyStandItem.ItemRare == 0) ? "Very Common" : ((MyStandItem.ItemRare == 1) ? "Common" : ((MyStandItem.ItemRare == 2) ? "Rare" : ((MyStandItem.ItemRare == 3) ? "Very Rare" : ((MyStandItem.ItemRare == 4) ? "Super Rare" : "Legendary")))));
		ItemRare.text = rarename;
		ItemStock.text = MyStandItem.AmountInStock + " <font-weight=700><color=#A0A0A0>in stock</color></font-weight>";
		ItemPrice.text = MyStandItem.ItemPrice + " <font-weight=700><color=#A0A0A0>marbles</color></font-weight>";
		Color BoxColor = RareColor[MyStandItem.ItemRare];
		if (AccountMaster.instance.Inventory.InventoryItems != null)
		{
			if (AccountMaster.instance.Inventory.InventoryItems.Contains(MyStandItem.ItemID))
			{
				PlayerBoughtMeAlready = true;
				BuyNote.text = "You already have the " + MyStandItem.ItemName;
				CheckMark.gameObject.SetActive(value: true);
				CheckMark.color = BoxColor;
			}
			else
			{
				BuyNote.text = "Press " + ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy:";
				PlayerBoughtMeAlready = false;
				CheckMark.gameObject.SetActive(value: false);
				CheckMark.color = BoxColor;
			}
		}
		else
		{
			BuyNote.text = "Press " + ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy:";
			PlayerBoughtMeAlready = false;
			CheckMark.gameObject.SetActive(value: false);
		}
		ItemRare.color = BoxColor;
		IconRare.color = BoxColor;
		BodyBorder.color = BoxColor;
		BodyCaret.color = BoxColor;
		GameObject MyStockItem = null;
		float height = 0f;
		for (int i = 0; i < GlobalAssets.instance.StockDatabase.Count; i++)
		{
			if (GlobalAssets.instance.StockDatabase[i].ItemID == MyStandItem.ItemID)
			{
				if (GlobalAssets.instance.StockDatabase[i].ItemObject != null)
				{
					MyStockItem = GlobalAssets.instance.StockDatabase[i].ItemObject;
					height = GlobalAssets.instance.StockDatabase[i].ItemYoffset;
				}
				else
				{
					MyStockItem = NoItemPrefab;
					height = 1f;
				}
			}
		}
		if (MySpawnedItem != null && IsUpdate && MySpawnedItem != MyStockItem)
		{
			Object.Destroy(MySpawnedItem);
		}
		if (MySpawnedItem == null)
		{
			MySpawnedItem = Object.Instantiate(MyStockItem, PlaceToSpawnObject.position, base.transform.rotation, PlaceToSpawnObject);
			MySpawnedItem.transform.position += new Vector3(0f, height, 0f);
		}
		if (!IsUpdate)
		{
			StartCoroutine(SetSizes());
		}
	}
}
