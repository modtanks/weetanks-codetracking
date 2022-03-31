using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyMaster : MonoBehaviour
{
	private static LobbyMaster _instance;

	public int MyPlayerID = 0;

	public string Player1Name;

	public string Player2Name;

	public bool LobbyStarted = false;

	public OnlinePlayerData OtherPlayerInfo;

	public OnlinePlayerData MyPlayerInfo;

	private bool prevRequestIsHere = true;

	public float waitingTimeBetweenRequests = 0.4f;

	public bool isDoingInfo = false;

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
		WWWForm form = new WWWForm();
		new OnlinePlayerData();
		form.AddField("username", AccountMaster.instance.Username);
		form.AddField("userid", AccountMaster.instance.UserID);
		form.AddField("key", AccountMaster.instance.Key);
		form.AddField("lobbyid", AccountMaster.instance.LobbyID);
		form.AddField("action", 1);
		form.AddField("playerData", JsonUtility.ToJson(MyPlayerInfo));
		UnityWebRequest uwr = UnityWebRequest.Post("https://weetanks.com/send_lobby_data.php", form);
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
