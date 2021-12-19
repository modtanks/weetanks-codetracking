using System;
using UnityEngine;

[Serializable]
public class UserCredentials
{
	public string key;

	public string id;

	public string name;

	public string hash;

	public ulong steamid;

	public UserCredentials(AccountMaster AM)
	{
		key = AM.Key;
		id = AM.UserID;
		name = AM.Username;
		hash = SystemInfo.deviceUniqueIdentifier;
		steamid = AM.SteamUserID;
	}
}
