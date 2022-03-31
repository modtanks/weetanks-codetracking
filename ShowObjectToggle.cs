using UnityEngine;

public class ShowObjectToggle : MonoBehaviour
{
	public GameObject theObject;

	public bool Showing = false;

	public void ChangeShow()
	{
		if (Showing)
		{
			Showing = false;
			theObject.SetActive(value: false);
		}
		else
		{
			Showing = true;
			theObject.SetActive(value: true);
		}
	}
}
