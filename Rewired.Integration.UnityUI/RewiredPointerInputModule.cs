using System;
using System.Collections.Generic;
using System.Text;
using Rewired.UI;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rewired.Integration.UnityUI;

public abstract class RewiredPointerInputModule : BaseInputModule
{
	protected class MouseState
	{
		private List<ButtonState> m_TrackedButtons = new List<ButtonState>();

		public bool AnyPressesThisFrame()
		{
			for (int i = 0; i < m_TrackedButtons.Count; i++)
			{
				if (m_TrackedButtons[i].eventData.PressedThisFrame())
				{
					return true;
				}
			}
			return false;
		}

		public bool AnyReleasesThisFrame()
		{
			for (int i = 0; i < m_TrackedButtons.Count; i++)
			{
				if (m_TrackedButtons[i].eventData.ReleasedThisFrame())
				{
					return true;
				}
			}
			return false;
		}

		public ButtonState GetButtonState(int button)
		{
			ButtonState tracked = null;
			for (int i = 0; i < m_TrackedButtons.Count; i++)
			{
				if (m_TrackedButtons[i].button == button)
				{
					tracked = m_TrackedButtons[i];
					break;
				}
			}
			if (tracked == null)
			{
				tracked = new ButtonState
				{
					button = button,
					eventData = new MouseButtonEventData()
				};
				m_TrackedButtons.Add(tracked);
			}
			return tracked;
		}

		public void SetButtonState(int button, PointerEventData.FramePressState stateForMouseButton, PlayerPointerEventData data)
		{
			ButtonState toModify = GetButtonState(button);
			toModify.eventData.buttonState = stateForMouseButton;
			toModify.eventData.buttonData = data;
		}
	}

	public class MouseButtonEventData
	{
		public PointerEventData.FramePressState buttonState;

		public PlayerPointerEventData buttonData;

		public bool PressedThisFrame()
		{
			return buttonState == PointerEventData.FramePressState.Pressed || buttonState == PointerEventData.FramePressState.PressedAndReleased;
		}

		public bool ReleasedThisFrame()
		{
			return buttonState == PointerEventData.FramePressState.Released || buttonState == PointerEventData.FramePressState.PressedAndReleased;
		}
	}

	protected class ButtonState
	{
		private int m_Button = 0;

		private MouseButtonEventData m_EventData;

		public MouseButtonEventData eventData
		{
			get
			{
				return m_EventData;
			}
			set
			{
				m_EventData = value;
			}
		}

		public int button
		{
			get
			{
				return m_Button;
			}
			set
			{
				m_Button = value;
			}
		}
	}

	private sealed class UnityInputSource : IMouseInputSource, ITouchInputSource
	{
		private Vector2 m_MousePosition;

		private Vector2 m_MousePositionPrev;

		private int m_LastUpdatedFrame = -1;

		int IMouseInputSource.playerId
		{
			get
			{
				TryUpdate();
				return 0;
			}
		}

		int ITouchInputSource.playerId
		{
			get
			{
				TryUpdate();
				return 0;
			}
		}

		bool IMouseInputSource.enabled
		{
			get
			{
				TryUpdate();
				return true;
			}
		}

		bool IMouseInputSource.locked
		{
			get
			{
				TryUpdate();
				return Cursor.lockState == CursorLockMode.Locked;
			}
		}

		int IMouseInputSource.buttonCount
		{
			get
			{
				TryUpdate();
				return 3;
			}
		}

		Vector2 IMouseInputSource.screenPosition
		{
			get
			{
				TryUpdate();
				return Input.mousePosition;
			}
		}

		Vector2 IMouseInputSource.screenPositionDelta
		{
			get
			{
				TryUpdate();
				return m_MousePosition - m_MousePositionPrev;
			}
		}

		Vector2 IMouseInputSource.wheelDelta
		{
			get
			{
				TryUpdate();
				return Input.mouseScrollDelta;
			}
		}

		bool ITouchInputSource.touchSupported
		{
			get
			{
				TryUpdate();
				return Input.touchSupported;
			}
		}

		int ITouchInputSource.touchCount
		{
			get
			{
				TryUpdate();
				return Input.touchCount;
			}
		}

		bool IMouseInputSource.GetButtonDown(int button)
		{
			TryUpdate();
			return Input.GetMouseButtonDown(button);
		}

		bool IMouseInputSource.GetButtonUp(int button)
		{
			TryUpdate();
			return Input.GetMouseButtonUp(button);
		}

		bool IMouseInputSource.GetButton(int button)
		{
			TryUpdate();
			return Input.GetMouseButton(button);
		}

		Touch ITouchInputSource.GetTouch(int index)
		{
			TryUpdate();
			return Input.GetTouch(index);
		}

		private void TryUpdate()
		{
			if (Time.frameCount != m_LastUpdatedFrame)
			{
				m_LastUpdatedFrame = Time.frameCount;
				m_MousePositionPrev = m_MousePosition;
				m_MousePosition = Input.mousePosition;
			}
		}
	}

	public const int kMouseLeftId = -1;

	public const int kMouseRightId = -2;

	public const int kMouseMiddleId = -3;

	public const int kFakeTouchesId = -4;

	private const int customButtonsStartingId = -2147483520;

	private const int customButtonsMaxCount = 128;

	private const int customButtonsLastId = -2147483392;

	private readonly List<IMouseInputSource> m_MouseInputSourcesList = new List<IMouseInputSource>();

	private Dictionary<int, Dictionary<int, PlayerPointerEventData>[]> m_PlayerPointerData = new Dictionary<int, Dictionary<int, PlayerPointerEventData>[]>();

	private ITouchInputSource m_UserDefaultTouchInputSource;

	private UnityInputSource __m_DefaultInputSource;

	private readonly MouseState m_MouseState = new MouseState();

	private UnityInputSource defaultInputSource => (__m_DefaultInputSource != null) ? __m_DefaultInputSource : (__m_DefaultInputSource = new UnityInputSource());

	private IMouseInputSource defaultMouseInputSource => defaultInputSource;

	protected ITouchInputSource defaultTouchInputSource => defaultInputSource;

	protected virtual bool isMouseSupported
	{
		get
		{
			int count = m_MouseInputSourcesList.Count;
			if (count == 0)
			{
				return defaultMouseInputSource.enabled;
			}
			for (int i = 0; i < count; i++)
			{
				if (m_MouseInputSourcesList[i].enabled)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected bool IsDefaultMouse(IMouseInputSource mouse)
	{
		return defaultMouseInputSource == mouse;
	}

	public IMouseInputSource GetMouseInputSource(int playerId, int mouseIndex)
	{
		if (mouseIndex < 0)
		{
			throw new ArgumentOutOfRangeException("mouseIndex");
		}
		if (m_MouseInputSourcesList.Count == 0 && IsDefaultPlayer(playerId))
		{
			return defaultMouseInputSource;
		}
		int count = m_MouseInputSourcesList.Count;
		int pointerCount = 0;
		for (int i = 0; i < count; i++)
		{
			IMouseInputSource source = m_MouseInputSourcesList[i];
			if (!UnityTools.IsNullOrDestroyed(source) && source.playerId == playerId)
			{
				if (mouseIndex == pointerCount)
				{
					return source;
				}
				pointerCount++;
			}
		}
		return null;
	}

	public void RemoveMouseInputSource(IMouseInputSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		m_MouseInputSourcesList.Remove(source);
	}

	public void AddMouseInputSource(IMouseInputSource source)
	{
		if (UnityTools.IsNullOrDestroyed(source))
		{
			throw new ArgumentNullException("source");
		}
		m_MouseInputSourcesList.Add(source);
	}

	public int GetMouseInputSourceCount(int playerId)
	{
		if (m_MouseInputSourcesList.Count == 0 && IsDefaultPlayer(playerId))
		{
			return 1;
		}
		int count = m_MouseInputSourcesList.Count;
		int pointerCount = 0;
		for (int i = 0; i < count; i++)
		{
			IMouseInputSource source = m_MouseInputSourcesList[i];
			if (!UnityTools.IsNullOrDestroyed(source) && source.playerId == playerId)
			{
				pointerCount++;
			}
		}
		return pointerCount;
	}

	public ITouchInputSource GetTouchInputSource(int playerId, int sourceIndex)
	{
		if (!UnityTools.IsNullOrDestroyed(m_UserDefaultTouchInputSource))
		{
			return m_UserDefaultTouchInputSource;
		}
		return defaultTouchInputSource;
	}

	public void RemoveTouchInputSource(ITouchInputSource source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (m_UserDefaultTouchInputSource == source)
		{
			m_UserDefaultTouchInputSource = null;
		}
	}

	public void AddTouchInputSource(ITouchInputSource source)
	{
		if (UnityTools.IsNullOrDestroyed(source))
		{
			throw new ArgumentNullException("source");
		}
		m_UserDefaultTouchInputSource = source;
	}

	public int GetTouchInputSourceCount(int playerId)
	{
		return IsDefaultPlayer(playerId) ? 1 : 0;
	}

	protected void ClearMouseInputSources()
	{
		m_MouseInputSourcesList.Clear();
	}

	protected abstract bool IsDefaultPlayer(int playerId);

	protected bool GetPointerData(int playerId, int pointerIndex, int pointerTypeId, out PlayerPointerEventData data, bool create, PointerEventType pointerEventType)
	{
		if (!m_PlayerPointerData.TryGetValue(playerId, out var pointerDataByIndex))
		{
			pointerDataByIndex = new Dictionary<int, PlayerPointerEventData>[pointerIndex + 1];
			for (int i = 0; i < pointerDataByIndex.Length; i++)
			{
				pointerDataByIndex[i] = new Dictionary<int, PlayerPointerEventData>();
			}
			m_PlayerPointerData.Add(playerId, pointerDataByIndex);
		}
		if (pointerIndex >= pointerDataByIndex.Length)
		{
			Dictionary<int, PlayerPointerEventData>[] newArray = new Dictionary<int, PlayerPointerEventData>[pointerIndex + 1];
			for (int j = 0; j < pointerDataByIndex.Length; j++)
			{
				newArray[j] = pointerDataByIndex[j];
			}
			newArray[pointerIndex] = new Dictionary<int, PlayerPointerEventData>();
			pointerDataByIndex = newArray;
			m_PlayerPointerData[playerId] = pointerDataByIndex;
		}
		Dictionary<int, PlayerPointerEventData> byMouseIndexDict = pointerDataByIndex[pointerIndex];
		if (!byMouseIndexDict.TryGetValue(pointerTypeId, out data))
		{
			if (!create)
			{
				return false;
			}
			data = CreatePointerEventData(playerId, pointerIndex, pointerTypeId, pointerEventType);
			byMouseIndexDict.Add(pointerTypeId, data);
			return true;
		}
		data.mouseSource = ((pointerEventType == PointerEventType.Mouse) ? GetMouseInputSource(playerId, pointerIndex) : null);
		data.touchSource = ((pointerEventType == PointerEventType.Touch) ? GetTouchInputSource(playerId, pointerIndex) : null);
		return false;
	}

	private PlayerPointerEventData CreatePointerEventData(int playerId, int pointerIndex, int pointerTypeId, PointerEventType pointerEventType)
	{
		PlayerPointerEventData data = new PlayerPointerEventData(base.eventSystem)
		{
			playerId = playerId,
			inputSourceIndex = pointerIndex,
			pointerId = pointerTypeId,
			sourceType = pointerEventType
		};
		switch (pointerEventType)
		{
		case PointerEventType.Mouse:
			data.mouseSource = GetMouseInputSource(playerId, pointerIndex);
			break;
		case PointerEventType.Touch:
			data.touchSource = GetTouchInputSource(playerId, pointerIndex);
			break;
		}
		if (pointerTypeId == -1)
		{
			data.buttonIndex = 0;
		}
		else if (pointerTypeId == -2)
		{
			data.buttonIndex = 1;
		}
		else if (pointerTypeId == -3)
		{
			data.buttonIndex = 2;
		}
		else if (pointerTypeId >= -2147483520 && pointerTypeId <= -2147483392)
		{
			data.buttonIndex = pointerTypeId - -2147483520;
		}
		return data;
	}

	protected void RemovePointerData(PlayerPointerEventData data)
	{
		if (m_PlayerPointerData.TryGetValue(data.playerId, out var pointerDataByIndex) && (uint)data.inputSourceIndex < (uint)pointerDataByIndex.Length)
		{
			pointerDataByIndex[data.inputSourceIndex].Remove(data.pointerId);
		}
	}

	protected PlayerPointerEventData GetTouchPointerEventData(int playerId, int touchDeviceIndex, Touch input, out bool pressed, out bool released)
	{
		PlayerPointerEventData pointerData;
		bool created = GetPointerData(playerId, touchDeviceIndex, input.fingerId, out pointerData, create: true, PointerEventType.Touch);
		pointerData.Reset();
		pressed = created || input.phase == TouchPhase.Began;
		released = input.phase == TouchPhase.Canceled || input.phase == TouchPhase.Ended;
		if (created)
		{
			pointerData.position = input.position;
		}
		if (pressed)
		{
			pointerData.delta = Vector2.zero;
		}
		else
		{
			pointerData.delta = input.position - pointerData.position;
		}
		pointerData.position = input.position;
		pointerData.button = PointerEventData.InputButton.Left;
		base.eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
		RaycastResult raycast = (pointerData.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache));
		m_RaycastResultCache.Clear();
		return pointerData;
	}

	protected virtual MouseState GetMousePointerEventData(int playerId, int mouseIndex)
	{
		IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, mouseIndex);
		if (mouseInputSource == null)
		{
			return null;
		}
		PlayerPointerEventData leftData;
		bool created = GetPointerData(playerId, mouseIndex, -1, out leftData, create: true, PointerEventType.Mouse);
		leftData.Reset();
		if (created)
		{
			leftData.position = mouseInputSource.screenPosition;
		}
		Vector2 pos = mouseInputSource.screenPosition;
		if (mouseInputSource.locked || !mouseInputSource.enabled)
		{
			leftData.position = new Vector2(-1f, -1f);
			leftData.delta = Vector2.zero;
		}
		else
		{
			leftData.delta = pos - leftData.position;
			leftData.position = pos;
		}
		leftData.scrollDelta = mouseInputSource.wheelDelta;
		leftData.button = PointerEventData.InputButton.Left;
		base.eventSystem.RaycastAll(leftData, m_RaycastResultCache);
		RaycastResult raycast = (leftData.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache));
		m_RaycastResultCache.Clear();
		GetPointerData(playerId, mouseIndex, -2, out var rightData, create: true, PointerEventType.Mouse);
		CopyFromTo(leftData, rightData);
		rightData.button = PointerEventData.InputButton.Right;
		GetPointerData(playerId, mouseIndex, -3, out var middleData, create: true, PointerEventType.Mouse);
		CopyFromTo(leftData, middleData);
		middleData.button = PointerEventData.InputButton.Middle;
		for (int j = 3; j < mouseInputSource.buttonCount; j++)
		{
			GetPointerData(playerId, mouseIndex, -2147483520 + j, out var data, create: true, PointerEventType.Mouse);
			CopyFromTo(leftData, data);
			data.button = (PointerEventData.InputButton)(-1);
		}
		m_MouseState.SetButtonState(0, StateForMouseButton(playerId, mouseIndex, 0), leftData);
		m_MouseState.SetButtonState(1, StateForMouseButton(playerId, mouseIndex, 1), rightData);
		m_MouseState.SetButtonState(2, StateForMouseButton(playerId, mouseIndex, 2), middleData);
		for (int i = 3; i < mouseInputSource.buttonCount; i++)
		{
			GetPointerData(playerId, mouseIndex, -2147483520 + i, out var data2, create: false, PointerEventType.Mouse);
			m_MouseState.SetButtonState(i, StateForMouseButton(playerId, mouseIndex, i), data2);
		}
		return m_MouseState;
	}

	protected PlayerPointerEventData GetLastPointerEventData(int playerId, int pointerIndex, int pointerTypeId, bool ignorePointerTypeId, PointerEventType pointerEventType)
	{
		if (!ignorePointerTypeId)
		{
			GetPointerData(playerId, pointerIndex, pointerTypeId, out var data, create: false, pointerEventType);
			return data;
		}
		if (!m_PlayerPointerData.TryGetValue(playerId, out var pointerDataByIndex))
		{
			return null;
		}
		if ((uint)pointerIndex >= (uint)pointerDataByIndex.Length)
		{
			return null;
		}
		using (Dictionary<int, PlayerPointerEventData>.Enumerator enumerator = pointerDataByIndex[pointerIndex].GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current.Value;
			}
		}
		return null;
	}

	private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
	{
		if (!useDragThreshold)
		{
			return true;
		}
		return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
	}

	protected virtual void ProcessMove(PlayerPointerEventData pointerEvent)
	{
		GameObject targetGO;
		if (pointerEvent.sourceType == PointerEventType.Mouse)
		{
			IMouseInputSource source = GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex);
			targetGO = ((source == null) ? null : ((!source.enabled || source.locked) ? null : pointerEvent.pointerCurrentRaycast.gameObject));
		}
		else
		{
			if (pointerEvent.sourceType != PointerEventType.Touch)
			{
				throw new NotImplementedException();
			}
			targetGO = pointerEvent.pointerCurrentRaycast.gameObject;
		}
		HandlePointerExitAndEnter(pointerEvent, targetGO);
	}

	protected virtual void ProcessDrag(PlayerPointerEventData pointerEvent)
	{
		if (!pointerEvent.IsPointerMoving() || pointerEvent.pointerDrag == null)
		{
			return;
		}
		if (pointerEvent.sourceType == PointerEventType.Mouse)
		{
			IMouseInputSource source = GetMouseInputSource(pointerEvent.playerId, pointerEvent.inputSourceIndex);
			if (source == null || source.locked || !source.enabled)
			{
				return;
			}
		}
		if (!pointerEvent.dragging && ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, base.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold))
		{
			ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
			pointerEvent.dragging = true;
		}
		if (pointerEvent.dragging)
		{
			if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
			{
				ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
				pointerEvent.eligibleForClick = false;
				pointerEvent.pointerPress = null;
				pointerEvent.rawPointerPress = null;
			}
			ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
		}
	}

	public override bool IsPointerOverGameObject(int pointerTypeId)
	{
		foreach (KeyValuePair<int, Dictionary<int, PlayerPointerEventData>[]> playerPointerDatum in m_PlayerPointerData)
		{
			Dictionary<int, PlayerPointerEventData>[] value = playerPointerDatum.Value;
			foreach (Dictionary<int, PlayerPointerEventData> perIndex in value)
			{
				if (perIndex.TryGetValue(pointerTypeId, out var data) && data.pointerEnter != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	protected void ClearSelection()
	{
		BaseEventData baseEventData = GetBaseEventData();
		foreach (KeyValuePair<int, Dictionary<int, PlayerPointerEventData>[]> playerPointerDatum in m_PlayerPointerData)
		{
			Dictionary<int, PlayerPointerEventData>[] byIndex = playerPointerDatum.Value;
			for (int i = 0; i < byIndex.Length; i++)
			{
				foreach (KeyValuePair<int, PlayerPointerEventData> item in byIndex[i])
				{
					HandlePointerExitAndEnter(item.Value, null);
				}
				byIndex[i].Clear();
			}
		}
		base.eventSystem.SetSelectedGameObject(null, baseEventData);
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder("<b>Pointer Input Module of type: </b>" + GetType());
		sb.AppendLine();
		foreach (KeyValuePair<int, Dictionary<int, PlayerPointerEventData>[]> playerSetKVP in m_PlayerPointerData)
		{
			sb.AppendLine("<B>Player Id:</b> " + playerSetKVP.Key);
			Dictionary<int, PlayerPointerEventData>[] byIndex = playerSetKVP.Value;
			for (int i = 0; i < byIndex.Length; i++)
			{
				sb.AppendLine("<B>Pointer Index:</b> " + i);
				foreach (KeyValuePair<int, PlayerPointerEventData> buttonSetKVP in byIndex[i])
				{
					sb.AppendLine("<B>Button Id:</b> " + buttonSetKVP.Key);
					sb.AppendLine(buttonSetKVP.Value.ToString());
				}
			}
		}
		return sb.ToString();
	}

	protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
	{
		GameObject selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);
		if (selectHandlerGO != base.eventSystem.currentSelectedGameObject)
		{
			base.eventSystem.SetSelectedGameObject(null, pointerEvent);
		}
	}

	protected void CopyFromTo(PointerEventData from, PointerEventData to)
	{
		to.position = from.position;
		to.delta = from.delta;
		to.scrollDelta = from.scrollDelta;
		to.pointerCurrentRaycast = from.pointerCurrentRaycast;
		to.pointerEnter = from.pointerEnter;
	}

	protected PointerEventData.FramePressState StateForMouseButton(int playerId, int mouseIndex, int buttonId)
	{
		IMouseInputSource mouseInputSource = GetMouseInputSource(playerId, mouseIndex);
		if (mouseInputSource == null)
		{
			return PointerEventData.FramePressState.NotChanged;
		}
		bool pressed = mouseInputSource.GetButtonDown(buttonId);
		bool released = mouseInputSource.GetButtonUp(buttonId);
		if (pressed && released)
		{
			return PointerEventData.FramePressState.PressedAndReleased;
		}
		if (pressed)
		{
			return PointerEventData.FramePressState.Pressed;
		}
		if (released)
		{
			return PointerEventData.FramePressState.Released;
		}
		return PointerEventData.FramePressState.NotChanged;
	}
}
