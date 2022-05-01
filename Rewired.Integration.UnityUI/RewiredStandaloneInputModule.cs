using System;
using System.Collections.Generic;
using Rewired.Components;
using Rewired.UI;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Rewired.Integration.UnityUI;

[AddComponentMenu("Rewired/Rewired Standalone Input Module")]
public sealed class RewiredStandaloneInputModule : RewiredPointerInputModule
{
	[Serializable]
	public class PlayerSetting
	{
		public int playerId;

		public List<Rewired.Components.PlayerMouse> playerMice = new List<Rewired.Components.PlayerMouse>();

		public PlayerSetting()
		{
		}

		private PlayerSetting(PlayerSetting other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			playerId = other.playerId;
			playerMice = new List<Rewired.Components.PlayerMouse>();
			if (other.playerMice == null)
			{
				return;
			}
			foreach (Rewired.Components.PlayerMouse i in other.playerMice)
			{
				playerMice.Add(i);
			}
		}

		public PlayerSetting Clone()
		{
			return new PlayerSetting(this);
		}
	}

	private const string DEFAULT_ACTION_MOVE_HORIZONTAL = "UIHorizontal";

	private const string DEFAULT_ACTION_MOVE_VERTICAL = "UIVertical";

	private const string DEFAULT_ACTION_SUBMIT = "UISubmit";

	private const string DEFAULT_ACTION_CANCEL = "UICancel";

	[Tooltip("(Optional) Link the Rewired Input Manager here for easier access to Player ids, etc.")]
	[SerializeField]
	private InputManager_Base rewiredInputManager;

	[SerializeField]
	[Tooltip("Use all Rewired game Players to control the UI. This does not include the System Player. If enabled, this setting overrides individual Player Ids set in Rewired Player Ids.")]
	private bool useAllRewiredGamePlayers = false;

	[SerializeField]
	[Tooltip("Allow the Rewired System Player to control the UI.")]
	private bool useRewiredSystemPlayer = false;

	[SerializeField]
	[Tooltip("A list of Player Ids that are allowed to control the UI. If Use All Rewired Game Players = True, this list will be ignored.")]
	private int[] rewiredPlayerIds = new int[1];

	[SerializeField]
	[Tooltip("Allow only Players with Player.isPlaying = true to control the UI.")]
	private bool usePlayingPlayersOnly = false;

	[SerializeField]
	[Tooltip("Player Mice allowed to interact with the UI. Each Player that owns a Player Mouse must also be allowed to control the UI or the Player Mouse will not function.")]
	private List<Rewired.Components.PlayerMouse> playerMice = new List<Rewired.Components.PlayerMouse>();

	[SerializeField]
	[Tooltip("Makes an axis press always move only one UI selection. Enable if you do not want to allow scrolling through UI elements by holding an axis direction.")]
	private bool moveOneElementPerAxisPress;

	[SerializeField]
	[Tooltip("If enabled, Action Ids will be used to set the Actions. If disabled, string names will be used to set the Actions.")]
	private bool setActionsById = false;

	[SerializeField]
	[Tooltip("Id of the horizontal Action for movement (if axis events are used).")]
	private int horizontalActionId = -1;

	[SerializeField]
	[Tooltip("Id of the vertical Action for movement (if axis events are used).")]
	private int verticalActionId = -1;

	[SerializeField]
	[Tooltip("Id of the Action used to submit.")]
	private int submitActionId = -1;

	[SerializeField]
	[Tooltip("Id of the Action used to cancel.")]
	private int cancelActionId = -1;

	[SerializeField]
	[Tooltip("Name of the horizontal axis for movement (if axis events are used).")]
	private string m_HorizontalAxis = "UIHorizontal";

	[SerializeField]
	[Tooltip("Name of the vertical axis for movement (if axis events are used).")]
	private string m_VerticalAxis = "UIVertical";

	[SerializeField]
	[Tooltip("Name of the action used to submit.")]
	private string m_SubmitButton = "UISubmit";

	[SerializeField]
	[Tooltip("Name of the action used to cancel.")]
	private string m_CancelButton = "UICancel";

	[SerializeField]
	[Tooltip("Number of selection changes allowed per second when a movement button/axis is held in a direction.")]
	private float m_InputActionsPerSecond = 10f;

	[SerializeField]
	[Tooltip("Delay in seconds before vertical/horizontal movement starts repeating continouously when a movement direction is held.")]
	private float m_RepeatDelay = 0f;

	[SerializeField]
	[Tooltip("Allows the mouse to be used to select elements.")]
	private bool m_allowMouseInput = true;

	[SerializeField]
	[Tooltip("Allows the mouse to be used to select elements if the device also supports touch control.")]
	private bool m_allowMouseInputIfTouchSupported = true;

	[SerializeField]
	[Tooltip("Allows touch input to be used to select elements.")]
	private bool m_allowTouchInput = true;

	[SerializeField]
	[Tooltip("Deselects the current selection on mouse/touch click when the pointer is not over a selectable object.")]
	private bool m_deselectIfBackgroundClicked = true;

	[SerializeField]
	[Tooltip("Deselects the current selection on mouse/touch click before selecting the next object.")]
	private bool m_deselectBeforeSelecting = true;

	[SerializeField]
	[FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
	[Tooltip("Forces the module to always be active.")]
	private bool m_ForceModuleActive;

	[NonSerialized]
	private int[] playerIds;

	private bool recompiling;

	[NonSerialized]
	private bool isTouchSupported;

	[NonSerialized]
	private double m_PrevActionTime;

	[NonSerialized]
	private Vector2 m_LastMoveVector;

	[NonSerialized]
	private int m_ConsecutiveMoveCount = 0;

	[NonSerialized]
	private bool m_HasFocus = true;

	public InputManager_Base RewiredInputManager
	{
		get
		{
			return rewiredInputManager;
		}
		set
		{
			rewiredInputManager = value;
		}
	}

	public bool UseAllRewiredGamePlayers
	{
		get
		{
			return useAllRewiredGamePlayers;
		}
		set
		{
			bool changed = value != useAllRewiredGamePlayers;
			useAllRewiredGamePlayers = value;
			if (changed)
			{
				SetupRewiredVars();
			}
		}
	}

	public bool UseRewiredSystemPlayer
	{
		get
		{
			return useRewiredSystemPlayer;
		}
		set
		{
			bool changed = value != useRewiredSystemPlayer;
			useRewiredSystemPlayer = value;
			if (changed)
			{
				SetupRewiredVars();
			}
		}
	}

	public int[] RewiredPlayerIds
	{
		get
		{
			return (int[])rewiredPlayerIds.Clone();
		}
		set
		{
			rewiredPlayerIds = ((value != null) ? ((int[])value.Clone()) : new int[0]);
			SetupRewiredVars();
		}
	}

	public bool UsePlayingPlayersOnly
	{
		get
		{
			return usePlayingPlayersOnly;
		}
		set
		{
			usePlayingPlayersOnly = value;
		}
	}

	public List<Rewired.Components.PlayerMouse> PlayerMice
	{
		get
		{
			return new List<Rewired.Components.PlayerMouse>(playerMice);
		}
		set
		{
			if (value == null)
			{
				playerMice = new List<Rewired.Components.PlayerMouse>();
				SetupRewiredVars();
			}
			else
			{
				playerMice = new List<Rewired.Components.PlayerMouse>(value);
				SetupRewiredVars();
			}
		}
	}

	public bool MoveOneElementPerAxisPress
	{
		get
		{
			return moveOneElementPerAxisPress;
		}
		set
		{
			moveOneElementPerAxisPress = value;
		}
	}

	public bool allowMouseInput
	{
		get
		{
			return m_allowMouseInput;
		}
		set
		{
			m_allowMouseInput = value;
		}
	}

	public bool allowMouseInputIfTouchSupported
	{
		get
		{
			return m_allowMouseInputIfTouchSupported;
		}
		set
		{
			m_allowMouseInputIfTouchSupported = value;
		}
	}

	public bool allowTouchInput
	{
		get
		{
			return m_allowTouchInput;
		}
		set
		{
			m_allowTouchInput = value;
		}
	}

	public bool deselectIfBackgroundClicked
	{
		get
		{
			return m_deselectIfBackgroundClicked;
		}
		set
		{
			m_deselectIfBackgroundClicked = value;
		}
	}

	private bool deselectBeforeSelecting
	{
		get
		{
			return m_deselectBeforeSelecting;
		}
		set
		{
			m_deselectBeforeSelecting = value;
		}
	}

	public bool SetActionsById
	{
		get
		{
			return setActionsById;
		}
		set
		{
			if (setActionsById != value)
			{
				setActionsById = value;
				SetupRewiredVars();
			}
		}
	}

	public int HorizontalActionId
	{
		get
		{
			return horizontalActionId;
		}
		set
		{
			if (value != horizontalActionId)
			{
				horizontalActionId = value;
				if (ReInput.isReady)
				{
					m_HorizontalAxis = ((ReInput.mapping.GetAction(value) != null) ? ReInput.mapping.GetAction(value).name : string.Empty);
				}
			}
		}
	}

	public int VerticalActionId
	{
		get
		{
			return verticalActionId;
		}
		set
		{
			if (value != verticalActionId)
			{
				verticalActionId = value;
				if (ReInput.isReady)
				{
					m_VerticalAxis = ((ReInput.mapping.GetAction(value) != null) ? ReInput.mapping.GetAction(value).name : string.Empty);
				}
			}
		}
	}

	public int SubmitActionId
	{
		get
		{
			return submitActionId;
		}
		set
		{
			if (value != submitActionId)
			{
				submitActionId = value;
				if (ReInput.isReady)
				{
					m_SubmitButton = ((ReInput.mapping.GetAction(value) != null) ? ReInput.mapping.GetAction(value).name : string.Empty);
				}
			}
		}
	}

	public int CancelActionId
	{
		get
		{
			return cancelActionId;
		}
		set
		{
			if (value != cancelActionId)
			{
				cancelActionId = value;
				if (ReInput.isReady)
				{
					m_CancelButton = ((ReInput.mapping.GetAction(value) != null) ? ReInput.mapping.GetAction(value).name : string.Empty);
				}
			}
		}
	}

	protected override bool isMouseSupported
	{
		get
		{
			if (!base.isMouseSupported)
			{
				return false;
			}
			if (!m_allowMouseInput)
			{
				return false;
			}
			return !isTouchSupported || m_allowMouseInputIfTouchSupported;
		}
	}

	private bool isTouchAllowed => m_allowTouchInput;

	[Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead")]
	public bool allowActivationOnMobileDevice
	{
		get
		{
			return m_ForceModuleActive;
		}
		set
		{
			m_ForceModuleActive = value;
		}
	}

	public bool forceModuleActive
	{
		get
		{
			return m_ForceModuleActive;
		}
		set
		{
			m_ForceModuleActive = value;
		}
	}

	public float inputActionsPerSecond
	{
		get
		{
			return m_InputActionsPerSecond;
		}
		set
		{
			m_InputActionsPerSecond = value;
		}
	}

	public float repeatDelay
	{
		get
		{
			return m_RepeatDelay;
		}
		set
		{
			m_RepeatDelay = value;
		}
	}

	public string horizontalAxis
	{
		get
		{
			return m_HorizontalAxis;
		}
		set
		{
			if (!(m_HorizontalAxis == value))
			{
				m_HorizontalAxis = value;
				if (ReInput.isReady)
				{
					horizontalActionId = ReInput.mapping.GetActionId(value);
				}
			}
		}
	}

	public string verticalAxis
	{
		get
		{
			return m_VerticalAxis;
		}
		set
		{
			if (!(m_VerticalAxis == value))
			{
				m_VerticalAxis = value;
				if (ReInput.isReady)
				{
					verticalActionId = ReInput.mapping.GetActionId(value);
				}
			}
		}
	}

	public string submitButton
	{
		get
		{
			return m_SubmitButton;
		}
		set
		{
			if (!(m_SubmitButton == value))
			{
				m_SubmitButton = value;
				if (ReInput.isReady)
				{
					submitActionId = ReInput.mapping.GetActionId(value);
				}
			}
		}
	}

	public string cancelButton
	{
		get
		{
			return m_CancelButton;
		}
		set
		{
			if (!(m_CancelButton == value))
			{
				m_CancelButton = value;
				if (ReInput.isReady)
				{
					cancelActionId = ReInput.mapping.GetActionId(value);
				}
			}
		}
	}

	private RewiredStandaloneInputModule()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		isTouchSupported = base.defaultTouchInputSource.touchSupported;
		TouchInputModule tim = GetComponent<TouchInputModule>();
		if (tim != null)
		{
			tim.enabled = false;
		}
		ReInput.InitializedEvent += OnRewiredInitialized;
		InitializeRewired();
	}

	public override void UpdateModule()
	{
		CheckEditorRecompile();
		if (!recompiling && ReInput.isReady && !m_HasFocus && !ShouldIgnoreEventsOnNoFocus())
		{
		}
	}

	public override bool IsModuleSupported()
	{
		return true;
	}

	public override bool ShouldActivateModule()
	{
		if (!base.ShouldActivateModule())
		{
			return false;
		}
		if (recompiling)
		{
			return false;
		}
		if (!ReInput.isReady)
		{
			return false;
		}
		bool shouldActivate = m_ForceModuleActive;
		for (int i = 0; i < playerIds.Length; i++)
		{
			Player player = ReInput.players.GetPlayer(playerIds[i]);
			if (player != null && (!usePlayingPlayersOnly || player.isPlaying))
			{
				shouldActivate |= GetButtonDown(player, submitActionId);
				shouldActivate |= GetButtonDown(player, cancelActionId);
				if (moveOneElementPerAxisPress)
				{
					shouldActivate |= GetButtonDown(player, horizontalActionId) || GetNegativeButtonDown(player, horizontalActionId);
					shouldActivate |= GetButtonDown(player, verticalActionId) || GetNegativeButtonDown(player, verticalActionId);
				}
				else
				{
					shouldActivate |= !Mathf.Approximately(GetAxis(player, horizontalActionId), 0f);
					shouldActivate |= !Mathf.Approximately(GetAxis(player, verticalActionId), 0f);
				}
			}
		}
		if (isMouseSupported)
		{
			shouldActivate |= DidAnyMouseMove();
			shouldActivate |= GetMouseButtonDownOnAnyMouse(0);
		}
		if (isTouchAllowed)
		{
			for (int j = 0; j < base.defaultTouchInputSource.touchCount; j++)
			{
				Touch touch = base.defaultTouchInputSource.GetTouch(j);
				shouldActivate |= touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
			}
		}
		return shouldActivate;
	}

	public override void ActivateModule()
	{
		if (m_HasFocus || !ShouldIgnoreEventsOnNoFocus())
		{
			base.ActivateModule();
			GameObject toSelect = base.eventSystem.currentSelectedGameObject;
			if (toSelect == null)
			{
				toSelect = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
		}
	}

	public override void DeactivateModule()
	{
		base.DeactivateModule();
		ClearSelection();
	}

	public override void Process()
	{
		if (!ReInput.isReady || (!m_HasFocus && ShouldIgnoreEventsOnNoFocus()) || !base.enabled || !base.gameObject.activeInHierarchy)
		{
			return;
		}
		bool usedEvent = SendUpdateEventToSelectedObject();
		if (base.eventSystem.sendNavigationEvents)
		{
			if (!usedEvent)
			{
				usedEvent |= SendMoveEventToSelectedObject();
			}
			if (!usedEvent)
			{
				SendSubmitEventToSelectedObject();
			}
		}
		if (!ProcessTouchEvents() && isMouseSupported)
		{
			ProcessMouseEvents();
		}
	}

	private bool ProcessTouchEvents()
	{
		if (!isTouchAllowed)
		{
			return false;
		}
		for (int i = 0; i < base.defaultTouchInputSource.touchCount; i++)
		{
			Touch touch = base.defaultTouchInputSource.GetTouch(i);
			if (touch.type != TouchType.Indirect)
			{
				bool pressed;
				bool released;
				PlayerPointerEventData pointer = GetTouchPointerEventData(0, 0, touch, out pressed, out released);
				ProcessTouchPress(pointer, pressed, released);
				if (!released)
				{
					ProcessMove(pointer);
					ProcessDrag(pointer);
				}
				else
				{
					RemovePointerData(pointer);
				}
			}
		}
		return base.defaultTouchInputSource.touchCount > 0;
	}

	private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
	{
		GameObject currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;
		if (pressed)
		{
			pointerEvent.eligibleForClick = true;
			pointerEvent.delta = Vector2.zero;
			pointerEvent.dragging = false;
			pointerEvent.useDragThreshold = true;
			pointerEvent.pressPosition = pointerEvent.position;
			pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
			HandleMouseTouchDeselectionOnSelectionChanged(currentOverGo, pointerEvent);
			if (pointerEvent.pointerEnter != currentOverGo)
			{
				HandlePointerExitAndEnter(pointerEvent, currentOverGo);
				pointerEvent.pointerEnter = currentOverGo;
			}
			GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
			if (newPressed == null)
			{
				newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
			}
			double time = ReInput.time.unscaledTime;
			if (newPressed == pointerEvent.lastPress)
			{
				double diffTime = time - (double)pointerEvent.clickTime;
				if (diffTime < 0.30000001192092896)
				{
					pointerEvent.clickCount++;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.clickTime = (float)time;
			}
			else
			{
				pointerEvent.clickCount = 1;
			}
			pointerEvent.pointerPress = newPressed;
			pointerEvent.rawPointerPress = currentOverGo;
			pointerEvent.clickTime = (float)time;
			pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);
			if (pointerEvent.pointerDrag != null)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
			}
		}
		if (released)
		{
			ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
			GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
			if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
			{
				ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
			}
			else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
			{
				ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
			}
			pointerEvent.eligibleForClick = false;
			pointerEvent.pointerPress = null;
			pointerEvent.rawPointerPress = null;
			if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.dragging = false;
			pointerEvent.pointerDrag = null;
			if (pointerEvent.pointerDrag != null)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.pointerDrag = null;
			ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
			pointerEvent.pointerEnter = null;
		}
	}

	private bool SendSubmitEventToSelectedObject()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		if (recompiling)
		{
			return false;
		}
		BaseEventData data = GetBaseEventData();
		for (int i = 0; i < playerIds.Length; i++)
		{
			Player player = ReInput.players.GetPlayer(playerIds[i]);
			if (player != null && (!usePlayingPlayersOnly || player.isPlaying))
			{
				if (GetButtonDown(player, submitActionId))
				{
					ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
					break;
				}
				if (GetButtonDown(player, cancelActionId))
				{
					ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
					break;
				}
			}
		}
		return data.used;
	}

	private Vector2 GetRawMoveVector()
	{
		if (recompiling)
		{
			return Vector2.zero;
		}
		Vector2 move = Vector2.zero;
		bool horizButton = false;
		bool vertButton = false;
		for (int i = 0; i < playerIds.Length; i++)
		{
			Player player = ReInput.players.GetPlayer(playerIds[i]);
			if (player == null || (usePlayingPlayersOnly && !player.isPlaying))
			{
				continue;
			}
			float horizontal = GetAxis(player, horizontalActionId);
			float vertical = GetAxis(player, verticalActionId);
			if (Mathf.Approximately(horizontal, 0f))
			{
				horizontal = 0f;
			}
			if (Mathf.Approximately(vertical, 0f))
			{
				vertical = 0f;
			}
			if (moveOneElementPerAxisPress)
			{
				if (GetButtonDown(player, horizontalActionId) && horizontal > 0f)
				{
					move.x += 1f;
				}
				if (GetNegativeButtonDown(player, horizontalActionId) && horizontal < 0f)
				{
					move.x -= 1f;
				}
				if (GetButtonDown(player, verticalActionId) && vertical > 0f)
				{
					move.y += 1f;
				}
				if (GetNegativeButtonDown(player, verticalActionId) && vertical < 0f)
				{
					move.y -= 1f;
				}
			}
			else
			{
				if (GetButton(player, horizontalActionId) && horizontal > 0f)
				{
					move.x += 1f;
				}
				if (GetNegativeButton(player, horizontalActionId) && horizontal < 0f)
				{
					move.x -= 1f;
				}
				if (GetButton(player, verticalActionId) && vertical > 0f)
				{
					move.y += 1f;
				}
				if (GetNegativeButton(player, verticalActionId) && vertical < 0f)
				{
					move.y -= 1f;
				}
			}
		}
		return move;
	}

	private bool SendMoveEventToSelectedObject()
	{
		if (recompiling)
		{
			return false;
		}
		double time = ReInput.time.unscaledTime;
		Vector2 movement = GetRawMoveVector();
		if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
		{
			m_ConsecutiveMoveCount = 0;
			return false;
		}
		bool similarDir = Vector2.Dot(movement, m_LastMoveVector) > 0f;
		CheckButtonOrKeyMovement(out var buttonDownHorizontal, out var buttonDownVertical);
		AxisEventData axisEventData = null;
		bool allow = buttonDownHorizontal || buttonDownVertical;
		if (allow)
		{
			axisEventData = GetAxisEventData(movement.x, movement.y, 0f);
			MoveDirection moveDir = axisEventData.moveDir;
			allow = ((moveDir == MoveDirection.Up || moveDir == MoveDirection.Down) && buttonDownVertical) || ((moveDir == MoveDirection.Left || moveDir == MoveDirection.Right) && buttonDownHorizontal);
		}
		if (!allow)
		{
			allow = ((!(m_RepeatDelay > 0f)) ? (time > m_PrevActionTime + (double)(1f / m_InputActionsPerSecond)) : ((!similarDir || m_ConsecutiveMoveCount != 1) ? (time > m_PrevActionTime + (double)(1f / m_InputActionsPerSecond)) : (time > m_PrevActionTime + (double)m_RepeatDelay)));
		}
		if (!allow)
		{
			return false;
		}
		if (axisEventData == null)
		{
			axisEventData = GetAxisEventData(movement.x, movement.y, 0f);
		}
		if (axisEventData.moveDir != MoveDirection.None)
		{
			ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			if (!similarDir)
			{
				m_ConsecutiveMoveCount = 0;
			}
			if (m_ConsecutiveMoveCount == 0 || !(buttonDownHorizontal || buttonDownVertical))
			{
				m_ConsecutiveMoveCount++;
			}
			m_PrevActionTime = time;
			m_LastMoveVector = movement;
		}
		else
		{
			m_ConsecutiveMoveCount = 0;
		}
		return axisEventData.used;
	}

	private void CheckButtonOrKeyMovement(out bool downHorizontal, out bool downVertical)
	{
		downHorizontal = false;
		downVertical = false;
		for (int i = 0; i < playerIds.Length; i++)
		{
			Player player = ReInput.players.GetPlayer(playerIds[i]);
			if (player != null && (!usePlayingPlayersOnly || player.isPlaying))
			{
				downHorizontal |= GetButtonDown(player, horizontalActionId) || GetNegativeButtonDown(player, horizontalActionId);
				downVertical |= GetButtonDown(player, verticalActionId) || GetNegativeButtonDown(player, verticalActionId);
			}
		}
	}

	private void ProcessMouseEvents()
	{
		for (int i = 0; i < playerIds.Length; i++)
		{
			Player player = ReInput.players.GetPlayer(playerIds[i]);
			if (player != null && (!usePlayingPlayersOnly || player.isPlaying))
			{
				int pointerCount = GetMouseInputSourceCount(playerIds[i]);
				for (int j = 0; j < pointerCount; j++)
				{
					ProcessMouseEvent(playerIds[i], j);
				}
			}
		}
	}

	private void ProcessMouseEvent(int playerId, int pointerIndex)
	{
		MouseState mouseData = GetMousePointerEventData(playerId, pointerIndex);
		if (mouseData == null)
		{
			return;
		}
		MouseButtonEventData leftButtonData = mouseData.GetButtonState(0).eventData;
		ProcessMousePress(leftButtonData);
		ProcessMove(leftButtonData.buttonData);
		ProcessDrag(leftButtonData.buttonData);
		ProcessMousePress(mouseData.GetButtonState(1).eventData);
		ProcessDrag(mouseData.GetButtonState(1).eventData.buttonData);
		ProcessMousePress(mouseData.GetButtonState(2).eventData);
		ProcessDrag(mouseData.GetButtonState(2).eventData.buttonData);
		IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, pointerIndex);
		if (mouseInputSource != null)
		{
			for (int i = 3; i < mouseInputSource.buttonCount; i++)
			{
				ProcessMousePress(mouseData.GetButtonState(i).eventData);
				ProcessDrag(mouseData.GetButtonState(i).eventData.buttonData);
			}
			if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				GameObject scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
				ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
			}
		}
	}

	private bool SendUpdateEventToSelectedObject()
	{
		if (base.eventSystem.currentSelectedGameObject == null)
		{
			return false;
		}
		BaseEventData data = GetBaseEventData();
		ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
		return data.used;
	}

	private void ProcessMousePress(MouseButtonEventData data)
	{
		PlayerPointerEventData pointerEvent = data.buttonData;
		if (GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex) == null)
		{
			return;
		}
		GameObject currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;
		if (data.PressedThisFrame())
		{
			pointerEvent.eligibleForClick = true;
			pointerEvent.delta = Vector2.zero;
			pointerEvent.dragging = false;
			pointerEvent.useDragThreshold = true;
			pointerEvent.pressPosition = pointerEvent.position;
			pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
			HandleMouseTouchDeselectionOnSelectionChanged(currentOverGo, pointerEvent);
			GameObject newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);
			if (newPressed == null)
			{
				newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
			}
			double time = ReInput.time.unscaledTime;
			if (newPressed == pointerEvent.lastPress)
			{
				double diffTime = time - (double)pointerEvent.clickTime;
				if (diffTime < 0.30000001192092896)
				{
					pointerEvent.clickCount++;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.clickTime = (float)time;
			}
			else
			{
				pointerEvent.clickCount = 1;
			}
			pointerEvent.pointerPress = newPressed;
			pointerEvent.rawPointerPress = currentOverGo;
			pointerEvent.clickTime = (float)time;
			pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);
			if (pointerEvent.pointerDrag != null)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
			}
		}
		if (data.ReleasedThisFrame())
		{
			ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
			GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
			if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
			{
				ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
			}
			else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
			{
				ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
			}
			pointerEvent.eligibleForClick = false;
			pointerEvent.pointerPress = null;
			pointerEvent.rawPointerPress = null;
			if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
			{
				ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
			}
			pointerEvent.dragging = false;
			pointerEvent.pointerDrag = null;
			if (currentOverGo != pointerEvent.pointerEnter)
			{
				HandlePointerExitAndEnter(pointerEvent, null);
				HandlePointerExitAndEnter(pointerEvent, currentOverGo);
			}
		}
	}

	private void HandleMouseTouchDeselectionOnSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
	{
		if (m_deselectIfBackgroundClicked && m_deselectBeforeSelecting)
		{
			DeselectIfSelectionChanged(currentOverGo, pointerEvent);
			return;
		}
		GameObject selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
		if (m_deselectIfBackgroundClicked)
		{
			if (selectHandlerGO != base.eventSystem.currentSelectedGameObject && selectHandlerGO != null)
			{
				base.eventSystem.SetSelectedGameObject(null, pointerEvent);
			}
		}
		else if (m_deselectBeforeSelecting && selectHandlerGO != null && selectHandlerGO != base.eventSystem.currentSelectedGameObject)
		{
			base.eventSystem.SetSelectedGameObject(null, pointerEvent);
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		m_HasFocus = hasFocus;
	}

	private bool ShouldIgnoreEventsOnNoFocus()
	{
		if (!ReInput.isReady)
		{
			return true;
		}
		return ReInput.configuration.ignoreInputWhenAppNotInFocus;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ReInput.InitializedEvent -= OnRewiredInitialized;
		ReInput.ShutDownEvent -= OnRewiredShutDown;
		ReInput.EditorRecompileEvent -= OnEditorRecompile;
	}

	protected override bool IsDefaultPlayer(int playerId)
	{
		if (playerIds == null)
		{
			return false;
		}
		if (!ReInput.isReady)
		{
			return false;
		}
		for (int tries = 0; tries < 3; tries++)
		{
			for (int i = 0; i < playerIds.Length; i++)
			{
				Player player = ReInput.players.GetPlayer(playerIds[i]);
				if (player != null && (tries >= 1 || !usePlayingPlayersOnly || player.isPlaying) && (tries >= 2 || player.controllers.hasMouse))
				{
					return playerIds[i] == playerId;
				}
			}
		}
		return false;
	}

	private void InitializeRewired()
	{
		if (!ReInput.isReady)
		{
			Debug.LogError("Rewired is not initialized! Are you missing a Rewired Input Manager in your scene?");
			return;
		}
		ReInput.ShutDownEvent -= OnRewiredShutDown;
		ReInput.ShutDownEvent += OnRewiredShutDown;
		ReInput.EditorRecompileEvent -= OnEditorRecompile;
		ReInput.EditorRecompileEvent += OnEditorRecompile;
		SetupRewiredVars();
	}

	private void SetupRewiredVars()
	{
		if (!ReInput.isReady)
		{
			return;
		}
		SetUpRewiredActions();
		if (useAllRewiredGamePlayers)
		{
			IList<Player> rwPlayers = (useRewiredSystemPlayer ? ReInput.players.AllPlayers : ReInput.players.Players);
			this.playerIds = new int[rwPlayers.Count];
			for (int i = 0; i < rwPlayers.Count; i++)
			{
				this.playerIds[i] = rwPlayers[i].id;
			}
		}
		else
		{
			bool foundSystem = false;
			List<int> playerIds = new List<int>(rewiredPlayerIds.Length + 1);
			for (int j = 0; j < rewiredPlayerIds.Length; j++)
			{
				Player player = ReInput.players.GetPlayer(rewiredPlayerIds[j]);
				if (player != null && !playerIds.Contains(player.id))
				{
					playerIds.Add(player.id);
					if (player.id == 9999999)
					{
						foundSystem = true;
					}
				}
			}
			if (useRewiredSystemPlayer && !foundSystem)
			{
				playerIds.Insert(0, ReInput.players.GetSystemPlayer().id);
			}
			this.playerIds = playerIds.ToArray();
		}
		SetUpRewiredPlayerMice();
	}

	private void SetUpRewiredPlayerMice()
	{
		if (!ReInput.isReady)
		{
			return;
		}
		ClearMouseInputSources();
		for (int i = 0; i < playerMice.Count; i++)
		{
			Rewired.Components.PlayerMouse mouse = playerMice[i];
			if (!UnityTools.IsNullOrDestroyed(mouse))
			{
				AddMouseInputSource(mouse);
			}
		}
	}

	private void SetUpRewiredActions()
	{
		if (!ReInput.isReady)
		{
			return;
		}
		if (!setActionsById)
		{
			horizontalActionId = ReInput.mapping.GetActionId(m_HorizontalAxis);
			verticalActionId = ReInput.mapping.GetActionId(m_VerticalAxis);
			submitActionId = ReInput.mapping.GetActionId(m_SubmitButton);
			cancelActionId = ReInput.mapping.GetActionId(m_CancelButton);
			return;
		}
		InputAction action = ReInput.mapping.GetAction(horizontalActionId);
		m_HorizontalAxis = ((action != null) ? action.name : string.Empty);
		if (action == null)
		{
			horizontalActionId = -1;
		}
		action = ReInput.mapping.GetAction(verticalActionId);
		m_VerticalAxis = ((action != null) ? action.name : string.Empty);
		if (action == null)
		{
			verticalActionId = -1;
		}
		action = ReInput.mapping.GetAction(submitActionId);
		m_SubmitButton = ((action != null) ? action.name : string.Empty);
		if (action == null)
		{
			submitActionId = -1;
		}
		action = ReInput.mapping.GetAction(cancelActionId);
		m_CancelButton = ((action != null) ? action.name : string.Empty);
		if (action == null)
		{
			cancelActionId = -1;
		}
	}

	private bool GetButton(Player player, int actionId)
	{
		if (actionId < 0)
		{
			return false;
		}
		return player.GetButton(actionId);
	}

	private bool GetButtonDown(Player player, int actionId)
	{
		if (actionId < 0)
		{
			return false;
		}
		return player.GetButtonDown(actionId);
	}

	private bool GetNegativeButton(Player player, int actionId)
	{
		if (actionId < 0)
		{
			return false;
		}
		return player.GetNegativeButton(actionId);
	}

	private bool GetNegativeButtonDown(Player player, int actionId)
	{
		if (actionId < 0)
		{
			return false;
		}
		return player.GetNegativeButtonDown(actionId);
	}

	private float GetAxis(Player player, int actionId)
	{
		if (actionId < 0)
		{
			return 0f;
		}
		return player.GetAxis(actionId);
	}

	private void CheckEditorRecompile()
	{
		if (recompiling && ReInput.isReady)
		{
			recompiling = false;
			InitializeRewired();
		}
	}

	private void OnEditorRecompile()
	{
		recompiling = true;
		ClearRewiredVars();
	}

	private void ClearRewiredVars()
	{
		Array.Clear(playerIds, 0, playerIds.Length);
		ClearMouseInputSources();
	}

	private bool DidAnyMouseMove()
	{
		for (int i = 0; i < playerIds.Length; i++)
		{
			int playerId = playerIds[i];
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null || (usePlayingPlayersOnly && !player.isPlaying))
			{
				continue;
			}
			int mouseCount = GetMouseInputSourceCount(playerId);
			for (int j = 0; j < mouseCount; j++)
			{
				IMouseInputSource source = GetMouseInputSource(playerId, j);
				if (source != null && source.screenPositionDelta.sqrMagnitude > 0f)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool GetMouseButtonDownOnAnyMouse(int buttonIndex)
	{
		for (int i = 0; i < playerIds.Length; i++)
		{
			int playerId = playerIds[i];
			Player player = ReInput.players.GetPlayer(playerId);
			if (player == null || (usePlayingPlayersOnly && !player.isPlaying))
			{
				continue;
			}
			int mouseCount = GetMouseInputSourceCount(playerId);
			for (int j = 0; j < mouseCount; j++)
			{
				IMouseInputSource source = GetMouseInputSource(playerId, j);
				if (source != null && source.GetButtonDown(buttonIndex))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnRewiredInitialized()
	{
		InitializeRewired();
	}

	private void OnRewiredShutDown()
	{
		ClearRewiredVars();
	}
}
