using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AccountMaster : MonoBehaviour
{
	public string Username;

	public string Key;

	public string UserID;

	public ulong SteamUserID;

	public bool isSignedIn = false;

	public bool hasDoneSignInCheck = false;

	public int TimePlayed = 0;

	private static AccountMaster _instance;

	public bool isInLobby = false;

	public string LobbyCode;

	public int LobbyID;

	public ProgressDataOnline PDO;

	public GameObject MarblesNotification;

	private SteamTest ST;

	public PlayerInventory Inventory = new PlayerInventory();

	public bool IsDoingTransaction = false;

	public string PreviousCode;

	private bool SetTime = false;

	private bool isSaving = false;

	public ShopStand ConnectedStand;

	public static AccountMaster instance => _instance;

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
		ST = GetComponent<SteamTest>();
	}

	private void Start()
	{
		LoadCredentials();
		InvokeRepeating("Count", 10f, 10f);
		StartCoroutine(GetDelayedInventory());
	}

	private IEnumerator GetDelayedInventory()
	{
		yield return new WaitForSeconds(2f);
		StartCoroutine(GetCloudInventory());
	}

	private void Count()
	{
		if (isSignedIn && PDO != null)
		{
			TimePlayed += 10;
			PDO.TimePlayed += 10;
			SetTime = false;
			if (TimePlayed % 300 == 0)
			{
				SaveCloudData(-1, -1, 0, bounceKill: false);
			}
		}
	}

	private void Update()
	{
		if ((bool)AchievementsTracker.instance && isSignedIn && AchievementsTracker.instance.StartedFromBegin && !SetTime)
		{
			if (GameMaster.instance.CurrentMission == 20 && !GameMaster.instance.GameHasStarted && GameMaster.instance.Enemies.Count < 1)
			{
				SetTime = true;
				StartCoroutine(SaveSpeedrunData(20, OptionsMainMenu.instance.currentDifficulty));
			}
			else if (GameMaster.instance.CurrentMission == 50 && !GameMaster.instance.GameHasStarted && GameMaster.instance.Enemies.Count < 1)
			{
				SetTime = true;
				StartCoroutine(SaveSpeedrunData(50, OptionsMainMenu.instance.currentDifficulty));
			}
			else if (GameMaster.instance.CurrentMission == 100 && !GameMaster.instance.GameHasStarted)
			{
				SetTime = true;
				StartCoroutine(SaveSpeedrunData(100, OptionsMainMenu.instance.currentDifficulty));
			}
		}
	}

	public void ShowMarbleNotification(int amount)
	{
		GameObject Notification = Object.Instantiate(MarblesNotification);
		if (!Notification)
		{
			return;
		}
		TextMeshProUGUI text = Notification.transform.GetChild(0).Find("AMOUNT").GetComponent<TextMeshProUGUI>();
		Object.Destroy(Notification, 2.75f);
		if ((bool)text)
		{
			if (amount > 1)
			{
				text.text = "You got " + amount + " marbles!";
			}
			else
			{
				text.text = "You got " + amount + " marble!";
			}
		}
	}

	public void IncreaseMarbles(int amount)
	{
		if (isSignedIn)
		{
			PDO.marbles += amount;
			SaveCloudData(5, amount, 0, bounceKill: false);
			ShowMarbleNotification(amount);
		}
	}

	public IEnumerator SaveSpeedrunData(int level, int difficulty)
	{
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		form.AddField("userid", UserID);
		form.AddField("username", Username);
		PDO.accountid = int.Parse(UserID);
		PDO.accountname = Username;
		form.AddField("sT", Mathf.RoundToInt(GameMaster.instance.counter * 1000f));
		form.AddField("Lv", level);
		form.AddField("dF", difficulty);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_stats.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			isSaving = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			isSaving = false;
			yield break;
		}
		isSaving = false;
		AssignData(uwr.downloadHandler.text);
		Debug.Log("SAVED");
	}

	public void UpdateServerStatus(int code)
	{
		Debug.Log("sending status...");
		StartCoroutine(IESaveStatus(code));
	}

	public IEnumerator IESaveStatus(int code)
	{
		Debug.Log("sending status...2");
		if (!isSignedIn)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		form.AddField("userid", UserID);
		form.AddField("username", Username);
		form.AddField("code", code);
		form.AddField("difficulty", OptionsMainMenu.instance.currentDifficulty);
		if (PreviousCode != null)
		{
			form.AddField("prev", PreviousCode);
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_log.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		Debug.Log("sending status..!.333");
		if (uwr.isNetworkError)
		{
			isSaving = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			Debug.Log("UPDATING STATSU FAILED");
			yield break;
		}
		PreviousCode = uwr.downloadHandler.text;
		string[] splitArray = uwr.downloadHandler.text.Split(char.Parse("/"));
		int NewMarbles = int.Parse(splitArray[1]);
		if (NewMarbles > 0)
		{
			ShowMarbleNotification(NewMarbles);
		}
	}

	public void SaveCloudData(int type, int amount, int secondary_amount, bool bounceKill)
	{
		Debug.Log("SAVING CALLED " + type + amount);
		StartCoroutine(IESaveCloudData(type, amount, secondary_amount, bounceKill));
	}

	public IEnumerator IESaveCloudData(int type, int amount, int secondary_amount, bool bounceKill)
	{
		if (!isSignedIn || type == 5)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		form.AddField("userid", UserID);
		form.AddField("username", Username);
		form.AddField("pT", TimePlayed);
		switch (type)
		{
		case 0:
			form.AddField("kT", amount);
			break;
		case 1:
			form.AddField("gW", 1);
			break;
		case 2:
			form.AddField("gL", 1);
			break;
		case 3:
			form.AddField("A", amount);
			break;
		case 4:
			form.AddField("A_U", amount);
			if (secondary_amount > 0)
			{
				form.AddField("A_R", secondary_amount);
			}
			break;
		case 5:
			form.AddField("M", amount);
			break;
		case 6:
			form.AddField("W", amount);
			form.AddField("Lv", GameMaster.instance.CurrentMission);
			break;
		case 7:
			form.AddField("dF", OptionsMainMenu.instance.currentDifficulty);
			form.AddField("cP", GameMaster.instance.CurrentMission + 1);
			break;
		}
		if (bounceKill)
		{
			form.AddField("kB", 1);
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_stats_b.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			isSaving = false;
		}
		else if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			isSaving = false;
		}
		else if (uwr.downloadHandler.text.Contains("killed"))
		{
			isSaving = false;
			AssignData(uwr.downloadHandler.text);
		}
	}

	public IEnumerator SaveNewCloudData(ProgressDataOnline newPDO)
	{
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		form.AddField("userid", UserID);
		form.AddField("username", Username);
		newPDO.accountid = int.Parse(UserID);
		newPDO.accountname = Username;
		form.AddField("NewSaveData", JsonUtility.ToJson(newPDO));
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_stats.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			isSaving = false;
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			isSaving = false;
			yield break;
		}
		isSaving = false;
		AssignData(uwr.downloadHandler.text);
		Debug.Log("SAVED");
	}

	public void SignOut(bool manual)
	{
		Username = null;
		Key = null;
		UserID = null;
		SaveCredentials();
		isSignedIn = false;
		hasDoneSignInCheck = true;
		GameMaster.instance.AccountID = 0;
		if (manual)
		{
			string savePath = Application.persistentDataPath + "/tank_progress.tnk";
			if (File.Exists(savePath) && manual)
			{
				SavingData.SaveData(GameMaster.instance, "tank_progress_backup");
				File.Delete(savePath);
			}
			GameMaster.instance.CurrentData = new ProgressDataOnline();
			PDO = new ProgressDataOnline();
			for (int j = 0; j < OptionsMainMenu.instance.AM.Length; j++)
			{
				OptionsMainMenu.instance.AM[j] = 5;
			}
			for (int i = 0; i < OptionsMainMenu.instance.AMselected.Count; i++)
			{
				OptionsMainMenu.instance.AMselected[i] = 0;
			}
			GameMaster.instance.ResetData();
		}
	}

	public void LoadCredentials()
	{
		string savePath = Application.persistentDataPath + "/user_credentials.tnk";
		if (!File.Exists(savePath))
		{
			if (ST.SteamAccountID > 1000)
			{
				Debug.Log("STEAM ID IS SET");
				StartCoroutine(CreateAccountViaSteam("https://www.weetanks.com/create_steam_account.php", ST.username, ST.SteamAccountID, IsSignIn: false));
			}
			else
			{
				SignOut(manual: false);
			}
			return;
		}
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(savePath, FileMode.Open);
		UserCredentials UC = formatter.Deserialize(stream) as UserCredentials;
		stream.Close();
		if (UC == null)
		{
			return;
		}
		if (UC.hash != SystemInfo.deviceUniqueIdentifier)
		{
			SignOut(manual: false);
			return;
		}
		if (ST.SteamAccountID > 1000 && UC.steamid > 1000 && ST.SteamAccountID == UC.steamid)
		{
			Debug.Log("creating new account steam, or logging in?");
			StartCoroutine(CreateAccountViaSteam("https://www.weetanks.com/create_steam_account.php", ST.username, ST.SteamAccountID, IsSignIn: true));
			return;
		}
		Username = UC.name;
		Key = UC.key;
		Debug.Log("LOADING:" + UC.key);
		UserID = UC.id;
		if (UC.key == "" || UC.key == null)
		{
			Debug.Log("EM<PTY KEY: " + ST.SteamAccountID);
			if (ST.SteamAccountID > 1000)
			{
				Debug.Log("STEAM ID IS SET");
				StartCoroutine(CreateAccountViaSteam("https://www.weetanks.com/create_steam_account.php", ST.username, ST.SteamAccountID, IsSignIn: true));
				return;
			}
		}
		StartCoroutine(LoadCloudData());
		if (UC.steamid > 1000)
		{
			SteamUserID = UC.steamid;
		}
	}

	public IEnumerator CreateAccountViaSteam(string url, string steamusername, ulong steamuserid, bool IsSignIn)
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
		}
		else
		{
			if (keyRequest.downloadHandler.text == "WAIT")
			{
				yield break;
			}
			string receivedKey = keyRequest.downloadHandler.text;
			WWWForm form = new WWWForm();
			form.AddField("username", steamusername);
			form.AddField("steamuserid", steamuserid.ToString());
			form.AddField("authKey", receivedKey);
			form.AddField("userData", JsonUtility.ToJson(GameMaster.instance.CurrentData));
			UnityWebRequest uwr = UnityWebRequest.Post(url, form);
			uwr.SetRequestHeader("Access-Control-Allow-Credentials", "true");
			uwr.SetRequestHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
			uwr.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
			uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");
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
				GameMaster.instance.CurrentData.marbles = 0;
				GameMaster.instance.CurrentData.accountname = null;
				isSignedIn = false;
				SignOut(manual: false);
				yield break;
			}
			if (uwr.downloadHandler.text.Contains("EXISTS"))
			{
				GameMaster.instance.CurrentData.marbles = 0;
				GameMaster.instance.CurrentData.accountname = null;
				isSignedIn = false;
				yield break;
			}
			isSignedIn = true;
			string[] splitArray = uwr.downloadHandler.text.Split(char.Parse("/"));
			SteamUserID = steamuserid;
			UserID = splitArray[1];
			GameMaster.instance.AccountID = int.Parse(splitArray[1]);
			Key = splitArray[0];
			Username = splitArray[2];
			isSignedIn = true;
			SaveCredentials();
			if (splitArray.Length > 3 && splitArray[3] != null)
			{
				AssignData(splitArray[3]);
			}
		}
	}

	public IEnumerator TransferAccountToSteam(string url)
	{
		NewMenuControl NMC = GameObject.Find("CanvasMover").transform.GetChild(0).gameObject.GetComponent<NewMenuControl>();
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			if ((bool)NMC)
			{
				NMC.StartCoroutine(NMC.FailedTransferSteam(null));
			}
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			if ((bool)NMC)
			{
				NMC.StartCoroutine(NMC.FailedTransferSteam(null));
			}
			yield break;
		}
		string receivedKey = keyRequest.downloadHandler.text;
		WWWForm form = new WWWForm();
		form.AddField("username", Username);
		form.AddField("steamusername", SteamTest.instance.username);
		form.AddField("userid", UserID);
		form.AddField("key", Key);
		form.AddField("steamuserid", SteamTest.instance.SteamAccountID.ToString());
		form.AddField("authKey", receivedKey);
		UnityWebRequest uwr = UnityWebRequest.Post(url, form);
		uwr.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		uwr.SetRequestHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
		uwr.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
		uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			if ((bool)NMC)
			{
				NMC.StartCoroutine(NMC.FailedTransferSteam(null));
			}
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("already connected"))
		{
			if ((bool)NMC)
			{
				NMC.StartCoroutine(NMC.FailedTransferSteam(uwr.downloadHandler.text));
			}
			yield break;
		}
		if (uwr.downloadHandler.text.Contains("FAILED") || uwr.downloadHandler.text.Contains("false"))
		{
			if ((bool)NMC)
			{
				NMC.StartCoroutine(NMC.FailedTransferSteam(null));
			}
			yield break;
		}
		if (uwr.downloadHandler.text.Contains("EXISTS"))
		{
			if ((bool)NMC)
			{
				isSignedIn = true;
				string[] splitArray2 = uwr.downloadHandler.text.Split(char.Parse("/"));
				SteamUserID = SteamTest.instance.SteamAccountID;
				SaveCredentials();
				if (splitArray2.Length > 3 && splitArray2[3] != null)
				{
					AssignData(splitArray2[3]);
				}
				if ((bool)NMC)
				{
					NMC.StartCoroutine(NMC.SuccesTransferSteam());
				}
			}
			yield break;
		}
		isSignedIn = true;
		string[] splitArray = uwr.downloadHandler.text.Split(char.Parse("/"));
		SteamUserID = SteamTest.instance.SteamAccountID;
		UserID = splitArray[1];
		GameMaster.instance.AccountID = int.Parse(splitArray[1]);
		if (splitArray[0] != "null")
		{
			Key = splitArray[0];
		}
		Username = splitArray[2];
		isSignedIn = true;
		SaveCredentials();
		if (splitArray.Length > 3 && splitArray[3] != null)
		{
			AssignData(splitArray[3]);
		}
		if ((bool)NMC)
		{
			NMC.StartCoroutine(NMC.SuccesTransferSteam());
		}
	}

	public IEnumerator LoadCloudData()
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			SignOut(manual: false);
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			Debug.Log("TOO FAST ");
			SignOut(manual: false);
			yield break;
		}
		string receivedKey = keyRequest.downloadHandler.text;
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		Debug.Log("ITS:" + Key);
		form.AddField("authKey", receivedKey);
		form.AddField("userid", UserID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/check_account.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!(uwr.downloadHandler.text == "FAILED"))
			{
				if (uwr.downloadHandler.text.Contains("killed"))
				{
					AssignData(uwr.downloadHandler.text);
				}
				else
				{
					PDO = new ProgressDataOnline();
					SaveNewCloudData(PDO);
				}
				isSignedIn = true;
				if (SteamUserID < 1000 && ST.SteamAccountID > 1000)
				{
					NewMenuControl NMC = GameObject.Find("CanvasMover").transform.GetChild(0).gameObject.GetComponent<NewMenuControl>();
					if ((bool)NMC)
					{
						NMC.EnableTransferMenu();
					}
				}
				yield break;
			}
			SignOut(manual: false);
		}
		hasDoneSignInCheck = true;
	}

	public IEnumerator GetCloudInventory()
	{
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		Debug.Log("ITS:" + Key);
		form.AddField("userid", UserID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/get_inventory.php", form);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			Inventory = null;
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!uwr.downloadHandler.text.Contains("FAILED"))
			{
				Inventory = JsonUtility.FromJson<PlayerInventory>("{\"InventoryItems\":" + uwr.downloadHandler.text + "}");
				yield break;
			}
			Inventory = null;
		}
		if ((bool)MapEditorMaster.instance)
		{
			MapEditorMaster.instance.UpdateInventoryItemsUI();
		}
	}

	private void AssignData(string post_data)
	{
		GameMaster.instance.CurrentData = JsonUtility.FromJson<ProgressDataOnline>(post_data);
		Debug.Log("saving new save data now");
		PDO = JsonUtility.FromJson<ProgressDataOnline>(post_data);
		if (PDO == null)
		{
			Debug.Log("NO DATA FOUND SUPER ERROR DESTRUCTION");
			PDO = new ProgressDataOnline();
			return;
		}
		if (PDO.killed.Count < 30)
		{
			Debug.Log("killed too low, adding more");
			for (int i = PDO.killed.Count; i < 30; i++)
			{
				PDO.killed.Add(0);
			}
		}
		OptionsMainMenu.instance.AM = PDO.AM;
		OptionsMainMenu.instance.AMselected = PDO.ActivatedAM;
		OptionsMainMenu.instance.CheckCustomHitmarkers();
		TimePlayed = PDO.TimePlayed;
		GameMaster.instance.totalKills = PDO.totalKills;
		GameMaster.instance.totalWins = PDO.totalWins;
		GameMaster.instance.totalDefeats = PDO.totalDefeats;
		if (PDO.hW.Length > 6)
		{
			GameMaster.instance.highestWaves = PDO.hW;
		}
		GameMaster.instance.TankColorKilled = PDO.killed;
		GameMaster.instance.survivalTanksKilled = PDO.survivalTanksKilled;
		GameMaster.instance.maxMissionReached = ((PDO.maxMission0 < PDO.maxMission1) ? PDO.maxMission1 : PDO.maxMission0);
		GameMaster.instance.maxMissionReachedHard = PDO.maxMission2;
		GameMaster.instance.maxMissionReachedKid = ((PDO.maxMission1 < PDO.maxMission2) ? PDO.maxMission2 : PDO.maxMission1);
		GameMaster.instance.totalKillsBounce = PDO.totalKillsBounce;
		GameMaster.instance.totalRevivesPerformed = PDO.totalRevivesPerformed;
		if ((bool)TankeyTownMaster.instance)
		{
			TankeyTownMaster.instance.MarblesText.text = PDO.marbles.ToString();
		}
		Debug.Log("done assigning saving data!" + OptionsMainMenu.instance.AM.Length);
	}

	public void SaveCredentials()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		string savePath = Application.persistentDataPath + "/user_credentials.tnk";
		FileStream stream = new FileStream(savePath, FileMode.Create);
		UserCredentials UC = new UserCredentials(this);
		formatter.Serialize(stream, UC);
		stream.Close();
	}

	public IEnumerator BuyTankeyTownItem(TankeyTownStock StandItem)
	{
		UnityWebRequest keyRequest = UnityWebRequest.Get("https://weetanks.com/create_ip_key.php");
		yield return keyRequest.SendWebRequest();
		if (keyRequest.isNetworkError)
		{
			Debug.Log("Error While Sending: " + keyRequest.error);
			SignOut(manual: false);
			ConnectedStand.TransactionFailed();
			yield break;
		}
		if (keyRequest.downloadHandler.text == "WAIT")
		{
			Debug.Log("TOO FAST ");
			SignOut(manual: false);
			ConnectedStand.TransactionFailed();
			yield break;
		}
		string receivedKey = keyRequest.downloadHandler.text;
		WWWForm form = new WWWForm();
		form.AddField("key", Key);
		Debug.Log("ITS:" + Key);
		form.AddField("userid", UserID);
		form.AddField("authKey", receivedKey);
		form.AddField("shopid", StandItem.ItemShopID);
		form.AddField("itemid", StandItem.ItemID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/buy_item.php", form);
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
			if (uwr.downloadHandler.text.Contains("stock"))
			{
				Debug.LogError("Out of stock!");
				ConnectedStand.MyStandItem.AmountInStock = 0;
				ConnectedStand.SetItemOnDisplay(IsUpdate: true);
			}
			else if (uwr.downloadHandler.text.Contains("already"))
			{
				Debug.LogError("Got item already!");
			}
			ConnectedStand.TransactionFailed();
		}
		else
		{
			ConnectedStand.TransactionSucces();
			string[] splitArray = uwr.downloadHandler.text.Split(char.Parse("/"));
			Inventory = JsonUtility.FromJson<PlayerInventory>("{\"InventoryItems\":" + splitArray[0] + "}");
			PDO.marbles = int.Parse(splitArray[1]);
			if ((bool)TankeyTownMaster.instance)
			{
				TankeyTownMaster.instance.MarblesText.text = PDO.marbles.ToString();
			}
		}
	}
}
