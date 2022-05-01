using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
public class InputBehaviorWindow : Window
{
	private class InputBehaviorInfo
	{
		private InputBehavior _inputBehavior;

		private UIControlSet _controlSet;

		private Dictionary<int, PropertyType> idToProperty;

		private InputBehavior copyOfOriginal;

		public InputBehavior inputBehavior => _inputBehavior;

		public UIControlSet controlSet => _controlSet;

		public InputBehaviorInfo(InputBehavior inputBehavior, UIControlSet controlSet, Dictionary<int, PropertyType> idToProperty)
		{
			_inputBehavior = inputBehavior;
			_controlSet = controlSet;
			this.idToProperty = idToProperty;
			copyOfOriginal = new InputBehavior(inputBehavior);
		}

		public void RestorePreviousData()
		{
			_inputBehavior.ImportData(copyOfOriginal);
		}

		public void RestoreDefaultData()
		{
			_inputBehavior.Reset();
			RefreshControls();
		}

		public void RestoreData(PropertyType propertyType, int controlId)
		{
			switch (propertyType)
			{
			case PropertyType.JoystickAxisSensitivity:
			{
				float value = copyOfOriginal.joystickAxisSensitivity;
				_inputBehavior.joystickAxisSensitivity = value;
				UISliderControl control = _controlSet.GetControl<UISliderControl>(controlId);
				if (control != null)
				{
					control.slider.value = value;
				}
				break;
			}
			case PropertyType.MouseXYAxisSensitivity:
			{
				float value2 = copyOfOriginal.mouseXYAxisSensitivity;
				_inputBehavior.mouseXYAxisSensitivity = value2;
				UISliderControl control2 = _controlSet.GetControl<UISliderControl>(controlId);
				if (control2 != null)
				{
					control2.slider.value = value2;
				}
				break;
			}
			}
		}

		public void RefreshControls()
		{
			if (_controlSet == null || idToProperty == null)
			{
				return;
			}
			foreach (KeyValuePair<int, PropertyType> pair in idToProperty)
			{
				UISliderControl control = _controlSet.GetControl<UISliderControl>(pair.Key);
				if (!(control == null))
				{
					switch (pair.Value)
					{
					case PropertyType.JoystickAxisSensitivity:
						control.slider.value = _inputBehavior.joystickAxisSensitivity;
						break;
					case PropertyType.MouseXYAxisSensitivity:
						control.slider.value = _inputBehavior.mouseXYAxisSensitivity;
						break;
					}
				}
			}
		}
	}

	public enum ButtonIdentifier
	{
		Done,
		Cancel,
		Default
	}

	private enum PropertyType
	{
		JoystickAxisSensitivity,
		MouseXYAxisSensitivity
	}

	private const float minSensitivity = 0.1f;

	[SerializeField]
	private RectTransform spawnTransform;

	[SerializeField]
	private Button doneButton;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	private Button defaultButton;

	[SerializeField]
	private TMP_Text doneButtonLabel;

	[SerializeField]
	private TMP_Text cancelButtonLabel;

	[SerializeField]
	private TMP_Text defaultButtonLabel;

	[SerializeField]
	private GameObject uiControlSetPrefab;

	[SerializeField]
	private GameObject uiSliderControlPrefab;

	private List<InputBehaviorInfo> inputBehaviorInfo;

	private Dictionary<int, Action<int>> buttonCallbacks;

	private int playerId;

	public override void Initialize(int id, Func<int, bool> isFocusedCallback)
	{
		if (spawnTransform == null || doneButton == null || cancelButton == null || defaultButton == null || uiControlSetPrefab == null || uiSliderControlPrefab == null || doneButtonLabel == null || cancelButtonLabel == null || defaultButtonLabel == null)
		{
			Debug.LogError("Rewired Control Mapper: All inspector values must be assigned!");
			return;
		}
		inputBehaviorInfo = new List<InputBehaviorInfo>();
		buttonCallbacks = new Dictionary<int, Action<int>>();
		doneButtonLabel.text = ControlMapper.GetLanguage().done;
		cancelButtonLabel.text = ControlMapper.GetLanguage().cancel;
		defaultButtonLabel.text = ControlMapper.GetLanguage().default_;
		base.Initialize(id, isFocusedCallback);
	}

	public void SetData(int playerId, ControlMapper.InputBehaviorSettings[] data)
	{
		if (!base.initialized)
		{
			return;
		}
		this.playerId = playerId;
		foreach (ControlMapper.InputBehaviorSettings item in data)
		{
			if (item == null || !item.isValid)
			{
				continue;
			}
			InputBehavior inputBehavior = GetInputBehavior(item.inputBehaviorId);
			if (inputBehavior != null)
			{
				UIControlSet set = CreateControlSet();
				Dictionary<int, PropertyType> idToProperty = new Dictionary<int, PropertyType>();
				string customTitle = ControlMapper.GetLanguage().GetCustomEntry(item.labelLanguageKey);
				if (!string.IsNullOrEmpty(customTitle))
				{
					set.SetTitle(customTitle);
				}
				else
				{
					set.SetTitle(inputBehavior.name);
				}
				if (item.showJoystickAxisSensitivity)
				{
					UISliderControl slider2 = CreateSlider(set, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(item.joystickAxisSensitivityLabelLanguageKey), item.joystickAxisSensitivityIcon, item.joystickAxisSensitivityMin, item.joystickAxisSensitivityMax, JoystickAxisSensitivityValueChanged, JoystickAxisSensitivityCanceled);
					slider2.slider.value = Mathf.Clamp(inputBehavior.joystickAxisSensitivity, item.joystickAxisSensitivityMin, item.joystickAxisSensitivityMax);
					idToProperty.Add(slider2.id, PropertyType.JoystickAxisSensitivity);
				}
				if (item.showMouseXYAxisSensitivity)
				{
					UISliderControl slider = CreateSlider(set, inputBehavior.id, null, ControlMapper.GetLanguage().GetCustomEntry(item.mouseXYAxisSensitivityLabelLanguageKey), item.mouseXYAxisSensitivityIcon, item.mouseXYAxisSensitivityMin, item.mouseXYAxisSensitivityMax, MouseXYAxisSensitivityValueChanged, MouseXYAxisSensitivityCanceled);
					slider.slider.value = Mathf.Clamp(inputBehavior.mouseXYAxisSensitivity, item.mouseXYAxisSensitivityMin, item.mouseXYAxisSensitivityMax);
					idToProperty.Add(slider.id, PropertyType.MouseXYAxisSensitivity);
				}
				inputBehaviorInfo.Add(new InputBehaviorInfo(inputBehavior, set, idToProperty));
			}
		}
		base.defaultUIElement = doneButton.gameObject;
	}

	public void SetButtonCallback(ButtonIdentifier buttonIdentifier, Action<int> callback)
	{
		if (base.initialized && callback != null)
		{
			if (buttonCallbacks.ContainsKey((int)buttonIdentifier))
			{
				buttonCallbacks[(int)buttonIdentifier] = callback;
			}
			else
			{
				buttonCallbacks.Add((int)buttonIdentifier, callback);
			}
		}
	}

	public override void Cancel()
	{
		if (!base.initialized)
		{
			return;
		}
		foreach (InputBehaviorInfo info in inputBehaviorInfo)
		{
			info.RestorePreviousData();
		}
		if (!buttonCallbacks.TryGetValue(1, out var callback))
		{
			if (cancelCallback != null)
			{
				cancelCallback();
			}
		}
		else
		{
			callback(base.id);
		}
	}

	public void OnDone()
	{
		if (base.initialized && buttonCallbacks.TryGetValue(0, out var callback))
		{
			callback(base.id);
		}
	}

	public void OnCancel()
	{
		Cancel();
	}

	public void OnRestoreDefault()
	{
		if (!base.initialized)
		{
			return;
		}
		foreach (InputBehaviorInfo info in inputBehaviorInfo)
		{
			info.RestoreDefaultData();
		}
	}

	private void JoystickAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
	{
		GetInputBehavior(inputBehaviorId).joystickAxisSensitivity = value;
	}

	private void MouseXYAxisSensitivityValueChanged(int inputBehaviorId, int controlId, float value)
	{
		GetInputBehavior(inputBehaviorId).mouseXYAxisSensitivity = value;
	}

	private void JoystickAxisSensitivityCanceled(int inputBehaviorId, int controlId)
	{
		GetInputBehaviorInfo(inputBehaviorId)?.RestoreData(PropertyType.JoystickAxisSensitivity, controlId);
	}

	private void MouseXYAxisSensitivityCanceled(int inputBehaviorId, int controlId)
	{
		GetInputBehaviorInfo(inputBehaviorId)?.RestoreData(PropertyType.MouseXYAxisSensitivity, controlId);
	}

	public override void TakeInputFocus()
	{
		base.TakeInputFocus();
	}

	private UIControlSet CreateControlSet()
	{
		GameObject instance = UnityEngine.Object.Instantiate(uiControlSetPrefab);
		instance.transform.SetParent(spawnTransform, worldPositionStays: false);
		return instance.GetComponent<UIControlSet>();
	}

	private UISliderControl CreateSlider(UIControlSet set, int inputBehaviorId, string defaultTitle, string overrideTitle, Sprite icon, float minValue, float maxValue, Action<int, int, float> valueChangedCallback, Action<int, int> cancelCallback)
	{
		UISliderControl control = set.CreateSlider(uiSliderControlPrefab, icon, minValue, maxValue, delegate(int cId, float value)
		{
			valueChangedCallback(inputBehaviorId, cId, value);
		}, delegate(int cId)
		{
			cancelCallback(inputBehaviorId, cId);
		});
		string title = (string.IsNullOrEmpty(overrideTitle) ? defaultTitle : overrideTitle);
		if (!string.IsNullOrEmpty(title))
		{
			control.showTitle = true;
			control.title.text = title;
		}
		else
		{
			control.showTitle = false;
		}
		control.showIcon = icon != null;
		return control;
	}

	private InputBehavior GetInputBehavior(int id)
	{
		return ReInput.mapping.GetInputBehavior(playerId, id);
	}

	private InputBehaviorInfo GetInputBehaviorInfo(int inputBehaviorId)
	{
		int count = inputBehaviorInfo.Count;
		for (int i = 0; i < count; i++)
		{
			if (inputBehaviorInfo[i].inputBehavior.id == inputBehaviorId)
			{
				return inputBehaviorInfo[i];
			}
		}
		return null;
	}
}
