using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePosition : MonoBehaviour
{
	public Vector3 mousePos;

	public Vector3 mouseInput;

	public LayerMask Mask1;

	public LayerMask Mask2;

	private CheckRaycastUI CheckCaster;

	private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();

	private void Start()
	{
		CheckCaster = Camera.main.GetComponent<CheckRaycastUI>();
	}

	private void Update()
	{
		if (Input.GetAxisRaw("Mouse X") > 0.5f || Input.GetAxisRaw("Mouse X") < -0.5f || Input.GetAxisRaw("Mouse Y") < -0.5f || Input.GetAxisRaw("Mouse Y") > 0.5f)
		{
			GameMaster.instance.isPlayingWithController = false;
		}
		if ((bool)OptionsMainMenu.instance && !OptionsMainMenu.instance.inAndroid)
		{
			MousePos(Input.mousePosition);
			mouseInput = Input.mousePosition;
		}
	}

	private void MousePos(Vector3 pos)
	{
		int num = 1 << LayerMask.NameToLayer("FLOOR");
		int num2 = 1 << LayerMask.NameToLayer("InvisibleFloor");
		int layerMask = num | num2;
		if (!GameMaster.instance.isPlayingWithController && Physics.Raycast(Camera.main.ScreenPointToRay(pos), out var hitInfo, 50000f, layerMask) && hitInfo.transform.tag == "Floor")
		{
			mousePos = hitInfo.point + new Vector3(0f, 0f, -1f);
		}
	}

	public bool PointIsOverUI(float x, float y)
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = new Vector2(x, y);
		tempRaycastResults.Clear();
		EventSystem.current.RaycastAll(pointerEventData, tempRaycastResults);
		foreach (RaycastResult tempRaycastResult in tempRaycastResults)
		{
			if (tempRaycastResult.gameObject.transform.tag == "IgnoreMobile")
			{
				Debug.LogError("mouseyPOS TANK SEES IGNORATION");
				return true;
			}
		}
		return tempRaycastResults.Count > 0;
	}
}
