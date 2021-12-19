using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
	private static SteamTest _instance;

	public bool SteamOverlayActive;

	public ulong SteamAccountID;

	public string username;

	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	public static SteamTest instance => _instance;

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
		if (SteamManager.Initialized)
		{
			username = SteamFriends.GetPersonaName();
			SteamAccountID = SteamUser.GetSteamID().m_SteamID;
			Debug.Log("STEAM got account: " + username);
		}
	}

	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
		}
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			Debug.Log("Steam Overlay has been activated");
			SteamOverlayActive = true;
		}
		else
		{
			Debug.Log("Steam Overlay has been closed");
			SteamOverlayActive = false;
		}
	}
}
