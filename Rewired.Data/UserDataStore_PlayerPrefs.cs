using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;

namespace Rewired.Data;

public class UserDataStore_PlayerPrefs : UserDataStore
{
	private class ControllerAssignmentSaveInfo
	{
		public class PlayerInfo
		{
			public int id;

			public bool hasKeyboard;

			public bool hasMouse;

			public JoystickInfo[] joysticks;

			public int joystickCount => (joysticks != null) ? joysticks.Length : 0;

			public int IndexOfJoystick(int joystickId)
			{
				for (int i = 0; i < joystickCount; i++)
				{
					if (joysticks[i] != null && joysticks[i].id == joystickId)
					{
						return i;
					}
				}
				return -1;
			}

			public bool ContainsJoystick(int joystickId)
			{
				return IndexOfJoystick(joystickId) >= 0;
			}
		}

		public class JoystickInfo
		{
			public Guid instanceGuid;

			public string hardwareIdentifier;

			public int id;
		}

		public PlayerInfo[] players;

		public int playerCount => (players != null) ? players.Length : 0;

		public ControllerAssignmentSaveInfo()
		{
		}

		public ControllerAssignmentSaveInfo(int playerCount)
		{
			players = new PlayerInfo[playerCount];
			for (int i = 0; i < playerCount; i++)
			{
				players[i] = new PlayerInfo();
			}
		}

		public int IndexOfPlayer(int playerId)
		{
			for (int i = 0; i < playerCount; i++)
			{
				if (players[i] != null && players[i].id == playerId)
				{
					return i;
				}
			}
			return -1;
		}

		public bool ContainsPlayer(int playerId)
		{
			return IndexOfPlayer(playerId) >= 0;
		}
	}

	private class JoystickAssignmentHistoryInfo
	{
		public readonly Joystick joystick;

		public readonly int oldJoystickId;

		public JoystickAssignmentHistoryInfo(Joystick joystick, int oldJoystickId)
		{
			if (joystick == null)
			{
				throw new ArgumentNullException("joystick");
			}
			this.joystick = joystick;
			this.oldJoystickId = oldJoystickId;
		}
	}

	private const string thisScriptName = "UserDataStore_PlayerPrefs";

	private const string logPrefix = "Rewired: ";

	private const string editorLoadedMessage = "\n***IMPORTANT:*** Changes made to the Rewired Input Manager configuration after the last time XML data was saved WILL NOT be used because the loaded old saved data has overwritten these values. If you change something in the Rewired Input Manager such as a Joystick Map or Input Behavior settings, you will not see these changes reflected in the current configuration. Clear PlayerPrefs using the inspector option on the UserDataStore_PlayerPrefs component.";

	private const string playerPrefsKeySuffix_controllerAssignments = "ControllerAssignments";

	private const int controllerMapPPKeyVersion_original = 0;

	private const int controllerMapPPKeyVersion_includeDuplicateJoystickIndex = 1;

	private const int controllerMapPPKeyVersion_supportDisconnectedControllers = 2;

	private const int controllerMapPPKeyVersion_includeFormatVersion = 2;

	private const int controllerMapPPKeyVersion = 2;

	[Tooltip("Should this script be used? If disabled, nothing will be saved or loaded.")]
	[SerializeField]
	private bool isEnabled = true;

	[Tooltip("Should saved data be loaded on start?")]
	[SerializeField]
	private bool loadDataOnStart = true;

	[Tooltip("Should Player Joystick assignments be saved and loaded? This is not totally reliable for all Joysticks on all platforms. Some platforms/input sources do not provide enough information to reliably save assignments from session to session and reboot to reboot.")]
	[SerializeField]
	private bool loadJoystickAssignments = true;

	[Tooltip("Should Player Keyboard assignments be saved and loaded?")]
	[SerializeField]
	private bool loadKeyboardAssignments = true;

	[Tooltip("Should Player Mouse assignments be saved and loaded?")]
	[SerializeField]
	private bool loadMouseAssignments = true;

	[Tooltip("The PlayerPrefs key prefix. Change this to change how keys are stored in PlayerPrefs. Changing this will make saved data already stored with the old key no longer accessible.")]
	[SerializeField]
	private string playerPrefsKeyPrefix = "RewiredSaveData";

	[NonSerialized]
	private bool allowImpreciseJoystickAssignmentMatching = true;

	[NonSerialized]
	private bool deferredJoystickAssignmentLoadPending;

	[NonSerialized]
	private bool wasJoystickEverDetected;

	[NonSerialized]
	private List<int> __allActionIds;

	[NonSerialized]
	private string __allActionIdsString;

	public bool IsEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
		}
	}

	public bool LoadDataOnStart
	{
		get
		{
			return loadDataOnStart;
		}
		set
		{
			loadDataOnStart = value;
		}
	}

	public bool LoadJoystickAssignments
	{
		get
		{
			return loadJoystickAssignments;
		}
		set
		{
			loadJoystickAssignments = value;
		}
	}

	public bool LoadKeyboardAssignments
	{
		get
		{
			return loadKeyboardAssignments;
		}
		set
		{
			loadKeyboardAssignments = value;
		}
	}

	public bool LoadMouseAssignments
	{
		get
		{
			return loadMouseAssignments;
		}
		set
		{
			loadMouseAssignments = value;
		}
	}

	public string PlayerPrefsKeyPrefix
	{
		get
		{
			return playerPrefsKeyPrefix;
		}
		set
		{
			playerPrefsKeyPrefix = value;
		}
	}

	private string playerPrefsKey_controllerAssignments => string.Format("{0}_{1}", playerPrefsKeyPrefix, "ControllerAssignments");

	private bool loadControllerAssignments => loadKeyboardAssignments || loadMouseAssignments || loadJoystickAssignments;

	private List<int> allActionIds
	{
		get
		{
			if (__allActionIds != null)
			{
				return __allActionIds;
			}
			List<int> ids = new List<int>();
			IList<InputAction> actions = ReInput.mapping.Actions;
			for (int i = 0; i < actions.Count; i++)
			{
				ids.Add(actions[i].id);
			}
			__allActionIds = ids;
			return ids;
		}
	}

	private string allActionIdsString
	{
		get
		{
			if (!string.IsNullOrEmpty(__allActionIdsString))
			{
				return __allActionIdsString;
			}
			StringBuilder sb = new StringBuilder();
			List<int> ids = allActionIds;
			for (int i = 0; i < ids.Count; i++)
			{
				if (i > 0)
				{
					sb.Append(",");
				}
				sb.Append(ids[i]);
			}
			__allActionIdsString = sb.ToString();
			return __allActionIdsString;
		}
	}

	public override void Save()
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
		}
		else
		{
			SaveAll();
		}
	}

	public override void SaveControllerData(int playerId, ControllerType controllerType, int controllerId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
		}
		else
		{
			SaveControllerDataNow(playerId, controllerType, controllerId);
		}
	}

	public override void SaveControllerData(ControllerType controllerType, int controllerId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
		}
		else
		{
			SaveControllerDataNow(controllerType, controllerId);
		}
	}

	public override void SavePlayerData(int playerId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
		}
		else
		{
			SavePlayerDataNow(playerId);
		}
	}

	public override void SaveInputBehavior(int playerId, int behaviorId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not save any data.", this);
		}
		else
		{
			SaveInputBehaviorNow(playerId, behaviorId);
		}
	}

	public override void Load()
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
		}
		else
		{
			int count = LoadAll();
		}
	}

	public override void LoadControllerData(int playerId, ControllerType controllerType, int controllerId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
		}
		else
		{
			int count = LoadControllerDataNow(playerId, controllerType, controllerId);
		}
	}

	public override void LoadControllerData(ControllerType controllerType, int controllerId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
		}
		else
		{
			int count = LoadControllerDataNow(controllerType, controllerId);
		}
	}

	public override void LoadPlayerData(int playerId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
		}
		else
		{
			int count = LoadPlayerDataNow(playerId);
		}
	}

	public override void LoadInputBehavior(int playerId, int behaviorId)
	{
		if (!isEnabled)
		{
			Debug.LogWarning("Rewired: UserDataStore_PlayerPrefs is disabled and will not load any data.", this);
		}
		else
		{
			int count = LoadInputBehaviorNow(playerId, behaviorId);
		}
	}

	protected override void OnInitialize()
	{
		if (loadDataOnStart)
		{
			Load();
			if (loadControllerAssignments && ReInput.controllers.joystickCount > 0)
			{
				wasJoystickEverDetected = true;
				SaveControllerAssignments();
			}
		}
	}

	protected override void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		if (isEnabled && args.controllerType == ControllerType.Joystick)
		{
			int count = LoadJoystickData(args.controllerId);
			if (loadDataOnStart && loadJoystickAssignments && !wasJoystickEverDetected)
			{
				StartCoroutine(LoadJoystickAssignmentsDeferred());
			}
			if (loadJoystickAssignments && !deferredJoystickAssignmentLoadPending)
			{
				SaveControllerAssignments();
			}
			wasJoystickEverDetected = true;
		}
	}

	protected override void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args)
	{
		if (isEnabled && args.controllerType == ControllerType.Joystick)
		{
			SaveJoystickData(args.controllerId);
		}
	}

	protected override void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		if (isEnabled && loadControllerAssignments)
		{
			SaveControllerAssignments();
		}
	}

	public override void SaveControllerMap(int playerId, ControllerMap controllerMap)
	{
		if (controllerMap != null)
		{
			Player player = ReInput.players.GetPlayer(playerId);
			if (player != null)
			{
				SaveControllerMap(player, controllerMap);
			}
		}
	}

	public override ControllerMap LoadControllerMap(int playerId, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
	{
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null)
		{
			return null;
		}
		return LoadControllerMap(player, controllerIdentifier, categoryId, layoutId);
	}

	private int LoadAll()
	{
		int count = 0;
		if (loadControllerAssignments && LoadControllerAssignmentsNow())
		{
			count++;
		}
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			count += LoadPlayerDataNow(allPlayers[i]);
		}
		return count + LoadAllJoystickCalibrationData();
	}

	private int LoadPlayerDataNow(int playerId)
	{
		return LoadPlayerDataNow(ReInput.players.GetPlayer(playerId));
	}

	private int LoadPlayerDataNow(Player player)
	{
		if (player == null)
		{
			return 0;
		}
		int count = 0;
		count += LoadInputBehaviors(player.id);
		count += LoadControllerMaps(player.id, ControllerType.Keyboard, 0);
		count += LoadControllerMaps(player.id, ControllerType.Mouse, 0);
		foreach (Joystick joystick in player.controllers.Joysticks)
		{
			count += LoadControllerMaps(player.id, ControllerType.Joystick, joystick.id);
		}
		RefreshLayoutManager(player.id);
		return count;
	}

	private int LoadAllJoystickCalibrationData()
	{
		int count = 0;
		IList<Joystick> joysticks = ReInput.controllers.Joysticks;
		for (int i = 0; i < joysticks.Count; i++)
		{
			count += LoadJoystickCalibrationData(joysticks[i]);
		}
		return count;
	}

	private int LoadJoystickCalibrationData(Joystick joystick)
	{
		if (joystick == null)
		{
			return 0;
		}
		return joystick.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick)) ? 1 : 0;
	}

	private int LoadJoystickCalibrationData(int joystickId)
	{
		return LoadJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
	}

	private int LoadJoystickData(int joystickId)
	{
		int count = 0;
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			Player player = allPlayers[i];
			if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
			{
				count += LoadControllerMaps(player.id, ControllerType.Joystick, joystickId);
				RefreshLayoutManager(player.id);
			}
		}
		return count + LoadJoystickCalibrationData(joystickId);
	}

	private int LoadControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
	{
		int count = 0;
		count += LoadControllerMaps(playerId, controllerType, controllerId);
		RefreshLayoutManager(playerId);
		return count + LoadControllerDataNow(controllerType, controllerId);
	}

	private int LoadControllerDataNow(ControllerType controllerType, int controllerId)
	{
		int count = 0;
		if (controllerType == ControllerType.Joystick)
		{
			count += LoadJoystickCalibrationData(controllerId);
		}
		return count;
	}

	private int LoadControllerMaps(int playerId, ControllerType controllerType, int controllerId)
	{
		int count = 0;
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null)
		{
			return count;
		}
		Controller controller = ReInput.controllers.GetController(controllerType, controllerId);
		if (controller == null)
		{
			return count;
		}
		IList<InputMapCategory> categories = ReInput.mapping.MapCategories;
		for (int i = 0; i < categories.Count; i++)
		{
			InputMapCategory category = categories[i];
			if (!category.userAssignable)
			{
				continue;
			}
			IList<InputLayout> layouts = ReInput.mapping.MapLayouts(controller.type);
			for (int j = 0; j < layouts.Count; j++)
			{
				InputLayout layout = layouts[j];
				ControllerMap controllerMap = LoadControllerMap(player, controller.identifier, category.id, layout.id);
				if (controllerMap != null)
				{
					player.controllers.maps.AddMap(controller, controllerMap);
					count++;
				}
			}
		}
		return count;
	}

	private ControllerMap LoadControllerMap(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
	{
		if (player == null)
		{
			return null;
		}
		string xml = GetControllerMapXml(player, controllerIdentifier, categoryId, layoutId);
		if (string.IsNullOrEmpty(xml))
		{
			return null;
		}
		ControllerMap controllerMap = ControllerMap.CreateFromXml(controllerIdentifier.controllerType, xml);
		if (controllerMap == null)
		{
			return null;
		}
		List<int> knownActionIds = GetControllerMapKnownActionIds(player, controllerIdentifier, categoryId, layoutId);
		AddDefaultMappingsForNewActions(controllerIdentifier, controllerMap, knownActionIds);
		return controllerMap;
	}

	private int LoadInputBehaviors(int playerId)
	{
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null)
		{
			return 0;
		}
		int count = 0;
		IList<InputBehavior> behaviors = ReInput.mapping.GetInputBehaviors(player.id);
		for (int i = 0; i < behaviors.Count; i++)
		{
			count += LoadInputBehaviorNow(player, behaviors[i]);
		}
		return count;
	}

	private int LoadInputBehaviorNow(int playerId, int behaviorId)
	{
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null)
		{
			return 0;
		}
		InputBehavior behavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
		if (behavior == null)
		{
			return 0;
		}
		return LoadInputBehaviorNow(player, behavior);
	}

	private int LoadInputBehaviorNow(Player player, InputBehavior inputBehavior)
	{
		if (player == null || inputBehavior == null)
		{
			return 0;
		}
		string xml = GetInputBehaviorXml(player, inputBehavior.id);
		if (xml == null || xml == string.Empty)
		{
			return 0;
		}
		return inputBehavior.ImportXmlString(xml) ? 1 : 0;
	}

	private bool LoadControllerAssignmentsNow()
	{
		try
		{
			ControllerAssignmentSaveInfo data = LoadControllerAssignmentData();
			if (data == null)
			{
				return false;
			}
			if (loadKeyboardAssignments || loadMouseAssignments)
			{
				LoadKeyboardAndMouseAssignmentsNow(data);
			}
			if (loadJoystickAssignments)
			{
				LoadJoystickAssignmentsNow(data);
			}
		}
		catch
		{
		}
		return true;
	}

	private bool LoadKeyboardAndMouseAssignmentsNow(ControllerAssignmentSaveInfo data)
	{
		try
		{
			if (data == null && (data = LoadControllerAssignmentData()) == null)
			{
				return false;
			}
			foreach (Player player in ReInput.players.AllPlayers)
			{
				if (data.ContainsPlayer(player.id))
				{
					ControllerAssignmentSaveInfo.PlayerInfo playerData = data.players[data.IndexOfPlayer(player.id)];
					if (loadKeyboardAssignments)
					{
						player.controllers.hasKeyboard = playerData.hasKeyboard;
					}
					if (loadMouseAssignments)
					{
						player.controllers.hasMouse = playerData.hasMouse;
					}
				}
			}
		}
		catch
		{
		}
		return true;
	}

	private bool LoadJoystickAssignmentsNow(ControllerAssignmentSaveInfo data)
	{
		try
		{
			if (ReInput.controllers.joystickCount == 0)
			{
				return false;
			}
			if (data == null && (data = LoadControllerAssignmentData()) == null)
			{
				return false;
			}
			foreach (Player player3 in ReInput.players.AllPlayers)
			{
				player3.controllers.ClearControllersOfType(ControllerType.Joystick);
			}
			List<JoystickAssignmentHistoryInfo> joystickHistory = (loadJoystickAssignments ? new List<JoystickAssignmentHistoryInfo>() : null);
			foreach (Player player2 in ReInput.players.AllPlayers)
			{
				if (!data.ContainsPlayer(player2.id))
				{
					continue;
				}
				ControllerAssignmentSaveInfo.PlayerInfo playerData2 = data.players[data.IndexOfPlayer(player2.id)];
				for (int j = 0; j < playerData2.joystickCount; j++)
				{
					ControllerAssignmentSaveInfo.JoystickInfo joystickInfo2 = playerData2.joysticks[j];
					if (joystickInfo2 == null)
					{
						continue;
					}
					Joystick joystick2 = FindJoystickPrecise(joystickInfo2);
					if (joystick2 != null)
					{
						if (joystickHistory.Find((JoystickAssignmentHistoryInfo x) => x.joystick == joystick2) == null)
						{
							joystickHistory.Add(new JoystickAssignmentHistoryInfo(joystick2, joystickInfo2.id));
						}
						player2.controllers.AddController(joystick2, removeFromOtherPlayers: false);
					}
				}
			}
			if (allowImpreciseJoystickAssignmentMatching)
			{
				foreach (Player player in ReInput.players.AllPlayers)
				{
					if (!data.ContainsPlayer(player.id))
					{
						continue;
					}
					ControllerAssignmentSaveInfo.PlayerInfo playerData = data.players[data.IndexOfPlayer(player.id)];
					for (int i = 0; i < playerData.joystickCount; i++)
					{
						ControllerAssignmentSaveInfo.JoystickInfo joystickInfo = playerData.joysticks[i];
						if (joystickInfo == null)
						{
							continue;
						}
						Joystick joystick = null;
						int index = joystickHistory.FindIndex((JoystickAssignmentHistoryInfo x) => x.oldJoystickId == joystickInfo.id);
						if (index >= 0)
						{
							joystick = joystickHistory[index].joystick;
						}
						else
						{
							if (!TryFindJoysticksImprecise(joystickInfo, out var matches))
							{
								continue;
							}
							foreach (Joystick match in matches)
							{
								if (joystickHistory.Find((JoystickAssignmentHistoryInfo x) => x.joystick == match) != null)
								{
									continue;
								}
								joystick = match;
								break;
							}
							if (joystick == null)
							{
								continue;
							}
							joystickHistory.Add(new JoystickAssignmentHistoryInfo(joystick, joystickInfo.id));
						}
						player.controllers.AddController(joystick, removeFromOtherPlayers: false);
					}
				}
			}
		}
		catch
		{
		}
		if (ReInput.configuration.autoAssignJoysticks)
		{
			ReInput.controllers.AutoAssignJoysticks();
		}
		return true;
	}

	private ControllerAssignmentSaveInfo LoadControllerAssignmentData()
	{
		try
		{
			if (!PlayerPrefs.HasKey(playerPrefsKey_controllerAssignments))
			{
				return null;
			}
			string json = PlayerPrefs.GetString(playerPrefsKey_controllerAssignments);
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			ControllerAssignmentSaveInfo data = JsonParser.FromJson<ControllerAssignmentSaveInfo>(json);
			if (data == null || data.playerCount == 0)
			{
				return null;
			}
			return data;
		}
		catch
		{
			return null;
		}
	}

	private IEnumerator LoadJoystickAssignmentsDeferred()
	{
		deferredJoystickAssignmentLoadPending = true;
		yield return new WaitForEndOfFrame();
		if (ReInput.isReady)
		{
			if (LoadJoystickAssignmentsNow(null))
			{
			}
			SaveControllerAssignments();
			deferredJoystickAssignmentLoadPending = false;
		}
	}

	private void SaveAll()
	{
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			SavePlayerDataNow(allPlayers[i]);
		}
		SaveAllJoystickCalibrationData();
		if (loadControllerAssignments)
		{
			SaveControllerAssignments();
		}
		PlayerPrefs.Save();
	}

	private void SavePlayerDataNow(int playerId)
	{
		SavePlayerDataNow(ReInput.players.GetPlayer(playerId));
		PlayerPrefs.Save();
	}

	private void SavePlayerDataNow(Player player)
	{
		if (player != null)
		{
			PlayerSaveData playerData = player.GetSaveData(userAssignableMapsOnly: true);
			SaveInputBehaviors(player, playerData);
			SaveControllerMaps(player, playerData);
		}
	}

	private void SaveAllJoystickCalibrationData()
	{
		IList<Joystick> joysticks = ReInput.controllers.Joysticks;
		for (int i = 0; i < joysticks.Count; i++)
		{
			SaveJoystickCalibrationData(joysticks[i]);
		}
	}

	private void SaveJoystickCalibrationData(int joystickId)
	{
		SaveJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
	}

	private void SaveJoystickCalibrationData(Joystick joystick)
	{
		if (joystick != null)
		{
			JoystickCalibrationMapSaveData saveData = joystick.GetCalibrationMapSaveData();
			string key = GetJoystickCalibrationMapPlayerPrefsKey(joystick);
			PlayerPrefs.SetString(key, saveData.map.ToXmlString());
		}
	}

	private void SaveJoystickData(int joystickId)
	{
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			Player player = allPlayers[i];
			if (player.controllers.ContainsController(ControllerType.Joystick, joystickId))
			{
				SaveControllerMaps(player.id, ControllerType.Joystick, joystickId);
			}
		}
		SaveJoystickCalibrationData(joystickId);
	}

	private void SaveControllerDataNow(int playerId, ControllerType controllerType, int controllerId)
	{
		SaveControllerMaps(playerId, controllerType, controllerId);
		SaveControllerDataNow(controllerType, controllerId);
		PlayerPrefs.Save();
	}

	private void SaveControllerDataNow(ControllerType controllerType, int controllerId)
	{
		if (controllerType == ControllerType.Joystick)
		{
			SaveJoystickCalibrationData(controllerId);
		}
		PlayerPrefs.Save();
	}

	private void SaveControllerMaps(Player player, PlayerSaveData playerSaveData)
	{
		foreach (ControllerMapSaveData saveData in playerSaveData.AllControllerMapSaveData)
		{
			SaveControllerMap(player, saveData.map);
		}
	}

	private void SaveControllerMaps(int playerId, ControllerType controllerType, int controllerId)
	{
		Player player = ReInput.players.GetPlayer(playerId);
		if (player == null || !player.controllers.ContainsController(controllerType, controllerId))
		{
			return;
		}
		ControllerMapSaveData[] saveData = player.controllers.maps.GetMapSaveData(controllerType, controllerId, userAssignableMapsOnly: true);
		if (saveData != null)
		{
			for (int i = 0; i < saveData.Length; i++)
			{
				SaveControllerMap(player, saveData[i].map);
			}
		}
	}

	private void SaveControllerMap(Player player, ControllerMap controllerMap)
	{
		string key = GetControllerMapPlayerPrefsKey(player, controllerMap.controller.identifier, controllerMap.categoryId, controllerMap.layoutId, 2);
		PlayerPrefs.SetString(key, controllerMap.ToXmlString());
		key = GetControllerMapKnownActionIdsPlayerPrefsKey(player, controllerMap.controller.identifier, controllerMap.categoryId, controllerMap.layoutId, 2);
		PlayerPrefs.SetString(key, allActionIdsString);
	}

	private void SaveInputBehaviors(Player player, PlayerSaveData playerSaveData)
	{
		if (player != null)
		{
			InputBehavior[] inputBehaviors = playerSaveData.inputBehaviors;
			for (int i = 0; i < inputBehaviors.Length; i++)
			{
				SaveInputBehaviorNow(player, inputBehaviors[i]);
			}
		}
	}

	private void SaveInputBehaviorNow(int playerId, int behaviorId)
	{
		Player player = ReInput.players.GetPlayer(playerId);
		if (player != null)
		{
			InputBehavior behavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
			if (behavior != null)
			{
				SaveInputBehaviorNow(player, behavior);
				PlayerPrefs.Save();
			}
		}
	}

	private void SaveInputBehaviorNow(Player player, InputBehavior inputBehavior)
	{
		if (player != null && inputBehavior != null)
		{
			string key = GetInputBehaviorPlayerPrefsKey(player, inputBehavior.id);
			PlayerPrefs.SetString(key, inputBehavior.ToXmlString());
		}
	}

	private bool SaveControllerAssignments()
	{
		try
		{
			ControllerAssignmentSaveInfo allPlayerData = new ControllerAssignmentSaveInfo(ReInput.players.allPlayerCount);
			for (int i = 0; i < ReInput.players.allPlayerCount; i++)
			{
				Player player = ReInput.players.AllPlayers[i];
				ControllerAssignmentSaveInfo.PlayerInfo playerData = new ControllerAssignmentSaveInfo.PlayerInfo();
				allPlayerData.players[i] = playerData;
				playerData.id = player.id;
				playerData.hasKeyboard = player.controllers.hasKeyboard;
				playerData.hasMouse = player.controllers.hasMouse;
				ControllerAssignmentSaveInfo.JoystickInfo[] joystickInfos = (playerData.joysticks = new ControllerAssignmentSaveInfo.JoystickInfo[player.controllers.joystickCount]);
				for (int j = 0; j < player.controllers.joystickCount; j++)
				{
					Joystick joystick = player.controllers.Joysticks[j];
					ControllerAssignmentSaveInfo.JoystickInfo joystickInfo = new ControllerAssignmentSaveInfo.JoystickInfo();
					joystickInfo.instanceGuid = joystick.deviceInstanceGuid;
					joystickInfo.id = joystick.id;
					joystickInfo.hardwareIdentifier = joystick.hardwareIdentifier;
					joystickInfos[j] = joystickInfo;
				}
			}
			PlayerPrefs.SetString(playerPrefsKey_controllerAssignments, JsonWriter.ToJson(allPlayerData));
			PlayerPrefs.Save();
		}
		catch
		{
		}
		return true;
	}

	private bool ControllerAssignmentSaveDataExists()
	{
		if (!PlayerPrefs.HasKey(playerPrefsKey_controllerAssignments))
		{
			return false;
		}
		string json = PlayerPrefs.GetString(playerPrefsKey_controllerAssignments);
		if (string.IsNullOrEmpty(json))
		{
			return false;
		}
		return true;
	}

	private string GetBasePlayerPrefsKey(Player player)
	{
		string key = playerPrefsKeyPrefix;
		return key + "|playerName=" + player.name;
	}

	private string GetControllerMapPlayerPrefsKey(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion)
	{
		string key = GetBasePlayerPrefsKey(player);
		key += "|dataType=ControllerMap";
		return key + GetControllerMapPlayerPrefsKeyCommonSuffix(player, controllerIdentifier, categoryId, layoutId, ppKeyVersion);
	}

	private string GetControllerMapKnownActionIdsPlayerPrefsKey(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion)
	{
		string key = GetBasePlayerPrefsKey(player);
		key += "|dataType=ControllerMap_KnownActionIds";
		return key + GetControllerMapPlayerPrefsKeyCommonSuffix(player, controllerIdentifier, categoryId, layoutId, ppKeyVersion);
	}

	private static string GetControllerMapPlayerPrefsKeyCommonSuffix(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion)
	{
		string key = "";
		if (ppKeyVersion >= 2)
		{
			key = key + "|kv=" + ppKeyVersion;
		}
		key = key + "|controllerMapType=" + GetControllerMapType(controllerIdentifier.controllerType).Name;
		key = key + "|categoryId=" + categoryId + "|layoutId=" + layoutId;
		if (ppKeyVersion >= 2)
		{
			key = key + "|hardwareGuid=" + controllerIdentifier.hardwareTypeGuid.ToString();
			if (controllerIdentifier.hardwareTypeGuid == Guid.Empty)
			{
				key = key + "|hardwareIdentifier=" + controllerIdentifier.hardwareIdentifier;
			}
			if (controllerIdentifier.controllerType == ControllerType.Joystick)
			{
				key = key + "|duplicate=" + GetDuplicateIndex(player, controllerIdentifier);
			}
		}
		else
		{
			key = key + "|hardwareIdentifier=" + controllerIdentifier.hardwareIdentifier;
			if (controllerIdentifier.controllerType == ControllerType.Joystick)
			{
				key = key + "|hardwareGuid=" + controllerIdentifier.hardwareTypeGuid.ToString();
				if (ppKeyVersion >= 1)
				{
					key = key + "|duplicate=" + GetDuplicateIndex(player, controllerIdentifier);
				}
			}
		}
		return key;
	}

	private string GetJoystickCalibrationMapPlayerPrefsKey(Joystick joystick)
	{
		string key = playerPrefsKeyPrefix;
		key += "|dataType=CalibrationMap";
		key = key + "|controllerType=" + joystick.type;
		key = key + "|hardwareIdentifier=" + joystick.hardwareIdentifier;
		return key + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
	}

	private string GetInputBehaviorPlayerPrefsKey(Player player, int inputBehaviorId)
	{
		string key = GetBasePlayerPrefsKey(player);
		key += "|dataType=InputBehavior";
		return key + "|id=" + inputBehaviorId;
	}

	private string GetControllerMapXml(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
	{
		for (int i = 2; i >= 0; i--)
		{
			string key = GetControllerMapPlayerPrefsKey(player, controllerIdentifier, categoryId, layoutId, i);
			if (PlayerPrefs.HasKey(key))
			{
				return PlayerPrefs.GetString(key);
			}
		}
		return null;
	}

	private List<int> GetControllerMapKnownActionIds(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId)
	{
		List<int> actionIds = new List<int>();
		string key = null;
		bool found = false;
		for (int j = 2; j >= 0; j--)
		{
			key = GetControllerMapKnownActionIdsPlayerPrefsKey(player, controllerIdentifier, categoryId, layoutId, j);
			if (PlayerPrefs.HasKey(key))
			{
				found = true;
				break;
			}
		}
		if (!found)
		{
			return actionIds;
		}
		string data = PlayerPrefs.GetString(key);
		if (string.IsNullOrEmpty(data))
		{
			return actionIds;
		}
		string[] split = data.Split(',');
		for (int i = 0; i < split.Length; i++)
		{
			if (!string.IsNullOrEmpty(split[i]) && int.TryParse(split[i], out var id))
			{
				actionIds.Add(id);
			}
		}
		return actionIds;
	}

	private string GetJoystickCalibrationMapXml(Joystick joystick)
	{
		string key = GetJoystickCalibrationMapPlayerPrefsKey(joystick);
		if (!PlayerPrefs.HasKey(key))
		{
			return string.Empty;
		}
		return PlayerPrefs.GetString(key);
	}

	private string GetInputBehaviorXml(Player player, int id)
	{
		string key = GetInputBehaviorPlayerPrefsKey(player, id);
		if (!PlayerPrefs.HasKey(key))
		{
			return string.Empty;
		}
		return PlayerPrefs.GetString(key);
	}

	private void AddDefaultMappingsForNewActions(ControllerIdentifier controllerIdentifier, ControllerMap controllerMap, List<int> knownActionIds)
	{
		if (controllerMap == null || knownActionIds == null || knownActionIds == null || knownActionIds.Count == 0)
		{
			return;
		}
		ControllerMap defaultMap = ReInput.mapping.GetControllerMapInstance(controllerIdentifier, controllerMap.categoryId, controllerMap.layoutId);
		if (defaultMap == null)
		{
			return;
		}
		List<int> unknownActionIds = new List<int>();
		foreach (int id in allActionIds)
		{
			if (!knownActionIds.Contains(id))
			{
				unknownActionIds.Add(id);
			}
		}
		if (unknownActionIds.Count == 0)
		{
			return;
		}
		foreach (ActionElementMap aem in defaultMap.AllMaps)
		{
			if (unknownActionIds.Contains(aem.actionId) && !controllerMap.DoesElementAssignmentConflict(aem))
			{
				ElementAssignment assignment = new ElementAssignment(controllerMap.controllerType, aem.elementType, aem.elementIdentifierId, aem.axisRange, aem.keyCode, aem.modifierKeyFlags, aem.actionId, aem.axisContribution, aem.invert);
				controllerMap.CreateElementMap(assignment);
			}
		}
	}

	private Joystick FindJoystickPrecise(ControllerAssignmentSaveInfo.JoystickInfo joystickInfo)
	{
		if (joystickInfo == null)
		{
			return null;
		}
		if (joystickInfo.instanceGuid == Guid.Empty)
		{
			return null;
		}
		IList<Joystick> joysticks = ReInput.controllers.Joysticks;
		for (int i = 0; i < joysticks.Count; i++)
		{
			if (joysticks[i].deviceInstanceGuid == joystickInfo.instanceGuid)
			{
				return joysticks[i];
			}
		}
		return null;
	}

	private bool TryFindJoysticksImprecise(ControllerAssignmentSaveInfo.JoystickInfo joystickInfo, out List<Joystick> matches)
	{
		matches = null;
		if (joystickInfo == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(joystickInfo.hardwareIdentifier))
		{
			return false;
		}
		IList<Joystick> joysticks = ReInput.controllers.Joysticks;
		for (int i = 0; i < joysticks.Count; i++)
		{
			if (string.Equals(joysticks[i].hardwareIdentifier, joystickInfo.hardwareIdentifier, StringComparison.OrdinalIgnoreCase))
			{
				if (matches == null)
				{
					matches = new List<Joystick>();
				}
				matches.Add(joysticks[i]);
			}
		}
		return matches != null;
	}

	private static int GetDuplicateIndex(Player player, ControllerIdentifier controllerIdentifier)
	{
		Controller controller = ReInput.controllers.GetController(controllerIdentifier);
		if (controller == null)
		{
			return 0;
		}
		int duplicateCount = 0;
		foreach (Controller c in player.controllers.Controllers)
		{
			if (c.type != controller.type)
			{
				continue;
			}
			bool isRecognized = false;
			if (controller.type == ControllerType.Joystick)
			{
				if ((c as Joystick).hardwareTypeGuid != controller.hardwareTypeGuid)
				{
					continue;
				}
				if (controller.hardwareTypeGuid != Guid.Empty)
				{
					isRecognized = true;
				}
			}
			if (isRecognized || !(c.hardwareIdentifier != controller.hardwareIdentifier))
			{
				if (c == controller)
				{
					return duplicateCount;
				}
				duplicateCount++;
			}
		}
		return duplicateCount;
	}

	private void RefreshLayoutManager(int playerId)
	{
		ReInput.players.GetPlayer(playerId)?.controllers.maps.layoutManager.Apply();
	}

	private static Type GetControllerMapType(ControllerType controllerType)
	{
		switch (controllerType)
		{
		case ControllerType.Custom:
			return typeof(CustomControllerMap);
		case ControllerType.Joystick:
			return typeof(JoystickMap);
		case ControllerType.Keyboard:
			return typeof(KeyboardMap);
		case ControllerType.Mouse:
			return typeof(MouseMap);
		default:
			Debug.LogWarning("Rewired: Unknown ControllerType " + controllerType);
			return null;
		}
	}
}
