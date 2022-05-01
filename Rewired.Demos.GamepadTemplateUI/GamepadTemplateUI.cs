using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos.GamepadTemplateUI;

public class GamepadTemplateUI : MonoBehaviour
{
	private class Stick
	{
		private RectTransform _transform;

		private Vector2 _origPosition;

		private int _xAxisElementId = -1;

		private int _yAxisElementId = -1;

		public Vector2 position
		{
			get
			{
				return (_transform != null) ? (_transform.anchoredPosition - _origPosition) : Vector2.zero;
			}
			set
			{
				if (!(_transform == null))
				{
					_transform.anchoredPosition = _origPosition + value;
				}
			}
		}

		public Stick(RectTransform transform, int xAxisElementId, int yAxisElementId)
		{
			if (!(transform == null))
			{
				_transform = transform;
				_origPosition = _transform.anchoredPosition;
				_xAxisElementId = xAxisElementId;
				_yAxisElementId = yAxisElementId;
			}
		}

		public void Reset()
		{
			if (!(_transform == null))
			{
				_transform.anchoredPosition = _origPosition;
			}
		}

		public bool ContainsElement(int elementId)
		{
			if (_transform == null)
			{
				return false;
			}
			return elementId == _xAxisElementId || elementId == _yAxisElementId;
		}

		public void SetAxisPosition(int elementId, float value)
		{
			if (!(_transform == null))
			{
				Vector2 position = this.position;
				if (elementId == _xAxisElementId)
				{
					position.x = value;
				}
				else if (elementId == _yAxisElementId)
				{
					position.y = value;
				}
				this.position = position;
			}
		}
	}

	private class UIElement
	{
		public int id;

		public ControllerUIElement element;

		public UIElement(int id, ControllerUIElement element)
		{
			this.id = id;
			this.element = element;
		}
	}

	private const float stickRadius = 20f;

	public int playerId = 0;

	[SerializeField]
	private RectTransform leftStick;

	[SerializeField]
	private RectTransform rightStick;

	[SerializeField]
	private ControllerUIElement leftStickX;

	[SerializeField]
	private ControllerUIElement leftStickY;

	[SerializeField]
	private ControllerUIElement leftStickButton;

	[SerializeField]
	private ControllerUIElement rightStickX;

	[SerializeField]
	private ControllerUIElement rightStickY;

	[SerializeField]
	private ControllerUIElement rightStickButton;

	[SerializeField]
	private ControllerUIElement actionBottomRow1;

	[SerializeField]
	private ControllerUIElement actionBottomRow2;

	[SerializeField]
	private ControllerUIElement actionBottomRow3;

	[SerializeField]
	private ControllerUIElement actionTopRow1;

	[SerializeField]
	private ControllerUIElement actionTopRow2;

	[SerializeField]
	private ControllerUIElement actionTopRow3;

	[SerializeField]
	private ControllerUIElement leftShoulder;

	[SerializeField]
	private ControllerUIElement leftTrigger;

	[SerializeField]
	private ControllerUIElement rightShoulder;

	[SerializeField]
	private ControllerUIElement rightTrigger;

	[SerializeField]
	private ControllerUIElement center1;

	[SerializeField]
	private ControllerUIElement center2;

	[SerializeField]
	private ControllerUIElement center3;

	[SerializeField]
	private ControllerUIElement dPadUp;

	[SerializeField]
	private ControllerUIElement dPadRight;

	[SerializeField]
	private ControllerUIElement dPadDown;

	[SerializeField]
	private ControllerUIElement dPadLeft;

	private UIElement[] _uiElementsArray;

	private Dictionary<int, ControllerUIElement> _uiElements = new Dictionary<int, ControllerUIElement>();

	private IList<ControllerTemplateElementTarget> _tempTargetList = new List<ControllerTemplateElementTarget>(2);

	private Stick[] _sticks;

	private Player player => ReInput.players.GetPlayer(playerId);

	private void Awake()
	{
		_uiElementsArray = new UIElement[23]
		{
			new UIElement(0, leftStickX),
			new UIElement(1, leftStickY),
			new UIElement(17, leftStickButton),
			new UIElement(2, rightStickX),
			new UIElement(3, rightStickY),
			new UIElement(18, rightStickButton),
			new UIElement(4, actionBottomRow1),
			new UIElement(5, actionBottomRow2),
			new UIElement(6, actionBottomRow3),
			new UIElement(7, actionTopRow1),
			new UIElement(8, actionTopRow2),
			new UIElement(9, actionTopRow3),
			new UIElement(14, center1),
			new UIElement(15, center2),
			new UIElement(16, center3),
			new UIElement(19, dPadUp),
			new UIElement(20, dPadRight),
			new UIElement(21, dPadDown),
			new UIElement(22, dPadLeft),
			new UIElement(10, leftShoulder),
			new UIElement(11, leftTrigger),
			new UIElement(12, rightShoulder),
			new UIElement(13, rightTrigger)
		};
		for (int i = 0; i < _uiElementsArray.Length; i++)
		{
			_uiElements.Add(_uiElementsArray[i].id, _uiElementsArray[i].element);
		}
		_sticks = new Stick[2]
		{
			new Stick(leftStick, 0, 1),
			new Stick(rightStick, 2, 3)
		};
		ReInput.ControllerConnectedEvent += OnControllerConnected;
		ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
	}

	private void Start()
	{
		if (ReInput.isReady)
		{
			DrawLabels();
		}
	}

	private void OnDestroy()
	{
		ReInput.ControllerConnectedEvent -= OnControllerConnected;
		ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
	}

	private void Update()
	{
		if (ReInput.isReady)
		{
			DrawActiveElements();
		}
	}

	private void DrawActiveElements()
	{
		for (int k = 0; k < _uiElementsArray.Length; k++)
		{
			_uiElementsArray[k].element.Deactivate();
		}
		for (int j = 0; j < _sticks.Length; j++)
		{
			_sticks[j].Reset();
		}
		IList<InputAction> actions = ReInput.mapping.Actions;
		for (int i = 0; i < actions.Count; i++)
		{
			ActivateElements(player, actions[i].id);
		}
	}

	private void ActivateElements(Player player, int actionId)
	{
		float axisValue = player.GetAxis(actionId);
		if (axisValue == 0f)
		{
			return;
		}
		IList<InputActionSourceData> sources = player.GetCurrentInputSources(actionId);
		for (int i = 0; i < sources.Count; i++)
		{
			InputActionSourceData source = sources[i];
			IGamepadTemplate gamepad = source.controller.GetTemplate<IGamepadTemplate>();
			if (gamepad == null)
			{
				continue;
			}
			gamepad.GetElementTargets(source.actionElementMap, _tempTargetList);
			for (int j = 0; j < _tempTargetList.Count; j++)
			{
				ControllerTemplateElementTarget target = _tempTargetList[j];
				int templateElementId = target.element.id;
				ControllerUIElement uiElement = _uiElements[templateElementId];
				if (target.elementType == ControllerTemplateElementType.Axis)
				{
					uiElement.Activate(axisValue);
				}
				else if (target.elementType == ControllerTemplateElementType.Button && (player.GetButton(actionId) || player.GetNegativeButton(actionId)))
				{
					uiElement.Activate(1f);
				}
				GetStick(templateElementId)?.SetAxisPosition(templateElementId, axisValue * 20f);
			}
		}
	}

	private void DrawLabels()
	{
		for (int j = 0; j < _uiElementsArray.Length; j++)
		{
			_uiElementsArray[j].element.ClearLabels();
		}
		IList<InputAction> actions = ReInput.mapping.Actions;
		for (int i = 0; i < actions.Count; i++)
		{
			DrawLabels(player, actions[i]);
		}
	}

	private void DrawLabels(Player player, InputAction action)
	{
		Controller controller = player.controllers.GetFirstControllerWithTemplate<IGamepadTemplate>();
		if (controller == null)
		{
			return;
		}
		IGamepadTemplate gamepad = controller.GetTemplate<IGamepadTemplate>();
		ControllerMap controllerMap = player.controllers.maps.GetMap(controller, "Default", "Default");
		if (controllerMap != null)
		{
			for (int i = 0; i < _uiElementsArray.Length; i++)
			{
				ControllerUIElement uiElement = _uiElementsArray[i].element;
				int elementId = _uiElementsArray[i].id;
				IControllerTemplateElement element = gamepad.GetElement(elementId);
				DrawLabel(uiElement, action, controllerMap, gamepad, element);
			}
		}
	}

	private void DrawLabel(ControllerUIElement uiElement, InputAction action, ControllerMap controllerMap, IControllerTemplate template, IControllerTemplateElement element)
	{
		if (element.source == null)
		{
			return;
		}
		if (element.source.type == ControllerTemplateElementSourceType.Axis)
		{
			IControllerTemplateAxisSource source2 = element.source as IControllerTemplateAxisSource;
			ActionElementMap aem;
			if (source2.splitAxis)
			{
				aem = controllerMap.GetFirstElementMapWithElementTarget(source2.positiveTarget, action.id, skipDisabledMaps: true);
				if (aem != null)
				{
					uiElement.SetLabel(aem.actionDescriptiveName, AxisRange.Positive);
				}
				aem = controllerMap.GetFirstElementMapWithElementTarget(source2.negativeTarget, action.id, skipDisabledMaps: true);
				if (aem != null)
				{
					uiElement.SetLabel(aem.actionDescriptiveName, AxisRange.Negative);
				}
				return;
			}
			aem = controllerMap.GetFirstElementMapWithElementTarget(source2.fullTarget, action.id, skipDisabledMaps: true);
			if (aem != null)
			{
				uiElement.SetLabel(aem.actionDescriptiveName, AxisRange.Full);
				return;
			}
			aem = controllerMap.GetFirstElementMapWithElementTarget(new ControllerElementTarget(source2.fullTarget)
			{
				axisRange = AxisRange.Positive
			}, action.id, skipDisabledMaps: true);
			if (aem != null)
			{
				uiElement.SetLabel(aem.actionDescriptiveName, AxisRange.Positive);
			}
			aem = controllerMap.GetFirstElementMapWithElementTarget(new ControllerElementTarget(source2.fullTarget)
			{
				axisRange = AxisRange.Negative
			}, action.id, skipDisabledMaps: true);
			if (aem != null)
			{
				uiElement.SetLabel(aem.actionDescriptiveName, AxisRange.Negative);
			}
		}
		else if (element.source.type == ControllerTemplateElementSourceType.Button)
		{
			IControllerTemplateButtonSource source = element.source as IControllerTemplateButtonSource;
			ActionElementMap aem = controllerMap.GetFirstElementMapWithElementTarget(source.target, action.id, skipDisabledMaps: true);
			if (aem != null)
			{
				uiElement.SetLabel(aem.actionDescriptiveName, AxisRange.Full);
			}
		}
	}

	private Stick GetStick(int elementId)
	{
		for (int i = 0; i < _sticks.Length; i++)
		{
			if (_sticks[i].ContainsElement(elementId))
			{
				return _sticks[i];
			}
		}
		return null;
	}

	private void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		DrawLabels();
	}

	private void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		DrawLabels();
	}
}
