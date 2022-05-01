using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour
{
	private static SteamTest _instance;

	public bool SteamOverlayActive = false;

	public ulong SteamAccountID;

	public string username = null;

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

	public void GetAchievement(int number)
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		if (number < 10)
		{
			string digit = "0" + number;
			if (SteamUserStats.SetAchievement("AM_" + digit))
			{
				Debug.LogWarning("NEW ACHIEVEMENT SET! AM_" + digit);
			}
			else
			{
				Debug.LogWarning("NEW ACHIEVEMENT FAILED TO SET! AM_" + digit);
			}
		}
		else if (SteamUserStats.SetAchievement("AM_" + number))
		{
			Debug.LogWarning("NEW ACHIEVEMENT SET! AM_" + number);
		}
		else
		{
			Debug.LogWarning("NEW ACHIEVEMENT FAILED TO SET! AM_" + number);
		}
	}

	private void Update()
	{
		if (!SteamManager.Initialized || !GameMaster.instance)
		{
			return;
		}
		if (GameMaster.instance.inMenuMode)
		{
			SteamFriends.SetRichPresence("steam_display", "#Status_AtMainMenu");
		}
		else if (GameMaster.instance.inTankeyTown)
		{
			SteamFriends.SetRichPresence("steam_display", "#Status_InTankeyTown");
		}
		else if ((bool)MapEditorMaster.instance)
		{
			if (!MapEditorMaster.instance.inPlayingMode)
			{
				SteamFriends.SetRichPresence("steam_display", "#Status_InMapEditor");
				return;
			}
			SteamFriends.SetRichPresence("gamestatus", "Custom Campaign");
			string lvl2 = "Mission " + (GameMaster.instance.CurrentMission + 1);
			SteamFriends.SetRichPresence("Level", lvl2);
			SteamFriends.SetRichPresence("steam_display", "#StatusWithLevel");
		}
		else if (GameMaster.instance.isZombieMode)
		{
			SteamFriends.SetRichPresence("gamestatus", "Survival Mode");
			string lvl3 = "Wave " + ZombieTankSpawner.instance.Wave;
			SteamFriends.SetRichPresence("Level", lvl3);
			SteamFriends.SetRichPresence("steam_display", "#StatusWithLevel");
		}
		else if (GameMaster.instance.isOfficialCampaign)
		{
			SteamFriends.SetRichPresence("gamestatus", "Campaign");
			string lvl = "Mission " + (GameMaster.instance.CurrentMission + 1);
			SteamFriends.SetRichPresence("Level", lvl);
			SteamFriends.SetRichPresence("steam_display", "#StatusWithLevel");
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
