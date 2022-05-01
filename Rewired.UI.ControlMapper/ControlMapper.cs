using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
public class ControlMapper : MonoBehaviour
{
	private abstract class GUIElement
	{
		public readonly GameObject gameObject;

		protected readonly TMP_Text text;

		public readonly Selectable selectable;

		protected readonly UIElementInfo uiElementInfo;

		protected bool permanentStateSet;

		protected readonly List<GUIElement> children;

		public RectTransform rectTransform { get; private set; }

		public GUIElement(GameObject gameObject)
		{
			if (gameObject == null)
			{
				Debug.LogError("Rewired Control Mapper: gameObject is null!");
				return;
			}
			selectable = gameObject.GetComponent<Selectable>();
			if (selectable == null)
			{
				Debug.LogError("Rewired Control Mapper: Selectable is null!");
				return;
			}
			this.gameObject = gameObject;
			rectTransform = gameObject.GetComponent<RectTransform>();
			text = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(gameObject);
			uiElementInfo = gameObject.GetComponent<UIElementInfo>();
			children = new List<GUIElement>();
		}

		public GUIElement(Selectable selectable, TMP_Text label)
		{
			if (selectable == null)
			{
				Debug.LogError("Rewired Control Mapper: Selectable is null!");
				return;
			}
			this.selectable = selectable;
			gameObject = selectable.gameObject;
			rectTransform = gameObject.GetComponent<RectTransform>();
			text = label;
			uiElementInfo = gameObject.GetComponent<UIElementInfo>();
			children = new List<GUIElement>();
		}

		public virtual void SetInteractible(bool state, bool playTransition)
		{
			SetInteractible(state, playTransition, permanent: false);
		}

		public virtual void SetInteractible(bool state, bool playTransition, bool permanent)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] != null)
				{
					children[i].SetInteractible(state, playTransition, permanent);
				}
			}
			if (!permanentStateSet && !(selectable == null))
			{
				if (permanent)
				{
					permanentStateSet = true;
				}
				if (selectable.interactable != state)
				{
					UITools.SetInteractable(selectable, state, playTransition);
				}
			}
		}

		public virtual void SetTextWidth(int value)
		{
			if (!(text == null))
			{
				LayoutElement e = text.GetComponent<LayoutElement>();
				if (e == null)
				{
					e = text.gameObject.AddComponent<LayoutElement>();
				}
				e.preferredWidth = value;
			}
		}

		public virtual void SetFirstChildObjectWidth(LayoutElementSizeType type, int value)
		{
			if (rectTransform.childCount != 0)
			{
				Transform child = rectTransform.GetChild(0);
				LayoutElement e = child.GetComponent<LayoutElement>();
				if (e == null)
				{
					e = child.gameObject.AddComponent<LayoutElement>();
				}
				switch (type)
				{
				case LayoutElementSizeType.MinSize:
					e.minWidth = value;
					break;
				case LayoutElementSizeType.PreferredSize:
					e.preferredWidth = value;
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}

		public virtual void SetLabel(string label)
		{
			if (!(text == null))
			{
				text.text = label;
			}
		}

		public virtual string GetLabel()
		{
			if (text == null)
			{
				return string.Empty;
			}
			return text.text;
		}

		public virtual void AddChild(GUIElement child)
		{
			children.Add(child);
		}

		public void SetElementInfoData(string identifier, int intData)
		{
			if (!(uiElementInfo == null))
			{
				uiElementInfo.identifier = identifier;
				uiElementInfo.intData = intData;
			}
		}

		public virtual void SetActive(bool state)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(state);
			}
		}

		protected virtual bool Init()
		{
			bool result = true;
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] != null && !children[i].Init())
				{
					result = false;
				}
			}
			if (selectable == null)
			{
				Debug.LogError("Rewired Control Mapper: UI Element is missing Selectable component!");
				result = false;
			}
			if (rectTransform == null)
			{
				Debug.LogError("Rewired Control Mapper: UI Element is missing RectTransform component!");
				result = false;
			}
			if (uiElementInfo == null)
			{
				Debug.LogError("Rewired Control Mapper: UI Element is missing UIElementInfo component!");
				result = false;
			}
			return result;
		}
	}

	private class GUIButton : GUIElement
	{
		protected Button button => selectable as Button;

		public ButtonInfo buttonInfo => uiElementInfo as ButtonInfo;

		public GUIButton(GameObject gameObject)
			: base(gameObject)
		{
			if (Init())
			{
			}
		}

		public GUIButton(Button button, TMP_Text label)
			: base(button, label)
		{
			if (Init())
			{
			}
		}

		public void SetButtonInfoData(string identifier, int intData)
		{
			SetElementInfoData(identifier, intData);
		}

		public void SetOnClickCallback(Action<ButtonInfo> callback)
		{
			if (!(button == null))
			{
				button.onClick.AddListener(delegate
				{
					callback(buttonInfo);
				});
			}
		}
	}

	private class GUIInputField : GUIElement
	{
		protected Button button => selectable as Button;

		public InputFieldInfo fieldInfo => uiElementInfo as InputFieldInfo;

		public bool hasToggle => toggle != null;

		public GUIToggle toggle { get; private set; }

		public int actionElementMapId
		{
			get
			{
				if (fieldInfo == null)
				{
					return -1;
				}
				return fieldInfo.actionElementMapId;
			}
			set
			{
				if (!(fieldInfo == null))
				{
					fieldInfo.actionElementMapId = value;
				}
			}
		}

		public int controllerId
		{
			get
			{
				if (fieldInfo == null)
				{
					return -1;
				}
				return fieldInfo.controllerId;
			}
			set
			{
				if (!(fieldInfo == null))
				{
					fieldInfo.controllerId = value;
				}
			}
		}

		public GUIInputField(GameObject gameObject)
			: base(gameObject)
		{
			if (Init())
			{
			}
		}

		public GUIInputField(Button button, TMP_Text label)
			: base(button, label)
		{
			if (Init())
			{
			}
		}

		public void SetFieldInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
		{
			SetElementInfoData(string.Empty, intData);
			if (!(fieldInfo == null))
			{
				fieldInfo.actionId = actionId;
				fieldInfo.axisRange = axisRange;
				fieldInfo.controllerType = controllerType;
			}
		}

		public void SetOnClickCallback(Action<InputFieldInfo> callback)
		{
			if (!(button == null))
			{
				button.onClick.AddListener(delegate
				{
					callback(fieldInfo);
				});
			}
		}

		public virtual void SetInteractable(bool state, bool playTransition, bool permanent)
		{
			if (!permanentStateSet)
			{
				if (hasToggle && !state)
				{
					toggle.SetInteractible(state, playTransition, permanent);
				}
				base.SetInteractible(state, playTransition, permanent);
			}
		}

		public void AddToggle(GUIToggle toggle)
		{
			if (toggle != null)
			{
				this.toggle = toggle;
			}
		}
	}

	private class GUIToggle : GUIElement
	{
		protected Toggle toggle => selectable as Toggle;

		public ToggleInfo toggleInfo => uiElementInfo as ToggleInfo;

		public int actionElementMapId
		{
			get
			{
				if (toggleInfo == null)
				{
					return -1;
				}
				return toggleInfo.actionElementMapId;
			}
			set
			{
				if (!(toggleInfo == null))
				{
					toggleInfo.actionElementMapId = value;
				}
			}
		}

		public GUIToggle(GameObject gameObject)
			: base(gameObject)
		{
			if (Init())
			{
			}
		}

		public GUIToggle(Toggle toggle, TMP_Text label)
			: base(toggle, label)
		{
			if (Init())
			{
			}
		}

		public void SetToggleInfoData(int actionId, AxisRange axisRange, ControllerType controllerType, int intData)
		{
			SetElementInfoData(string.Empty, intData);
			if (!(toggleInfo == null))
			{
				toggleInfo.actionId = actionId;
				toggleInfo.axisRange = axisRange;
				toggleInfo.controllerType = controllerType;
			}
		}

		public void SetOnSubmitCallback(Action<ToggleInfo, bool> callback)
		{
			if (toggle == null)
			{
				return;
			}
			EventTrigger trigger = toggle.GetComponent<EventTrigger>();
			if (trigger == null)
			{
				trigger = toggle.gameObject.AddComponent<EventTrigger>();
			}
			EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
			triggerEvent.AddListener(delegate(BaseEventData data)
			{
				if (!(data is PointerEventData pointerEventData) || pointerEventData.button == PointerEventData.InputButton.Left)
				{
					callback(toggleInfo, toggle.isOn);
				}
			});
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				callback = triggerEvent,
				eventID = EventTriggerType.Submit
			};
			EventTrigger.Entry entry2 = new EventTrigger.Entry
			{
				callback = triggerEvent,
				eventID = EventTriggerType.PointerClick
			};
			if (trigger.triggers != null)
			{
				trigger.triggers.Clear();
			}
			else
			{
				trigger.triggers = new List<EventTrigger.Entry>();
			}
			trigger.triggers.Add(entry);
			trigger.triggers.Add(entry2);
		}

		public void SetToggleState(bool state)
		{
			if (!(toggle == null))
			{
				toggle.isOn = state;
			}
		}
	}

	private class GUILabel
	{
		public GameObject gameObject { get; private set; }

		private TMP_Text text { get; set; }

		public RectTransform rectTransform { get; private set; }

		public GUILabel(GameObject gameObject)
		{
			if (gameObject == null)
			{
				Debug.LogError("Rewired Control Mapper: gameObject is null!");
				return;
			}
			text = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(gameObject);
			Check();
		}

		public GUILabel(TMP_Text label)
		{
			text = label;
			if (Check())
			{
			}
		}

		public void SetSize(int width, int height)
		{
			if (!(text == null))
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
		}

		public void SetWidth(int width)
		{
			if (!(text == null))
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
		}

		public void SetHeight(int height)
		{
			if (!(text == null))
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
		}

		public void SetLabel(string label)
		{
			if (!(text == null))
			{
				text.text = label;
			}
		}

		public void SetFontStyle(FontStyles style)
		{
			if (!(text == null))
			{
				text.fontStyle = style;
			}
		}

		public void SetTextAlignment(TextAlignmentOptions alignment)
		{
			if (!(text == null))
			{
				text.alignment = alignment;
			}
		}

		public void SetActive(bool state)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(state);
			}
		}

		private bool Check()
		{
			bool result = true;
			if (text == null)
			{
				Debug.LogError("Rewired Control Mapper: Button is missing Text child component!");
				result = false;
			}
			gameObject = text.gameObject;
			rectTransform = text.GetComponent<RectTransform>();
			return result;
		}
	}

	[Serializable]
	public class MappingSet
	{
		public enum ActionListMode
		{
			ActionCategory,
			Action
		}

		[SerializeField]
		[Tooltip("The Map Category that will be displayed to the user for remapping.")]
		private int _mapCategoryId;

		[SerializeField]
		[Tooltip("Choose whether you want to list Actions to display for this Map Category by individual Action or by all the Actions in an Action Category.")]
		private ActionListMode _actionListMode;

		[SerializeField]
		private int[] _actionCategoryIds;

		[SerializeField]
		private int[] _actionIds;

		private IList<int> _actionCategoryIdsReadOnly;

		private IList<int> _actionIdsReadOnly;

		public int mapCategoryId => _mapCategoryId;

		public ActionListMode actionListMode => _actionListMode;

		public IList<int> actionCategoryIds
		{
			get
			{
				if (_actionCategoryIds == null)
				{
					return null;
				}
				if (_actionCategoryIdsReadOnly == null)
				{
					_actionCategoryIdsReadOnly = new ReadOnlyCollection<int>(_actionCategoryIds);
				}
				return _actionCategoryIdsReadOnly;
			}
		}

		public IList<int> actionIds
		{
			get
			{
				if (_actionIds == null)
				{
					return null;
				}
				if (_actionIdsReadOnly == null)
				{
					_actionIdsReadOnly = new ReadOnlyCollection<int>(_actionIds);
				}
				return _actionIds;
			}
		}

		public bool isValid
		{
			get
			{
				if (_mapCategoryId < 0 || ReInput.mapping.GetMapCategory(_mapCategoryId) == null)
				{
					return false;
				}
				return true;
			}
		}

		public static MappingSet Default => new MappingSet(0, ActionListMode.ActionCategory, new int[1], new int[0]);

		public MappingSet()
		{
			_mapCategoryId = -1;
			_actionCategoryIds = new int[0];
			_actionIds = new int[0];
			_actionListMode = ActionListMode.ActionCategory;
		}

		private MappingSet(int mapCategoryId, ActionListMode actionListMode, int[] actionCategoryIds, int[] actionIds)
		{
			_mapCategoryId = mapCategoryId;
			_actionListMode = actionListMode;
			_actionCategoryIds = actionCategoryIds;
			_actionIds = actionIds;
		}
	}

	[Serializable]
	public class InputBehaviorSettings
	{
		[SerializeField]
		[Tooltip("The Input Behavior that will be displayed to the user for modification.")]
		private int _inputBehaviorId = -1;

		[SerializeField]
		[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
		private bool _showJoystickAxisSensitivity = true;

		[SerializeField]
		[Tooltip("If checked, a slider will be displayed so the user can change this value.")]
		private bool _showMouseXYAxisSensitivity = true;

		[SerializeField]
		[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed as the title for the Input Behavior control set. Otherwise, the name field of the InputBehavior will be used.")]
		private string _labelLanguageKey = string.Empty;

		[SerializeField]
		[Tooltip("If set to a non-blank value, this name will be displayed above the individual slider control. Otherwise, no name will be displayed.")]
		private string _joystickAxisSensitivityLabelLanguageKey = string.Empty;

		[SerializeField]
		[Tooltip("If set to a non-blank value, this key will be used to look up the name in Language to be displayed above the individual slider control. Otherwise, no name will be displayed.")]
		private string _mouseXYAxisSensitivityLabelLanguageKey = string.Empty;

		[SerializeField]
		[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
		private Sprite _joystickAxisSensitivityIcon;

		[SerializeField]
		[Tooltip("The icon to display next to the slider. Set to none for no icon.")]
		private Sprite _mouseXYAxisSensitivityIcon;

		[SerializeField]
		[Tooltip("Minimum value the user is allowed to set for this property.")]
		private float _joystickAxisSensitivityMin = 0f;

		[SerializeField]
		[Tooltip("Maximum value the user is allowed to set for this property.")]
		private float _joystickAxisSensitivityMax = 2f;

		[SerializeField]
		[Tooltip("Minimum value the user is allowed to set for this property.")]
		private float _mouseXYAxisSensitivityMin = 0f;

		[SerializeField]
		[Tooltip("Maximum value the user is allowed to set for this property.")]
		private float _mouseXYAxisSensitivityMax = 2f;

		public int inputBehaviorId => _inputBehaviorId;

		public bool showJoystickAxisSensitivity => _showJoystickAxisSensitivity;

		public bool showMouseXYAxisSensitivity => _showMouseXYAxisSensitivity;

		public string labelLanguageKey => _labelLanguageKey;

		public string joystickAxisSensitivityLabelLanguageKey => _joystickAxisSensitivityLabelLanguageKey;

		public string mouseXYAxisSensitivityLabelLanguageKey => _mouseXYAxisSensitivityLabelLanguageKey;

		public Sprite joystickAxisSensitivityIcon => _joystickAxisSensitivityIcon;

		public Sprite mouseXYAxisSensitivityIcon => _mouseXYAxisSensitivityIcon;

		public float joystickAxisSensitivityMin => _joystickAxisSensitivityMin;

		public float joystickAxisSensitivityMax => _joystickAxisSensitivityMax;

		public float mouseXYAxisSensitivityMin => _mouseXYAxisSensitivityMin;

		public float mouseXYAxisSensitivityMax => _mouseXYAxisSensitivityMax;

		public bool isValid => _inputBehaviorId >= 0 && (_showJoystickAxisSensitivity || _showMouseXYAxisSensitivity);
	}

	[Serializable]
	private class Prefabs
	{
		[SerializeField]
		private GameObject _button;

		[SerializeField]
		private GameObject _fitButton;

		[SerializeField]
		private GameObject _inputGridLabel;

		[SerializeField]
		private GameObject _inputGridHeaderLabel;

		[SerializeField]
		private GameObject _inputGridFieldButton;

		[SerializeField]
		private GameObject _inputGridFieldInvertToggle;

		[SerializeField]
		private GameObject _window;

		[SerializeField]
		private GameObject _windowTitleText;

		[SerializeField]
		private GameObject _windowContentText;

		[SerializeField]
		private GameObject _fader;

		[SerializeField]
		private GameObject _calibrationWindow;

		[SerializeField]
		private GameObject _inputBehaviorsWindow;

		[SerializeField]
		private GameObject _centerStickGraphic;

		[SerializeField]
		private GameObject _moveStickGraphic;

		public GameObject button => _button;

		public GameObject fitButton => _fitButton;

		public GameObject inputGridLabel => _inputGridLabel;

		public GameObject inputGridHeaderLabel => _inputGridHeaderLabel;

		public GameObject inputGridFieldButton => _inputGridFieldButton;

		public GameObject inputGridFieldInvertToggle => _inputGridFieldInvertToggle;

		public GameObject window => _window;

		public GameObject windowTitleText => _windowTitleText;

		public GameObject windowContentText => _windowContentText;

		public GameObject fader => _fader;

		public GameObject calibrationWindow => _calibrationWindow;

		public GameObject inputBehaviorsWindow => _inputBehaviorsWindow;

		public GameObject centerStickGraphic => _centerStickGraphic;

		public GameObject moveStickGraphic => _moveStickGraphic;

		public bool Check()
		{
			if (_button == null || _fitButton == null || _inputGridLabel == null || _inputGridHeaderLabel == null || _inputGridFieldButton == null || _inputGridFieldInvertToggle == null || _window == null || _windowTitleText == null || _windowContentText == null || _fader == null || _calibrationWindow == null || _inputBehaviorsWindow == null)
			{
				return false;
			}
			return true;
		}
	}

	[Serializable]
	private class References
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private CanvasGroup _mainCanvasGroup;

		[SerializeField]
		private Transform _mainContent;

		[SerializeField]
		private Transform _mainContentInner;

		[SerializeField]
		private UIGroup _playersGroup;

		[SerializeField]
		private Transform _controllerGroup;

		[SerializeField]
		private Transform _controllerGroupLabelGroup;

		[SerializeField]
		private UIGroup _controllerSettingsGroup;

		[SerializeField]
		private UIGroup _assignedControllersGroup;

		[SerializeField]
		private Transform _settingsAndMapCategoriesGroup;

		[SerializeField]
		private UIGroup _settingsGroup;

		[SerializeField]
		private UIGroup _mapCategoriesGroup;

		[SerializeField]
		private Transform _inputGridGroup;

		[SerializeField]
		private Transform _inputGridContainer;

		[SerializeField]
		private Transform _inputGridHeadersGroup;

		[SerializeField]
		private Scrollbar _inputGridVScrollbar;

		[SerializeField]
		private ScrollRect _inputGridScrollRect;

		[SerializeField]
		private Transform _inputGridInnerGroup;

		[SerializeField]
		private TMP_Text _controllerNameLabel;

		[SerializeField]
		private Button _removeControllerButton;

		[SerializeField]
		private Button _assignControllerButton;

		[SerializeField]
		private Button _calibrateControllerButton;

		[SerializeField]
		private Button _doneButton;

		[SerializeField]
		private Button _restoreDefaultsButton;

		[SerializeField]
		private Selectable _defaultSelection;

		[SerializeField]
		private GameObject[] _fixedSelectableUIElements;

		[SerializeField]
		private Image _mainBackgroundImage;

		public Canvas canvas => _canvas;

		public CanvasGroup mainCanvasGroup => _mainCanvasGroup;

		public Transform mainContent => _mainContent;

		public Transform mainContentInner => _mainContentInner;

		public UIGroup playersGroup => _playersGroup;

		public Transform controllerGroup => _controllerGroup;

		public Transform controllerGroupLabelGroup => _controllerGroupLabelGroup;

		public UIGroup controllerSettingsGroup => _controllerSettingsGroup;

		public UIGroup assignedControllersGroup => _assignedControllersGroup;

		public Transform settingsAndMapCategoriesGroup => _settingsAndMapCategoriesGroup;

		public UIGroup settingsGroup => _settingsGroup;

		public UIGroup mapCategoriesGroup => _mapCategoriesGroup;

		public Transform inputGridGroup => _inputGridGroup;

		public Transform inputGridContainer => _inputGridContainer;

		public Transform inputGridHeadersGroup => _inputGridHeadersGroup;

		public Scrollbar inputGridVScrollbar => _inputGridVScrollbar;

		public ScrollRect inputGridScrollRect => _inputGridScrollRect;

		public Transform inputGridInnerGroup => _inputGridInnerGroup;

		public TMP_Text controllerNameLabel => _controllerNameLabel;

		public Button removeControllerButton => _removeControllerButton;

		public Button assignControllerButton => _assignControllerButton;

		public Button calibrateControllerButton => _calibrateControllerButton;

		public Button doneButton => _doneButton;

		public Button restoreDefaultsButton => _restoreDefaultsButton;

		public Selectable defaultSelection => _defaultSelection;

		public GameObject[] fixedSelectableUIElements => _fixedSelectableUIElements;

		public Image mainBackgroundImage => _mainBackgroundImage;

		public LayoutElement inputGridLayoutElement { get; set; }

		public Transform inputGridActionColumn { get; set; }

		public Transform inputGridKeyboardColumn { get; set; }

		public Transform inputGridMouseColumn { get; set; }

		public Transform inputGridControllerColumn { get; set; }

		public Transform inputGridHeader1 { get; set; }

		public Transform inputGridHeader2 { get; set; }

		public Transform inputGridHeader3 { get; set; }

		public Transform inputGridHeader4 { get; set; }

		public bool Check()
		{
			if (_canvas == null || _mainCanvasGroup == null || _mainContent == null || _mainContentInner == null || _playersGroup == null || _controllerGroup == null || _controllerGroupLabelGroup == null || _controllerSettingsGroup == null || _assignedControllersGroup == null || _settingsAndMapCategoriesGroup == null || _settingsGroup == null || _mapCategoriesGroup == null || _inputGridGroup == null || _inputGridContainer == null || _inputGridHeadersGroup == null || _inputGridVScrollbar == null || _inputGridScrollRect == null || _inputGridInnerGroup == null || _controllerNameLabel == null || _removeControllerButton == null || _assignControllerButton == null || _calibrateControllerButton == null || _doneButton == null || _restoreDefaultsButton == null || _defaultSelection == null)
			{
				return false;
			}
			return true;
		}
	}

	private class InputActionSet
	{
		private int _actionId;

		private AxisRange _axisRange;

		public int actionId => _actionId;

		public AxisRange axisRange => _axisRange;

		public InputActionSet(int actionId, AxisRange axisRange)
		{
			_actionId = actionId;
			_axisRange = axisRange;
		}
	}

	private class InputMapping
	{
		public string actionName { get; private set; }

		public InputFieldInfo fieldInfo { get; private set; }

		public ControllerMap map { get; private set; }

		public ActionElementMap aem { get; private set; }

		public ControllerType controllerType { get; private set; }

		public int controllerId { get; private set; }

		public ControllerPollingInfo pollingInfo { get; set; }

		public ModifierKeyFlags modifierKeyFlags { get; set; }

		public AxisRange axisRange
		{
			get
			{
				AxisRange axisRange = AxisRange.Positive;
				if (pollingInfo.elementType == ControllerElementType.Axis)
				{
					axisRange = ((fieldInfo.axisRange != 0) ? ((pollingInfo.axisPole == Pole.Positive) ? AxisRange.Positive : AxisRange.Negative) : AxisRange.Full);
				}
				return axisRange;
			}
		}

		public string elementName
		{
			get
			{
				if (controllerType == ControllerType.Keyboard)
				{
					return GetLanguage().GetElementIdentifierName(pollingInfo.keyboardKey, modifierKeyFlags);
				}
				return GetLanguage().GetElementIdentifierName(pollingInfo.controller, pollingInfo.elementIdentifierId, (pollingInfo.axisPole == Pole.Positive) ? AxisRange.Positive : AxisRange.Negative);
			}
		}

		public InputMapping(string actionName, InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, ControllerType controllerType, int controllerId)
		{
			this.actionName = actionName;
			this.fieldInfo = fieldInfo;
			this.map = map;
			this.aem = aem;
			this.controllerType = controllerType;
			this.controllerId = controllerId;
		}

		public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo)
		{
			this.pollingInfo = pollingInfo;
			return ToElementAssignment();
		}

		public ElementAssignment ToElementAssignment(ControllerPollingInfo pollingInfo, ModifierKeyFlags modifierKeyFlags)
		{
			this.pollingInfo = pollingInfo;
			this.modifierKeyFlags = modifierKeyFlags;
			return ToElementAssignment();
		}

		public ElementAssignment ToElementAssignment()
		{
			return new ElementAssignment(controllerType, pollingInfo.elementType, pollingInfo.elementIdentifierId, axisRange, pollingInfo.keyboardKey, modifierKeyFlags, fieldInfo.actionId, (fieldInfo.axisRange == AxisRange.Negative) ? Pole.Negative : Pole.Positive, invert: false, (aem != null) ? aem.id : (-1));
		}
	}

	private class AxisCalibrator
	{
		public AxisCalibrationData data;

		public readonly Joystick joystick;

		public readonly int axisIndex;

		private Controller.Axis axis;

		private bool firstRun;

		public bool isValid => axis != null;

		public AxisCalibrator(Joystick joystick, int axisIndex)
		{
			data = default(AxisCalibrationData);
			this.joystick = joystick;
			this.axisIndex = axisIndex;
			if (joystick != null && axisIndex >= 0 && joystick.axisCount > axisIndex)
			{
				axis = joystick.Axes[axisIndex];
				data = joystick.calibrationMap.GetAxis(axisIndex).GetData();
			}
			firstRun = true;
		}

		public void RecordMinMax()
		{
			if (axis != null)
			{
				float valueRaw = axis.valueRaw;
				if (firstRun || valueRaw < data.min)
				{
					data.min = valueRaw;
				}
				if (firstRun || valueRaw > data.max)
				{
					data.max = valueRaw;
				}
				firstRun = false;
			}
		}

		public void RecordZero()
		{
			if (axis != null)
			{
				data.zero = axis.valueRaw;
			}
		}

		public void Commit()
		{
			if (axis != null)
			{
				AxisCalibration calibration = joystick.calibrationMap.GetAxis(axisIndex);
				if (calibration != null && !((double)Mathf.Abs(data.max - data.min) < 0.1))
				{
					calibration.SetData(data);
				}
			}
		}
	}

	private class IndexedDictionary<TKey, TValue>
	{
		private class Entry
		{
			public TKey key;

			public TValue value;

			public Entry(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}
		}

		private List<Entry> list;

		public int Count => list.Count;

		public TValue this[int index] => list[index].value;

		public IndexedDictionary()
		{
			list = new List<Entry>();
		}

		public TValue Get(TKey key)
		{
			int index = IndexOfKey(key);
			if (index < 0)
			{
				throw new Exception("Key does not exist!");
			}
			return list[index].value;
		}

		public bool TryGet(TKey key, out TValue value)
		{
			value = default(TValue);
			int index = IndexOfKey(key);
			if (index < 0)
			{
				return false;
			}
			value = list[index].value;
			return true;
		}

		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				throw new Exception("Key " + key.ToString() + " is already in use!");
			}
			list.Add(new Entry(key, value));
		}

		public int IndexOfKey(TKey key)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TKey>.Default.Equals(list[i].key, key))
				{
					return i;
				}
			}
			return -1;
		}

		public bool ContainsKey(TKey key)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (EqualityComparer<TKey>.Default.Equals(list[i].key, key))
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			list.Clear();
		}
	}

	private enum LayoutElementSizeType
	{
		MinSize,
		PreferredSize
	}

	private enum WindowType
	{
		None,
		ChooseJoystick,
		JoystickAssignmentConflict,
		ElementAssignment,
		ElementAssignmentPrePolling,
		ElementAssignmentPolling,
		ElementAssignmentResult,
		ElementAssignmentConflict,
		Calibration,
		CalibrateStep1,
		CalibrateStep2
	}

	private class InputGrid
	{
		private InputGridEntryList list;

		private List<GameObject> groups;

		public InputGrid()
		{
			list = new InputGridEntryList();
			groups = new List<GameObject>();
		}

		public void AddMapCategory(int mapCategoryId)
		{
			list.AddMapCategory(mapCategoryId);
		}

		public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			list.AddAction(mapCategoryId, action, axisRange);
		}

		public void AddActionCategory(int mapCategoryId, int actionCategoryId)
		{
			list.AddActionCategory(mapCategoryId, actionCategoryId);
		}

		public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
		{
			list.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, fieldSetContainer);
		}

		public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField)
		{
			list.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, inputField);
		}

		public void AddGroup(GameObject group)
		{
			groups.Add(group);
		}

		public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label)
		{
			list.AddActionLabel(mapCategoryId, actionId, axisRange, label);
		}

		public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label)
		{
			list.AddActionCategoryLabel(mapCategoryId, actionCategoryId, label);
		}

		public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			return list.Contains(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
		}

		public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			return list.GetGUIInputField(mapCategoryId, actionId, axisRange, controllerType, fieldIndex);
		}

		public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId)
		{
			return list.GetActionSets(mapCategoryId);
		}

		public void SetColumnHeight(int mapCategoryId, float height)
		{
			list.SetColumnHeight(mapCategoryId, height);
		}

		public float GetColumnHeight(int mapCategoryId)
		{
			return list.GetColumnHeight(mapCategoryId);
		}

		public void SetFieldsActive(int mapCategoryId, bool state)
		{
			list.SetFieldsActive(mapCategoryId, state);
		}

		public void SetFieldLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
		{
			list.SetLabel(mapCategoryId, actionId, axisRange, controllerType, index, label);
		}

		public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
		{
			list.PopulateField(mapCategoryId, actionId, axisRange, controllerType, controllerId, index, actionElementMapId, label, invert);
		}

		public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
		{
			list.SetFixedFieldData(mapCategoryId, actionId, axisRange, controllerType, controllerId);
		}

		public void InitializeFields(int mapCategoryId)
		{
			list.InitializeFields(mapCategoryId);
		}

		public void Show(int mapCategoryId)
		{
			list.Show(mapCategoryId);
		}

		public void HideAll()
		{
			list.HideAll();
		}

		public void ClearLabels(int mapCategoryId)
		{
			list.ClearLabels(mapCategoryId);
		}

		private void ClearGroups()
		{
			for (int i = 0; i < groups.Count; i++)
			{
				if (!(groups[i] == null))
				{
					UnityEngine.Object.Destroy(groups[i]);
				}
			}
		}

		public void ClearAll()
		{
			ClearGroups();
			list.Clear();
		}
	}

	private class InputGridEntryList
	{
		private class MapCategoryEntry
		{
			private List<ActionEntry> _actionList;

			private IndexedDictionary<int, ActionCategoryEntry> _actionCategoryList;

			private float _columnHeight;

			public List<ActionEntry> actionList => _actionList;

			public IndexedDictionary<int, ActionCategoryEntry> actionCategoryList => _actionCategoryList;

			public float columnHeight
			{
				get
				{
					return _columnHeight;
				}
				set
				{
					_columnHeight = value;
				}
			}

			public MapCategoryEntry()
			{
				_actionList = new List<ActionEntry>();
				_actionCategoryList = new IndexedDictionary<int, ActionCategoryEntry>();
			}

			public ActionEntry GetActionEntry(int actionId, AxisRange axisRange)
			{
				int index = IndexOfActionEntry(actionId, axisRange);
				if (index < 0)
				{
					return null;
				}
				return _actionList[index];
			}

			public int IndexOfActionEntry(int actionId, AxisRange axisRange)
			{
				int count = _actionList.Count;
				for (int i = 0; i < count; i++)
				{
					if (_actionList[i].Matches(actionId, axisRange))
					{
						return i;
					}
				}
				return -1;
			}

			public bool ContainsActionEntry(int actionId, AxisRange axisRange)
			{
				return IndexOfActionEntry(actionId, axisRange) >= 0;
			}

			public ActionEntry AddAction(InputAction action, AxisRange axisRange)
			{
				if (action == null)
				{
					return null;
				}
				if (ContainsActionEntry(action.id, axisRange))
				{
					return null;
				}
				_actionList.Add(new ActionEntry(action, axisRange));
				return _actionList[_actionList.Count - 1];
			}

			public ActionCategoryEntry GetActionCategoryEntry(int actionCategoryId)
			{
				if (!_actionCategoryList.ContainsKey(actionCategoryId))
				{
					return null;
				}
				return _actionCategoryList.Get(actionCategoryId);
			}

			public ActionCategoryEntry AddActionCategory(int actionCategoryId)
			{
				if (actionCategoryId < 0)
				{
					return null;
				}
				if (_actionCategoryList.ContainsKey(actionCategoryId))
				{
					return null;
				}
				_actionCategoryList.Add(actionCategoryId, new ActionCategoryEntry(actionCategoryId));
				return _actionCategoryList.Get(actionCategoryId);
			}

			public void SetAllActive(bool state)
			{
				for (int j = 0; j < _actionCategoryList.Count; j++)
				{
					_actionCategoryList[j].SetActive(state);
				}
				for (int i = 0; i < _actionList.Count; i++)
				{
					_actionList[i].SetActive(state);
				}
			}
		}

		private class ActionEntry
		{
			private IndexedDictionary<int, FieldSet> fieldSets;

			public GUILabel label;

			public readonly InputAction action;

			public readonly AxisRange axisRange;

			public readonly InputActionSet actionSet;

			public ActionEntry(InputAction action, AxisRange axisRange)
			{
				this.action = action;
				this.axisRange = axisRange;
				actionSet = new InputActionSet(action.id, axisRange);
				fieldSets = new IndexedDictionary<int, FieldSet>();
			}

			public void SetLabel(GUILabel label)
			{
				this.label = label;
			}

			public bool Matches(int actionId, AxisRange axisRange)
			{
				if (action.id != actionId)
				{
					return false;
				}
				if (this.axisRange != axisRange)
				{
					return false;
				}
				return true;
			}

			public void AddInputFieldSet(ControllerType controllerType, GameObject fieldSetContainer)
			{
				if (!fieldSets.ContainsKey((int)controllerType))
				{
					fieldSets.Add((int)controllerType, new FieldSet(fieldSetContainer));
				}
			}

			public void AddInputField(ControllerType controllerType, int fieldIndex, GUIInputField inputField)
			{
				if (fieldSets.ContainsKey((int)controllerType))
				{
					FieldSet fieldSet = fieldSets.Get((int)controllerType);
					if (!fieldSet.fields.ContainsKey(fieldIndex))
					{
						fieldSet.fields.Add(fieldIndex, inputField);
					}
				}
			}

			public GUIInputField GetGUIInputField(ControllerType controllerType, int fieldIndex)
			{
				if (!fieldSets.ContainsKey((int)controllerType))
				{
					return null;
				}
				if (!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldIndex))
				{
					return null;
				}
				return fieldSets.Get((int)controllerType).fields.Get(fieldIndex);
			}

			public bool Contains(ControllerType controllerType, int fieldId)
			{
				if (!fieldSets.ContainsKey((int)controllerType))
				{
					return false;
				}
				if (!fieldSets.Get((int)controllerType).fields.ContainsKey(fieldId))
				{
					return false;
				}
				return true;
			}

			public void SetFieldLabel(ControllerType controllerType, int index, string label)
			{
				if (fieldSets.ContainsKey((int)controllerType) && fieldSets.Get((int)controllerType).fields.ContainsKey(index))
				{
					fieldSets.Get((int)controllerType).fields.Get(index).SetLabel(label);
				}
			}

			public void PopulateField(ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
			{
				if (fieldSets.ContainsKey((int)controllerType) && fieldSets.Get((int)controllerType).fields.ContainsKey(index))
				{
					GUIInputField field = fieldSets.Get((int)controllerType).fields.Get(index);
					field.SetLabel(label);
					field.actionElementMapId = actionElementMapId;
					field.controllerId = controllerId;
					if (field.hasToggle)
					{
						field.toggle.SetInteractible(state: true, playTransition: false);
						field.toggle.SetToggleState(invert);
						field.toggle.actionElementMapId = actionElementMapId;
					}
				}
			}

			public void SetFixedFieldData(ControllerType controllerType, int controllerId)
			{
				if (fieldSets.ContainsKey((int)controllerType))
				{
					FieldSet setEntries = fieldSets.Get((int)controllerType);
					int count = setEntries.fields.Count;
					for (int i = 0; i < count; i++)
					{
						setEntries.fields[i].controllerId = controllerId;
					}
				}
			}

			public void Initialize()
			{
				for (int i = 0; i < fieldSets.Count; i++)
				{
					FieldSet fieldSet = fieldSets[i];
					int count = fieldSet.fields.Count;
					for (int j = 0; j < count; j++)
					{
						GUIInputField field = fieldSet.fields[j];
						if (field.hasToggle)
						{
							field.toggle.SetInteractible(state: false, playTransition: false);
							field.toggle.SetToggleState(state: false);
							field.toggle.actionElementMapId = -1;
						}
						field.SetLabel("");
						field.actionElementMapId = -1;
						field.controllerId = -1;
					}
				}
			}

			public void SetActive(bool state)
			{
				if (label != null)
				{
					label.SetActive(state);
				}
				int count = fieldSets.Count;
				for (int i = 0; i < count; i++)
				{
					fieldSets[i].groupContainer.SetActive(state);
				}
			}

			public void ClearLabels()
			{
				for (int i = 0; i < fieldSets.Count; i++)
				{
					FieldSet fieldSet = fieldSets[i];
					int count = fieldSet.fields.Count;
					for (int j = 0; j < count; j++)
					{
						GUIInputField field = fieldSet.fields[j];
						field.SetLabel("");
					}
				}
			}

			public void SetFieldsActive(bool state)
			{
				for (int i = 0; i < fieldSets.Count; i++)
				{
					FieldSet fieldSet = fieldSets[i];
					int count = fieldSet.fields.Count;
					for (int j = 0; j < count; j++)
					{
						GUIInputField field = fieldSet.fields[j];
						field.SetInteractible(state, playTransition: false);
						if (field.hasToggle && (!state || field.toggle.actionElementMapId >= 0))
						{
							field.toggle.SetInteractible(state, playTransition: false);
						}
					}
				}
			}
		}

		private class FieldSet
		{
			public readonly GameObject groupContainer;

			public readonly IndexedDictionary<int, GUIInputField> fields;

			public FieldSet(GameObject groupContainer)
			{
				this.groupContainer = groupContainer;
				fields = new IndexedDictionary<int, GUIInputField>();
			}
		}

		private class ActionCategoryEntry
		{
			public readonly int actionCategoryId;

			public GUILabel label;

			public ActionCategoryEntry(int actionCategoryId)
			{
				this.actionCategoryId = actionCategoryId;
			}

			public void SetLabel(GUILabel label)
			{
				this.label = label;
			}

			public void SetActive(bool state)
			{
				if (label != null)
				{
					label.SetActive(state);
				}
			}
		}

		private IndexedDictionary<int, MapCategoryEntry> entries;

		public InputGridEntryList()
		{
			entries = new IndexedDictionary<int, MapCategoryEntry>();
		}

		public void AddMapCategory(int mapCategoryId)
		{
			if (mapCategoryId >= 0 && !entries.ContainsKey(mapCategoryId))
			{
				entries.Add(mapCategoryId, new MapCategoryEntry());
			}
		}

		public void AddAction(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			AddActionEntry(mapCategoryId, action, axisRange);
		}

		private ActionEntry AddActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			if (action == null)
			{
				return null;
			}
			if (!entries.TryGet(mapCategoryId, out var entry))
			{
				return null;
			}
			return entry.AddAction(action, axisRange);
		}

		public void AddActionLabel(int mapCategoryId, int actionId, AxisRange axisRange, GUILabel label)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				entry.GetActionEntry(actionId, axisRange)?.SetLabel(label);
			}
		}

		public void AddActionCategory(int mapCategoryId, int actionCategoryId)
		{
			AddActionCategoryEntry(mapCategoryId, actionCategoryId);
		}

		private ActionCategoryEntry AddActionCategoryEntry(int mapCategoryId, int actionCategoryId)
		{
			if (!entries.TryGet(mapCategoryId, out var entry))
			{
				return null;
			}
			return entry.AddActionCategory(actionCategoryId);
		}

		public void AddActionCategoryLabel(int mapCategoryId, int actionCategoryId, GUILabel label)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				entry.GetActionCategoryEntry(actionCategoryId)?.SetLabel(label);
			}
		}

		public void AddInputFieldSet(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, GameObject fieldSetContainer)
		{
			GetActionEntry(mapCategoryId, action, axisRange)?.AddInputFieldSet(controllerType, fieldSetContainer);
		}

		public void AddInputField(int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int fieldIndex, GUIInputField inputField)
		{
			GetActionEntry(mapCategoryId, action, axisRange)?.AddInputField(controllerType, fieldIndex, inputField);
		}

		public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange)
		{
			return GetActionEntry(mapCategoryId, actionId, axisRange) != null;
		}

		public bool Contains(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			return GetActionEntry(mapCategoryId, actionId, axisRange)?.Contains(controllerType, fieldIndex) ?? false;
		}

		public void SetColumnHeight(int mapCategoryId, float height)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				entry.columnHeight = height;
			}
		}

		public float GetColumnHeight(int mapCategoryId)
		{
			if (!entries.TryGet(mapCategoryId, out var entry))
			{
				return 0f;
			}
			return entry.columnHeight;
		}

		public GUIInputField GetGUIInputField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
		{
			return GetActionEntry(mapCategoryId, actionId, axisRange)?.GetGUIInputField(controllerType, fieldIndex);
		}

		private ActionEntry GetActionEntry(int mapCategoryId, int actionId, AxisRange axisRange)
		{
			if (actionId < 0)
			{
				return null;
			}
			if (!entries.TryGet(mapCategoryId, out var entry))
			{
				return null;
			}
			return entry.GetActionEntry(actionId, axisRange);
		}

		private ActionEntry GetActionEntry(int mapCategoryId, InputAction action, AxisRange axisRange)
		{
			if (action == null)
			{
				return null;
			}
			return GetActionEntry(mapCategoryId, action.id, axisRange);
		}

		public IEnumerable<InputActionSet> GetActionSets(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				List<ActionEntry> list = entry.actionList;
				int count = list?.Count ?? 0;
				for (int i = 0; i < count; i++)
				{
					yield return list[i].actionSet;
				}
			}
		}

		public void SetFieldsActive(int mapCategoryId, bool state)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				List<ActionEntry> list = entry.actionList;
				int count = list?.Count ?? 0;
				for (int i = 0; i < count; i++)
				{
					list[i].SetFieldsActive(state);
				}
			}
		}

		public void SetLabel(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int index, string label)
		{
			GetActionEntry(mapCategoryId, actionId, axisRange)?.SetFieldLabel(controllerType, index, label);
		}

		public void PopulateField(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId, int index, int actionElementMapId, string label, bool invert)
		{
			GetActionEntry(mapCategoryId, actionId, axisRange)?.PopulateField(controllerType, controllerId, index, actionElementMapId, label, invert);
		}

		public void SetFixedFieldData(int mapCategoryId, int actionId, AxisRange axisRange, ControllerType controllerType, int controllerId)
		{
			GetActionEntry(mapCategoryId, actionId, axisRange)?.SetFixedFieldData(controllerType, controllerId);
		}

		public void InitializeFields(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				List<ActionEntry> list = entry.actionList;
				int count = list?.Count ?? 0;
				for (int i = 0; i < count; i++)
				{
					list[i].Initialize();
				}
			}
		}

		public void Show(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				entry.SetAllActive(state: true);
			}
		}

		public void HideAll()
		{
			for (int i = 0; i < entries.Count; i++)
			{
				entries[i].SetAllActive(state: false);
			}
		}

		public void ClearLabels(int mapCategoryId)
		{
			if (entries.TryGet(mapCategoryId, out var entry))
			{
				List<ActionEntry> list = entry.actionList;
				int count = list?.Count ?? 0;
				for (int i = 0; i < count; i++)
				{
					list[i].ClearLabels();
				}
			}
		}

		public void Clear()
		{
			entries.Clear();
		}
	}

	private class WindowManager
	{
		private List<Window> windows;

		private GameObject windowPrefab;

		private Transform parent;

		private GameObject fader;

		private int idCounter = 0;

		public bool isWindowOpen
		{
			get
			{
				for (int i = windows.Count - 1; i >= 0; i--)
				{
					if (!(windows[i] == null))
					{
						return true;
					}
				}
				return false;
			}
		}

		public Window topWindow
		{
			get
			{
				for (int i = windows.Count - 1; i >= 0; i--)
				{
					if (!(windows[i] == null))
					{
						return windows[i];
					}
				}
				return null;
			}
		}

		public WindowManager(GameObject windowPrefab, GameObject faderPrefab, Transform parent)
		{
			this.windowPrefab = windowPrefab;
			this.parent = parent;
			windows = new List<Window>();
			fader = UnityEngine.Object.Instantiate(faderPrefab);
			fader.transform.SetParent(parent, worldPositionStays: false);
			fader.GetComponent<RectTransform>().localScale = Vector2.one;
			SetFaderActive(state: false);
		}

		public Window OpenWindow(string name, int width, int height)
		{
			Window window = InstantiateWindow(name, width, height);
			UpdateFader();
			return window;
		}

		public Window OpenWindow(GameObject windowPrefab, string name)
		{
			if (windowPrefab == null)
			{
				Debug.LogError("Rewired Control Mapper: Window Prefab is null!");
				return null;
			}
			Window window = InstantiateWindow(name, windowPrefab);
			UpdateFader();
			return window;
		}

		public void CloseTop()
		{
			int i = windows.Count - 1;
			while (i >= 0)
			{
				if (windows[i] == null)
				{
					windows.RemoveAt(i);
					i--;
					continue;
				}
				DestroyWindow(windows[i]);
				windows.RemoveAt(i);
				break;
			}
			UpdateFader();
		}

		public void CloseWindow(int windowId)
		{
			CloseWindow(GetWindow(windowId));
		}

		public void CloseWindow(Window window)
		{
			if (window == null)
			{
				return;
			}
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (windows[i] == null)
				{
					windows.RemoveAt(i);
				}
				else if (!(windows[i] != window))
				{
					DestroyWindow(windows[i]);
					windows.RemoveAt(i);
					break;
				}
			}
			UpdateFader();
			FocusTopWindow();
		}

		public void CloseAll()
		{
			SetFaderActive(state: false);
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (windows[i] == null)
				{
					windows.RemoveAt(i);
				}
				else
				{
					DestroyWindow(windows[i]);
					windows.RemoveAt(i);
				}
			}
			UpdateFader();
		}

		public void CancelAll()
		{
			if (!isWindowOpen)
			{
				return;
			}
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (!(windows[i] == null))
				{
					windows[i].Cancel();
				}
			}
			CloseAll();
		}

		public Window GetWindow(int windowId)
		{
			if (windowId < 0)
			{
				return null;
			}
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (!(windows[i] == null) && windows[i].id == windowId)
				{
					return windows[i];
				}
			}
			return null;
		}

		public bool IsFocused(int windowId)
		{
			if (windowId < 0)
			{
				return false;
			}
			if (topWindow == null)
			{
				return false;
			}
			return topWindow.id == windowId;
		}

		public void Focus(int windowId)
		{
			Focus(GetWindow(windowId));
		}

		public void Focus(Window window)
		{
			if (!(window == null))
			{
				window.TakeInputFocus();
				DefocusOtherWindows(window.id);
			}
		}

		private void DefocusOtherWindows(int focusedWindowId)
		{
			if (focusedWindowId < 0)
			{
				return;
			}
			for (int i = windows.Count - 1; i >= 0; i--)
			{
				if (!(windows[i] == null) && windows[i].id != focusedWindowId)
				{
					windows[i].Disable();
				}
			}
		}

		private void UpdateFader()
		{
			if (!isWindowOpen)
			{
				SetFaderActive(state: false);
				return;
			}
			Transform windowParent = topWindow.transform.parent;
			if (!(windowParent == null))
			{
				SetFaderActive(state: true);
				fader.transform.SetAsLastSibling();
				int topWindowIndex = topWindow.transform.GetSiblingIndex();
				fader.transform.SetSiblingIndex(topWindowIndex);
			}
		}

		private void FocusTopWindow()
		{
			if (!(topWindow == null))
			{
				topWindow.TakeInputFocus();
			}
		}

		private void SetFaderActive(bool state)
		{
			fader.SetActive(state);
		}

		private Window InstantiateWindow(string name, int width, int height)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = "Window";
			}
			GameObject instance = UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
			if (instance == null)
			{
				return null;
			}
			Window window = instance.GetComponent<Window>();
			if (window != null)
			{
				window.Initialize(GetNewId(), IsFocused);
				windows.Add(window);
				window.SetSize(width, height);
			}
			return window;
		}

		private Window InstantiateWindow(string name, GameObject windowPrefab)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = "Window";
			}
			if (windowPrefab == null)
			{
				return null;
			}
			GameObject instance = UITools.InstantiateGUIObject<Window>(windowPrefab, parent, name);
			if (instance == null)
			{
				return null;
			}
			Window window = instance.GetComponent<Window>();
			if (window != null)
			{
				window.Initialize(GetNewId(), IsFocused);
				windows.Add(window);
			}
			return window;
		}

		private void DestroyWindow(Window window)
		{
			if (!(window == null))
			{
				UnityEngine.Object.Destroy(window.gameObject);
			}
		}

		private int GetNewId()
		{
			int id = idCounter;
			idCounter++;
			return id;
		}

		public void ClearCompletely()
		{
			CloseAll();
			if (fader != null)
			{
				UnityEngine.Object.Destroy(fader);
			}
		}
	}

	public const int versionMajor = 1;

	public const int versionMinor = 1;

	public const bool usesTMPro = true;

	private const float blockInputOnFocusTimeout = 0.1f;

	private const string buttonIdentifier_playerSelection = "PlayerSelection";

	private const string buttonIdentifier_removeController = "RemoveController";

	private const string buttonIdentifier_assignController = "AssignController";

	private const string buttonIdentifier_calibrateController = "CalibrateController";

	private const string buttonIdentifier_editInputBehaviors = "EditInputBehaviors";

	private const string buttonIdentifier_mapCategorySelection = "MapCategorySelection";

	private const string buttonIdentifier_assignedControllerSelection = "AssignedControllerSelection";

	private const string buttonIdentifier_done = "Done";

	private const string buttonIdentifier_restoreDefaults = "RestoreDefaults";

	[SerializeField]
	[Tooltip("Must be assigned a Rewired Input Manager scene object or prefab.")]
	private InputManager _rewiredInputManager;

	[SerializeField]
	[Tooltip("Set to True to prevent the Game Object from being destroyed when a new scene is loaded.\n\nNOTE: Changing this value from True to False at runtime will have no effect because Object.DontDestroyOnLoad cannot be undone once set.")]
	private bool _dontDestroyOnLoad;

	[SerializeField]
	[Tooltip("Open the control mapping screen immediately on start. Mainly used for testing.")]
	private bool _openOnStart = false;

	[SerializeField]
	[Tooltip("The Layout of the Keyboard Maps to be displayed.")]
	private int _keyboardMapDefaultLayout = 0;

	[SerializeField]
	[Tooltip("The Layout of the Mouse Maps to be displayed.")]
	private int _mouseMapDefaultLayout = 0;

	[SerializeField]
	[Tooltip("The Layout of the Mouse Maps to be displayed.")]
	private int _joystickMapDefaultLayout = 0;

	[SerializeField]
	private MappingSet[] _mappingSets = new MappingSet[1] { MappingSet.Default };

	[SerializeField]
	[Tooltip("Display a selectable list of Players. If your game only supports 1 player, you can disable this.")]
	private bool _showPlayers = true;

	[SerializeField]
	[Tooltip("Display the Controller column for input mapping.")]
	private bool _showControllers = true;

	[SerializeField]
	[Tooltip("Display the Keyboard column for input mapping.")]
	private bool _showKeyboard = true;

	[SerializeField]
	[Tooltip("Display the Mouse column for input mapping.")]
	private bool _showMouse = true;

	[SerializeField]
	[Tooltip("The maximum number of controllers allowed to be assigned to a Player. If set to any value other than 1, a selectable list of currently-assigned controller will be displayed to the user. [0 = infinite]")]
	private int _maxControllersPerPlayer = 1;

	[SerializeField]
	[Tooltip("Display section labels for each Action Category in the input field grid. Only applies if Action Categories are used to display the Action list.")]
	private bool _showActionCategoryLabels = false;

	[SerializeField]
	[Tooltip("The number of input fields to display for the keyboard. If you want to support alternate mappings on the same device, set this to 2 or more.")]
	private int _keyboardInputFieldCount = 2;

	[SerializeField]
	[Tooltip("The number of input fields to display for the mouse. If you want to support alternate mappings on the same device, set this to 2 or more.")]
	private int _mouseInputFieldCount = 1;

	[SerializeField]
	[Tooltip("The number of input fields to display for joysticks. If you want to support alternate mappings on the same device, set this to 2 or more.")]
	private int _controllerInputFieldCount = 1;

	[SerializeField]
	[Tooltip("Display a full-axis input assignment field for every axis-type Action in the input field grid. Also displays an invert toggle for the user  to invert the full-axis assignment direction.\n\n*IMPORTANT*: This field is required if you have made any full-axis assignments in the Rewired Input Manager or in saved XML user data. Disabling this field when you have full-axis assignments will result in the inability for the user to view, remove, or modify these full-axis assignments. In addition, these assignments may cause conflicts when trying to remap the same axes to Actions.")]
	private bool _showFullAxisInputFields = true;

	[SerializeField]
	[Tooltip("Display a positive and negative input assignment field for every axis-type Action in the input field grid.\n\n*IMPORTANT*: These fields are required to assign buttons, keyboard keys, and hat or D-Pad directions to axis-type Actions. If you have made any split-axis assignments or button/key/D-pad assignments to axis-type Actions in the Rewired Input Manager or in saved XML user data, disabling these fields will result in the inability for the user to view, remove, or modify these assignments. In addition, these assignments may cause conflicts when trying to remap the same elements to Actions.")]
	private bool _showSplitAxisInputFields = true;

	[SerializeField]
	[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to make the conflicting assignment anyway.")]
	private bool _allowElementAssignmentConflicts = false;

	[SerializeField]
	[Tooltip("If enabled, when an element assignment conflict is found, an option will be displayed that allows the user to swap conflicting assignments. This only applies to the first conflicting assignment found. This option will not be displayed if allowElementAssignmentConflicts is true.")]
	private bool _allowElementAssignmentSwap = false;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Action label column.")]
	private int _actionLabelWidth = 200;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Keyboard column.")]
	private int _keyboardColMaxWidth = 360;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Mouse column.")]
	private int _mouseColMaxWidth = 200;

	[SerializeField]
	[Tooltip("The width in relative pixels of the Controller column.")]
	private int _controllerColMaxWidth = 200;

	[SerializeField]
	[Tooltip("The height in relative pixels of the input grid button rows.")]
	private int _inputRowHeight = 40;

	[SerializeField]
	[Tooltip("The padding of the input grid button rows.")]
	private RectOffset _inputRowPadding = new RectOffset();

	[SerializeField]
	[Tooltip("The width in relative pixels of spacing between input fields in a single column.")]
	private int _inputRowFieldSpacing = 0;

	[SerializeField]
	[Tooltip("The width in relative pixels of spacing between columns.")]
	private int _inputColumnSpacing = 40;

	[SerializeField]
	[Tooltip("The height in relative pixels of the space between Action Category sections. Only applicable if Show Action Category Labels is checked.")]
	private int _inputRowCategorySpacing = 20;

	[SerializeField]
	[Tooltip("The width in relative pixels of the invert toggle buttons.")]
	private int _invertToggleWidth = 40;

	[SerializeField]
	[Tooltip("The width in relative pixels of generated popup windows.")]
	private int _defaultWindowWidth = 500;

	[SerializeField]
	[Tooltip("The height in relative pixels of generated popup windows.")]
	private int _defaultWindowHeight = 400;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller when assigning a controller to a Player. If this time elapses with no user input a controller, the assignment will be canceled.")]
	private float _controllerAssignmentTimeout = 5f;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller while waiting for axes to be centered before assigning input.")]
	private float _preInputAssignmentTimeout = 5f;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller when assigning input. If this time elapses with no user input on the target controller, the assignment will be canceled.")]
	private float _inputAssignmentTimeout = 5f;

	[SerializeField]
	[Tooltip("The time in seconds the user has to press an element on a controller during calibration.")]
	private float _axisCalibrationTimeout = 5f;

	[SerializeField]
	[Tooltip("If checked, mouse X-axis movement will always be ignored during input assignment. Check this if you don't want the horizontal mouse axis to be user-assignable to any Actions.")]
	private bool _ignoreMouseXAxisAssignment = true;

	[SerializeField]
	[Tooltip("If checked, mouse Y-axis movement will always be ignored during input assignment. Check this if you don't want the vertical mouse axis to be user-assignable to any Actions.")]
	private bool _ignoreMouseYAxisAssignment = true;

	[SerializeField]
	[Tooltip("An Action that when activated will alternately close or open the main screen as long as no popup windows are open.")]
	private int _screenToggleAction = -1;

	[SerializeField]
	[Tooltip("An Action that when activated will open the main screen if it is closed.")]
	private int _screenOpenAction = -1;

	[SerializeField]
	[Tooltip("An Action that when activated will close the main screen as long as no popup windows are open.")]
	private int _screenCloseAction = -1;

	[SerializeField]
	[Tooltip("An Action that when activated will cancel and close any open popup window. Use with care because the element assigned to this Action can never be mapped by the user (because it would just cancel his assignment).")]
	private int _universalCancelAction = -1;

	[SerializeField]
	[Tooltip("If enabled, Universal Cancel will also close the main screen if pressed when no windows are open.")]
	private bool _universalCancelClosesScreen = true;

	[SerializeField]
	[Tooltip("If checked, controls will be displayed which will allow the user to customize certain Input Behavior settings.")]
	private bool _showInputBehaviorSettings;

	[SerializeField]
	[Tooltip("Customizable settings for user-modifiable Input Behaviors. This can be used for settings like Mouse Look Sensitivity.")]
	private InputBehaviorSettings[] _inputBehaviorSettings;

	[SerializeField]
	[Tooltip("If enabled, UI elements will be themed based on the settings in Theme Settings.")]
	private bool _useThemeSettings = true;

	[SerializeField]
	[Tooltip("Must be assigned a ThemeSettings object. Used to theme UI elements.")]
	private ThemeSettings _themeSettings;

	[SerializeField]
	[Tooltip("Must be assigned a LanguageData object. Used to retrieve language entries for UI elements.")]
	private LanguageDataBase _language;

	[SerializeField]
	[Tooltip("A list of prefabs. You should not have to modify this.")]
	private Prefabs prefabs;

	[SerializeField]
	[Tooltip("A list of references to elements in the hierarchy. You should not have to modify this.")]
	private References references;

	[SerializeField]
	[Tooltip("Show the label for the Players button group?")]
	private bool _showPlayersGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Controller button group?")]
	private bool _showControllerGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Assigned Controllers button group?")]
	private bool _showAssignedControllersGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Settings button group?")]
	private bool _showSettingsGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the Map Categories button group?")]
	private bool _showMapCategoriesGroupLabel = true;

	[SerializeField]
	[Tooltip("Show the label for the current controller name?")]
	private bool _showControllerNameLabel = true;

	[SerializeField]
	[Tooltip("Show the Assigned Controllers group? If joystick auto-assignment is enabled in the Rewired Input Manager and the max joysticks per player is set to any value other than 1, the Assigned Controllers group will always be displayed.")]
	private bool _showAssignedControllers = true;

	private Action _ScreenClosedEvent;

	private Action _ScreenOpenedEvent;

	private Action _PopupWindowOpenedEvent;

	private Action _PopupWindowClosedEvent;

	private Action _InputPollingStartedEvent;

	private Action _InputPollingEndedEvent;

	[SerializeField]
	[Tooltip("Event sent when the UI is closed.")]
	private UnityEvent _onScreenClosed;

	[SerializeField]
	[Tooltip("Event sent when the UI is opened.")]
	private UnityEvent _onScreenOpened;

	[SerializeField]
	[Tooltip("Event sent when a popup window is closed.")]
	private UnityEvent _onPopupWindowClosed;

	[SerializeField]
	[Tooltip("Event sent when a popup window is opened.")]
	private UnityEvent _onPopupWindowOpened;

	[SerializeField]
	[Tooltip("Event sent when polling for input has started.")]
	private UnityEvent _onInputPollingStarted;

	[SerializeField]
	[Tooltip("Event sent when polling for input has ended.")]
	private UnityEvent _onInputPollingEnded;

	private static ControlMapper Instance;

	private bool initialized;

	private int playerCount;

	private InputGrid inputGrid;

	private WindowManager windowManager;

	private int currentPlayerId;

	private int currentMapCategoryId;

	private List<GUIButton> playerButtons;

	private List<GUIButton> mapCategoryButtons;

	private List<GUIButton> assignedControllerButtons;

	private GUIButton assignedControllerButtonsPlaceholder;

	private List<GameObject> miscInstantiatedObjects;

	private GameObject canvas;

	private GameObject lastUISelection;

	private int currentJoystickId = -1;

	private float blockInputOnFocusEndTime;

	private bool isPollingForInput;

	private InputMapping pendingInputMapping;

	private AxisCalibrator pendingAxisCalibration;

	private Action<InputFieldInfo> inputFieldActivatedDelegate;

	private Action<ToggleInfo, bool> inputFieldInvertToggleStateChangedDelegate;

	private Action _restoreDefaultsDelegate;

	public InputManager rewiredInputManager
	{
		get
		{
			return _rewiredInputManager;
		}
		set
		{
			_rewiredInputManager = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool dontDestroyOnLoad
	{
		get
		{
			return _dontDestroyOnLoad;
		}
		set
		{
			if (value != _dontDestroyOnLoad && value)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			}
			_dontDestroyOnLoad = value;
		}
	}

	public int keyboardMapDefaultLayout
	{
		get
		{
			return _keyboardMapDefaultLayout;
		}
		set
		{
			_keyboardMapDefaultLayout = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int mouseMapDefaultLayout
	{
		get
		{
			return _mouseMapDefaultLayout;
		}
		set
		{
			_mouseMapDefaultLayout = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int joystickMapDefaultLayout
	{
		get
		{
			return _joystickMapDefaultLayout;
		}
		set
		{
			_joystickMapDefaultLayout = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showPlayers
	{
		get
		{
			return _showPlayers && ReInput.players.playerCount > 1;
		}
		set
		{
			_showPlayers = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showControllers
	{
		get
		{
			return _showControllers;
		}
		set
		{
			_showControllers = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showKeyboard
	{
		get
		{
			return _showKeyboard;
		}
		set
		{
			_showKeyboard = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showMouse
	{
		get
		{
			return _showMouse;
		}
		set
		{
			_showMouse = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int maxControllersPerPlayer
	{
		get
		{
			return _maxControllersPerPlayer;
		}
		set
		{
			_maxControllersPerPlayer = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showActionCategoryLabels
	{
		get
		{
			return _showActionCategoryLabels;
		}
		set
		{
			_showActionCategoryLabels = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int keyboardInputFieldCount
	{
		get
		{
			return _keyboardInputFieldCount;
		}
		set
		{
			_keyboardInputFieldCount = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int mouseInputFieldCount
	{
		get
		{
			return _mouseInputFieldCount;
		}
		set
		{
			_mouseInputFieldCount = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int controllerInputFieldCount
	{
		get
		{
			return _controllerInputFieldCount;
		}
		set
		{
			_controllerInputFieldCount = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showFullAxisInputFields
	{
		get
		{
			return _showFullAxisInputFields;
		}
		set
		{
			_showFullAxisInputFields = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showSplitAxisInputFields
	{
		get
		{
			return _showSplitAxisInputFields;
		}
		set
		{
			_showSplitAxisInputFields = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool allowElementAssignmentConflicts
	{
		get
		{
			return _allowElementAssignmentConflicts;
		}
		set
		{
			_allowElementAssignmentConflicts = value;
			InspectorPropertyChanged();
		}
	}

	public bool allowElementAssignmentSwap
	{
		get
		{
			return _allowElementAssignmentSwap;
		}
		set
		{
			_allowElementAssignmentSwap = value;
			InspectorPropertyChanged();
		}
	}

	public int actionLabelWidth
	{
		get
		{
			return _actionLabelWidth;
		}
		set
		{
			_actionLabelWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int keyboardColMaxWidth
	{
		get
		{
			return _keyboardColMaxWidth;
		}
		set
		{
			_keyboardColMaxWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int mouseColMaxWidth
	{
		get
		{
			return _mouseColMaxWidth;
		}
		set
		{
			_mouseColMaxWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int controllerColMaxWidth
	{
		get
		{
			return _controllerColMaxWidth;
		}
		set
		{
			_controllerColMaxWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int inputRowHeight
	{
		get
		{
			return _inputRowHeight;
		}
		set
		{
			_inputRowHeight = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int inputColumnSpacing
	{
		get
		{
			return _inputColumnSpacing;
		}
		set
		{
			_inputColumnSpacing = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int inputRowCategorySpacing
	{
		get
		{
			return _inputRowCategorySpacing;
		}
		set
		{
			_inputRowCategorySpacing = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int invertToggleWidth
	{
		get
		{
			return _invertToggleWidth;
		}
		set
		{
			_invertToggleWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int defaultWindowWidth
	{
		get
		{
			return _defaultWindowWidth;
		}
		set
		{
			_defaultWindowWidth = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public int defaultWindowHeight
	{
		get
		{
			return _defaultWindowHeight;
		}
		set
		{
			_defaultWindowHeight = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public float controllerAssignmentTimeout
	{
		get
		{
			return _controllerAssignmentTimeout;
		}
		set
		{
			_controllerAssignmentTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public float preInputAssignmentTimeout
	{
		get
		{
			return _preInputAssignmentTimeout;
		}
		set
		{
			_preInputAssignmentTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public float inputAssignmentTimeout
	{
		get
		{
			return _inputAssignmentTimeout;
		}
		set
		{
			_inputAssignmentTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public float axisCalibrationTimeout
	{
		get
		{
			return _axisCalibrationTimeout;
		}
		set
		{
			_axisCalibrationTimeout = value;
			InspectorPropertyChanged();
		}
	}

	public bool ignoreMouseXAxisAssignment
	{
		get
		{
			return _ignoreMouseXAxisAssignment;
		}
		set
		{
			_ignoreMouseXAxisAssignment = value;
			InspectorPropertyChanged();
		}
	}

	public bool ignoreMouseYAxisAssignment
	{
		get
		{
			return _ignoreMouseYAxisAssignment;
		}
		set
		{
			_ignoreMouseYAxisAssignment = value;
			InspectorPropertyChanged();
		}
	}

	public bool universalCancelClosesScreen
	{
		get
		{
			return _universalCancelClosesScreen;
		}
		set
		{
			_universalCancelClosesScreen = value;
			InspectorPropertyChanged();
		}
	}

	public bool showInputBehaviorSettings
	{
		get
		{
			return _showInputBehaviorSettings;
		}
		set
		{
			_showInputBehaviorSettings = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool useThemeSettings
	{
		get
		{
			return _useThemeSettings;
		}
		set
		{
			_useThemeSettings = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public LanguageDataBase language
	{
		get
		{
			return _language;
		}
		set
		{
			_language = value;
			if (_language != null)
			{
				_language.Initialize();
			}
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showPlayersGroupLabel
	{
		get
		{
			return _showPlayersGroupLabel;
		}
		set
		{
			_showPlayersGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showControllerGroupLabel
	{
		get
		{
			return _showControllerGroupLabel;
		}
		set
		{
			_showControllerGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showAssignedControllersGroupLabel
	{
		get
		{
			return _showAssignedControllersGroupLabel;
		}
		set
		{
			_showAssignedControllersGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showSettingsGroupLabel
	{
		get
		{
			return _showSettingsGroupLabel;
		}
		set
		{
			_showSettingsGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showMapCategoriesGroupLabel
	{
		get
		{
			return _showMapCategoriesGroupLabel;
		}
		set
		{
			_showMapCategoriesGroupLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showControllerNameLabel
	{
		get
		{
			return _showControllerNameLabel;
		}
		set
		{
			_showControllerNameLabel = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public bool showAssignedControllers
	{
		get
		{
			return _showAssignedControllers;
		}
		set
		{
			_showAssignedControllers = value;
			InspectorPropertyChanged(reset: true);
		}
	}

	public Action restoreDefaultsDelegate
	{
		get
		{
			return _restoreDefaultsDelegate;
		}
		set
		{
			_restoreDefaultsDelegate = value;
		}
	}

	public bool isOpen
	{
		get
		{
			if (!initialized)
			{
				return references.canvas != null && references.canvas.gameObject.activeInHierarchy;
			}
			return canvas.activeInHierarchy;
		}
	}

	private bool isFocused
	{
		get
		{
			if (!initialized)
			{
				return false;
			}
			return !windowManager.isWindowOpen;
		}
	}

	private bool inputAllowed
	{
		get
		{
			if (blockInputOnFocusEndTime > Time.unscaledTime)
			{
				return false;
			}
			return true;
		}
	}

	private int inputGridColumnCount
	{
		get
		{
			int count = 1;
			if (_showKeyboard)
			{
				count++;
			}
			if (_showMouse)
			{
				count++;
			}
			if (_showControllers)
			{
				count++;
			}
			return count;
		}
	}

	private int inputGridWidth => _actionLabelWidth + (_showKeyboard ? _keyboardColMaxWidth : 0) + (_showMouse ? _mouseColMaxWidth : 0) + (_showControllers ? _controllerColMaxWidth : 0) + (inputGridColumnCount - 1) * _inputColumnSpacing;

	private Player currentPlayer => ReInput.players.GetPlayer(currentPlayerId);

	private InputCategory currentMapCategory => ReInput.mapping.GetMapCategory(currentMapCategoryId);

	private MappingSet currentMappingSet
	{
		get
		{
			if (currentMapCategoryId < 0)
			{
				return null;
			}
			for (int i = 0; i < _mappingSets.Length; i++)
			{
				if (_mappingSets[i].mapCategoryId == currentMapCategoryId)
				{
					return _mappingSets[i];
				}
			}
			return null;
		}
	}

	private Joystick currentJoystick => ReInput.controllers.GetJoystick(currentJoystickId);

	private bool isJoystickSelected => currentJoystickId >= 0;

	private GameObject currentUISelection => (EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null;

	private bool showSettings => _showInputBehaviorSettings && _inputBehaviorSettings.Length != 0;

	private bool showMapCategories
	{
		get
		{
			if (_mappingSets == null)
			{
				return false;
			}
			if (_mappingSets.Length <= 1)
			{
				return false;
			}
			return true;
		}
	}

	public event Action ScreenClosedEvent
	{
		add
		{
			_ScreenClosedEvent = (Action)Delegate.Combine(_ScreenClosedEvent, value);
		}
		remove
		{
			_ScreenClosedEvent = (Action)Delegate.Remove(_ScreenClosedEvent, value);
		}
	}

	public event Action ScreenOpenedEvent
	{
		add
		{
			_ScreenOpenedEvent = (Action)Delegate.Combine(_ScreenOpenedEvent, value);
		}
		remove
		{
			_ScreenOpenedEvent = (Action)Delegate.Remove(_ScreenOpenedEvent, value);
		}
	}

	public event Action PopupWindowClosedEvent
	{
		add
		{
			_PopupWindowClosedEvent = (Action)Delegate.Combine(_PopupWindowClosedEvent, value);
		}
		remove
		{
			_PopupWindowClosedEvent = (Action)Delegate.Remove(_PopupWindowClosedEvent, value);
		}
	}

	public event Action PopupWindowOpenedEvent
	{
		add
		{
			_PopupWindowOpenedEvent = (Action)Delegate.Combine(_PopupWindowOpenedEvent, value);
		}
		remove
		{
			_PopupWindowOpenedEvent = (Action)Delegate.Remove(_PopupWindowOpenedEvent, value);
		}
	}

	public event Action InputPollingStartedEvent
	{
		add
		{
			_InputPollingStartedEvent = (Action)Delegate.Combine(_InputPollingStartedEvent, value);
		}
		remove
		{
			_InputPollingStartedEvent = (Action)Delegate.Remove(_InputPollingStartedEvent, value);
		}
	}

	public event Action InputPollingEndedEvent
	{
		add
		{
			_InputPollingEndedEvent = (Action)Delegate.Combine(_InputPollingEndedEvent, value);
		}
		remove
		{
			_InputPollingEndedEvent = (Action)Delegate.Remove(_InputPollingEndedEvent, value);
		}
	}

	public event UnityAction onScreenClosed
	{
		add
		{
			_onScreenClosed.AddListener(value);
		}
		remove
		{
			_onScreenClosed.RemoveListener(value);
		}
	}

	public event UnityAction onScreenOpened
	{
		add
		{
			_onScreenOpened.AddListener(value);
		}
		remove
		{
			_onScreenOpened.RemoveListener(value);
		}
	}

	public event UnityAction onPopupWindowClosed
	{
		add
		{
			_onPopupWindowClosed.AddListener(value);
		}
		remove
		{
			_onPopupWindowClosed.RemoveListener(value);
		}
	}

	public event UnityAction onPopupWindowOpened
	{
		add
		{
			_onPopupWindowOpened.AddListener(value);
		}
		remove
		{
			_onPopupWindowOpened.RemoveListener(value);
		}
	}

	public event UnityAction onInputPollingStarted
	{
		add
		{
			_onInputPollingStarted.AddListener(value);
		}
		remove
		{
			_onInputPollingStarted.RemoveListener(value);
		}
	}

	public event UnityAction onInputPollingEnded
	{
		add
		{
			_onInputPollingEnded.AddListener(value);
		}
		remove
		{
			_onInputPollingEnded.RemoveListener(value);
		}
	}

	private void Awake()
	{
		if (_dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
		}
		PreInitialize();
		if (isOpen)
		{
			Initialize();
			Open(force: true);
		}
	}

	private void Start()
	{
		if (_openOnStart)
		{
			Open(force: false);
		}
	}

	private void Update()
	{
		if (isOpen && initialized)
		{
			CheckUISelection();
		}
	}

	private void OnDestroy()
	{
		ReInput.ControllerConnectedEvent -= OnJoystickConnected;
		ReInput.ControllerDisconnectedEvent -= OnJoystickDisconnected;
		ReInput.ControllerPreDisconnectEvent -= OnJoystickPreDisconnect;
		UnsubscribeMenuControlInputEvents();
	}

	private void PreInitialize()
	{
		if (!ReInput.isReady)
		{
			Debug.LogError("Rewired Control Mapper: Rewired has not been initialized! Are you missing a Rewired Input Manager in your scene?");
		}
		else
		{
			SubscribeMenuControlInputEvents();
		}
	}

	private void Initialize()
	{
		if (initialized || !ReInput.isReady)
		{
			return;
		}
		if (_rewiredInputManager == null)
		{
			_rewiredInputManager = UnityEngine.Object.FindObjectOfType<InputManager>();
			if (_rewiredInputManager == null)
			{
				Debug.LogError("Rewired Control Mapper: A Rewired Input Manager was not assigned in the inspector or found in the current scene! Control Mapper will not function.");
				return;
			}
		}
		if (Instance != null)
		{
			Debug.LogError("Rewired Control Mapper: Only one ControlMapper can exist at one time!");
			return;
		}
		Instance = this;
		if (prefabs == null || !prefabs.Check())
		{
			Debug.LogError("Rewired Control Mapper: All prefabs must be assigned in the inspector!");
			return;
		}
		if (references == null || !references.Check())
		{
			Debug.LogError("Rewired Control Mapper: All references must be assigned in the inspector!");
			return;
		}
		references.inputGridLayoutElement = references.inputGridContainer.GetComponent<LayoutElement>();
		if (references.inputGridLayoutElement == null)
		{
			Debug.LogError("Rewired Control Mapper: InputGridContainer is missing LayoutElement component!");
			return;
		}
		if (_showKeyboard && _keyboardInputFieldCount < 1)
		{
			Debug.LogWarning("Rewired Control Mapper: Keyboard Input Fields must be at least 1!");
			_keyboardInputFieldCount = 1;
		}
		if (_showMouse && _mouseInputFieldCount < 1)
		{
			Debug.LogWarning("Rewired Control Mapper: Mouse Input Fields must be at least 1!");
			_mouseInputFieldCount = 1;
		}
		if (_showControllers && _controllerInputFieldCount < 1)
		{
			Debug.LogWarning("Rewired Control Mapper: Controller Input Fields must be at least 1!");
			_controllerInputFieldCount = 1;
		}
		if (_maxControllersPerPlayer < 0)
		{
			Debug.LogWarning("Rewired Control Mapper: Max Controllers Per Player must be at least 0 (no limit)!");
			_maxControllersPerPlayer = 0;
		}
		if (_useThemeSettings && _themeSettings == null)
		{
			Debug.LogWarning("Rewired Control Mapper: To use theming, Theme Settings must be set in the inspector! Theming has been disabled.");
			_useThemeSettings = false;
		}
		if (_language == null)
		{
			Debug.LogError("Rawired UI: Language must be set in the inspector!");
			return;
		}
		_language.Initialize();
		inputFieldActivatedDelegate = OnInputFieldActivated;
		inputFieldInvertToggleStateChangedDelegate = OnInputFieldInvertToggleStateChanged;
		ReInput.ControllerConnectedEvent += OnJoystickConnected;
		ReInput.ControllerDisconnectedEvent += OnJoystickDisconnected;
		ReInput.ControllerPreDisconnectEvent += OnJoystickPreDisconnect;
		playerCount = ReInput.players.playerCount;
		canvas = references.canvas.gameObject;
		windowManager = new WindowManager(prefabs.window, prefabs.fader, references.canvas.transform);
		playerButtons = new List<GUIButton>();
		mapCategoryButtons = new List<GUIButton>();
		assignedControllerButtons = new List<GUIButton>();
		miscInstantiatedObjects = new List<GameObject>();
		currentMapCategoryId = _mappingSets[0].mapCategoryId;
		Draw();
		CreateInputGrid();
		CreateLayout();
		SubscribeFixedUISelectionEvents();
		initialized = true;
	}

	private void OnJoystickConnected(ControllerStatusChangedEventArgs args)
	{
		if (initialized && _showControllers)
		{
			ClearVarsOnJoystickChange();
			ForceRefresh();
		}
	}

	private void OnJoystickDisconnected(ControllerStatusChangedEventArgs args)
	{
		if (initialized && _showControllers)
		{
			ClearVarsOnJoystickChange();
			ForceRefresh();
		}
	}

	private void OnJoystickPreDisconnect(ControllerStatusChangedEventArgs args)
	{
		if (initialized && _showControllers)
		{
		}
	}

	public void OnButtonActivated(ButtonInfo buttonInfo)
	{
		if (initialized && inputAllowed)
		{
			switch (buttonInfo.identifier)
			{
			case "PlayerSelection":
				OnPlayerSelected(buttonInfo.intData, redraw: true);
				break;
			case "AssignedControllerSelection":
				OnControllerSelected(buttonInfo.intData);
				break;
			case "RemoveController":
				OnRemoveCurrentController();
				break;
			case "AssignController":
				ShowAssignControllerWindow();
				break;
			case "CalibrateController":
				ShowCalibrateControllerWindow();
				break;
			case "EditInputBehaviors":
				ShowEditInputBehaviorsWindow();
				break;
			case "MapCategorySelection":
				OnMapCategorySelected(buttonInfo.intData, redraw: true);
				break;
			case "Done":
				Close(save: true);
				break;
			case "RestoreDefaults":
				OnRestoreDefaults();
				break;
			}
		}
	}

	public void OnInputFieldActivated(InputFieldInfo fieldInfo)
	{
		if (!initialized || !inputAllowed || currentPlayer == null)
		{
			return;
		}
		InputAction action = ReInput.mapping.GetAction(fieldInfo.actionId);
		if (action == null)
		{
			return;
		}
		AxisRange range = ((action.type == InputActionType.Axis) ? fieldInfo.axisRange : AxisRange.Full);
		string actionName = _language.GetActionName(action.id, range);
		ControllerMap map = GetControllerMap(fieldInfo.controllerType);
		if (map != null)
		{
			ActionElementMap aem = ((fieldInfo.actionElementMapId >= 0) ? map.GetElementMap(fieldInfo.actionElementMapId) : null);
			if (aem != null)
			{
				ShowBeginElementAssignmentReplacementWindow(fieldInfo, action, map, aem, actionName);
			}
			else
			{
				ShowCreateNewElementAssignmentWindow(fieldInfo, action, map, actionName);
			}
		}
	}

	public void OnInputFieldInvertToggleStateChanged(ToggleInfo toggleInfo, bool newState)
	{
		if (initialized && inputAllowed)
		{
			SetActionAxisInverted(newState, toggleInfo.controllerType, toggleInfo.actionElementMapId);
		}
	}

	private void OnPlayerSelected(int playerId, bool redraw)
	{
		if (initialized)
		{
			currentPlayerId = playerId;
			ClearVarsOnPlayerChange();
			if (redraw)
			{
				Redraw(listsChanged: true, playTransitions: true);
			}
		}
	}

	private void OnControllerSelected(int joystickId)
	{
		if (initialized)
		{
			currentJoystickId = joystickId;
			Redraw(listsChanged: true, playTransitions: true);
		}
	}

	private void OnRemoveCurrentController()
	{
		if (currentPlayer != null && currentJoystickId >= 0)
		{
			RemoveController(currentPlayer, currentJoystickId);
			ClearVarsOnJoystickChange();
			Redraw(listsChanged: false, playTransitions: false);
		}
	}

	private void OnMapCategorySelected(int id, bool redraw)
	{
		if (initialized)
		{
			currentMapCategoryId = id;
			if (redraw)
			{
				Redraw(listsChanged: true, playTransitions: true);
			}
		}
	}

	private void OnRestoreDefaults()
	{
		if (initialized)
		{
			ShowRestoreDefaultsWindow();
		}
	}

	private void OnScreenToggleActionPressed(InputActionEventData data)
	{
		if (!isOpen)
		{
			Open();
		}
		else if (initialized && isFocused)
		{
			Close(save: true);
		}
	}

	private void OnScreenOpenActionPressed(InputActionEventData data)
	{
		Open();
	}

	private void OnScreenCloseActionPressed(InputActionEventData data)
	{
		if (initialized && isOpen && isFocused)
		{
			Close(save: true);
		}
	}

	private void OnUniversalCancelActionPressed(InputActionEventData data)
	{
		if (!initialized || !isOpen)
		{
			return;
		}
		if (_universalCancelClosesScreen)
		{
			if (isFocused)
			{
				Close(save: true);
				return;
			}
		}
		else if (isFocused)
		{
			return;
		}
		CloseAllWindows();
	}

	private void OnWindowCancel(int windowId)
	{
		if (initialized && windowId >= 0)
		{
			CloseWindow(windowId);
		}
	}

	private void OnRemoveElementAssignment(int windowId, ControllerMap map, ActionElementMap aem)
	{
		if (map != null && aem != null)
		{
			map.DeleteElementMap(aem.id);
			CloseWindow(windowId);
		}
	}

	private void OnBeginElementAssignment(InputFieldInfo fieldInfo, ControllerMap map, ActionElementMap aem, string actionName)
	{
		if (!(fieldInfo == null) && map != null)
		{
			pendingInputMapping = new InputMapping(actionName, fieldInfo, map, aem, fieldInfo.controllerType, fieldInfo.controllerId);
			switch (fieldInfo.controllerType)
			{
			case ControllerType.Joystick:
				ShowElementAssignmentPrePollingWindow();
				break;
			case ControllerType.Keyboard:
				ShowElementAssignmentPollingWindow();
				break;
			case ControllerType.Mouse:
				ShowElementAssignmentPollingWindow();
				break;
			default:
				throw new NotImplementedException();
			}
		}
	}

	private void OnControllerAssignmentConfirmed(int windowId, Player player, int controllerId)
	{
		if (windowId >= 0 && player != null && controllerId >= 0)
		{
			AssignController(player, controllerId);
			CloseWindow(windowId);
		}
	}

	private void OnMouseAssignmentConfirmed(int windowId, Player player)
	{
		if (windowId < 0 || player == null)
		{
			return;
		}
		IList<Player> playes = ReInput.players.Players;
		for (int i = 0; i < playes.Count; i++)
		{
			if (playes[i] != player)
			{
				playes[i].controllers.hasMouse = false;
			}
		}
		player.controllers.hasMouse = true;
		CloseWindow(windowId);
	}

	private void OnElementAssignmentConflictReplaceConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers, bool allowSwap)
	{
		if (currentPlayer == null || mapping == null)
		{
			return;
		}
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
			CloseWindow(windowId);
			return;
		}
		ElementAssignmentConflictInfo firstConflict = default(ElementAssignmentConflictInfo);
		ActionElementMap firstConflictAEM = null;
		ActionElementMap origAemToReplaceCopy = null;
		bool swap = false;
		if (allowSwap && mapping.aem != null && GetFirstElementAssignmentConflict(conflictCheck, out firstConflict, skipOtherPlayers))
		{
			swap = true;
			origAemToReplaceCopy = new ActionElementMap(mapping.aem);
			firstConflictAEM = new ActionElementMap(firstConflict.elementMap);
		}
		IList<Player> allPlayers = ReInput.players.AllPlayers;
		for (int i = 0; i < allPlayers.Count; i++)
		{
			Player player = allPlayers[i];
			if (!skipOtherPlayers || player == currentPlayer || player == ReInput.players.SystemPlayer)
			{
				player.controllers.conflictChecking.RemoveElementAssignmentConflicts(conflictCheck);
			}
		}
		mapping.map.ReplaceOrCreateElementMap(assignment);
		if (allowSwap && swap)
		{
			int swapActionId = firstConflictAEM.actionId;
			Pole swapAxisContribution = firstConflictAEM.axisContribution;
			bool swapInvert = firstConflictAEM.invert;
			AxisRange swapAxisRange = origAemToReplaceCopy.axisRange;
			ControllerElementType swapElementType = origAemToReplaceCopy.elementType;
			int swapElementIdentifierId = origAemToReplaceCopy.elementIdentifierId;
			KeyCode swapKeyCode = origAemToReplaceCopy.keyCode;
			ModifierKeyFlags swapModifierKeyFlags = origAemToReplaceCopy.modifierKeyFlags;
			if (swapElementType == firstConflictAEM.elementType && swapElementType == ControllerElementType.Axis)
			{
				if (swapAxisRange != firstConflictAEM.axisRange)
				{
					if (swapAxisRange == AxisRange.Full)
					{
						swapAxisRange = AxisRange.Positive;
					}
					else if (firstConflictAEM.axisRange != 0)
					{
					}
				}
			}
			else if (swapElementType == ControllerElementType.Axis && (firstConflictAEM.elementType == ControllerElementType.Button || (firstConflictAEM.elementType == ControllerElementType.Axis && firstConflictAEM.axisRange != 0)) && swapAxisRange == AxisRange.Full)
			{
				swapAxisRange = AxisRange.Positive;
			}
			if (swapElementType != 0 || swapAxisRange != 0)
			{
				swapInvert = false;
			}
			int usedFieldCount = 0;
			foreach (ActionElementMap aem in firstConflict.controllerMap.ElementMapsWithAction(swapActionId))
			{
				if (SwapIsSameInputRange(swapElementType, swapAxisRange, swapAxisContribution, aem.elementType, aem.axisRange, aem.axisContribution))
				{
					usedFieldCount++;
				}
			}
			if (usedFieldCount < GetControllerInputFieldCount(mapping.controllerType))
			{
				firstConflict.controllerMap.ReplaceOrCreateElementMap(ElementAssignment.CompleteAssignment(mapping.controllerType, swapElementType, swapElementIdentifierId, swapAxisRange, swapKeyCode, swapModifierKeyFlags, swapActionId, swapAxisContribution, swapInvert));
			}
		}
		CloseWindow(windowId);
	}

	private void OnElementAssignmentAddConfirmed(int windowId, InputMapping mapping, ElementAssignment assignment)
	{
		if (currentPlayer != null && mapping != null)
		{
			mapping.map.ReplaceOrCreateElementMap(assignment);
			CloseWindow(windowId);
		}
	}

	private void OnRestoreDefaultsConfirmed(int windowId)
	{
		if (_restoreDefaultsDelegate == null)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (_showControllers)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);
				}
				if (_showKeyboard)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
				}
				if (_showMouse)
				{
					player.controllers.maps.LoadDefaultMaps(ControllerType.Mouse);
				}
			}
		}
		CloseWindow(windowId);
		if (_restoreDefaultsDelegate != null)
		{
			_restoreDefaultsDelegate();
		}
	}

	private void OnAssignControllerWindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		ControllerPollingInfo info = ReInput.controllers.polling.PollAllControllersOfTypeForFirstElementDown(ControllerType.Joystick);
		if (info.success)
		{
			InputPollingStopped();
			if (ReInput.controllers.IsControllerAssigned(ControllerType.Joystick, info.controllerId) && !currentPlayer.controllers.ContainsController(ControllerType.Joystick, info.controllerId))
			{
				ShowControllerAssignmentConflictWindow(info.controllerId);
			}
			else
			{
				OnControllerAssignmentConfirmed(windowId, currentPlayer, info.controllerId);
			}
		}
		else
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
		}
	}

	private void OnElementAssignmentPrePollingWindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (!window.timer.finished)
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			ControllerPollingInfo pollingInfo;
			switch (pendingInputMapping.controllerType)
			{
			case ControllerType.Joystick:
				if (currentPlayer.controllers.joystickCount == 0)
				{
					return;
				}
				pollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, currentJoystick.id);
				break;
			case ControllerType.Keyboard:
			case ControllerType.Mouse:
				pollingInfo = ReInput.controllers.polling.PollControllerForFirstButtonDown(pendingInputMapping.controllerType, 0);
				break;
			default:
				throw new NotImplementedException();
			}
			if (!pollingInfo.success)
			{
				return;
			}
		}
		ShowElementAssignmentPollingWindow();
	}

	private void OnJoystickElementAssignmentPollingWindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
		if (currentPlayer.controllers.joystickCount == 0)
		{
			return;
		}
		ControllerPollingInfo pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Joystick, currentJoystick.id);
		if (pollingInfo.success && IsAllowedAssignment(pendingInputMapping, pollingInfo))
		{
			ElementAssignment assignment = pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, assignment, skipOtherPlayers: false))
			{
				pendingInputMapping.map.ReplaceOrCreateElementMap(assignment);
				InputPollingStopped();
				CloseWindow(windowId);
			}
			else
			{
				InputPollingStopped();
				ShowElementAssignmentConflictWindow(assignment, skipOtherPlayers: false);
			}
		}
	}

	private void OnKeyboardElementAssignmentPollingWindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		PollKeyboardForAssignment(out var pollingInfo, out var modifierKeyPressed, out var modifierFlags, out var label);
		if (modifierKeyPressed)
		{
			window.timer.Start(_inputAssignmentTimeout);
		}
		window.SetContentText(modifierKeyPressed ? string.Empty : Mathf.CeilToInt(window.timer.remaining).ToString(), 2);
		window.SetContentText(label, 1);
		if (pollingInfo.success && IsAllowedAssignment(pendingInputMapping, pollingInfo))
		{
			ElementAssignment assignment = pendingInputMapping.ToElementAssignment(pollingInfo, modifierFlags);
			if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, assignment, skipOtherPlayers: false))
			{
				pendingInputMapping.map.ReplaceOrCreateElementMap(assignment);
				InputPollingStopped();
				CloseWindow(windowId);
			}
			else
			{
				InputPollingStopped();
				ShowElementAssignmentConflictWindow(assignment, skipOtherPlayers: false);
			}
		}
	}

	private void OnMouseElementAssignmentPollingWindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingInputMapping == null)
		{
			return;
		}
		InputPollingStarted();
		if (window.timer.finished)
		{
			InputPollingStopped();
			CloseWindow(windowId);
			return;
		}
		window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
		ControllerPollingInfo pollingInfo;
		if (_ignoreMouseXAxisAssignment || _ignoreMouseYAxisAssignment)
		{
			pollingInfo = default(ControllerPollingInfo);
			foreach (ControllerPollingInfo p in ReInput.controllers.polling.PollControllerForAllElementsDown(ControllerType.Mouse, 0))
			{
				if (p.elementType == ControllerElementType.Axis && ((_ignoreMouseXAxisAssignment && p.elementIndex == 0) || (_ignoreMouseYAxisAssignment && p.elementIndex == 1)))
				{
					continue;
				}
				pollingInfo = p;
				break;
			}
		}
		else
		{
			pollingInfo = ReInput.controllers.polling.PollControllerForFirstElementDown(ControllerType.Mouse, 0);
		}
		if (pollingInfo.success && IsAllowedAssignment(pendingInputMapping, pollingInfo))
		{
			ElementAssignment assignment = pendingInputMapping.ToElementAssignment(pollingInfo);
			if (!HasElementAssignmentConflicts(currentPlayer, pendingInputMapping, assignment, skipOtherPlayers: true))
			{
				pendingInputMapping.map.ReplaceOrCreateElementMap(assignment);
				InputPollingStopped();
				CloseWindow(windowId);
			}
			else
			{
				InputPollingStopped();
				ShowElementAssignmentConflictWindow(assignment, skipOtherPlayers: true);
			}
		}
	}

	private void OnCalibrateAxisStep1WindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingAxisCalibration == null || !pendingAxisCalibration.isValid)
		{
			return;
		}
		InputPollingStarted();
		if (!window.timer.finished)
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			if (currentPlayer.controllers.joystickCount == 0 || !pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
			{
				return;
			}
		}
		pendingAxisCalibration.RecordZero();
		CloseWindow(windowId);
		ShowCalibrateAxisStep2Window();
	}

	private void OnCalibrateAxisStep2WindowUpdate(int windowId)
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = windowManager.GetWindow(windowId);
		if (windowId < 0 || pendingAxisCalibration == null || !pendingAxisCalibration.isValid)
		{
			return;
		}
		if (!window.timer.finished)
		{
			window.SetContentText(Mathf.CeilToInt(window.timer.remaining).ToString(), 1);
			pendingAxisCalibration.RecordMinMax();
			if (currentPlayer.controllers.joystickCount == 0 || !pendingAxisCalibration.joystick.PollForFirstButtonDown().success)
			{
				return;
			}
		}
		EndAxisCalibration();
		InputPollingStopped();
		CloseWindow(windowId);
	}

	private void ShowAssignControllerWindow()
	{
		if (currentPlayer != null && ReInput.controllers.joystickCount != 0)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				window.SetUpdateCallback(OnAssignControllerWindowUpdate);
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.assignControllerWindowTitle);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.assignControllerWindowMessage);
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.timer.Start(_controllerAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowControllerAssignmentConflictWindow(int controllerId)
	{
		if (currentPlayer == null || ReInput.controllers.joystickCount == 0)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (window == null)
		{
			return;
		}
		string otherPlayer = string.Empty;
		IList<Player> players = ReInput.players.Players;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != currentPlayer && players[i].controllers.ContainsController(ControllerType.Joystick, controllerId))
			{
				otherPlayer = _language.GetPlayerName(players[i].id);
				break;
			}
		}
		Joystick joystick = ReInput.controllers.GetJoystick(controllerId);
		window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.controllerAssignmentConflictWindowTitle);
		window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetControllerAssignmentConflictWindowMessage(_language.GetControllerName(joystick), otherPlayer, _language.GetPlayerName(currentPlayer.id)));
		UnityAction cancelCallback = delegate
		{
			OnWindowCancel(window.id);
		};
		window.cancelCallback = cancelCallback;
		window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.yes, delegate
		{
			OnControllerAssignmentConfirmed(window.id, currentPlayer, controllerId);
		}, cancelCallback, setDefault: true);
		window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.no, cancelCallback, cancelCallback, setDefault: false);
		windowManager.Focus(window);
	}

	private void ShowBeginElementAssignmentReplacementWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, ActionElementMap aem, string actionName)
	{
		GUIInputField field = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
		if (field == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (!(window == null))
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, actionName);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), field.GetLabel());
			UnityAction cancelCallback = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = cancelCallback;
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.replace, delegate
			{
				OnBeginElementAssignment(fieldInfo, map, aem, actionName);
			}, cancelCallback, setDefault: true);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.remove, delegate
			{
				OnRemoveElementAssignment(window.id, map, aem);
			}, cancelCallback, setDefault: false);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.cancel, cancelCallback, cancelCallback, setDefault: false);
			windowManager.Focus(window);
		}
	}

	private void ShowCreateNewElementAssignmentWindow(InputFieldInfo fieldInfo, InputAction action, ControllerMap map, string actionName)
	{
		GUIInputField field = inputGrid.GetGUIInputField(currentMapCategoryId, action.id, fieldInfo.axisRange, fieldInfo.controllerType, fieldInfo.intData);
		if (field != null)
		{
			OnBeginElementAssignment(fieldInfo, map, null, actionName);
		}
	}

	private void ShowElementAssignmentPrePollingWindow()
	{
		if (pendingInputMapping == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (!(window == null))
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.elementAssignmentPrePollingWindowMessage);
			if (prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(OnElementAssignmentPrePollingWindowUpdate);
			window.timer.Start(_preInputAssignmentTimeout);
			windowManager.Focus(window);
		}
	}

	private void ShowElementAssignmentPollingWindow()
	{
		if (pendingInputMapping == null)
		{
			return;
		}
		switch (pendingInputMapping.controllerType)
		{
		case ControllerType.Joystick:
			ShowJoystickElementAssignmentPollingWindow();
			break;
		case ControllerType.Keyboard:
			ShowKeyboardElementAssignmentPollingWindow();
			break;
		case ControllerType.Mouse:
			if (currentPlayer.controllers.hasMouse)
			{
				ShowMouseElementAssignmentPollingWindow();
			}
			else
			{
				ShowMouseAssignmentConflictWindow();
			}
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private void ShowJoystickElementAssignmentPollingWindow()
	{
		if (pendingInputMapping != null)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				string message = ((pendingInputMapping.axisRange == AxisRange.Full && _showFullAxisInputFields && !_showSplitAxisInputFields) ? _language.GetJoystickElementAssignmentPollingWindowMessage_FullAxisFieldOnly(pendingInputMapping.actionName) : _language.GetJoystickElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.SetUpdateCallback(OnJoystickElementAssignmentPollingWindowUpdate);
				window.timer.Start(_inputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowKeyboardElementAssignmentPollingWindow()
	{
		if (pendingInputMapping != null)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetKeyboardElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, 0f - (window.GetContentTextHeight(0) + 50f)), "");
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.SetUpdateCallback(OnKeyboardElementAssignmentPollingWindowUpdate);
				window.timer.Start(_inputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowMouseElementAssignmentPollingWindow()
	{
		if (pendingInputMapping != null)
		{
			Window window = OpenWindow(closeOthers: true);
			if (!(window == null))
			{
				string message = ((pendingInputMapping.axisRange == AxisRange.Full && _showFullAxisInputFields && !_showSplitAxisInputFields) ? _language.GetMouseElementAssignmentPollingWindowMessage_FullAxisFieldOnly(pendingInputMapping.actionName) : _language.GetMouseElementAssignmentPollingWindowMessage(pendingInputMapping.actionName));
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, pendingInputMapping.actionName);
				window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
				window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
				window.SetUpdateCallback(OnMouseElementAssignmentPollingWindowUpdate);
				window.timer.Start(_inputAssignmentTimeout);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowElementAssignmentConflictWindow(ElementAssignment assignment, bool skipOtherPlayers)
	{
		if (pendingInputMapping == null)
		{
			return;
		}
		bool blocked = IsBlockingAssignmentConflict(pendingInputMapping, assignment, skipOtherPlayers);
		string message = (blocked ? _language.GetElementAlreadyInUseBlocked(pendingInputMapping.elementName) : _language.GetElementAlreadyInUseCanReplace(pendingInputMapping.elementName, _allowElementAssignmentConflicts));
		Window window = OpenWindow(closeOthers: true);
		if (window == null)
		{
			return;
		}
		window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.elementAssignmentConflictWindowMessage);
		window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
		UnityAction cancelCallback = delegate
		{
			OnWindowCancel(window.id);
		};
		window.cancelCallback = cancelCallback;
		if (blocked)
		{
			window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.okay, cancelCallback, cancelCallback, setDefault: true);
		}
		else
		{
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.replace, delegate
			{
				OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers, allowSwap: false);
			}, cancelCallback, setDefault: true);
			if (_allowElementAssignmentConflicts)
			{
				window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.add, delegate
				{
					OnElementAssignmentAddConfirmed(window.id, pendingInputMapping, assignment);
				}, cancelCallback, setDefault: false);
			}
			else if (ShowSwapButton(window.id, pendingInputMapping, assignment, skipOtherPlayers))
			{
				window.CreateButton(prefabs.fitButton, UIPivot.BottomCenter, UIAnchor.BottomCenter, Vector2.zero, _language.swap, delegate
				{
					OnElementAssignmentConflictReplaceConfirmed(window.id, pendingInputMapping, assignment, skipOtherPlayers, allowSwap: true);
				}, cancelCallback, setDefault: false);
			}
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.cancel, cancelCallback, cancelCallback, setDefault: false);
		}
		windowManager.Focus(window);
	}

	private void ShowMouseAssignmentConflictWindow()
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: true);
		if (window == null)
		{
			return;
		}
		string otherPlayer = string.Empty;
		IList<Player> players = ReInput.players.Players;
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != currentPlayer && players[i].controllers.hasMouse)
			{
				otherPlayer = _language.GetPlayerName(players[i].id);
				break;
			}
		}
		window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.mouseAssignmentConflictWindowTitle);
		window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetMouseAssignmentConflictWindowMessage(otherPlayer, _language.GetPlayerName(currentPlayer.id)));
		UnityAction cancelCallback = delegate
		{
			OnWindowCancel(window.id);
		};
		window.cancelCallback = cancelCallback;
		window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, _language.yes, delegate
		{
			OnMouseAssignmentConfirmed(window.id, currentPlayer);
		}, cancelCallback, setDefault: true);
		window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, _language.no, cancelCallback, cancelCallback, setDefault: false);
		windowManager.Focus(window);
	}

	private void ShowCalibrateControllerWindow()
	{
		if (currentPlayer != null && currentPlayer.controllers.joystickCount != 0)
		{
			CalibrationWindow window = OpenWindow(prefabs.calibrationWindow, "CalibrationWindow", closeOthers: true) as CalibrationWindow;
			if (!(window == null))
			{
				Joystick joystick = currentJoystick;
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateControllerWindowTitle);
				window.SetJoystick(currentPlayer.id, joystick);
				window.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Done, CloseWindow);
				window.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Calibrate, StartAxisCalibration);
				window.SetButtonCallback(CalibrationWindow.ButtonIdentifier.Cancel, CloseWindow);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowCalibrateAxisStep1Window()
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: false);
		if (window == null || pendingAxisCalibration == null)
		{
			return;
		}
		Joystick joystick = pendingAxisCalibration.joystick;
		if (joystick.axisCount == 0)
		{
			return;
		}
		int axisIndex = pendingAxisCalibration.axisIndex;
		if (axisIndex >= 0 && axisIndex < joystick.axisCount)
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep1WindowTitle);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetCalibrateAxisStep1WindowMessage(_language.GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[axisIndex].id, AxisRange.Full)));
			if (prefabs.centerStickGraphic != null)
			{
				window.AddContentImage(prefabs.centerStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(OnCalibrateAxisStep1WindowUpdate);
			window.timer.Start(_axisCalibrationTimeout);
			windowManager.Focus(window);
		}
	}

	private void ShowCalibrateAxisStep2Window()
	{
		if (currentPlayer == null)
		{
			return;
		}
		Window window = OpenWindow(closeOthers: false);
		if (window == null || pendingAxisCalibration == null)
		{
			return;
		}
		Joystick joystick = pendingAxisCalibration.joystick;
		if (joystick.axisCount == 0)
		{
			return;
		}
		int axisIndex = pendingAxisCalibration.axisIndex;
		if (axisIndex >= 0 && axisIndex < joystick.axisCount)
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.calibrateAxisStep2WindowTitle);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), _language.GetCalibrateAxisStep2WindowMessage(_language.GetElementIdentifierName(joystick, joystick.AxisElementIdentifiers[axisIndex].id, AxisRange.Full)));
			if (prefabs.moveStickGraphic != null)
			{
				window.AddContentImage(prefabs.moveStickGraphic, UIPivot.BottomCenter, UIAnchor.BottomCenter, new Vector2(0f, 40f));
			}
			window.AddContentText(prefabs.windowContentText, UIPivot.BottomCenter, UIAnchor.BottomHStretch, Vector2.zero, "");
			window.SetUpdateCallback(OnCalibrateAxisStep2WindowUpdate);
			window.timer.Start(_axisCalibrationTimeout);
			windowManager.Focus(window);
		}
	}

	private void ShowEditInputBehaviorsWindow()
	{
		if (currentPlayer != null && _inputBehaviorSettings != null)
		{
			InputBehaviorWindow window = OpenWindow(prefabs.inputBehaviorsWindow, "EditInputBehaviorsWindow", closeOthers: true) as InputBehaviorWindow;
			if (!(window == null))
			{
				window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, _language.inputBehaviorSettingsWindowTitle);
				window.SetData(currentPlayer.id, _inputBehaviorSettings);
				window.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Done, CloseWindow);
				window.SetButtonCallback(InputBehaviorWindow.ButtonIdentifier.Cancel, CloseWindow);
				windowManager.Focus(window);
			}
		}
	}

	private void ShowRestoreDefaultsWindow()
	{
		if (currentPlayer != null)
		{
			OpenModal(_language.restoreDefaultsWindowTitle, _language.restoreDefaultsWindowMessage, _language.yes, OnRestoreDefaultsConfirmed, _language.no, OnWindowCancel, closeOthers: true);
		}
	}

	private void CreateInputGrid()
	{
		InitializeInputGrid();
		CreateHeaderLabels();
		CreateActionLabelColumn();
		CreateKeyboardInputFieldColumn();
		CreateMouseInputFieldColumn();
		CreateControllerInputFieldColumn();
		CreateInputActionLabels();
		CreateInputFields();
		inputGrid.HideAll();
		ResetInputGridScrollBar();
	}

	private void InitializeInputGrid()
	{
		if (inputGrid == null)
		{
			inputGrid = new InputGrid();
		}
		else
		{
			inputGrid.ClearAll();
		}
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet set = _mappingSets[i];
			if (set == null || !set.isValid)
			{
				continue;
			}
			InputMapCategory mapCat = ReInput.mapping.GetMapCategory(set.mapCategoryId);
			if (mapCat == null || !mapCat.userAssignable)
			{
				continue;
			}
			inputGrid.AddMapCategory(set.mapCategoryId);
			if (set.actionListMode == MappingSet.ActionListMode.ActionCategory)
			{
				IList<int> actionCategoryIds = set.actionCategoryIds;
				for (int k = 0; k < actionCategoryIds.Count; k++)
				{
					int actionCatId = actionCategoryIds[k];
					InputCategory actionCat = ReInput.mapping.GetActionCategory(actionCatId);
					if (actionCat == null || !actionCat.userAssignable)
					{
						continue;
					}
					inputGrid.AddActionCategory(set.mapCategoryId, actionCatId);
					foreach (InputAction action2 in ReInput.mapping.UserAssignableActionsInCategory(actionCatId))
					{
						if (action2.type == InputActionType.Axis)
						{
							if (_showFullAxisInputFields)
							{
								inputGrid.AddAction(set.mapCategoryId, action2, AxisRange.Full);
							}
							if (_showSplitAxisInputFields)
							{
								inputGrid.AddAction(set.mapCategoryId, action2, AxisRange.Positive);
								inputGrid.AddAction(set.mapCategoryId, action2, AxisRange.Negative);
							}
						}
						else if (action2.type == InputActionType.Button)
						{
							inputGrid.AddAction(set.mapCategoryId, action2, AxisRange.Positive);
						}
					}
				}
				continue;
			}
			IList<int> actionIds = set.actionIds;
			for (int j = 0; j < actionIds.Count; j++)
			{
				InputAction action = ReInput.mapping.GetAction(actionIds[j]);
				if (action == null)
				{
					continue;
				}
				if (action.type == InputActionType.Axis)
				{
					if (_showFullAxisInputFields)
					{
						inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Full);
					}
					if (_showSplitAxisInputFields)
					{
						inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Positive);
						inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Negative);
					}
				}
				else if (action.type == InputActionType.Button)
				{
					inputGrid.AddAction(set.mapCategoryId, action, AxisRange.Positive);
				}
			}
		}
		references.inputGridInnerGroup.GetComponent<HorizontalLayoutGroup>().spacing = _inputColumnSpacing;
		references.inputGridLayoutElement.flexibleWidth = 0f;
		references.inputGridLayoutElement.preferredWidth = inputGridWidth;
	}

	private void RefreshInputGridStructure()
	{
		if (currentMappingSet != null)
		{
			inputGrid.HideAll();
			inputGrid.Show(currentMappingSet.mapCategoryId);
			references.inputGridInnerGroup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inputGrid.GetColumnHeight(currentMappingSet.mapCategoryId));
		}
	}

	private void CreateHeaderLabels()
	{
		references.inputGridHeader1 = CreateNewColumnGroup("ActionsHeader", references.inputGridHeadersGroup, _actionLabelWidth).transform;
		GUILabel label = CreateLabel(prefabs.inputGridHeaderLabel, _language.actionColumnLabel, references.inputGridHeader1, Vector2.zero);
		if (_showKeyboard)
		{
			references.inputGridHeader2 = CreateNewColumnGroup("KeybordHeader", references.inputGridHeadersGroup, _keyboardColMaxWidth).transform;
			label = CreateLabel(prefabs.inputGridHeaderLabel, _language.keyboardColumnLabel, references.inputGridHeader2, Vector2.zero);
			label.SetTextAlignment(TextAlignmentOptions.Center);
		}
		if (_showMouse)
		{
			references.inputGridHeader3 = CreateNewColumnGroup("MouseHeader", references.inputGridHeadersGroup, _mouseColMaxWidth).transform;
			label = CreateLabel(prefabs.inputGridHeaderLabel, _language.mouseColumnLabel, references.inputGridHeader3, Vector2.zero);
			label.SetTextAlignment(TextAlignmentOptions.Center);
		}
		if (_showControllers)
		{
			references.inputGridHeader4 = CreateNewColumnGroup("ControllerHeader", references.inputGridHeadersGroup, _controllerColMaxWidth).transform;
			label = CreateLabel(prefabs.inputGridHeaderLabel, _language.controllerColumnLabel, references.inputGridHeader4, Vector2.zero);
			label.SetTextAlignment(TextAlignmentOptions.Center);
		}
	}

	private void CreateActionLabelColumn()
	{
		Transform columnXform = CreateNewColumnGroup("ActionLabelColumn", references.inputGridInnerGroup, _actionLabelWidth).transform;
		references.inputGridActionColumn = columnXform;
	}

	private void CreateKeyboardInputFieldColumn()
	{
		if (_showKeyboard)
		{
			CreateInputFieldColumn("KeyboardColumn", ControllerType.Keyboard, _keyboardColMaxWidth, _keyboardInputFieldCount, disableFullAxis: true);
		}
	}

	private void CreateMouseInputFieldColumn()
	{
		if (_showMouse)
		{
			CreateInputFieldColumn("MouseColumn", ControllerType.Mouse, _mouseColMaxWidth, _mouseInputFieldCount, disableFullAxis: false);
		}
	}

	private void CreateControllerInputFieldColumn()
	{
		if (_showControllers)
		{
			CreateInputFieldColumn("ControllerColumn", ControllerType.Joystick, _controllerColMaxWidth, _controllerInputFieldCount, disableFullAxis: false);
		}
	}

	private void CreateInputFieldColumn(string name, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
	{
		Transform columnXform = CreateNewColumnGroup(name, references.inputGridInnerGroup, maxWidth).transform;
		switch (controllerType)
		{
		case ControllerType.Joystick:
			references.inputGridControllerColumn = columnXform;
			break;
		case ControllerType.Keyboard:
			references.inputGridKeyboardColumn = columnXform;
			break;
		case ControllerType.Mouse:
			references.inputGridMouseColumn = columnXform;
			break;
		default:
			throw new NotImplementedException();
		}
	}

	private void CreateInputActionLabels()
	{
		Transform columnXform = references.inputGridActionColumn;
		for (int mappingSetIndex = 0; mappingSetIndex < _mappingSets.Length; mappingSetIndex++)
		{
			MappingSet set = _mappingSets[mappingSetIndex];
			if (set == null || !set.isValid)
			{
				continue;
			}
			int yPos = 0;
			if (set.actionListMode == MappingSet.ActionListMode.ActionCategory)
			{
				int categoryCount = 0;
				IList<int> actionCategoryIds = set.actionCategoryIds;
				for (int j = 0; j < actionCategoryIds.Count; j++)
				{
					InputCategory category2 = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
					if (category2 == null || !category2.userAssignable || CountIEnumerable(ReInput.mapping.UserAssignableActionsInCategory(category2.id)) == 0)
					{
						continue;
					}
					if (_showActionCategoryLabels)
					{
						if (categoryCount > 0)
						{
							yPos -= _inputRowCategorySpacing;
						}
						GUILabel label3 = CreateLabel(_language.GetActionCategoryName(category2.id), columnXform, new Vector2(0f, yPos));
						label3.SetFontStyle(FontStyles.Bold);
						label3.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
						inputGrid.AddActionCategoryLabel(set.mapCategoryId, category2.id, label3);
						yPos -= _inputRowHeight;
					}
					foreach (InputAction action2 in ReInput.mapping.UserAssignableActionsInCategory(category2.id, sort: true))
					{
						if (action2.type == InputActionType.Axis)
						{
							if (_showFullAxisInputFields)
							{
								GUILabel label2 = CreateLabel(_language.GetActionName(action2.id, AxisRange.Full), columnXform, new Vector2(0f, yPos));
								label2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(set.mapCategoryId, action2.id, AxisRange.Full, label2);
								yPos -= _inputRowHeight;
							}
							if (_showSplitAxisInputFields)
							{
								string positiveDescriptiveName = _language.GetActionName(action2.id, AxisRange.Positive);
								GUILabel label2 = CreateLabel(positiveDescriptiveName, columnXform, new Vector2(0f, yPos));
								label2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(set.mapCategoryId, action2.id, AxisRange.Positive, label2);
								yPos -= _inputRowHeight;
								string negativeDescriptiveName = _language.GetActionName(action2.id, AxisRange.Negative);
								label2 = CreateLabel(negativeDescriptiveName, columnXform, new Vector2(0f, yPos));
								label2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
								inputGrid.AddActionLabel(set.mapCategoryId, action2.id, AxisRange.Negative, label2);
								yPos -= _inputRowHeight;
							}
						}
						else if (action2.type == InputActionType.Button)
						{
							GUILabel label2 = CreateLabel(_language.GetActionName(action2.id), columnXform, new Vector2(0f, yPos));
							label2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(set.mapCategoryId, action2.id, AxisRange.Positive, label2);
							yPos -= _inputRowHeight;
						}
					}
					categoryCount++;
				}
			}
			else
			{
				IList<int> actionIds = set.actionIds;
				for (int i = 0; i < actionIds.Count; i++)
				{
					InputAction action = ReInput.mapping.GetAction(actionIds[i]);
					if (action == null || !action.userAssignable)
					{
						continue;
					}
					InputCategory category = ReInput.mapping.GetActionCategory(action.categoryId);
					if (category == null || !category.userAssignable)
					{
						continue;
					}
					if (action.type == InputActionType.Axis)
					{
						if (_showFullAxisInputFields)
						{
							GUILabel label = CreateLabel(_language.GetActionName(action.id, AxisRange.Full), columnXform, new Vector2(0f, yPos));
							label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Full, label);
							yPos -= _inputRowHeight;
						}
						if (_showSplitAxisInputFields)
						{
							GUILabel label = CreateLabel(_language.GetActionName(action.id, AxisRange.Positive), columnXform, new Vector2(0f, yPos));
							label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Positive, label);
							yPos -= _inputRowHeight;
							label = CreateLabel(_language.GetActionName(action.id, AxisRange.Negative), columnXform, new Vector2(0f, yPos));
							label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
							inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Negative, label);
							yPos -= _inputRowHeight;
						}
					}
					else if (action.type == InputActionType.Button)
					{
						GUILabel label = CreateLabel(_language.GetActionName(action.id), columnXform, new Vector2(0f, yPos));
						label.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
						inputGrid.AddActionLabel(set.mapCategoryId, action.id, AxisRange.Positive, label);
						yPos -= _inputRowHeight;
					}
				}
			}
			inputGrid.SetColumnHeight(set.mapCategoryId, -yPos);
		}
	}

	private void CreateInputFields()
	{
		if (_showControllers)
		{
			CreateInputFields(references.inputGridControllerColumn, ControllerType.Joystick, _controllerColMaxWidth, _controllerInputFieldCount, disableFullAxis: false);
		}
		if (_showKeyboard)
		{
			CreateInputFields(references.inputGridKeyboardColumn, ControllerType.Keyboard, _keyboardColMaxWidth, _keyboardInputFieldCount, disableFullAxis: true);
		}
		if (_showMouse)
		{
			CreateInputFields(references.inputGridMouseColumn, ControllerType.Mouse, _mouseColMaxWidth, _mouseInputFieldCount, disableFullAxis: false);
		}
	}

	private void CreateInputFields(Transform columnXform, ControllerType controllerType, int maxWidth, int cols, bool disableFullAxis)
	{
		for (int mappingSetIndex = 0; mappingSetIndex < _mappingSets.Length; mappingSetIndex++)
		{
			MappingSet set = _mappingSets[mappingSetIndex];
			if (set == null || !set.isValid)
			{
				continue;
			}
			int fieldWidth = maxWidth / cols;
			int yPos = 0;
			int categoryCount = 0;
			if (set.actionListMode == MappingSet.ActionListMode.ActionCategory)
			{
				IList<int> actionCategoryIds = set.actionCategoryIds;
				for (int j = 0; j < actionCategoryIds.Count; j++)
				{
					InputCategory category2 = ReInput.mapping.GetActionCategory(actionCategoryIds[j]);
					if (category2 == null || !category2.userAssignable || CountIEnumerable(ReInput.mapping.UserAssignableActionsInCategory(category2.id)) == 0)
					{
						continue;
					}
					if (_showActionCategoryLabels)
					{
						yPos -= ((categoryCount > 0) ? (_inputRowHeight + _inputRowCategorySpacing) : _inputRowHeight);
					}
					foreach (InputAction action2 in ReInput.mapping.UserAssignableActionsInCategory(category2.id, sort: true))
					{
						if (action2.type == InputActionType.Axis)
						{
							if (_showFullAxisInputFields)
							{
								CreateInputFieldSet(columnXform, set.mapCategoryId, action2, AxisRange.Full, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
							}
							if (_showSplitAxisInputFields)
							{
								CreateInputFieldSet(columnXform, set.mapCategoryId, action2, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
								CreateInputFieldSet(columnXform, set.mapCategoryId, action2, AxisRange.Negative, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
							}
						}
						else if (action2.type == InputActionType.Button)
						{
							CreateInputFieldSet(columnXform, set.mapCategoryId, action2, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
						}
						categoryCount++;
					}
				}
				continue;
			}
			IList<int> actionIds = set.actionIds;
			for (int i = 0; i < actionIds.Count; i++)
			{
				InputAction action = ReInput.mapping.GetAction(actionIds[i]);
				if (action == null || !action.userAssignable)
				{
					continue;
				}
				InputCategory category = ReInput.mapping.GetActionCategory(action.categoryId);
				if (category == null || !category.userAssignable)
				{
					continue;
				}
				if (action.type == InputActionType.Axis)
				{
					if (_showFullAxisInputFields)
					{
						CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Full, controllerType, cols, fieldWidth, ref yPos, disableFullAxis);
					}
					if (_showSplitAxisInputFields)
					{
						CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
						CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Negative, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
					}
				}
				else if (action.type == InputActionType.Button)
				{
					CreateInputFieldSet(columnXform, set.mapCategoryId, action, AxisRange.Positive, controllerType, cols, fieldWidth, ref yPos, disableFullAxis: false);
				}
			}
		}
	}

	private void CreateInputFieldSet(Transform parent, int mapCategoryId, InputAction action, AxisRange axisRange, ControllerType controllerType, int cols, int fieldWidth, ref int yPos, bool disableFullAxis)
	{
		GameObject layoutGroup = CreateNewGUIObject("FieldLayoutGroup", parent, new Vector2(0f, yPos));
		HorizontalLayoutGroup hLayoutGroup = layoutGroup.AddComponent<HorizontalLayoutGroup>();
		hLayoutGroup.padding = _inputRowPadding;
		hLayoutGroup.spacing = _inputRowFieldSpacing;
		RectTransform rt = layoutGroup.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(0f, 1f);
		rt.anchorMax = new Vector2(1f, 1f);
		rt.pivot = new Vector2(0f, 1f);
		rt.sizeDelta = Vector2.zero;
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inputRowHeight);
		inputGrid.AddInputFieldSet(mapCategoryId, action, axisRange, controllerType, layoutGroup);
		for (int fieldIndex = 0; fieldIndex < cols; fieldIndex++)
		{
			int toggleWidth = ((axisRange == AxisRange.Full) ? _invertToggleWidth : 0);
			GUIInputField field = CreateInputField(hLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, fieldIndex);
			field.SetFirstChildObjectWidth(LayoutElementSizeType.PreferredSize, fieldWidth - toggleWidth);
			inputGrid.AddInputField(mapCategoryId, action, axisRange, controllerType, fieldIndex, field);
			if (axisRange == AxisRange.Full)
			{
				if (!disableFullAxis)
				{
					GUIToggle toggle = CreateToggle(prefabs.inputGridFieldInvertToggle, hLayoutGroup.transform, Vector2.zero, "", action.id, axisRange, controllerType, fieldIndex);
					toggle.SetFirstChildObjectWidth(LayoutElementSizeType.MinSize, toggleWidth);
					field.AddToggle(toggle);
				}
				else
				{
					field.SetInteractible(state: false, playTransition: false, permanent: true);
				}
			}
		}
		yPos -= _inputRowHeight;
	}

	private void PopulateInputFields()
	{
		inputGrid.InitializeFields(currentMapCategoryId);
		if (currentPlayer == null)
		{
			return;
		}
		inputGrid.SetFieldsActive(currentMapCategoryId, state: true);
		foreach (InputActionSet actionSet in inputGrid.GetActionSets(currentMapCategoryId))
		{
			if (_showKeyboard)
			{
				ControllerType controllerType = ControllerType.Keyboard;
				int controllerId = 0;
				int layoutId = _keyboardMapDefaultLayout;
				int fieldCount = _keyboardInputFieldCount;
				ControllerMap map3 = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
				PopulateInputFieldGroup(actionSet, map3, controllerType, controllerId, fieldCount);
			}
			if (_showMouse)
			{
				ControllerType controllerType = ControllerType.Mouse;
				int controllerId = 0;
				int layoutId = _mouseMapDefaultLayout;
				int fieldCount = _mouseInputFieldCount;
				ControllerMap map2 = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
				if (currentPlayer.controllers.hasMouse)
				{
					PopulateInputFieldGroup(actionSet, map2, controllerType, controllerId, fieldCount);
				}
			}
			if (isJoystickSelected && currentPlayer.controllers.joystickCount > 0)
			{
				ControllerType controllerType = ControllerType.Joystick;
				int controllerId = currentJoystick.id;
				int layoutId = _joystickMapDefaultLayout;
				int fieldCount = _controllerInputFieldCount;
				ControllerMap map = GetControllerMapOrCreateNew(controllerType, controllerId, layoutId);
				PopulateInputFieldGroup(actionSet, map, controllerType, controllerId, fieldCount);
			}
			else
			{
				DisableInputFieldGroup(actionSet, ControllerType.Joystick, _controllerInputFieldCount);
			}
		}
	}

	private void PopulateInputFieldGroup(InputActionSet actionSet, ControllerMap controllerMap, ControllerType controllerType, int controllerId, int maxFields)
	{
		if (controllerMap == null)
		{
			return;
		}
		int count = 0;
		inputGrid.SetFixedFieldData(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId);
		foreach (ActionElementMap aem in controllerMap.ElementMapsWithAction(actionSet.actionId))
		{
			if (aem.elementType == ControllerElementType.Button)
			{
				if (actionSet.axisRange == AxisRange.Full)
				{
					continue;
				}
				if (actionSet.axisRange == AxisRange.Positive)
				{
					if (aem.axisContribution == Pole.Negative)
					{
						continue;
					}
				}
				else if (actionSet.axisRange == AxisRange.Negative && aem.axisContribution == Pole.Positive)
				{
					continue;
				}
				inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), invert: false);
			}
			else if (aem.elementType == ControllerElementType.Axis)
			{
				if (actionSet.axisRange == AxisRange.Full)
				{
					if (aem.axisRange != 0)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), aem.invert);
				}
				else if (actionSet.axisRange == AxisRange.Positive)
				{
					if ((aem.axisRange == AxisRange.Full && ReInput.mapping.GetAction(actionSet.actionId).type != InputActionType.Button) || aem.axisContribution == Pole.Negative)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), invert: false);
				}
				else if (actionSet.axisRange == AxisRange.Negative)
				{
					if (aem.axisRange == AxisRange.Full || aem.axisContribution == Pole.Positive)
					{
						continue;
					}
					inputGrid.PopulateField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, controllerId, count, aem.id, _language.GetElementIdentifierName(aem), invert: false);
				}
			}
			count++;
			if (count > maxFields)
			{
				break;
			}
		}
	}

	private void DisableInputFieldGroup(InputActionSet actionSet, ControllerType controllerType, int fieldCount)
	{
		for (int i = 0; i < fieldCount; i++)
		{
			inputGrid.GetGUIInputField(currentMapCategoryId, actionSet.actionId, actionSet.axisRange, controllerType, i)?.SetInteractible(state: false, playTransition: false);
		}
	}

	private void ResetInputGridScrollBar()
	{
		references.inputGridInnerGroup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		references.inputGridVScrollbar.value = 1f;
		references.inputGridScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
	}

	private void CreateLayout()
	{
		references.playersGroup.gameObject.SetActive(showPlayers);
		references.controllerGroup.gameObject.SetActive(_showControllers);
		references.assignedControllersGroup.gameObject.SetActive(_showControllers && ShowAssignedControllers());
		references.settingsAndMapCategoriesGroup.gameObject.SetActive(showSettings || showMapCategories);
		references.settingsGroup.gameObject.SetActive(showSettings);
		references.mapCategoriesGroup.gameObject.SetActive(showMapCategories);
	}

	private void Draw()
	{
		DrawPlayersGroup();
		DrawControllersGroup();
		DrawSettingsGroup();
		DrawMapCategoriesGroup();
		DrawWindowButtonsGroup();
	}

	private void DrawPlayersGroup()
	{
		if (!showPlayers)
		{
			return;
		}
		references.playersGroup.labelText = _language.playersGroupLabel;
		references.playersGroup.SetLabelActive(_showPlayersGroupLabel);
		for (int i = 0; i < playerCount; i++)
		{
			Player player = ReInput.players.GetPlayer(i);
			if (player != null)
			{
				GameObject instance = UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.playersGroup.content, "Player" + i + "Button");
				GUIButton button = new GUIButton(instance);
				button.SetLabel(_language.GetPlayerName(player.id));
				button.SetButtonInfoData("PlayerSelection", player.id);
				button.SetOnClickCallback(OnButtonActivated);
				button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
				playerButtons.Add(button);
			}
		}
	}

	private void DrawControllersGroup()
	{
		if (_showControllers)
		{
			references.controllerSettingsGroup.labelText = _language.controllerSettingsGroupLabel;
			references.controllerSettingsGroup.SetLabelActive(_showControllerGroupLabel);
			references.controllerNameLabel.gameObject.SetActive(_showControllerNameLabel);
			references.controllerGroupLabelGroup.gameObject.SetActive(_showControllerGroupLabel || _showControllerNameLabel);
			if (ShowAssignedControllers())
			{
				references.assignedControllersGroup.labelText = _language.assignedControllersGroupLabel;
				references.assignedControllersGroup.SetLabelActive(_showAssignedControllersGroupLabel);
			}
			ButtonInfo buttonInfo = references.removeControllerButton.GetComponent<ButtonInfo>();
			buttonInfo.text.text = _language.removeControllerButtonLabel;
			buttonInfo = references.calibrateControllerButton.GetComponent<ButtonInfo>();
			buttonInfo.text.text = _language.calibrateControllerButtonLabel;
			buttonInfo = references.assignControllerButton.GetComponent<ButtonInfo>();
			buttonInfo.text.text = _language.assignControllerButtonLabel;
			GUIButton guiButton = CreateButton(_language.none, references.assignedControllersGroup.content, Vector2.zero);
			guiButton.SetInteractible(state: false, playTransition: false, permanent: true);
			assignedControllerButtonsPlaceholder = guiButton;
		}
	}

	private void DrawSettingsGroup()
	{
		if (showSettings)
		{
			references.settingsGroup.labelText = _language.settingsGroupLabel;
			references.settingsGroup.SetLabelActive(_showSettingsGroupLabel);
			GUIButton button = CreateButton(_language.inputBehaviorSettingsButtonLabel, references.settingsGroup.content, Vector2.zero);
			miscInstantiatedObjects.Add(button.gameObject);
			button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
			button.SetButtonInfoData("EditInputBehaviors", 0);
			button.SetOnClickCallback(OnButtonActivated);
		}
	}

	private void DrawMapCategoriesGroup()
	{
		if (!showMapCategories || _mappingSets == null)
		{
			return;
		}
		references.mapCategoriesGroup.labelText = _language.mapCategoriesGroupLabel;
		references.mapCategoriesGroup.SetLabelActive(_showMapCategoriesGroupLabel);
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			MappingSet set = _mappingSets[i];
			if (set != null)
			{
				InputMapCategory cat = ReInput.mapping.GetMapCategory(set.mapCategoryId);
				if (cat != null)
				{
					GameObject instance = UITools.InstantiateGUIObject<ButtonInfo>(prefabs.button, references.mapCategoriesGroup.content, cat.name + "Button");
					GUIButton button = new GUIButton(instance);
					button.SetLabel(_language.GetMapCategoryName(cat.id));
					button.SetButtonInfoData("MapCategorySelection", cat.id);
					button.SetOnClickCallback(OnButtonActivated);
					button.buttonInfo.OnSelectedEvent += OnUIElementSelected;
					mapCategoryButtons.Add(button);
				}
			}
		}
	}

	private void DrawWindowButtonsGroup()
	{
		references.doneButton.GetComponent<ButtonInfo>().text.text = _language.doneButtonLabel;
		references.restoreDefaultsButton.GetComponent<ButtonInfo>().text.text = _language.restoreDefaultsButtonLabel;
	}

	private void Redraw(bool listsChanged, bool playTransitions)
	{
		RedrawPlayerGroup(playTransitions);
		RedrawControllerGroup();
		RedrawMapCategoriesGroup(playTransitions);
		RedrawInputGrid(listsChanged);
		if (currentUISelection == null || !currentUISelection.activeInHierarchy)
		{
			RestoreLastUISelection();
		}
	}

	private void RedrawPlayerGroup(bool playTransitions)
	{
		if (showPlayers)
		{
			for (int i = 0; i < playerButtons.Count; i++)
			{
				bool state = currentPlayerId != playerButtons[i].buttonInfo.intData;
				playerButtons[i].SetInteractible(state, playTransitions);
			}
		}
	}

	private void RedrawControllerGroup()
	{
		int prevSelectedButtonJoyId = -1;
		references.controllerNameLabel.text = _language.none;
		UITools.SetInteractable(references.removeControllerButton, state: false, playTransition: false);
		UITools.SetInteractable(references.assignControllerButton, state: false, playTransition: false);
		UITools.SetInteractable(references.calibrateControllerButton, state: false, playTransition: false);
		if (ShowAssignedControllers())
		{
			foreach (GUIButton button in assignedControllerButtons)
			{
				if (!(button.gameObject == null))
				{
					if (currentUISelection == button.gameObject)
					{
						prevSelectedButtonJoyId = button.buttonInfo.intData;
					}
					UnityEngine.Object.Destroy(button.gameObject);
				}
			}
			assignedControllerButtons.Clear();
			assignedControllerButtonsPlaceholder.SetActive(state: true);
		}
		Player player = ReInput.players.GetPlayer(currentPlayerId);
		if (player == null)
		{
			return;
		}
		if (ShowAssignedControllers())
		{
			if (player.controllers.joystickCount > 0)
			{
				assignedControllerButtonsPlaceholder.SetActive(state: false);
			}
			foreach (Joystick joystick in player.controllers.Joysticks)
			{
				GUIButton button3 = CreateButton(_language.GetControllerName(joystick), references.assignedControllersGroup.content, Vector2.zero);
				button3.SetButtonInfoData("AssignedControllerSelection", joystick.id);
				button3.SetOnClickCallback(OnButtonActivated);
				button3.buttonInfo.OnSelectedEvent += OnUIElementSelected;
				assignedControllerButtons.Add(button3);
				if (joystick.id == currentJoystickId)
				{
					button3.SetInteractible(state: false, playTransition: true);
				}
			}
			if (player.controllers.joystickCount > 0 && !isJoystickSelected)
			{
				currentJoystickId = player.controllers.Joysticks[0].id;
				assignedControllerButtons[0].SetInteractible(state: false, playTransition: false);
			}
			if (prevSelectedButtonJoyId >= 0)
			{
				foreach (GUIButton button2 in assignedControllerButtons)
				{
					if (button2.buttonInfo.intData == prevSelectedButtonJoyId)
					{
						SetUISelection(button2.gameObject);
						break;
					}
				}
			}
		}
		else if (player.controllers.joystickCount > 0 && !isJoystickSelected)
		{
			currentJoystickId = player.controllers.Joysticks[0].id;
		}
		if (isJoystickSelected && player.controllers.joystickCount > 0)
		{
			references.removeControllerButton.interactable = true;
			references.controllerNameLabel.text = _language.GetControllerName(currentJoystick);
			if (currentJoystick.axisCount > 0)
			{
				references.calibrateControllerButton.interactable = true;
			}
		}
		int playerJoystickCount = player.controllers.joystickCount;
		int totalJoystickCount = ReInput.controllers.joystickCount;
		int maxJoysticksPerPlayer = GetMaxControllersPerPlayer();
		bool infiniteJoysticksPerPlayer = maxJoysticksPerPlayer == 0;
		if (totalJoystickCount > 0 && playerJoystickCount < totalJoystickCount && (maxJoysticksPerPlayer == 1 || infiniteJoysticksPerPlayer || playerJoystickCount < maxJoysticksPerPlayer))
		{
			UITools.SetInteractable(references.assignControllerButton, state: true, playTransition: false);
		}
	}

	private void RedrawMapCategoriesGroup(bool playTransitions)
	{
		if (showMapCategories)
		{
			for (int i = 0; i < mapCategoryButtons.Count; i++)
			{
				bool state = currentMapCategoryId != mapCategoryButtons[i].buttonInfo.intData;
				mapCategoryButtons[i].SetInteractible(state, playTransitions);
			}
		}
	}

	private void RedrawInputGrid(bool listsChanged)
	{
		if (listsChanged)
		{
			RefreshInputGridStructure();
		}
		PopulateInputFields();
		if (listsChanged)
		{
			ResetInputGridScrollBar();
		}
	}

	private void ForceRefresh()
	{
		if (windowManager.isWindowOpen)
		{
			CloseAllWindows();
		}
		else
		{
			Redraw(listsChanged: false, playTransitions: false);
		}
	}

	private void CreateInputCategoryRow(ref int rowCount, InputCategory category)
	{
		CreateLabel(_language.GetMapCategoryName(category.id), references.inputGridActionColumn, new Vector2(0f, (float)(rowCount * _inputRowHeight) * -1f));
		rowCount++;
	}

	private GUILabel CreateLabel(string labelText, Transform parent, Vector2 offset)
	{
		return CreateLabel(prefabs.inputGridLabel, labelText, parent, offset);
	}

	private GUILabel CreateLabel(GameObject prefab, string labelText, Transform parent, Vector2 offset)
	{
		GameObject instance = InstantiateGUIObject(prefab, parent, offset);
		TMP_Text text = UnityTools.GetComponentInSelfOrChildren<TMP_Text>(instance);
		if (text == null)
		{
			Debug.LogError("Rewired Control Mapper: Label prefab is missing Text component!");
			return null;
		}
		text.text = labelText;
		return new GUILabel(instance);
	}

	private GUIButton CreateButton(string labelText, Transform parent, Vector2 offset)
	{
		GUIButton button = new GUIButton(InstantiateGUIObject(prefabs.button, parent, offset));
		button.SetLabel(labelText);
		return button;
	}

	private GUIButton CreateFitButton(string labelText, Transform parent, Vector2 offset)
	{
		GUIButton button = new GUIButton(InstantiateGUIObject(prefabs.fitButton, parent, offset));
		button.SetLabel(labelText);
		return button;
	}

	private GUIInputField CreateInputField(Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
	{
		GUIInputField field = CreateInputField(parent, offset);
		field.SetLabel("");
		field.SetFieldInfoData(actionId, axisRange, controllerType, fieldIndex);
		field.SetOnClickCallback(inputFieldActivatedDelegate);
		field.fieldInfo.OnSelectedEvent += OnUIElementSelected;
		return field;
	}

	private GUIInputField CreateInputField(Transform parent, Vector2 offset)
	{
		return new GUIInputField(InstantiateGUIObject(prefabs.inputGridFieldButton, parent, offset));
	}

	private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset, string label, int actionId, AxisRange axisRange, ControllerType controllerType, int fieldIndex)
	{
		GUIToggle toggle = CreateToggle(prefab, parent, offset);
		toggle.SetToggleInfoData(actionId, axisRange, controllerType, fieldIndex);
		toggle.SetOnSubmitCallback(inputFieldInvertToggleStateChangedDelegate);
		toggle.toggleInfo.OnSelectedEvent += OnUIElementSelected;
		return toggle;
	}

	private GUIToggle CreateToggle(GameObject prefab, Transform parent, Vector2 offset)
	{
		return new GUIToggle(InstantiateGUIObject(prefab, parent, offset));
	}

	private GameObject InstantiateGUIObject(GameObject prefab, Transform parent, Vector2 offset)
	{
		if (prefab == null)
		{
			Debug.LogError("Rewired Control Mapper: Prefab is null!");
			return null;
		}
		GameObject instance = UnityEngine.Object.Instantiate(prefab);
		return InitializeNewGUIGameObject(instance, parent, offset);
	}

	private GameObject CreateNewGUIObject(string name, Transform parent, Vector2 offset)
	{
		GameObject instance = new GameObject();
		instance.name = name;
		instance.AddComponent<RectTransform>();
		return InitializeNewGUIGameObject(instance, parent, offset);
	}

	private GameObject InitializeNewGUIGameObject(GameObject gameObject, Transform parent, Vector2 offset)
	{
		if (gameObject == null)
		{
			Debug.LogError("Rewired Control Mapper: GameObject is null!");
			return null;
		}
		RectTransform instanceXform = gameObject.GetComponent<RectTransform>();
		if (instanceXform == null)
		{
			Debug.LogError("Rewired Control Mapper: GameObject does not have a RectTransform component!");
			return gameObject;
		}
		if (parent != null)
		{
			instanceXform.SetParent(parent, worldPositionStays: false);
		}
		instanceXform.anchoredPosition = offset;
		return gameObject;
	}

	private GameObject CreateNewColumnGroup(string name, Transform parent, int maxWidth)
	{
		GameObject group = CreateNewGUIObject(name, parent, Vector2.zero);
		inputGrid.AddGroup(group);
		LayoutElement layout = group.AddComponent<LayoutElement>();
		if (maxWidth >= 0)
		{
			layout.preferredWidth = maxWidth;
		}
		RectTransform rectXForm = group.GetComponent<RectTransform>();
		rectXForm.anchorMin = new Vector2(0f, 0f);
		rectXForm.anchorMax = new Vector2(1f, 0f);
		return group;
	}

	private Window OpenWindow(bool closeOthers)
	{
		return OpenWindow(string.Empty, closeOthers);
	}

	private Window OpenWindow(string name, bool closeOthers)
	{
		if (closeOthers)
		{
			windowManager.CancelAll();
		}
		Window window = windowManager.OpenWindow(name, _defaultWindowWidth, _defaultWindowHeight);
		if (window == null)
		{
			return null;
		}
		ChildWindowOpened();
		return window;
	}

	private Window OpenWindow(GameObject windowPrefab, bool closeOthers)
	{
		return OpenWindow(windowPrefab, string.Empty, closeOthers);
	}

	private Window OpenWindow(GameObject windowPrefab, string name, bool closeOthers)
	{
		if (closeOthers)
		{
			windowManager.CancelAll();
		}
		Window window = windowManager.OpenWindow(windowPrefab, name);
		if (window == null)
		{
			return null;
		}
		ChildWindowOpened();
		return window;
	}

	private void OpenModal(string title, string message, string confirmText, Action<int> confirmAction, string cancelText, Action<int> cancelAction, bool closeOthers)
	{
		Window window = OpenWindow(closeOthers);
		if (!(window == null))
		{
			window.CreateTitleText(prefabs.windowTitleText, Vector2.zero, title);
			window.AddContentText(prefabs.windowContentText, UIPivot.TopCenter, UIAnchor.TopHStretch, new Vector2(0f, -100f), message);
			UnityAction cancelCallback = delegate
			{
				OnWindowCancel(window.id);
			};
			window.cancelCallback = cancelCallback;
			window.CreateButton(prefabs.fitButton, UIPivot.BottomLeft, UIAnchor.BottomLeft, Vector2.zero, confirmText, delegate
			{
				OnRestoreDefaultsConfirmed(window.id);
			}, cancelCallback, setDefault: false);
			window.CreateButton(prefabs.fitButton, UIPivot.BottomRight, UIAnchor.BottomRight, Vector2.zero, cancelText, cancelCallback, cancelCallback, setDefault: true);
			windowManager.Focus(window);
		}
	}

	private void CloseWindow(int windowId)
	{
		if (windowManager.isWindowOpen)
		{
			windowManager.CloseWindow(windowId);
			ChildWindowClosed();
		}
	}

	private void CloseTopWindow()
	{
		if (windowManager.isWindowOpen)
		{
			windowManager.CloseTop();
			ChildWindowClosed();
		}
	}

	private void CloseAllWindows()
	{
		if (windowManager.isWindowOpen)
		{
			windowManager.CancelAll();
			ChildWindowClosed();
			InputPollingStopped();
		}
	}

	private void ChildWindowOpened()
	{
		if (windowManager.isWindowOpen)
		{
			SetIsFocused(state: false);
			if (_PopupWindowOpenedEvent != null)
			{
				_PopupWindowOpenedEvent();
			}
			if (_onPopupWindowOpened != null)
			{
				_onPopupWindowOpened.Invoke();
			}
		}
	}

	private void ChildWindowClosed()
	{
		if (!windowManager.isWindowOpen)
		{
			SetIsFocused(state: true);
			if (_PopupWindowClosedEvent != null)
			{
				_PopupWindowClosedEvent();
			}
			if (_onPopupWindowClosed != null)
			{
				_onPopupWindowClosed.Invoke();
			}
		}
	}

	private bool HasElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		if (player == null || mapping == null)
		{
			return false;
		}
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			return false;
		}
		if (skipOtherPlayers)
		{
			if (ReInput.players.SystemPlayer.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck))
			{
				return true;
			}
			if (player.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck))
			{
				return true;
			}
			return false;
		}
		return ReInput.controllers.conflictChecking.DoesElementAssignmentConflict(conflictCheck);
	}

	private bool IsBlockingAssignmentConflict(InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			return false;
		}
		if (skipOtherPlayers)
		{
			foreach (ElementAssignmentConflictInfo item in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				if (!item.isUserAssignable)
				{
					return true;
				}
			}
			foreach (ElementAssignmentConflictInfo item2 in currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				if (!item2.isUserAssignable)
				{
					return true;
				}
			}
		}
		else
		{
			foreach (ElementAssignmentConflictInfo item3 in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				if (!item3.isUserAssignable)
				{
					return true;
				}
			}
		}
		return false;
	}

	private IEnumerable<ElementAssignmentConflictInfo> ElementAssignmentConflicts(Player player, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		if (player == null || mapping == null || !CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			yield break;
		}
		if (skipOtherPlayers)
		{
			foreach (ElementAssignmentConflictInfo conflict3 in ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				if (!conflict3.isUserAssignable)
				{
					yield return conflict3;
				}
			}
			foreach (ElementAssignmentConflictInfo conflict2 in player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
			{
				if (!conflict2.isUserAssignable)
				{
					yield return conflict2;
				}
			}
			yield break;
		}
		foreach (ElementAssignmentConflictInfo conflict in ReInput.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck))
		{
			if (!conflict.isUserAssignable)
			{
				yield return conflict;
			}
		}
	}

	private bool CreateConflictCheck(InputMapping mapping, ElementAssignment assignment, out ElementAssignmentConflictCheck conflictCheck)
	{
		if (mapping == null || currentPlayer == null)
		{
			conflictCheck = default(ElementAssignmentConflictCheck);
			return false;
		}
		conflictCheck = assignment.ToElementAssignmentConflictCheck();
		conflictCheck.playerId = currentPlayer.id;
		conflictCheck.controllerType = mapping.controllerType;
		conflictCheck.controllerId = mapping.controllerId;
		conflictCheck.controllerMapId = mapping.map.id;
		conflictCheck.controllerMapCategoryId = mapping.map.categoryId;
		if (mapping.aem != null)
		{
			conflictCheck.elementMapId = mapping.aem.id;
		}
		return true;
	}

	private void PollKeyboardForAssignment(out ControllerPollingInfo pollingInfo, out bool modifierKeyPressed, out ModifierKeyFlags modifierFlags, out string label)
	{
		pollingInfo = default(ControllerPollingInfo);
		label = string.Empty;
		modifierKeyPressed = false;
		modifierFlags = ModifierKeyFlags.None;
		int modifierPressedCount = 0;
		ControllerPollingInfo nonModifierKeyInfo = default(ControllerPollingInfo);
		ControllerPollingInfo firstModifierKeyInfo = default(ControllerPollingInfo);
		ModifierKeyFlags curModifiers = ModifierKeyFlags.None;
		foreach (ControllerPollingInfo info in ReInput.controllers.Keyboard.PollForAllKeys())
		{
			KeyCode key = info.keyboardKey;
			if (key == KeyCode.AltGr)
			{
				continue;
			}
			if (Keyboard.IsModifierKey(info.keyboardKey))
			{
				if (modifierPressedCount == 0)
				{
					firstModifierKeyInfo = info;
				}
				curModifiers |= Keyboard.KeyCodeToModifierKeyFlags(key);
				modifierPressedCount++;
			}
			else if (nonModifierKeyInfo.keyboardKey == KeyCode.None)
			{
				nonModifierKeyInfo = info;
			}
		}
		if (nonModifierKeyInfo.keyboardKey != 0)
		{
			if (ReInput.controllers.Keyboard.GetKeyDown(nonModifierKeyInfo.keyboardKey))
			{
				if (modifierPressedCount == 0)
				{
					pollingInfo = nonModifierKeyInfo;
					return;
				}
				pollingInfo = nonModifierKeyInfo;
				modifierFlags = curModifiers;
			}
		}
		else
		{
			if (modifierPressedCount <= 0)
			{
				return;
			}
			modifierKeyPressed = true;
			if (modifierPressedCount == 1)
			{
				if (ReInput.controllers.Keyboard.GetKeyTimePressed(firstModifierKeyInfo.keyboardKey) > 1.0)
				{
					pollingInfo = firstModifierKeyInfo;
				}
				else
				{
					label = Keyboard.GetKeyName(firstModifierKeyInfo.keyboardKey);
				}
			}
			else
			{
				label = _language.ModifierKeyFlagsToString(curModifiers);
			}
		}
	}

	private bool GetFirstElementAssignmentConflict(ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict, bool skipOtherPlayers)
	{
		if (GetFirstElementAssignmentConflict(currentPlayer, conflictCheck, out conflict))
		{
			return true;
		}
		if (GetFirstElementAssignmentConflict(ReInput.players.SystemPlayer, conflictCheck, out conflict))
		{
			return true;
		}
		if (!skipOtherPlayers)
		{
			IList<Player> players = ReInput.players.Players;
			for (int i = 0; i < players.Count; i++)
			{
				Player player = players[i];
				if (player != currentPlayer && GetFirstElementAssignmentConflict(player, conflictCheck, out conflict))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool GetFirstElementAssignmentConflict(Player player, ElementAssignmentConflictCheck conflictCheck, out ElementAssignmentConflictInfo conflict)
	{
		using (IEnumerator<ElementAssignmentConflictInfo> enumerator = player.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				ElementAssignmentConflictInfo c = (conflict = enumerator.Current);
				return true;
			}
		}
		conflict = default(ElementAssignmentConflictInfo);
		return false;
	}

	private void StartAxisCalibration(int axisIndex)
	{
		if (currentPlayer != null && currentPlayer.controllers.joystickCount != 0)
		{
			Joystick joystick = currentJoystick;
			if (axisIndex >= 0 && axisIndex < joystick.axisCount)
			{
				pendingAxisCalibration = new AxisCalibrator(joystick, axisIndex);
				ShowCalibrateAxisStep1Window();
			}
		}
	}

	private void EndAxisCalibration()
	{
		if (pendingAxisCalibration != null)
		{
			pendingAxisCalibration.Commit();
			pendingAxisCalibration = null;
		}
	}

	private void SetUISelection(GameObject selection)
	{
		if (!(EventSystem.current == null))
		{
			EventSystem.current.SetSelectedGameObject(selection);
		}
	}

	private void RestoreLastUISelection()
	{
		if (lastUISelection == null || !lastUISelection.activeInHierarchy)
		{
			SetDefaultUISelection();
		}
		else
		{
			SetUISelection(lastUISelection);
		}
	}

	private void SetDefaultUISelection()
	{
		if (isOpen)
		{
			if (references.defaultSelection == null)
			{
				SetUISelection(null);
			}
			else
			{
				SetUISelection(references.defaultSelection.gameObject);
			}
		}
	}

	private void SelectDefaultMapCategory(bool redraw)
	{
		currentMapCategoryId = GetDefaultMapCategoryId();
		OnMapCategorySelected(currentMapCategoryId, redraw);
		if (!showMapCategories)
		{
			return;
		}
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			InputMapCategory cat = ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId);
			if (cat != null)
			{
				currentMapCategoryId = _mappingSets[i].mapCategoryId;
				break;
			}
		}
		if (currentMapCategoryId >= 0)
		{
			for (int j = 0; j < _mappingSets.Length; j++)
			{
				bool state = ((_mappingSets[j].mapCategoryId != currentMapCategoryId) ? true : false);
				mapCategoryButtons[j].SetInteractible(state, playTransition: false);
			}
		}
	}

	private void CheckUISelection()
	{
		if (isFocused && currentUISelection == null)
		{
			RestoreLastUISelection();
		}
	}

	private void OnUIElementSelected(GameObject selectedObject)
	{
		lastUISelection = selectedObject;
	}

	private void SetIsFocused(bool state)
	{
		references.mainCanvasGroup.interactable = state;
		if (state)
		{
			Redraw(listsChanged: false, playTransitions: false);
			RestoreLastUISelection();
			blockInputOnFocusEndTime = Time.unscaledTime + 0.1f;
		}
	}

	public void Toggle()
	{
		if (isOpen)
		{
			Close(save: true);
		}
		else
		{
			Open();
		}
	}

	public void Open()
	{
		Open(force: false);
	}

	private void Open(bool force)
	{
		if (!initialized)
		{
			Initialize();
		}
		if (initialized && (force || !isOpen))
		{
			Clear();
			canvas.SetActive(value: true);
			OnPlayerSelected(0, redraw: false);
			SelectDefaultMapCategory(redraw: false);
			SetDefaultUISelection();
			Redraw(listsChanged: true, playTransitions: false);
			if (_ScreenOpenedEvent != null)
			{
				_ScreenOpenedEvent();
			}
			if (_onScreenOpened != null)
			{
				_onScreenOpened.Invoke();
			}
		}
	}

	public void Close(bool save)
	{
		if (initialized && isOpen)
		{
			if (save && ReInput.userDataStore != null)
			{
				ReInput.userDataStore.Save();
			}
			Clear();
			canvas.SetActive(value: false);
			SetUISelection(null);
			if (_ScreenClosedEvent != null)
			{
				_ScreenClosedEvent();
			}
			if (_onScreenClosed != null)
			{
				_onScreenClosed.Invoke();
			}
		}
	}

	private void Clear()
	{
		windowManager.CancelAll();
		lastUISelection = null;
		pendingInputMapping = null;
		pendingAxisCalibration = null;
		InputPollingStopped();
	}

	private void ClearCompletely()
	{
		Clear();
		ClearSpawnedObjects();
		ClearAllVars();
	}

	private void ClearSpawnedObjects()
	{
		windowManager.ClearCompletely();
		inputGrid.ClearAll();
		foreach (GUIButton item4 in playerButtons)
		{
			UnityEngine.Object.Destroy(item4.gameObject);
		}
		playerButtons.Clear();
		foreach (GUIButton item3 in mapCategoryButtons)
		{
			UnityEngine.Object.Destroy(item3.gameObject);
		}
		mapCategoryButtons.Clear();
		foreach (GUIButton item2 in assignedControllerButtons)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		assignedControllerButtons.Clear();
		if (assignedControllerButtonsPlaceholder != null)
		{
			UnityEngine.Object.Destroy(assignedControllerButtonsPlaceholder.gameObject);
			assignedControllerButtonsPlaceholder = null;
		}
		foreach (GameObject item in miscInstantiatedObjects)
		{
			UnityEngine.Object.Destroy(item);
		}
		miscInstantiatedObjects.Clear();
	}

	private void ClearVarsOnPlayerChange()
	{
		currentJoystickId = -1;
	}

	private void ClearVarsOnJoystickChange()
	{
		currentJoystickId = -1;
	}

	private void ClearAllVars()
	{
		initialized = false;
		Instance = null;
		playerCount = 0;
		inputGrid = null;
		windowManager = null;
		currentPlayerId = -1;
		currentMapCategoryId = -1;
		playerButtons = null;
		mapCategoryButtons = null;
		miscInstantiatedObjects = null;
		canvas = null;
		lastUISelection = null;
		currentJoystickId = -1;
		pendingInputMapping = null;
		pendingAxisCalibration = null;
		inputFieldActivatedDelegate = null;
		inputFieldInvertToggleStateChangedDelegate = null;
		isPollingForInput = false;
	}

	public void Reset()
	{
		if (initialized)
		{
			ClearCompletely();
			Initialize();
			if (isOpen)
			{
				Open(force: true);
			}
		}
	}

	private void SetActionAxisInverted(bool state, ControllerType controllerType, int actionElementMapId)
	{
		if (currentPlayer != null && GetControllerMap(controllerType) is ControllerMapWithAxes map)
		{
			ActionElementMap aem = map.GetElementMap(actionElementMapId);
			if (aem != null)
			{
				aem.invert = state;
			}
		}
	}

	private ControllerMap GetControllerMap(ControllerType type)
	{
		if (currentPlayer == null)
		{
			return null;
		}
		int controllerId = 0;
		switch (type)
		{
		case ControllerType.Joystick:
			if (currentPlayer.controllers.joystickCount > 0)
			{
				controllerId = currentJoystick.id;
				break;
			}
			return null;
		default:
			throw new NotImplementedException();
		case ControllerType.Keyboard:
		case ControllerType.Mouse:
			break;
		}
		return currentPlayer.controllers.maps.GetFirstMapInCategory(type, controllerId, currentMapCategoryId);
	}

	private ControllerMap GetControllerMapOrCreateNew(ControllerType controllerType, int controllerId, int layoutId)
	{
		ControllerMap map = GetControllerMap(controllerType);
		if (map == null)
		{
			currentPlayer.controllers.maps.AddEmptyMap(controllerType, controllerId, currentMapCategoryId, layoutId);
			map = currentPlayer.controllers.maps.GetMap(controllerType, controllerId, currentMapCategoryId, layoutId);
		}
		return map;
	}

	private int CountIEnumerable<T>(IEnumerable<T> enumerable)
	{
		if (enumerable == null)
		{
			return 0;
		}
		IEnumerator<T> enumerator = enumerable.GetEnumerator();
		if (enumerator == null)
		{
			return 0;
		}
		int count = 0;
		while (enumerator.MoveNext())
		{
			count++;
		}
		return count;
	}

	private int GetDefaultMapCategoryId()
	{
		if (_mappingSets.Length == 0)
		{
			return 0;
		}
		for (int i = 0; i < _mappingSets.Length; i++)
		{
			if (ReInput.mapping.GetMapCategory(_mappingSets[i].mapCategoryId) != null)
			{
				return _mappingSets[i].mapCategoryId;
			}
		}
		return 0;
	}

	private void SubscribeFixedUISelectionEvents()
	{
		if (references.fixedSelectableUIElements == null)
		{
			return;
		}
		GameObject[] fixedSelectableUIElements = references.fixedSelectableUIElements;
		foreach (GameObject go in fixedSelectableUIElements)
		{
			UIElementInfo elementInfo = UnityTools.GetComponent<UIElementInfo>(go);
			if (!(elementInfo == null))
			{
				elementInfo.OnSelectedEvent += OnUIElementSelected;
			}
		}
	}

	private void SubscribeMenuControlInputEvents()
	{
		SubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);
		SubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);
		SubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);
		SubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
	}

	private void UnsubscribeMenuControlInputEvents()
	{
		UnsubscribeRewiredInputEventAllPlayers(_screenToggleAction, OnScreenToggleActionPressed);
		UnsubscribeRewiredInputEventAllPlayers(_screenOpenAction, OnScreenOpenActionPressed);
		UnsubscribeRewiredInputEventAllPlayers(_screenCloseAction, OnScreenCloseActionPressed);
		UnsubscribeRewiredInputEventAllPlayers(_universalCancelAction, OnUniversalCancelActionPressed);
	}

	private void SubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
	{
		if (actionId < 0 || callback == null)
		{
			return;
		}
		if (ReInput.mapping.GetAction(actionId) == null)
		{
			Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
			return;
		}
		foreach (Player p in ReInput.players.AllPlayers)
		{
			p.AddInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
		}
	}

	private void UnsubscribeRewiredInputEventAllPlayers(int actionId, Action<InputActionEventData> callback)
	{
		if (actionId < 0 || callback == null || !ReInput.isReady)
		{
			return;
		}
		if (ReInput.mapping.GetAction(actionId) == null)
		{
			Debug.LogWarning("Rewired Control Mapper: " + actionId + " is not a valid Action id!");
			return;
		}
		foreach (Player p in ReInput.players.AllPlayers)
		{
			p.RemoveInputEventDelegate(callback, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, actionId);
		}
	}

	private int GetMaxControllersPerPlayer()
	{
		if (_rewiredInputManager.userData.ConfigVars.autoAssignJoysticks)
		{
			return _rewiredInputManager.userData.ConfigVars.maxJoysticksPerPlayer;
		}
		return _maxControllersPerPlayer;
	}

	private bool ShowAssignedControllers()
	{
		if (!_showControllers)
		{
			return false;
		}
		if (_showAssignedControllers)
		{
			return true;
		}
		if (GetMaxControllersPerPlayer() != 1)
		{
			return true;
		}
		return false;
	}

	private void InspectorPropertyChanged(bool reset = false)
	{
		if (reset)
		{
			Reset();
		}
	}

	private void AssignController(Player player, int controllerId)
	{
		if (player == null || player.controllers.ContainsController(ControllerType.Joystick, controllerId))
		{
			return;
		}
		if (GetMaxControllersPerPlayer() == 1)
		{
			RemoveAllControllers(player);
			ClearVarsOnJoystickChange();
		}
		foreach (Player p in ReInput.players.Players)
		{
			if (p != player)
			{
				RemoveController(p, controllerId);
			}
		}
		player.controllers.AddController(ControllerType.Joystick, controllerId, removeFromOtherPlayers: false);
		if (ReInput.userDataStore != null)
		{
			ReInput.userDataStore.LoadControllerData(player.id, ControllerType.Joystick, controllerId);
		}
	}

	private void RemoveAllControllers(Player player)
	{
		if (player != null)
		{
			IList<Joystick> joysticks = player.controllers.Joysticks;
			for (int i = joysticks.Count - 1; i >= 0; i--)
			{
				RemoveController(player, joysticks[i].id);
			}
		}
	}

	private void RemoveController(Player player, int controllerId)
	{
		if (player != null && player.controllers.ContainsController(ControllerType.Joystick, controllerId))
		{
			if (ReInput.userDataStore != null)
			{
				ReInput.userDataStore.SaveControllerData(player.id, ControllerType.Joystick, controllerId);
			}
			player.controllers.RemoveController(ControllerType.Joystick, controllerId);
		}
	}

	private bool IsAllowedAssignment(InputMapping pendingInputMapping, ControllerPollingInfo pollingInfo)
	{
		if (pendingInputMapping == null)
		{
			return false;
		}
		if (pendingInputMapping.axisRange == AxisRange.Full && !_showSplitAxisInputFields && pollingInfo.elementType == ControllerElementType.Button)
		{
			return false;
		}
		return true;
	}

	private void InputPollingStarted()
	{
		bool prev = isPollingForInput;
		isPollingForInput = true;
		if (!prev)
		{
			if (_InputPollingStartedEvent != null)
			{
				_InputPollingStartedEvent();
			}
			if (_onInputPollingStarted != null)
			{
				_onInputPollingStarted.Invoke();
			}
		}
	}

	private void InputPollingStopped()
	{
		bool prev = isPollingForInput;
		isPollingForInput = false;
		if (prev)
		{
			if (_InputPollingEndedEvent != null)
			{
				_InputPollingEndedEvent();
			}
			if (_onInputPollingEnded != null)
			{
				_onInputPollingEnded.Invoke();
			}
		}
	}

	private int GetControllerInputFieldCount(ControllerType controllerType)
	{
		return controllerType switch
		{
			ControllerType.Keyboard => _keyboardInputFieldCount, 
			ControllerType.Mouse => _mouseInputFieldCount, 
			ControllerType.Joystick => _controllerInputFieldCount, 
			_ => throw new NotImplementedException(), 
		};
	}

	private bool ShowSwapButton(int windowId, InputMapping mapping, ElementAssignment assignment, bool skipOtherPlayers)
	{
		if (currentPlayer == null)
		{
			return false;
		}
		if (!_allowElementAssignmentSwap)
		{
			return false;
		}
		if (mapping == null || mapping.aem == null)
		{
			return false;
		}
		if (!CreateConflictCheck(mapping, assignment, out var conflictCheck))
		{
			Debug.LogError("Rewired Control Mapper: Error creating conflict check!");
			return false;
		}
		List<ElementAssignmentConflictInfo> conflicts = new List<ElementAssignmentConflictInfo>();
		conflicts.AddRange(currentPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck));
		conflicts.AddRange(ReInput.players.SystemPlayer.controllers.conflictChecking.ElementAssignmentConflicts(conflictCheck));
		if (conflicts.Count == 0)
		{
			return false;
		}
		ActionElementMap origAemToReplace = mapping.aem;
		ElementAssignmentConflictInfo firstConflict = conflicts[0];
		int swapActionId = firstConflict.elementMap.actionId;
		Pole swapAxisContribution = firstConflict.elementMap.axisContribution;
		AxisRange swapAxisRange = origAemToReplace.axisRange;
		ControllerElementType swapElementType = origAemToReplace.elementType;
		if (swapElementType == firstConflict.elementMap.elementType && swapElementType == ControllerElementType.Axis)
		{
			if (swapAxisRange != firstConflict.elementMap.axisRange)
			{
				if (swapAxisRange == AxisRange.Full)
				{
					swapAxisRange = AxisRange.Positive;
				}
				else if (firstConflict.elementMap.axisRange != 0)
				{
				}
			}
		}
		else if (swapElementType == ControllerElementType.Axis && (firstConflict.elementMap.elementType == ControllerElementType.Button || (firstConflict.elementMap.elementType == ControllerElementType.Axis && firstConflict.elementMap.axisRange != 0)) && swapAxisRange == AxisRange.Full)
		{
			swapAxisRange = AxisRange.Positive;
		}
		int usedFieldCount = 0;
		if (assignment.actionId == firstConflict.actionId && mapping.map == firstConflict.controllerMap)
		{
			Controller controller = ReInput.controllers.GetController(mapping.controllerType, mapping.controllerId);
			if (SwapIsSameInputRange(swapElementType, swapAxisRange, swapAxisContribution, controller.GetElementById(assignment.elementIdentifierId).type, assignment.axisRange, assignment.axisContribution))
			{
				usedFieldCount++;
			}
		}
		foreach (ActionElementMap aem in firstConflict.controllerMap.ElementMapsWithAction(swapActionId))
		{
			if (aem.id != origAemToReplace.id && conflicts.FindIndex((ElementAssignmentConflictInfo x) => x.elementMapId == aem.id) < 0 && SwapIsSameInputRange(swapElementType, swapAxisRange, swapAxisContribution, aem.elementType, aem.axisRange, aem.axisContribution))
			{
				usedFieldCount++;
			}
		}
		return usedFieldCount < GetControllerInputFieldCount(mapping.controllerType);
	}

	private bool SwapIsSameInputRange(ControllerElementType origElementType, AxisRange origAxisRange, Pole origAxisContribution, ControllerElementType conflictElementType, AxisRange conflictAxisRange, Pole conflictAxisContribution)
	{
		if ((origElementType == ControllerElementType.Button || (origElementType == ControllerElementType.Axis && origAxisRange != 0)) && (conflictElementType == ControllerElementType.Button || (conflictElementType == ControllerElementType.Axis && conflictAxisRange != 0)) && conflictAxisContribution == origAxisContribution)
		{
			return true;
		}
		if (origElementType == ControllerElementType.Axis && origAxisRange == AxisRange.Full && conflictElementType == ControllerElementType.Axis && conflictAxisRange == AxisRange.Full)
		{
			return true;
		}
		return false;
	}

	public static void ApplyTheme(ThemedElement.ElementInfo[] elementInfo)
	{
		if (!(Instance == null) && !(Instance._themeSettings == null) && Instance._useThemeSettings)
		{
			Instance._themeSettings.Apply(elementInfo);
		}
	}

	public static LanguageDataBase GetLanguage()
	{
		if (Instance == null)
		{
			return null;
		}
		return Instance._language;
	}
}
