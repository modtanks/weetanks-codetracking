using UnityEngine;

public class MapBorders : MonoBehaviour
{
	public void HideMapBorders()
	{
		foreach (Transform item in base.transform)
		{
			Collider component = item.GetComponent<Collider>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			foreach (Transform item2 in item)
			{
				MeshRenderer component2 = item2.GetComponent<MeshRenderer>();
				if ((bool)component2)
				{
					component2.enabled = false;
				}
			}
		}
	}

	public void ShowMapBorders()
	{
		foreach (Transform item in base.transform)
		{
			Collider component = item.GetComponent<Collider>();
			if ((bool)component)
			{
				component.enabled = true;
			}
			foreach (Transform item2 in item)
			{
				MeshRenderer component2 = item2.GetComponent<MeshRenderer>();
				if ((bool)component2)
				{
					component2.enabled = true;
				}
			}
		}
	}
}
