using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRaycastUI : MonoBehaviour
{
	public bool PointIsOverUI(float x, float y)
	{
		Debug.LogError("mouse position is" + Input.mousePosition.ToString() + " and the input X is " + x + " with y of " + y);
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(x, y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		foreach (RaycastResult item in results)
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
