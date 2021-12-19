using UnityEngine;

public class MobileEnabler : MonoBehaviour
{
	private void Start()
	{
		if (!OptionsMainMenu.instance.inAndroid)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
