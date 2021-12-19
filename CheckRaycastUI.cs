using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRaycastUI : MonoBehaviour
{
	public bool PointIsOverUI(float x, float y)
	{
		Debug.LogError("mouse position is" + Input.mousePosition.ToString() + " and the input X is " + x + " with y of " + y);
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = new Vector2(x, y);
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		foreach (RaycastResult item in list)
		{
			if (item.gameObject.transform.tag == "IgnoreMobile")
			{
				Debug.LogError("SEES IGNORATION");
				return true;
			}
		}
		return false;
	}
}
