using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
public class UIControlSet : MonoBehaviour
{
	[SerializeField]
	private TMP_Text title;

	private Dictionary<int, UIControl> _controls;

	private Dictionary<int, UIControl> controls => _controls ?? (_controls = new Dictionary<int, UIControl>());

	public void SetTitle(string text)
	{
		if (!(title == null))
		{
			title.text = text;
		}
	}

	public T GetControl<T>(int uniqueId) where T : UIControl
	{
		controls.TryGetValue(uniqueId, out var control);
		return control as T;
	}

	public UISliderControl CreateSlider(GameObject prefab, Sprite icon, float minValue, float maxValue, Action<int, float> valueChangedCallback, Action<int> cancelCallback)
	{
		GameObject instance = UnityEngine.Object.Instantiate(prefab);
		UISliderControl control = instance.GetComponent<UISliderControl>();
		if (control == null)
		{
			UnityEngine.Object.Destroy(instance);
			Debug.LogError("Prefab missing UISliderControl component!");
			return null;
		}
		instance.transform.SetParent(base.transform, worldPositionStays: false);
		if (control.iconImage != null)
		{
			control.iconImage.sprite = icon;
		}
		if (control.slider != null)
		{
			control.slider.minValue = minValue;
			control.slider.maxValue = maxValue;
			if (valueChangedCallback != null)
			{
				control.slider.onValueChanged.AddListener(delegate(float value)
				{
					valueChangedCallback(control.id, value);
				});
			}
			if (cancelCallback != null)
			{
				control.SetCancelCallback(delegate
				{
					cancelCallback(control.id);
				});
			}
		}
		controls.Add(control.id, control);
		return control;
	}
}
