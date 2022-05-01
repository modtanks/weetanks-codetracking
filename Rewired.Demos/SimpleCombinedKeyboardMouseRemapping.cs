using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.Demos;

[AddComponentMenu("")]
public class SimpleCombinedKeyboardMouseRemapping : MonoBehaviour
{
	private class Row
	{
		public InputAction action;

		public AxisRange actionRange;

		public Button button;

		public Text text;
	}

	private struct TargetMapping
	{
		public ControllerMap controllerMap;

		public int actionElementMapId;
	}

	private const string category = "Default";

	private const string layout = "Default";

	private const string uiCategory = "UI";

	private InputMapper inputMapper_keyboard = new InputMapper();

	private InputMapper inputMapper_mouse = new InputMapper();

	public GameObject buttonPrefab;

	public GameObject textPrefab;

	public RectTransform fieldGroupTransform;

	public RectTransform actionGroupTransform;

	public Text controllerNameUIText;

	public Text statusUIText;

	private List<Row> rows = new List<Row>();

	private TargetMapping _replaceTargetMapping;

	private Player player => ReInput.players.GetPlayer(0);

	private void OnEnable()
	{
		if (ReInput.isReady)
		{
			inputMapper_keyboard.options.timeout = 5f;
			inputMapper_mouse.options.timeout = 5f;
			inputMapper_mouse.options.ignoreMouseXAxis = true;
			inputMapper_mouse.options.ignoreMouseYAxis = true;
			inputMapper_keyboard.options.allowButtonsOnFullAxisAssignment = false;
			inputMapper_mouse.options.allowButtonsOnFullAxisAssignment = false;
			inputMapper_keyboard.InputMappedEvent += OnInputMapped;
			inputMapper_keyboard.StoppedEvent += OnStopped;
			inputMapper_mouse.InputMappedEvent += OnInputMapped;
			inputMapper_mouse.StoppedEvent += OnStopped;
			InitializeUI();
		}
	}

	private void OnDisable()
	{
		inputMapper_keyboard.Stop();
		inputMapper_mouse.Stop();
		inputMapper_keyboard.RemoveAllEventListeners();
		inputMapper_mouse.RemoveAllEventListeners();
	}

	private void RedrawUI()
	{
		controllerNameUIText.text = "Keyboard/Mouse";
		for (int i = 0; i < rows.Count; i++)
		{
			Row row = rows[i];
			InputAction action = rows[i].action;
			string name = string.Empty;
			int actionElementMapId = -1;
			for (int j = 0; j < 2; j++)
			{
				ControllerType controllerType = ((j != 0) ? ControllerType.Mouse : ControllerType.Keyboard);
				ControllerMap controllerMap = player.controllers.maps.GetMap(controllerType, 0, "Default", "Default");
				foreach (ActionElementMap actionElementMap in controllerMap.ElementMapsWithAction(action.id))
				{
					if (actionElementMap.ShowInField(row.actionRange))
					{
						name = actionElementMap.elementIdentifierName;
						actionElementMapId = actionElementMap.id;
						break;
					}
				}
				if (actionElementMapId >= 0)
				{
					break;
				}
			}
			row.text.text = name;
			row.button.onClick.RemoveAllListeners();
			int index = i;
			row.button.onClick.AddListener(delegate
			{
				OnInputFieldClicked(index, actionElementMapId);
			});
		}
	}

	private void ClearUI()
	{
		controllerNameUIText.text = string.Empty;
		for (int i = 0; i < rows.Count; i++)
		{
			rows[i].text.text = string.Empty;
		}
	}

	private void InitializeUI()
	{
		foreach (Transform t in actionGroupTransform)
		{
			Object.Destroy(t.gameObject);
		}
		foreach (Transform t2 in fieldGroupTransform)
		{
			Object.Destroy(t2.gameObject);
		}
		foreach (InputAction action in ReInput.mapping.ActionsInCategory("Default"))
		{
			if (action.type == InputActionType.Axis)
			{
				CreateUIRow(action, AxisRange.Full, action.descriptiveName);
				CreateUIRow(action, AxisRange.Positive, (!string.IsNullOrEmpty(action.positiveDescriptiveName)) ? action.positiveDescriptiveName : (action.descriptiveName + " +"));
				CreateUIRow(action, AxisRange.Negative, (!string.IsNullOrEmpty(action.negativeDescriptiveName)) ? action.negativeDescriptiveName : (action.descriptiveName + " -"));
			}
			else if (action.type == InputActionType.Button)
			{
				CreateUIRow(action, AxisRange.Positive, action.descriptiveName);
			}
		}
		RedrawUI();
	}

	private void CreateUIRow(InputAction action, AxisRange actionRange, string label)
	{
		GameObject labelGo = Object.Instantiate(textPrefab);
		labelGo.transform.SetParent(actionGroupTransform);
		labelGo.transform.SetAsLastSibling();
		labelGo.GetComponent<Text>().text = label;
		GameObject buttonGo = Object.Instantiate(buttonPrefab);
		buttonGo.transform.SetParent(fieldGroupTransform);
		buttonGo.transform.SetAsLastSibling();
		rows.Add(new Row
		{
			action = action,
			actionRange = actionRange,
			button = buttonGo.GetComponent<Button>(),
			text = buttonGo.GetComponentInChildren<Text>()
		});
	}

	private void OnInputFieldClicked(int index, int actionElementMapToReplaceId)
	{
		if (index >= 0 && index < rows.Count)
		{
			ControllerMap keyboardMap = player.controllers.maps.GetMap(ControllerType.Keyboard, 0, "Default", "Default");
			ControllerMap mouseMap = player.controllers.maps.GetMap(ControllerType.Mouse, 0, "Default", "Default");
			ControllerMap controllerMapWithReplacement = (keyboardMap.ContainsElementMap(actionElementMapToReplaceId) ? keyboardMap : ((!mouseMap.ContainsElementMap(actionElementMapToReplaceId)) ? null : mouseMap));
			_replaceTargetMapping = new TargetMapping
			{
				actionElementMapId = actionElementMapToReplaceId,
				controllerMap = controllerMapWithReplacement
			};
			StartCoroutine(StartListeningDelayed(index, keyboardMap, mouseMap, actionElementMapToReplaceId));
		}
	}

	private IEnumerator StartListeningDelayed(int index, ControllerMap keyboardMap, ControllerMap mouseMap, int actionElementMapToReplaceId)
	{
		yield return new WaitForSeconds(0.1f);
		inputMapper_keyboard.Start(new InputMapper.Context
		{
			actionId = rows[index].action.id,
			controllerMap = keyboardMap,
			actionRange = rows[index].actionRange,
			actionElementMapToReplace = keyboardMap.GetElementMap(actionElementMapToReplaceId)
		});
		inputMapper_mouse.Start(new InputMapper.Context
		{
			actionId = rows[index].action.id,
			controllerMap = mouseMap,
			actionRange = rows[index].actionRange,
			actionElementMapToReplace = mouseMap.GetElementMap(actionElementMapToReplaceId)
		});
		player.controllers.maps.SetMapsEnabled(state: false, "UI");
		statusUIText.text = "Listening...";
	}

	private void OnInputMapped(InputMapper.InputMappedEventData data)
	{
		inputMapper_keyboard.Stop();
		inputMapper_mouse.Stop();
		if (_replaceTargetMapping.controllerMap != null && data.actionElementMap.controllerMap != _replaceTargetMapping.controllerMap)
		{
			_replaceTargetMapping.controllerMap.DeleteElementMap(_replaceTargetMapping.actionElementMapId);
		}
		RedrawUI();
	}

	private void OnStopped(InputMapper.StoppedEventData data)
	{
		statusUIText.text = string.Empty;
		player.controllers.maps.SetMapsEnabled(state: true, "UI");
	}
}
