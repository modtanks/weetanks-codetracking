using System;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

[RequireComponent(typeof(CanvasScalerExt))]
public class CanvasScalerFitter : MonoBehaviour
{
	[Serializable]
	private class BreakPoint
	{
		[SerializeField]
		public string name;

		[SerializeField]
		public float screenAspectRatio;

		[SerializeField]
		public Vector2 referenceResolution;
	}

	[SerializeField]
	private BreakPoint[] breakPoints;

	private CanvasScalerExt canvasScaler;

	private int screenWidth;

	private int screenHeight;

	private Action ScreenSizeChanged;

	private void OnEnable()
	{
		canvasScaler = GetComponent<CanvasScalerExt>();
		Update();
		canvasScaler.ForceRefresh();
	}

	private void Update()
	{
		if (Screen.width != screenWidth || Screen.height != screenHeight)
		{
			screenWidth = Screen.width;
			screenHeight = Screen.height;
			UpdateSize();
		}
	}

	private void UpdateSize()
	{
		if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize || breakPoints == null)
		{
			return;
		}
		float xRatio = (float)Screen.width / (float)Screen.height;
		float closest = float.PositiveInfinity;
		int closestIndex = 0;
		for (int i = 0; i < breakPoints.Length; i++)
		{
			float ratio = Mathf.Abs(xRatio - breakPoints[i].screenAspectRatio);
			if ((!(ratio > breakPoints[i].screenAspectRatio) || MathTools.IsNear(breakPoints[i].screenAspectRatio, 0.01f)) && ratio < closest)
			{
				closest = ratio;
				closestIndex = i;
			}
		}
		canvasScaler.referenceResolution = breakPoints[closestIndex].referenceResolution;
	}
}
