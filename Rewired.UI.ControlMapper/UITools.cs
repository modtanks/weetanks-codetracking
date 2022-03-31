using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

public static class UITools
{
	public static GameObject InstantiateGUIObject<T>(GameObject prefab, Transform parent, string name) where T : Component
	{
		GameObject instance = InstantiateGUIObject_Pre<T>(prefab, parent, name);
		if (instance == null)
		{
			return null;
		}
		RectTransform rt = instance.GetComponent<RectTransform>();
		if (rt == null)
		{
			Debug.LogError(name + " prefab is missing RectTransform component!");
		}
		else
		{
			rt.localScale = Vector3.one;
		}
		return instance;
	}

	public static GameObject InstantiateGUIObject<T>(GameObject prefab, Transform parent, string name, Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition) where T : Component
	{
		GameObject instance = InstantiateGUIObject_Pre<T>(prefab, parent, name);
		if (instance == null)
		{
			return null;
		}
		RectTransform rt = instance.GetComponent<RectTransform>();
		if (rt == null)
		{
			Debug.LogError(name + " prefab is missing RectTransform component!");
		}
		else
		{
			rt.localScale = Vector3.one;
			rt.pivot = pivot;
			rt.anchorMin = anchorMin;
			rt.anchorMax = anchorMax;
			rt.anchoredPosition = anchoredPosition;
		}
		return instance;
	}

	private static GameObject InstantiateGUIObject_Pre<T>(GameObject prefab, Transform parent, string name) where T : Component
	{
		if (prefab == null)
		{
			Debug.LogError(name + " prefab is null!");
			return null;
		}
		GameObject instance = Object.Instantiate(prefab);
		if (!string.IsNullOrEmpty(name))
		{
			instance.name = name;
		}
		T comp = instance.GetComponent<T>();
		if ((Object)comp == (Object)null)
		{
			Debug.LogError(name + " prefab is missing the " + comp.GetType().ToString() + " component!");
			return null;
		}
		if (parent != null)
		{
			instance.transform.SetParent(parent, worldPositionStays: false);
		}
		return instance;
	}

	public static Vector3 GetPointOnRectEdge(RectTransform rectTransform, Vector2 dir)
	{
		if (rectTransform == null)
		{
			return Vector3.zero;
		}
		if (dir != Vector2.zero)
		{
			dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
		}
		Rect rect = rectTransform.rect;
		dir = rect.center + Vector2.Scale(rect.size, dir * 0.5f);
		return dir;
	}

	public static Rect GetWorldSpaceRect(RectTransform rt)
	{
		if (rt == null)
		{
			return default(Rect);
		}
		Rect rect = rt.rect;
		Vector2 tl = rt.TransformPoint(new Vector2(rect.xMin, rect.yMin));
		Vector2 bl = rt.TransformPoint(new Vector2(rect.xMin, rect.yMax));
		return new Rect(width: ((Vector2)rt.TransformPoint(new Vector2(rect.xMax, rect.yMin))).x - tl.x, x: tl.x, y: tl.y, height: bl.y - tl.y);
	}

	public static Rect TransformRectTo(Transform from, Transform to, Rect rect)
	{
		Vector3 topLeft;
		Vector3 bottomLeft;
		Vector3 topRight;
		if (from != null)
		{
			topLeft = from.TransformPoint(new Vector2(rect.xMin, rect.yMin));
			bottomLeft = from.TransformPoint(new Vector2(rect.xMin, rect.yMax));
			topRight = from.TransformPoint(new Vector2(rect.xMax, rect.yMin));
		}
		else
		{
			topLeft = new Vector2(rect.xMin, rect.yMin);
			bottomLeft = new Vector2(rect.xMin, rect.yMax);
			topRight = new Vector2(rect.xMax, rect.yMin);
		}
		if (to != null)
		{
			topLeft = to.InverseTransformPoint(topLeft);
			bottomLeft = to.InverseTransformPoint(bottomLeft);
			topRight = to.InverseTransformPoint(topRight);
		}
		return new Rect(topLeft.x, topLeft.y, topRight.x - topLeft.x, topLeft.y - bottomLeft.y);
	}

	public static Rect InvertY(Rect rect)
	{
		return new Rect(rect.xMin, rect.yMin, rect.width, 0f - rect.height);
	}

	public static void SetInteractable(Selectable selectable, bool state, bool playTransition)
	{
		if (selectable == null)
		{
			return;
		}
		if (!playTransition)
		{
			if (selectable.transition == Selectable.Transition.ColorTint)
			{
				ColorBlock colorBlock = selectable.colors;
				float prevFadeDuration = colorBlock.fadeDuration;
				colorBlock.fadeDuration = 0f;
				selectable.colors = colorBlock;
				selectable.interactable = state;
				colorBlock.fadeDuration = prevFadeDuration;
				selectable.colors = colorBlock;
			}
			else
			{
				selectable.interactable = state;
			}
		}
		else
		{
			selectable.interactable = state;
		}
	}
}
