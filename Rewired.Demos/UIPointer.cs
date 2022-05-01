using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos;

[AddComponentMenu("")]
[RequireComponent(typeof(RectTransform))]
public sealed class UIPointer : UIBehaviour
{
	[Tooltip("Should the hardware pointer be hidden?")]
	[SerializeField]
	private bool _hideHardwarePointer = true;

	[Tooltip("Sets the pointer to the last sibling in the parent hierarchy. Do not enable this on multiple UIPointers under the same parent transform or they will constantly fight each other for dominance.")]
	[SerializeField]
	private bool _autoSort = true;

	private Canvas _canvas;

	public bool autoSort
	{
		get
		{
			return _autoSort;
		}
		set
		{
			if (value != _autoSort)
			{
				_autoSort = value;
				if (value)
				{
					base.transform.SetAsLastSibling();
				}
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		Graphic[] graphics = GetComponentsInChildren<Graphic>();
		Graphic[] array = graphics;
		foreach (Graphic g in array)
		{
			g.raycastTarget = false;
		}
		if (_hideHardwarePointer)
		{
			Cursor.visible = false;
		}
		if (_autoSort)
		{
			base.transform.SetAsLastSibling();
		}
		GetDependencies();
	}

	private void Update()
	{
		if (_autoSort && base.transform.GetSiblingIndex() < base.transform.parent.childCount - 1)
		{
			base.transform.SetAsLastSibling();
		}
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		GetDependencies();
	}

	protected override void OnCanvasGroupChanged()
	{
		base.OnCanvasGroupChanged();
		GetDependencies();
	}

	public void OnScreenPositionChanged(Vector2 screenPosition)
	{
		if (!(_canvas == null))
		{
			Camera camera = null;
			RenderMode renderMode = _canvas.renderMode;
			RenderMode renderMode2 = renderMode;
			if (renderMode2 != 0 && (uint)(renderMode2 - 1) <= 1u)
			{
				camera = _canvas.worldCamera;
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform.parent as RectTransform, screenPosition, camera, out var point);
			base.transform.localPosition = new Vector3(point.x, point.y, base.transform.localPosition.z);
		}
	}

	private void GetDependencies()
	{
		_canvas = base.transform.root.GetComponentInChildren<Canvas>();
	}
}
