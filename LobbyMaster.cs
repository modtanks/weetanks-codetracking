using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyMaster : MonoBehaviour
{
	private static LobbyMaster _instance;

	public int MyPlayerID;

	public string Player1Name;

	public string Player2Name;

	public bool LobbyStarted;

	public OnlinePlayerData OtherPlayerInfo;

	public OnlinePlayerData MyPlayerInfo;

	private bool prevRequestIsHere = true;

	public float waitingTimeBetweenRequests = 0.4f;

	public bool isDoingInfo;

	public static LobbyMaster instance => _instance;

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
		OtherPlayerInfo = new OnlinePlayerData();
		MyPlayerInfo = new OnlinePlayerData();
	}

	public IEnumerator DoPlayerInfo()
	{
		isDoingInfo = true;
		if (!prevRequestIsHere)
		{
			yield break;
		}
		prevRequestIsHere = false;
		WWWForm wWWForm = new WWWForm();
		new OnlinePlayerData();
		wWWForm.AddField("username", AccountMaster.instance.Username);
		wWWForm.AddField("userid", AccountMaster.instance.UserID);
		wWWForm.AddField("key", AccountMaster.instance.Key);
		wWWForm.AddField("lobbyid", AccountMaster.instance.LobbyID);
		wWWForm.AddField("action", 1);
		wWWForm.AddField("playerData", JsonUtility.ToJson(MyPlayerInfo));
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/send_lobby_data.php", wWWForm);
		uwr.chunkedTransfer = false;
		yield return uwr.SendWebRequest();
		prevRequestIsHere = true;
		if (uwr.isNetworkError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
			yield break;
		}
		Debug.Log("Received: " + uwr.downloadHandler.text);
		if (uwr.downloadHandler.text.Contains("FAILED"))
		{
			yield return new WaitForSeconds(waitingTimeBetweenRequests);
			StartCoroutine(DoPlayerInfo());
		}
		else if (uwr.downloadHandler.text.Contains("NO_PLAYER"))
		{
			SceneManager.LoadScene(0);
		}
		else if (uwr.downloadHandler.text.Contains("STOPPED"))
		{
			AccountMaster.instance.LobbyID = 0;
			AccountMaster.instance.LobbyCode = null;
			SceneManager.LoadScene(0);
		}
		else
		{
			OtherPlayerInfo = JsonUtility.FromJson<OnlinePlayerData>(uwr.downloadHandler.text);
			yield return new WaitForSeconds(waitingTimeBetweenRequests);
			StartCoroutine(DoPlayerInfo());
		}
	}
}
