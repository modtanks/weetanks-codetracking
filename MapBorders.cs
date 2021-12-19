using UnityEngine;

public class MapBorders : MonoBehaviour
{
	public void HideMapBorders()
	{
		foreach (Transform item in base.transform)
		{
			foreach (Transform item2 in item)
			{
				MeshRenderer component = item2.GetComponent<MeshRenderer>();
				if ((bool)component)
				{
					component.enabled = false;
				}
			}
		}
	}

	public void ShowMapBorders()
	{
		foreach (Transform item in base.transform)
		{
			foreach (Transform item2 in item)
			{
				MeshRenderer component = item2.GetComponent<MeshRenderer>();
				if ((bool)component)
				{
					component.enabled = true;
				}
			}
		}
	}
}
