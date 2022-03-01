using UnityEngine;

public class RemoveOnDemo : MonoBehaviour
{
	private void Start()
	{
		if (OptionsMainMenu.instance.IsDemo)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
