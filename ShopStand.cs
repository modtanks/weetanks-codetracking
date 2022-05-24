using System.Collections;
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

	public bool PlayerNearMe;

	public Animator MyAnimator;

	public Animator MyAnimatorObject;

	public GameObject NoItemPrefab;

	public GameObject MySpawnedItem;

	public Transform PlaceToSpawnObject;

	public bool PlayerBoughtMeAlready;

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
				else if (AccountMaster.instance.Inventory.InventoryItems.Count <= 0)
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
		else if (ReInput.players.GetPlayer(0).GetButtonDown("Use") && PlayerBoughtMeAlready)
		{
			SFXManager.instance.PlaySFX(ErrorSound, 0.8f, null);
		}
	}

	public void TransactionSucces()
	{
		SFXManager.instance.PlaySFX(BoughtSound, 0.8f, null);
		TutorialMaster.instance.ShowTutorial("Item bought succesfully!");
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
		RectTransform rectTransform = ItemTitle.GetComponent(typeof(RectTransform)) as RectTransform;
		RectTransform rtDesc = ItemDescription.GetComponent(typeof(RectTransform)) as RectTransform;
		PercentChangeX = SizeOfTitle.x / BaseWidth;
		if (PercentChangeX <= 1f)
		{
			PercentChangeX = 1f;
			SizeOfTitle.x = BaseWidth;
		}
		rectTransform.sizeDelta = new Vector2(SizeOfTitle.x, rectTransform.sizeDelta.y);
		rtDesc.sizeDelta = new Vector2(SizeOfTitle.x, rtDesc.sizeDelta.y);
		yield return new WaitForSeconds(0.1f);
		Vector2 renderedValues = ItemDescription.GetRenderedValues(onlyVisibleCharacters: true);
		PercentChangeY = renderedValues.y / BaseHeight;
		PercentChangeY -= 0.1f;
		if (PercentChangeY <= 1f)
		{
			PercentChangeY = 1f;
			renderedValues.y = BaseHeight;
		}
		else
		{
			HLGIcons.padding.top = 3;
		}
		for (int i = 0; i < Boxes.Length; i++)
		{
			RectTransform rectTransform2 = Boxes[i].GetComponent(typeof(RectTransform)) as RectTransform;
			rectTransform2.sizeDelta = new Vector2(rectTransform2.sizeDelta.x * PercentChangeX, rectTransform2.sizeDelta.y * PercentChangeY);
		}
		rtDesc.sizeDelta = new Vector2(SizeOfTitle.x, renderedValues.y);
	}

	public void SetItemOnDisplay(bool IsUpdate)
	{
		ItemTitle.text = MyStandItem.ItemName;
		ItemDescription.text = MyStandItem.ItemDescription;
		string text = ((MyStandItem.ItemRare == 0) ? "Very Common" : ((MyStandItem.ItemRare == 1) ? "Common" : ((MyStandItem.ItemRare == 2) ? "Rare" : ((MyStandItem.ItemRare == 3) ? "Very Rare" : ((MyStandItem.ItemRare == 4) ? "Super Rare" : "Legendary")))));
		ItemRare.text = text;
		if (MyStandItem.AmountInStock <= 0)
		{
			ItemStock.text = "<color=#FF0000>" + MyStandItem.AmountInStock + "</color> <font-weight=700><color=#A0A0A0>in stock</color></font-weight>";
			float num = Mathf.Floor((float)MyStandItem.ItemPrice * 0.5f);
			ItemPrice.text = "<color=#FF0000>" + MyStandItem.ItemPrice + " (+" + num + ")</color>";
		}
		else
		{
			ItemStock.text = MyStandItem.AmountInStock + " <font-weight=700><color=#A0A0A0>in stock</color></font-weight>";
			ItemPrice.text = MyStandItem.ItemPrice + " <font-weight=700><color=#A0A0A0>marbles</color></font-weight>";
		}
		Color color = RareColor[MyStandItem.ItemRare];
		if (AccountMaster.instance.Inventory.InventoryItems != null)
		{
			if (AccountMaster.instance.Inventory.InventoryItems.Contains(MyStandItem.ItemID))
			{
				PlayerBoughtMeAlready = true;
				BuyNote.text = "You already own this";
				CheckMark.gameObject.SetActive(value: true);
				CheckMark.color = color;
			}
			else
			{
				BuyNote.text = "Press " + ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy:";
				PlayerBoughtMeAlready = false;
				CheckMark.gameObject.SetActive(value: false);
				CheckMark.color = color;
			}
		}
		else
		{
			BuyNote.text = "Press " + ReInput.players.GetPlayer(0).controllers.maps.GetFirstButtonMapWithAction("Use", skipDisabledMaps: true).elementIdentifierName + " to buy:";
			PlayerBoughtMeAlready = false;
			CheckMark.gameObject.SetActive(value: false);
		}
		ItemRare.color = color;
		IconRare.color = color;
		BodyBorder.color = color;
		BodyCaret.color = color;
		GameObject gameObject = null;
		float y = 0f;
		for (int i = 0; i < GlobalAssets.instance.StockDatabase.Count; i++)
		{
			if (GlobalAssets.instance.StockDatabase[i].ItemID == MyStandItem.ItemID)
			{
				if (GlobalAssets.instance.StockDatabase[i].ItemObject != null)
				{
					gameObject = GlobalAssets.instance.StockDatabase[i].ItemObject;
					y = GlobalAssets.instance.StockDatabase[i].ItemYoffset;
				}
				else
				{
					gameObject = NoItemPrefab;
					y = 1f;
				}
			}
		}
		if (MySpawnedItem != null && IsUpdate && MySpawnedItem != gameObject)
		{
			Object.Destroy(MySpawnedItem);
			MySpawnedItem = null;
		}
		if (MySpawnedItem == null)
		{
			MySpawnedItem = Object.Instantiate(gameObject, PlaceToSpawnObject.position, base.transform.rotation, PlaceToSpawnObject);
			MySpawnedItem.transform.position += new Vector3(0f, y, 0f);
		}
		if (!IsUpdate)
		{
			StartCoroutine(SetSizes());
		}
	}
}
