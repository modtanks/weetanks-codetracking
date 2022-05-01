using Rewired.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[AddComponentMenu("")]
[RequireComponent(typeof(Selectable))]
public class ScrollRectSelectableChild : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public bool useCustomEdgePadding = false;

	public float customEdgePadding = 50f;

	private ScrollRect parentScrollRect;

	private Selectable _selectable;

	private RectTransform parentScrollRectContentTransform => parentScrollRect.content;

	private Selectable selectable => _selectable ?? (_selectable = GetComponent<Selectable>());

	private RectTransform rectTransform => base.transform as RectTransform;

	private void Start()
	{
		parentScrollRect = base.transform.GetComponentInParent<ScrollRect>();
		if (parentScrollRect == null)
		{
			Debug.LogError("Rewired Control Mapper: No ScrollRect found! This component must be a child of a ScrollRect!");
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (!(parentScrollRect == null) && eventData is AxisEventData)
		{
			RectTransform parentScrollRectTransform = parentScrollRect.transform as RectTransform;
			Rect relSelectableRect = MathTools.TransformRect(rectTransform.rect, rectTransform, parentScrollRectTransform);
			Rect viewRect = parentScrollRectTransform.rect;
			Rect paddedViewRect = parentScrollRectTransform.rect;
			float yPad = ((!useCustomEdgePadding) ? relSelectableRect.height : customEdgePadding);
			paddedViewRect.yMax -= yPad;
			paddedViewRect.yMin += yPad;
			if (!MathTools.RectContains(paddedViewRect, relSelectableRect) && MathTools.GetOffsetToContainRect(paddedViewRect, relSelectableRect, out var offset))
			{
				Vector2 newPos = parentScrollRectContentTransform.anchoredPosition;
				newPos.x = Mathf.Clamp(newPos.x + offset.x, 0f, Mathf.Abs(viewRect.width - parentScrollRectContentTransform.sizeDelta.x));
				newPos.y = Mathf.Clamp(newPos.y + offset.y, 0f, Mathf.Abs(viewRect.height - parentScrollRectContentTransform.sizeDelta.y));
				parentScrollRectContentTransform.anchoredPosition = newPos;
			}
		}
	}
}
