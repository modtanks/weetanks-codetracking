using UnityEngine;

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
}
