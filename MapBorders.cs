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
			MeshRenderer component2 = item.GetComponent<MeshRenderer>();
			if ((bool)component2)
			{
				component2.enabled = false;
			}
			foreach (Transform item2 in item)
			{
				MeshRenderer component3 = item2.GetComponent<MeshRenderer>();
				if ((bool)component3)
				{
					component3.enabled = false;
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
			MeshRenderer component2 = item.GetComponent<MeshRenderer>();
			if ((bool)component2)
			{
				component2.enabled = true;
			}
			foreach (Transform item2 in item)
			{
				MeshRenderer component3 = item2.GetComponent<MeshRenderer>();
				if ((bool)component3)
				{
					component3.enabled = true;
				}
			}
		}
	}
}
