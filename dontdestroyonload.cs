using UnityEngine;

public class dontdestroyonload : MonoBehaviour
{
	private void Update()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
