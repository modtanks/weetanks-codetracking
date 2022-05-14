using UnityEngine;

public class SetGraphicSettings : MonoBehaviour
{
	private float lastRatio;

	private void Update()
	{
		float num = Screen.width / Screen.height;
		if (num != lastRatio && Application.platform == RuntimePlatform.Android)
		{
			Camera component = GetComponent<Camera>();
			lastRatio = num;
			if ((double)num > 1.8)
			{
				component.orthographicSize = 10.4f;
			}
		}
	}
}
