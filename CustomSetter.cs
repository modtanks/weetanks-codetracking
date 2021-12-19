using UnityEngine;

public class CustomSetter : MonoBehaviour
{
	public int AMselectedID;

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
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			array = objToDeactive;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
		else
		{
			GameObject[] array = objToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = objToDeactive;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
		}
	}
}
