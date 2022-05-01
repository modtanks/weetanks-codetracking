using UnityEngine;

public class CustomSetter : MonoBehaviour
{
	public int AMselectedID = 0;

	public GameObject[] objToDeactive;

	public GameObject[] objToActivate;

	private void Start()
	{
		if (!OptionsMainMenu.instance)
		{
			return;
		}
		if (OptionsMainMenu.instance.AMselected.Contains(AMselectedID))
		{
			GameObject[] array = objToActivate;
			foreach (GameObject obj3 in array)
			{
				obj3.SetActive(value: true);
			}
			GameObject[] array2 = objToDeactive;
			foreach (GameObject obj4 in array2)
			{
				obj4.SetActive(value: false);
			}
		}
		else
		{
			GameObject[] array3 = objToActivate;
			foreach (GameObject obj in array3)
			{
				obj.SetActive(value: false);
			}
			GameObject[] array4 = objToDeactive;
			foreach (GameObject obj2 in array4)
			{
				obj2.SetActive(value: true);
			}
		}
	}
}
