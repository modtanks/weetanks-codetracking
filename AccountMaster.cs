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

	public bool isSignedIn;

	public bool hasDoneSignInCheck;

	public int TimePlayed;

	private static AccountMaster _instance;

	public bool isInLobby;

	public string LobbyCode;

	public int LobbyID;

	public bool CanSetNewPassword;

	public ProgressDataOnline PDO;

	public GameObject MarblesNotification;

	private SteamTest ST;

	public PlayerInventory Inventory = new PlayerInventory();

	public bool IsDoingTransaction;

	public string PreviousCode;

	private bool SetTime;

	private bool isSaving;

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
		GameObject gameObject = Object.Instantiate(MarblesNotification);
		if (!gameObject)
		{
			return;
		}
		TextMeshProUGUI component = gameObject.transform.GetChild(0).Find("AMOUNT").GetComponent<TextMeshProUGUI>();
		Object.Destroy(gameObject, 2.75f);
		if ((bool)component)
		{
			if (amount > 1)
			{
				component.text = "You got " + amount + " marbles!";
			}
			else
			{
				component.text = "You got " + amount + " marble!";
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
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		wWWForm.AddField("userid", UserID);
		wWWForm.AddField("username", Username);
		PDO.accountid = int.Parse(UserID);
		PDO.accountname = Username;
		wWWForm.AddField("sT", Mathf.RoundToInt(GameMaster.instance.counter * 1000f));
		wWWForm.AddField("Lv", level);
		wWWForm.AddField("dF", difficulty);
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_stats.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			isSaving = false;
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				isSaving = false;
			}
			else
			{
				isSaving = false;
				AssignData(uwr.downloadHandler.text);
				Debug.Log("SAVED");
			}
		}
		uwr.Dispose();
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
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		wWWForm.AddField("userid", UserID);
		wWWForm.AddField("username", Username);
		wWWForm.AddField("code", code);
		wWWForm.AddField("difficulty", OptionsMainMenu.instance.currentDifficulty);
		if (PreviousCode != null)
		{
			wWWForm.AddField("prev", PreviousCode);
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_log.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		Debug.Log("sending status..!.333");
		if (uwr.isNetworkError)
		{
			isSaving = false;
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				Debug.Log("UPDATING STATSU FAILED");
			}
			else
			{
				PreviousCode = uwr.downloadHandler.text;
				int num = int.Parse(uwr.downloadHandler.text.Split(char.Parse("/"))[1]);
				if (num > 0)
				{
					ShowMarbleNotification(num);
				}
			}
		}
		uwr.Dispose();
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
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		wWWForm.AddField("userid", UserID);
		wWWForm.AddField("username", Username);
		wWWForm.AddField("pT", TimePlayed);
		switch (type)
		{
		case 0:
			wWWForm.AddField("kT", amount);
			break;
		case 1:
			wWWForm.AddField("gW", 1);
			break;
		case 2:
			wWWForm.AddField("gL", 1);
			break;
		case 3:
			wWWForm.AddField("A", amount);
			break;
		case 4:
			wWWForm.AddField("A_U", amount);
			if (secondary_amount > 0)
			{
				wWWForm.AddField("A_R", secondary_amount);
			}
			break;
		case 5:
			wWWForm.AddField("M", amount);
			break;
		case 6:
			wWWForm.AddField("W", amount);
			wWWForm.AddField("Lv", GameMaster.instance.CurrentMission);
			break;
		case 7:
			wWWForm.AddField("dF", OptionsMainMenu.instance.currentDifficulty);
			wWWForm.AddField("cP", GameMaster.instance.CurrentMission + 1);
			break;
		}
		if (bounceKill)
		{
			wWWForm.AddField("kB", 1);
		}
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_stats_b.php", wWWForm);
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
		uwr.Dispose();
	}

	public IEnumerator SaveNewCloudData(ProgressDataOnline newPDO)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		wWWForm.AddField("userid", UserID);
		wWWForm.AddField("username", Username);
		newPDO.accountid = int.Parse(UserID);
		newPDO.accountname = Username;
		wWWForm.AddField("NewSaveData", JsonUtility.ToJson(newPDO));
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/update_user_stats.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			isSaving = false;
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				isSaving = false;
			}
			else
			{
				isSaving = false;
				AssignData(uwr.downloadHandler.text);
				Debug.Log("SAVED");
			}
		}
		uwr.Dispose();
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
			string path = Application.persistentDataPath + "/tank_progress.tnk";
			if (File.Exists(path) && manual)
			{
				SavingData.SaveData(GameMaster.instance, "tank_progress_backup");
				File.Delete(path);
			}
			GameMaster.instance.CurrentData = new ProgressDataOnline();
			PDO = new ProgressDataOnline();
			for (int i = 0; i < OptionsMainMenu.instance.AM.Length; i++)
			{
				OptionsMainMenu.instance.AM[i] = 5;
			}
			for (int j = 0; j < OptionsMainMenu.instance.AMselected.Count; j++)
			{
				OptionsMainMenu.instance.AMselected[j] = 0;
			}
			GameMaster.instance.ResetData();
		}
	}

	public void LoadCredentials()
	{
		string path = Application.persistentDataPath + "/user_credentials.tnk";
		if (!File.Exists(path))
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
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(path, FileMode.Open);
		UserCredentials userCredentials = binaryFormatter.Deserialize(fileStream) as UserCredentials;
		fileStream.Close();
		if (userCredentials == null)
		{
			return;
		}
		if (userCredentials.hash != SystemInfo.deviceUniqueIdentifier)
		{
			SignOut(manual: false);
			return;
		}
		if (ST.SteamAccountID > 1000 && userCredentials.steamid > 1000 && ST.SteamAccountID == userCredentials.steamid)
		{
			Debug.Log("creating new account steam, or logging in?");
			StartCoroutine(CreateAccountViaSteam("https://www.weetanks.com/create_steam_account.php", ST.username, ST.SteamAccountID, IsSignIn: true));
			return;
		}
		Username = userCredentials.name;
		Key = userCredentials.key;
		Debug.Log("LOADING:" + userCredentials.key);
		UserID = userCredentials.id;
		if (userCredentials.key == "" || userCredentials.key == null)
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
		if (userCredentials.steamid > 1000)
		{
			SteamUserID = userCredentials.steamid;
		}
	}

	public IEnumerator CreateAccountViaSteam(string url, string steamusername, ulong steamuserid, bool IsSignIn)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", steamusername);
		wWWForm.AddField("steamuserid", steamuserid.ToString());
		wWWForm.AddField("userData", JsonUtility.ToJson(GameMaster.instance.CurrentData));
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
		uwr.SetRequestHeader("Access-Control-Allow-Credentials", "true");
		uwr.SetRequestHeader("Access-Control-Allow-Headers", "Accept, Content-Type, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
		uwr.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, PUT, OPTIONS");
		uwr.SetRequestHeader("Access-Control-Allow-Origin", "*");
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("FAILED"))
			{
				GameMaster.instance.CurrentData.marbles = 0;
				GameMaster.instance.CurrentData.accountname = null;
				isSignedIn = false;
				SignOut(manual: false);
			}
			else if (uwr.downloadHandler.text.Contains("EXISTS"))
			{
				GameMaster.instance.CurrentData.marbles = 0;
				GameMaster.instance.CurrentData.accountname = null;
				isSignedIn = false;
			}
			else
			{
				Debug.Log("signed in correctly!");
				isSignedIn = true;
				string[] array = uwr.downloadHandler.text.Split(char.Parse("/"));
				SteamUserID = steamuserid;
				UserID = array[1];
				GameMaster.instance.AccountID = int.Parse(array[1]);
				Key = array[0];
				Username = array[2];
				isSignedIn = true;
				SaveCredentials();
				if (array.Length > 3 && array[3] != null)
				{
					AssignData(array[3]);
				}
				StartCoroutine(CanSetPasswordCheck());
			}
		}
		uwr.Dispose();
	}

	public IEnumerator TransferAccountToSteam(string url)
	{
		NewMenuControl NMC = GameObject.Find("CanvasMover").transform.GetChild(0).gameObject.GetComponent<NewMenuControl>();
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("username", Username);
		wWWForm.AddField("steamusername", SteamTest.instance.username);
		wWWForm.AddField("userid", UserID);
		wWWForm.AddField("key", Key);
		wWWForm.AddField("steamuserid", SteamTest.instance.SteamAccountID.ToString());
		UnityWebRequest uwr = UnityWebRequest.Post(url, wWWForm);
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
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("already connected"))
			{
				if ((bool)NMC)
				{
					NMC.StartCoroutine(NMC.FailedTransferSteam(uwr.downloadHandler.text));
				}
			}
			else if (uwr.downloadHandler.text.Contains("FAILED") || uwr.downloadHandler.text.Contains("false"))
			{
				if ((bool)NMC)
				{
					NMC.StartCoroutine(NMC.FailedTransferSteam(null));
				}
			}
			else if (uwr.downloadHandler.text.Contains("overwritten"))
			{
				if ((bool)NMC)
				{
					NMC.StartCoroutine(NMC.SuccesTransferSteam());
				}
			}
			else if (uwr.downloadHandler.text.Contains("EXISTS"))
			{
				if ((bool)NMC)
				{
					isSignedIn = true;
					string[] array = uwr.downloadHandler.text.Split(char.Parse("/"));
					SteamUserID = SteamTest.instance.SteamAccountID;
					SaveCredentials();
					if (array.Length > 3 && array[3] != null)
					{
						AssignData(array[3]);
					}
					if ((bool)NMC)
					{
						NMC.StartCoroutine(NMC.SuccesTransferSteam());
					}
				}
			}
			else
			{
				isSignedIn = true;
				string[] array2 = uwr.downloadHandler.text.Split(char.Parse("/"));
				SteamUserID = SteamTest.instance.SteamAccountID;
				UserID = array2[1];
				GameMaster.instance.AccountID = int.Parse(array2[1]);
				if (array2[0] != "null")
				{
					Key = array2[0];
				}
				Username = array2[2];
				isSignedIn = true;
				SaveCredentials();
				if (array2.Length > 3 && array2[3] != null)
				{
					AssignData(array2[3]);
				}
				if ((bool)NMC)
				{
					NMC.StartCoroutine(NMC.SuccesTransferSteam());
				}
			}
		}
		uwr.Dispose();
	}

	public IEnumerator CanSetPasswordCheck()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		wWWForm.AddField("userid", UserID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/can_update_password.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("approved"))
			{
				CanSetNewPassword = true;
			}
			else
			{
				CanSetNewPassword = false;
			}
		}
		uwr.Dispose();
		StartCoroutine(GetAchievementsCheck());
	}

	private IEnumerator GetAchievementsCheck()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		wWWForm.AddField("userid", UserID);
		if (SteamTest.instance.SteamAccountID < 10000)
		{
			yield break;
		}
		wWWForm.AddField("steamid", SteamTest.instance.SteamAccountID.ToString());
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/can_get_steam_achievements.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (uwr.downloadHandler.text.Contains("/"))
			{
				string[] array = uwr.downloadHandler.text.Split(char.Parse("/"));
				for (int i = 0; i < array.Length; i++)
				{
					if (int.Parse(array[i]) == 1)
					{
						SteamTest.instance.GetAchievement(i);
					}
				}
			}
		}
		uwr.Dispose();
	}

	public IEnumerator LoadCloudData()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		Debug.Log("ITS:" + Key);
		wWWForm.AddField("userid", UserID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/check_account.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			Debug.Log("Received: " + uwr.downloadHandler.text);
			if (!uwr.downloadHandler.text.Contains("FAILED"))
			{
				if (uwr.downloadHandler.text.Contains("killed"))
				{
					Debug.Log("loaded cloud data!");
					StartCoroutine(CanSetPasswordCheck());
					AssignData(uwr.downloadHandler.text);
				}
				else
				{
					Debug.Log("no cloud data.. creating new!");
					PDO = new ProgressDataOnline();
					SaveNewCloudData(PDO);
				}
				isSignedIn = true;
				if (SteamUserID < 1000 && ST.SteamAccountID > 1000)
				{
					NewMenuControl component = GameObject.Find("CanvasMover").transform.GetChild(0).gameObject.GetComponent<NewMenuControl>();
					if ((bool)component)
					{
						component.EnableTransferMenu();
					}
				}
				yield break;
			}
			SignOut(manual: false);
		}
		uwr.Dispose();
		hasDoneSignInCheck = true;
	}

	public IEnumerator GetCloudInventory()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		Debug.Log("ITS:" + Key);
		wWWForm.AddField("userid", UserID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/get_inventory.php", wWWForm);
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
		uwr.Dispose();
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
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/user_credentials.tnk", FileMode.Create);
		UserCredentials graph = new UserCredentials(this);
		binaryFormatter.Serialize(fileStream, graph);
		fileStream.Close();
	}

	public IEnumerator BuyTankeyTownItem(TankeyTownStock StandItem)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("key", Key);
		Debug.Log("ITS:" + Key);
		wWWForm.AddField("userid", UserID);
		wWWForm.AddField("shopid", StandItem.ItemShopID);
		wWWForm.AddField("itemid", StandItem.ItemID);
		UnityWebRequest uwr = UnityWebRequest.Post("https://www.weetanks.com/buy_item.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
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
				string[] array = uwr.downloadHandler.text.Split(char.Parse("/"));
				Inventory = JsonUtility.FromJson<PlayerInventory>("{\"InventoryItems\":" + array[0] + "}");
				PDO.marbles = int.Parse(array[1]);
				if ((bool)TankeyTownMaster.instance)
				{
					TankeyTownMaster.instance.MarblesText.text = PDO.marbles.ToString();
				}
			}
		}
		uwr.Dispose();
	}
}
