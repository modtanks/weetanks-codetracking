using System;
using System.Collections.Generic;
using Rewired.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper;

public static class UISelectionUtility
{
	private static Selectable[] s_reusableAllSelectables = new Selectable[0];

	public static Selectable FindNextSelectable(Selectable selectable, Transform transform, Vector3 direction)
	{
		RectTransform rectTransform = transform as RectTransform;
		if (rectTransform == null)
		{
			return null;
		}
		if (Selectable.allSelectableCount > s_reusableAllSelectables.Length)
		{
			s_reusableAllSelectables = new Selectable[Selectable.allSelectableCount];
		}
		int selectableCount = Selectable.AllSelectablesNoAlloc(s_reusableAllSelectables);
		IList<Selectable> allSelectables = s_reusableAllSelectables;
		direction.Normalize();
		Vector2 localDir = direction;
		Vector2 searchStartPos = UITools.GetPointOnRectEdge(rectTransform, localDir);
		bool isHoriz = localDir == Vector2.right * -1f || localDir == Vector2.right;
		float minCenterDistSqMag = float.PositiveInfinity;
		float minDirectLineSqMag = float.PositiveInfinity;
		Selectable bestCenterDistPick = null;
		Selectable bestDirectLinePick = null;
		Vector2 directLineCastEndPos = searchStartPos + localDir * 999999f;
		for (int i = 0; i < selectableCount; i++)
		{
			Selectable targetSelectable = allSelectables[i];
			if (targetSelectable == selectable || targetSelectable == null || targetSelectable.navigation.mode == Navigation.Mode.None || (!targetSelectable.IsInteractable() && !ReflectionTools.GetPrivateField<Selectable, bool>(targetSelectable, "m_GroupsAllowInteraction")))
			{
				continue;
			}
			RectTransform targetSelectableRectTransform = targetSelectable.transform as RectTransform;
			if (targetSelectableRectTransform == null)
			{
				continue;
			}
			Rect targetSelecableRect = UITools.InvertY(UITools.TransformRectTo(targetSelectableRectTransform, transform, targetSelectableRectTransform.rect));
			if (MathTools.LineIntersectsRect(searchStartPos, directLineCastEndPos, targetSelecableRect, out var directLineSqMag))
			{
				if (isHoriz)
				{
					directLineSqMag *= 0.25f;
				}
				if (directLineSqMag < minDirectLineSqMag)
				{
					minDirectLineSqMag = directLineSqMag;
					bestDirectLinePick = targetSelectable;
				}
			}
			Vector2 targetSelectableCenter = UnityTools.TransformPoint(targetSelectableRectTransform, transform, targetSelectableRectTransform.rect.center);
			Vector2 searchPosToTargetSelectableCenter = targetSelectableCenter - searchStartPos;
			float angle = Mathf.Abs(Vector2.Angle(localDir, searchPosToTargetSelectableCenter));
			if (!(angle > 75f))
			{
				float score = searchPosToTargetSelectableCenter.sqrMagnitude;
				if (score < minCenterDistSqMag)
				{
					minCenterDistSqMag = score;
					bestCenterDistPick = targetSelectable;
				}
			}
		}
		if (bestDirectLinePick != null && bestCenterDistPick != null)
		{
			if (minDirectLineSqMag > minCenterDistSqMag)
			{
				return bestCenterDistPick;
			}
			return bestDirectLinePick;
		}
		Array.Clear(s_reusableAllSelectables, 0, s_reusableAllSelectables.Length);
		return bestDirectLinePick ?? bestCenterDistPick;
	}
}
