using UnityEngine;

public class MapBorders : MonoBehaviour
{
	public void HideMapBorders()
	{
		foreach (Transform child in base.transform)
		{
			Collider BorderCollider = child.GetComponent<Collider>();
			if ((bool)BorderCollider)
			{
				BorderCollider.enabled = false;
			}
			foreach (Transform child2 in child)
			{
				MeshRenderer MR = child2.GetComponent<MeshRenderer>();
				if ((bool)MR)
				{
					MR.enabled = false;
				}
			}
		}
	}

	public void ShowMapBorders()
	{
		foreach (Transform child in base.transform)
		{
			Collider BorderCollider = child.GetComponent<Collider>();
			if ((bool)BorderCollider)
			{
				BorderCollider.enabled = true;
			}
			foreach (Transform child2 in child)
			{
				MeshRenderer MR = child2.GetComponent<MeshRenderer>();
				if ((bool)MR)
				{
					MR.enabled = true;
				}
			}
		}
	}
}
