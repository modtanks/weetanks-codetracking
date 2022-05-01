using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public bool isThirdPersonCam;

	public IEnumerator Shake(float duration, float magnitude)
	{
		Vector3 originalPos = (isThirdPersonCam ? base.transform.localPosition : base.transform.position);
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;
			float z = Random.Range(-1f, 1f) * magnitude;
			base.transform.localPosition += new Vector3(x, y, z);
			elapsed += Time.deltaTime;
			if ((bool)GameMaster.instance && GameMaster.instance.GameHasPaused)
			{
				break;
			}
			yield return null;
		}
		if (isThirdPersonCam)
		{
			base.transform.localPosition = originalPos;
		}
		else
		{
			base.transform.position = originalPos;
		}
	}
}
