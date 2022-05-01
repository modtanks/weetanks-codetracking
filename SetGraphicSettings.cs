using UnityEngine;

public class SetGraphicSettings : MonoBehaviour
{
	private float lastRatio = 0f;

	private void Update()
	{
		float ratio = Screen.width / Screen.height;
		if (ratio != lastRatio && Application.platform == RuntimePlatform.Android)
		{
			Camera mycam = GetComponent<Camera>();
			lastRatio = ratio;
			if ((double)ratio > 1.8)
			{
				mycam.orthographicSize = 10.4f;
			}
		}
	}
}
