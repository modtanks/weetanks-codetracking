using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class ControlRemappingDemo1 : MonoBehaviour
{
	private class ControllerSelection
	{
		private int _id;

		private int _idPrev;

		private ControllerType _type;

		private ControllerType _typePrev;

		public int id
		{
			get
			{
				return _id;
			}
			set
			{
				_idPrev = _id;
				_id = value;
			}
		}

		public ControllerType type
		{
			get
			{
				return _type;
			}
			set
			{
				_typePrev = _type;
				_type = value;
			}
		}

		public int idPrev => _idPrev;

		public ControllerType typePrev => _typePrev;

		public bool hasSelection => _id >= 0;

		public ControllerSelection()
		{
			Clear();
		}

		public void Set(int id, ControllerType type)
		{
			this.id = id;
			this.type = type;
		}

		public void Clear()
		{
			_id = -1;
			_idPrev = -1;
			_type = ControllerType.Joystick;
			_typePrev = ControllerType.Joystick;
		}
	}

	private class DialogHelper
	{
		public enum DialogType
		{
			None = 0,
			JoystickConflict = 1,
			ElementConflict = 2,
			KeyConflict = 3,
			DeleteAssignmentConfirmation = 10,
			AssignElement = 11
		}

		private const float openBusyDelay = 0.25f;

		private const float closeBusyDelay = 0.1f;

		private DialogType _type;

		private bool _enabled;

		private float _busyTime;

		private bool _busyTimerRunning;

		private Action<int> drawWindowDelegate;

		private GUI.WindowFunction drawWindowFunction;

		private WindowProperties windowProperties;

		private int currentActionId;

		private Action<int, UserResponse> resultCallback;

		private float busyTimer
		{
			get
			{
				if (!_busyTimerRunning)
				{
					return 0f;
				}
				return _busyTime - Time.realtimeSinceStartup;
			}
		}

		public bool enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (value)
				{
					if (_type != 0)
					{
						StateChanged(0.25f);
					}
				}
				else
				{
					_enabled = value;
					_type = DialogType.None;
					StateChanged(0.1f);
				}
			}
		}

		public DialogType type
		{
			get
			{
				if (!_enabled)
				{
					return DialogType.None;
				}
				return _type;
			}
			set
			{
				if (value == DialogType.None)
				{
					_enabled = false;
					StateChanged(0.1f);
				}
				else
				{
					_enabled = true;
					StateChanged(0.25f);
				}
				_type = value;
			}
		}

		public bool busy => _busyTimerRunning;

		public DialogHelper()
		{
			drawWindowDelegate = DrawWindow;
			drawWindowFunction = drawWindowDelegate.Invoke;
		}

		public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback)
		{
			StartModal(queueActionId, type, windowProperties, resultCallback, -1f);
		}

		public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback, float openBusyDelay)
		{
			currentActionId = queueActionId;
			this.windowProperties = windowProperties;
			this.type = type;
			this.resultCallback = resultCallback;
			if (openBusyDelay >= 0f)
			{
				StateChanged(openBusyDelay);
			}
		}

		public void Update()
		{
			Draw();
			UpdateTimers();
		}

		public void Draw()
		{
			if (_enabled)
			{
				bool origGuiEnabled = GUI.enabled;
				GUI.enabled = true;
				GUILayout.Window(windowProperties.windowId, windowProperties.rect, drawWindowFunction, windowProperties.title);
				GUI.FocusWindow(windowProperties.windowId);
				if (GUI.enabled != origGuiEnabled)
				{
					GUI.enabled = origGuiEnabled;
				}
			}
		}

		public void DrawConfirmButton()
		{
			DrawConfirmButton("Confirm");
		}

		public void DrawConfirmButton(string title)
		{
			bool origGUIEnabled = GUI.enabled;
			if (busy)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button(title))
			{
				Confirm(UserResponse.Confirm);
			}
			if (GUI.enabled != origGUIEnabled)
			{
				GUI.enabled = origGUIEnabled;
			}
		}

		public void DrawConfirmButton(UserResponse response)
		{
			DrawConfirmButton(response, "Confirm");
		}

		public void DrawConfirmButton(UserResponse response, string title)
		{
			bool origGUIEnabled = GUI.enabled;
			if (busy)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button(title))
			{
				Confirm(response);
			}
			if (GUI.enabled != origGUIEnabled)
			{
				GUI.enabled = origGUIEnabled;
			}
		}

		public void DrawCancelButton()
		{
			DrawCancelButton("Cancel");
		}

		public void DrawCancelButton(string title)
		{
			bool origGUIEnabled = GUI.enabled;
			if (busy)
			{
				GUI.enabled = false;
			}
			if (GUILayout.Button(title))
			{
				Cancel();
			}
			if (GUI.enabled != origGUIEnabled)
			{
				GUI.enabled = origGUIEnabled;
			}
		}

		public void Confirm()
		{
			Confirm(UserResponse.Confirm);
		}

		public void Confirm(UserResponse response)
		{
			resultCallback(currentActionId, response);
			Close();
		}

		public void Cancel()
		{
			resultCallback(currentActionId, UserResponse.Cancel);
			Close();
		}

		private void DrawWindow(int windowId)
		{
			windowProperties.windowDrawDelegate(windowProperties.title, windowProperties.message);
		}

		private void UpdateTimers()
		{
			if (_busyTimerRunning && busyTimer <= 0f)
			{
				_busyTimerRunning = false;
			}
		}

		private void StartBusyTimer(float time)
		{
			_busyTime = time + Time.realtimeSinceStartup;
			_busyTimerRunning = true;
		}

		private void Close()
		{
			Reset();
			StateChanged(0.1f);
		}

		private void StateChanged(float delay)
		{
			StartBusyTimer(delay);
		}

		private void Reset()
		{
			_enabled = false;
			_type = DialogType.None;
			currentActionId = -1;
			resultCallback = null;
		}

		private void ResetTimers()
		{
			_busyTimerRunning = false;
		}

		public void FullReset()
		{
			Reset();
			ResetTimers();
		}
	}

	private abstract class QueueEntry
	{
		public enum State
		{
			Waiting,
			Confirmed,
			Canceled
		}

		private static int uidCounter;

		public int id { get; protected set; }

		public QueueActionType queueActionType { get; protected set; }

		public State state { get; protected set; }

		public UserResponse response { get; protected set; }

		protected static int nextId
		{
			get
			{
				int id = uidCounter;
				uidCounter++;
				return id;
			}
		}

		public QueueEntry(QueueActionType queueActionType)
		{
			id = nextId;
			this.queueActionType = queueActionType;
		}

		public void Confirm(UserResponse response)
		{
			state = State.Confirmed;
			this.response = response;
		}

		public void Cancel()
		{
			state = State.Canceled;
		}
	}

	private class JoystickAssignmentChange : QueueEntry
	{
		public int playerId { get; private set; }

		public int joystickId { get; private set; }

		public bool assign { get; private set; }

		public JoystickAssignmentChange(int newPlayerId, int joystickId, bool assign)
			: base(QueueActionType.JoystickAssignment)
		{
			playerId = newPlayerId;
			this.joystickId = joystickId;
			this.assign = assign;
		}
	}

	private class ElementAssignmentChange : QueueEntry
	{
		public ElementAssignmentChangeType changeType { get; set; }

		public InputMapper.Context context { get; private set; }

		public ElementAssignmentChange(ElementAssignmentChangeType changeType, InputMapper.Context context)
			: base(QueueActionType.ElementAssignment)
		{
			this.changeType = changeType;
			this.context = context;
		}

		public ElementAssignmentChange(ElementAssignmentChange other)
			: this(other.changeType, other.context.Clone())
		{
		}
	}

	private class FallbackJoystickIdentification : QueueEntry
	{
		public int joystickId { get; private set; }

		public string joystickName { get; private set; }

		public FallbackJoystickIdentification(int joystickId, string joystickName)
			: base(QueueActionType.FallbackJoystickIdentification)
		{
			this.joystickId = joystickId;
			this.joystickName = joystickName;
		}
	}

	private class Calibration : QueueEntry
	{
		public int selectedElementIdentifierId;

		public bool recording;

		public Player player { get; private set; }

		public ControllerType controllerType { get; private set; }

		public Joystick joystick { get; private set; }

		public CalibrationMap calibrationMap { get; private set; }

		public Calibration(Player player, Joystick joystick, CalibrationMap calibrationMap)
			: base(QueueActionType.Calibrate)
		{
			this.player = player;
			this.joystick = joystick;
			this.calibrationMap = calibrationMap;
			selectedElementIdentifierId = -1;
		}
	}

	private struct WindowProperties
	{
		public int windowId;

		public Rect rect;

		public Action<string, string> windowDrawDelegate;

		public string title;

		public string message;
	}

	private enum QueueActionType
	{
		None,
		JoystickAssignment,
		ElementAssignment,
		FallbackJoystickIdentification,
		Calibrate
	}

	private enum ElementAssignmentChangeType
	{
		Add,
		Replace,
		Remove,
		ReassignOrRemove,
		ConflictCheck
	}

	public enum UserResponse
	{
		Confirm,
		Cancel,
		Custom1,
		Custom2
	}

	private const float defaultModalWidth = 250f;

	private const float defaultModalHeight = 200f;

	private const float assignmentTimeout = 5f;

	private DialogHelper dialog;

	private InputMapper inputMapper = new InputMapper();

	private InputMapper.ConflictFoundEventData conflictFoundEventData;

	private bool guiState;

	private bool busy;

	private bool pageGUIState;

	private Player selectedPlayer;

	private int selectedMapCategoryId;

	private ControllerSelection selectedController;

	private ControllerMap selectedMap;

	private bool showMenu;

	private bool startListening;

	private Vector2 actionScrollPos;

	private Vector2 calibrateScrollPos;

	private Queue<QueueEntry> actionQueue;

	private bool setupFinished;

	[NonSerialized]
	private bool initialized;

	private bool isCompiling;

	private GUIStyle style_wordWrap;

	private GUIStyle style_centeredBox;

	private void Awake()
	{
		inputMapper.options.timeout = 5f;
		inputMapper.options.ignoreMouseXAxis = true;
		inputMapper.options.ignoreMouseYAxis = true;
		Initialize();
	}

	private void OnEnable()
	{
		Subscribe();
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void Initialize()
	{
		dialog = new DialogHelper();
		actionQueue = new Queue<QueueEntry>();
		selectedController = new ControllerSelection();
		ReInput.ControllerConnectedEvent += JoystickConnected;
		ReInput.ControllerPreDisconnectEvent += JoystickPreDisconnect;
		ReInput.ControllerDisconnectedEvent += JoystickDisconnected;
		ResetAll();
		initialized = true;
		ReInput.userDataStore.Load();
		if (ReInput.unityJoystickIdentificationRequired)
		{
			IdentifyAllJoysticks();
		}
	}

	private void Setup()
	{
		if (!setupFinished)
		{
			style_wordWrap = new GUIStyle(GUI.skin.label);
			style_wordWrap.wordWrap = true;
			style_centeredBox = new GUIStyle(GUI.skin.box);
			style_centeredBox.alignment = TextAnchor.MiddleCenter;
			setupFinished = true;
		}
	}

	private void Subscribe()
	{
		Unsubscribe();
		inputMapper.ConflictFoundEvent += OnConflictFound;
		inputMapper.StoppedEvent += OnStopped;
	}

	private void Unsubscribe()
	{
		inputMapper.RemoveAllEventListeners();
	}

	public void OnGUI()
	{
		if (initialized)
		{
			Setup();
			HandleMenuControl();
			if (!showMenu)
			{
				DrawInitialScreen();
				return;
			}
			SetGUIStateStart();
			ProcessQueue();
			DrawPage();
			ShowDialog();
			SetGUIStateEnd();
			busy = false;
		}
	}

	private void HandleMenuControl()
	{
		if (!dialog.enabled && Event.current.type == EventType.Layout && ReInput.players.GetSystemPlayer().GetButtonDown("Menu"))
		{
			if (showMenu)
			{
				ReInput.userDataStore.Save();
				Close();
			}
			else
			{
				Open();
			}
		}
	}

	private void Close()
	{
		ClearWorkingVars();
		showMenu = false;
	}

	private void Open()
	{
		showMenu = true;
	}

	private void DrawInitialScreen()
	{
		ActionElementMap map = ReInput.players.GetSystemPlayer().controllers.maps.GetFirstElementMapWithAction("Menu", skipDisabledMaps: true);
		GUIContent content = ((map == null) ? new GUIContent("There is no element assigned to open the menu!") : new GUIContent("Press " + map.elementIdentifierName + " to open the menu."));
		GUILayout.BeginArea(GetScreenCenteredRect(300f, 50f));
		GUILayout.Box(content, style_centeredBox, GUILayout.ExpandHeight(expand: true), GUILayout.ExpandWidth(expand: true));
		GUILayout.EndArea();
	}

	private void DrawPage()
	{
		if (GUI.enabled != pageGUIState)
		{
			GUI.enabled = pageGUIState;
		}
		Rect screenRect = new Rect(((float)Screen.width - (float)Screen.width * 0.9f) * 0.5f, ((float)Screen.height - (float)Screen.height * 0.9f) * 0.5f, (float)Screen.width * 0.9f, (float)Screen.height * 0.9f);
		GUILayout.BeginArea(screenRect);
		DrawPlayerSelector();
		DrawJoystickSelector();
		DrawMouseAssignment();
		DrawControllerSelector();
		DrawCalibrateButton();
		DrawMapCategories();
		actionScrollPos = GUILayout.BeginScrollView(actionScrollPos);
		DrawCategoryActions();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	private void DrawPlayerSelector()
	{
		if (ReInput.players.allPlayerCount == 0)
		{
			GUILayout.Label("There are no players.");
			return;
		}
		GUILayout.Space(15f);
		GUILayout.Label("Players:");
		GUILayout.BeginHorizontal();
		foreach (Player player in ReInput.players.GetPlayers(includeSystemPlayer: true))
		{
			if (selectedPlayer == null)
			{
				selectedPlayer = player;
			}
			bool prevValue = ((player == selectedPlayer) ? true : false);
			bool value = GUILayout.Toggle(prevValue, (player.descriptiveName != string.Empty) ? player.descriptiveName : player.name, "Button", GUILayout.ExpandWidth(expand: false));
			if (value != prevValue && value)
			{
				selectedPlayer = player;
				selectedController.Clear();
				selectedMapCategoryId = -1;
			}
		}
		GUILayout.EndHorizontal();
	}

	private void DrawMouseAssignment()
	{
		bool origGuiEnabled = GUI.enabled;
		if (selectedPlayer == null)
		{
			GUI.enabled = false;
		}
		GUILayout.Space(15f);
		GUILayout.Label("Assign Mouse:");
		GUILayout.BeginHorizontal();
		bool prevValue = ((selectedPlayer != null && selectedPlayer.controllers.hasMouse) ? true : false);
		bool value = GUILayout.Toggle(prevValue, "Assign Mouse", "Button", GUILayout.ExpandWidth(expand: false));
		if (value != prevValue)
		{
			if (value)
			{
				selectedPlayer.controllers.hasMouse = true;
				foreach (Player player in ReInput.players.Players)
				{
					if (player != selectedPlayer)
					{
						player.controllers.hasMouse = false;
					}
				}
			}
			else
			{
				selectedPlayer.controllers.hasMouse = false;
			}
		}
		GUILayout.EndHorizontal();
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
	}

	private void DrawJoystickSelector()
	{
		bool origGuiEnabled = GUI.enabled;
		if (selectedPlayer == null)
		{
			GUI.enabled = false;
		}
		GUILayout.Space(15f);
		GUILayout.Label("Assign Joysticks:");
		GUILayout.BeginHorizontal();
		bool prevValue = ((selectedPlayer == null || selectedPlayer.controllers.joystickCount == 0) ? true : false);
		bool value = GUILayout.Toggle(prevValue, "None", "Button", GUILayout.ExpandWidth(expand: false));
		if (value != prevValue)
		{
			selectedPlayer.controllers.ClearControllersOfType(ControllerType.Joystick);
			ControllerSelectionChanged();
		}
		if (selectedPlayer != null)
		{
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				prevValue = selectedPlayer.controllers.ContainsController(joystick);
				value = GUILayout.Toggle(prevValue, joystick.name, "Button", GUILayout.ExpandWidth(expand: false));
				if (value != prevValue)
				{
					EnqueueAction(new JoystickAssignmentChange(selectedPlayer.id, joystick.id, value));
				}
			}
		}
		GUILayout.EndHorizontal();
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
	}

	private void DrawControllerSelector()
	{
		if (selectedPlayer == null)
		{
			return;
		}
		bool origGuiEnabled = GUI.enabled;
		GUILayout.Space(15f);
		GUILayout.Label("Controller to Map:");
		GUILayout.BeginHorizontal();
		if (!selectedController.hasSelection)
		{
			selectedController.Set(0, ControllerType.Keyboard);
			ControllerSelectionChanged();
		}
		bool prevValue = selectedController.type == ControllerType.Keyboard;
		bool value = GUILayout.Toggle(prevValue, "Keyboard", "Button", GUILayout.ExpandWidth(expand: false));
		if (value != prevValue)
		{
			selectedController.Set(0, ControllerType.Keyboard);
			ControllerSelectionChanged();
		}
		if (!selectedPlayer.controllers.hasMouse)
		{
			GUI.enabled = false;
		}
		prevValue = selectedController.type == ControllerType.Mouse;
		value = GUILayout.Toggle(prevValue, "Mouse", "Button", GUILayout.ExpandWidth(expand: false));
		if (value != prevValue)
		{
			selectedController.Set(0, ControllerType.Mouse);
			ControllerSelectionChanged();
		}
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
		foreach (Joystick i in selectedPlayer.controllers.Joysticks)
		{
			prevValue = selectedController.type == ControllerType.Joystick && selectedController.id == i.id;
			value = GUILayout.Toggle(prevValue, i.name, "Button", GUILayout.ExpandWidth(expand: false));
			if (value != prevValue)
			{
				selectedController.Set(i.id, ControllerType.Joystick);
				ControllerSelectionChanged();
			}
		}
		GUILayout.EndHorizontal();
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
	}

	private void DrawCalibrateButton()
	{
		if (selectedPlayer == null)
		{
			return;
		}
		bool origGuiEnabled = GUI.enabled;
		GUILayout.Space(10f);
		Controller controller = (selectedController.hasSelection ? selectedPlayer.controllers.GetController(selectedController.type, selectedController.id) : null);
		if (controller == null || selectedController.type != ControllerType.Joystick)
		{
			GUI.enabled = false;
			GUILayout.Button("Select a controller to calibrate", GUILayout.ExpandWidth(expand: false));
			if (GUI.enabled != origGuiEnabled)
			{
				GUI.enabled = origGuiEnabled;
			}
		}
		else if (GUILayout.Button("Calibrate " + controller.name, GUILayout.ExpandWidth(expand: false)) && controller is Joystick joystick)
		{
			CalibrationMap calibrationMap = joystick.calibrationMap;
			if (calibrationMap != null)
			{
				EnqueueAction(new Calibration(selectedPlayer, joystick, calibrationMap));
			}
		}
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
	}

	private void DrawMapCategories()
	{
		if (selectedPlayer == null || !selectedController.hasSelection)
		{
			return;
		}
		bool origGuiEnabled = GUI.enabled;
		GUILayout.Space(15f);
		GUILayout.Label("Categories:");
		GUILayout.BeginHorizontal();
		foreach (InputMapCategory category in ReInput.mapping.UserAssignableMapCategories)
		{
			if (!selectedPlayer.controllers.maps.ContainsMapInCategory(selectedController.type, category.id))
			{
				GUI.enabled = false;
			}
			else if (selectedMapCategoryId < 0)
			{
				selectedMapCategoryId = category.id;
				selectedMap = selectedPlayer.controllers.maps.GetFirstMapInCategory(selectedController.type, selectedController.id, category.id);
			}
			bool prevValue = ((category.id == selectedMapCategoryId) ? true : false);
			bool value = GUILayout.Toggle(prevValue, (category.descriptiveName != string.Empty) ? category.descriptiveName : category.name, "Button", GUILayout.ExpandWidth(expand: false));
			if (value != prevValue)
			{
				selectedMapCategoryId = category.id;
				selectedMap = selectedPlayer.controllers.maps.GetFirstMapInCategory(selectedController.type, selectedController.id, category.id);
			}
			if (GUI.enabled != origGuiEnabled)
			{
				GUI.enabled = origGuiEnabled;
			}
		}
		GUILayout.EndHorizontal();
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
	}

	private void DrawCategoryActions()
	{
		if (selectedPlayer == null || selectedMapCategoryId < 0)
		{
			return;
		}
		bool origGuiEnabled = GUI.enabled;
		if (selectedMap == null)
		{
			return;
		}
		GUILayout.Space(15f);
		GUILayout.Label("Actions:");
		InputMapCategory mapCategory = ReInput.mapping.GetMapCategory(selectedMapCategoryId);
		if (mapCategory == null)
		{
			return;
		}
		InputCategory actionCategory = ReInput.mapping.GetActionCategory(mapCategory.name);
		if (actionCategory == null)
		{
			return;
		}
		float labelWidth = 150f;
		foreach (InputAction action in ReInput.mapping.ActionsInCategory(actionCategory.id))
		{
			string name = ((action.descriptiveName != string.Empty) ? action.descriptiveName : action.name);
			if (action.type == InputActionType.Button)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(name, GUILayout.Width(labelWidth));
				DrawAddActionMapButton(selectedPlayer.id, action, AxisRange.Positive, selectedController, selectedMap);
				foreach (ActionElementMap elementMap4 in selectedMap.AllMaps)
				{
					if (elementMap4.actionId == action.id)
					{
						DrawActionAssignmentButton(selectedPlayer.id, action, AxisRange.Positive, selectedController, selectedMap, elementMap4);
					}
				}
				GUILayout.EndHorizontal();
			}
			else
			{
				if (action.type != 0)
				{
					continue;
				}
				if (selectedController.type != 0)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(name, GUILayout.Width(labelWidth));
					DrawAddActionMapButton(selectedPlayer.id, action, AxisRange.Full, selectedController, selectedMap);
					foreach (ActionElementMap elementMap3 in selectedMap.AllMaps)
					{
						if (elementMap3.actionId == action.id && elementMap3.elementType != ControllerElementType.Button && elementMap3.axisType != AxisType.Split)
						{
							DrawActionAssignmentButton(selectedPlayer.id, action, AxisRange.Full, selectedController, selectedMap, elementMap3);
							DrawInvertButton(selectedPlayer.id, action, Pole.Positive, selectedController, selectedMap, elementMap3);
						}
					}
					GUILayout.EndHorizontal();
				}
				string positiveName = ((action.positiveDescriptiveName != string.Empty) ? action.positiveDescriptiveName : (action.descriptiveName + " +"));
				GUILayout.BeginHorizontal();
				GUILayout.Label(positiveName, GUILayout.Width(labelWidth));
				DrawAddActionMapButton(selectedPlayer.id, action, AxisRange.Positive, selectedController, selectedMap);
				foreach (ActionElementMap elementMap2 in selectedMap.AllMaps)
				{
					if (elementMap2.actionId == action.id && elementMap2.axisContribution == Pole.Positive && elementMap2.axisType != AxisType.Normal)
					{
						DrawActionAssignmentButton(selectedPlayer.id, action, AxisRange.Positive, selectedController, selectedMap, elementMap2);
					}
				}
				GUILayout.EndHorizontal();
				string negativeName = ((action.negativeDescriptiveName != string.Empty) ? action.negativeDescriptiveName : (action.descriptiveName + " -"));
				GUILayout.BeginHorizontal();
				GUILayout.Label(negativeName, GUILayout.Width(labelWidth));
				DrawAddActionMapButton(selectedPlayer.id, action, AxisRange.Negative, selectedController, selectedMap);
				foreach (ActionElementMap elementMap in selectedMap.AllMaps)
				{
					if (elementMap.actionId == action.id && elementMap.axisContribution == Pole.Negative && elementMap.axisType != AxisType.Normal)
					{
						DrawActionAssignmentButton(selectedPlayer.id, action, AxisRange.Negative, selectedController, selectedMap, elementMap);
					}
				}
				GUILayout.EndHorizontal();
			}
		}
		if (GUI.enabled != origGuiEnabled)
		{
			GUI.enabled = origGuiEnabled;
		}
	}

	private void DrawActionAssignmentButton(int playerId, InputAction action, AxisRange actionRange, ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
	{
		if (GUILayout.Button(elementMap.elementIdentifierName, GUILayout.ExpandWidth(expand: false), GUILayout.MinWidth(30f)))
		{
			InputMapper.Context context = new InputMapper.Context
			{
				actionId = action.id,
				actionRange = actionRange,
				controllerMap = controllerMap,
				actionElementMapToReplace = elementMap
			};
			EnqueueAction(new ElementAssignmentChange(ElementAssignmentChangeType.ReassignOrRemove, context));
			startListening = true;
		}
		GUILayout.Space(4f);
	}

	private void DrawInvertButton(int playerId, InputAction action, Pole actionAxisContribution, ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
	{
		bool value = elementMap.invert;
		bool newValue = GUILayout.Toggle(value, "Invert", GUILayout.ExpandWidth(expand: false));
		if (newValue != value)
		{
			elementMap.invert = newValue;
		}
		GUILayout.Space(10f);
	}

	private void DrawAddActionMapButton(int playerId, InputAction action, AxisRange actionRange, ControllerSelection controller, ControllerMap controllerMap)
	{
		if (GUILayout.Button("Add...", GUILayout.ExpandWidth(expand: false)))
		{
			InputMapper.Context context = new InputMapper.Context
			{
				actionId = action.id,
				actionRange = actionRange,
				controllerMap = controllerMap
			};
			EnqueueAction(new ElementAssignmentChange(ElementAssignmentChangeType.Add, context));
			startListening = true;
		}
		GUILayout.Space(10f);
	}

	private void ShowDialog()
	{
		dialog.Update();
	}

	private void DrawModalWindow(string title, string message)
	{
		if (dialog.enabled)
		{
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			dialog.DrawConfirmButton("Okay");
			GUILayout.FlexibleSpace();
			dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}
	}

	private void DrawModalWindow_OkayOnly(string title, string message)
	{
		if (dialog.enabled)
		{
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			dialog.DrawConfirmButton("Okay");
			GUILayout.EndHorizontal();
		}
	}

	private void DrawElementAssignmentWindow(string title, string message)
	{
		if (!dialog.enabled)
		{
			return;
		}
		GUILayout.Space(5f);
		GUILayout.Label(message, style_wordWrap);
		GUILayout.FlexibleSpace();
		if (!(actionQueue.Peek() is ElementAssignmentChange entry))
		{
			dialog.Cancel();
			return;
		}
		float time;
		if (!dialog.busy)
		{
			if (startListening && inputMapper.status == InputMapper.Status.Idle)
			{
				inputMapper.Start(entry.context);
				startListening = false;
			}
			if (conflictFoundEventData != null)
			{
				dialog.Confirm();
				return;
			}
			time = inputMapper.timeRemaining;
			if (time == 0f)
			{
				dialog.Cancel();
				return;
			}
		}
		else
		{
			time = inputMapper.options.timeout;
		}
		GUILayout.Label("Assignment will be canceled in " + (int)Mathf.Ceil(time) + "...", style_wordWrap);
	}

	private void DrawElementAssignmentProtectedConflictWindow(string title, string message)
	{
		if (dialog.enabled)
		{
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap);
			GUILayout.FlexibleSpace();
			if (!(actionQueue.Peek() is ElementAssignmentChange))
			{
				dialog.Cancel();
				return;
			}
			GUILayout.BeginHorizontal();
			dialog.DrawConfirmButton(UserResponse.Custom1, "Add");
			GUILayout.FlexibleSpace();
			dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}
	}

	private void DrawElementAssignmentNormalConflictWindow(string title, string message)
	{
		if (dialog.enabled)
		{
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap);
			GUILayout.FlexibleSpace();
			if (!(actionQueue.Peek() is ElementAssignmentChange))
			{
				dialog.Cancel();
				return;
			}
			GUILayout.BeginHorizontal();
			dialog.DrawConfirmButton(UserResponse.Confirm, "Replace");
			GUILayout.FlexibleSpace();
			dialog.DrawConfirmButton(UserResponse.Custom1, "Add");
			GUILayout.FlexibleSpace();
			dialog.DrawCancelButton();
			GUILayout.EndHorizontal();
		}
	}

	private void DrawReassignOrRemoveElementAssignmentWindow(string title, string message)
	{
		if (dialog.enabled)
		{
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap);
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			dialog.DrawConfirmButton("Reassign");
			GUILayout.FlexibleSpace();
			dialog.DrawCancelButton("Remove");
			GUILayout.EndHorizontal();
		}
	}

	private void DrawFallbackJoystickIdentificationWindow(string title, string message)
	{
		if (!dialog.enabled)
		{
			return;
		}
		if (!(actionQueue.Peek() is FallbackJoystickIdentification entry))
		{
			dialog.Cancel();
			return;
		}
		GUILayout.Space(5f);
		GUILayout.Label(message, style_wordWrap);
		GUILayout.Label("Press any button or axis on \"" + entry.joystickName + "\" now.", style_wordWrap);
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Skip"))
		{
			dialog.Cancel();
		}
		else if (!dialog.busy && ReInput.controllers.SetUnityJoystickIdFromAnyButtonOrAxisPress(entry.joystickId, 0.8f, positiveAxesOnly: false))
		{
			dialog.Confirm();
		}
	}

	private void DrawCalibrationWindow(string title, string message)
	{
		if (!dialog.enabled)
		{
			return;
		}
		if (!(actionQueue.Peek() is Calibration entry))
		{
			dialog.Cancel();
			return;
		}
		GUILayout.Space(5f);
		GUILayout.Label(message, style_wordWrap);
		GUILayout.Space(20f);
		GUILayout.BeginHorizontal();
		bool origGUIEnabled = GUI.enabled;
		GUILayout.BeginVertical(GUILayout.Width(200f));
		calibrateScrollPos = GUILayout.BeginScrollView(calibrateScrollPos);
		if (entry.recording)
		{
			GUI.enabled = false;
		}
		IList<ControllerElementIdentifier> axisIdentifiers = entry.joystick.AxisElementIdentifiers;
		for (int i = 0; i < axisIdentifiers.Count; i++)
		{
			ControllerElementIdentifier identifier = axisIdentifiers[i];
			bool isSelected = entry.selectedElementIdentifierId == identifier.id;
			bool newValue = GUILayout.Toggle(isSelected, identifier.name, "Button", GUILayout.ExpandWidth(expand: false));
			if (isSelected != newValue)
			{
				entry.selectedElementIdentifierId = identifier.id;
			}
		}
		if (GUI.enabled != origGUIEnabled)
		{
			GUI.enabled = origGUIEnabled;
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUILayout.Width(200f));
		if (entry.selectedElementIdentifierId >= 0)
		{
			float axisValue = entry.joystick.GetAxisRawById(entry.selectedElementIdentifierId);
			GUILayout.Label("Raw Value: " + axisValue);
			int axisIndex = entry.joystick.GetAxisIndexById(entry.selectedElementIdentifierId);
			AxisCalibration axis = entry.calibrationMap.GetAxis(axisIndex);
			GUILayout.Label("Calibrated Value: " + entry.joystick.GetAxisById(entry.selectedElementIdentifierId));
			GUILayout.Label("Zero: " + axis.calibratedZero);
			GUILayout.Label("Min: " + axis.calibratedMin);
			GUILayout.Label("Max: " + axis.calibratedMax);
			GUILayout.Label("Dead Zone: " + axis.deadZone);
			GUILayout.Space(15f);
			bool newEnabled = GUILayout.Toggle(axis.enabled, "Enabled", "Button", GUILayout.ExpandWidth(expand: false));
			if (axis.enabled != newEnabled)
			{
				axis.enabled = newEnabled;
			}
			GUILayout.Space(10f);
			bool newRecording = GUILayout.Toggle(entry.recording, "Record Min/Max", "Button", GUILayout.ExpandWidth(expand: false));
			if (newRecording != entry.recording)
			{
				if (newRecording)
				{
					axis.calibratedMax = 0f;
					axis.calibratedMin = 0f;
				}
				entry.recording = newRecording;
			}
			if (entry.recording)
			{
				axis.calibratedMin = Mathf.Min(axis.calibratedMin, axisValue, axis.calibratedMin);
				axis.calibratedMax = Mathf.Max(axis.calibratedMax, axisValue, axis.calibratedMax);
				GUI.enabled = false;
			}
			if (GUILayout.Button("Set Zero", GUILayout.ExpandWidth(expand: false)))
			{
				axis.calibratedZero = axisValue;
			}
			if (GUILayout.Button("Set Dead Zone", GUILayout.ExpandWidth(expand: false)))
			{
				axis.deadZone = axisValue;
			}
			bool newInvert = GUILayout.Toggle(axis.invert, "Invert", "Button", GUILayout.ExpandWidth(expand: false));
			if (axis.invert != newInvert)
			{
				axis.invert = newInvert;
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Reset", GUILayout.ExpandWidth(expand: false)))
			{
				axis.Reset();
			}
			if (GUI.enabled != origGUIEnabled)
			{
				GUI.enabled = origGUIEnabled;
			}
		}
		else
		{
			GUILayout.Label("Select an axis to begin.");
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		if (entry.recording)
		{
			GUI.enabled = false;
		}
		if (GUILayout.Button("Close"))
		{
			calibrateScrollPos = default(Vector2);
			dialog.Confirm();
		}
		if (GUI.enabled != origGUIEnabled)
		{
			GUI.enabled = origGUIEnabled;
		}
	}

	private void DialogResultCallback(int queueActionId, UserResponse response)
	{
		foreach (QueueEntry entry in actionQueue)
		{
			if (entry.id != queueActionId)
			{
				continue;
			}
			if (response != UserResponse.Cancel)
			{
				entry.Confirm(response);
			}
			else
			{
				entry.Cancel();
			}
			break;
		}
	}

	private Rect GetScreenCenteredRect(float width, float height)
	{
		return new Rect((float)Screen.width * 0.5f - width * 0.5f, (float)((double)Screen.height * 0.5 - (double)(height * 0.5f)), width, height);
	}

	private void EnqueueAction(QueueEntry entry)
	{
		if (entry != null)
		{
			busy = true;
			GUI.enabled = false;
			actionQueue.Enqueue(entry);
		}
	}

	private void ProcessQueue()
	{
		if (dialog.enabled || busy || actionQueue.Count == 0)
		{
			return;
		}
		while (actionQueue.Count > 0)
		{
			QueueEntry queueEntry = actionQueue.Peek();
			bool goNext = false;
			switch (queueEntry.queueActionType)
			{
			case QueueActionType.JoystickAssignment:
				goNext = ProcessJoystickAssignmentChange((JoystickAssignmentChange)queueEntry);
				break;
			case QueueActionType.ElementAssignment:
				goNext = ProcessElementAssignmentChange((ElementAssignmentChange)queueEntry);
				break;
			case QueueActionType.FallbackJoystickIdentification:
				goNext = ProcessFallbackJoystickIdentification((FallbackJoystickIdentification)queueEntry);
				break;
			case QueueActionType.Calibrate:
				goNext = ProcessCalibration((Calibration)queueEntry);
				break;
			}
			if (!goNext)
			{
				break;
			}
			actionQueue.Dequeue();
		}
	}

	private bool ProcessJoystickAssignmentChange(JoystickAssignmentChange entry)
	{
		if (entry.state == QueueEntry.State.Canceled)
		{
			return true;
		}
		Player player = ReInput.players.GetPlayer(entry.playerId);
		if (player == null)
		{
			return true;
		}
		if (!entry.assign)
		{
			player.controllers.RemoveController(ControllerType.Joystick, entry.joystickId);
			ControllerSelectionChanged();
			return true;
		}
		if (player.controllers.ContainsController(ControllerType.Joystick, entry.joystickId))
		{
			return true;
		}
		if (!ReInput.controllers.IsJoystickAssigned(entry.joystickId) || entry.state == QueueEntry.State.Confirmed)
		{
			player.controllers.AddController(ControllerType.Joystick, entry.joystickId, removeFromOtherPlayers: true);
			ControllerSelectionChanged();
			return true;
		}
		dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties
		{
			title = "Joystick Reassignment",
			message = "This joystick is already assigned to another player. Do you want to reassign this joystick to " + player.descriptiveName + "?",
			rect = GetScreenCenteredRect(250f, 200f),
			windowDrawDelegate = DrawModalWindow
		}, DialogResultCallback);
		return false;
	}

	private bool ProcessElementAssignmentChange(ElementAssignmentChange entry)
	{
		switch (entry.changeType)
		{
		case ElementAssignmentChangeType.ReassignOrRemove:
			return ProcessRemoveOrReassignElementAssignment(entry);
		case ElementAssignmentChangeType.Remove:
			return ProcessRemoveElementAssignment(entry);
		case ElementAssignmentChangeType.Add:
		case ElementAssignmentChangeType.Replace:
			return ProcessAddOrReplaceElementAssignment(entry);
		case ElementAssignmentChangeType.ConflictCheck:
			return ProcessElementAssignmentConflictCheck(entry);
		default:
			throw new NotImplementedException();
		}
	}

	private bool ProcessRemoveOrReassignElementAssignment(ElementAssignmentChange entry)
	{
		if (entry.context.controllerMap == null)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Canceled)
		{
			ElementAssignmentChange newEntry = new ElementAssignmentChange(entry);
			newEntry.changeType = ElementAssignmentChangeType.Remove;
			actionQueue.Enqueue(newEntry);
			return true;
		}
		if (entry.state == QueueEntry.State.Confirmed)
		{
			ElementAssignmentChange newEntry2 = new ElementAssignmentChange(entry);
			newEntry2.changeType = ElementAssignmentChangeType.Replace;
			actionQueue.Enqueue(newEntry2);
			return true;
		}
		dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
		{
			title = "Reassign or Remove",
			message = "Do you want to reassign or remove this assignment?",
			rect = GetScreenCenteredRect(250f, 200f),
			windowDrawDelegate = DrawReassignOrRemoveElementAssignmentWindow
		}, DialogResultCallback);
		return false;
	}

	private bool ProcessRemoveElementAssignment(ElementAssignmentChange entry)
	{
		if (entry.context.controllerMap == null)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Canceled)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Confirmed)
		{
			entry.context.controllerMap.DeleteElementMap(entry.context.actionElementMapToReplace.id);
			return true;
		}
		dialog.StartModal(entry.id, DialogHelper.DialogType.DeleteAssignmentConfirmation, new WindowProperties
		{
			title = "Remove Assignment",
			message = "Are you sure you want to remove this assignment?",
			rect = GetScreenCenteredRect(250f, 200f),
			windowDrawDelegate = DrawModalWindow
		}, DialogResultCallback);
		return false;
	}

	private bool ProcessAddOrReplaceElementAssignment(ElementAssignmentChange entry)
	{
		if (entry.state == QueueEntry.State.Canceled)
		{
			inputMapper.Stop();
			return true;
		}
		if (entry.state == QueueEntry.State.Confirmed)
		{
			if (Event.current.type != EventType.Layout)
			{
				return false;
			}
			if (conflictFoundEventData != null)
			{
				ElementAssignmentChange newEntry = new ElementAssignmentChange(entry);
				newEntry.changeType = ElementAssignmentChangeType.ConflictCheck;
				actionQueue.Enqueue(newEntry);
			}
			return true;
		}
		string message;
		if (entry.context.controllerMap.controllerType != 0)
		{
			message = ((entry.context.controllerMap.controllerType != ControllerType.Mouse) ? "Press any button or axis to assign it to this action." : "Press any mouse button or axis to assign it to this action.\n\nTo assign mouse movement axes, move the mouse quickly in the direction you want mapped to the action. Slow movements will be ignored.");
		}
		else
		{
			message = ((Application.platform != 0 && Application.platform != RuntimePlatform.OSXPlayer) ? "Press any key to assign it to this action. You may also use the modifier keys Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second." : "Press any key to assign it to this action. You may also use the modifier keys Command, Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second.");
			if (Application.isEditor)
			{
				message += "\n\nNOTE: Some modifier key combinations will not work in the Unity Editor, but they will work in a game build.";
			}
		}
		dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
		{
			title = "Assign",
			message = message,
			rect = GetScreenCenteredRect(250f, 200f),
			windowDrawDelegate = DrawElementAssignmentWindow
		}, DialogResultCallback);
		return false;
	}

	private bool ProcessElementAssignmentConflictCheck(ElementAssignmentChange entry)
	{
		if (entry.context.controllerMap == null)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Canceled)
		{
			inputMapper.Stop();
			return true;
		}
		if (conflictFoundEventData == null)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Confirmed)
		{
			if (entry.response == UserResponse.Confirm)
			{
				conflictFoundEventData.responseCallback(InputMapper.ConflictResponse.Replace);
			}
			else
			{
				if (entry.response != UserResponse.Custom1)
				{
					throw new NotImplementedException();
				}
				conflictFoundEventData.responseCallback(InputMapper.ConflictResponse.Add);
			}
			return true;
		}
		if (conflictFoundEventData.isProtected)
		{
			string message2 = conflictFoundEventData.assignment.elementDisplayName + " is already in use and is protected from reassignment. You cannot remove the protected assignment, but you can still assign the action to this element. If you do so, the element will trigger multiple actions when activated.";
			dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
			{
				title = "Assignment Conflict",
				message = message2,
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawElementAssignmentProtectedConflictWindow
			}, DialogResultCallback);
		}
		else
		{
			string message = conflictFoundEventData.assignment.elementDisplayName + " is already in use. You may replace the other conflicting assignments, add this assignment anyway which will leave multiple actions assigned to this element, or cancel this assignment.";
			dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
			{
				title = "Assignment Conflict",
				message = message,
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawElementAssignmentNormalConflictWindow
			}, DialogResultCallback);
		}
		return false;
	}

	private bool ProcessFallbackJoystickIdentification(FallbackJoystickIdentification entry)
	{
		if (entry.state == QueueEntry.State.Canceled)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Confirmed)
		{
			return true;
		}
		dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties
		{
			title = "Joystick Identification Required",
			message = "A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:",
			rect = GetScreenCenteredRect(250f, 200f),
			windowDrawDelegate = DrawFallbackJoystickIdentificationWindow
		}, DialogResultCallback, 1f);
		return false;
	}

	private bool ProcessCalibration(Calibration entry)
	{
		if (entry.state == QueueEntry.State.Canceled)
		{
			return true;
		}
		if (entry.state == QueueEntry.State.Confirmed)
		{
			return true;
		}
		dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties
		{
			title = "Calibrate Controller",
			message = "Select an axis to calibrate on the " + entry.joystick.name + ".",
			rect = GetScreenCenteredRect(450f, 480f),
			windowDrawDelegate = DrawCalibrationWindow
		}, DialogResultCallback);
		return false;
	}

	private void PlayerSelectionChanged()
	{
		ClearControllerSelection();
	}

	private void ControllerSelectionChanged()
	{
		ClearMapSelection();
	}

	private void ClearControllerSelection()
	{
		selectedController.Clear();
		ClearMapSelection();
	}

	private void ClearMapSelection()
	{
		selectedMapCategoryId = -1;
		selectedMap = null;
	}

	private void ResetAll()
	{
		ClearWorkingVars();
		initialized = false;
		showMenu = false;
	}

	private void ClearWorkingVars()
	{
		selectedPlayer = null;
		ClearMapSelection();
		selectedController.Clear();
		actionScrollPos = default(Vector2);
		dialog.FullReset();
		actionQueue.Clear();
		busy = false;
		startListening = false;
		conflictFoundEventData = null;
		inputMapper.Stop();
	}

	private void SetGUIStateStart()
	{
		guiState = true;
		if (busy)
		{
			guiState = false;
		}
		pageGUIState = guiState && !busy && !dialog.enabled && !dialog.busy;
		if (GUI.enabled != guiState)
		{
			GUI.enabled = guiState;
		}
	}

	private void SetGUIStateEnd()
	{
		guiState = true;
		if (!GUI.enabled)
		{
			GUI.enabled = guiState;
		}
	}

	private void JoystickConnected(ControllerStatusChangedEventArgs args)
	{
		if (ReInput.controllers.IsControllerAssigned(args.controllerType, args.controllerId))
		{
			foreach (Player player in ReInput.players.AllPlayers)
			{
				if (player.controllers.ContainsController(args.controllerType, args.controllerId))
				{
					ReInput.userDataStore.LoadControllerData(player.id, args.controllerType, args.controllerId);
				}
			}
		}
		else
		{
			ReInput.userDataStore.LoadControllerData(args.controllerType, args.controllerId);
		}
		if (ReInput.unityJoystickIdentificationRequired)
		{
			IdentifyAllJoysticks();
		}
	}

	private void JoystickPreDisconnect(ControllerStatusChangedEventArgs args)
	{
		if (selectedController.hasSelection && args.controllerType == selectedController.type && args.controllerId == selectedController.id)
		{
			ClearControllerSelection();
		}
		if (!showMenu)
		{
			return;
		}
		if (ReInput.controllers.IsControllerAssigned(args.controllerType, args.controllerId))
		{
			foreach (Player player in ReInput.players.AllPlayers)
			{
				if (player.controllers.ContainsController(args.controllerType, args.controllerId))
				{
					ReInput.userDataStore.SaveControllerData(player.id, args.controllerType, args.controllerId);
				}
			}
			return;
		}
		ReInput.userDataStore.SaveControllerData(args.controllerType, args.controllerId);
	}

	private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
	{
		if (showMenu)
		{
			ClearWorkingVars();
		}
		if (ReInput.unityJoystickIdentificationRequired)
		{
			IdentifyAllJoysticks();
		}
	}

	private void OnConflictFound(InputMapper.ConflictFoundEventData data)
	{
		conflictFoundEventData = data;
	}

	private void OnStopped(InputMapper.StoppedEventData data)
	{
		conflictFoundEventData = null;
	}

	public void IdentifyAllJoysticks()
	{
		if (ReInput.controllers.joystickCount == 0)
		{
			return;
		}
		ClearWorkingVars();
		Open();
		foreach (Joystick joystick in ReInput.controllers.Joysticks)
		{
			actionQueue.Enqueue(new FallbackJoystickIdentification(joystick.id, joystick.name));
		}
	}

	protected void CheckRecompile()
	{
	}

	private void RecompileWindow(int windowId)
	{
	}
}
