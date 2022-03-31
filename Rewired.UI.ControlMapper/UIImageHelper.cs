using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
[RequireComponent(typeof(Image))]
public class UIImageHelper : MonoBehaviour
{
	[Serializable]
	private class State
	{
		[SerializeField]
		public Color color;

		public void Set(Image image)
		{
			if (!(image == null))
			{
				image.color = color;
			}
		}
	}

	[SerializeField]
	private State enabledState;

	[SerializeField]
	private State disabledState;

	private bool currentState;

	public void SetEnabledState(bool newState)
	{
		currentState = newState;
		State state = (newState ? enabledState : disabledState);
		if (state != null)
		{
			Image image = base.gameObject.GetComponent<Image>();
			if (image == null)
			{
				Debug.LogError("Image is missing!");
			}
			else
			{
				state.Set(image);
			}
		}
	}

	public void SetEnabledStateColor(Color color)
	{
		enabledState.color = color;
	}

	public void SetDisabledStateColor(Color color)
	{
		disabledState.color = color;
	}

	public void Refresh()
	{
		State state = (currentState ? enabledState : disabledState);
		Image image = base.gameObject.GetComponent<Image>();
		if (!(image == null))
		{
			state.Set(image);
		}
	}
}
